//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

//#define VZ_EMULATE_GEARVR

using UnityEngine;
using System;
using System.Text;
using System.IO;

#if UNITY_PS4 && !UNITY_EDITOR
using UnityEngine.PS4.VR;
#endif

public class VZController : MonoBehaviour
{
   //***********************************************************************
   // PUBLIC API
   //***********************************************************************

   public class Button
   {
      public bool Down { get; private set; }

      public Button()
      {
         Down = false;
      }

      public void Set(bool down)
      {
         if (down == Down)
            return;

         if (down)
         {
            Down = true;
            mDownTime = Time.time;
            mPossibleFalseUp = false;
         }
         else if (Time.time - mDownTime < 0.1f)
         {
            // Don't update Down yet
            mPossibleFalseUp = true;
         }
         else
         {
            Down = false;
            mPossibleFalseUp = false;
         }
      }

      public void Update()
      {
         // Handle possible false button up
         if (mPossibleFalseUp && Time.time - mDownTime > .2f)
         {
            mPossibleFalseUp = false;
            Down = false;
         }

         mChanged = (mLastDown != Down);
         mLastDown = Down;
      }

      public bool Pressed()
      {
         return mChanged && Down;
      }

      public bool Released()
      {
         return mChanged && !Down && mDownTime != 0;
      }

      public bool Held(float period)
      {
         return Down && mDownTime != 0 && Time.time - mDownTime > period;
      }

      public void Clear()
      {
         mDownTime = 0;
      }

      float mDownTime;
      bool mPossibleFalseUp = false;
      bool mLastDown = false;
      bool mChanged = false;
   }

   public Button LeftButton = new Button();
   public Button RightButton = new Button();
   public Button DpadUp = new Button();
   public Button DpadDown = new Button();
   public Button DpadLeft = new Button();
   public Button DpadRight = new Button();
   public Button RightUp = new Button();
   public Button RightDown = new Button();
   public Button RightLeft = new Button();
   public Button RightRight = new Button();
   public float InputSpeed { get; private set; } 
   public float HeadRot { get; private set; }  // positive counterclockwise
   public float HeadLean { get; private set; } // positive left
   public float HeadBend { get; private set; }
   public bool IsSteamVR { get; private set; }
   public float Distance { get; private set; }
   public Transform Head { get; private set; }

   public VZBikeState BikeState()
   {
      return mBikeState;
   }

   public bool HasHmd()
   {
      return mHasHmd;
   }

   public void ResetDistance()
   {
      Distance = 0;
   }

   public void Recenter()
   {
      if (!IsHeadTracked())
      {
         Debug.LogError("Can't recenter if head isn't tracked");
         return;
      }

      // Reset hmd
      if (mHasHmd)
      {
         // Don't use Unity Recenter, leave Camera uncalibrated and manually offset
         float yawOffset = -mCamera.transform.localEulerAngles.y;
         float ang = -yawOffset * Mathf.Deg2Rad;
         float sinAng = Mathf.Sin(ang);
         float cosAng = Mathf.Cos(ang);
         float xOffset = mCamera.localPosition.z * sinAng - mCamera.localPosition.x * cosAng;
         float zOffset = -mCamera.localPosition.z * cosAng - mCamera.localPosition.x * sinAng;
         float yOffset = -mCamera.localPosition.y;

         mCameraOffset.localPosition = new Vector3(xOffset, yOffset, zOffset);
         mCameraOffset.localEulerAngles = new Vector3(0, yawOffset, 0);

         Head.localPosition = Vector3.zero;
         Head.localEulerAngles = Vector3.zero;
      }
   }

   public bool IsHeadTracked()
   {
      if (!mHasHmd)
         return true;

#if VZ_EMULATE_GEARVR
      return true;
#endif

#if UNITY_PS4 && !UNITY_EDITOR
      int handle = PlayStationVR.GetHmdHandle();

      PlayStationVRTrackingStatus status;
      Tracker.GetTrackedDeviceStatus(handle, out status);
      if (status != PlayStationVRTrackingStatus.Tracking)
         return false;

      PlayStationVRTrackingQuality quality;
      Tracker.GetTrackedDevicePositionQuality(handle, out quality);
      return quality == PlayStationVRTrackingQuality.Full;
#elif VZ_GAME
      if (IsSteamVR)
         return !SteamVR.outOfRange;
      else
         return OVRPlugin.positionTracked;
#else
      return true;
#endif
   }

