using System;
using System.Collections.Generic;
using Codestetic.Core.Domain.Directory;
using Codestetic.Core.Domain.Employees;

namespace Codestetic.Core.Domain.Documents
{
    public class Document : BaseEntity
    {
        #region Fields
        private ICollection<DocumentDetails> _documentDetails;
        #endregion Fields

        #region Constructors
        public Document()
        {
            Date = DateTime.Now;
            Status = DocumentStatus.Pending;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the (formatted) document number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the (formatted) document date and time
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the document type id 
        /// </summary>
        public int DocumentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the document type id 
        /// </summary>
        public int BaseDocumentId { get; set; }

        /// <summary>
        /// Gets or sets a value Region identifier (City, Provance)
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// Gets or sets a value Author identifier
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// Gets or sets a value Comment text
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets a value last update document in format UTC
        /// </summary>
        public DateTime? UpdatedOnUtc { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the entity has been deleted (status document - Active, Deleted)
        /// </summary>
        public int StatusId { get; set; }
        #endregion Properties

        #region Navigation properties
        public virtual string DocumentType { get; set; }

        /// <summary>
        /// Gets or sets a value base document
        /// </summary>
        public virtual object BaseDocument { get; set; } // ???

        /// <summary>
        /// Gets or sets value DocumentDetails
        /// </summary>
        public virtual ICollection<DocumentDetails> DocumentDetails
        {
            get { return _documentDetails ?? (_documentDetails = new List<DocumentDetails>()); }
            protected set { _documentDetails = value; }
        }
        public virtual Region Region { get; set; }
        public virtual Employee Author { get; set; }
        #endregion Navigation properties

        #region Custom properties
        /// <summary>
        /// Gets or sets the order status
        /// </summary>
        public virtual DocumentStatus Status
        {
            get { return (DocumentStatus)this.StatusId; }
            set { this.StatusId = (int)value; }
        }
        #endregion Custom properties
    }
}
