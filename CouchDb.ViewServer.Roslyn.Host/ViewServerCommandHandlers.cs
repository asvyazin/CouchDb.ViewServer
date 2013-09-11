namespace CouchDb.ViewServer.Roslyn.Host
{
	using CouchDb.ViewServer.Impl;

	public class ViewServerCommandHandlers : IViewServerCommandHandlers
	{
		public dynamic AddFun(string funSource)
		{
			throw new System.NotImplementedException();
		}

		public dynamic MapDoc(dynamic document)
		{
			throw new System.NotImplementedException();
		}

		public dynamic Reduce(string[] reduceFuns, dynamic[] mapResults)
		{
			throw new System.NotImplementedException();
		}

		public dynamic Rereduce(string[] rereduceFuns, dynamic[] reduceResults)
		{
			throw new System.NotImplementedException();
		}

		public dynamic Reset(dynamic options)
		{
			throw new System.NotImplementedException();
		}
	}
}