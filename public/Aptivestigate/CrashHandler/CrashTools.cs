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
                        Console.WriteLine("---- FATAL CRASH DETECTED ----");
                        Console.WriteLine();
                        Console.WriteLine(LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_FATALCRASH_DESC"));
                        Environment.FailFast(
                            $"""
                            
                            FATAL CRASH DETECTED (TRIPLE FAULT)
                            ===================================

                            First chance
                            ------------
                            {ex}

                            Second chance
                            -------------
                            {ex2}
                            """, ex3);
                    }
                }
            }
        }

        private static void DefaultCrashHandler(UnhandledExceptionEventArgs eventArgs)
        {
            Console.WriteLine("---- CRASH DETECTED ----");
            Console.WriteLine();
            Console.WriteLine(LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASH_DESC"));
            
            // Write the crash dump
            using var crashFileWriter = CreateCrashFile(out Guid crashId);
            bool isException = eventArgs.ExceptionObject is Exception;
            crashFileWriter.WriteLine(
                $"""
                =============================== Crash report ===============================

                Below is the crash report about what happened in the application while it
                was performing your requested operation.

                Crash ID:             {crashId}
                Time of incident:     {DateTimeOffset.Now}
                
                --------------------------- Exception Information --------------------------

                Terminating:          {eventArgs.IsTerminating}

                {(isException ? eventArgs.ExceptionObject : LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_CRASHDUMP_EXCINFO_GENERALFAULT"))}
                
                ---------------------------- Exception Analysis ----------------------------

                Below may help you find out why:

                {AnalyzeException(eventArgs.ExceptionObject as Exception)}

                ---------------------------- Runtime Information ---------------------------

                Operating system:     {PlatformHelper.GetPlatform()}
                OS description:       {RuntimeInformation.OSDescription}
                OS architecture:      {RuntimeInformation.OSArchitecture}
                Process architecture: {RuntimeInformation.ProcessArchitecture}

                .NET runtime:         {RuntimeInformation.FrameworkDescription}
                Running on dotnetfx:  {PlatformHelper.IsDotNetFx()}

                Generic runtime ID:   {PlatformHelper.GetCurrentGenericRid()}
                General runtime ID:   {PlatformHelper.GetCurrentGenericRid(false)}
                
                --------------------------- Operation Information --------------------------

                Running from N-KS:    {PlatformHelper.IsRunningFromNitrocid()}
                Running from Mono:    {PlatformHelper.IsRunningFromMono()}
                Running from Screen:  {PlatformHelper.IsRunningFromScreen()}
                Running from TMUX:    {PlatformHelper.IsRunningFromTmux()}

                Running on WSL:       {PlatformHelper.IsOnUnixWsl()}
                Running on MUSL:      {PlatformHelper.IsOnUnixMusl()}
                Running on Android:   {PlatformHelper.IsOnAndroid()}

                Running on GUI:       {PlatformHelper.IsOnGui()}
                Running on X.Org:     {PlatformHelper.IsOnX11()}
                Running on Wayland:   {PlatformHelper.IsOnWayland()}

                Terminal type:        {PlatformHelper.GetTerminalType()}
                Terminal emulator:    {PlatformHelper.GetTerminalEmulator()}

                =============================== Crash report ===============================
                """
            );
            crashFileWriter.Close();
        }

        private static void FatalCrashHandler(Exception exception)
        {
            Console.WriteLine("---- SECOND CRASH DETECTED ----");
            Console.WriteLine();
            Console.WriteLine(LanguageTools.GetLocalized("APTIVESTIGATE_CRASHHANDLER_SECONDCRASH_DESC"));

            // Write the crash dump
            using var crashFileWriter = CreateCrashFile(out Guid crashId, true);
            crashFileWriter.WriteLine(
                $"""
                ============================ Fatal crash report ============================

                Below is the fatal crash report about what happened in the application while
                it was handling the error.

                Crash ID:             {crashId}
                Time of incident:     {DateTimeOffset.Now}
                
                --------------------------- Exception Information --------------------------

                {exception}
                
                ============================ Fatal crash report ============================
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
                Frame count:          {stackTrace.FrameCount}

                """
            );
            for (int i = 0; i < traceFrames.Length; i++)
            {
                StackFrame? traceFrame = traceFrames[i];
                analysisBuilder.AppendLine(
                    $"""
                    ..............................................

                    Frame number:         {i + 1}/{stackTrace.FrameCount}

                    """
                );
                if (traceFrame.HasMethod())
                {
                    var method = traceFrame.GetMethod();
                    var parameters = method.GetParameters();
                    analysisBuilder.AppendLine(
                        $"""
                        Method:               {method.Name}
                        Static method:        {method.IsStatic}
                        Method type:          {method.MemberType}
                        Declaring type:       {method.DeclaringType}
                        Reflected type:       {method.ReflectedType}
                        Attributes:           {method.Attributes}
                        Implementation flags: {method.MethodImplementationFlags}
                        Method address:       0x{method.MethodHandle.Value.ToInt64():X2}
                        """
                    );
                    foreach (var parameter in parameters)
                    {
                        analysisBuilder.AppendLine(
                            $"""
                            Parameter type name:  {parameter.ParameterType.Name}
                            Parameter full type:  {parameter.ParameterType.FullName}
                            Parameter name:       {parameter.Name}
                            Position (from 0):    {parameter.Position}
                            Contains default:     {parameter.HasDefaultValue}
                            Default value:        {parameter.DefaultValue}
                            """
                        );
                    }
                }
                if (traceFrame.HasILOffset())
                {
                    analysisBuilder.AppendLine(
                        $"""
                        IL offset:            0x{traceFrame.GetILOffset():X2}
                        """
                    );
                }
                if (traceFrame.HasNativeImage())
                {
                    analysisBuilder.AppendLine(
                        $"""
                        Native image base:    0x{traceFrame.GetNativeImageBase().ToInt64():X2}
                        Native image IP:      0x{traceFrame.GetNativeIP().ToInt64():X2}
                        Native image offset:  0x{traceFrame.GetNativeOffset():X2}
                        """
                    );
                }
                if (traceFrame.HasSource())
                {
                    analysisBuilder.AppendLine(
                        $"""
                        File name:            {traceFrame.GetFileName()}
                        File line number:     {traceFrame.GetFileLineNumber()}
                        File column number:   {traceFrame.GetFileColumnNumber()}
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