   public virtual void Restart()
   {
      LeftButton.Clear();
      RightButton.Clear();
      DpadUp.Clear();
      DpadDown.Clear();
      DpadLeft.Clear();
      DpadRight.Clear();
      RightUp.Clear();
      RightDown.Clear();
      RightLeft.Clear();
      RightRight.Clear();
   }

   public GameObject TransitionCanvas()
   {
      return mTransitionCanvas;
   }

   public string BikeSender()
   {
      if (mBikeState.Type == (int)VZBikeType.Beta)
      {
         // Beta bike
         return mBikeState.Sender();
      }
      else
      {
         // Alpha bike hack
#if UNITY_PS4 && !UNITY_EDITOR
         return Network.player.ipAddress; // MachineName crashes offline!
#else
         return System.Environment.MachineName;
#endif
      }
   }

   public void TryConnectBike()
   {
      VZPlugin.ConnectBike(ref mBikeState);
   }

   public Transform Neck()
   {
      return mNeck;
   }

   public Camera Camera()
   {
      return mCamera.GetComponent<Camera>();
   }

   //***********************************************************************
   // PRIVATE API
   //***********************************************************************

   enum ControllerType
   {
      Keyboard,
      DS4,
      Logitech,
      Xbox,
      X360
   };

   const float kControllerMaxSpeed = 10.0f;
   const float kHeadDead = 0.02f;
#if VZ_EMULATE_GEARVR
   const float kHeadWidth = 0.0f;
#else
   const float kHeadWidth = 0.1f;
#endif

   Transform mCameraOffset;
   Transform mCamera;
   VZBikeState mBikeState = new VZBikeState();
   ControllerType mController;
   float mControllerPitch = 0;
   float mControllerYaw = 0;
   float mControllerLean = 0;
   float mControllerBend = 0;
   float mControllerPitchVel = 0;
   float mControllerYawVel = 0;
   float mControllerLeanVel = 0;
   float mControllerBendVel = 0;
   bool mHasHmd;
   GameObject mTransitionCanvas;
   Transform mNeck;

#if UNITY_PS4 && !UNITY_EDITOR
   int mPS4RenderScale = 2;

   void PSVRCompleted(DialogStatus status, DialogResult result)
   {
      Debug.Log("PSVR Completed " + status + " " + result);

      mHasHmd = (result == DialogResult.OK);

      PlayStationVRSettings.reprojectionSyncType = PlayStationVRReprojectionSyncType.ReprojectionSyncVsync;
      PlayStationVRSettings.reprojectionFrameDeltaType = PlayStationVRReprojectionFrameDeltaType.UnityCameraAndHeadRotation;
      PlayStationVRSettings.minOutputColor = new Color(0.005f, 0.005f, 0.005f);

      UnityEngine.VR.VRSettings.loadedDevice = UnityEngine.VR.VRDeviceType.PlayStationVR;
      UnityEngine.VR.VRSettings.showDeviceView = true;

      // HACK need for force file setting
//      QualitySettings.antiAliasing = 2;

      // Delay setting scale
      UnityEngine.VR.VRSettings.renderScale = 1.4f;
//      mPS4RenderScale = 2;

      Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
      UnityEngine.VR.VRSettings.enabled = mHasHmd;
   }
#endif

   protected virtual void Awake()
   {
      Distance = 0;

      // Neck/camera/head setup
      mNeck = transform.Find("Neck");
      mCameraOffset = mNeck.Find("CameraOffset");
      mCamera = mCameraOffset.Find("Camera");
      Head = mNeck.Find("Head");

      // Setup transition canvas
      mTransitionCanvas = mNeck.Find("Head/TransitionCanvas").gameObject;
      mTransitionCanvas.SetActive(true);

      // SteamVR
      IsSteamVR = false;

      // SteamVR
      IsSteamVR = (UnityEngine.VR.VRSettings.loadedDeviceName == "OpenVR");

      // Init plugin
      VZPlugin.Init(Application.dataPath + Path.DirectorySeparatorChar + "Plugins");
   }

