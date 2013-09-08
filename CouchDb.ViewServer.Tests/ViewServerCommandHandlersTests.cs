namespace CouchDb.ViewServer.Tests
{
	using System;

	using CouchDb.ViewServer.Host;

	using Moq;

	using Newtonsoft.Json;

	using Xunit;

	public class ViewServerCommandHandlersTests
	{
		private readonly ViewServerCommandHandlers viewServerCommandHandlers;

		private readonly Mock<IObjectCreator> objectCreator = new Mock<IObjectCreator>();

		public ViewServerCommandHandlersTests()
		{
			viewServerCommandHandlers = new ViewServerCommandHandlers(objectCreator.Object);
		}

		[Fact]
		public void MapTest()
		{
			const string Fun1 = "fun1";
			var mapper1 = new Mock<IViewMapper>();
			objectCreator.Setup(c => c.CreateObjectByTypeName(Fun1)).Returns(mapper1.Object).Verifiable();
			viewServerCommandHandlers.AddFun(Fun1);

			const string Fun2 = "fun2";
			var mapper2 = new Mock<IViewMapper>();
			objectCreator.Setup(c => c.CreateObjectByTypeName(Fun2)).Returns(mapper2.Object).Verifiable();
			viewServerCommandHandlers.AddFun(Fun2);

			var doc = new object();

			var key11 = Guid.NewGuid();
			var value11 = Guid.NewGuid();
			var key12 = Guid.NewGuid();
			var value12 = Guid.NewGuid();
			mapper1.Setup(m => m.MapDocument(It.IsAny<IViewMap>(), doc)).Callback((IViewMap map, dynamic d) =>
			{
				map.Emit(key11, value11);
				map.Emit(key12, value12);
			}).Verifiable();

			var key21 = Guid.NewGuid();
			var value21 = Guid.NewGuid();
			var key22 = Guid.NewGuid();
			var value22 = Guid.NewGuid();
			mapper2.Setup(m => m.MapDocument(It.IsAny<IViewMap>(), doc)).Callback((IViewMap map, dynamic d) =>
			{
				map.Emit(key21, value21);
				map.Emit(key22, value22);
			}).Verifiable();

			var result = this.viewServerCommandHandlers.MapDoc(doc);
			AssertEqualJson(new []
			{
				new []
				{
					new []{key11, value11},
					new []{key12, value12}
				},
				new []
				{
					new []{key21, value21},
					new []{key22, value22}
				}
			}, result);
			objectCreator.Verify();
			mapper1.Verify();
			mapper2.Verify();
		}

		private static void AssertEqualJson(dynamic expected, dynamic actual)
		{
			Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
		}
	}
}