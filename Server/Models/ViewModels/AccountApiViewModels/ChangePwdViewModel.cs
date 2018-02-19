using System.ComponentModel.DataAnnotations;

namespace netcore.Models.ViewModels.AccountApiViewModels
{
    public class ChangePwdViewModel
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}