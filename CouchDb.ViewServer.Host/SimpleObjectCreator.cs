﻿namespace CouchDb.ViewServer.Host
{
	using System;

	public class SimpleObjectCreator : IObjectCreator
	{
		public object CreateObjectByTypeName(string typeName)
		{
			return CreateObjectByType(Type.GetType(typeName));
		}

		private static object CreateObjectByType(Type type)
		{
			var parameterlessConstructor = type.GetConstructor(Type.EmptyTypes);
			if (parameterlessConstructor == null)
				throw new ArgumentException(string.Format("Type {0} must have parameterless constructor", type));

			return parameterlessConstructor.Invoke(new object[0]);
		}
	}
}