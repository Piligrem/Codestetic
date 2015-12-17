using System;
using Codestetic.Core.Configuration;
using Codestetic.Core.Domain.Common;

namespace Codestetic.Core.Domain.Employees
{
    public class RewardPointsSettings : BaseEntity, ISettings
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether Reward Points Program is enabled
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets a value category repair Id
        /// </summary>
        public int CategoryRepairId { get; set; }
        
        /// <summary>
        /// Gets or sets value recipient type Id 
        /// </summary>
        public int UserTypeId { get; set; }
        public UserType UserType
        {
            get { return (UserType)UserTypeId; }
            set { UserTypeId = (int)value; }
        }

        /// <summary>
        /// Gets or sets price of reward
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///  Gets or sets ratio rewards for region represetative
        /// </summary>
        public double Ratio { get; set; }
        #endregion Properties

        #region Navigation properties
        public virtual CategoryRepair CategoryRepairId { get; set; }
        #endregion Navigation properties
    }
}