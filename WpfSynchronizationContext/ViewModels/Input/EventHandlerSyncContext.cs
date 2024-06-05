using System;

namespace WpfSynchronizationContext.ViewModels.Input
{
   /// <summary>
   /// 
   /// </summary>
   /// <param name="owner"></param>
   public class EventHandlerSyncContext(object owner) : EventSyncContext<EventHandler>(owner)
   {
      /// <summary> Вызывает обработчик события. </summary>
      public void Invoke()
      {
         object owner = Owner;
         Execute(handler => () => handler(owner, EventArgs.Empty));
      }
   }
}