   void Start()
   {
      InputSpeed = 0;
      HeadRot = 0;
      HeadLean = 0;
      HeadBend = 0;

#if !UNITY_PS4 && VZ_GAME
      // Redirect Fmod to Rift headphones
      if (GameObject.Find("FMOD_StudioSystem") != null)
      {
         FMOD.System sys = FMODUnity.RuntimeManager.LowlevelSystem;
//         Debug.Log(sys);

         int num;
         sys.getNumDrivers(out num);
//         Debug.Log(num);

         for (int i = 0; i < num; ++i)
         {
            int namelen = 100;
            StringBuilder name = new StringBuilder(namelen);
            Guid guid;
            int systemrate;
            FMOD.SPEAKERMODE speakermode;
            int speakermodechannels;

            sys.getDriverInfo(i, name, namelen, out guid, out systemrate, out speakermode, out speakermodechannels);

//            Debug.Log(i + " " + name + " " + namelen + " " + guid + " " + systemrate + " " + speakermode + " " + speakermodechannels);

            if (name.ToString() == "Headphones (Rift Audio)")
            {
               sys.setDriver(i);
               Debug.Log("Redirecting Fmod audio to Rift headphones");
               break;
            }
         }
      }
#endif

      // Setup hmd
#if VZ_GAME
      if (VZReplay.Playback())
      {
         mHasHmd = false;
         UnityEngine.VR.VRSettings.enabled = false;

         if (VZReplay.Instance.Camera != null)
            Camera().enabled = false;
      }
      else
#endif
      {
         mHasHmd = UnityEngine.VR.VRDevice.isPresent;
         UnityEngine.VR.VRSettings.enabled = mHasHmd;

#if UNITY_PS4 && !UNITY_EDITOR
         HmdSetupDialog.OpenAsync(0, PSVRCompleted); 
#endif
      }

      // Try serial communication
#if VZ_GAME
      if (VZReplay.Playback())
      {
         mBikeState.Type = (int)VZBikeType.None;
      }
      else
#endif
      {
         TryConnectBike();
      }

      // Determine controller for other functions
      mController = ControllerType.Keyboard;

      string[] joysticks = Input.GetJoystickNames();

      if (joysticks.Length > 0)
      {
//			Debug.Log("Joystick = " + joysticks[0]);

         if (joysticks[0] == "Logitech Dual Action")
         {
            mController = ControllerType.Logitech;
         }
         else if (joysticks[0] == "Controller (Xbox One For Windows)")
         {
            //Debug.Log("Xbox controller selected.");
            mController = ControllerType.Xbox;
         }
         else if (joysticks[0] == "Controller (XBOX 360 For Windows)")
         {
             //Debug.Log("Xbox 360 controller selected.");
             mController = ControllerType.X360;
         }
#if UNITY_PS4 && !UNITY_EDITOR
         else if (joysticks[0] != "")
#else
         else if (joysticks[0] == "Wireless Controller")
#endif
         {
            mController = ControllerType.DS4;
         }
      }

      // Log
      String hmd = "None";
      if (mHasHmd)
      {
         if (IsSteamVR)
            hmd = "SteamVR";
         else
         {
#if UNITY_PS4 && !UNITY_EDITOR
            hmd = "PSVR";
#else
            hmd = "Oculus";
#endif
         }
      }
      Debug.Log("Bike:" + mBikeState.Type + " Hmd:" + hmd + " Controller:" + mController);
   }

   protected virtual void OnDestroy()
   {
      VZPlugin.CloseBike();
   }

   float ControllerSpeedAxis()
   { 
      // 0 to 1 from trigger
      switch (mController)
      {
         case ControllerType.Keyboard: 
            return Input.GetAxis("speed_keyboard");
         case ControllerType.Logitech: 
            return Input.GetAxis("speed_logitech");
         case ControllerType.Xbox:
	         return Input.GetAxis("speed_xbox");
         case ControllerType.X360:
            return Input.GetAxis("speed_x360");
         case ControllerType.DS4:
         {
#if UNITY_PS4 && !UNITY_EDITOR
            return Input.GetAxis("speed_ps4");
#else
            return Input.GetAxis("speed_pc_ps4");
#endif
         }
         default:
            return 0;            
      }
   }

   float ControllerPitchAxis()
   {
      switch (mController)
      {
         case ControllerType.Keyboard:
            return Input.GetAxis("pitch_keyboard");
         case ControllerType.Logitech:
            return Input.GetAxis("pitch_logitech");
         case ControllerType.Xbox:
	         return Input.GetAxis("pitch_keyboard");
         case ControllerType.X360:
            return Input.GetAxis("pitch_x360");
         case ControllerType.DS4:
#if UNITY_PS4 && !UNITY_EDITOR
            return Input.GetAxis("pitch_ps4");
#else
            return Input.GetAxis("pitch_pc_ps4");
#endif
         default:
            return 0;            
      }
   }

