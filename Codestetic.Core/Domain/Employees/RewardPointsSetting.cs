using Codestetic.Core.Configuration;
using Codestetic.Core.Domain.Document;

namespace Codestetic.Core.Domain.Employees
{
    public class RewardPointsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether Reward Points Program is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets amount of reward
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value category repair Id
        /// </summary>
        public int CategoryRepairId { get; set; }

        /// <summary>
        /// Gets or sets amount of reward
        /// </summary>
        public int RecipientTypeId { get; set; }

        /// <summary>
        /// Points are awarded when the document status is
        /// </summary>
        public DocumentStatus PointsForPurchases_Awarded { get; set; }

        /// <summary>
        /// Points are canceled when the document is
        /// </summary>
        public DocumentStatus PointsForPurchases_Canceled { get; set; }
    }
}