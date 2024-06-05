// Ignore Spelling: Wsc
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfSynchronizationContext.Services.Connection;
using WpfSynchronizationContext.ViewModels.Input;

namespace WpfSynchronizationContext.ViewModels
{
   internal sealed partial class MainViewModel(IConnectionService connectionService) : ObservableValidator
   {
      private readonly IConnectionService? _connectionService = connectionService;

      [ObservableProperty]
      [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
      [NotifyCanExecuteChangedFor(nameof(DisconnectCommand))]
      private bool _isConnected = false;

      #region Connect

      private IRelayCommand? _connectCommand;

      public IRelayCommand ConnectCommand => _connectCommand ??= new RelayCommand(Connect, CanConnect);

      private bool CanConnect() => !IsConnected;

      private void Connect()
      {
#if DEBUG
         Debug.WriteLine(message: $"Отладка: Connect Command executing.");
#endif
         _connectionService?.Connect();
         IsConnected = _connectionService?.IsConnected ?? false;
         if (IsConnected) _connectionService!.Disconnected += OnDisconnected;
      }

      #endregion Connect

      #region Connect With Synchronization Context

      //private RelayCommandSyncContext? _connectCommand;
      private IRelayCommand? _connectWscCommand;

      public IRelayCommand ConnectWscCommand
         => _connectWscCommand ??= new RelayCommandSyncContext(ConnectWsc, CanConnectWsc);

      private bool CanConnectWsc(object? parameter) => !IsConnected;

      private void ConnectWsc(object? parameter)
      {
#if DEBUG
         Debug.WriteLine(message: $"Отладка: Connect Command executing.");
#endif
         _connectionService?.Connect();
         IsConnected = _connectionService?.IsConnected ?? false;
         //DisconnectCommand.NotifyCanExecuteChanged();
         if (IsConnected) _connectionService!.Disconnected += OnDisconnected;
      }

      #endregion Connect With Synchronization Context

      #region Disconnect

      private IRelayCommand? _disconnectCommand;

      public IRelayCommand DisconnectCommand => _disconnectCommand ??= new RelayCommand(Disconnect, CanDisconnect);

      private bool CanDisconnect() => IsConnected;

      /// <summary> Метод отключения от торговой системы Quik. </summary>
      private void Disconnect()
      {
#if DEBUG
         Debug.WriteLine(message: $"Отладка: Disconnect Command executing.");
#endif
         _connectionService!.Disconnected -= OnDisconnected;
         _connectionService?.Disconnect();
         IsConnected = false;
      }

      #endregion Disconnect

      #region Disconnect With Synchronization Context

      private IRelayCommand? _disconnectWscCommand;

      //      public IRelayCommand ConnectCommand => _connectCommand ??= new RelayCommandSyncContext(Connect, CanConnect());
      public IRelayCommand DisconnectWscCommand
         => _disconnectWscCommand ??= new RelayCommandSyncContext(DisconnectWsc, CanDisconnectWsc);

      private bool CanDisconnectWsc(object? parameter) => IsConnected;

      /// <summary> Метод отключения от торговой системы Quik. </summary>
      private void DisconnectWsc(object? parameter)
      {
#if DEBUG
         Debug.WriteLine(message: $"Отладка: Disconnect Command executing.");
#endif
         _connectionService!.Disconnected -= OnDisconnected;
         _connectionService?.Disconnect();
         IsConnected = false;
      }

      #endregion Disconnect With Synchronization Context

      private void OnDisconnected()
      {
#if DEBUG
         Debug.WriteLine(message: $"Отладка: Event Handler executing.");
#endif
         IsConnected = false;
      }
   }
}
