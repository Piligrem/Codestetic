using System;
using Codestetic.Core.Configuration;

namespace Codestetic.Core.Domain.Employees
{
    public class RewardPointsSettings : BaseEntity, ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether Reward Points Program is enabled
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets a value category repair Id
        /// </summary>
        public int CategoryRepairId { get; set; }
        
        /// <summary>
        /// Gets or sets value recipient type Id 
        /// </summary>
        public int RecipientTypeId { get; set; }

        /// <summary>
        /// Gets or sets price of reward
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///  Gets or sets ratio rewards for region represetative
        /// </summary>
        public double Ratio { get; set; }

    }
}