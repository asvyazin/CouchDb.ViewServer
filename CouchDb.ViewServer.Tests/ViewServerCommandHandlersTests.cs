namespace CouchDb.ViewServer.Tests
{
	using System;
	using System.Collections.Generic;

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
			mapper1.Setup(m => m.MapDocument(doc)).Returns((dynamic d) => new[]
			{
				new Emit(key11, value11),
				new Emit(key12, value12)
			}).Verifiable();

			var key21 = Guid.NewGuid();
			var value21 = Guid.NewGuid();
			var key22 = Guid.NewGuid();
			var value22 = Guid.NewGuid();
			mapper2.Setup(m => m.MapDocument(doc)).Returns((dynamic d) => new[]
			{
				new Emit(key21, value21),
				new Emit(key22, value22)
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

		[Fact]
		public void ReduceTest()
		{
			const string Reducer = "reducer";
			var reducer = new Mock<IViewReducer>();
			objectCreator.Setup(c => c.CreateObjectByTypeName(Reducer)).Returns(reducer.Object).Verifiable();

			var key1 = Guid.NewGuid();
			var docId1 = Guid.NewGuid();
			var value1 = Guid.NewGuid();
			var key2 = Guid.NewGuid();
			var docId2 = Guid.NewGuid();
			var value2 = Guid.NewGuid();
			var expectedResult = Guid.NewGuid();

			reducer.Setup(r => r.Reduce(It.IsAny<IEnumerable<MapResult>>())).Returns((IEnumerable<MapResult> r) =>
			{
				Assert.Equal(new []
				{
					new MapResult
					{
						Key = key1,
						DocumentId = docId1,
						Value = value1,
					},
					new MapResult
					{
						Key = key2,
						DocumentId = docId2,
						Value = value2,
					}
				}, r);
				return expectedResult;
			}).Verifiable();

			var result = viewServerCommandHandlers.Reduce(new[] { Reducer }, new dynamic[]
			{
				new dynamic[] { new dynamic[] { key1, docId1 }, value1 },
				new dynamic[] { new dynamic[] { key2, docId2 }, value2 }
			});

			Assert.Equal(expectedResult, result[0]);

			objectCreator.Verify();
			reducer.Verify();
		}

		private static void AssertEqualJson(dynamic expected, dynamic actual)
		{
			Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
		}
	}
}