   float ControllerYawAxis()
   {
      switch (mController)
      {
#if UNITY_PS4 && !UNITY_EDITOR
         case ControllerType.DS4:
            return Input.GetAxis("yaw_ps4");
#endif
         case ControllerType.Xbox:
	         return Input.GetAxis("yaw_xbox");
         case ControllerType.X360:
            return Input.GetAxis("yaw_x360");
         default:
            return Input.GetAxis("yaw");
      }
   }

   bool ControllerRightButton()
   {
#if UNITY_PS4 && !UNITY_EDITOR
      return Input.GetAxis("right_ps4") > 0.01f;
#else
      return Input.GetButton("right");
#endif
   }

   bool ControllerLeftButton()
   {
#if UNITY_PS4 && !UNITY_EDITOR
      return Input.GetAxis("left_ps4") > 0.01f;
#else
      return Input.GetButton("left");
#endif
   }

   float ControllerLean()
   {
      switch (mController)
      {
         case ControllerType.Xbox:
	         return Input.GetAxis("lean_xbox");
         case ControllerType.X360:
             return Input.GetAxis("lean_x360");
         default:
	         return Input.GetAxis("lean");
      }
   }

   protected virtual void Update()
   {
#if UNITY_PS4 && !UNITY_EDITOR
      // HACK still not fixed in Unity 5.4
      if (mPS4RenderScale != 0)
      {
         mPS4RenderScale--;
         UnityEngine.VR.VRSettings.renderScale = 1.4f;
      }
#endif

      // Quit on ESCAPE
      if (Input.GetKey("escape"))
         Application.Quit();

#if VZ_GAME
      // Get replay record
      if (VZReplay.Playback())
      {
         VZReplay.Record record = VZReplay.Instance.GetRecord();

         if (record != null)
         {
            InputSpeed = record.inputSpeed;

            LeftButton.Set(record.leftButton);
            RightButton.Set(record.rightButton);
            DpadUp.Set(record.dpadUp);
            DpadDown.Set(record.dpadDown);
            DpadLeft.Set(record.dpadLeft);
            DpadRight.Set(record.dpadRight);
            RightUp.Set(record.rightUp);
            RightDown.Set(record.rightDown);
            RightLeft.Set(record.rightLeft);
            RightRight.Set(record.rightRight);

            mCamera.localRotation = record.headRotation;
            mCamera.localPosition = record.headPosition;
         }
      }
      else
#endif
      {
         // Get speed and buttons
         if (mBikeState.Type >= 0)
         {
            VZPlugin.UpdateBike(ref mBikeState, Time.time);

            InputSpeed = mBikeState.Speed;

            LeftButton.Set(mBikeState.LeftTrigger);
            RightButton.Set(mBikeState.RightTrigger);
            DpadUp.Set(mBikeState.DpadUp);
            DpadDown.Set(mBikeState.DpadDown);
            DpadLeft.Set(mBikeState.DpadLeft);
            DpadRight.Set(mBikeState.DpadRight);
            RightUp.Set(mBikeState.RightUp);
            RightDown.Set(mBikeState.RightDown);
            RightLeft.Set(mBikeState.RightLeft);
            RightRight.Set(mBikeState.RightRight);
         }
#if UNITY_EDITOR || UNITY_PS4
         else
         {
            // Read from joypad/keyboard if no serial
            InputSpeed = ControllerSpeedAxis() * kControllerMaxSpeed;
            LeftButton.Set(ControllerLeftButton());
            RightButton.Set(ControllerRightButton());
         }
#endif

         // Rotate head without hmd 
         if (!mHasHmd)
         {
            mControllerYaw = Mathf.SmoothDamp(mControllerYaw, ControllerYawAxis() * 2 * Mathf.Rad2Deg, ref mControllerYawVel, 1);
            mControllerPitch = Mathf.SmoothDamp(mControllerPitch, ControllerPitchAxis() * Mathf.Rad2Deg, ref mControllerPitchVel, 1);
            mCamera.localEulerAngles = new Vector3(mControllerPitch, mControllerYaw, 0);

		      //Debug.Log(string.Format("yaw = {0}, pitch = {1}", ControllerYawAxis(), ControllerPitchAxis()));
	      }

   		// Translate head without hmd 
         if (!mHasHmd)
         {
            mControllerLean = Mathf.SmoothDamp(mControllerLean, ControllerLean() / 4, ref mControllerLeanVel, 0.5f);
            mControllerBend = Mathf.SmoothDamp(mControllerBend, Input.GetAxis("bend") / 4, ref mControllerBendVel, 0.5f);

		    //Debug.Log(string.Format("lean = {0}, bend = {1}", ControllerLean(), Input.GetAxis("bend")));

            Vector3 pos = mCamera.localPosition;
            pos.x = mControllerLean;
            pos.z = mControllerBend;
            mCamera.localPosition = pos;
         }
#if VZ_EMULATE_GEARVR
         else if (mBikeState.Type >= 0)
         {
            float lean = 0;
            float bend = 0;

            if (mBikeState.DpadLeft)
               lean = -1;
            else if (mBikeState.DpadRight)
               lean = 1;
            if (mBikeState.DpadUp)
               bend = 1;
            else if (mBikeState.DpadDown)
               bend = -1;

            mControllerLean = Mathf.SmoothDamp(mControllerLean, lean / 4, ref mControllerLeanVel, 0.5f);
            mControllerBend = Mathf.SmoothDamp(mControllerBend, bend / 4, ref mControllerBendVel, 0.5f);

            Vector3 pos = mCamera.localPosition;
            pos.x = mControllerLean;
            pos.z = mControllerBend;
            mCamera.localPosition = pos;
         }
#endif
      }

      // Update Head from Camera and CameraOffset
      float ang = -mCameraOffset.localEulerAngles.y * Mathf.Deg2Rad;
      float sinAng = Mathf.Sin(ang);
      float cosAng = Mathf.Cos(ang);
      float x = -mCamera.localPosition.z * sinAng + mCamera.localPosition.x * cosAng + mCameraOffset.localPosition.x;
      float z = mCamera.localPosition.z * cosAng + mCamera.localPosition.x * sinAng + mCameraOffset.localPosition.z;
      float y = mCamera.localPosition.y + mCameraOffset.localPosition.y;

      Head.localPosition = new Vector3(x, y, z);

      Vector3 localAngles = mCamera.localEulerAngles;
      localAngles.y += mCameraOffset.localEulerAngles.y;
      Head.localEulerAngles = localAngles;

      // Update button data
      LeftButton.Update();
      RightButton.Update();
      DpadUp.Update();
      DpadDown.Update();
      DpadLeft.Update();
      DpadRight.Update();
      RightUp.Update();
      RightDown.Update();
      RightLeft.Update();
      RightRight.Update();

      // Get head rot
      HeadRot = -Head.localEulerAngles.y * Mathf.Deg2Rad;

      if (HeadRot < -Mathf.PI)
         HeadRot += Mathf.PI * 2;

      // Get head lean & bend
      HeadLean = -Head.localPosition.x;
      HeadBend = Head.localPosition.z;

      // Subtract rot from lean/bend if hmd (controller doesn't simulate head width)
      if (mHasHmd)
      {
#if UNITY_PS4 && !UNITY_EDITOR
         /* PS4 seems to move less
         HeadLean *= 1.25f;
         HeadBend *= 1.50f;
         */
#endif

         // Adjust lean
         if (Mathf.Abs(HeadLean) < kHeadDead)
         {
            HeadLean = 0;
         }
         else if (HeadLean > 0)
         {
            if (HeadRot > Mathf.PI / 2.0f)
               HeadLean -= kHeadWidth;
            else 
               HeadLean -= kHeadWidth * Mathf.Sin(HeadRot); 

            HeadLean -= kHeadDead;

            if (HeadLean < 0)
               HeadLean = 0;
         }
         else
         {
            if (HeadRot < -Mathf.PI / 2.0f)
               HeadLean += kHeadWidth;
            else 
               HeadLean -= kHeadWidth * Mathf.Sin(HeadRot); 

            HeadLean += kHeadDead;

            if (HeadLean > 0)
               HeadLean = 0;
         }

         // Adjust bend
         float headPitch = Head.localEulerAngles.x * Mathf.Deg2Rad;

         if (headPitch < -Mathf.PI)
            headPitch += Mathf.PI * 2;

         HeadBend += kHeadWidth * (1 - Mathf.Cos(headPitch)); 
      }

      // Update distance

      // Adjust "real" speed by resistance factor, which isn't yet incorporated into InputSpeed because all
      // current games don't want it there
      float realSpeed = InputSpeed * ResistanceFactor();

      Distance += Mathf.Abs(realSpeed) * Time.deltaTime;
   }

   protected virtual float ResistanceFactor()
   {
      // Resistance has been experimentally determined to be 0.534 + 0.2372(r), where r is resistance setting.
      //
      // But don't have min/max resistance here, so assume resistance setting of 3. BikeState.InputSpeed was tuned at resistance 5,
      // so ratio of that resistance at 3

      return 0.724f;
   }
}

