﻿using System;
using System.Web;

namespace Codestetic.Core.Events
{
	/// <summary>
	/// for Application_BeginRequest
	/// </summary>
	/// <remarks>codehint: sm-add</remarks>
	public class AppBeginRequestEvent
	{
		public HttpContext Context { get; set; }
	}
}
