﻿using System;
using System.Web;

namespace Codestetic.Core.Events
{
	/// <summary>
	/// for Application_EndRequest
	/// </summary>
	/// <remarks>codehint: sm-add</remarks>
	public class AppEndRequestEvent
	{
		public HttpContext Context { get; set; }
	}
}
