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
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Aptivestigate.Serilog
{
    public class SerilogLogger : BaseLogger
    {
        private readonly LoggerConfiguration logConfig;
        private readonly Logger log;

        public override void Debug(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogEventLevel.Debug))
                log.Debug(message, args);
        }

        public override void Debug(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogEventLevel.Debug))
                log.Debug(ex, message, args);
        }

        public override void Error(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogEventLevel.Error))
                log.Error(message, args);
        }

        public override void Error(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogEventLevel.Error))
                log.Error(ex, message, args);
        }

        public override void Fatal(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogEventLevel.Fatal))
                log.Fatal(message, args);
        }

        public override void Fatal(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogEventLevel.Fatal))
                log.Fatal(ex, message, args);
        }

        public override void Info(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogEventLevel.Information))
                log.Information(message, args);
        }

        public override void Info(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogEventLevel.Information))
                log.Information(ex, message, args);
        }

        public override void Warning(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogEventLevel.Warning))
                log.Warning(message, args);
        }

        public override void Warning(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogEventLevel.Warning))
                log.Warning(ex, message, args);
        }

        public SerilogLogger(LoggerConfiguration? configurator = null)
        {
            logConfig = configurator ?? new LoggerConfiguration().WriteTo.Console();
            log = logConfig.CreateLogger();
        }
    }
}
