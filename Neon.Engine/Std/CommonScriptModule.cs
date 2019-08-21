using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;

namespace Neon.Engine.Std
{

	[ScriptModule]
	public class CommonScriptModule
	{

		private readonly IIoTService _ioTService;
		public CommonScriptModule(IIoTService ioTService)
		{
			_ioTService = ioTService;
		}

		[ScriptFunction("get_entity_type", "Get entity Type passing name")]
		public string GetEntityType(string name)
		{
			return _ioTService.GetEntityTypeByName(name);
		}

		[ScriptFunction("cast_entity", "Transform entity to Generic entity")]
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
