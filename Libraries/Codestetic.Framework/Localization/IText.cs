using System;
using Codestetic.Web.Core.Localization;

namespace Codestetic.Web.Framework.Localization
{
    public interface IText
    {
        LocalizedString Get(string key, params object[] args);
    }
}
