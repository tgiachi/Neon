using Newtonsoft.Json;

namespace Neon.Engine.Data
{
	public class PlexHookMetaData
	{
		[JsonProperty("librarySectionType")]
		public string LibrarySectionType { get; set; }


		[JsonProperty("librarySectionTitle")]
		public string LibrarySectionTitle { get; set; }

		[JsonProperty("ratingKey")]
		public string RatingKey { get; set; }


		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("guid")]
		public string Guid { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("rating")]
		public double Rating { get; set; }



	}
}
