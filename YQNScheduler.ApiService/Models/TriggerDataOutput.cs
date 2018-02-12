using YQNScheduler.DomainService.QuartzModel;

namespace YQNScheduler.ApiService.Models
{

    public class TriggerDataOutput : CommandResultWithErrorDetails
    {
         public TriggerData Trigger { get; set; }
    }
}