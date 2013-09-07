namespace CouchDb.ViewServer.Host
{
	static class Program
	{
		static void Main()
		{
			var viewServerProtocol = new ViewServerProtocol();
			var objectCreator = new SimpleObjectCreator();
			var viewServerCommandHandlers = new ViewServerCommandHandlers(objectCreator);
			var host = new ViewServerHost(viewServerProtocol, viewServerCommandHandlers);
			host.Run().Wait();
		}
	}
}
