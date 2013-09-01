namespace CouchDb.ViewServer
{
	using System;
	using System.Threading.Tasks;

	using Newtonsoft.Json;

	public class ViewServer : IViewServer
	{
		public async Task Write(dynamic outputValue)
		{
			var outputString = await JsonConvert.SerializeObjectAsync(outputValue);
			Console.WriteLine(outputString);
		}

		public async Task<dynamic> Read()
		{
			var inputString = Console.ReadLine();
			var result = await JsonConvert.DeserializeObjectAsync<dynamic>(inputString);
			return result;
		}

		public async Task Log(string message)
		{
			await Write(new[] { "log", message });
		}
	}
}