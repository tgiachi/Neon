using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Attributes.OAuth
{

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class OAuthReceiverAttribute : Attribute
	{
		public string ProviderName { get; set; }

		public OAuthReceiverAttribute(string providerName)
		{
			ProviderName = providerName;
		}
	}
}
