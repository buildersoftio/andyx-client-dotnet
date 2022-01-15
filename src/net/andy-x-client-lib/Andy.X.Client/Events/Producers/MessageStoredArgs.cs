using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andy.X.Client.Events.Producers
{
    public class MessageStoredArgs
    {
        public Guid Id { get; set; }
        public DateTime StoredDate { get; set; }
    }
}
