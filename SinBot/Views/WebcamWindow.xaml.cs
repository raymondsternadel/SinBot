using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;

namespace SinBot.Views
{
    /// <summary>
    /// Interaction logic for WebcamWindow.xaml
    /// </summary>
    public partial class WebcamWindow : Window
    {
        private FilterInfoCollection captureDevices;
        private VideoCaptureDevice videoSource;

        public WebcamWindow()
        {
            InitializeComponent();
            
            captureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            
            videoSource = new VideoCaptureDevice(captureDevices[0].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var rect = new System.Drawing.Rectangle();
                rect.Width = eventArgs.Frame.Width;
                rect.Height = eventArgs.Frame.Height;

                var bitmap = (Bitmap)eventArgs.Frame.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                using (var memory = new MemoryStream())
                {
                    bitmap.MakeTransparent(System.Drawing.Color.Black);
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                    bitmapimage.Freeze();



                    imgWebcam.Source = bitmapimage;
                }
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right/2 + 110;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            SessionContext.WebcamWindow = this;
        }
    }
}
