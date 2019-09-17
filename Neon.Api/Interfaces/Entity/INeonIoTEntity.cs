using MediatR;
using System;

namespace Neon.Api.Interfaces.Entity
{
	public interface INeonIoTEntity : INotification, INeonEntity
	{
		string GroupName { get; set; }
		string Name { get; set; }
		string EntityType { get; set; }
		DateTime EventDateTime { get; set; }
	}
}
