using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.SecondaryControllerGrabActions;

public class VRTK_SwapControllerPlusVelocityGrabAction : VRTK_SwapControllerGrabAction {

    /// <summary>
    /// The OnDropAction method is executed when the current grabbed object is dropped and can be used up to clean up any secondary grab actions.
    /// </summary>
    public override void OnDropAction()
    {
        GameObject playerTransform = (GameObject) GameObject.FindGameObjectWithTag("Player");

        Vector3 addedVelocity = playerTransform.GetComponent<Rigidbody>().velocity;
        Vector3 cubeVelocity = this.GetComponent<Rigidbody>().velocity;
        this.GetComponent<Rigidbody>().velocity += addedVelocity;
        Vector3 finalVelocity = this.GetComponent<Rigidbody>().velocity;
        //Debug.Log("ON DROP ACTION, velocity = " + cubeVelocity + ", add = " + addedVelocity + ", total = " + finalVelocity);
    }
}