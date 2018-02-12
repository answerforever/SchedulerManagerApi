namespace YQNScheduler.ApiService.Models
{
    public class JobInput : GroupInput
    {
         public string Job { get; set; }

        public string Description { get; set; }

        public string ClassName { get; set; }

        public string NameSpace { get; set; }

        public string RequestUrl { get; set; }

        public string Parameters { get; set; }
    }
}