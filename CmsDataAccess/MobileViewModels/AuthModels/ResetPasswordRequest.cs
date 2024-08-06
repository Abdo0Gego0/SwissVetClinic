using System.ComponentModel.DataAnnotations;

namespace CmsDataAccess.MobileViewModels.AuthModels
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required, MinLength(8, ErrorMessage = "Please enter at least 8 characters, dude!")]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
