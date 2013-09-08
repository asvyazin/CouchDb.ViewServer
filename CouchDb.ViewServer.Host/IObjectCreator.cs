namespace CouchDb.ViewServer.Host
{
	public interface IObjectCreator
	{
		object CreateObjectByTypeName(string typeName);
	}
}