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
