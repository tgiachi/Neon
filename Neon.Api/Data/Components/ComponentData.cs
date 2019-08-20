using System;

namespace Neon.Api.Data.Components
{
	public class ComponentData
	{
		public Guid Id { get; set; }

		public ComponentInfo Info { get; set; }

		public ComponentStatusEnum Status { get; set; }

		public Exception Error { get; set; }


		public override string ToString()
		{
			return $"Id: {Id} - Name: {Info.Name} - Status: {Status}";
		}
	}
}
