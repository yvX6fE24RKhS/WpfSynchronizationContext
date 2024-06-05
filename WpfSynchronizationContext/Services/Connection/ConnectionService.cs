using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSynchronizationContext.Services.Connection
{
   internal class ConnectionService : IConnectionService
   {
      private CancellationTokenSource _cancelTokenSource = new();
      private Task? _connection;

      public event Action? Disconnected;
      public bool IsConnected => !_connection?.IsCompleted ?? false;
      public void Connect()
      {
         _cancelTokenSource = new();

         _connection = Task.Factory.StartNew
            (
               () => Thread.Sleep(12000),
               _cancelTokenSource.Token
            );

         _connection = _connection.ContinueWith
         (
            (antecendent) => Disconnected?.Invoke(), 
            TaskContinuationOptions.OnlyOnRanToCompletion
            & TaskContinuationOptions.OnlyOnFaulted
         );

      }
      public void Disconnect()
      {
         _cancelTokenSource.Cancel();
         Thread.Sleep(100);
         _cancelTokenSource.Dispose();
      }

   }
}


