using GalaSoft.MvvmLight;
using NFCTrust.Models;
using NFCTrust.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace NFCTrust.ViewModel
{
    public class UserViewModel : ViewModelBase, INavigable
    {
        private User user;

        public User UserModel
        {
            get { return user; }
            set { user = value; RaisePropertyChanged(); }
        }

        public UserViewModel()
        {

        }

        public async void SaveUser()
        {
            var userSaved = await NfcClient.SaveUser(UserModel);
            ///Save to settings to edit later or use user id !!!
        }

        public void Activate(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Deactivate(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
