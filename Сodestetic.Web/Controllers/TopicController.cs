﻿using System.Web.Mvc;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Services.Topics;
using Codestetic.Web.Infrastructure.Cache;
using Codestetic.Web.Models.Topics;
using Codestetic.Web.Framework.Controllers;

namespace Codestetic.Web.Controllers
{
    public partial class TopicController : PublicControllerBase
    {
        #region Fields
        private readonly ITopicService _topicService;
        private readonly IWorkContext _workContext;
		private readonly ISiteContext _siteContext;
        private readonly ILocalizationService _localizationService;
        private readonly ICacheManager _cacheManager;
        #endregion Fields

        #region Constructors
        public TopicController(ITopicService topicService,
            ILocalizationService localizationService,
            IWorkContext workContext,
			ISiteContext siteContext,
			ICacheManager cacheManager)
        {
            this._topicService = topicService;
            this._workContext = workContext;
			this._siteContext = siteContext;
            this._localizationService = localizationService;
            this._cacheManager = cacheManager;
        }
        #endregion Constructors

        #region Utilities
        [NonAction]
        protected TopicModel PrepareTopicModel(string systemName)
        {
			//load by store
			var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return null;

            var model = new TopicModel()
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Title),
                Body = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Body),
                MetaKeywords = topic.GetLocalized(x => x.MetaKeywords),
                MetaDescription = topic.GetLocalized(x => x.MetaDescription),
                MetaTitle = topic.GetLocalized(x => x.MetaTitle),
            };
            return model;
        }
        #endregion Utilities

        #region Methods
        public ActionResult TopicDetails(string systemName)
        {
			var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return RedirectToRoute("HomePage");
            return View("TopicDetails", cacheModel);
        }

        public ActionResult TopicDetailsPopup(string systemName)
        {
			var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return RedirectToRoute("HomePage");

            ViewBag.IsPopup = true;
            return View("TopicDetails", cacheModel);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult TopicBlock(string systemName, bool bodyOnly = false)
        {
			var cacheKey = string.Format(ModelCacheEventConsumer.TOPIC_MODEL_KEY, systemName, _workContext.WorkingLanguage.Id, 0);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return Content("");

            ViewBag.BodyOnly = bodyOnly;
            return PartialView(cacheModel);
        }

        [ChildActionOnly]
        public ActionResult TopicWidget(TopicWidgetModel model)
        {
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Authenticate(int id, string password)
        {
            var authResult = false;
            var title = string.Empty;
            var body = string.Empty;
            var error = string.Empty;

            var topic = _topicService.GetTopicById(id);

            if (topic != null)
            {
                if (topic.Password != null && topic.Password.Equals(password))
                {
                    authResult = true;
                    title = topic.GetLocalized(x => x.Title);
                    body = topic.GetLocalized(x => x.Body);
                }
                else
                {
                    error = _localizationService.GetResource("Topic.WrongPassword");
                }
            }
            return Json(new { Authenticated = authResult, Title = title, Body = body, Error = error });
        }
        #endregion Methods
    }
}
