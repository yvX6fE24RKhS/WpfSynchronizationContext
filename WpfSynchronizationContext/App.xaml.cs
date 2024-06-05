// Ignore Spelling: App
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using WpfSynchronizationContext.Services.Connection;
using WpfSynchronizationContext.ViewModels;

namespace WpfSynchronizationContext
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      public App()
      {
         Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
         Services = ConfigureServices();
      }

      public new static App Current => (App)Application.Current;

      public IServiceProvider Services { get; }

      private static Ioc ConfigureServices()
      {
         ServiceCollection services = new();

         try
         {
            Ioc.Default.ConfigureServices(
                services
                // Services
                .AddSingleton<IConnectionService, ConnectionService>()
                // View Models
                .AddTransient<MainViewModel>()
                .BuildServiceProvider()
            );
         }
         catch (Exception ex)
         {
            Application.Current.Shutdown(ex.HResult);
         }

         return Ioc.Default;
      }
   }

}
