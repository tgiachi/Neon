using MediatR;
using System;

namespace Neon.Api.Interfaces.Entity
{
	public interface INeonIoTEntity : INotification
	{
		Guid Id { get; set; }
		string Name { get; set; }
		string EntityType { get; set; }
		DateTime EventDateTime { get; set; }
	}
}
