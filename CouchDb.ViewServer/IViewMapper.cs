namespace CouchDb.ViewServer
{
	public interface IViewMapper
	{
		void MapDocument(IViewMap map, dynamic document);
	}
}