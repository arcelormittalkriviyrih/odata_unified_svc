using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.Log
{
	/// <summary>	A dynamic logger. </summary>
	public class DynamicLogger
	{
		#region Consts

		/// <summary>
		/// The name of the folder logging information is written to.
		/// </summary>
		private const string cLogFolderName = "Log";

		/// <summary>
		/// The extension of log files (with dot prefix).
		/// </summary>
		private const string cLogFileExtension = ".log";

		/// <summary>
		/// The layout pattern which is used when logging into a file.
		/// </summary>
		private const string cLogFileLayoutPattern = "%-5level %date{dd-MM-yyyy HH:mm:ss,fff} - %message%newline";

		/// <summary>
		/// The maximal size of a log file.
		/// </summary>
		private const string cMaxLogFileSize = "100KB";

		/// <summary>
		/// The maximum number of backup files that are kept before the oldest is erased.
		/// </summary>
		private const int cMaxSizeRollBackups = 2;

		#endregion

		#region Private Fields

		/// <summary>
		/// Lock for using logging system
		/// 
		/// It must be static to syncronize access
		/// from different web instances
		/// </summary>
		private static object m_LockLogObject = new object();

		#endregion

		#region Logging methods

		/// <summary>
		/// Path to log file from configuration
		/// </summary>
		private static string m_LogFilePath = ConfigurationManager.AppSettings["LogFilePath"];

		/// <summary>
		/// Returns path to log file for this web service
		/// </summary>
		private string GetPathToLogFile()
		{
			return m_LogFilePath;
		}

		/// <summary>
		/// Returns special lock object for
		/// access to logger.
		/// 
		/// By default used static object.
		/// </summary>
		private object LockLogObject
		{
			get { return m_LockLogObject; }
		}

		/// <summary>
		/// Logger writes log into the log-file
		/// </summary>
		/// <param name="text">String to write into the log</param>
		public string WriteLoggerLogInfo(string text)
		{
			string lvResult = text;

			lock (LockLogObject)
			{
				Logger.Info(text);
			}

			return lvResult;
		}

		/// <summary>
		/// Logger writes log into the log-file
		/// </summary>
		/// <param name="text">String to write into the log</param>
		public string WriteLoggerLogError(string text)
		{
			string lvResult = text;

			lock (LockLogObject)
			{
				Logger.Error(text);
			}

			return lvResult;
		}

		/// <summary>
		/// Logger writes log into the log-file
		/// </summary>
		/// <param name="text">String to write into the log</param>
		/// <param name="exception">Exception to log</param>
		public string WriteLoggerLogError(string text, Exception exception)
		{
			string lvResult;

			lock (LockLogObject)
			{
				Logger.Error(text, exception);
				lvResult = StackTraceException(exception);
			}

			return lvResult;
		}

		/// <summary>
		/// Get path to log file on file system and create all 
		/// directories in this path which not yet exists.
		/// </summary>
		private string GetLogFile()
		{
			// get the path to log file from config
			string lvFilePath = GetPathToLogFile();

			if (lvFilePath.Contains(Path.DirectorySeparatorChar.ToString()))
			{
				string lvDir = lvFilePath.Substring(0, lvFilePath.LastIndexOf(Path.DirectorySeparatorChar));
				if (!Directory.Exists(lvDir))
				{
					// try to create dir
					try
					{
						Directory.CreateDirectory(lvDir);
					}
					catch
					{
						throw new Exception(string.Format("Unable to create directory '{0}'", lvDir));
					}
				}
			}

			return lvFilePath;
		}

		/// <summary>
		/// String representation of given exception stack trace
		/// (with all inner exceptions)
		/// </summary>
		public string StackTraceException(Exception exception)
		{
			string lvStackTrace = "";

			if (exception != null)
			{
				lvStackTrace += "\n" + exception.Message + "\n" + exception.StackTrace;

				if (exception.InnerException != null)
				{
					lvStackTrace += StackTraceException(exception.InnerException);
				}
			}

			return lvStackTrace;
		}

		#endregion

		#region Properties

		#region Logger

		/// <summary>
		/// The value of the Logger property.
		/// </summary>
		private static ILog m_Logger = LogManager.GetLogger(typeof(DynamicLogger));

		/// <summary>
		/// Gets the object which is used to write logs.
		/// </summary>
		private static ILog Logger
		{
			get
			{
				return m_Logger;
			}
		}

		#endregion

		#endregion

		#region Singleton

		/// <summary>	The instance. </summary>
		private static DynamicLogger instance;

		/// <summary>	Gets the instance. </summary>
		///
		/// <value>	The instance. </value>

		public static DynamicLogger Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new DynamicLogger();
				}
				return instance;
			}
		}

		#region Constructors

		/// <summary>	Static constructor. </summary>
		static DynamicLogger()
		{
			ConfigureFileLogger(m_LogFilePath);
		}

		/// <summary>	Default constructor. </summary>
		public DynamicLogger()
		{
		}

		/// <summary>
		/// Configures the default file logger.
		/// </summary>
		public static void ConfigureFileLogger(string logFilePath)
		{
			#region Check arguments

			if (string.IsNullOrEmpty(logFilePath))
				throw new ArgumentNullException("logFilePath");

			#endregion

			RollingFileAppender lvFileAppender = new RollingFileAppender();

			lvFileAppender.File = logFilePath;

			lvFileAppender.Layout = new PatternLayout(cLogFileLayoutPattern);
			lvFileAppender.AppendToFile = true;
			lvFileAppender.MaximumFileSize = cMaxLogFileSize;
			lvFileAppender.MaxSizeRollBackups = cMaxSizeRollBackups;

			lvFileAppender.Threshold = Level.Debug;

			lvFileAppender.ActivateOptions();

			BasicConfigurator.Configure(lvFileAppender);
		}

		#endregion

		#endregion
	}
}