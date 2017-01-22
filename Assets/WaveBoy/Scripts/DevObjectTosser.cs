using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevObjectTosser : MonoBehaviour {
    public GameObject TossedObjectPrefab;
    public GameObject scorePrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Debug.Log("DEBUG: TOSSING A PAPER");
            Vector3 fwd = this.transform.forward;
            Vector3 tossStart = this.transform.position + fwd * 1.0f;
            GameObject tossedObject = (GameObject)Instantiate(TossedObjectPrefab, tossStart, this.transform.rotation);
            tossedObject.GetComponent<Rigidbody>().isKinematic = false;
            tossedObject.GetComponent<Rigidbody>().AddTorque(Vector3.up * 100f);
            tossedObject.GetComponent<Rigidbody>().AddForce(fwd * 1000f);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Debug.Log("DEBUG: WAVING AT POINT");
            // Let's write this WAVE FUNCTION!
            // So let's assume that the "wave" vector direction is just fwd this time.
            // Later it should be from the player, through the hand position.
            Vector3 fwd = this.transform.forward;
            DoWaveFromDirectionVector(fwd);
        }
    }

    private void DoWaveFromDirectionVector(Vector3 fwd)
    {

        Ray originalFamousRay = new Ray(this.transform.position, fwd);

        // We actually want to do like a ton MORE rays in a box around this one,
        // and wave at whoever they all hit.  Raycasts are cheap!
        int numRaycastsX = 10;
        int numRaycastsY = 10;
        float rayBoxWidth = 5f;
        float rayBoxHeight = 5f;
        bool triggeredWave = false;
        for (int i = 0; i < numRaycastsX; i++)
        {
            for (int j = 0; j < numRaycastsY; j++)
            {
                // Make it ray
                Vector3 localDirectionAdjust = new Vector3(-rayBoxWidth / 2f + rayBoxWidth * i / (numRaycastsX - 1),
                                                           -rayBoxHeight / 2f + rayBoxHeight * j / (numRaycastsY - 1),
                                                           0);
                Vector3 newOrigin = originalFamousRay.origin + this.transform.TransformDirection(localDirectionAdjust);
                Ray newRay = new Ray(newOrigin, originalFamousRay.direction);
                RaycastHit hit;
                float rayDist = 100f;

                if (!triggeredWave && Physics.Raycast(newRay, out hit, rayDist))
                {
                    // Did we hit somebody?  If so, it counts as a wave, but only if they're not already waving.
                    if (hit.collider.gameObject.tag == "Person")
                    {
                        Debug.DrawLine(newRay.origin, hit.point, Color.magenta, 4.0f);

                        //Are they waving?
                        Animator personAnim = hit.collider.gameObject.GetComponent<Animator>();
                        int upperBodyLayer = personAnim.GetLayerIndex("UpperBody");
                        AnimatorStateInfo currentStateInfo = personAnim.GetCurrentAnimatorStateInfo(upperBodyLayer);
                        bool isWaving = currentStateInfo.IsName("UpperBody.OnWave");
                        if (!isWaving) {
                            // MAKE 'EM WAVE
                            personAnim.SetTrigger("OnWave");

                            // Pop a nice score object!
                            GameObject scoreObj = (GameObject)GameObject.Instantiate(scorePrefab, hit.point, Camera.main.transform.rotation);
                            ScoreShownOnHit scoreScript = scoreObj.GetComponent<ScoreShownOnHit>();
                            scoreScript.UpdateTextAndGo("+1000");

                            // TODO: Tell the gamemanager!
                            //GameManager.instance.score +=00 pointsScored;

                            triggeredWave = true;
                        }
                    }
                }
                else
                {
                    // No hit, but pretty editor line
                    Debug.DrawLine(newRay.origin, newRay.origin + newRay.direction * rayDist, Color.white, 4.0f);
                }

            }
        }
    }
}
