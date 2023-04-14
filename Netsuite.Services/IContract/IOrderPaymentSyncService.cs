using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsuite.Services.IContract
{
    public interface IOrderPaymentSyncService
    {
        Task GetPaymentMessage();
    }
}
