using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public enum OrderStatus
    {
        Ordered = 1,
        Processed = 2,
        Completed = 3
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime OrderedOn { get; set; }

        [Required]
        public int UserId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public decimal OrderTotal { get; set; }

        public ICollection<OrderItems> OrderItems { get; set; }
    }
}
