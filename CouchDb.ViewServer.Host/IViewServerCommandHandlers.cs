namespace CouchDb.ViewServer.Host
{
	public interface IViewServerCommandHandlers
	{
		void AddFun(string funSource);

		dynamic MapDoc(dynamic document);

		dynamic Reduce(string[] reduceFuns, dynamic[] mapResults);

		dynamic Rereduce(string[] rereduceFuns, dynamic[] reduceResults);
	}
}