using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Data.Services
{
	public class ServiceInfo
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
		public ServiceStatus Status { get; set; }

		public Exception Error { get; set; }


		public ServiceInfo()
		{
			Id = Guid.NewGuid();
			Status = ServiceStatus.Stopped;
		}

		
	}

	public enum ServiceStatus
	{
		Stopped,
		Starting,
		Started,
		Error
	}
}
