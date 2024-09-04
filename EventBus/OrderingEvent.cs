using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public class OrderingEvent : IntegrationEvent
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
