namespace CouchDb.ViewServer.Host
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class ViewServerCommandHandlers : IViewServerCommandHandlers
	{
		private readonly IObjectCreator objectCreator;

		private readonly List<IViewMapper> mappers = new List<IViewMapper>();

		public ViewServerCommandHandlers(IObjectCreator objectCreator)
		{
			this.objectCreator = objectCreator;
		}

		public void AddFun(string fun)
		{
			var mapperType = Type.GetType(fun);
			var mapper = (IViewMapper)objectCreator.CreateObjectByType(mapperType);
			mappers.Add(mapper);
		}

		public dynamic MapDoc(dynamic document)
		{
			var result = new List<dynamic>();
			foreach (var mapper in mappers)
			{
				var viewMap = new ViewMap();
				mapper.MapDocument(viewMap, document);
				result.Add(viewMap.Entries.Select(e => new[]{e.Key, e.Value}).ToList());
			}
			return result;
		}

		public dynamic Reduce(string[] reduceFuns, dynamic[] mapResults)
		{
			throw new NotImplementedException();
		}

		public dynamic Rereduce(string[] rereduceFuns, dynamic[] reduceResults)
		{
			throw new NotImplementedException();
		}
	}
}