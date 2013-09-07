namespace CouchDb.ViewServer.Host
{
	using System.Collections.Generic;

	public class ViewMap : IViewMap
	{
		private readonly List<ViewMapEntry> entries = new List<ViewMapEntry>();

		public void Emit(dynamic key, dynamic value)
		{
			entries.Add(new ViewMapEntry(key, value));
		}

		public IEnumerable<ViewMapEntry> Entries
		{
			get
			{
				return entries;
			}
		}
	}
}