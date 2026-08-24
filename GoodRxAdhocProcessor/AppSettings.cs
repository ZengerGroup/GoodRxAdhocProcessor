using System;
using System.Collections.Generic;
using System.Text;

namespace GoodRxAdhocProcessor
{
    internal class AppSettings
    {
        public string AdhocPath { get; set; }
        public string SheetName { get; set; } = "Adhocs";
        public string PricingSheet { get; set; } = "Pricing Tab";
        public string FtpPath { get; set;  }
        public string DefaultPath { get; set; }
    }
}
