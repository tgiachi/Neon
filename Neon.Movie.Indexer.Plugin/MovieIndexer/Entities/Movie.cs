using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Interfaces.Entity;

namespace Neon.Movie.Indexer.Plugin.MovieIndexer.Entities
{
	public class Movie : INeonEntity
	{
		public string Id { get; set; }

		public string Title { get; set; }

		public string MovieCategoryId { get; set; }

	}
}
