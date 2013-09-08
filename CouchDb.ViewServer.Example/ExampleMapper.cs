namespace CouchDb.ViewServer.Example
{
	[CouchDbView("test", "example")]
	public class ExampleMapper: IViewMapper
	{
		public void MapDocument(IViewMap map, dynamic document)
		{
			if (document.type == "machine" && document.color != null)
			{
				map.Emit(document.color, null);
			}
		}
	}
}