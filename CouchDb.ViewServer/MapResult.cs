namespace CouchDb.ViewServer
{
	public class MapResult
	{
		public dynamic Key { get; set; }
		public dynamic DocumentId { get; set; }
		public dynamic Value { get; set; }

		public override string ToString()
		{
			return string.Format("Key: {0}, DocumentId: {1}, Value: {2}", this.Key, this.DocumentId, this.Value);
		}

		protected bool Equals(MapResult other)
		{
			return Equals(this.Key, other.Key) && Equals(this.DocumentId, other.DocumentId) && Equals(this.Value, other.Value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != this.GetType())
			{
				return false;
			}
			return Equals((MapResult)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (this.Key != null ? this.Key.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (this.DocumentId != null ? this.DocumentId.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (this.Value != null ? this.Value.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}