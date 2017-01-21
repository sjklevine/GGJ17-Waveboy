//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

[StructLayout(LayoutKind.Sequential, Pack=8)]
public struct VZMotionInput
{
   public float DownhillFactor;
   public float UphillFactor;
   public float ControllerHeadLean;
   public float LeanFudge;
   public float ApparentLean;
   public float ControllerHeadRot;
   public float SpeedFudge;
   public float MaxTurn;
   public float StoppedTurnFraction;
   public float BodyRot;
   public float Scale;
   public float LandingHardness;
   public float LandingRadius;
   public float ControllerInputSpeed;
   public float SpeedMultiplier;
   public float SpeedMultiplierSpeedSettleTime;
   public float SpeedSettleTimeWhenAccelerating;
   public float SpeedSettleTimeWhenBraking;
   public float MaxSpeed;
   public float MaxVertSpeed;
   public float ColliderSpeedMultiplier;
   public float VelocityX;
   public float VelocityY;
   public float VelocityZ;
   public float DeltaTime;
   public float HitDistance;
   public float HitNormalX;
   public float HitNormalY;
   public float HitNormalZ;
   public float PlayerX;
   public float PlayerY;
   public float PlayerZ;
   [MarshalAs(UnmanagedType.U1)]
   public bool Colliding;
   [MarshalAs(UnmanagedType.U1)]
   public bool OnMenu;
   [MarshalAs(UnmanagedType.U1)]
   public bool AllowDrift;
   [MarshalAs(UnmanagedType.U1)]
   public bool AllowYaw;
   [MarshalAs(UnmanagedType.U1)]
   public bool AllowPitch;
   [MarshalAs(UnmanagedType.U1)]
   public bool AllowRoll;
   [MarshalAs(UnmanagedType.U1)]
   public bool Reverse;
   [MarshalAs(UnmanagedType.U1)]
   public bool RaycastResult;
   [MarshalAs(UnmanagedType.U1)]
   public bool RotateAtStop;

   public void Init()
   {
      DownhillFactor = 0.5f;
      UphillFactor = 1.414f;
      ControllerHeadLean = 0;
      LeanFudge = 2;
      ApparentLean = 0.5f;
      ControllerHeadRot = 0;
      SpeedFudge = 1;
      MaxTurn = 0.5f;
      StoppedTurnFraction = 0;
      BodyRot = 0;
      Scale = 1;
      LandingHardness = 2;
      LandingRadius = 0.25f;
      ControllerInputSpeed = 0;
      SpeedMultiplier = 1;
      SpeedMultiplierSpeedSettleTime = 3;
      SpeedSettleTimeWhenAccelerating = 3;
      SpeedSettleTimeWhenBraking = 3;
      MaxSpeed = 12;
      MaxVertSpeed = 4;
      ColliderSpeedMultiplier = 1;
      VelocityX = 0;
      VelocityY = 0;
      VelocityZ = 0;
      DeltaTime = 0.016f;
      HitDistance = 0;
      HitNormalX = 0;
      HitNormalY = 1;
      HitNormalZ = 0;
      PlayerX = 0;
      PlayerY = 0;
      PlayerZ = 0;
      Colliding = true;
      OnMenu = false;
      AllowDrift = true;
      AllowYaw = true;
      AllowPitch = false;
      AllowRoll = false;
      Reverse = false;
      RaycastResult = false;
      RotateAtStop = false;
   }
}

[StructLayout(LayoutKind.Sequential, Pack=8)]
public struct VZMotionOutput
{
   public float BodyRot;
   public float RotVel;
   public float Turn;
   public float VelDot;
   public float TransverseDot;
   public float Speed;
   public float Lean;
   public float TransformEulerX;
   public float TransformEulerY;
   public float TransformEulerZ;
   public float VelocityX;
   public float VelocityY;
   public float VelocityZ;
   public float PlayerX;
   public float PlayerY;
   public float PlayerZ;
   [MarshalAs(UnmanagedType.U1)]
   public bool LandingSmoothed;
}

[StructLayout(LayoutKind.Sequential, Pack=8)]
public struct VZBikeState
{
   public float Timestamp;
   public float TimeAtLastPulseMs;
   public float HeartRate;
   public float BatteryVolts;
   public float Speed;
   public int Pulses;
   public int FilteredResistance;
   public int Type;
   public int BetaVersion;
   [MarshalAs(UnmanagedType.U1)]
   public bool LeftTrigger;
   [MarshalAs(UnmanagedType.U1)]
   public bool DpadUp;
   [MarshalAs(UnmanagedType.U1)]
   public bool DpadDown;
   [MarshalAs(UnmanagedType.U1)]
   public bool DpadLeft;
   [MarshalAs(UnmanagedType.U1)]
   public bool DpadRight;
   [MarshalAs(UnmanagedType.U1)]
   public bool RightTrigger;
   [MarshalAs(UnmanagedType.U1)]
   public bool RightUp;
   [MarshalAs(UnmanagedType.U1)]
   public bool RightDown;
   [MarshalAs(UnmanagedType.U1)]
   public bool RightLeft;
   [MarshalAs(UnmanagedType.U1)]
   public bool RightRight;
   [MarshalAs(UnmanagedType.U1)]
   public bool Connected;
   public byte Sender0;
   public byte Sender1;
   public byte Sender2;
   public byte Sender3;
   public byte Sender4;
   public byte Sender5;

