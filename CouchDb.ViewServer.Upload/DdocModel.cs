namespace CouchDb.ViewServer.Upload
{
	using System.Collections.Generic;

	using Newtonsoft.Json;

	public class DdocModel
	{
		[JsonProperty("language")]
		public string Language { get; set; }

		[JsonProperty("views")]
		public IDictionary<string, dynamic> Views { get; set; }

		public override string ToString()
		{
			return string.Format("Language: {0}, Views: {1}", this.Language, this.Views);
		}
	}
}