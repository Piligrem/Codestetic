using Specter.Web.Core.Configuration;

namespace Specter.Web.Framework.WebApi
{
	public class WebApiSettings : ISettings
	{
		public int ValidMinutePeriod { get; set; }
		public bool LogUnauthorized { get; set; }
	}
}
