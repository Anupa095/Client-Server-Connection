using System;
using System.ComponentModel.DataAnnotations;

namespace OrderServiceAPI.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
