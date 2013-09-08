namespace CouchDb.ViewServer
{
	using System.Linq;
	using System.Threading.Tasks;

	using NLog;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class ViewServerProtocol : IViewServerProtocol
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		private readonly IConsoleWrapper console;

		public ViewServerProtocol(IConsoleWrapper console)
		{
			this.console = console;
		}

		public async Task Write(dynamic outputValue)
		{
			await this.DoWrite(outputValue, false);
		}

		private async Task DoWrite(dynamic outputValue, bool preventSelfCall)
		{
			var outputString = await JsonConvert.SerializeObjectAsync(outputValue);
			if (!preventSelfCall)
				Logger.Debug("Output: \"{0}\"", outputString);
			console.WriteLine(outputString);
		}

		public async Task<ViewServerCommand> Read()
		{
			var inputString = console.ReadLine();
			Logger.Debug("Input: \"{0}\"", inputString);
			var commandJson = await JsonConvert.DeserializeObjectAsync<dynamic>(inputString);
			var result = ((JArray)commandJson).Cast<dynamic>().ToArray();
			var commandName = (string)result[0];
			var commandArguments = result.Skip(1).ToArray();
			return new ViewServerCommand { Name = commandName, Arguments = commandArguments };
		}

		public async Task Log(string message)
		{
			await this.DoWrite(new[] { "log", message }, true);
		}

		public void LogSync(string message)
		{
			this.Log(message).Wait();
		}

		public async Task Error(string code, string message)
		{
			await this.Write(new[] { "error", code, message });
		}

		public void ErrorSync(string code, string message)
		{
			this.Error(code, message).Wait();
		}
	}
}