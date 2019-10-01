using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Neon.Api.Interfaces.Base;
using Neon.Api.Interfaces.Managers;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Entities;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Interfaces.Services;
using Neon.Movie.Indexer.Plugin.MovieIndexer.Services;

namespace Neon.Movie.Indexer.Plugin.MovieIndexer.Controllers
{
	[ApiController]
	[Route("api/plugins/moviesindexer/")]
	public class MoviesIndexerController : ControllerBase
	{
		private readonly IMoviesIndexerService _moviesIndexerService;

		public MoviesIndexerController(IMoviesIndexerService moviesIndexerService)
		{
			_moviesIndexerService = moviesIndexerService;
		}


		[HttpGet]
		[Route("available/indexers")]
		public ActionResult<List<string>> GetAvailableIndexers()
		{
			return _moviesIndexerService.AvailableIndexers;
		}

		[HttpPost]
		[Route("start/{name}")]
		public ActionResult<bool> StartIndexer(string name)
		{
			return Ok(_moviesIndexerService.StartIndexer(name));
		}

		[HttpGet]
		[Route("movies/categories")]
		public ActionResult<List<MovieCategory>> GetCategories()
		{
			return Ok(_moviesIndexerService.GetMovieCategories());
		}

		[HttpGet]
		[Route("movies/category/{id}")]
		public ActionResult<List<MovieCategory>> GetMovieCategory(string id)
		{
			return Ok(_moviesIndexerService.GetMoviesByCategoryId(id));
		}

		[HttpGet]
		[Route("movies/links/{id}")]
		public ActionResult<List<MovieLink>> GetMovieLink(string id)
		{
			return Ok(_moviesIndexerService.GetMovieLinkById(id));
		}
	}
}
