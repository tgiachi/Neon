using System;

namespace Neon.Api.Attributes.WebHook
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class WebHookReceiverAttribute : Attribute
	{
		public string ProviderName { get; set; }

		public WebHookReceiverAttribute(string providerName)
		{
			ProviderName = providerName;
		}
	}
}
