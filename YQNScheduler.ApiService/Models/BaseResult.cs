using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YQNScheduler.ApiService.Models
{
    public class BaseResult
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}