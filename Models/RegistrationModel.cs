using Microsoft.AspNetCore.Http;

namespace Registration_MVC.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DOB { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public IFormFile? ProfilePic { get; set; }
    }
}