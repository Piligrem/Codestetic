using Codestetic.Core.Domain.Employees;
using Codestetic.Core.Domain.Directory;
using Codestetic.Core.Domain.Localization;

namespace Codestetic.Core
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the current employee
        /// </summary>
        Employee CurrentEmployee { get; set; }

        /// <summary>
        /// Gets or sets the original employee (in case the current one is impersonated)
        /// </summary>
        Employee OriginalEmployeeIfImpersonated { get; }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }

        /// <summary>
        /// Gets a value indicating whether a language exists and is published within a store's scope.
        /// </summary>
        /// <param name="seoCode">The unique seo code of the language to check for</param>
        /// <param name="storeId">The store id (will be resolved internally when 0)</param>
        /// <returns>Whether the language exists and is published</returns>
        bool IsPublishedLanguage(string seoCode, int storeId = 0);

        /// <summary>
        /// Gets the default (fallback) language for a store
        /// </summary>
        /// <param name="storeId">The store id (will be resolved internally when 0)</param>
        /// <returns>The unique seo code of the language to check for</returns>
        string GetDefaultLanguageSeoCode(int storeId = 0);

         /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }

        ///// <summary>
        ///// Get or set a value indicating whether we're in the public shop
        ///// </summary>
        //bool IsPublic { get; set; }
    }
}
