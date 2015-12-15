using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;

using Codestetic.Web.Areas.Admin.Models.Settings;
using Codestetic.Web.Core.Domain.Media;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.Localization;
using Codestetic.Web.Areas.Admin.Models.Localization;
using Codestetic.Web.Areas.Admin.Models.Devices;
using Codestetic.Web.Core.Domain.Devices;
using Codestetic.Web.Areas.Admin.Models.Users;
using Codestetic.Web.Models.Track;
using Codestetic.Web.Core.Domain.Tracker;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Core.Domain.GeoZones;
using Codestetic.Web.Services.GeoZones;
using Codestetic.Web.Models.GeoZones;
using Codestetic.Web.Services.Devices;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Models.GPS;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Models.Notices;
using Codestetic.Web.Core.Domain.Notices;
using Codestetic.Web.Areas.Admin.Models.Common;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Areas.Admin.Models.Messages;
using Codestetic.Web.Core.Domain.Messages;
using Codestetic.Web.Core.Domain.Directory;
using System.Web.Mvc;

namespace Codestetic.Web
{
    public static class MappingExtensions
    {
        #region Media
        public static MediaSettingsModel ToModel(this MediaSettings entity)
        {
            return Mapper.Map<MediaSettings, MediaSettingsModel>(entity);
        }
        public static MediaSettings ToEntity(this MediaSettingsModel model)
        {
            return Mapper.Map<MediaSettingsModel, MediaSettings>(model);
        }
        public static MediaSettings ToEntity(this MediaSettingsModel model, MediaSettings destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion Media

        #region User/UserSettings
        public static AdvanceUserSettingsModel.UserSettingsModel ToModel(this UserSettings entity)
        {
            return Mapper.Map<UserSettings, AdvanceUserSettingsModel.UserSettingsModel>(entity);
        }
        public static UserSettings ToEntity(this AdvanceUserSettingsModel.UserSettingsModel model)
        {
            return Mapper.Map<AdvanceUserSettingsModel.UserSettingsModel, UserSettings>(model);
        }
        public static UserSettings ToEntity(this AdvanceUserSettingsModel.UserSettingsModel model, UserSettings destination)
        {
            return Mapper.Map(model, destination);
        }
        public static AdvanceUserSettingsModel.AddressSettingsModel ToModel(this AddressSettings entity)
        {
            return Mapper.Map<AddressSettings, AdvanceUserSettingsModel.AddressSettingsModel>(entity);
        }
        public static AddressSettings ToEntity(this AdvanceUserSettingsModel.AddressSettingsModel model)
        {
            return Mapper.Map<AdvanceUserSettingsModel.AddressSettingsModel, AddressSettings>(model);
        }
        public static AddressSettings ToEntity(this AdvanceUserSettingsModel.AddressSettingsModel model, AddressSettings destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion User/UserSettings

        #region Address
        public static void PrepareModel(this AddressModel model,
            Address address, bool excludeProperties,
            AddressSettings addressSettings,
            ILocalizationService localizationService = null,
            Func<IList<Country>> loadCountries = null)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (addressSettings == null)
                throw new ArgumentNullException("addressSettings");

            if (!excludeProperties && address != null)
            {
                model.Id = address.Id;
                model.FirstName = address.FirstName;
                model.LastName = address.LastName;
                model.Email = address.Email;
                model.Company = address.Company;
                model.CountryId = address.CountryId;
                model.CountryName = address.Country != null
                    ? address.Country.GetLocalized(x => x.Name)
                    : null;
                model.City = address.City;
                model.Address1 = address.Address1;
                model.Address2 = address.Address2;
                model.ZipPostalCode = address.ZipPostalCode;
                model.PhoneNumber = address.PhoneNumber;
                model.FaxNumber = address.FaxNumber;
            }

            //countries and states
            if (addressSettings.CountryEnabled && loadCountries != null)
            {
                if (localizationService == null)
                    throw new ArgumentNullException("localizationService");

                model.AvailableCountries.Add(new SelectListItem() { Text = localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in loadCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }
            }

            //form fields
            model.CompanyEnabled = addressSettings.CompanyEnabled;
            model.CompanyRequired = addressSettings.CompanyRequired;
            model.StreetAddressEnabled = addressSettings.StreetAddressEnabled;
            model.StreetAddressRequired = addressSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = addressSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = addressSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = addressSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = addressSettings.ZipPostalCodeRequired;
            model.CityEnabled = addressSettings.CityEnabled;
            model.CityRequired = addressSettings.CityRequired;
            model.CountryEnabled = addressSettings.CountryEnabled;
            model.PhoneEnabled = addressSettings.PhoneEnabled;
            model.PhoneRequired = addressSettings.PhoneRequired;
            model.FaxEnabled = addressSettings.FaxEnabled;
            model.FaxRequired = addressSettings.FaxRequired;
        }

        public static AddressModel ToModel(this Address entity)
        {
            return Mapper.Map<Address, AddressModel>(entity);
        }
        public static Address ToEntity(this AddressModel model)
        {
            if (model == null)
                return null;

            var entity = new Address();
            return ToEntity(model, entity);
        }
        public static Address ToEntity(this AddressModel model, Address destination)
        {
            if (model == null)
                return destination;

            destination.Id = model.Id;
            destination.FirstName = model.FirstName;
            destination.LastName = model.LastName;
            destination.Email = model.Email;
            destination.Company = model.Company;
            destination.CountryId = model.CountryId;
            destination.City = model.City;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;

            return destination;
        }
        #endregion Address

        #region Notify
        public static NotifyModel[] ToModel(this IList<Notify> entity, IList<ScheduleNotifyTask> tasks)
        {
            if (entity == null || tasks == null)
                return null;

            var model = new List<NotifyModel>();
            tasks.Each(t =>
            {
                var item = new NotifyModel() { Id = 0, Name = t.Name, ScheduleTaskId = t.Id, Enabled = false, Value = "" };
                if (entity.Any(e => e.ScheduleTaskId == t.Id))
                {
                    var notify = entity.FirstOrDefault(n => n.ScheduleTaskId == t.Id);
                    item.Id = notify.Id;
                    item.Enabled = notify.Enabled;
                    item.Value = notify.Value;
                }

                model.Add(item);
            });

            return model.ToArray();
        }
        #endregion Notify

        #region DevicePosition
        public static DevicePositionModel ToModel(this DevicePosition devicePosition)
        {
            if (devicePosition == null) return null;

            return new DevicePositionModel()
            {
                Longitude = devicePosition.Longitude,
                Latitude = devicePosition.Latitude,
                Altitude = devicePosition.Altitude,
                Angle = devicePosition.Angle,
                Speed = devicePosition.Speed,
            };
        }
        #endregion DevicePosition

        #region DeviceSetting
        public static DeviceSettingModel ToModel(this DeviceSetting deviceSetting, IDateTimeHelper dateTimeHelper)
        {
            //if (deviceSetting == null) return null;
            deviceSetting = deviceSetting ?? new DeviceSetting();
            return new DeviceSettingModel()
            {
                Id = deviceSetting.Id,
                IntervalUpdateDevice = deviceSetting.IntervalUpdateDevice,
                //IntervalUpdateLocation = deviceSetting.IntervalUpdateLocation,
                ControlSatellites = deviceSetting.ControlSatellites,
                ControlBattery = deviceSetting.ControlBattery,
                MinBatteryLevel = deviceSetting.MinBatteryLevel,
                ControlGSM = deviceSetting.ControlGSM,
                ControlButtonSos = deviceSetting.ControlButtonSos,
                ControlInGeoZone = deviceSetting.ControlInGeoZone,
                ControlOutGeoZone = deviceSetting.ControlOutGeoZone,
                UpdateTime = dateTimeHelper.ConvertToUserTime(deviceSetting.UpdateOnUtc != null ? deviceSetting.UpdateOnUtc.Value : DateTime.MinValue, DateTimeKind.Utc)
            };
        }
        #endregion DeviceSetting

        #region DeviceIndicator
        public static DeviceIndicatorModel ToModel(this DeviceIndicator deviceIndicator, DeviceSetting deviceSetting, IDateTimeHelper dateTimeHelper, ILocalizationService localizationService, int languageId = 0)
        {
            var result = new DeviceIndicatorModel();
            deviceIndicator = deviceIndicator ?? new DeviceIndicator();
            deviceSetting = deviceSetting ?? new DeviceSetting();

            result.Id = deviceIndicator.Id;

            //result.Info.MarkerFormat = string.Empty;
            //result.Info.MarkerValue = "icon-info_now";
            result.Info.TooltipFormat = localizationService.GetResource("Indicator.Info", languageId);
            //result.Info.TooltipValue = dateTimeHelper.ConvertToUserTime(deviceIndicator.TimestampOnUtc, DateTimeKind.Utc);

            var tooltipFormat = string.Empty;
            tooltipFormat = localizationService.GetResource("Indicator.Satellites", languageId);

            //result.Satellites.MarkerFormat = "icon-satellites_{0}";
            //result.Satellites.MarkerValue = ConvertGPSValues.SatellitesMarker(deviceIndicator.Satellites);
            result.Satellites.TooltipFormat = tooltipFormat;
            //result.Satellites.TooltipValue = deviceIndicator.Satellites;

            tooltipFormat = string.Empty;
            tooltipFormat = localizationService.GetResource("Indicator.Battery", languageId);

            //result.Battery.MarkerFormat = "icon-gsm_{0}";
            //result.Battery.MarkerValue = ConvertGPSValues.BatteryMarker(deviceIndicator.Battery);
            result.Battery.TooltipFormat = tooltipFormat;
            //result.Battery.TooltipValue = ConvertGPSValues.Battery(deviceIndicator.Battery);

            tooltipFormat = string.Empty;
            tooltipFormat = localizationService.GetResource("Indicator.GSM", languageId);

            //result.GSM.MarkerFormat = "icon-battery_{0}";
            //result.GSM.MarkerValue = ConvertGPSValues.GSMMarker(deviceIndicator.GSM);
            result.GSM.TooltipFormat = tooltipFormat;
            //result.GSM.TooltipValue = ConvertGPSValues.GSM(deviceIndicator.GSM);

            tooltipFormat = localizationService.GetResource("Indicator.ControlGeoZone", languageId);
            //result.GeoZoneInfo.MarkerFormat = string.Empty;
            //result.GeoZoneInfo.MarkerValue = "icon-geozone_info";
            result.GeoZoneInfo.TooltipFormat = tooltipFormat;
            //var controlGeoZone = false;
            //if (deviceSetting.ControlInGeoZone || deviceSetting.ControlOutGeoZone)
            //    controlGeoZone = true;

            //result.GeoZoneInfo.TooltipValue = localizationService.GetResource("Indicator.ControlGeoZone.{0}".FormatWith(controlGeoZone), languageId); ;
            return result;
        }
        #endregion DeviceIndicator

        #region GeoZone
        public static IList<GeoZoneModel> ToModel(this IList<GeoZone> entity, bool zones = false)
        {
            if (entity == null)
                return null;

            var model = new List<GeoZoneModel>();
            var deviceService = EngineContext.Current.Resolve<IDeviceService>();

            entity.Each(e =>
            {
                var geoZoneModel = new GeoZoneModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    StrokeColor = e.StrokeColor,
                    FillColor = e.FillColor,
                    StrokeWidth = e.StrokeWidth,
                    PrototypeZone = Enum.GetName(typeof(PrototypeZone), e.PrototypeZone),
                    Zone = zones ? Geo.Parse<List<List<List<double>>>>(e.Zone) : null,
                    Devices = deviceService.GetDevicesByGeoZoneId(e.Id).Select(d => d.Id).ToList(),
                };

                model.Add(geoZoneModel);
            });
            return model;
        }
        public static IList<GeoZone> ToEntity(this IList<GeoZoneModel> model, User user)
        {
            if (model == null || user == null)
                return null;

            var entity = new List<GeoZone>();
            var geoZoneService = EngineContext.Current.Resolve<IGeoZoneService>();

            model.Each(e =>
            {
                var polygon = Geo.ConvertZone(e.Zone);
                var geoZone = new GeoZone();
                if (polygon != null)
                {
                    if (e.Id == 0)
                    {
                        geoZone.UserId = user.Id;
                        geoZone.CreatedOnUtc = DateTime.UtcNow;
                    }
                    else
                    {
                        geoZone = geoZoneService.GetGeoZoneById(e.Id);
                        geoZone.UpdateOnUtc = DateTime.UtcNow;
                    }
                    geoZone.Name = e.Name;
                    geoZone.StrokeColor = e.StrokeColor;
                    geoZone.FillColor = e.FillColor;
                    geoZone.StrokeWidth = e.StrokeWidth;
                    geoZone.PrototypeZone = (PrototypeZone)Enum.Parse(typeof(PrototypeZone), e.PrototypeZone);
                    geoZone.Zone = Geo.ToPolygon(polygon);

                    entity.Add(geoZone);
                }
            });
            return entity;
        }
        #endregion GeoZone

        #region Track
        public static TrackModel ToModel(this IList<Track> entity)
        {
            var dateHelper = EngineContext.Current.Resolve<IDateTimeHelper>();
            var model = new TrackModel();
            entity.Each(e =>
            {
                var info = new TrackInfoModel()
                {
                    Id = e.Id,
                    Date = dateHelper.ConvertToUserTime(e.TimestampOnUtc, DateTimeKind.Utc).Value,
                    Angle = e.Angle,
                    Speed = e.Speed,
                    Altitude = e.Altitude,
                };
                model.Info.Add(info);
                model.Position.Add(Geo.Parse<List<double>>(e.Position));
            });
            return model;
        }
        #endregion Track

        #region Users/UserRoles
        //user roles
        public static UserRoleModel ToModel(this UserRole entity)
        {
            Mapper.CreateMap<UserRole, UserRoleModel>()
                .ForSourceMember(x => x.CreatedOnUtc, y => y.Ignore());

            return Mapper.Map<UserRole, UserRoleModel>(entity);
        }

        public static UserRole ToEntity(this UserRoleModel model)
        {
            return Mapper.Map<UserRoleModel, UserRole>(model);
        }

        public static UserRole ToEntity(this UserRoleModel model, UserRole destination)
        {
            return Mapper.Map(model, destination);
        }

        public static UserModel ToModel(this User entity)
        {
            Mapper.CreateMap<User, UserModel>()
                .ForSourceMember(x => x.IsSystemAccount, y => y.Ignore())
                .ForSourceMember(x => x.LastActivityDateUtc, y => y.Ignore())
                .ForSourceMember(x => x.LastIpAddress, y => y.Ignore())
                .ForSourceMember(x => x.LastLoginDateUtc, y => y.Ignore())
                .ForSourceMember(x => x.Notifies, y => y.Ignore())
                .ForSourceMember(x => x.Password, y => y.Ignore())
                .ForSourceMember(x => x.PasswordFormat, y => y.Ignore())
                .ForSourceMember(x => x.PasswordFormatId, y => y.Ignore())
                .ForSourceMember(x => x.PasswordSalt, y => y.Ignore())
                .ForSourceMember(x => x.SystemName, y => y.Ignore())
                .ForSourceMember(x => x.UserGuid, y => y.Ignore())
                .ForSourceMember(x => x.UserRoles, y => y.Ignore())
                .ForMember(x => x.FullName, y => y.MapFrom(t => t.GetFullName()));
            return Mapper.Map<User, UserModel>(entity);
        }
        #endregion Users/UserRoles

        #region Device/DeviceType
        public static DeviceTypeModel ToModel(this DeviceType entity)
        {
            Mapper.CreateMap<DeviceType, DeviceTypeModel>()
                .ForSourceMember(x => x.Devices, y => y.Ignore())
                .ForMember(y => y.DeviceModel, y => y.MapFrom(source => source.Model));

            return Mapper.Map<DeviceType, DeviceTypeModel>(entity);
        }
        #endregion Device/DeviceType

        #region Languages
        public static LanguageModel ToModel(this Language entity)
        {
            return Mapper.Map<Language, LanguageModel>(entity);
        }

        public static Language ToEntity(this LanguageModel model)
        {
            return Mapper.Map<LanguageModel, Language>(model);
        }

        public static Language ToEntity(this LanguageModel model, Language destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion Languages

        #region Queued email

        public static QueuedEmailModel ToModel(this QueuedEmail entity)
        {
            return Mapper.Map<QueuedEmail, QueuedEmailModel>(entity);
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model)
        {
            return Mapper.Map<QueuedEmailModel, QueuedEmail>(model);
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model, QueuedEmail destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion Queued email

        #region Email account
        public static EmailAccountModel ToModel(this EmailAccount entity)
        {
            return Mapper.Map<EmailAccount, EmailAccountModel>(entity);
        }
        public static EmailAccount ToEntity(this EmailAccountModel model)
        {
            return Mapper.Map<EmailAccountModel, EmailAccount>(model);
        }
        public static EmailAccount ToEntity(this EmailAccountModel model, EmailAccount destination)
        {
            return Mapper.Map(model, destination);
        }
        #endregion Email account
    }
}
