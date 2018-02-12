namespace YQNScheduler.ApiService.Models
{

    public class CommandResultWithErrorDetails : BaseResult
    {
        public Property[] ErrorDetails { get; set; }
    }
}