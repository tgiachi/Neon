using Neon.Api.Data.OAuth;

namespace Neon.Api.Interfaces.Oauth
{
	public interface IOAuthReceiver
	{
		void OnOAuthReceived(string provider, OAuthResult result);
	}
}
