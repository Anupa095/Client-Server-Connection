using System.ComponentModel.DataAnnotations;

namespace ProductServiceAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
    }
}
