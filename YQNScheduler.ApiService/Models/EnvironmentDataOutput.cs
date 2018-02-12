﻿namespace YQNScheduler.ApiService.Models
{
    public class EnvironmentDataOutput : CommandResultWithErrorDetails
    {
        public string SelfVersion { get; set; }

        public string QuartzVersion { get; set; }

        public string DotNetVersion { get; set; }

        public string CustomCssUrl { get; set; }
    }
}