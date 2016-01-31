using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(GameObject other)
    {
        Debug.Log("Kill Zone");
        if (other.name.Contains("Ball"))
        {
            Destroy(other);
        }
    }
}
