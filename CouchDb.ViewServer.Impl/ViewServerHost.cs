namespace CouchDb.ViewServer.Impl
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using NLog;

	public class ViewServerHost
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		private readonly IViewServerProtocol viewServerProtocol;

		private readonly IViewServerCommandHandlers viewServerCommandHandlers;

		private readonly Dictionary<string, Func<dynamic[], dynamic>> commandHandlers = new Dictionary<string, Func<dynamic[], dynamic>>();

		public ViewServerHost(IViewServerProtocol viewServerProtocol, IViewServerCommandHandlers viewServerCommandHandlers)
		{
			this.viewServerProtocol = viewServerProtocol;
			this.viewServerCommandHandlers = viewServerCommandHandlers;
			this.RegisterCommandHandler("reset", this.ResetHandler);
			this.RegisterCommandHandler("add_fun", this.AddFunHandler);
			this.RegisterCommandHandler("map_doc", this.MapDocHandler);
			this.RegisterCommandHandler("reduce", this.ReduceHandler);
			this.RegisterCommandHandler("rereduce", this.RereduceHandler);
		}

		private dynamic ResetHandler(dynamic[] args)
		{
			if (args.Length != 1)
				throw new ArgumentException("reset must have only one parameter");

			var options = args[0];
			Log.Info("reset, options: {0}", options);
			return this.viewServerCommandHandlers.Reset(options);
		}

		private dynamic AddFunHandler(dynamic[] args)
		{
			if (args.Length != 1)
				throw new ArgumentException("add_fun must have only one parameter");

			var fun = args[0].Value;
			Log.Info("loading map fun: \"{0}\"", fun);
			return this.viewServerCommandHandlers.AddFun(fun);
		}

		private dynamic MapDocHandler(dynamic[] args)
		{
			if (args.Length != 1)
				throw new ArgumentException("map_doc must have only one parameter");

			var document = args[0];
			Log.Info("mapping document: {0}", document);
			return this.viewServerCommandHandlers.MapDoc(document);
		}

		private dynamic ReduceHandler(dynamic[] args)
		{
			if (args.Length != 2)
				throw new ArgumentException("reduce must have two parameters");

			return this.viewServerCommandHandlers.Reduce(args[0].Value, args[1]);
		}

		private dynamic RereduceHandler(dynamic[] args)
		{
			if (args.Length != 2)
				throw new ArgumentException("rereduce must have two parameters");

			return this.viewServerCommandHandlers.Rereduce(args[0].Value, args[1]);
		}

		private void RegisterCommandHandler(string commandName, Func<dynamic[], dynamic> commandHandler)
		{
			this.commandHandlers.Add(commandName, commandHandler);
		}

		public async Task Run()
		{
			Log.Debug("starting couchdb viewserver...");
			while (true)
			{
				ViewServerCommand viewServerCommand;
				try
				{
					viewServerCommand = await this.viewServerProtocol.Read();
				}
				catch (Exception e)
				{
					Log.Error("Unexpected exception reading command: {0}", e);
					this.viewServerProtocol.ErrorSync("unexpected_error", e.ToString());
					continue;
				}

				Func<dynamic[], dynamic> commandHandler;
				if (!this.commandHandlers.TryGetValue(viewServerCommand.Name, out commandHandler))
				{
					await this.viewServerProtocol.Error("unknown_command", string.Format("Unknown viewserver command: {0}", viewServerCommand.Name));
					continue;
				}

				dynamic result;
				try
				{
					result = commandHandler(viewServerCommand.Arguments);
				}
				catch (Exception e)
				{
					Log.Error("Unexpected exception handling command: {0}", e);
					this.viewServerProtocol.ErrorSync("unexpected_error", e.ToString());
					continue;
				}
				await this.viewServerProtocol.Write(result);
			}
// ReSharper disable FunctionNeverReturns
		}
// ReSharper restore FunctionNeverReturns
	}
}