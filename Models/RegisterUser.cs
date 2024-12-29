using System.Drawing;

namespace WhatsApp_Clone.Models
{
    public class RegisterUser
    {
        public string? Name { get; set; }
        public string Email { get; set; }
        public string? About { get; set; }
        public bool? ImageChanged { get; set; }
        public IFormFile? Image { get; set; }


    }
}

