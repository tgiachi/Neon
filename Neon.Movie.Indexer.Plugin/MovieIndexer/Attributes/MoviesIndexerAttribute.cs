using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Movie.Indexer.Plugin.MovieIndexer.Attributes
{


	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class MoviesIndexerAttribute : Attribute
	{
		public string Name { get; set; }

		public string BaseUrl { get; set; }
		
		public MoviesIndexerAttribute(string name, string baseUrl)
		{
			Name = name;
			BaseUrl = baseUrl;
		}
	}
}
