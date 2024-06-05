using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfSynchronizationContext.ViewModels;

namespace WpfSynchronizationContext.Views
{
   /// <summary>
   /// Логика взаимодействия для MainView.xaml
   /// </summary>
   public partial class MainView : Window
   {
      private readonly MainViewModel _mainViewModel;

      public MainView()
      {
         InitializeComponent();
         _mainViewModel = App.Current.Services.GetService<MainViewModel>()!;
         DataContext = _mainViewModel;
      }
   }
}
