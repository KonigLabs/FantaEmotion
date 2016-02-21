
using System.Collections.Generic;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Factories;
using KonigLabs.FantaEmotion.CommonViewModels.ViewModels.Navigation;
using KonigLabs.FantaEmotion.Entities;
using KonigLabs.FantaEmotion.PatternProcessing.ImageProcessors;
using KonigLabs.FantaEmotion.ViewModel.Providers;
using KonigLabs.FantaEmotion.ViewModel.ViewModels;

namespace KonigLabs.FantaEmotion.ViewModel.Factories
{
    public class TakePhotoViewModelFactory : ViewModelBaseFactory<TakePhotoViewModel>
    {
        IViewModelNavigator _navigator;
        CompositionModelProcessorFactory _imageProcessor;
        SettingsProvider _settingsProvider;
        public TakePhotoViewModelFactory(IViewModelNavigator navigator, CompositionModelProcessorFactory processor, SettingsProvider settingsProvider)
        {
            _navigator = navigator;
            _imageProcessor = processor;
            _settingsProvider = settingsProvider;
        }
        protected override TakePhotoViewModel GetViewModel(object param)
        {

            return new TakePhotoViewModel(_navigator, _imageProcessor.Create(new Template { Images = new List<TemplateImage>() }), _settingsProvider);
        }
    }
}
