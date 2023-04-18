using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsuite.Services.Entity
{
    public class PaymentInformation
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string ValidId { get; set; }
        public string EmailAddress { get; set; }
        public  string Message { get; set; }
        public string PaymentAmount { get; set; }
        public string Card { get; set; }
    }
}
