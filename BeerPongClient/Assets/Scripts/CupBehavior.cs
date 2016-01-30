using UnityEngine;
using System.Collections;

public class CupBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Ball"))
        {
            iTween.ShakeScale(gameObject, iTween.Hash("amount", new Vector3(0.5f, 0.5f, 0.5f), "time", 0.5f));
            iTween.ScaleTo(gameObject, iTween.Hash("scale", new Vector3(0f, 0f, 0f), "time", 0.5f, "delay", 0.3f, "easeType", "easeInQuart", "oncomplete", "KillMe", "oncompletetarget", gameObject));
        }
    }

    void KillMe()
    {
        Destroy(gameObject);
    }
}
