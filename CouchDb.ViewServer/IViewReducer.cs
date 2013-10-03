namespace CouchDb.ViewServer
{
	using System.Collections.Generic;

	public interface IViewReducer
	{
		dynamic Reduce(IEnumerable<MapResult> mapResults);

		dynamic Rereduce(IEnumerable<dynamic> reduceResults);
	}
}