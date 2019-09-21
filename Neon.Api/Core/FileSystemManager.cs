using Neon.Api.Data.Config.Root;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Utils;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Neon.Api.Data.FileSystem;
using NodaTime.Text;


namespace Neon.Api.Core
{
	public class FileSystemManager : IFileSystemManager
	{
		private static string _pidFilename = "neon.pid";

		private readonly ILogger _logger;
		private readonly string _rootDirectory;
		private readonly ISecretKeyManager _secretKeyManager;

		public FileSystemManager(ILogger logger, NeonConfig neonConfig, ISecretKeyManager secretKeyManager)
		{
			_logger = logger.ForContext<FileSystemManager>();
			_rootDirectory = neonConfig.EngineConfig.HomeDirectory;
			_secretKeyManager = secretKeyManager;
		}

		public void CreateDirectory(string directory)
		{
			if (!Directory.Exists(Path.Combine(_rootDirectory, directory)))
			{
				Directory.CreateDirectory(Path.Combine(_rootDirectory, directory));
			}
		}

		public string BuildFilePath(string path)
		{
			return Path.Combine(_rootDirectory, path);
		}

		public void Start()
		{
			CreateDirectory(_rootDirectory);
			_logger.Information($"Root directory is {_rootDirectory}");

			if (File.Exists(Path.Combine(_rootDirectory, _pidFilename)))
			{
				var pidContent = File.ReadAllText(Path.Combine(_rootDirectory, _pidFilename));
				_logger.Warning($"Another Neon instance is running with pid {pidContent}");
			}

			File.WriteAllText(Path.Combine(_rootDirectory, _pidFilename), $"{Process.GetCurrentProcess().Id}");
			_logger.Information($"Neon instance PID: {Process.GetCurrentProcess().Id}");
		}

		public void Stop()
		{
			File.Delete(Path.Combine(_rootDirectory, _pidFilename));
		}

		public bool IsFileExists(string filename)
		{
			return File.Exists(Path.Combine(_rootDirectory, filename));
		}

		public bool WriteToFile<T>(string filename, T obj)
		{
			obj = _secretKeyManager.ProcessSave(obj);
			File.WriteAllText(Path.Combine(_rootDirectory, filename), obj.ToYaml());
			return true;
		}

		public bool WriteToFile(string filename, string context)
		{
			File.WriteAllText(Path.Combine(_rootDirectory, filename), context);
			return true;
		}

		public T ReadFromFile<T>(string filename)
		{
			if (!File.Exists(Path.Combine(_rootDirectory, filename))) return default(T);

			var obj = File.ReadAllText(Path.Combine(_rootDirectory, filename)).FromYaml<T>();
			if (obj == null) return default(T);

			obj = _secretKeyManager.ProcessLoad(obj);
			return obj;
		}

		public object ReadFromFile(string filename, Type type)
		{
			if (!File.Exists(Path.Combine(_rootDirectory, filename))) return null;
			var obj = File.ReadAllText(Path.Combine(_rootDirectory, filename)).FromYaml(type);

			obj = _secretKeyManager.ProcessLoad(obj);
			return obj;
		}

		public string ReadFromFile(string filename)
		{
			return File.ReadAllText(Path.Combine(_rootDirectory, filename));
		}

		public  FileSystemLsResult LsDirectory(string directory)
		{
			var lsResult = new FileSystemLsResult() {Directory = directory};
			directory = BuildFilePath(directory);

			if (!Directory.Exists(directory)) return lsResult;
			var fileResult = Directory.GetFiles(directory);

			foreach (var f in fileResult)
			{
				lsResult.Files.Add(new FileSystemFileInfoResult()
				{
					FileName = Path.GetFileName(f),
					FileSize =  new FileInfo(f).Length
				} );
			}

			return lsResult;

		}

		public FileSystemContent GetFile(string filename)
		{
			var result = new FileSystemContent()
			{
				FileName = filename,
				FileSize = -1,
			};

			filename = BuildFilePath(filename);

			if (!File.Exists(filename)) return result;

			result.Content = File.ReadAllText(filename);
			result.FileSize = new FileInfo(filename).Length;


			return result;
		}
	}
}
