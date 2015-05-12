using GalaSoft.MvvmLight.Command;
using NFCTrust.Writer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using NFCTrust.Writer.Service;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight;

namespace NFCTrust.Writer.ViewModel
{
    public class DriverViewModel : ViewModelBase, INavigable
    {
        #region navigation
        private INavigationService navigationService;

        public RelayCommand DetailsCommand { get; set; }

        public DriverViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
          //  if (!DesignMode.DesignModeEnabled)
              //  InitCaptureMedia();

        }
        #endregion
        private Driver driver;
        public Driver DriverModel
        {
            get
            {
                if (driver == null)
                {
                    driver = new Driver();
                }
                return driver;
            }
            set { driver = value; }
        }
        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                RaisePropertyChanged();
            }
        }
        private MediaCapture photoCapture;

        public MediaCapture PhotoCapture
        {
            get
            {
                if (photoCapture == null)
                {
                    photoCapture = new MediaCapture();
                }
                return photoCapture;
            }
            set { photoCapture = value; }
        }
        private CaptureElement captureElement;

        public CaptureElement CaptureElement
        {
            get
            {
                if (captureElement == null)
                {
                    captureElement = new CaptureElement() { Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill, Width = 350, Height = 500, Visibility = Windows.UI.Xaml.Visibility.Collapsed };
                }
                return captureElement;
            }
            set { captureElement = value; }
        }
        private WriteableBitmap previewImage;

        public WriteableBitmap PreviewImage
        {
            get { return previewImage; }
            set { previewImage = value; RaisePropertyChanged(); }
        }
        private BitmapImage resultImage;

        public BitmapImage ResultImage
        {
            get { return resultImage; }
            set { resultImage = value; RaisePropertyChanged(); }
        }



        async void InitCaptureMedia()
        {
            var cameraID = await GetCameraID(Windows.Devices.Enumeration.Panel.Front);
            PhotoCapture = new MediaCapture();
            await PhotoCapture.InitializeAsync(new MediaCaptureInitializationSettings()
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.VideoPreview,
                AudioDeviceId = string.Empty,
                VideoDeviceId = cameraID.Id
            });
            CaptureElement.Source = PhotoCapture;
            PhotoCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
        }

        async void InitPreviewAsync()
        {
            await PhotoCapture.StartPreviewAsync();
            var maxResolution = photoCapture.
                VideoDeviceController.
                GetAvailableMediaStreamProperties(MediaStreamType.Photo).Aggregate((i1, i2) => (i1 as VideoEncodingProperties).Width > (i2 as VideoEncodingProperties).Width ? i1 : i2);
            await PhotoCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, maxResolution);
            CaptureElement.Visibility = Windows.UI.Xaml.Visibility.Visible;
            TakePictureCommand.RaiseCanExecuteChanged();
            InitCameraCommand.RaiseCanExecuteChanged();
            PreviewImage = null;
        }
        private static async Task<DeviceInformation> GetCameraID(Windows.Devices.Enumeration.Panel desired)
        {
            DeviceInformation deviceID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
                .FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desired);

            if (deviceID != null) return deviceID;
            else throw new Exception(string.Format("Camera of type {0} doesn't exist.", desired));
        }


        public async void TakePicture()
        {
            ImageEncodingProperties format = ImageEncodingProperties.CreateJpeg();

            //rotate and save the image
            using (var imageStream = new InMemoryRandomAccessStream())
            {
                //generate stream from MediaCapture
                await PhotoCapture.CapturePhotoToStreamAsync(format, imageStream);
                await PhotoCapture.StopPreviewAsync();

                //create decoder and encoder
                BitmapDecoder dec = await BitmapDecoder.CreateAsync(imageStream);
                BitmapEncoder enc = await BitmapEncoder.CreateForTranscodingAsync(imageStream, dec);

                //roate the image
                enc.BitmapTransform.Rotation = BitmapRotation.Clockwise90Degrees;

                //write changes to the image stream
                await enc.FlushAsync();
                PreviewImage = new WriteableBitmap(350, 500);
                PreviewImage.SetSource(imageStream);

            }
            CaptureElement.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            using (var ms = PreviewImage.PixelBuffer.AsStream())
            {
                MemoryStream memoryStream = new MemoryStream();
                ms.CopyTo(memoryStream);
                DriverModel.Picture = memoryStream.ToArray();
            }
            TakePictureCommand.RaiseCanExecuteChanged();
            InitCameraCommand.RaiseCanExecuteChanged();
            //PhotoCapture.Dispose();
        }
        #region Commands
        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(this.SaveDriver, () => DriverModel != null);
                }
                return saveCommand;
            }
            set { saveCommand = value; }
        }
        private RelayCommand takePictureCommand;

        public RelayCommand TakePictureCommand
        {
            get
            {
                if (takePictureCommand == null)
                {
                    takePictureCommand = new RelayCommand(this.TakePicture, () => CaptureElement.Visibility == Windows.UI.Xaml.Visibility.Visible);
                }
                return takePictureCommand;
            }
            set { takePictureCommand = value; }
        }
        private RelayCommand initCameraCommand;

        public RelayCommand InitCameraCommand
        {
            get
            {
                if (initCameraCommand == null)
                {
                    initCameraCommand = new RelayCommand(this.InitPreviewAsync, () => CaptureElement.Visibility == Windows.UI.Xaml.Visibility.Collapsed);
                } return initCameraCommand;
            }
            set { initCameraCommand = value; }
        }

        #endregion


        #region HttpActions
        public async void SaveDriver()
        {
            Status = "Guardando...";
            var driver = await new NFCService().SaveDriver(DriverModel);
            Status = "Completado";
            navigationService.NavigateTo("Car", driver.Id);
            PhotoCapture.Dispose();
            PhotoCapture = null;
            //ResultImage = new BitmapImage();
            //var stream = new InMemoryRandomAccessStream();
            //await stream.WriteAsync(driver.Picture.AsBuffer());
            //stream.Seek(0);
            //ResultImage.SetSource(stream);
            //RaisePropertyChanged("ResultImage");
        }
        #endregion



        public void Activate(object parameter)
        {

        }

        public void Deactivate(object parameter)
        {

        }
    }
}
