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

using System;

namespace Aptivestigate.Logging
{
    /// <summary>
    /// Abstract base logger class
    /// </summary>
    public abstract class BaseLogger
    {
        /// <summary>
        /// Adds a message to the debug log
        /// </summary>
        /// <param name="message">Message to be added to the debug log</param>
        /// <param name="args">Arguments to format</param>
        public abstract void Debug(string message, params object?[]? args);
        
        /// <summary>
        /// Adds a message to the debug log
        /// </summary>
        /// <param name="ex">Exception data</param>
        /// <param name="message">Message to be added to the debug log</param>
        /// <param name="args">Arguments to format</param>
        public abstract void Debug(Exception ex, string message, params object?[]? args);

        /// <summary>
        /// Adds a message to the info log
        /// </summary>
        /// <param name="message">Message to be added to the info log</param>
        /// <param name="args">Arguments to format</param>
        public abstract void Info(string message, params object?[]? args);

        /// <summary>
        /// Adds a message to the info log
        /// </summary>
        /// <param name="ex">Exception data</param>
        /// <param name="message">Message to be added to the info log</param>
        /// <param name="args">Arguments to format</param>
        public abstract void Info(Exception ex, string message, params object?[]? args);

        /// <summary>
        /// Adds a message to the warning log
        /// </summary>
        /// <param name="message">Message to be added to the warning log</param>
        /// <param name="args">Arguments to format</param>
        public abstract void Warning(string message, params object?[]? args);

        /// <summary>
        /// Adds a message to the warning log
        /// </summary>
        /// <param name="ex">Exception data</param>
        /// <param name="message">Message to be added to the warning log</param>
        /// <param name="args">Arguments to format</param>
        public abstract void Warning(Exception ex, string message, params object?[]? args);

        /// <summary>
        /// Adds a message to the error log
        /// </summary>
        /// <param name="message">Message to be added to the error log</param>
        /// <param name="args">Arguments to format</param>
        public abstract void Error(string message, params object?[]? args);

        /// <summary>
        /// Adds a message to the error log
        /// </summary>
        /// <param name="ex">Exception data</param>
        /// <param name="message">Message to be added to the error log</param>
        /// <param name="args">Arguments to format</param>
        public abstract void Error(Exception ex, string message, params object?[]? args);

        /// <summary>
        /// Adds a message to the fatal log
        /// </summary>
        /// <param name="message">Message to be added to the fatal log</param>
        /// <param name="args">Arguments to format</param>
        public abstract void Fatal(string message, params object?[]? args);

        /// <summary>
        /// Adds a message to the fatal log
        /// </summary>
        /// <param name="ex">Exception data</param>
        /// <param name="message">Message to be added to the fatal log</param>
        /// <param name="args">Arguments to format</param>
        public abstract void Fatal(Exception ex, string message, params object?[]? args);
    }
}
