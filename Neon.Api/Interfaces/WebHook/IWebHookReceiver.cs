namespace Neon.Api.Interfaces.WebHook
{
	public interface IWebHookReceiver
	{
		void OnHook(string payload);
	}
}
