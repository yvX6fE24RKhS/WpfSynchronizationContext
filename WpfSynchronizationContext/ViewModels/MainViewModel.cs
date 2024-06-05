using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

      //private RelayCommandSyncContext? _connectCommand;
      private RelayCommand? _connectCommand;

      public IRelayCommand ConnectCommand => _connectCommand ??= new RelayCommand(Connect, CanConnect);

      private bool CanConnect() => !IsConnected;

      private void Connect()
      {
#if DEBUG
         Debug.WriteLine(message: $"Отладка: ConnectCommand executing.");
#endif
         _connectionService?.Connect();
         IsConnected = _connectionService?.IsConnected ?? false;
         //DisconnectCommand.NotifyCanExecuteChanged();
         if (IsConnected) _connectionService!.Disconnected += OnDisconnected;
      }

      #endregion Connect

      #region Disconnect

      private RelayCommand? _disconnectCommand;

      //      public IRelayCommand ConnectCommand => _connectCommand ??= new RelayCommandSyncContext(Connect, CanConnect());
      public IRelayCommand DisconnectCommand => _disconnectCommand ??= new RelayCommand(Disconnect, CanDisconnect);

      private bool CanDisconnect() => IsConnected;

      /// <summary> Метод отключения от торговой системы Quik. </summary>
      private void Disconnect()
      {
#if DEBUG
         Debug.WriteLine(message: $"Отладка: DisconnectCommand executing.");
#endif
         _connectionService!.Disconnected -= OnDisconnected;
         _connectionService?.Disconnect();
         IsConnected = false;
      }

      #endregion Disconnect

      private void OnDisconnected()
      {
#if DEBUG
         Debug.WriteLine(message: $"Отладка: Event Handler executing.");
#endif
         IsConnected = false;
      }
   }
}
