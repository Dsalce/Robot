using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectToolsBox
{
    /// <summary>
    /// This class is use to control the kinect events
    /// </summary>
    public class KinectChooser
    {
        private KinectSensorChooser sensorChooser;
        /// <summary>
        /// Get the kinect sensor
        /// </summary>
        public KinectSensorChooser sensor
        {
            get
            {
                return this.sensorChooser;
            }
           
        }
        private KinectRegion kReg;
       
        /// <summary>
        /// KinectChooser Constructor 
        /// </summary>
        /// <param name="kReg"></param>
        /// <param name="sensorChooserUi"></param>
        public KinectChooser(KinectRegion kReg, KinectSensorChooserUI sensorChooserUi)
        {
            
            this.kReg = kReg;
            sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();
        }



        /// <summary>
        /// This event control all the change of the kinect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {

            if (args.OldSensor == null)
            {
                try
                {
                    //oldsensor
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (Exception) { }
            }

            if (args.NewSensor == null) { return; }


            try
            {
                //newsensor
                args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                //enable skeleton tracking
                args.NewSensor.SkeletonStream.Enable();
                try
                {
                    //new sensor tracking
                    args.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    args.NewSensor.DepthStream.Range = DepthRange.Near;
                    args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true ;
                  
                }
                catch (InvalidOperationException)
                {
                    //if the setting above fails
                    args.NewSensor.DepthStream.Range = DepthRange.Default;
                    args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;

                }
            }
            catch (InvalidOperationException) { }

            kReg.KinectSensor = args.NewSensor;

        }
        /// <summary>
        /// Start then kinect tracking
        /// </summary>
        public void Start()
        {
            this.sensorChooser.Start();
        }
        /// <summary>
        /// Stop the kinect tracking
        /// </summary>
        public void Stop()
        {
            this.sensorChooser.Stop();
        }

      

    }
}
