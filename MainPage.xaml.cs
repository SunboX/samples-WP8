using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MWBCameraDemo.Resources;

using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Markup;
using System.Windows.Resources;
using System.IO;
using System.Windows.Media.Imaging;
using System.Reflection;

using Windows.Phone.Media.Capture;
using Windows.Phone.Media.Devices;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Diagnostics;
using System.Windows.Media;
using Microsoft.Devices;

using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;

using BarcodeLib;

namespace MWBCameraDemo
{


    public partial class MainPage : PhoneApplicationPage
    {

        public const int MAX_RESOLUTION = 1280 * 768;

        public static PhotoCaptureDevice cameraDevice;
        public static Boolean isProcessing = false;
        private byte[] pixels = null;
        private DateTime lastTime;
        private BackgroundWorker bw = new BackgroundWorker();

        private async Task InitializeCamera(CameraSensorLocation sensorLocation)
        {




            Windows.Foundation.Size captureResolution = new Windows.Foundation.Size(640, 480);
            Windows.Foundation.Size previewResolution = new Windows.Foundation.Size(640, 480);

            IReadOnlyList<Windows.Foundation.Size> prevSizes = PhotoCaptureDevice.GetAvailablePreviewResolutions(sensorLocation);
            IReadOnlyList<Windows.Foundation.Size> captSizes = PhotoCaptureDevice.GetAvailableCaptureResolutions(sensorLocation);

            double minDiff = 9999999;
            int minIndex = -1;


            double bestAspect = 1000;

            int bestAspectResIndex = 0;


            double aspect = App.Current.Host.Content.ActualHeight / App.Current.Host.Content.ActualWidth;

            minDiff = 9999999;
            minIndex = -1;
            for (int i = 0; i < captSizes.Count; i++)
            {
                double w = captSizes[i].Width;
                double h = captSizes[i].Height;

                double resAspect = w / h;

                double diff = aspect - resAspect;
                if (diff < 0)
                    diff = -diff;


                if (diff < bestAspect)
                {
                    bestAspect = diff;
                    bestAspectResIndex = i;
                }

                diff = Math.Abs(captureResolution.Width - w) + Math.Abs(captureResolution.Height - h);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    minIndex = i;
                }
            }

            if (bestAspectResIndex >= 0)
            {
                captureResolution.Width = captSizes[bestAspectResIndex].Width;
                captureResolution.Height = captSizes[bestAspectResIndex].Height;
            }

            Windows.Foundation.Size initialResolution = captureResolution;

