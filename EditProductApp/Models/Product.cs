using System;
using System.ComponentModel.DataAnnotations;

namespace EditProductApp
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Please enter valid name")]
        [MaxLength(100, ErrorMessage = "Name max length is 100 symbols")]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ProductUpdateTimeStamp { get; set; }
    }
}