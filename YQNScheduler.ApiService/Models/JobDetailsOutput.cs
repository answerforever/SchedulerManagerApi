namespace YQNScheduler.ApiService.Models
{
    public class JobDetailsOutput : CommandResultWithErrorDetails
    {
        public Property[] JobDataMap { get; set; }

        public Property[] JobProperties { get; set; }
    }
}