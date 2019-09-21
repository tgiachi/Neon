using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Data.FileSystem
{
	public class FileSystemLsResult
	{

		public string Directory { get; set; }


		public List<FileSystemFileInfoResult> Files { get; set; }

		public FileSystemLsResult()
		{
			Files = new List<FileSystemFileInfoResult>();
		}

	}
}
