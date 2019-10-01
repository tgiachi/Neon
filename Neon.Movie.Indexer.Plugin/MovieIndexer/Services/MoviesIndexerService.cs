using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Interfaces.NoSql;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Attributes;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Data;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Entities;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Interfaces;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Interfaces.Services;

namespace Neon.Movie.Indexer.Plugin.MovieIndexer.Services
{

	[NeonService("Movies Indexer Service", "Collect and index movies from streaming websites", 100)]
	public class MoviesIndexerService : IMoviesIndexerService
	{
		private readonly ILogger _logger;
		public IHttpClientFactory HttpClientFactory { get; }


		private readonly ILoggerFactory _loggerFactory;
		private readonly ITaskQueueService _taskQueueService;
		private INoSqlConnector _noSqlConnector;

		public List<string> AvailableIndexers => _moviesIndexersTypes.Keys.ToList();

	
		private readonly Dictionary<string, MoviesIndexerData> _moviesIndexersTypes = new Dictionary<string, MoviesIndexerData>();


		public MoviesIndexerService(ILogger<MoviesIndexerService> logger, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, ITaskQueueService taskQueueService, INoSqlService noSqlService)
		{

			_logger = logger;
			_loggerFactory = loggerFactory;
			HttpClientFactory = httpClientFactory;
			_taskQueueService = taskQueueService;
			_noSqlConnector = noSqlService.GetNoSqlConnector("mongo_db");
			_noSqlConnector.Configure("mongodb://localhost:27017/movies_db");
		}

		private void ScanMoviesIndexers()
		{
			if (_logger != null)
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
		}

		public MovieCategory AddMovieCategory(string name)
		{
			var entity = _noSqlConnector.Query<MovieCategory>("movies_categories")
				.FirstOrDefault(category => category.Name.ToLower() == name.ToLower());

			if (entity == null)
			{
				entity = new MovieCategory()
				{
					Name = name.ToLower()
				};

				entity = _noSqlConnector.Insert("movies_categories", entity);
			}

			return entity;
		}

		public Entities.Movie AddMovie(MovieCategory category, string name)
		{
			var entity = _noSqlConnector.Query<Entities.Movie>("movies").FirstOrDefault(movie => movie.Title == name);

			if (entity == null)
			{
				entity = new Entities.Movie()
				{
					MovieCategoryId = category.Id,
					Title = name,

				};

				entity = _noSqlConnector.Insert("movies", entity);
			}

			return entity;

		}

		public MovieLink AddMovieLink(Entities.Movie movie, string provider, string link)
		{
			var entity = _noSqlConnector.Query<MovieLink>("movies_link").FirstOrDefault(movieLink =>
				movieLink.MovieId == movie.Id && movieLink.Link == link);

			if (entity == null)
			{
				entity = new MovieLink()
				{
					Link = link,
					Provider = provider,
					MovieId = movie.Id
				};

				entity = _noSqlConnector.Insert("movies_link", entity);


			}

			return entity;

		}

		public List<MovieCategory> GetMovieCategories()
		{
			return _noSqlConnector.List<MovieCategory>("movies_categories");
		}

		public List<Entities.Movie> GetMoviesByCategoryId(string categoryId)
		{
			return _noSqlConnector.Query<Entities.Movie>("movies").Where(m => m.MovieCategoryId == categoryId).ToList();
		}

		public List<MovieLink> GetMovieLinkById(string movieId)
		{
			return _noSqlConnector.Query<MovieLink>("movies_link").Where(m => m.MovieId == movieId).ToList();
		}

		public bool StartIndexer(string name)
		{
			var indexer = _moviesIndexersTypes[name];

			if (indexer != null)
			{
				var objIndexer = (IMoviesIndexer)Activator.CreateInstance(indexer.IndexerType, new object[] { _loggerFactory.CreateLogger(indexer.IndexerType), this, _taskQueueService });
				_taskQueueService.Queue(async () => await objIndexer.Start());
				return true;
			}

			return false;

		}



		public async Task<bool> Start()
		{
			ScanMoviesIndexers();

			await _noSqlConnector.Start();
			return true;
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}


	}
}
