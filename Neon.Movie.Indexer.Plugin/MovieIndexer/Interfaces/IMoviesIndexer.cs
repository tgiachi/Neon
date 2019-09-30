using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Movie.Indexer.Plugin.MovieIndexer.Interfaces
{
	public interface IMoviesIndexer
	{
		Task<bool> Start();

	}

}
