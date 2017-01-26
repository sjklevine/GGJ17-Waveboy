using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTK.SecondaryControllerGrabActions;

public class VRTK_WaveBoyCustomGrabAction : VRTK_SwapControllerGrabAction {

    public GameObject thisPrefab;
    private Transform savedParent;

    public void Start()
    {
        savedParent = this.transform.parent;
    }

    /// <summary>
    /// The OnDropAction method is executed when the current gra0bbed object is dropped and can be used up to clean up any secondary grab actions.
    /// </summary>
    public override void OnDropAction()
    {
        GameObject playerTransform = (GameObject) GameObject.FindGameObjectWithTag("Player");

        Vector3 addedVelocity = playerTransform.GetComponent<Rigidbody>().velocity;
        Vector3 cubeVelocity = this.GetComponent<Rigidbody>().velocity;
        this.GetComponent<Rigidbody>().velocity += addedVelocity;
        Vector3 finalVelocity = this.GetComponent<Rigidbody>().velocity;
        //Debug.Log("ON DROP ACTION, velocity = " + cubeVelocity + ", add = " + addedVelocity + ", total = " + finalVelocity);

        // Spawn THREE new papers in the cart!  This ensures you never run out, and is also pleasantly silly.
        makeNewGuy(addedVelocity);
        makeNewGuy(addedVelocity);
        makeNewGuy(addedVelocity);

        // To aid in being able to wave after you toss, lock out waves for a period after dropping.
        //playerTransform.GetComponent<WaveBoyWaveDetector>().ResetLockoutTimer();
        WaveBoyWaveDetector.triggerLockout = true;
    }

    public void makeNewGuy(Vector3 addedVelocity)
    {
        GameObject newGuy = (GameObject)GameObject.Instantiate(thisPrefab, savedParent.transform.position + Vector3.up * 0.1f, Quaternion.identity);
        newGuy.transform.SetParent(savedParent, true);
        newGuy.GetComponent<Rigidbody>().velocity += addedVelocity;
        newGuy.GetComponent<Rigidbody>().isKinematic = false;

    }
}