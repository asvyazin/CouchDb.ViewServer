namespace CouchDb.ViewServer.Host
{
	public class ViewMapEntry
	{
		public ViewMapEntry(dynamic key, dynamic value)
		{
			this.Key = key;
			this.Value = value;
		}

		public dynamic Value { get; private set; }

		public dynamic Key { get; private set; }
	}
}