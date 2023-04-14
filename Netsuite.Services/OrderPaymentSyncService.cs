using Microsoft.Extensions.Options;
using Netsuite.Core;
using Netsuite.Services.IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netsuite.Services
{
    public class OrderPaymentSyncService : IOrderPaymentSyncService
    {
        private readonly AppSettings _appSettings;

        public OrderPaymentSyncService(IOptions<AppSettings> settings)
        {
            _appSettings = settings.Value;
        }

        public async Task GetPaymentMessage()
        {
            await Task.Run(() => Console.WriteLine(_appSettings.Message));
        }
    }
}
