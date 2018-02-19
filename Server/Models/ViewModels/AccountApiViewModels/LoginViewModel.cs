using System.ComponentModel.DataAnnotations;

namespace netcore.Models.ViewModels.AccountApiViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UserNameOrEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}