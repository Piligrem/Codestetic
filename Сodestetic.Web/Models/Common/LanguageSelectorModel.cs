using Codestetic.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Codestetic.Web.Models.Common
{
    public partial class LanguageSelectorModel : ModelBase
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
            ReturnUrls = new Dictionary<string, string>();
        }
        public IList<LanguageModel> AvailableLanguages { get; set; }
        public IDictionary<string, string> ReturnUrls { get; private set; }
        public long CurrentLanguageId { get; set; }
        public bool UseImages { get; set; }
    }
}