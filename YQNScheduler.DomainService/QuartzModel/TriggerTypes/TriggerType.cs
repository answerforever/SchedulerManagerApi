namespace YQNScheduler.DomainService.QuartzModel.TriggerTypes
{
    public class TriggerType
    {
        private readonly string _code;

        public TriggerType(string code)
        {
            _code = code;
        }

        public string Code
        {
            get { return _code; }
        }
    }
}