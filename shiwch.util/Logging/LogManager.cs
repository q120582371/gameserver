using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace shiwch.util.Logging
{
    public static class LogManager
    {
        static LogManager()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(file));

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
                GetLogger("UnhandledException").Fatal("server Fatal.", ex);
            else
                GetLogger("UnhandledException").Fatal(e.ToString());
            log4net.LogManager.Shutdown();
        }

        /// <summary>
        /// 根据类名获取ILog
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILog GetLogger(Type type)
        {
            return new Log4NetWrapper(log4net.LogManager.GetLogger(type));
        }
        /// <summary>
        /// 根据名称获取ILog
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILog GetLogger(string name)
        {
            return new Log4NetWrapper(log4net.LogManager.GetLogger(name));
        }

        private static ILog debug = GetLogger("debug");
        private static ILog warn = GetLogger("warn");
        private static ILog info = GetLogger("info");
        private static ILog error = GetLogger("error");
        private static ILog fatal = GetLogger("fatal");

        #region [ ILog                         ]

        public static bool IsDebugEnabled
        {
            get { return debug.IsDebugEnabled; }
        }

        public static bool IsInfoEnabled
        {
            get { return info.IsInfoEnabled; }
        }

        public static bool IsWarnEnabled
        {
            get { return warn.IsWarnEnabled; }
        }

        public static bool IsErrorEnabled
        {
            get { return error.IsErrorEnabled; }
        }

        public static bool IsFatalEnabled
        {
            get { return fatal.IsFatalEnabled; }
        }

        public static void Debug(object message)
        {
            debug.Debug(message);
        }

        public static void Debug(object message, Exception exception)
        {
            debug.Debug(message, exception);
        }

        public static void DebugFormat(string format, object arg0)
        {
            debug.DebugFormat(format, arg0);
        }

        public static void DebugFormat(string format, object arg0, object arg1)
        {
            debug.DebugFormat(format, arg0, arg1);
        }

        public static void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            debug.DebugFormat(format, arg0, arg1, arg2);
        }

        public static void DebugFormat(string format, params object[] args)
        {
            debug.DebugFormat(format, args);
        }

        public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            debug.DebugFormat(provider, format, args);
        }

        public static void Info(object message)
        {
            info.Info(message);
        }

        public static void Info(object message, Exception exception)
        {
            info.Info(message, exception);
        }

        public static void InfoFormat(string format, object arg0)
        {
            info.InfoFormat(format, arg0);
        }

        public static void InfoFormat(string format, object arg0, object arg1)
        {
            info.InfoFormat(format, arg0, arg1);
        }

        public static void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            info.InfoFormat(format, arg0, arg1, arg2);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            info.InfoFormat(format, args);
        }

        public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            info.InfoFormat(provider, format, args);
        }

        public static void Warn(object message)
        {
            warn.Warn(message);
        }

        public static void Warn(object message, Exception exception)
        {
            warn.Warn(message, exception);
        }

        public static void WarnFormat(string format, object arg0)
        {
            warn.WarnFormat(format, arg0);
        }

        public static void WarnFormat(string format, object arg0, object arg1)
        {
            warn.WarnFormat(format, arg0, arg1);
        }

        public static void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            warn.WarnFormat(format, arg0, arg1, arg2);
        }

        public static void WarnFormat(string format, params object[] args)
        {
            warn.WarnFormat(format, args);
        }

        public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            warn.WarnFormat(provider, format, args);
        }

        public static void Error(object message)
        {
            error.Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            error.Error(message, exception);
        }

        public static void ErrorFormat(string format, object arg0)
        {
            error.ErrorFormat(format, arg0);
        }

        public static void ErrorFormat(string format, object arg0, object arg1)
        {
            error.ErrorFormat(format, arg0, arg1);
        }

        public static void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            error.ErrorFormat(format, arg0, arg1, arg2);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            error.ErrorFormat(format, args);
        }

        public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            error.ErrorFormat(provider, format, args);
        }

        public static void Fatal(object message)
        {
            fatal.Fatal(message);
        }

        public static void Fatal(object message, Exception exception)
        {
            fatal.Fatal(message, exception);
        }

        public static void FatalFormat(string format, object arg0)
        {
            fatal.FatalFormat(format, arg0);
        }

        public static void FatalFormat(string format, object arg0, object arg1)
        {
            fatal.FatalFormat(format, arg0, arg1);
        }

        public static void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            fatal.FatalFormat(format, arg0, arg1, arg2);
        }

        public static void FatalFormat(string format, params object[] args)
        {
            fatal.FatalFormat(format, args);
        }

        public static void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            fatal.FatalFormat(provider, format, args);
        }

        #endregion
    }
}
