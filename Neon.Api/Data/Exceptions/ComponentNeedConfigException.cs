using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Neon.Api.Data.Exceptions
{
	[Serializable]
	public class ComponentNeedConfigException : Exception
	{
		public string ComponentName { get; set; }

		public List<string> ConfigKeys = new List<string>();

		public ComponentNeedConfigException()
		{
		}

		public ComponentNeedConfigException(string message) : base(message)
		{
		}

		public ComponentNeedConfigException(string message, Exception inner) : base(message, inner)
		{
		}

		protected ComponentNeedConfigException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}

}
