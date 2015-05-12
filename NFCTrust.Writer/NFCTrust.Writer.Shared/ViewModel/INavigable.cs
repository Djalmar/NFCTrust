using System;
using System.Collections.Generic;
using System.Text;

namespace NFCTrust.Writer.ViewModel
{
    public interface INavigable
    {
        void Activate(object parameter);
        void Deactivate(object parameter);
    }
}
