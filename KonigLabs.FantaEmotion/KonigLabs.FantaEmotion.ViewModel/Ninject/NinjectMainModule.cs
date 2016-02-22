using System.Collections.Generic;
using System.Linq;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Factories;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation;
using KonigLabs.FantaEmotion.PatternProcessing.ImageProcessors;
using KonigLabs.FantaEmotion.ViewModel.Factories;
using KonigLabs.FantaEmotion.ViewModel.ViewModels;
using Ninject;
using Ninject.Modules;

namespace KonigLabs.FantaEmotion.ViewModel.Ninject
{
    public class NinjectMainModule: NinjectModule
    {
        public override void Load()
        {
            
            Bind<MainViewModel>().ToSelf();
            Bind<WelcomViewModel>().ToSelf();
            Bind<CompositionModelProcessorFactory>().ToSelf();
            Bind<TakePhotoViewModelFactory>().ToSelf();
            Bind<WelcomViewModelFactory>().ToSelf();
            Bind<TakePhotoResultViewModelFactory>().ToSelf();
            Bind<TakeVideoResultViewModelFactory>().ToSelf();
            Bind<IViewModelNavigator>().To<ViewModelNavigator>()
                .WithConstructorArgument(typeof(IChildrenViewModelsFactory),
                    x => new ChildrenViewModelsFactory(Enumerable.Empty<IViewModelFactory>()));
            Bind<IViewModelNavigator>().To<ViewModelNavigator>().WhenInjectedExactlyInto<MainViewModel>().WithConstructorArgument(typeof(IChildrenViewModelsFactory), x =>
            {
                var children = new List<IViewModelFactory>
                {
                    x.Kernel.Get<WelcomViewModelFactory>()

                };

                return new ChildrenViewModelsFactory(children);
            });
            Bind<IViewModelNavigator>().To<ViewModelNavigator>().WhenInjectedExactlyInto<WelcomViewModelFactory>().WithConstructorArgument(typeof(IChildrenViewModelsFactory), x =>
            {
                var children = new List<IViewModelFactory>
                {
                    x.Kernel.Get<TakePhotoViewModelFactory>()

                };

                return new ChildrenViewModelsFactory(children);
            });
            Bind<IViewModelNavigator>().To<ViewModelNavigator>().WhenInjectedExactlyInto<TakePhotoViewModelFactory>().WithConstructorArgument(typeof(IChildrenViewModelsFactory), x =>
            {
                var children = new List<IViewModelFactory>
                {
                    x.Kernel.Get<TakePhotoResultViewModelFactory>(),
                    x.Kernel.Get<TakeVideoResultViewModelFactory>()
                };

                return new ChildrenViewModelsFactory(children);
            });
        }
    }
}
