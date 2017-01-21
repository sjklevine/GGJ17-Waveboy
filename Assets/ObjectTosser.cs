using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTosser : MonoBehaviour {
    public GameObject TossedObjectPrefab;
    public GameObject WaveParticlesPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("DEBUG: TOSSING A PAPER");
            Vector3 fwd = this.transform.forward;
            Vector3 tossStart = this.transform.position + fwd * 1.0f;
            GameObject tossedObject = (GameObject)Instantiate(TossedObjectPrefab, tossStart, this.transform.rotation);
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
                GameObject particles = (GameObject)Instantiate(WaveParticlesPrefab, hit.transform.position, this.transform.rotation);
            }
        }
    }
}
