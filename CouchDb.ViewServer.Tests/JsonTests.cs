namespace CouchDb.ViewServer.Tests
{
	using Newtonsoft.Json;

	using Xunit;

	public class JsonTests
	{
		[Fact]
		public void DynamicArray()
		{
			var result = JsonConvert.DeserializeObject<dynamic>("[{\"_id\": \"idValue1\"}, {\"_id\": \"idValue2\"}]");
			Assert.NotNull(result);
			Assert.NotNull(result[0]);
			Assert.NotNull(result[1]);
			Assert.Equal<dynamic>("idValue1", result[0]._id.Value);
			Assert.Equal<dynamic>("idValue2", result[1]._id.Value);
		}
	}
}