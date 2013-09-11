namespace CouchDb.ViewServer.Impl
{
	public interface IViewServerCommandHandlers
	{
		dynamic AddFun(string funSource);

		dynamic MapDoc(dynamic document);

		dynamic Reduce(string[] reduceFuns, dynamic[] mapResults);

		dynamic Rereduce(string[] rereduceFuns, dynamic[] reduceResults);

		dynamic Reset(dynamic options);
	}
}