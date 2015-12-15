using Specter.Web.Core.Configuration;

namespace Specter.Web.Core.Domain.Directory
{
    public class CurrencySettings : ISettings
    {
        public long PrimaryCurrencyId { get; set; }
        public long PrimaryExchangeRateCurrencyId { get; set; }
        public string ActiveExchangeRateProviderSystemName { get; set; }
        public bool AutoUpdateEnabled { get; set; }
        public long LastUpdateTime { get; set; }
    }
}