using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Data
{
	public class NotifierData
	{
		public string Name { get; set; }

		public Type NotifierType { get; set; }

		public Type NotifierConfigType { get; set; }
	}
}
