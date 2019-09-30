using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Interfaces.Entity;

namespace Neon.Movie.Indexer.Plugin.MovieIndexer.Entities
{
	public class MovieLink : INeonEntity
	{
		public string Id { get; set; }

		public string MovieId { get; set; }

		public string Provider { get;set; }

		public string Link { get; set; }
	}
}
