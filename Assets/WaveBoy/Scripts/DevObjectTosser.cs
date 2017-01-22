using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevObjectTosser : MonoBehaviour {
    public GameObject TossedObjectPrefab;
    public GameObject WaveParticlesPrefab;
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
            Debug.Log("DEBUG: WAVING AT POINT");
            // Raycast straight from the camera, plop some particles there.
            Vector3 fwd = this.transform.forward;

            Ray ray = new Ray(this.transform.position, fwd);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) {
                Debug.DrawLine(ray.origin, hit.point);
                Instantiate(WaveParticlesPrefab, hit.transform.position, this.transform.rotation);

                if (hit.collider.gameObject.tag == "Person")
                {
                    //Take this opportunity to make the person animate!
                    Animator personAnim = hit.collider.gameObject.GetComponent<Animator>();
                    personAnim.SetTrigger("OnWave");

                    // Pop a nice score object!
                    GameObject scoreObj = (GameObject)GameObject.Instantiate(scorePrefab, hit.point, Camera.main.transform.rotation);
                    ScoreShownOnHit scoreScript = scoreObj.GetComponent<ScoreShownOnHit>();
                    scoreScript.UpdateTextAndGo("+1000");

                    // TODO: Tell the gamemanager!
                    //GameManager.instance.score +=00 pointsScored;
                }
            }
        }
    }
}
