namespace WpfSynchronizationContext.ViewModels.Input
{
   /// <summary>
   /// 
   /// </summary>
   /// <param name="owner"></param>
   /// <remarks> См. <see href="https://www.cyberforum.ru/csharp-net/thread2947941.html#post16080545">Элд Хасп, "Исполнение делегатов обобщённого типа"</see>. </remarks>

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
