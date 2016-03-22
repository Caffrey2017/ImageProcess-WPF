﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Runtime.InteropServices;

namespace DynamicFaceDetect
{
    /// <summary>
    /// Interaction logic for FaceDetect.xaml
    /// </summary>
    public partial class FaceDetect : Window
    {
        private Capture capture;
        private HaarCascade haarCascade;
        private DispatcherTimer timer;

        public FaceDetect()
        {
            InitializeComponent();
            capture = new Capture();
            haarCascade = new HaarCascade(@"haarcascade_frontalface_alt_tree.xml");
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Image<Bgr, byte> currentFrame = capture.QueryFrame();

            if (currentFrame != null)
            {
                Image<Gray, byte> grayFrame = currentFrame.Convert<Gray, byte>();

                var detectedFaces = grayFrame.DetectHaarCascade(haarCascade)[0];

                foreach (var face in detectedFaces)
                    currentFrame.Draw(face.rect, new Bgr(243, 150, 33), 3);

                image1.Source = ToBitmapSource(currentFrame);
            }
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); // obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); // release the HBitmap
                return bs;
            }
        }
    }
}
