using UnityEngine;
using System.Collections;

public class VZColliderInfo : MonoBehaviour
{
   public float SpeedMultiplier = 1.0f;
   
   public enum GroundTypes
   {
      None,
      Dirt,
      Grass,
      Pavement,
      Water
   };
   public GroundTypes GroundType = GroundTypes.None;
}
