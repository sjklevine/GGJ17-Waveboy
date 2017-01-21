//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

#if VZ_GAME
#define VZ_PLAYMAKER
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

#if VZ_PLAYMAKER
using HutongGames.PlayMaker;
#endif

#if VZ_GAME
public class VZPlayer : Photon.PunBehaviour
#else
public class VZPlayer : MonoBehaviour
#endif
{
   //***********************************************************************
   // PUBLIC API
   //***********************************************************************

   public float SpeedFudge = 1.0f;
   public float DownhillFactor = 0.5f;
   public float UphillFactor = 1.414f;
   public float MaxVertSpeed = 4.0f;
   public float MaxTurn = 0.5f;
   public float MaxSpeed = 12.0f;
   public float LeanFudge = 2.0f;
   public float LeanIn = 0.5f;
   public float LandingHardness = 2.0f;
   public float LandingRadius = 0.25f;
   public float NeckHeight = 0.0f;
   public bool AllowRotate = true;
   public bool AllowPitch = true;
   public bool AllowRoll = false;
   public bool AllowDrift = true;
   public bool RotateAtStop = false;
   public GameObject BodyPrefab;
   public float NearClipPlane = 0.2f;
   public float FarClipPlane = 3000;
   public float SlowRotateLimit = 0.0f; // amount of turn at zero speed
   public bool Reverse = false;

   public static VZPlayer Instance { get; private set; }
    public VZController Controller;

   public void Initialize(Vector3 position, Quaternion rotation)
   {
      Rigidbody rb = GetComponent<Rigidbody>();
      if (rb != null)
      {
         rb.MovePosition(position);
         rb.MoveRotation(rotation);
      }

      transform.position = position;
      transform.rotation = rotation;

      mBodyRot = Mathf.Atan2(transform.forward.z, transform.forward.x);

      UpdateNeck();

      mInitialPos = transform.position;
      mInitialRot = Mathf.Atan2(transform.forward.z, transform.forward.x);
      mLiftOffHeight = transform.position.y;

      // Internal values were tuned for actual scale of 0.5
      mScale = transform.localScale.x * 2.0f;
   }

   public int RaycastMask()
   {
      return mRaycastMask;
   }

   public float RotVel()
   {
      return mRotVel;
   }

   public float BodyRot()
   {
      return mBodyRot;
   }

   public float Altitude()
   {
      if (mColliding)
         return 0;
      else
         return transform.position.y - mLiftOffHeight;
   }

   public bool Colliding()
   {
      return mColliding;
   }

   public float Speed()
   {
      return mSpeed;
   }

   public float Lean()
   {
      return mLean;
   }

   public float HeadYaw()
   {
      Vector3 forward = Controller.Head.TransformDirection(Vector3.forward);

      return Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
   }

   public float Turn()
   {
      return mTurn;
   }

   public float Scale()
   {
      return mScale;
   }

   public bool NormalMode()
   {
      return State() == kStateNormal;
   }

   //***********************************************************************
   // PROTECTED API
   //***********************************************************************

   protected const string kStateCalibrating = "Calibrating";
   protected const string kStateNormal = "Normal";

   protected float mSpeedMultiplier = 1;
   protected float mInitialRot;
   protected float mScale;
   protected int mRaycastMask;
   protected float mSpeedSettleTimeWhenAccelerating = 3.0f;
   protected float mSpeedSettleTimeWhenBraking = 3.0f;
   protected VZColliderInfo mColliderInfo = null;
   protected bool mIsQuitting = false;
   protected VZMotionInput mInput = new VZMotionInput();
   protected float mPower = 0.0f;
   protected string mPrevState = "";

   protected string State()
   {
      return mState;
   }

   protected float Power()
   {
      return mPower;
   }

   protected void SetState(string state)
   {
      Debug.Log("from " + mState + " to " + state);
      mPrevState = mState;
      mState = state;
   }

   protected virtual void Awake()
   {
      Instance = this;

      // Instantiate controller prefab
      if (Controller == null)
      {
         GameObject go = Instantiate(BodyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
         Controller = go.GetComponent<VZController>();
         Controller.transform.localScale = Vector3.one;
         Controller.name = "VZController";
         DontDestroyOnLoad(Controller);
      }

      // Reparent it to us
      Controller.transform.SetParent(transform, false);

      // Raycast mask
      mRaycastMask = ~(LayerMask.GetMask("VZPlayerCollider") | LayerMask.GetMask("Ignore Raycast") | LayerMask.GetMask("VZObjectCollider"));

#if VZ_PLAYMAKER
      // Cache PlayMaker FSM variable references.
      mPlayMakerVariables.Init();
#endif
   }

   protected virtual void Start()
   {
      mInput.OnMenu = false;

      // Transition canvas
      Controller.TransitionCanvas().GetComponent<Canvas>().enabled = true;

      // Override clip planes
      Controller.Camera().nearClipPlane = NearClipPlane;
      Controller.Camera().farClipPlane = FarClipPlane;

      Transform raycastOrigin = transform.Find("RaycastOrigin");
      if (raycastOrigin != null)
      {
         mRaycastOffset = raycastOrigin.localPosition;
      }

      Controller.Neck().localPosition = new Vector3(0, NeckHeight, 0);

      // Initialize rot and pos
      Initialize(transform.position, transform.rotation);

      GetComponent<Rigidbody>().freezeRotation = true;

      // Either calibrate or fade down now
      if (sFirstTime)
      {
         sFirstTime = false;

         // Goto initial state
         SetState(InitialState());
      }
      else
        {
            Debug.Log("Restart on start");
            Restart(true);
      }
   }

   protected virtual string InitialState()
   {
      return kStateCalibrating;
   }

   // Update rotation here to avoid blurring fixed objects in oculus
	protected virtual void Update()
   {
      // Update based on state
      if (State() == kStateCalibrating)
      {
         UpdateCalibrating();
      }
      else if (State() == kStateNormal)
      {
         UpdateNormal();
      }

#if VZ_PLAYMAKER
      // Update PlayMaker variables.
      mPlayMakerVariables.Update();
#endif
   }

   protected virtual void OnDestroy()
   {
      // Reset globals for next play mode
      if (mIsQuitting)
      {
         Destroy(Controller);
         Controller = null;

         sFirstTime = true;
      }
      // Else unparent shared model and destroy hud we gave it
      else
      {
         Controller.transform.SetParent(null, false);
      }
   }

   protected virtual void OnCollisionEnter(Collision collision)
   {
      mColliding = true;
   }

   protected virtual void OnCollisionStay(Collision collision)
   {
      mColliding = true;
   }

   protected virtual void OnCollisionExit(Collision collision)
   {
      mColliding = false;
      mLiftOffHeight = transform.position.y;
   }

   protected virtual void Restart(bool initLevel)
   {
        
        if (initLevel)
      {
#if VZ_PLAYMAKER
         // Let PlayMaker FSMs know that a restart took place.
         mPlayMakerVariables.PlayerRestarted.Value = true;
#endif
      }

      // Reset transition canvas alpha
      Controller.TransitionCanvas().GetComponent<CanvasGroup>().alpha = 1;

      // Reset controller
      Controller.Restart();

      // Reset state
      mSpeedMultiplier = 1.0f;

      // Reset initial position and rotation
      transform.position = mInitialPos;
      GetComponent<Rigidbody>().velocity = Vector3.zero;
      mBodyRot = mInitialRot;

      VZPlugin.ResetMotion();

      SetState(kStateNormal);

      StartCoroutine(FadeDown(2));
   }

   protected virtual void LandingSmoothed()
   {
      // TODO for subclass to use
   }

   protected virtual void UpdateNormal()
   {
      // Fill input
      mInput.DownhillFactor = DownhillFactor;
      mInput.UphillFactor = UphillFactor;
      mInput.ControllerHeadLean = Controller.HeadLean;
      mInput.LeanFudge = LeanFudge;
      mInput.ApparentLean = LeanIn;
      mInput.SpeedFudge = SpeedFudge;
      mInput.MaxTurn = MaxTurn;
      mInput.StoppedTurnFraction = SlowRotateLimit;
      mInput.AllowDrift = AllowDrift;
      mInput.AllowYaw = AllowRotate;
      mInput.AllowPitch = AllowPitch;
      mInput.AllowRoll = AllowRoll;
      mInput.RotateAtStop = RotateAtStop;
      mInput.ControllerHeadRot = Controller.HeadRot;
      mInput.Colliding = mColliding;
      mInput.BodyRot = mBodyRot;
      mInput.Scale = mScale;
      mInput.LandingHardness = LandingHardness;
      mInput.LandingRadius = LandingRadius;
      mInput.ControllerInputSpeed = Controller.InputSpeed;
      mInput.SpeedMultiplier = mSpeedMultiplier;
      mInput.SpeedMultiplierSpeedSettleTime = mSpeedMultiplierSpeedSettleTime;
      mInput.SpeedSettleTimeWhenAccelerating = mSpeedSettleTimeWhenAccelerating;
      mInput.SpeedSettleTimeWhenBraking = mSpeedSettleTimeWhenBraking;
      mInput.MaxSpeed = MaxSpeed;
      mInput.MaxVertSpeed = MaxVertSpeed;
      mInput.Reverse = Reverse;
      mInput.DeltaTime = Time.deltaTime;
      mInput.PlayerX = transform.position.x;
      mInput.PlayerY = transform.position.y;
      mInput.PlayerZ = transform.position.z;

      Vector3 velocity = GetComponent<Rigidbody>().velocity;
      mInput.VelocityX = velocity.x;
      mInput.VelocityY = velocity.y;
      mInput.VelocityZ = velocity.z;

      Vector3 rayOrigin = transform.position + transform.TransformVector(mRaycastOffset);
      Vector3 rayDir = Vector3.down;
      RaycastHit hit;
      mInput.RaycastResult = Physics.Raycast(rayOrigin, rayDir, out hit, 1000.0f, mRaycastMask);

      if (mInput.RaycastResult)
      {
         mColliderInfo = hit.collider.GetComponent<VZColliderInfo>();
         mInput.HitDistance = hit.distance;
         mInput.HitNormalX = hit.normal.x;
         mInput.HitNormalY = hit.normal.y;
         mInput.HitNormalZ = hit.normal.z;
      }
      else
      {
         mColliderInfo = null;
      }

      if (mColliderInfo != null)
      {
         mInput.ColliderSpeedMultiplier = mColliderInfo.SpeedMultiplier;
      }
      else
      {
         mInput.ColliderSpeedMultiplier = 1;
      }

      // Move state
      VZPlugin.UpdateMotion(ref mInput, ref mOutput);

      // Calculate our power
      const float kMass = 68.0f; // todo: get this from the user
      mPower = CalculatePower(Time.deltaTime, kMass, mSpeed, mOutput.Speed);

      // Handle output
      mBodyRot = mOutput.BodyRot;
      mRotVel = mOutput.RotVel;
      mTurn = mOutput.Turn;
      mSpeed = mOutput.Speed;
      mLean = mOutput.Lean;

      if (mOutput.LandingSmoothed)
         LandingSmoothed();

      transform.eulerAngles = new Vector3(mOutput.TransformEulerX, mOutput.TransformEulerY, mOutput.TransformEulerZ);

      transform.position = new Vector3(mOutput.PlayerX, mOutput.PlayerY, mOutput.PlayerZ);

      // Update velocity
      GetComponent<Rigidbody>().velocity = new Vector3(mOutput.VelocityX, mOutput.VelocityY, mOutput.VelocityZ);

#if VZ_GAME
      // Override with replay
      if (VZReplay.Playback())
      {
         VZReplay.Record record = VZReplay.Instance.GetRecord();

         if (record != null)
         {
            GetComponent<Rigidbody>().velocity = record.playerVelocity;
            mBodyRot = record.bodyRot;

            if (VZReplay.Instance.OverridePlayer)
            {
               transform.position = record.playerPosition;
               transform.rotation = record.playerRotation;
            }
         }
      }
#endif

      // Keep neck vertical
      UpdateNeck();
   }

   protected Vector3 InitialPos()
   {
      return mInitialPos; 
   }

   protected virtual void OnApplicationQuit()
   {
      mIsQuitting = true;
   }

   protected void UpdateNeck()
   {
      float rotY = -mBodyRot * Mathf.Rad2Deg + 90;
      Controller.Neck().eulerAngles = new Vector3(0, rotY, 0);

      Controller.Neck().localPosition = new Vector3(0, NeckHeight, 0);
   }

   protected virtual void UpdateCalibrating()
   {
      // Hold both buttons to calibrate
      if (Controller.LeftButton.Held(0.5f) && Controller.RightButton.Held(0.5f) && Controller.IsHeadTracked())
      {
         Controller.LeftButton.Clear();
         Controller.RightButton.Clear();
          
            Controller.Recenter();

         Restart(true);
      }
   }

   protected virtual IEnumerator FadeDown(float fadeTime)
   {
      // Fade alpha down to zero
      CanvasGroup group = Controller.TransitionCanvas().GetComponent<CanvasGroup>();
      float time = 0;
      float alpha = group.alpha;

      while (time < fadeTime)
      {
         time += Time.deltaTime;
         group.alpha = Mathf.SmoothStep(alpha, 0.0f, time / fadeTime);
         yield return null;
      }

      // Deactivate and reset alpha
      Controller.TransitionCanvas().SetActive(false);
      group.alpha = 1.0f;
   }

   //***********************************************************************
   // PRIVATE API
   //***********************************************************************

#if VZ_PLAYMAKER
   // PlayMaker FSM variables.
   class PlayMakerVariables
   {
      public FsmFloat ControllerDistance;
      public FsmVector3 ControllerHeadForward;
      public FsmVector3 ControllerHeadPosition;
      public FsmFloat ControllerHeadBend;
      public FsmFloat ControllerHeadLean;
      public FsmFloat ControllerHeadRot;
      public FsmFloat ControllerInputSpeed;
      public FsmBool ControllerLeftButtonDown;
      public FsmBool ControllerLeftButtonPressed;
      public FsmBool ControllerLeftButtonReleased;
      public FsmBool ControllerRightButtonDown;
      public FsmBool ControllerRightButtonPressed;
      public FsmBool ControllerRightButtonReleased;
      public FsmGameObject Player;
      public FsmBool PlayerRestarted;
      public FsmFloat PlayerSpeed;

      public int Hold = 0;

      public void Init()
      {
         ControllerDistance = FsmVariables.GlobalVariables.FindFsmFloat("VZController.Distance");
         ControllerHeadForward = FsmVariables.GlobalVariables.FindFsmVector3("VZController.Head.Forward");
         ControllerHeadPosition = FsmVariables.GlobalVariables.FindFsmVector3("VZController.Head.Position");
         ControllerHeadBend = FsmVariables.GlobalVariables.FindFsmFloat("VZController.Head.Bend");
         ControllerHeadLean = FsmVariables.GlobalVariables.FindFsmFloat("VZController.Head.Lean");
         ControllerHeadRot = FsmVariables.GlobalVariables.FindFsmFloat("VZController.Head.Rot");
         ControllerInputSpeed = FsmVariables.GlobalVariables.FindFsmFloat("VZController.InputSpeed");
         ControllerLeftButtonDown = FsmVariables.GlobalVariables.FindFsmBool("VZController.LeftButton.Down");
         ControllerLeftButtonPressed = FsmVariables.GlobalVariables.FindFsmBool("VZController.LeftButton.Pressed");
         ControllerLeftButtonReleased = FsmVariables.GlobalVariables.FindFsmBool("VZController.LeftButton.Released");
         ControllerRightButtonDown = FsmVariables.GlobalVariables.FindFsmBool("VZController.RightButton.Down");
         ControllerRightButtonPressed = FsmVariables.GlobalVariables.FindFsmBool("VZController.RightButton.Pressed");
         ControllerRightButtonReleased = FsmVariables.GlobalVariables.FindFsmBool("VZController.RightButton.Released");

         Player = FsmVariables.GlobalVariables.FindFsmGameObject("VZPlayer");
         PlayerRestarted = FsmVariables.GlobalVariables.FindFsmBool("VZPlayer.Restarted");
         PlayerSpeed = FsmVariables.GlobalVariables.FindFsmFloat("VZPlayer.Speed");

         Player.Value = Instance.gameObject;
         PlayerRestarted.Value = false;
      }

      public void Update()
      {
         ControllerDistance.Value = Controller.Distance;
         ControllerHeadForward.Value = Controller.Head.forward;
         ControllerHeadPosition.Value = Controller.Head.position;
         ControllerHeadBend.Value = Controller.HeadBend;
         ControllerHeadLean.Value = Instance.Lean();
         ControllerHeadRot.Value = Controller.HeadRot * Mathf.Rad2Deg;
         ControllerInputSpeed.Value = Controller.InputSpeed;
         ControllerLeftButtonDown.Value = Controller.LeftButton.Down;
         ControllerLeftButtonPressed.Value = Controller.LeftButton.Pressed();
         ControllerLeftButtonReleased.Value = Controller.LeftButton.Released();
         ControllerRightButtonDown.Value = Controller.RightButton.Down;
         ControllerRightButtonPressed.Value = Controller.RightButton.Pressed();
         ControllerRightButtonReleased.Value = Controller.RightButton.Released();

         PlayerSpeed.Value = Instance.Speed() * Instance.Scale();
      }
   }

   PlayMakerVariables mPlayMakerVariables = new PlayMakerVariables();
#endif

   bool mColliding = false;
   float mTurn = 0;
   float mLean = 0;
   float mSpeed = 0;
   float mRotVel = 0;
   VZMotionOutput mOutput = new VZMotionOutput();
   Vector3 mInitialPos;
   float mLiftOffHeight;
   float mBodyRot = 0;
   Vector3 mRaycastOffset = new Vector3(0, 1, 0);
   float mSpeedMultiplierSpeedSettleTime = 0.5f;
   string mState = "";

   static bool sFirstTime = true;

   static float CalculatePower(float deltaTime, float mass, float oldSpeed, float speed)
   {
      // from strava: https://support.strava.com/hc/en-us/articles/216917107-Power-Calculations
      // p (rolling) = cnv
      // p (gravity) = mgv * sin(arctan(grade))
      // p (acceleration) = mav

      // calculate acceleration
      float a = (speed - oldSpeed) / deltaTime;
      if (a < 0.0f)
         a = 0.0f;

      // p (rolling)
      const float c = 0.0f;   // rolling resistance = ?
      float n = 1.0f;   // not sure what this means. ?
      float g = -0.98f;
      float p_rolling = c * n*g*mass * speed;

      // p (gravity)
      float grade = 0.0f;  // angle of incline
      float p_gravity = mass * g * speed * System.Convert.ToSingle(Math.Sin(Math.Atan(grade)));

      // p (acceleration)
      float p_acceleration = mass * a * speed;

      return p_rolling + p_gravity + p_acceleration;
   }
}

