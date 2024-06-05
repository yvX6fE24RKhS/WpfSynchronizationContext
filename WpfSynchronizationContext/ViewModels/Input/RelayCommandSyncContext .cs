using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace WpfSynchronizationContext.ViewModels.Input
{
   /// <summary>
   /// 
   /// </summary>
   /// <remarks> См. <see href="https://www.cyberforum.ru/csharp-net/thread2947941.html#post16080545">Элд Хасп, "Исполнение делегатов обобщённого типа"</see>. </remarks>
   public class RelayCommandSyncContext : ICommand, IRelayCommand
   {
      #region Fields

      //original//private readonly CanExecuteHandler<object> canExecute;
      private readonly Predicate<object> _canExecute;
      //original//private readonly ExecuteHandler<object> execute;
      private readonly Action<object> _execute;
      private readonly EventHandlerSyncContext _canExecuteChangedSyncContext;

      #endregion Fields

      #region Constructors

      /// <summary>Конструктор команды.</summary>
      /// <param name="execute">Выполняемый метод команды.</param>
      /// <param name="canExecute">Метод, возвращающий состояние команды.</param>
      //original//public RelayCommandSyncContext(ExecuteHandler<object> execute, CanExecuteHandler<object> canExecute = null)
      public RelayCommandSyncContext(Action<object> execute, Predicate<object> canExecute = null!)
         : this()
      {
         //original//this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
         _execute = execute ?? throw new ArgumentNullException(nameof(execute));
         //original//this.canExecute = canExecute;
         _canExecute = canExecute;
      }

      /// <inheritdoc cref="RelayCommand(ExecuteHandler, CanExecuteHandler)"/>
      //original//public RelayCommandSyncContext(ExecuteHandler execute, CanExecuteHandler canExecute = null)
      public RelayCommandSyncContext(Action execute, Func<bool>? canExecute = null)
         : this
         (
            p => execute(),
            p => canExecute?.Invoke() ?? true
         )
      { }

      //original//protected RelayCommandSyncContext()
      //original//{
      //original//   CanExecuteChangedSyncContext = new EventHandlerSyncContext(this);
      //original//}
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
      private protected RelayCommandSyncContext() =>_canExecuteChangedSyncContext = new EventHandlerSyncContext(this);
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

      #endregion Constructors

      #region Events

      //original//public event EventHandler CanExecuteChanged
      /// <summary>  </summary>
      public event EventHandler? CanExecuteChanged
      {
         //original//add => CanExecuteChangedSyncContext.Add(value);
         add => _canExecuteChangedSyncContext.Add(value);
         //original//remove => CanExecuteChangedSyncContext.Remove(value);
         remove => _canExecuteChangedSyncContext.Remove(value);
      }

      #endregion Events

      #region Implementation of ICommand

      /// <summary>Вызов метода, возвращающего состояние команды.</summary>
      /// <param name="parameter">Параметр команды.</param>
      /// <returns><see langword="true"/> - если выполнение команды разрешено.</returns>
      //original//public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;
      public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter!) ?? true;

      /// <summary>Вызов выполняющего метода команды.</summary>
      /// <param name="parameter">Параметр команды.</param>
      //original//public void Execute(object parameter) => execute?.Invoke(parameter);
      public void Execute(object? parameter) => _execute?.Invoke(parameter!);

      #endregion Implementation of ICommand

      #region Implementation of IRelayCommand

      /// <summary> Метод, подымающий событие <see cref="CanExecuteChanged"/>. </summary>
      public void NotifyCanExecuteChanged()
      {
         _canExecuteChangedSyncContext?.Invoke();
      }

      #endregion Implementation of IRelayCommand

      #region Properties

      #endregion Properties

      #region Methods

      //original//private class EventHandlerItem
      //original//{
      //original//   public EventHandler EventsField;
      //original//}

      //original///// <summary>Метод, подымающий событие <see cref="CanExecuteChanged"/>.</summary>
      //original//public void RaiseCanExecuteChanged()
      //original//{
      //original//CanExecuteChangedSyncContext.Invoke();
      //original//}

      #endregion Methods
   }

}