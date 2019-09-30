using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.Logging;
using Neon.Api.Interfaces.Services;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Attributes;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Entities;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Interfaces;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Interfaces.Services;

namespace Neon.Movie.Indexer.Plugin.MovieIndexer.Indexers
{

	[MoviesIndexer("Altadefinizione01.cc", "https://altadefinizione01.cc")]
	public class Altadefinizione01Indexer : IMoviesIndexer
	{
		private readonly ILogger _logger;
		private readonly IMoviesIndexerService _moviesIndexerService;
		private readonly ITaskQueueService _taskQueueService;
		private readonly Dictionary<string, string> _categories = new Dictionary<string, string>();
		private readonly string _baseUrl;

		private readonly HttpClient _httpClient;
		private readonly Queue<string> _categoriesPages = new Queue<string>();

		public Altadefinizione01Indexer(ILogger logger, IMoviesIndexerService moviesIndexerService, ITaskQueueService taskQueueService)
		{
			_logger = logger;
			_moviesIndexerService = moviesIndexerService;
			_taskQueueService = taskQueueService;
			var attr = GetType().GetCustomAttribute<MoviesIndexerAttribute>();
			_httpClient = moviesIndexerService.HttpClientFactory.CreateClient("linkDownloader");
			_baseUrl = attr.BaseUrl;
		}

		public async Task<bool> Start()
		{
			_logger.LogInformation("Starting Altadefinizione Indexer");
			await DownloadCategories();

			foreach (var keyValuePair in _categories)
			{
				_taskQueueService.Queue(async () => { await DownloadCategory(keyValuePair.Key, keyValuePair.Value); });
			}
			return true;

		}

		private async Task DownloadCategory(string name, string path)
		{
			var category = _moviesIndexerService.AddMovieCategory(name);
			_logger.LogInformation($"Downloading number of pages of category {name}");
			path = path + "page/1";
			var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
			var document = await context.OpenAsync(path);

			var pages = document.QuerySelectorAll(".wp-pagenavi a");

			var pagesMax = pages[pages.Count() - 2] as IHtmlAnchorElement;

			_logger.LogInformation($"Category {name} pages: {pagesMax.Text}");

			for (var i = 1; i < int.Parse(pagesMax.Text); i++)
			{
				var i1 = i;
				_taskQueueService.Queue(async () => await DownloadPage(category, path, i1));
			}
		}

		private async Task DownloadPage(MovieCategory category, string path, int pageNumber)
		{
			var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
			var document = await context.OpenAsync(_baseUrl + "/" + category.Name + "/page/" + pageNumber);
			var movies = document.QuerySelectorAll(".cover.boxcaption h2 a");

			movies.ToList().ForEach(movie =>
			{
				var movieLink = movie as IHtmlAnchorElement;

				_taskQueueService.Queue(async () =>
					await DownloadMovie(category, movieLink.InnerHtml, movieLink.PathName));
			});
		}

		private async Task DownloadMovie(MovieCategory category, string movieName, string movieLink)
		{
			var movie = _moviesIndexerService.AddMovie(category, movieName);
			var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
			var document = await context.OpenAsync(_baseUrl + movieLink);
			var links = document.QuerySelectorAll(".dwnDesk.linkDown");

			links.ToList().ForEach(async link =>
			{
				var providerLink = link as IHtmlAnchorElement;

				var content = await _httpClient.GetStringAsync(providerLink.Href);

				var startIndex = content.IndexOf("window.location = '");

				var videoLinkRaw = content.Substring(startIndex, content.IndexOf("';", startIndex));
				videoLinkRaw = videoLinkRaw.Split("\n")[0];

				var videoLink = videoLinkRaw.Replace("window.location = '", "").Replace("';", "");

				var videoUrl = new Url(videoLink);

				_moviesIndexerService.AddMovieLink(movie, videoUrl.HostName, videoLink);

			});

			_logger.LogInformation($"Movie {movieName} ({category.Name}) indexed");
		}

		private async Task DownloadCategories()
		{
			var attr = GetType().GetCustomAttribute<MoviesIndexerAttribute>();
			var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
			var document = await context.OpenAsync(attr.BaseUrl);
		
			var li = document.QuerySelectorAll(".kategori_list li a");
			li.ToList().ForEach(link =>
			{
				var href = (IHtmlAnchorElement)link;

				if (href.PathName != "/")
				{
					_categories.Add(href.Text, attr.BaseUrl + href.PathName);
				}
			});
		}
	}
}
