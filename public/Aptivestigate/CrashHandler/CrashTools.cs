//
// Aptivestigate  Copyright (C) 2024-2025  Aptivi
//
// This file is part of Aptivestigate
//
// Aptivestigate is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Aptivestigate is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using Aptivestigate.Languages;
using Aptivestigate.Logging;
using Aptivestigate.Paths;
using SpecProbe.Software.Platform;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Aptivestigate.CrashHandler
{
    /// <summary>
    /// Application crash handling tools
    /// </summary>
    public static class CrashTools
    {
        private static bool crashHandling = false;
        private static Action<UnhandledExceptionEventArgs>? crashHandler = null;

        /// <summary>
        /// Whether the crash handler is installed or not
        /// </summary>
        public static bool CrashHandling =>
            crashHandling;
        
        /// <summary>
        /// Installs the application crash handler
        /// </summary>
        /// <param name="crashHandler">Crash handler to use</param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void InstallCrashHandler(Action<UnhandledExceptionEventArgs>? crashHandler = null)
        {
            // Check to see if we already have a handler installed
            if (CrashHandling)
                throw new InvalidOperationException(LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_EXCEPTION_ALREADYINSTALLED"));

            AppDomain.CurrentDomain.UnhandledException += HandleCrash;
            CrashTools.crashHandler = crashHandler;
            crashHandling = true;
        }

        private static void HandleCrash(object sender, UnhandledExceptionEventArgs e)
        {
            bool isDefault = crashHandler is null;
            var finalDelegate = crashHandler ?? new Action<UnhandledExceptionEventArgs>(DefaultCrashHandler);
            try
            {
                // Run the crash handler (first chance)
                finalDelegate.Invoke(e);
            }
            catch (Exception ex)
            {
                try
                {
                    // Looks like that the crash handler has crashed, so check the type and re-run if necessary
                    // with the default crash handler
                    if (!isDefault)
                    {
                        finalDelegate = new Action<UnhandledExceptionEventArgs>(DefaultCrashHandler);
                        HandleCrash(sender, e);
                    }
                    else
                        throw ex;
                }
                catch (Exception ex2)
                {
                    // Looks like that we've crashed again (second fault). As a last chance, try to make a crash
                    // file representing a fatal crash and dump only the necessary details.
                    try
                    {
                        FatalCrashHandler(ex2);
                    }
                    catch (Exception ex3)
                    {
                        // We're totally screwed! Bail out!
                        Console.WriteLine("---- " + LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_FATALCRASH") + " ----");
                        Console.WriteLine();
                        Console.WriteLine(LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_FATALCRASH_DESC"));
                        Environment.FailFast(
                            $"""
                            
                            {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_FATALCRASH_EVENT_TRIPLEFAULT")}
                            ===================================

                            {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_FATALCRASH_EVENT_FIRSTCHANCE")}
                            ------------
                            {ex}

                            {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_FATALCRASH_EVENT_SECONDCHANCE")}
                            -------------
                            {ex2}
                            """, ex3);
                    }
                }
            }
        }

        private static void DefaultCrashHandler(UnhandledExceptionEventArgs eventArgs)
        {
            Console.WriteLine("---- " + LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASH") + " ----");
            Console.WriteLine();
            Console.WriteLine(LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASH_DESC"));
            
            // Write the crash dump
            using var crashFileWriter = CreateCrashFile(out Guid crashId);
            bool isException = eventArgs.ExceptionObject is Exception;
            crashFileWriter.WriteLine(
                $"""
                ==========> {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_HEADER")}

                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_DESC")}

                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_CRASHID"),-25} {crashId}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_CRASHTIME"),-25} {DateTimeOffset.Now}
                
                -----> {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCINFO_HEADER")}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCINFO_TERMINATING"),-25} {eventArgs.IsTerminating}

                {(isException ? eventArgs.ExceptionObject : LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCINFO_GENERALFAULT"))}
                
                -----> {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_HEADER")}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_DESC")}

                {AnalyzeException(eventArgs.ExceptionObject as Exception)}
                
                -----> {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_RUNTIMEINFO_HEADER")}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_RUNTIMEINFO_OS"),-25} {PlatformHelper.GetPlatform()}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_RUNTIMEINFO_OSDESC"),-25} {RuntimeInformation.OSDescription}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_RUNTIMEINFO_OSARCH"),-25} {RuntimeInformation.OSArchitecture}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_RUNTIMEINFO_PROCARCH"),-25} {RuntimeInformation.ProcessArchitecture}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_RUNTIMEINFO_DOTNETRUNTIME"),-25} {RuntimeInformation.FrameworkDescription}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_RUNTIMEINFO_DOTNETFX"),-25} {PlatformHelper.IsDotNetFx()}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_RUNTIMEINFO_GENERICRID"),-25} {PlatformHelper.GetCurrentGenericRid()}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_RUNTIMEINFO_GENERALRID"),-25} {PlatformHelper.GetCurrentGenericRid(false)}
                
                -----> {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_HEADER")}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_RUNNINGFROMNITROCID"),-25} {PlatformHelper.IsRunningFromNitrocid()}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_RUNNINGFROMMONO"),-25} {PlatformHelper.IsRunningFromMono()}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_RUNNINGFROMSCREEN"),-25} {PlatformHelper.IsRunningFromScreen()}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_RUNNINGFROMTMUX"),-25} {PlatformHelper.IsRunningFromTmux()}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_RUNNINGONWSL"),-25} {PlatformHelper.IsOnUnixWsl()}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_RUNNINGONMUSL"),-25} {PlatformHelper.IsOnUnixMusl()}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_RUNNINGONANDROID"),-25} {PlatformHelper.IsOnAndroid()}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_RUNNINGONGUI"),-25} {PlatformHelper.IsOnGui()}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_RUNNINGONXORG"),-25} {PlatformHelper.IsOnX11()}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_RUNNINGONWAYLAND"),-25} {PlatformHelper.IsOnWayland()}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_TERMINALTYPE"),-25} {PlatformHelper.GetTerminalType()}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_OPERINFO_TERMINALEMU"),-25} {PlatformHelper.GetTerminalEmulator()}

                ==========> {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_HEADER")}
                """
            );
            crashFileWriter.Close();
        }

        private static void FatalCrashHandler(Exception exception)
        {
            Console.WriteLine("---- " + LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_SECONDCRASH") + " ----");
            Console.WriteLine();
            Console.WriteLine(LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_SECONDCRASH_DESC"));

            // Write the crash dump
            using var crashFileWriter = CreateCrashFile(out Guid crashId, true);
            crashFileWriter.WriteLine(
                $"""
                ==========> {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_SECONDCRASHDUMP_HEADER")}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_SECONDCRASHDUMP_DESC")}
                
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_CRASHID"),-25} {crashId}
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_CRASHTIME"),-25} {DateTimeOffset.Now}
                
                -----> {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCINFO_HEADER")}

                {exception}
                
                ==========> {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_SECONDCRASHDUMP_HEADER")}
                """
            );
            crashFileWriter.Close();
        }

        private static string AnalyzeException(Exception? exception)
        {
            // Check the exception
            if (exception is null)
                return LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_NOEXCEPTION");

            // Now, analyze the exception
            var analysisBuilder = new StringBuilder();
            var stackTrace = new StackTrace(exception, true);
            var traceFrames = stackTrace.GetFrames();
            analysisBuilder.AppendLine(
                $"""
                {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAMECOUNT"),-25} {stackTrace.FrameCount}

                """
            );
            for (int i = 0; i < traceFrames.Length; i++)
            {
                StackFrame? traceFrame = traceFrames[i];
                analysisBuilder.AppendLine(
                    $"""
                    ..............................................

                    {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_NUM"),-25} {i + 1}/{stackTrace.FrameCount}

                    """
                );
                if (traceFrame.HasMethod())
                {
                    var method = traceFrame.GetMethod();
                    var parameters = method.GetParameters();
                    analysisBuilder.AppendLine(
                        $"""
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHOD"),-25} {method.Name}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_STATICMETHOD"),-25} {method.IsStatic}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODTYPE"),-25} {method.MemberType}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODDECLARINGTYPE"),-25} {method.DeclaringType}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODREFLECTEDTYPE"),-25} {method.ReflectedType}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODATTRIBUTES"),-25} {method.Attributes}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODFLAGS"),-25} {method.MethodImplementationFlags}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODADDRESS"),-25} 0x{method.MethodHandle.Value.ToInt64():X2}
                        """
                    );
                    foreach (var parameter in parameters)
                    {
                        analysisBuilder.AppendLine(
                            $"""
                            {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODPARAM_TYPENAME"),-25} {parameter.ParameterType.Name}
                            {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODPARAM_FULLTYPE"),-25} {parameter.ParameterType.FullName}
                            {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODPARAM_NAME"),-25} {parameter.Name}
                            {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODPARAM_POS"),-25} {parameter.Position}
                            {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODPARAM_HASDEFAULT"),-25} {parameter.HasDefaultValue}
                            {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODPARAM_DEFAULTVALUE"),-25} {parameter.DefaultValue}
                            """
                        );
                    }
                }
                if (traceFrame.HasILOffset())
                {
                    analysisBuilder.AppendLine(
                        $"""
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODILOFFSET"),-25} 0x{traceFrame.GetILOffset():X2}
                        """
                    );
                }
                if (traceFrame.HasNativeImage())
                {
                    analysisBuilder.AppendLine(
                        $"""
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODNATIVEIMAGEBASE"),-25} 0x{traceFrame.GetNativeImageBase().ToInt64():X2}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODNATIVEIMAGEIP"),-25} 0x{traceFrame.GetNativeIP().ToInt64():X2}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODNATIVEIMAGEOFFSET"),-25} 0x{traceFrame.GetNativeOffset():X2}
                        """
                    );
                }
                if (traceFrame.HasSource())
                {
                    analysisBuilder.AppendLine(
                        $"""
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODSOURCENAME"),-25} {traceFrame.GetFileName()}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODSOURCELINE"),-25} {traceFrame.GetFileLineNumber()}
                        {LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCANALYSIS_FRAME_METHODSOURCECOLUMN"),-25} {traceFrame.GetFileColumnNumber()}
                        """
                    );
                }
            }
            return analysisBuilder.ToString();
        }

        private static StreamWriter CreateCrashFile(out Guid crashId, bool fatal = false)
        {
            lock (LogTools.genLock)
            {
                string dumpFilePath = LogPathTools.GetPath(LogPath.Crashes);
                string assembly = Assembly.GetEntryAssembly().GetName().Name;
                crashId = Guid.NewGuid();
                Directory.CreateDirectory(dumpFilePath);
                return File.CreateText(Path.Combine(dumpFilePath, $"{(fatal ? "f_" : "")}crash_{assembly}_{DateTimeOffset.Now:yyyyMMddHHmmssfffffff}_{crashId}.txt"));
            }
        }
    }
}
