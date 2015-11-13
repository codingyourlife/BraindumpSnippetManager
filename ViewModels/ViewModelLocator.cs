namespace SnippetManager
{
    using GalaSoft.MvvmLight.Ioc;
    using Microsoft.Practices.ServiceLocation;
    using ViewModels;

    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default); //SimpleIoc container init
            SimpleIoc.Default.Register<MainViewModel>();

            // IoC Dependency Injection - reslove configuration
            //SimpleIoc.Default.Register<IStreamingApp, TvNetflixApp>();
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>(); //returns singleton
    }
}
