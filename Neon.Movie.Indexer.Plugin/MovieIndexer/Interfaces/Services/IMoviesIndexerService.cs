using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Neon.Api.Interfaces.Base;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Entities;

namespace Neon.Movie.Indexer.Plugin.MovieIndexer.Interfaces.Services
{
	public interface IMoviesIndexerService : INeonService
	{
		List<string> AvailableIndexers { get; }

		MovieCategory AddMovieCategory(string name);


		Entities.Movie AddMovie(MovieCategory category, string name);

		MovieLink AddMovieLink(Entities.Movie movie, string provider, string link);

		bool StartIndexer(string name);

		IHttpClientFactory HttpClientFactory { get; }
	}
}
