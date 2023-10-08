using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProductManagerAPI.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }  
        public string? Name { get; set; }  
        public string? Description { get; set; }
        public string? ImageName { get; set; }
        public double Price { get; set; }   
        public bool InStock { get; set; }  
        public int Quantity { get; set; }  
        public long CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public virtual Category? Categories { get; set; }

        [NotMapped]
        public byte[]? ImageData{ get; set; } 
    }
}
