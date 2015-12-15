using System;
using Codestetic.Core.Domain.Document;

namespace Codestetic.Core.Domain.Employees
{
    /// <summary>
    /// Represents a reward point history entry
    /// </summary>
    public partial class RewardPointsHistory : BaseEntity
    {
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
        /// Gets or sets the order for which points were redeemed as a payment
        /// </summary>
        public virtual Document DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Employee Employee { get; set; }
    }
}
