using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;
using KinectToolsBox;
using Microsoft.Kinect.Toolkit.Controls;

namespace practicaEsqueletoBrazo
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       private System.IO.Ports.SerialPort ArduinoPort;
       private KinectSensor miKinect;
       private KinectChooser sensorChooser;
       private Boolean isGrip;
       byte[] datosColor = null;
       private Joint j;
       private String servoVal;
       private String dis;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void load_window(object sender, RoutedEventArgs e)
        {
            //Esto es opcional pero ayuda a colocar el dispositivo Kinect a un cierto angulo de inclinacion, desde -27 a 27
           ArduinoPort = new System.IO.Ports.SerialPort();
            ArduinoPort.PortName = "COM3";  //sustituir por vuestro 
            ArduinoPort.BaudRate = 9600;
            ArduinoPort.Open();
            
           
            this.isGrip = false;
            //miKinect.Stop();


            this.miKinect = KinectSensor.KinectSensors[0]; //this.sensorChooser.sensor.Kinect;// 
            miKinect.SkeletonStream.Enable();
            miKinect.ColorStream.Enable();
            this.miKinect.Start();
            miKinect.ElevationAngle = 15;
            this.miKinect.Stop();
            sensorChooser = new KinectChooser(this.kinectRegion, this.sensorChooserUi);

            
           // miKinect.ColorFrameReady += miKinect_ColorFrameReady;
           // miKinect.SkeletonFrameReady += miKinect_SkeletonFrameReady;
           // AsyncCallback callBack = new AsyncCallback(OnQuery);
           // KinectRegion.AddQueryInteractionStatusHandler(this.kinectRegion, OnQuery);
             miKinect.ColorFrameReady += miKinect_ColorFrameReady;
             Task t1 = Task.Run(() => KinectRegion.AddQueryInteractionStatusHandler(this.kinectRegion, OnQuery));
             Task t2 = Task.Run(() =>  miKinect.SkeletonFrameReady += miKinect_SkeletonFrameReady);
            // Task t3 = Task.Run(() =>this.dist.Text = ArduinoPort.ReadByte().ToString());
           
 
        
         
        }

       
        WriteableBitmap colorImagenBitmap = null;
        void miKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            
            using (ColorImageFrame framesColor = e.OpenColorImageFrame())
            {
                if (framesColor == null) return;

                if (datosColor == null)
                    datosColor = new byte[framesColor.PixelDataLength];

                framesColor.CopyPixelDataTo(datosColor);

                if (colorImagenBitmap == null)
                {
                    this.colorImagenBitmap = new WriteableBitmap(
                        framesColor.Width,
                        framesColor.Height,
                        96,
                        96,
                        PixelFormats.Bgr32,
                        null);
                }

                this.colorImagenBitmap.WritePixels(
                    new Int32Rect(0, 0, framesColor.Width, framesColor.Height),
                    datosColor,
                    framesColor.Width * framesColor.BytesPerPixel,
                    0
                    );
                //se añade a la ventana
                this.canvasEsqueleto.Background = new ImageBrush(colorImagenBitmap);
               // this.dist.Text = this.dis;
                
            }
        }

    

        private void OnQuery(object sender, QueryInteractionStatusEventArgs handPointerEventArgs)
        {
            //If a grip detected change the cursor image to grip
            if (handPointerEventArgs.HandPointer.HandEventType == HandEventType.Grip)
            {
                
                this.isGrip = true;
                handPointerEventArgs.IsInGripInteraction = true;
                


            }
            else if (handPointerEventArgs.HandPointer.HandEventType == HandEventType.GripRelease)
            {
               
                this.isGrip = false;
                handPointerEventArgs.IsInGripInteraction = false;
              

            }
            else if (handPointerEventArgs.HandPointer.HandEventType == HandEventType.None)
            {
                handPointerEventArgs.IsInGripInteraction = this.isGrip;
                
            }

          
            handPointerEventArgs.Handled = true;

           
        }
     

        private void miKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
           //this.canvasEsqueleto.Children.Clear();
            Skeleton[] esqueletos = null;

            using (SkeletonFrame frameEsqueleto = e.OpenSkeletonFrame())
            {
                if (frameEsqueleto != null)
                {
                    esqueletos = new Skeleton[frameEsqueleto.SkeletonArrayLength];
                    frameEsqueleto.CopySkeletonDataTo(esqueletos);
                }
            }

            if (esqueletos == null) return;

            foreach (Skeleton esqueleto in esqueletos)
            {
                if (esqueleto.TrackingState == SkeletonTrackingState.Tracked)
                {
                    Joint handJoint = esqueleto.Joints[JointType.HandRight];
                    Joint elbowJoint = esqueleto.Joints[JointType.ElbowRight];

                    this.mandarSeñal(esqueleto);
                   /* agregarLinea(esqueleto.Joints[JointType.ShoulderCenter], esqueleto.Joints[JointType.ShoulderRight]);
                    agregarLinea(esqueleto.Joints[JointType.ShoulderRight], esqueleto.Joints[JointType.ElbowRight]);
                    agregarLinea(esqueleto.Joints[JointType.ElbowRight], esqueleto.Joints[JointType.WristRight]);
                    agregarLinea(esqueleto.Joints[JointType.WristRight], esqueleto.Joints[JointType.HandRight]);*/


                }
            }
        }
        private int valX = 0;
        private int valY = 0;
        void mandarSeñal(Skeleton esqueleto)
        {
           
            if (esqueleto.Joints[JointType.HandRight].Position.X != j.Position.X)
            {
                Joint arm = esqueleto.Joints[JointType.HandRight];

                int angle = (int)(esqueleto.Joints[JointType.HandRight].Position.X * 100.0f);
                // if (angle >= 0 && valY >= 0 || angle <= 0 && valY <= 0)  {
                //  ArduinoPort.WriteLine("X" + angle.ToString());
               // Console.WriteLine(" Ang:" + angle);
                servoVal = servoVal + angle;
                //while (((char)ArduinoPort.ReadChar()) != 'E') ;
                valX = angle;
                // }
            }
            if (esqueleto.Joints[JointType.HandRight].Position.Y != j.Position.Y)
            {
             Joint arm= esqueleto.Joints[JointType.HandRight];
           
             int angle =(int)(esqueleto.Joints[JointType.HandRight].Position.Y * 100.0f);
             //if (angle >= 0 && valY >= 0 || angle <= 0 && valY <= 0){
                //ArduinoPort.WriteLine("Y"+angle.ToString());
             servoVal = servoVal + "," + angle;
                 //while (((char)ArduinoPort.ReadChar()) != 'E') ;
                  valY=angle;
            // }
            }
            
           if (esqueleto.Joints[JointType.HandRight].Position.Z != j.Position.Z)
            {
                Joint arm = esqueleto.Joints[JointType.HandRight];
                //  double radians = Math.Atan2(esqueleto.Joints[JointType.HandRight].Position.Y, esqueleto.Joints[JointType.HandRight].Position.X);
                // double angle = radians * (360/ Math.PI);
                int angle = (int)(esqueleto.Joints[JointType.HandRight].Position.Z * 100.0f);
                //ArduinoPort.WriteLine("Z" + angle.ToString());
                servoVal = servoVal+"," + angle;
                 Console.WriteLine(" Ang:" +angle);
                //  int aux = ((char)ArduinoPort.ReadChar());
                //while (((char)ArduinoPort.ReadChar()) != 'E') ;
            }

           
            Console.WriteLine(" X:" + esqueleto.Joints[JointType.HandRight].Position.X+" Y:"+esqueleto.Joints[JointType.HandRight].Position.Y+" Z:"+esqueleto.Joints[JointType.HandRight].Position.Z);
             this.j = esqueleto.Joints[JointType.HandRight];
             if (servoVal.Length > 0)
             {
                 if (this.isGrip) { servoVal = servoVal + ",10x"; } else { servoVal = servoVal + ",100x"; }
                 Console.WriteLine(servoVal);
                 ArduinoPort.WriteLine(servoVal);
                
                 while (((char)ArduinoPort.ReadChar()) != 'E') ;
               //  this.dis = ArduinoPort.ReadByte().ToString();
             }
            this.servoVal = "";
       
        }


        void agregarLinea(Joint j1, Joint j2)
        {

            
            Line lineaHueso = new Line();
            lineaHueso.Stroke = new SolidColorBrush(Colors.Red);
            lineaHueso.StrokeThickness = 5;

            ColorImagePoint j1P = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution1280x960Fps12);
            lineaHueso.X1 = j1P.X;
            lineaHueso.Y1 = j1P.Y;

            // lineaHueso.Z1 = j1P.Z;

            ColorImagePoint j2P = miKinect.CoordinateMapper.MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution1280x960Fps12);
            lineaHueso.X2 = j2P.X;
            lineaHueso.Y2 = j2P.Y;
            // lineaHueso.Z2 = j2P.Z;
            this.canvasEsqueleto.Children.Add(lineaHueso);



        }

       
    }

}