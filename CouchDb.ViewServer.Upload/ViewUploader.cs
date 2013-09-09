namespace CouchDb.ViewServer.Upload
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Reflection;
	using System.Threading.Tasks;

	using MyCouch;

	using Newtonsoft.Json.Linq;

	public static class ViewUploader
	{
		private const string ViewLanguage = ".net";

		public static async Task UploadView(IClient couchdbClient, string ddocName, string viewName, Type viewType)
		{
			if (!typeof(IViewMapper).IsAssignableFrom(viewType))
				throw new InvalidOperationException(string.Format("View type {0} does not implement IViewMapper interface", viewType));

			var ddocId = string.Format("_design/{0}", ddocName);
			var ddocResponse = await couchdbClient.Documents.GetAsync(ddocId);
			if (!ddocResponse.IsSuccess)
			{
				if (ddocResponse.StatusCode != HttpStatusCode.NotFound)
					throw new HttpRequestException(string.Format("HTTP error getting design document {0}: {1}", ddocName, ddocResponse.GenerateToStringDebugVersion()));

				await PutNewDdoc(couchdbClient, ddocId, viewName, viewType);
				return;
			}

			var ddocRev = ddocResponse.Rev;
			var existingDdoc = couchdbClient.Serializer.Deserialize<DdocModel>(ddocResponse.Content);
			if (existingDdoc.Language != ViewLanguage)
				throw new InvalidOperationException(string.Format("Can not update view - existing design document {0} has different language", existingDdoc));

			dynamic viewObj;
			if (existingDdoc.Views.TryGetValue(viewName, out viewObj))
			{
				var newMap = viewType.AssemblyQualifiedName;
				if (viewObj.map == newMap)
					return;
				viewObj.map = newMap;
			}
			else
			{
				viewObj = CreateNewView(viewType);
				existingDdoc.Views.Add(viewName, viewObj);
			}

			await PutUpdatedDdoc(couchdbClient, ddocId, ddocRev, existingDdoc);
		}

		private static async Task PutNewDdoc(IClient couchdbClient, string ddocId, string viewName, Type viewType)
		{
			var newDdoc = new DdocModel
			{
				Language = ViewLanguage,
				Views = new Dictionary<string, dynamic> { { viewName, CreateNewView(viewType) } }
			};
			var newDdocJson = couchdbClient.Serializer.Serialize(newDdoc);
			await couchdbClient.Documents.PutAsync(ddocId, newDdocJson);
		}

		private static dynamic CreateNewView(Type viewType)
		{
			return new JObject { { "map", viewType.AssemblyQualifiedName } };
		}

		private static async Task PutUpdatedDdoc(IClient couchdbClient, string ddocId, string ddocRev, DdocModel updatedDdoc)
		{
			var updatedDdocJson = couchdbClient.Serializer.Serialize(updatedDdoc);
			await couchdbClient.Documents.PutAsync(ddocId, ddocRev, updatedDdocJson);
		}

		public static async Task UploadView(IClient couchdbClient, Type viewType)
		{
			var attribute = viewType.GetCustomAttribute<CouchDbViewAttribute>(false);
			await UploadView(couchdbClient, attribute.DesignDocumentName, attribute.ViewName, viewType);
		}

		public static async Task UploadAllViewsFromAssembly(IClient couchdbClient, Assembly assembly)
		{
			var viewTypes = assembly.GetTypes().Where(IsCouchDbView);
			await Task.WhenAll(viewTypes.Select(viewType => UploadView(couchdbClient, viewType)));
		}

		private static bool IsCouchDbView(Type type)
		{
			return type.CustomAttributes.Any(a => a.AttributeType == typeof(CouchDbViewAttribute));
		}
	}
}