namespace CouchDb.ViewServer.Example
{
	using System.Collections.Generic;

	[CouchDbView("test", "example")]
	public class ExampleMapper: IViewMapper
	{
		public IEnumerable<Emit> MapDocument(dynamic document)
		{
			if (document.type == "machine" && document.color != null)
			{
				yield return new Emit(document.color, null);
			}
		}
	}
}