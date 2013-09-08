namespace CouchDb.ViewServer
{
	using System;

	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class CouchDbViewAttribute: Attribute
	{
		public CouchDbViewAttribute(string designDocumentName, string viewName)
		{
			DesignDocumentName = designDocumentName;
			ViewName = viewName;
		}

		public string DesignDocumentName { get; private set; }

		public string ViewName { get; private set; }
	}
}