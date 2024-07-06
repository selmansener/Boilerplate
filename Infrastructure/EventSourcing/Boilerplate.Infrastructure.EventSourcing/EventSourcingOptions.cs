using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boilerplate.Infrastructure.EventSourcing
{
    public class EventSourcingOptions
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string ElasticHostUrl { get; set; }

        public string CertificateFingerprint { get; set; }
    }
}
