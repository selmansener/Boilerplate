﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boilerplate.Infrastructure.EventBusRabbitMQ
{
    public class EventBusConfig
    {
        public string ConnectionString { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int RetryCount { get; set; }

        public string SubscriptionClientName { get; set; }
    }
}
