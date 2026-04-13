using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MYCV.Domain.Enums;

namespace MYCV.Domain.Entities
{
    public class UserSubscription : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// Selected subscription package
        /// </summary>
        [Required]
        public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Monthly;

        /// <summary>
        /// Subscription start date
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Subscription expiry date
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Payment method like bKash / Nagad
        /// </summary>
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Payment transaction number
        /// </summary>
        [MaxLength(100)]
        public string? PaymentTransactionId { get; set; }

        /// <summary>
        /// Optional admin/user note
        /// </summary>
        [MaxLength(250)]
        public string? Remarks { get; set; }

        /// <summary>
        /// Auto check subscription expiry
        /// </summary>
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow > EndDate;

        /// <summary>
        /// Auto calculated amount by selected plan
        /// </summary>
        [NotMapped]
        public decimal Amount => GetAmountByPlan(Plan);

        /// <summary>
        /// Calculate EndDate from selected plan
        /// </summary>
        public void CalculateEndDate()
        {
            EndDate = Plan switch
            {
                SubscriptionPlan.Weekly => StartDate.AddDays(7),
                SubscriptionPlan.Monthly => StartDate.AddMonths(1),
                SubscriptionPlan.Quarterly => StartDate.AddMonths(3),
                SubscriptionPlan.HalfYearly => StartDate.AddMonths(6),
                SubscriptionPlan.Yearly => StartDate.AddYears(1),
                _ => StartDate.AddMonths(1)
            };
        }

        /// <summary>
        /// Get package price by selected plan
        /// </summary>
        public static decimal GetAmountByPlan(SubscriptionPlan plan)
        {
            return plan switch
            {
                SubscriptionPlan.Weekly => 50,
                SubscriptionPlan.Monthly => 200,
                SubscriptionPlan.Quarterly => 500,
                SubscriptionPlan.HalfYearly => 900,
                SubscriptionPlan.Yearly => 1700,
                _ => 200
            };
        }
    }
}