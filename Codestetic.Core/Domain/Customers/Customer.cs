using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Codestetic.Core.Domain.Common;
using Codestetic.Core.Domain.Catalog;

namespace Codestetic.Core.Domain.Customers
{
    [DataContract]
    public partial class Customer : BaseEntity, ISoftDeletable
    {
        #region Fields
        private ICollection<Address> _addresses;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets BrandId
        /// </summary>
        [DataMember]
        public int BrandId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime? CreatedOnUtc { get; set; }
        #endregion Properties

        #region Navigation properties
        /// <summary>
        /// Gets or sets customer addresses
        /// </summary>
        public virtual ICollection<Address> Addresses
        {
            get { return _addresses ?? (_addresses = new List<Address>()); }
            protected set { _addresses = value; }
        }
        public virtual Brand Brand { get; set; }
        #endregion
    }
}
