using Codestetic.Core.Domain.Warehouses;
using System;

namespace Codestetic.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a reward point history entry
    /// </summary>
    public partial class RemainsProduct : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the employee identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets warehouse id
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// Gets or sets an product quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets an product quality identifier
        /// </summary>
        public int QualityId { get; set; }
        public ProductQuality Quality
        {
            get { return (ProductQuality)QualityId; }
            set { QualityId = (int)value; }
        }
        #endregion Properties

        #region Navigation propeties
        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        #endregion Navigation propeties
    }
}
