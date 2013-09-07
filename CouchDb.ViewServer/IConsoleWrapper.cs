namespace CouchDb.ViewServer
{
	public interface IConsoleWrapper
	{
		string ReadLine();

		void WriteLine(string line);
	}
}