using System.Collections.Generic;

namespace Codestetic.Core.Domain.Documents
{
    /// <summary>
    /// Represents an document details
    /// </summary>
    public class DocumentDetails : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the document identifier
        /// </summary>
        public int DocumentId { get; set; }

        /// <summary>
        /// Gets or sets document details (string for XML Serialization)
        /// </summary>
        public string Details { get; set; }
        #endregion Properties

        #region Navigation properties
        /// <summary>
        /// Gets or sets the document
        /// </summary>
        public virtual Document Document { get; set; }
        #endregion Navigation properties

        #region Custom properties
        /// <summary>
        /// Gets or sets document details fields
        /// </summary>
        public virtual IList<Field> Fields { get; set; } // ???
        #endregion Custom properties
    }
}
