using System;

namespace Neon.Api.Attributes.Websocket
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class WebSocketHubAttribute : Attribute
	{

		public string Path { get; set; }

		public WebSocketHubAttribute(string path)
		{
			Path = path;
		}
	}
}
