using System;
using System.Collections.Generic;
using System.Text;

namespace OfflineSyncingExample.Models
{
    public class Contact
    {
        public string Id { get; set; }        public DateTimeOffset? UpdatedAt { get; set; }        public DateTimeOffset? CreatedAt { get; set; }        public string Version { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }
    }
}
