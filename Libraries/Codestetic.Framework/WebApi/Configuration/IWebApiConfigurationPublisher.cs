namespace Specter.Web.Framework.WebApi.Configuration
{
	public interface IWebApiConfigurationPublisher
	{
		void Configure(WebApiConfigurationBroadcaster data);
	}
}
