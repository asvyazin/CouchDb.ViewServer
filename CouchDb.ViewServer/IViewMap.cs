namespace CouchDb.ViewServer
{
	public interface IViewMap
	{
		void Emit(dynamic key, dynamic value);
	}
}