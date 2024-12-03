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

using Aptivestigate.CrashHandler;
using Aptivestigate.Log4Net;
using Aptivestigate.Logging;
using Aptivestigate.NLog;
using Aptivestigate.Serilog;
using System;
using System.Linq;
using Terminaux.Colors.Data;
using Terminaux.Writer.ConsoleWriters;

namespace Aptivestigate.Demo
{
    internal class Program
    {
        static int Main(string[] args)
        {
            bool isLogForNet = args.Contains("-log4net");
            bool isNLog = args.Contains("-nlog");
            bool isSerilog = args.Contains("-serilog");
            CrashTools.InstallCrashHandler();

            // We need to choose the adapter according to the command line arguments
            BaseLogger logger;
            if (isLogForNet)
                logger = new Log4NetLogger("Demonstration");
            else if (isNLog)
                logger = new NLogLogger("Demonstration");
            else if (isSerilog)
                logger = new SerilogLogger();
            else
            {
                TextWriterColor.WriteColor("You need to select one of -log4net, -nlog, or -serilog flags to test.", ConsoleColors.Red);
                return 1;
            }

            // Now, test logging.
            var exc = new Exception("We really can't do this.");
            LogTools.Debug(logger, "Test message without formatting");
            LogTools.Debug(logger, "Test message with formatting: {0}, {1}", "Hello", "John Smith");
            LogTools.Info(logger, "This is an informational message!");
            LogTools.Info(logger, "Saying: {0}, {1}", "Hello", "John Smith");
            LogTools.Warning(logger, "Warning: this may not work properly.");
            LogTools.Warning(logger, "Warning: a component, {0}, may not work properly.", "fusion reactor");
            LogTools.Error(logger, "Error: Ship is out of fuel!");
            LogTools.Error(logger, "Error: Fuel level is empty! {0}/{1}", 0, 320);
            LogTools.Error(logger, exc, "Error.");
            LogTools.Fatal(logger, "FATAL ERROR!");
            LogTools.Fatal(logger, "FATAL ERROR: We can't do this! {0}", "Invalid operation.");
            LogTools.Fatal(logger, exc, "FATAL ERROR!");
            throw exc;
        }
    }
}
