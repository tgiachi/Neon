using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Neon.Api.Data.FileSystem
{
	public class FileSystemContent
	{
		public string FileName { get; set; }

		public long FileSize { get; set; }

		public string Content { get; set; }
	}
}
