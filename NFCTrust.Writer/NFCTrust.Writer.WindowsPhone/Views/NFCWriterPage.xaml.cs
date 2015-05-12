using NdefLibrary.Ndef;
using NFCTrust.Writer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Networking.Proximity;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace NFCTrust.Writer.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 


    public sealed partial class NFCWriterPage : Page
    {
        private ProximityDevice _device;
        private long _subscriptionIdNdef;
        private long _publishingMessageId;
        public string CarRegistration { get; set; }
        public NFCWriterPage()
        {
            this.InitializeComponent();
            this.Loaded += NFCWriterPage_Loaded;
        }

        void NFCWriterPage_Loaded(object sender, RoutedEventArgs e)
        {
            _device = ProximityDevice.GetDefault();
            // Update status text for UI
            SetStatusOutput(_device != null ? "NFC Iniciado" : "NFC Fallo");
            btnWrite.Click += btnWrite_Click;
        }

        void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            var record = new NdefLaunchAppRecord { Arguments = CarRegistration };
            record.AddPlatformAppId("WindowsPhone", "{86860476-c12a-47db-9f45-eedf2d21d33c}");
            PublishRecord(record, true);
        }
        private void PublishRecord(NdefRecord record, bool writeToTag)
        {
            if (_device == null) return;
            // Make sure we're not already publishing another message
            StopPublishingMessage(false);
            // Wrap the NDEF record into an NDEF message
            var message = new NdefMessage { record };
            // Convert the NDEF message to a byte array
            var msgArray = message.ToByteArray();
            // Publish the NDEF message to a tag or to another device, depending on the writeToTag parameter
            // Save the publication ID so that we can cancel publication later
            _publishingMessageId = _device.PublishBinaryMessage((writeToTag ? "NDEF:WriteTag" : "NDEF"), msgArray.AsBuffer(), MessageWrittenHandler);
            // Update status text for UI
            SetStatusOutput(string.Format((writeToTag ? "Escribiendo..." : "Escribiendo...")));
            // Update enabled / disabled state of buttons in the User Interface
            //UpdateUiForNfcStatus();
        }
        private void MessageWrittenHandler(ProximityDevice sender, long messageid)
        {
            // Stop publishing the message
            StopPublishingMessage(false);
            // Update status text for UI
            SetStatusOutput("Se escribio correctamente");
            btnHome.IsEnabled = true;
            btnHome.Click += btnHome_Click;
        }

        void btnHome_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void StopPublishingMessage(bool writeToStatusOutput)
        {
            if (_publishingMessageId != 0 && _device != null)
            {
                // Stop publishing the message
                _device.StopPublishingMessage(_publishingMessageId);
                _publishingMessageId = 0;
                // Update enabled / disabled state of buttons in the User Interface
                //UpdateUiForNfcStatus();
                // Update status text for UI - only if activated
                if (writeToStatusOutput) SetStatusOutput("Se detuvo la escritura");
            }
        }
        private async void SetStatusOutput(string newStatus)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(() => { if (NFCStatus != null) NFCStatus.Text = newStatus; }));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
                CarRegistration = e.Parameter.ToString();
        }
    }
}
