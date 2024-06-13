using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceFetcher.Business.DTOs
{
    public class DQBResultDTO<T>
    {
        public IEnumerable<T> Data { get; set; }

        public int Count { get; set; }
    }
}
