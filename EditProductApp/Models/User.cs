using System.ComponentModel.DataAnnotations;

namespace EditProductApp
{
    public class User
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Please enter valid name")]
        [MaxLength(10, ErrorMessage = "Name max length is 10 symbols")]
        public string Name { get; set; }
    }
}