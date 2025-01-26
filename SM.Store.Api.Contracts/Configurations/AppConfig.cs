using System;
using System.Collections.Generic;
using System.Text;

namespace SM.Store.Api.Contracts.Configurations
{
    public class AppConfig
    {
        public string TestConfig1 { get; set; }
        public bool UseInMemoryDatabase { get; set; }
    }
}
