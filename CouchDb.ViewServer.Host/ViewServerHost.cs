namespace CouchDb.ViewServer.Host
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public class ViewServerHost
	{
		private readonly IViewServerProtocol viewServerProtocol;

		private readonly IViewServerCommandHandlers viewServerCommandHandlers;

		private readonly Dictionary<string, Func<dynamic[], dynamic>> commandHandlers = new Dictionary<string, Func<dynamic[], dynamic>>();

		public ViewServerHost(IViewServerProtocol viewServerProtocol, IViewServerCommandHandlers viewServerCommandHandlers)
		{
			this.viewServerProtocol = viewServerProtocol;
			this.viewServerCommandHandlers = viewServerCommandHandlers;
			this.RegisterCommandHandler("reset", args => "true");
			this.RegisterCommandHandler("add_fun", this.AddFunHandler);
			this.RegisterCommandHandler("map_doc", this.MapDocHandler);
			this.RegisterCommandHandler("reduce", this.ReduceHandler);
			this.RegisterCommandHandler("rereduce", this.RereduceHandler);
		}

		private dynamic AddFunHandler(dynamic[] args)
		{
			if (args.Length != 1)
				throw new ArgumentException("add_fun must have only one parameter");

			return viewServerCommandHandlers.AddFun(args[0]);
		}

		private dynamic MapDocHandler(dynamic[] args)
		{
			if (args.Length != 1)
				throw new ArgumentException("map_doc must have only one parameter");

			return viewServerCommandHandlers.MapDoc(args[0]);
		}

		private dynamic ReduceHandler(dynamic[] args)
		{
			if (args.Length != 2)
				throw new ArgumentException("reduce must have two parameters");

			return viewServerCommandHandlers.Reduce(args[0], args[1]);
		}

		private dynamic RereduceHandler(dynamic[] args)
		{
			if (args.Length != 2)
				throw new ArgumentException("rereduce must have two parameters");

			return viewServerCommandHandlers.Rereduce(args[0], args[1]);
		}

		private void RegisterCommandHandler(string commandName, Func<dynamic[], dynamic> commandHandler)
		{
			commandHandlers.Add(commandName, commandHandler);
		}

		public async Task Run()
		{
			while (true)
			{
				var viewServerCommand = await viewServerProtocol.Read();

				Func<dynamic[], dynamic> commandHandler;
				if (!commandHandlers.TryGetValue(viewServerCommand.Name, out commandHandler))
				{
					await viewServerProtocol.Error("unknown_command", string.Format("Unknown viewserver command: {0}", viewServerCommand.Name));
					continue;
				}

				dynamic result;
				try
				{
					result = commandHandler(viewServerCommand.Arguments);
				}
				catch (Exception e)
				{
					viewServerProtocol.ErrorSync("unexpected_error", e.Message);
					continue;
				}
				await viewServerProtocol.Write(result);
			}
// ReSharper disable FunctionNeverReturns
		}
// ReSharper restore FunctionNeverReturns
	}
}