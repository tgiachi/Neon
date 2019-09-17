using Neon.Api.Attributes.Entities;
using Neon.Api.Interfaces.Entity;
using System;

namespace Neon.Api.Data.Entities
{
	public class NeonIoTBaseEntity : INeonIoTEntity
	{
		[IgnorePropertyCompare]
		public string Id { get; set; }

		public string Name { get; set; }

		public string GroupName { get; set; }

		public string EntityType { get; set; }

		[IgnorePropertyCompare]
		public DateTime EventDateTime { get; set; }
	}
}
