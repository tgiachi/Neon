using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Attributes.Notifiers
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class NotifierAttribute : Attribute
	{
		public string Name { get; set; }

		public Type ConfigType { get; set; }

		public NotifierAttribute(string name)
		{
			Name = name;
		}
	}
}
