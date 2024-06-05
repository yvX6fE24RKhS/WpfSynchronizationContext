using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WpfSynchronizationContext.ViewModels.Input
{
   /// <summary> Контекст синхронизации потоков события. </summary>
   /// <typeparam name="THandler"> Делегат, определяющий метод обработки события. </typeparam>
   /// <remarks> См. <see href="https://www.cyberforum.ru/csharp-net/thread2947941.html#post16080545">Элд Хасп, "Исполнение делегатов обобщённого типа"</see>. </remarks>
   public class EventSyncContext<THandler> where THandler : notnull, Delegate
   {
      #region Fields

      /// <summary> Коллекция обработчиков из потоков с Контекстом Синхронизации. </summary>
      private readonly Dictionary<SynchronizationContext, THandler> contextHandlers = [];

      /// <summary> Словарь отношений обработчик-контекст синхронизации. </summary>
      private readonly Dictionary<THandler, (SynchronizationContext context, int count)> contexts = [];

      /// <summary>  </summary>
      private readonly object lockEvent = new();

      /// <summary> Список обработчиков безразличных к потоку. </summary>
      private THandler noContextHandlers = null!;

      #endregion Fields

      #region Constructors

      /// <summary> Инициализирует экземпляр класса <see cref="EventSyncContext"/> </summary>
      /// <param name="owner">  </param>
      protected EventSyncContext(object owner) => Owner = owner;

      #endregion Constructors

      #region Properties

      /// <summary>  </summary>
      public object Owner { get; }

      #endregion Properties

      #region Methods

      /// <summary> Добавляет обработчик события в список, связанный с контекстом. </summary>
      /// <param name="handler"> Обработчик события. </param>
      public void Add(THandler? handler)
      {
         // Если обработчика нет - выход без выкидывания исключения.
         if (handler == null)
            return;

         // Блокировка на время изменения коллекций
         lock (lockEvent)
         {
            // Контекст синхронизации текущего потока.
            SynchronizationContext? sc = SynchronizationContext.Current;

            // Если ранее был такой же обработчик.
            if (contexts.TryGetValue(handler, out (SynchronizationContext context, int count) contextCount))
            {
               // Если ранее он был из потока без Контекста.
               if (contextCount.context == null)
               {
                  // То Контекст для этого обработчика не нужен.
                  sc = null;
               }

               // Если ранее он поступал из потока с другим Контекстом.
               else if (sc != contextCount.context)
               {
                  // То Контекст для этого обработчика не нужен,
                  // так же нужно переписать все его предыдущие вхождения
                  // в список обработчиков без контекста.
                  THandler list = contextHandlers[contextCount.context];
                  list = (THandler)Delegate.RemoveAll(list, handler)!;
                  // Если в списке связанным с Контекстом больше нет обработчиков,
                  // то Контекст удаляется из словаря.
                  if (list == null)
                  {
                     contextHandlers.Remove(contextCount.context);
                  }

                  // Иначе - запоминается
                  else
                  {
                     contextHandlers[contextCount.context] = list;
                  }

                  // Добавление удалённых обработчиков в список без Контекста.
                  noContextHandlers = (THandler)Delegate.Combine(Enumerable.Repeat(handler, contextCount.count)
                                                                                    .Append(noContextHandlers)
                                                                                    .ToArray())!;

                  // Сброс Контекста добавляемого обработчика.
                  contextCount.context = sc = null!;
               }

               // Увеличение счётчика одинаковых обработчиков и его запоминание.
               contextCount.count++;
               contexts[handler] = contextCount;

               // Если Контекста нет (или он не важен),
               // то запись в список обработчиков без Контекста.
               if (sc == null)
               {
                  noContextHandlers = (THandler)Delegate.Combine(noContextHandlers, handler);
               }

               // Иначе запись в список обработчиков текущего Контекста.
               else
               {
                  contextHandlers[sc] = (THandler)Delegate.Combine(contextHandlers[sc], handler);
               }
            }

            // Если этот обработчик приходит первый раз.
            else
            {
               // Создание и запоминание его Контекста и Количества.
               contextCount = (sc!, 1);
               contexts[handler] = contextCount;

               // Если Контекста нет (или он не важен),
               // то запись в список обработчиков без Контекста.
               if (sc == null)
               {
                  noContextHandlers = (THandler)Delegate.Combine(noContextHandlers, handler);
               }

               // Иначе запись в список обработчиков текущего Контекста.
               else
               {
                  // Если этот Контекст ранее был,
                  // то добавление в список для него.
                  if (contextHandlers.TryGetValue(sc, out THandler? list))
                  {
                     contextHandlers[sc] = (THandler)Delegate.Combine(list, handler);
                  }

                  // Иначе просто добавление обработчика в словарь.
                  else
                  {
                     contextHandlers[sc] = handler;
                  }
               }
            }
         }

      }

      /// <summary> Удаляет обработчик события из списка, связанного с контекстом. </summary>
      /// <param name="handler"> Обработчик события. </param>
      public void Remove(THandler? handler)
      {
         // Если обработчика нет - выход без выкидывания исключения.
         if (handler == null)
            return;

         // Блокировка на время изменения коллекций
         lock (lockEvent)
         {
            // Если ранее был этот обработчик.
            if (contexts.TryGetValue(handler, out (SynchronizationContext context, int count) contextCount))
            {
               // Уменьшение счётчика одинаковых обработчиков
               // и его запоминание или очистка словаря.
               contextCount.count--;
               if (contextCount.count >= 0)
               {
                  contexts[handler] = contextCount;
               }
               else
               {
                  contexts.Remove(handler);
               }

               // Если Контекста нет,
               // то удаление из списка обработчиков без Контекста.
               if (contextCount.context == null)
               {
                  noContextHandlers = (THandler)Delegate.Remove(noContextHandlers, handler)!;
               }

               // Иначе удаление из списка обработчиков с Контекстом.
               else if (!contextHandlers.TryGetValue(contextCount.context, out THandler? list))
               {
                  list = (THandler)Delegate.Remove(list, handler)!;
                  if (list == null)
                  {
                     contextHandlers.Remove(contextCount.context);
                  }

                  else
                  {
                     contextHandlers[contextCount.context] = list;
                  }
               }
            }
         }

      }

      /// <summary>  </summary>
      /// <param name="handlerToAction"></param>
      protected void Execute(Func<THandler, Action> handlerToAction)
      {
         THandler noContextHandlers;
         (SynchronizationContext context, THandler handler)[] contextHandlers;
         // Блокировка на время копирования обработчиков
         lock (lockEvent)
         {
            noContextHandlers = this.noContextHandlers;
            contextHandlers = this.contextHandlers
                                  .Select(pair => (pair.Key, pair.Value))
                                  .ToArray();
         }

         // Запуск асинхронного исполнения обработчиков с контекстом
         Task[] tasks = contextHandlers
             .Select
             (
               pair => Task.Run
               (
                  () => pair.context.Post(_ => handlerToAction(pair.handler)(), null)
               )
             )
             .ToArray();

         // Запуск синхронного исполнения обработчиков без контекста
         if (noContextHandlers != null)
         {
            handlerToAction(noContextHandlers)();
         }

         // Ожидание завершения асинхронной обработки
         Task.WaitAll(tasks);
      }

      #endregion Methods
   }
}