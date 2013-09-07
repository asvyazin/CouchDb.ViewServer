namespace CouchDb.ViewServer
{
	using System.Threading.Tasks;

	public interface IViewServerProtocol
	{
		Task Write(dynamic outputValue);

		Task<ViewServerCommand> Read();

		Task Log(string message);

		Task Error(string code, string message);

		void ErrorSync(string code, string message);
	}
}