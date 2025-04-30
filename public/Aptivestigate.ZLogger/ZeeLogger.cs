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
using Microsoft.Extensions.Logging;
using System;
using ZLogger;

namespace Aptivestigate.ZLogger
{
    /// <summary>
    /// Inherited logger class for ZLogger
    /// </summary>
    public class ZeeLogger : BaseLogger
    {
        private readonly ILoggerFactory logConfig;
        private readonly ILogger log;

        /// <summary>
        /// Adds a message to the debug log
        /// </summary>
        /// <param name="message">Message to be added to the debug log</param>
        /// <param name="args">Arguments to format</param>
        public override void Debug(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Debug))
            {
                if (args is null)
                    log.LogDebug(message);
                else
                    log.LogDebug(message, args);
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
                if (args is null)
                    log.LogDebug(ex, message);
                else
                    log.LogDebug(ex, message, args);
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
                if (args is null)
                    log.LogError(message);
                else
                    log.LogError(message, args);
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
                if (args is null)
                    log.LogError(ex, message);
                else
                    log.LogError(ex, message, args);
            }
        }

        /// <summary>
        /// Adds a message to the fatal log
        /// </summary>
        /// <param name="message">Message to be added to the fatal log</param>
        /// <param name="args">Arguments to format</param>
        public override void Fatal(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Critical))
            {
                if (args is null)
                    log.LogCritical(message);
                else
                    log.LogCritical(message, args);
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
            if (log.IsEnabled(LogLevel.Critical))
            {
                if (args is null)
                    log.LogCritical(ex, message);
                else
                    log.LogCritical(ex, message, args);
            }
        }

        /// <summary>
        /// Adds a message to the info log
        /// </summary>
        /// <param name="message">Message to be added to the info log</param>
        /// <param name="args">Arguments to format</param>
        public override void Info(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Information))
            {
                if (args is null)
                    log.LogInformation(message);
                else
                    log.LogInformation(message, args);
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
            if (log.IsEnabled(LogLevel.Information))
            {
                if (args is null)
                    log.LogInformation(ex, message);
                else
                    log.LogInformation(ex, message, args);
            }
        }

        /// <summary>
        /// Adds a message to the warning log
        /// </summary>
        /// <param name="message">Message to be added to the warning log</param>
        /// <param name="args">Arguments to format</param>
        public override void Warning(string message, params object?[]? args)
        {
            if (log.IsEnabled(LogLevel.Warning))
            {
                if (args is null)
                    log.LogWarning(message);
                else
                    log.LogWarning(message, args);
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
            if (log.IsEnabled(LogLevel.Warning))
            {
                if (args is null)
                    log.LogWarning(ex, message);
                else
                    log.LogWarning(ex, message, args);
            }
        }

        /// <summary>
        /// Makes a new logger instance
        /// </summary>
        /// <param name="name">Context to log with</param>
        /// <param name="configurator">Configuration instance that configures NLog</param>
        public ZeeLogger(string name, ILoggerFactory? configurator = null)
        {
            logConfig = configurator ?? LoggerFactory.Create(builder => builder.AddZLoggerConsole());
            log = logConfig.CreateLogger(name);
        }
    }
}
