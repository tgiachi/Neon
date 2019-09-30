using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Utils;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Attributes;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Data;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Interfaces.Services;

namespace Neon.Movie.Indexer.Plugin.MovieIndexer.Services
{

	[NeonService("Movies Indexer Service", "Collect and index movies from streaming websites", 100)]
	public class MoviesIndexerService : IMoviesIndexerService
	{
		private readonly ILogger _logger;
		private readonly IHttpClientFactory _httpClientFactory;
		
		private  readonly Dictionary<string, MoviesIndexerData> _moviesIndexersTypes = new Dictionary<string, MoviesIndexerData>();


		public MoviesIndexerService(ILogger<MoviesIndexerService> logger, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_httpClientFactory = httpClientFactory;
		}

		private void ScanMoviesIndexers()
		{
			_logger.LogInformation($"Scan movies indexers");
			AssemblyUtils.GetAttribute<MoviesIndexerAttribute>().ForEach(t =>
			{
				_logger.LogInformation($"Adding {t.Name} in movie indexers");
				var attr = t.GetCustomAttribute<MoviesIndexerAttribute>();

				_moviesIndexersTypes.Add(attr.Name, new MoviesIndexerData()
				{
					IndexerType = t,
					Name = attr.Name,
					BaseUrl = attr.BaseUrl
				});
			});
		}
		

		public Task<bool> Start()
		{
			ScanMoviesIndexers();
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			throw new NotImplementedException();
		}
	}
}
