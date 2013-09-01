namespace CouchDb.ViewServer
{
	using System.Threading.Tasks;

	public interface IViewServer
	{
		Task Write(dynamic outputValue);

		Task<dynamic> Read();

		Task Log(string message);
	}
}