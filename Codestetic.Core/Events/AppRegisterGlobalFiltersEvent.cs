using System;
using System.Web;
using System.Web.Mvc;

namespace Codestetic.Core.Events
{
	/// <summary>
	/// to register global filters in Application_Start
	/// </summary>
	/// <remarks>codehint: sm-add</remarks>
	public class AppRegisterGlobalFiltersEvent
	{
		public GlobalFilterCollection Filters { get; set; }
	}
}
