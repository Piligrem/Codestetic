using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Codestetic.Core.Domain.Catalog
{
    [DataContract]
    class ProductGroup : BaseEntity
    {

        private ICollection<Product> _product;
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        #region Navigation properties

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual ICollection<Product> Products
        {
            get { return _product ?? (_product = new List<Product>()); }
            protected set { _product = value; }
        }
        #endregion
    }
}
