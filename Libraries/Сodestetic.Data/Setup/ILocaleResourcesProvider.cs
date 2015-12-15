using System;

namespace Codestetic.Web.Data.Setup
{
	
	public interface ILocaleResourcesProvider
	{
		void MigrateLocaleResources(LocaleResourcesBuilder builder);
	}

}
