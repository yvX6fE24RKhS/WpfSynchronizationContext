using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSynchronizationContext.Services.Connection
{
   internal interface IConnectionService
   {
      event Action? Disconnected;

      bool IsConnected { get; }

      void Connect();

      void Disconnect();
   }
}
