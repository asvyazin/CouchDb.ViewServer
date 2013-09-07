namespace CouchDb.ViewServer
{
	using System;

	public class ConsoleWrapper : IConsoleWrapper
	{
		public string ReadLine()
		{
			return Console.ReadLine();
		}

		public void WriteLine(string line)
		{
			Console.WriteLine(line);
		}
	}
}