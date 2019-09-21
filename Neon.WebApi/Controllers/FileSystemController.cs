using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.FileSystem;
using Neon.Api.Interfaces.Managers;

namespace Neon.WebApi.Controllers
{
	[ApiController]
	[Route("api/filesystem")]
	public class FileSystemController : ControllerBase
	{
		private readonly IFileSystemManager _fileSystemManager;
		private readonly NeonConfig _neonConfig;

		public FileSystemController(IFileSystemManager fileSystemManager, NeonConfig neonConfig)
		{
			_fileSystemManager = fileSystemManager;
			_neonConfig = neonConfig;
		}

		[HttpGet]
		[Route("ls/scripts")]
		public ActionResult<FileSystemLsResult> ScriptsLs()
		{
			return Ok(_fileSystemManager.LsDirectory(_neonConfig.ServicesConfig.ScriptEngineConfig.ScriptsDirectory
				.DirectoryName));
		}

		[HttpGet]
		[Route("read/file")]
		public ActionResult<FileSystemContent> ReadFile(string filename)
		{
			return Ok(_fileSystemManager.GetFile(filename));
		}


	}
}
