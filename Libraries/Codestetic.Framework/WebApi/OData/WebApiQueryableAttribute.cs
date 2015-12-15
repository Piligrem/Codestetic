﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.OData;

namespace Specter.Web.Framework.WebApi.OData
{
    public class WebApiQueryableAttribute : EnableQueryAttribute //QueryableAttribute
	{
		public bool PagingOptional { get; set; }

		protected virtual bool MissingClientPaging(HttpActionExecutedContext actionExecutedContext)
		{
			if (PagingOptional)
				return false;

			try
			{
				var responseContent = actionExecutedContext.Response.Content as ObjectContent;
				bool singleResult = (responseContent != null && responseContent.Value is SingleResult);

				if (singleResult)
					return false;

				var query = actionExecutedContext.Request.RequestUri.Query;

				bool missingClientPaging = query.IsNullOrEmpty() || !query.Contains("$top=");

				if (missingClientPaging)
				{
					actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
						"Missing client paging. Please specify odata $top query option. Maximum value is {0}.".FormatWith(WebApiGlobal.MaxTop));

					return true;
				}
			}
			catch (Exception exc)
			{
				exc.Dump();
			}
			return false;
		}

		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			if (MissingClientPaging(actionExecutedContext))
				return;

			base.OnActionExecuted(actionExecutedContext);
		}
	}
}