            try
            {
                PhotoCaptureDevice d = null;
                if (cameraDevice == null)
                {

                    System.Diagnostics.Debug.WriteLine("Settinge camera initial resolution: " + initialResolution.Width + "x" + initialResolution.Height + "......");

                    bool initialized = false;

                    try
                    {
                        d = await PhotoCaptureDevice.OpenAsync(sensorLocation, initialResolution);
                        System.Diagnostics.Debug.WriteLine("Success " + initialResolution);
                        initialized = true;
                        captureResolution = initialResolution;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to set initial resolution: " + initialResolution);

                    }

                    if (!initialized)
                        try
                        {
                            d = await PhotoCaptureDevice.OpenAsync(sensorLocation, captSizes.ElementAt<Windows.Foundation.Size>(0));
                            System.Diagnostics.Debug.WriteLine("Success " + captSizes.ElementAt<Windows.Foundation.Size>(0));
                            initialized = true;
                            captureResolution = captSizes.ElementAt<Windows.Foundation.Size>(0);
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("Failed to set initial resolution: " + captSizes.ElementAt<Windows.Foundation.Size>(0));
                        }

                    //try to not use too high resolution

                    if (d.PreviewResolution.Height * d.PreviewResolution.Width > MAX_RESOLUTION)
                    {

                        bestAspectResIndex = -1;

                        aspect = (double)captureResolution.Width / captureResolution.Height;

                        for (int i = 0; i < prevSizes.Count; i++)
                        {
                            double w = prevSizes[i].Width;
                            double h = prevSizes[i].Height;

                            double resAspect = w / h;

                            double diff = aspect - resAspect;
                            if (diff < 0.01 && diff > -0.01)
                            {

                                if (w * h <= MAX_RESOLUTION)
                                {
                                    previewResolution = prevSizes.ElementAt<Windows.Foundation.Size>(i);
                                    bestAspectResIndex = i;
                                    break;
                                }
                            }



                        }


                        if (bestAspectResIndex >= 0)
                            try
                            {
                                await d.SetPreviewResolutionAsync(previewResolution);
                            }
                            catch (Exception e)
                            {

                            }

                    }

                    System.Diagnostics.Debug.WriteLine("Preview resolution: " + d.PreviewResolution);

                    d.SetProperty(KnownCameraGeneralProperties.EncodeWithOrientation,
                                  d.SensorLocation == CameraSensorLocation.Back ?
                                  d.SensorRotationInDegrees : -d.SensorRotationInDegrees);


                    cameraDevice = d;
                }

                cameraDevice.PreviewFrameAvailable += new TypedEventHandler<ICameraCaptureDevice, Object>(cam_PreviewFrameAvailable);


                IReadOnlyList<object> flashProperties = PhotoCaptureDevice.GetSupportedPropertyValues(sensorLocation, KnownCameraAudioVideoProperties.VideoTorchMode);




                videoBrush.SetSource(cameraDevice);
                DispatcherTimer focusTimer = new DispatcherTimer();
                focusTimer.Interval = TimeSpan.FromSeconds(3);
                focusTimer.Tick += delegate
                {
                    Debug.WriteLine("Camera focusing");
                    cameraDevice.FocusAsync();
                    //Console.WriteLine("focus");
                };
                focusTimer.Start();

                bw.WorkerReportsProgress = false;
                bw.WorkerSupportsCancellation = false;
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);

            }
            catch (Exception e)
            {
                Debug.WriteLine("Camera initialization error: " + e.Message);
            }

        }


        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Byte[] result = new Byte[10000];
            BackgroundWorker worker = sender as BackgroundWorker;

            ThreadArguments ta = e.Argument as ThreadArguments;

            int resLen = Scanner.MWBscanGrayscaleImage(ta.pixels, ta.width, ta.height, result);

            if (lastTime != null && lastTime.Ticks > 0)
            {
                long timePrev = lastTime.Ticks;
                long timeNow = DateTime.Now.Ticks;
                long timeDifference = (timeNow - timePrev) / 10000;
                System.Diagnostics.Debug.WriteLine("frame time: {0}", timeDifference);

            }

            lastTime = DateTime.Now;
            //ignore all results shorter than 4 characters
            if (resLen > 4 || ((resLen > 0 && Scanner.MWBgetLastType() != Scanner.FOUND_39 && Scanner.MWBgetLastType() != Scanner.FOUND_25_INTERLEAVED && Scanner.MWBgetLastType() != Scanner.FOUND_25_INTERLEAVED)))
            {
                string resultString = System.Text.Encoding.UTF8.GetString(result, 0, resLen);

                int bcType = Scanner.MWBgetLastType();
                String typeName = BarcodeHelper.getBarcodeName(bcType);

                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    MessageBox.Show(resultString, typeName, new MessageBoxButton() { });
                    isProcessing = false;
                });


            }
            else
            {
                isProcessing = false;
            }

           

        }

        class ThreadArguments
        {
            public int width { get; set; }
            public int height { get; set; }
            public byte[] pixels { get; set; }
        }

        void cam_PreviewFrameAvailable(ICameraCaptureDevice device, object sender)
        {
            if (isProcessing)
                return;
            isProcessing = true;
            int len = (int)device.PreviewResolution.Width * (int)device.PreviewResolution.Height;
            if (pixels == null)
                pixels = new byte[len];
            device.GetPreviewBufferY(pixels);
            Byte[] result = new Byte[10000];
            int width = (int)device.PreviewResolution.Width;
            int height = (int)device.PreviewResolution.Height;

            ThreadArguments ta = new ThreadArguments();
            ta.height = height;
            ta.width = width;
            ta.pixels = pixels;
           
            bw.RunWorkerAsync(ta);


        }



       
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            BarcodeHelper.initDecoder();

            int ver = Scanner.MWBgetLibVersion();
            int v1 = (ver >> 16);
            int v2 = (ver >> 8) & 0xff;
            int v3 = (ver & 0xff);
            String libVersion = String.Format("{0}.{1}.{2}", v1, v2, v3);

            System.Diagnostics.Debug.WriteLine("Lib version: " +libVersion);
           // System.Diagnostics.Debugger.Log(1, "1","Lib version: " + libVersion);
           

           

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
           
            base.OnNavigatedTo(e);
            InitializeCamera(CameraSensorLocation.Back);

            MWOverlay.addOverlay(canvas);

        }



        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {

            MWOverlay.removeOverlay();
            base.OnNavigatingFrom(e);
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            // Switch the placement of the buttons based on an orientation change.

            if (videoBrush == null)
                return;

            if ((e.Orientation & PageOrientation.LandscapeLeft) == (PageOrientation.LandscapeLeft))
            {

                videoBrush.RelativeTransform = new CompositeTransform()
                {
                    CenterX = 0.5,
                    CenterY = 0.5,
                    Rotation = 0
                };


            }

            else
                if ((e.Orientation & PageOrientation.LandscapeRight) == (PageOrientation.LandscapeRight))
                {

                    videoBrush.RelativeTransform = new CompositeTransform()
                    {
                        CenterX = 0.5,
                        CenterY = 0.5,
                        Rotation = 180
                    };


                }


        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}