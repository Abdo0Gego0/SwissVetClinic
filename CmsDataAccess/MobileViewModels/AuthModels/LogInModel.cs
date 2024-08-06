namespace CmsDataAccess.MobileViewModels.AuthModels
{
    public class LogInModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? fcm_token { get; set; }
        public string? LangCode { get; set; } = "en-US";

    }
}
