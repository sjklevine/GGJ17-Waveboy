using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCollider : MonoBehaviour {

    public GameObject player;
    public Transform section1;
    public Transform section2;
    public Transform section3;
    public Transform section4;
    public Transform section5;
    public Vector3 pos1;
    public Vector3 pos2;
    public Vector3 Pos3;
    public Vector3 Pos4;
    public Vector3 Pos5;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerCollision(Collider player)
    {
        if(player.transform.tag == "Player"){

        }
    }

}
