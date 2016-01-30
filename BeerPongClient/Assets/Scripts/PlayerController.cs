using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	private bool  playEnabled;
	private float timeStart;
	private int   myPoints;

    public GameObject pingPong;

	// Use this for initialization
	void Start()
	{
		playEnabled = true;
		myPoints = 0;

        UnityEngine.VR.VRSettings.enabled = !UnityEngine.VR.VRSettings.enabled;
        EnablePlay();
	}
	
	// Update is called once per frame
	void Update()
	{
        Debug.Log(playEnabled);
		if (Time.time - timeStart > 30f && playEnabled)
        {
			TimeUp();
		}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(pingPong, new Vector3(transform.position.x - 3, transform.position.y, transform.position.z), Quaternion.identity);
        }
	}

	/*
	 * Receive points from current ball.
	 */
	public void ReceivePoints(int points)
	{
		myPoints += points;
	}


	/*
	 * Only one player is active at a time.
	 * Start timer on activate.
	 */
	public bool IsEnabled()
	{
		return playEnabled;
	}

	public void EnablePlay()
	{
		playEnabled = true;
		StartTimer();
	}

	public void DisablePlay()
	{
		playEnabled = false;
	}


	/*
	 * Timer for player's turn.
	 * 30 second timer.
	 * Started on activate.
	 * Can be reset if ball bounces back to player's own side when thrown.
	 */
	public void StartTimer()
	{
		timeStart = Time.time;
	}

	void TimeUp()
	{
        Debug.Log("time up");
		DisablePlay();
	}
}
