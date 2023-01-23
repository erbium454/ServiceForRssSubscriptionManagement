using System.ComponentModel.DataAnnotations;

namespace ServiceForRssSubscriptionManagement.Models.DataModels
{
    public class User
    {
        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage ="Not Valid email")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password is required")]
        [MinLength(6, ErrorMessage ="Password is too short")]
        public string Password { get; set; }
    }
}
