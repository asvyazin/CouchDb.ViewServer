namespace CouchDb.ViewServer.Host
{
	using System;
	using System.IO;
	using System.Reflection;

	using CouchDb.ViewServer.NLog;

	using global::NLog;
	using global::NLog.Config;
	using global::NLog.Targets;

	static class Program
	{
		static void Main()
		{
			var consoleWrapper = new ConsoleWrapper();
			var viewServerProtocol = new ViewServerProtocol(consoleWrapper);
			InitNLog(viewServerProtocol);
			var objectCreator = new SimpleObjectCreator();
			var viewServerCommandHandlers = new ViewServerCommandHandlers(objectCreator);
			var host = new ViewServerHost(viewServerProtocol, viewServerCommandHandlers);
			host.Run().Wait();
		}

		private static void InitNLog(IViewServerProtocol viewServerProtocol)
		{
			var config = new LoggingConfiguration();
			var couchDbTarget = new LogTarget(viewServerProtocol);
			config.AddTarget("couchdb", couchDbTarget);
			var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
			if (string.IsNullOrEmpty(currentDirectory))
				throw new InvalidOperationException("Could not get current directory");

			var logFileName = Path.Combine(currentDirectory, "viewserver.log");
			var fileTarget = new FileTarget { FileName = logFileName };
			config.AddTarget("file", fileTarget);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, couchDbTarget));
			LogManager.Configuration = config;
		}
	}
}
