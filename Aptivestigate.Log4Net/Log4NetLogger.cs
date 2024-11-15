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
using log4net;
using log4net.Config;
using System;

namespace Aptivestigate.Log4Net
{
    public class Log4NetLogger : BaseLogger
    {
        private readonly ILog log;

        public override void Debug(string message, params object?[]? args)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat(message, args);
        }

        public override void Debug(Exception ex, string message, params object?[]? args)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat(message + $"\n\n{ex}", args);
        }

        public override void Error(string message, params object?[]? args)
        {
            if (log.IsErrorEnabled)
                log.ErrorFormat(message, args);
        }

        public override void Error(Exception ex, string message, params object?[]? args)
        {
            if (log.IsErrorEnabled)
                log.ErrorFormat(message + $"\n\n{ex}", args);
        }

        public override void Fatal(string message, params object?[]? args)
        {
            if (log.IsFatalEnabled)
                log.FatalFormat(message, args);
        }

        public override void Fatal(Exception ex, string message, params object?[]? args)
        {
            if (log.IsFatalEnabled)
                log.FatalFormat(message + $"\n\n{ex}", args);
        }

        public override void Info(string message, params object?[]? args)
        {
            if (log.IsInfoEnabled)
                log.InfoFormat(message, args);
        }

        public override void Info(Exception ex, string message, params object?[]? args)
        {
            if (log.IsInfoEnabled)
                log.InfoFormat(message + $"\n\n{ex}", args);
        }

        public override void Warning(string message, params object?[]? args)
        {
            if (log.IsWarnEnabled)
                log.WarnFormat(message, args);
        }

        public override void Warning(Exception ex, string message, params object?[]? args)
        {
            if (log.IsWarnEnabled)
                log.WarnFormat(message + $"\n\n{ex}", args);
        }

        public Log4NetLogger(string context, Action? configurator = null)
        {
            log = LogManager.GetLogger(context);
            if (configurator is not null)
                configurator.Invoke();
            else
                BasicConfigurator.Configure();
        }
    }
}