   public string Sender()
   {
      char[] s = new char[12];
      s[0] = HexChar(Sender0 & 0xF);
      s[1] = HexChar(Sender0 >> 4);
      s[2] = HexChar(Sender1 & 0xF);
      s[3] = HexChar(Sender1 >> 4);
      s[4] = HexChar(Sender2 & 0xF);
      s[5] = HexChar(Sender2 >> 4);
      s[6] = HexChar(Sender3 & 0xF);
      s[7] = HexChar(Sender3 >> 4);
      s[8] = HexChar(Sender4 & 0xF);
      s[9] = HexChar(Sender4 >> 4);
      s[10] = HexChar(Sender5 & 0xF);
      s[11] = HexChar(Sender5 >> 4);

      return new string(s);
   }

   char HexChar(int i)
   {
      switch (i)
      {
         case 0: return '0';
         case 1: return '1';
         case 2: return '2';
         case 3: return '3';
         case 4: return '4';
         case 5: return '5';
         case 6: return '6';
         case 7: return '7';
         case 8: return '8';
         case 9: return '9';
         case 10: return 'A';
         case 11: return 'B';
         case 12: return 'C';
         case 13: return 'D';
         case 14: return 'E';
         case 15: return 'F';
         default: return '?';
      }
   }
}

public enum VZBikeType
{
   NeedsDriver = -2,
   None = -1,
   Wired = 0,
   Alpha = 1,
   Beta = 2
}

public static class VZPlugin
{
   [DllImport("VZPlugin")]
   public static extern void StartCounter();

   [DllImport("VZPlugin")]
   public static extern double GetCounter(bool print);

#if UNITY_PS4 && !UNITY_EDITOR
	[DllImport("VZPlugin")]
	public static extern bool PS4HeadsetConnected();

	[DllImport("VZPlugin")]
	static extern void PS4Print(IntPtr msg);

   public static void PS4Print(string str)
   {
      IntPtr msg = Marshal.StringToHGlobalAnsi(str);
      PS4Print(msg);
      Marshal.FreeHGlobal(msg);
   }

	[DllImport("VZPlugin")]
	static extern void PS4SendTelemetry(IntPtr bikeProfile, IntPtr body);

   public static void PS4SendTelemetry(string bikeProfile, string body)
   {
      IntPtr _bikeProfile = Marshal.StringToHGlobalAnsi(bikeProfile);
      IntPtr _body = Marshal.StringToHGlobalAnsi(body);
      PS4SendTelemetry(_bikeProfile, _body);
      Marshal.FreeHGlobal(_bikeProfile);
      Marshal.FreeHGlobal(_body);
   }
#else

   [DllImport("VZPlugin")]
   public static extern void PCShowConsole();

   [DllImport("VZPlugin")]
   static extern IntPtr PCGraphicsDriverVersion();

   public static void PCGraphicsDriverVersion(out string version)
   {
      IntPtr buffer = PCGraphicsDriverVersion();
      version = Marshal.PtrToStringAnsi(buffer);
   }

   [DllImport("VZPlugin")]
   static extern IntPtr PCOculusVersion();

   public static void PCOculusVersion(out string version)
   {
      IntPtr buffer = PCOculusVersion();
      version = Marshal.PtrToStringAnsi(buffer);
   }
#endif

   [DllImport("VZPlugin")]
   static extern void VZSetGameName(IntPtr msg);

   public static void VZSetGameName(string str)
   {
      IntPtr msg = Marshal.StringToHGlobalAnsi(str);
      VZSetGameName(msg);
      Marshal.FreeHGlobal(msg);
   }
   [DllImport("VZPlugin", EntryPoint="VZInit")]
   static extern void VZInit();

   public static void Init(string dllPath = null)
   {
      // Init plugin dir
      if (dllPath != null)
      {
#if UNITY_EDITOR_32
         // nothing needed
#elif UNITY_EDITOR_64
         // nothing needed
#else
         // Player
         // add plugins path to the environment for the steam dll.
         var currentPath = Environment.GetEnvironmentVariable("PATH",
            EnvironmentVariableTarget.Process);

         if (currentPath != null && currentPath.Contains(dllPath) == false)
            Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator + dllPath, EnvironmentVariableTarget.Process);
#endif
      }

      VZInit();
   }

   [DllImport("VZPlugin", EntryPoint="VZResetMotion")]
   public static extern void ResetMotion();

   [DllImport("VZPlugin", EntryPoint="VZSetTurnSettleTime")]
   public static extern void SetTurnSettleTime(float time);

   [DllImport("VZPlugin", EntryPoint="VZUpdateMotion")]
   public static extern void UpdateMotion(ref VZMotionInput input, ref VZMotionOutput output);

   [DllImport("VZPlugin", EntryPoint="VZConnectBike")]
   public static extern void ConnectBike(ref VZBikeState state);

   [DllImport("VZPlugin", EntryPoint="VZUpdateBike")]
   public static extern bool UpdateBike(ref VZBikeState state, float time);

   [DllImport("VZPlugin", EntryPoint="VZCloseBike")]
   public static extern void CloseBike();
}
