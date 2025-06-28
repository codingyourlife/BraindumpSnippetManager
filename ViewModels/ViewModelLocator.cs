namespace SnippetManager
{
    using CommonServiceLocator;
    using GalaSoft.MvvmLight.Ioc;
    using ViewModels;
    using Interfaces;
    using Services;

    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default); //SimpleIoc container init
            
            // Register services
            SimpleIoc.Default.Register<IClipboardActions, ClipboardActions>();
            SimpleIoc.Default.Register<ISnippetFileService, SnippetFileService>();
            
            // Register ViewModels
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<EditViewModel>();

            // IoC Dependency Injection - reslove configuration
            //SimpleIoc.Default.Register<IStreamingApp, TvNetflixApp>();
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>(); //returns singleton
        public EditViewModel EditViewModel => ServiceLocator.Current.GetInstance<EditViewModel>(); //returns singleton
    }
}
