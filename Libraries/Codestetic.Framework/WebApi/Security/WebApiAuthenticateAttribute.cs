using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Specter.Web.Services.Users;
using Specter.Web.Core;
using Specter.Web.Core.Domain.Users;
using Specter.Web.Core.Domain.Logging;
using Specter.Web.Core.Infrastructure;
using Specter.Web.Services.Localization;
using Specter.Web.Core.Logging;
using Specter.Web.Services.Security;

namespace Specter.Web.Framework.WebApi.Security
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class WebApiAuthenticateAttribute : System.Web.Http.AuthorizeAttribute
	{
		private readonly IWorkContext _workContext;
		private readonly IPermissionService _permissionService;

		protected HmacAuthentication _hmac = new HmacAuthentication();

		public WebApiAuthenticateAttribute()
		{
			var engine = EngineContext.Current;

			_workContext = engine.Resolve<IWorkContext>();
			_permissionService = engine.Resolve<IPermissionService>();
		}

		/// <summary>The system name of the permission</summary>
		public string Permission { get; set; }

		protected string CreateContentMd5Hash(HttpRequestMessage request)
		{
			if (request != null && request.Content != null)
			{
				byte[] contentBytes = request.Content.ReadAsByteArrayAsync().Result;

				if (contentBytes != null && contentBytes.Length > 0)
					return _hmac.CreateContentMd5Hash(contentBytes);
			}
			return "";
		}
		protected virtual bool HasPermission(HttpActionContext actionContext, User user)
		{
			bool result = true;

			try
			{
				if (Permission.HasValue() && _permissionService.GetPermissionRecordBySystemName(Permission) != null)
				{
					result = _permissionService.Authorize(Permission, user);
				}
			}
			catch (Exception)
			{
			}
			return result;
		}
		protected virtual void LogUnauthorized(HttpActionContext actionContext, HmacResult result, User user)
		{
			try
			{
				var logger = EngineContext.Current.Resolve<ILogger>();
				var localization = EngineContext.Current.Resolve<ILocalizationService>();

				string strResult = result.ToString();
				string description = localization.GetResource("Admin.WebApi.AuthResult." + strResult, 0, false, strResult);

				var logContext = new LogContext()
				{
					ShortMessage = localization.GetResource("Admin.WebApi.UnauthorizedRequest").FormatWith(strResult),
					FullMessage = "{0}\r\n{1}".FormatWith(description, actionContext.Request.Headers.ToString()),
					LogLevel = LogLevel.Warning,
					User = user,
					HashNotFullMessage = true,
					HashIpAddress = true
				};

				logger.InsertLog(logContext);
			}
			catch (Exception exc)
			{
				exc.Dump();
			}
		}
        protected virtual User GetUser(long userId)
		{
			User user = null;
			try
			{
				user = EngineContext.Current.Resolve<IUserService>().GetUserById(userId);
			}
			catch (Exception exc)
			{
				exc.Dump();
			}
			return user;
		}
		protected virtual HmacResult IsAuthenticated(HttpActionContext actionContext, DateTime now, WebApiControllingCacheData cacheControllingData, out User user)
		{
			user = null;

			var request = HttpContext.Current.Request;
			DateTime headDateTime;

			if (request == null)
				return HmacResult.FailedForUnknownReason;

			if (cacheControllingData.ApiUnavailable)
				return HmacResult.ApiUnavailable;

			string headContentMd5 = request.Headers["Content-Md5"] ?? request.Headers["Content-MD5"];
			string headTimestamp = request.Headers[WebApiGlobal.Header.Date];
			string headPublicKey = request.Headers[WebApiGlobal.Header.PublicKey];
			string scheme = actionContext.Request.Headers.Authorization.Scheme;
			string signatureConsumer = actionContext.Request.Headers.Authorization.Parameter;

			if (string.IsNullOrWhiteSpace(headPublicKey))
				return HmacResult.UserInvalid;

			if (!_hmac.IsAuthorizationHeaderValid(scheme, signatureConsumer))
				return HmacResult.InvalidAuthorizationHeader;

			if (!_hmac.ParseTimestamp(headTimestamp, out headDateTime))
				return HmacResult.InvalidTimestamp;

			int maxMinutes = (cacheControllingData.ValidMinutePeriod <= 0 ? WebApiGlobal.DefaultTimePeriodMinutes : cacheControllingData.ValidMinutePeriod);

			if (Math.Abs((headDateTime - now).TotalMinutes) > maxMinutes)
				return HmacResult.TimestampOutOfPeriod;

			var cacheUserData = WebApiCaching.UserData();

			var apiUser = cacheUserData.FirstOrDefault(x => x.PublicKey == headPublicKey);
			if (apiUser == null)
				return HmacResult.UserUnknown;

			if (!apiUser.Enabled)
				return HmacResult.UserDisabled;

			if (apiUser.LastRequest.HasValue && headDateTime <= apiUser.LastRequest.Value)
				return HmacResult.TimestampOlderThanLastRequest;

			var context = new WebApiRequestContext()
			{
				HttpMethod = request.HttpMethod,
				HttpAcceptType = request.Headers["Accept"],
				PublicKey = headPublicKey,
				SecretKey = apiUser.SecretKey,
				Url = HttpUtility.UrlDecode(request.Url.AbsoluteUri.ToLower())
			};

			string contentMd5 = CreateContentMd5Hash(actionContext.Request);

			if (headContentMd5.HasValue() && headContentMd5 != contentMd5)
				return HmacResult.ContentMd5NotMatching;

			string messageRepresentation = _hmac.CreateMessageRepresentation(context, contentMd5, headTimestamp);

			if (string.IsNullOrEmpty(messageRepresentation))
				return HmacResult.MissingMessageRepresentationParameter;

			string signatureProvider = _hmac.CreateSignature(apiUser.SecretKey, messageRepresentation);

			if (signatureProvider != signatureConsumer)
				return HmacResult.InvalidSignature;

			user = GetUser(apiUser.UserId);
			if (user == null)
				return HmacResult.UserUnknown;

			if (!HasPermission(actionContext, user))
				return HmacResult.UserHasNoPermission;

			//var headers = HttpContext.Current.Response.Headers;
			//headers.Add(ApiHeaderName.LastRequest, apiUser.LastRequest.HasValue ? apiUser.LastRequest.Value.ToString("o") : "");

			apiUser.LastRequest = now;

			return HmacResult.Success;
		}

		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var result = HmacResult.FailedForUnknownReason;
			var cacheControllingData = WebApiCaching.ControllingData();
			var now = DateTime.UtcNow;
			User user = null;

			try
			{
				result = IsAuthenticated(actionContext, now, cacheControllingData, out user);
			}
			catch (Exception exc)
			{
				exc.Dump();
			}

			if (result == HmacResult.Success)
			{
				_workContext.CurrentUser = user;

				var response = HttpContext.Current.Response;

				response.AddHeader(WebApiGlobal.Header.Version, cacheControllingData.Version);
				response.AddHeader(WebApiGlobal.Header.MaxTop, WebApiGlobal.MaxTop.ToString());
				response.AddHeader(WebApiGlobal.Header.Date, now.ToString("o"));

				response.Cache.SetCacheability(HttpCacheability.NoCache);
			}
			else
			{
				actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

				var headers = actionContext.Response.Headers;

				var scheme = _hmac.GetWwwAuthenticateScheme(actionContext.Request.Headers.Authorization.Scheme);
				headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(scheme));		// see RFC-2616

				headers.Add(WebApiGlobal.Header.Version, cacheControllingData.Version);
				headers.Add(WebApiGlobal.Header.MaxTop, WebApiGlobal.MaxTop.ToString());
				headers.Add(WebApiGlobal.Header.Date, now.ToString("o"));
				headers.Add(WebApiGlobal.Header.HmacResultId, ((int)result).ToString());
				headers.Add(WebApiGlobal.Header.HmacResultDescription, result.ToString());

				if (cacheControllingData.LogUnauthorized)
					LogUnauthorized(actionContext, result, user);
			}
		}

		/// <remarks>we should never get here... just for security reason</remarks>
		protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
		{
			var message = new HttpResponseMessage(HttpStatusCode.Unauthorized);
			throw new HttpResponseException(message);
		}
	}
}
