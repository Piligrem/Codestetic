using System;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Domain.Configuration;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.Localization;
//using Specter.Web.Core.Domain.Themes;
using Codestetic.Web.Core.Events;
//using Specter.Web.Core.Themes;
//using Specter.Web.Framework.Events;

namespace Codestetic.Web.Framework
{
    public class FrameworkCacheConsumer :
        //IConsumer<EntityInserted<ThemeVariable>>,
        //IConsumer<EntityUpdated<ThemeVariable>>,
        //IConsumer<EntityDeleted<ThemeVariable>>,
        //IConsumer<EntityInserted<Role>>,
        //IConsumer<EntityUpdated<Role>>,
        //IConsumer<EntityDeleted<Role>>,
        //IConsumer<EntityUpdated<Setting>>,
        //IConsumer<ThemeTouched>,
        IConsumer<EntityInserted<Language>>,
        IConsumer<EntityUpdated<Language>>,
        IConsumer<EntityDeleted<Language>>
    {

        /// <summary>
        /// Key for ThemeVariables caching
        /// </summary>
        /// <remarks>
        /// {0} : theme name
        /// </remarks>
        public const string THEMEVARS_LESSCSS_KEY = "specter.pres.themevars-lesscss-{0}";
        public const string THEMEVARS_LESSCSS_THEME_KEY = "specter.pres.themevars-lesscss-{0}";

        public const string LANGUAGE_MAP_KEY = "specter.fw.langmap";

        private readonly ICacheManager _cacheManager;
        private readonly ICacheManager _aspCache;

        public FrameworkCacheConsumer(Func<string, ICacheManager> cache)
        {
            this._cacheManager = cache("static");
            this._aspCache = cache("aspnet");
        }

        #region Themes
        //public void HandleEvent(EntityInserted<ThemeVariable> eventMessage)
        //{
        //    _aspCache.Remove(BuildThemeVarsCacheKey(eventMessage.Entity));
        //}

        //public void HandleEvent(EntityUpdated<ThemeVariable> eventMessage)
        //{
        //    _aspCache.Remove(BuildThemeVarsCacheKey(eventMessage.Entity));
        //}

        //public void HandleEvent(EntityDeleted<ThemeVariable> eventMessage)
        //{
        //    _aspCache.Remove(BuildThemeVarsCacheKey(eventMessage.Entity));
        //}

        //public void HandleEvent(ThemeTouched eventMessage)
        //{
        //    var cacheKey = BuildThemeVarsCacheKey(eventMessage.ThemeName);
        //    _aspCache.RemoveByPattern(cacheKey);
        //}
        #endregion Themes

        #region Role
        //public void HandleEvent(EntityDeleted<Role> eventMessage)
        //{
        //    _cacheManager.RemoveByPattern(CUSTOMERROLES_TAX_DISPLAY_TYPES_PATTERN_KEY);
        //}

        //public void HandleEvent(EntityUpdated<Role> eventMessage)
        //{
        //    _cacheManager.RemoveByPattern(CUSTOMERROLES_TAX_DISPLAY_TYPES_PATTERN_KEY);
        //}

        //public void HandleEvent(EntityInserted<Role> eventMessage)
        //{
        //    _cacheManager.RemoveByPattern(CUSTOMERROLES_TAX_DISPLAY_TYPES_PATTERN_KEY);
        //}
        #endregion Role

        #region Language
        public void HandleEvent(EntityInserted<Language> eventMessage)
        {
            _cacheManager.Remove(LANGUAGE_MAP_KEY);
        }

        public void HandleEvent(EntityUpdated<Language> eventMessage)
        {
            _cacheManager.Remove(LANGUAGE_MAP_KEY);
        }

        public void HandleEvent(EntityDeleted<Language> eventMessage)
        {
            _cacheManager.Remove(LANGUAGE_MAP_KEY);
        }
        #endregion Language

        #region Helpers
        //private static string BuildThemeVarsCacheKey(ThemeVariable entity)
        //{
        //    return BuildThemeVarsCacheKey(entity.Theme);
        //}

        //internal static string BuildThemeVarsCacheKey(string themeName)
        //{
        //    return THEMEVARS_LESSCSS_KEY.FormatInvariant(themeName);
        //}
        #endregion Helpers
    }
}
