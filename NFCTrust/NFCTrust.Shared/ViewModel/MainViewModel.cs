using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using NFCTrust.Models;

namespace NFCTrust.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, INavigable
    {
        #region ViewObjects
        //objects for user validation
        #endregion

        private INavigationService navigationService;

        public MainViewModel(INavigationService navigationService)
        {
            if (!IsInDesignMode)
            {
                this.navigationService = navigationService;
            }
        }

        #region Navigation

        public void Activate(object parameter)
        {
            if (parameter != null && parameter != "")
            {
                navigationService.NavigateTo("CarDriver", parameter);
            }

        }

        public void Deactivate(object parameter)
        {

        }
        #endregion
    }
}