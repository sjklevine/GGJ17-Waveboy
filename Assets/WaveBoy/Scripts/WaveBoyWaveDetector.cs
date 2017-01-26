using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBoyWaveDetector : MonoBehaviour {
    public GameObject scorePrefab;
    private OvrAvatar mahAvatar;
    public GameObject voice;
    //private bool allowedToWave = true;

    void Start()
    {
        mahAvatar = this.GetComponent<OvrAvatar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mahAvatar.Driver != null)
        {
            // Get the current pose from the driver
            OvrAvatarDriver.PoseFrame pose;
            if (mahAvatar.Driver.GetCurrentPose(out pose))
            {
                // Update the various avatar components with this pose
                if (mahAvatar.ControllerLeft != null)
                {
                    if (pose.controllerLeftPose.indexTrigger == 0 && pose.controllerLeftPose.handTrigger == 0)
                    {
                        Vector3 leftDir = mahAvatar.HandLeft.transform.position - Camera.main.transform.position;
                        Ray leftRay = new Ray(Camera.main.transform.position, leftDir);
                        DoWaveFromDirectionVector(leftRay);
                    }
                }
                if (mahAvatar.ControllerRight != null)
                {
                    if (pose.controllerRightPose.indexTrigger == 0 && pose.controllerRightPose.handTrigger == 0)
                    {
                        Vector3 rightDir = mahAvatar.HandRight.transform.position - Camera.main.transform.position;
                        Ray rightRay = new Ray(Camera.main.transform.position, rightDir);
                        DoWaveFromDirectionVector(rightRay);
                    }
                }
            }
        }
    }
    /*
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(3.0f);
        allowedToWave = true;
    }
    */

    private void DoWaveFromDirectionVector(Ray originalFamousRay)
    {
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
                //Vector3 newOrigin = originalFamousRay.origin + this.transform.TransformDirection(localDirectionAdjust);
                Vector3 newOrigin = originalFamousRay.origin + localDirectionAdjust;
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
                        if (!isWaving && !personAnim.GetBool("HasWaved")) {
                            // MAKE 'EM WAVE
                            personAnim.SetTrigger("OnWave");

                            // Pop a nice score object!
                            GameObject scoreObj = (GameObject)GameObject.Instantiate(scorePrefab, hit.point, Camera.main.transform.rotation);
                            ScoreShownOnHit scoreScript = scoreObj.GetComponent<ScoreShownOnHit>();
                            scoreScript.UpdateTextAndGo("+1000");

                            // TODO: Tell the gamemanager!
                            GameManager.instance.score += 1000;
                            //TODO: check if male or female
                            voice.GetComponent<PeopleSpeach>().PlayVoice();
                            //allowedToWave = false;
                            //StartCoroutine(Wait());
                            triggeredWave = true;
                            personAnim.SetBool("HasWaved", true);
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
