namespace CouchDb.ViewServer.Tests
{
	using Moq;

	using Xunit;

	public class ViewServerProtocolTests
	{
		private readonly ViewServerProtocol viewServerProtocol;

		private readonly Mock<IConsoleWrapper> console = new Mock<IConsoleWrapper>();

		public ViewServerProtocolTests()
		{
			viewServerProtocol = new ViewServerProtocol(console.Object);
		}

		[Fact]
		public void ReadJsonArray()
		{
			console.Setup(c => c.ReadLine()).Returns("[\"a\", \"b\", \"c\"]");
			var command = viewServerProtocol.Read().Result;
			Assert.NotNull(command);
			Assert.Equal("a", command.Name);
			Assert.Equal(2, command.Arguments.Length);
		}
	}
}