namespace CouchDb.ViewServer.Host
{
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

			var fileTarget = new FileTarget { FileName = "${basedir}/viewserver.log" };
			config.AddTarget("file", fileTarget);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, couchDbTarget));
			LogManager.Configuration = config;
		}
	}
}
