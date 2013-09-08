namespace CouchDb.ViewServer.Upload
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;

	using MyCouch;

	using Newtonsoft.Json.Linq;

	public class ViewUploader
	{
		private const string ViewLanguage = ".net";

		public static async Task UploadView(IClient couchdbClient, string ddocName, string viewName, Type viewType)
		{
			if (!viewType.IsSubclassOf(typeof(IViewMapper)))
				throw new InvalidOperationException("View must implement IViewMapper interface");

			var ddocId = string.Format("_design/{0}", ddocName);
			var ddocResponse = await couchdbClient.Documents.GetAsync(ddocId);
			if (!ddocResponse.IsSuccess)
			{
				if (ddocResponse.StatusCode != HttpStatusCode.NotFound)
					throw new HttpRequestException(string.Format("HTTP error getting design document {0}: {1}", ddocName, ddocResponse));

				await PutNewDdoc(couchdbClient, ddocId, viewName, viewType);
				return;
			}

			var ddocRev = ddocResponse.Rev;
			var existingDdoc = couchdbClient.Serializer.Deserialize<DdocModel>(ddocResponse.Content);
			if (existingDdoc.Language != ViewLanguage)
				throw new InvalidOperationException(string.Format("Can not update view - existing design document {0} has different language", existingDdoc));

			dynamic viewObj;
			if (existingDdoc.Views.TryGetValue(viewName, out viewObj))
				UpdateMap(viewObj, viewType);
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

		private static void UpdateMap(dynamic viewObj, Type viewType)
		{
			viewObj.map = viewType.FullName;
		}

		private static dynamic CreateNewView(Type viewType)
		{
			return new JObject { { "map", viewType.FullName } };
		}

		private static async Task PutUpdatedDdoc(IClient couchdbClient, string ddocId, string ddocRev, DdocModel updatedDdoc)
		{
			var updatedDdocJson = couchdbClient.Serializer.Serialize(updatedDdoc);
			await couchdbClient.Documents.PutAsync(ddocId, ddocRev, updatedDdocJson);
		}
	}
}