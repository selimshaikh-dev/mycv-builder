using MYCV.Domain.Entities;
using MYCV.Domain.Enums;

namespace MYCV.Application.DTOs
{
    /// <summary>
    /// DTO for user subscription information
    /// </summary>
    public class UserSubscriptionDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// Selected subscription package
        /// </summary>
        public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Monthly;

        /// <summary>
        /// Subscription start date
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Subscription expiry date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Payment method used by user
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Payment transaction number
        /// </summary>
        public string? PaymentTransactionId { get; set; }

        /// <summary>
        /// Optional user/admin remarks
        /// </summary>
        public string? Remarks { get; set; }

        /// <summary>
        /// Active flag from BaseEntity
        /// Important for subscription validation
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Runtime expiry check
        /// </summary>
        public bool IsExpired => DateTime.UtcNow > EndDate;

        /// <summary>
        /// Runtime plan amount
        /// </summary>
        public decimal Amount => UserSubscription.GetAmountByPlan(Plan);
    }
}