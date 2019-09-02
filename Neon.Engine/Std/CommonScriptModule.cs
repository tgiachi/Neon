using System;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Config.Services;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using Neon.Engine.Services;

namespace Neon.Engine.Std
{

	[ScriptModule]
	public class CommonScriptModule
	{

		private readonly IIoTService _ioTService;
		private readonly IScriptEngineService _scriptEngineService;
		private readonly ScriptEngineConfig _scriptEngineConfig;
		private readonly IFileSystemManager _fileSystemManager;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger _logger;
		public CommonScriptModule(IIoTService ioTService, IScriptEngineService scriptEngineService, IFileSystemManager fileSystemManager, NeonConfig neonConfig, ILogger<ScriptEngineService> logger, IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
			_ioTService = ioTService;
			_fileSystemManager = fileSystemManager;
			_scriptEngineService = scriptEngineService;
			_scriptEngineConfig = neonConfig.ServicesConfig.ScriptEngineConfig;
		}

		[ScriptFunction("include", "Include and execute file")]
		public void Include(string filename)
		{
			var scriptFullFilename =
				_fileSystemManager.BuildFilePath(Path.Combine(_scriptEngineConfig.ScriptsDirectory.DirectoryName, filename));

			if (File.Exists(scriptFullFilename))
			{
				_logger.LogDebug($"Including file {filename}");
				_scriptEngineService.ExecuteCode(File.ReadAllText(scriptFullFilename));
			}
			else
				throw new FileNotFoundException($"File {filename} not found!", filename);
		}

		[ScriptFunction("include_remote", "Include remote url")]
		public async void IncludeRemote(string urlName)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient();

				var code = await httpClient.GetStringAsync(urlName);

				_logger.LogDebug($"Including url {urlName}");

				_scriptEngineService.ExecuteCode(code);

			}
			catch (Exception ex)
			{
				throw new Exception($"Error during include_remote function: {ex.Message}", ex);
			}

		}

		[ScriptFunction("get_entity_type", "Get entity Type passing name")]
		public string GetEntityType(string name)
		{
			return _ioTService.GetEntityTypeByName(name);
		}

		//[ScriptFunction("cast_entity", "Transform entity to Generic entity")]
		public T GetEntityType<T>(object entity) where T : class
		{
			var castedEntity = ((INeonIoTEntity)entity);
			var type = castedEntity.EntityType;

			var openCast = _ioTService.GetType().GetMethod(nameof(_ioTService.GetEntityByType));
			var closeCast = openCast.MakeGenericMethod(AssemblyUtils.GetType(type));
			return (T)closeCast.Invoke(_ioTService, new object[] { castedEntity.Name, castedEntity.EntityType });
		}


		static T Cast<T>(object entity) where T : class
		{
			return entity as T;
		}
	}
}
