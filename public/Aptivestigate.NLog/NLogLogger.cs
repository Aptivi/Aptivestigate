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

using Aptivestigate.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;

namespace Aptivestigate.NLog
{
    public class NLogLogger : BaseLogger
    {
        private readonly LoggingConfiguration logConfig;
        private readonly Logger log;

        public override void Debug(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Debug))
                log.Debug(message, args);
        }

        public override void Debug(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Debug))
                log.Debug(ex, message, args);
        }

        public override void Error(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Error))
                log.Error(message, args);
        }

        public override void Error(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Error))
                log.Error(ex, message, args);
        }

        public override void Fatal(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Fatal))
                log.Fatal(message, args);
        }

        public override void Fatal(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Fatal))
                log.Fatal(ex, message, args);
        }

        public override void Info(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Info))
                log.Info(message, args);
        }

        public override void Info(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Info))
                log.Info(ex, message, args);
        }

        public override void Warning(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Warn))
                log.Warn(message, args);
        }

        public override void Warning(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Warn))
                log.Warn(ex, message, args);
        }

        public NLogLogger(string name, LoggingConfiguration? configurator = null)
        {
            logConfig = configurator ?? new LoggingConfiguration();
            if (configurator is null)
            {
                logConfig.AddTarget("Aptivestigate", new ColoredConsoleTarget());
                logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, "Aptivestigate");
            }
            LogManager.Configuration = logConfig;
            log = LogManager.GetLogger(name);
        }
    }
}
