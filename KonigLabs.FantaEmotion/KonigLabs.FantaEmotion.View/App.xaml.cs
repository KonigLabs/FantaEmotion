using System.Windows;
using KonigLabs.FantaEmotion.CommonViewModels.Ninject;
using KonigLabs.FantaEmotion.ViewModel.Ninject;
using KonigLabs.FantaEmotion.ViewModel.ViewModels;
using Ninject;

namespace KonigLabs.FantaEmotion.View
{
       
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainViewModel _mainViewModel;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitApp();
        }

        public void InitApp()
        {
            //инициализация Ninject
            var kernel = NinjectBootstrapper.GetKernel(new NinjectBaseModule(),new NinjectMainModule());
            _mainViewModel = kernel.Get<MainViewModel>();
            MainWindow = new MainWindow { DataContext = _mainViewModel };
            MainWindow.Closed += (s, e) =>
            {
                _mainViewModel.Dispose();
            };
            MainWindow.Show();
        }
    }
}
