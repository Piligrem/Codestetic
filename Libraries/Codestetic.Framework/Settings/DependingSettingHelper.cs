using System.Linq;
using System.Web.Mvc;
using Fasterflect;
using System.Collections.Generic;
using Codestetic.Web.Services.Configuration;

namespace Codestetic.Web.Framework.Settings
{
    public class DependingSettingHelper
    {
        private ViewDataDictionary _viewData;

        public DependingSettingHelper(ViewDataDictionary viewData)
        {
            _viewData = viewData;
        }

        public static string ViewDataKey { get { return "DependingSettingData"; } }

        public DependingSettingData Data
        {
            get { return _viewData[ViewDataKey] as DependingSettingData; }
        }

        private bool IsOverrideChecked(string settingKey, FormCollection form)
        {
            var rawOverrideKey = form.AllKeys.FirstOrDefault(k => k.IsCaseInsensitiveEqual(settingKey + "_OverrideForSite"));

            if (rawOverrideKey.HasValue())
            {
                var checkboxValue = form[rawOverrideKey].EmptyNull().ToLower();
                return checkboxValue.Contains("on") || checkboxValue.Contains("true");
            }
            return false;
        }

        public bool IsOverrideChecked(object settings, string name, FormCollection form)
        {
            var key = settings.GetType().Name + "." + name;
            return IsOverrideChecked(key, form);
        }

        public void AddOverrideKey(object settings, string name)
        {
            var key = settings.GetType().Name + "." + name;
            Data.OverrideSettingKeys.Add(key);
        }

        public void CreateViewDataObject(string rootSettingClass = null)
        {
            _viewData[ViewDataKey] = new DependingSettingData()
            {
                RootSettingClass = rootSettingClass
            };
        }

        public void GetOverrideKeys(object settings, object model, ISettingService settingService, bool isRootModel = true)
        {
            var data = Data;
            if (data == null)
                data = new DependingSettingData();

            var settingName = settings.GetType().Name;
            var properties = settings.GetType().GetProperties();

            var modelType = model.GetType();

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var modelProperty = modelType.GetProperty(name);

                if (modelProperty == null)
                    continue;	// setting is not configurable or missing or whatever... however we don't need the override info

                var key = settingName + "." + name;
                var setting = settingService.GetSettingByKey<string>(key);

                if (setting != null)
                    data.OverrideSettingKeys.Add(key);
            }

            if (isRootModel)
            {
                data.RootSettingClass = settingName;

                _viewData[ViewDataKey] = data;
            }
        }
        public void UpdateSettings(object settings, FormCollection form, ISettingService settingService)
        {
            var settingName = settings.GetType().Name;
            var properties = settings.GetType().GetProperties();

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var key = settingName + "." + name;

                //if (storeId == 0 || IsOverrideChecked(key, form))
                {
                    dynamic value = settings.TryGetPropertyValue(name);
                    settingService.SetSetting(key, value == null ? "" : value, false);
                }
                //else if (storeId > 0)
                //{
                //    settingService.DeleteSetting(key);
                //}
            }
        }
    }
}
