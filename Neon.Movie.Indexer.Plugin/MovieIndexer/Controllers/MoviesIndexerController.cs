using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Neon.Api.Interfaces.Base;
using Neon.Api.Interfaces.Managers;
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
		[Route("test")]
		public ActionResult<string> TestGet()
		{
			return Ok("ok");
		}
	}
}
