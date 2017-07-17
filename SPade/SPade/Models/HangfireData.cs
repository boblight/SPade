using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPade.ViewModels.Student;

namespace SPade.Models
{
    public class HangfireData
    {
        //this object is for the data that is retreived from the Hangfire State column
        public DateTime SucceededAt { get; set; }
        public int PerformanceDuration { get; set; }
        public int Latency { get; set; }
        public int Result { get; set; }
    }
}