using System;
using System.Runtime.CompilerServices;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Infrastructure;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Framework
{
    public class ResourceDisplayName : System.ComponentModel.DisplayNameAttribute, IModelAttribute
    {
        private readonly string _callerPropertyName;

        public ResourceDisplayName(string resourceKey, [CallerMemberName] string propertyName = null) : base(resourceKey)
        {
            ResourceKey = resourceKey;
            _callerPropertyName = propertyName;
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                string value = null;
                var langId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.Id;
                value = EngineContext.Current.Resolve<ILocalizationService>()
                        .GetResource(ResourceKey, langId, true, "" /* ResourceKey */, true);

                if (value.IsEmpty() && _callerPropertyName.HasValue())
                {
                    value = _callerPropertyName.SplitPascalCase();
                }

                return value;
            }
        }

        public string Name
        {
            get { return "ResourceDisplayName"; }
        }
    }
}
