namespace CouchDb.ViewServer
{
	public class Emit
	{
		public Emit(dynamic key, dynamic value)
		{
			this.Key = key;
			this.Value = value;
		}

		public dynamic Key { get; private set; }

		public dynamic Value { get; private set; }
	}
}