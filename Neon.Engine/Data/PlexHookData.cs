using MediatR;
using Newtonsoft.Json;

namespace Neon.Engine.Data
{
	public class PlexHookData : INotification
	{

		[JsonProperty("event")]
		public string Event { get; set; }


		public PlexHookMetaData Metadata { get; set; }
	}
}
