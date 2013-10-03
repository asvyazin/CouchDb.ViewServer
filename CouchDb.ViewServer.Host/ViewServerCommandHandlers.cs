namespace CouchDb.ViewServer.Host
{
	using System.Collections.Generic;
	using System.Linq;

	using CouchDb.ViewServer.Impl;

	using Newtonsoft.Json.Linq;

	public class ViewServerCommandHandlers : IViewServerCommandHandlers
	{
		private readonly IObjectCreator objectCreator;

		private readonly List<IViewMapper> mappers = new List<IViewMapper>();

		public ViewServerCommandHandlers(IObjectCreator objectCreator)
		{
			this.objectCreator = objectCreator;
		}

		public dynamic AddFun(string fun)
		{
			var mapper = (IViewMapper)objectCreator.CreateObjectByTypeName(fun);
			mappers.Add(mapper);
			return true;
		}

		public dynamic MapDoc(dynamic document)
		{
			return this.mappers
				.Select<IViewMapper, IEnumerable<Emit>>(mapper => mapper.MapDocument(document))
				.Select(emittedDocuments => emittedDocuments.Select(e => new[] { e.Key, e.Value }).ToList())
				.ToList();
		}

		public dynamic Reduce(string[] reduceFuns, dynamic[] mapResults)
		{
			var results = ConvertMapResults(mapResults).ToList();
			var reducers = reduceFuns.Select(t => objectCreator.CreateObjectByTypeName(t)).Cast<IViewReducer>();
			return reducers.Select(r => r.Reduce(results)).ToList();
		}

		private static IEnumerable<MapResult> ConvertMapResults(IEnumerable<dynamic> mapResults)
		{
			return mapResults.Select<dynamic, MapResult>(r => ConvertMapResult(r));
		}

		private static MapResult ConvertMapResult(dynamic mapResult)
		{
			var keyDocIdAndValue = mapResult;
			var keyAndDocId = keyDocIdAndValue[0];
			return new MapResult
			{
				Key = keyAndDocId[0],
				DocumentId = keyAndDocId[1],
				Value = keyDocIdAndValue[1],
			};
		}

		public dynamic Rereduce(string[] rereduceFuns, dynamic[] reduceResults)
		{
			var reducers = rereduceFuns.Select(t => objectCreator.CreateObjectByTypeName(t)).Cast<IViewReducer>();
			return reducers.Select(r => r.Rereduce(reduceResults)).ToList();
		}

		public dynamic Reset(dynamic options)
		{
			return true;
		}
	}
}