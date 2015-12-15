using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Codestetic.Core.Domain.Common;
using Codestetic.Core.Domain.Documents;
//using Codestetic.Core.Domain.Forums;

namespace Codestetic.Core.Domain.Employees
{
    /// <summary>
    /// Represents a employee
    /// </summary>
    [DataContract]
    public partial class RegionalRepresentative : BaseEntity
    {
        private ICollection<EmployeeRole> _employeeRoles;
        private ICollection<Document> _document;
        private ICollection<RewardPointsHistory> _rewardPointsHistory;
        private ICollection<Address> _addresses;
       // private ICollection<ForumTopic> _forumTopics;
       // private ICollection<ForumPost> _forumPosts;

		/// <summary>
		/// Ctor
		/// </summary>
        public RegionalRepresentative()
        {
            this.RegionalRepresentativeGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the employee Guid
        /// </summary>
        [DataMember]
        public Guid RegionalRepresentativeGuid { get; set; }

		/// <summary>
		/// Gets or sets the username
		/// </summary>
        [DataMember]
        public string Username { get; set; }

		/// <summary>
		/// Gets or sets the email
		/// </summary>
        [DataMember]
        public string Email { get; set; }

		/// <summary>
		/// Gets or sets the password
		/// </summary>
        [DataMember]
        public string Password { get; set; }

		/// <summary>
		/// Gets or sets the password salt
		/// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets max count repair
        /// </summary>
        public int MaxCountRepair { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }
        
        #region Navigation properties

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<EmployeeRole> EmployeeRoles
        {
            get { return _employeeRoles ?? (_employeeRoles = new List<EmployeeRole>()); }
            protected set { _employeeRoles = value; }
        }

        /// <summary>
        /// Gets or sets orders
        /// </summary>
        public virtual ICollection<Document> Documents
        {
            get { return _document ?? (_document = new List<Document>()); }
            protected set { _document = value; }            
        }

        /// <summary>
        /// Gets or sets reward points history
        /// </summary>
        public virtual ICollection<RewardPointsHistory> RewardPointsHistory
        {
            get { return _rewardPointsHistory ?? (_rewardPointsHistory = new List<RewardPointsHistory>()); }
            protected set { _rewardPointsHistory = value; }            
        }

        /// <summary>
        /// Gets or sets customer addresses
        /// </summary>
        public virtual ICollection<Address> Addresses
        {
            get { return _addresses ?? (_addresses = new List<Address>()); }
            protected set { _addresses = value; }            
        }
        
        #endregion

        #region Reward points

        public void AddRewardPointsHistoryEntry(int points, string message = "",
            Document documentId = null, decimal usedAmount = 0M)
        {
            int newPointsBalance = this.GetRewardPointsBalance() + points;

            var rewardPointsHistory = new RewardPointsHistory()
            {
                Employee = this,
                TypeUserId = TypeUser.RegionalRepresentative,
                Amount = usedAmount,
                Message = message,
                CreatedOnUtc = DateTime.UtcNow,
                DocumentId = documentId,
            };

            this.RewardPointsHistory.Add(rewardPointsHistory);
        }

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        public int GetRewardPointsBalance()
        {
            int result = 0;
            if (this.RewardPointsHistory.Count > 0)
                result = this.RewardPointsHistory.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id).FirstOrDefault().PointsBalance;
            return result;
        }

        #endregion
    }
}
