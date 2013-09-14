namespace CouchDb.ViewServer
{
	using System.Collections.Generic;

	public interface IViewMapper
	{
		IEnumerable<Emit> MapDocument(dynamic document);
	}
}