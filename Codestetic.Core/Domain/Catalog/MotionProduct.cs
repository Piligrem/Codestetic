using System;
using Codestetic.Core.Domain.Documents;

namespace Codestetic.Core.Domain.Catalog
{
    /// <summary>
    /// Represents product motion
    /// </summary>
    public partial class MotionProduct : BaseEntity
    {
        /// <summary>
        /// Gets or sets the employee identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets an product quality identifier
        /// </summary>
        public int QualityId { get; set; }

         /// <summary>
        /// Gets or sets an product quantity
        /// </summary>
        public int Quantity { get; set; }

         /// <summary>
        /// Gets or sets warehouse id
        /// </summary>
        public int WarehouseId { get; set; }

                  /// <summary>
        /// Gets or sets the Document Id
        /// </summary>
        public int DocumentId { get; set; }       

        /// <summary>
        /// Gets or sets an document type id
        /// </summary>
        public int DocumentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the document
        /// </summary>
        public virtual Document Document { get; set; }
    }
    }
}



