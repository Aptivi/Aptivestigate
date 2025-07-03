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
    /// <summary>
    /// Inherited logger class for NLog
    /// </summary>
    public class NLogLogger : BaseLogger
    {
        private readonly LoggingConfiguration logConfig;
        private readonly Logger log;

        /// <summary>
        /// Adds a message to the debug log
        /// </summary>
        /// <param name="message">Message to be added to the debug log</param>
        /// <param name="args">Arguments to format</param>
        public override void Debug(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Debug))
            {
                if (args is not null)
                    log.Debug(message, args);
                else
                    log.Debug(message);
            }
        }

        /// <summary>
        /// Adds a message to the debug log
        /// </summary>
        /// <param name="ex">Exception data</param>
        /// <param name="message">Message to be added to the debug log</param>
        /// <param name="args">Arguments to format</param>
        public override void Debug(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Debug))
            {
                if (args is not null)
                    log.Debug(ex, message, args);
                else
                    log.Debug(ex, message);
            }
        }

        /// <summary>
        /// Adds a message to the error log
        /// </summary>
        /// <param name="message">Message to be added to the error log</param>
        /// <param name="args">Arguments to format</param>
        public override void Error(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Error))
            {
                if (args is not null)
                    log.Error(message, args);
                else
                    log.Error(message);
            }
        }

        /// <summary>
        /// Adds a message to the error log
        /// </summary>
        /// <param name="ex">Exception data</param>
        /// <param name="message">Message to be added to the error log</param>
        /// <param name="args">Arguments to format</param>
        public override void Error(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Error))
            {
                if (args is not null)
                    log.Error(ex, message, args);
                else
                    log.Error(ex, message);
            }
        }

        /// <summary>
        /// Adds a message to the fatal log
        /// </summary>
        /// <param name="message">Message to be added to the fatal log</param>
        /// <param name="args">Arguments to format</param>
        public override void Fatal(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Fatal))
            {
                if (args is not null)
                    log.Fatal(message, args);
                else
                    log.Fatal(message);
            }
        }

        /// <summary>
        /// Adds a message to the fatal log
        /// </summary>
        /// <param name="ex">Exception data</param>
        /// <param name="message">Message to be added to the fatal log</param>
        /// <param name="args">Arguments to format</param>
        public override void Fatal(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Fatal))
            {
                if (args is not null)
                    log.Fatal(ex, message, args);
                else
                    log.Fatal(ex, message);
            }
        }

        /// <summary>
        /// Adds a message to the info log
        /// </summary>
        /// <param name="message">Message to be added to the info log</param>
        /// <param name="args">Arguments to format</param>
        public override void Info(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Info))
            {
                if (args is not null)
                    log.Info(message, args);
                else
                    log.Info(message);
            }
        }

        /// <summary>
        /// Adds a message to the info log
        /// </summary>
        /// <param name="ex">Exception data</param>
        /// <param name="message">Message to be added to the info log</param>
        /// <param name="args">Arguments to format</param>
        public override void Info(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Info))
            {
                if (args is not null)
                    log.Info(ex, message, args);
                else
                    log.Info(ex, message);
            }
        }

        /// <summary>
        /// Adds a message to the warning log
        /// </summary>
        /// <param name="message">Message to be added to the warning log</param>
        /// <param name="args">Arguments to format</param>
        public override void Warning(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Warn))
            {
                if (args is not null)
                    log.Warn(message, args);
                else
                    log.Warn(message);
            }
        }

        /// <summary>
        /// Adds a message to the warning log
        /// </summary>
        /// <param name="ex">Exception data</param>
        /// <param name="message">Message to be added to the warning log</param>
        /// <param name="args">Arguments to format</param>
        public override void Warning(Exception ex, string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Warn))
            {
                if (args is not null)
                    log.Warn(ex, message, args);
                else
                    log.Warn(ex, message);
            }
        }

        /// <summary>
        /// Makes a new logger instance
        /// </summary>
        /// <param name="name">Context to log with</param>
        /// <param name="configurator">Configuration instance that configures NLog</param>
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
