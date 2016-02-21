using KonigLabs.FantaEmotion.Common.Helpers;
using KonigLabs.FantaEmotion.CommonViewModels.Messenger;
using KonigLabs.FantaEmotion.CommonViewModels.Services;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation;
using Ninject.Modules;

namespace KonigLabs.FantaEmotion.CommonViewModels.Ninject
{
    public class NinjectBaseModule : NinjectModule
    {
        public override void Load()
        {
            //Bind<IMappingEngine>()
            //    .ToMethod(x => MappingEngineConfigurator.CreateEngine(new BasicProfile()));

            Bind<MessageFactory>().ToSelf();
            Bind<IMessenger>().To<MvvmLightMessenger>().InSingletonScope();
            Bind<IHashBuilder>().To<HashBuilder>();
            Bind<ViewModelStorage>().ToSelf().InSingletonScope();
            Bind<IDialogService>().To<DialogService>();
        }
    }
}
