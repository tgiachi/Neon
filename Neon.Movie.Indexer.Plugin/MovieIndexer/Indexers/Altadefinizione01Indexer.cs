using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Attributes;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Interfaces;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Interfaces.Services;

namespace Neon.Movie.Indexer.Plugin.MovieIndexer.Indexers
{

	[MoviesIndexer("Altadefinizione01.cc", "https://altadefinizione01.cc")]
	public class Altadefinizione01Indexer : IMoviesIndexer
	{
		private readonly ILogger _logger;
		private readonly IMoviesIndexerService _moviesIndexerService;
		public Altadefinizione01Indexer(ILogger<Altadefinizione01Indexer> logger, IMoviesIndexerService moviesIndexerService)
		{
			_logger = logger;
			_moviesIndexerService = moviesIndexerService;
		}
	}
}
