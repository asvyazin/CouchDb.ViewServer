namespace CouchDb.ViewServer.Tests
{
	using CouchDb.ViewServer.Example;
	using CouchDb.ViewServer.Upload;

	using MyCouch;

	using Xunit;

	public class ViewUploaderTests
	{
		[Fact]
		public void UploadExampleView()
		{
			using (var couchdbClient = new Client("http://localhost:5984/test"))
			{
				ViewUploader.UploadAllViewsFromAssembly(couchdbClient, typeof(ExampleMapper).Assembly).Wait();
			}
		}
	}
}