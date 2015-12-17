using System;
using Codestetic.Core.Domain.Documents;
using Codestetic.Core.Domain.Common;

namespace Codestetic.Core.Domain.Employees
{
    /// <summary>
    /// Represents a reward point history entry
    /// </summary>
    public partial class RewardPointsHistory : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the employee identifier
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets type of user id
        /// </summary>
        public int TypeUserId { get; set; }

        /// <summary>
        /// Gets or sets the order for which points were redeemed as a payment
        /// </summary>
        public virtual int DocumentId { get; set; }
        #endregion Properties

        #region Navigation properties
        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Employee Employee { get; set; }
        public virtual UserType TypeUser { get; set; }
        public virtual Document Document { get; set; }
        #endregion Navigation properties
    }
}
