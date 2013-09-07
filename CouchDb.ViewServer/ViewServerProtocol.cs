namespace CouchDb.ViewServer
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using Newtonsoft.Json;

	public class ViewServerProtocol : IViewServerProtocol
	{
		public async Task Write(dynamic outputValue)
		{
			var outputString = await JsonConvert.SerializeObjectAsync(outputValue);
			Console.WriteLine(outputString);
		}

		public async Task<ViewServerCommand> Read()
		{
			var inputString = Console.ReadLine();
			var result = (dynamic[])(await JsonConvert.DeserializeObjectAsync<dynamic>(inputString));
			var commandName = (string)result[0];
			var commandArguments = result.Skip(1).ToArray();
			return new ViewServerCommand { Name = commandName, Arguments = commandArguments };
		}

		public async Task Log(string message)
		{
			await this.Write(new[] { "log", message });
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