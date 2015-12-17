using System.Collections.Generic;
using Codestetic.Core.Domain.Localization;

namespace Codestetic.Core.Domain.Directory
{
    /// <summary>
    /// Represents a Region
    /// </summary>
    public partial class Region : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the numeric code
        /// </summary>
        public int NumericCode { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
