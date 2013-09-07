namespace CouchDb.ViewServer.Host
{
	using System;

	public interface IObjectCreator
	{
		object CreateObjectByType(Type type);
	}
}