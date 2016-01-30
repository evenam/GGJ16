using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	private bool playEnabled;
	private float timeStart;
	private int myPoints;

	// Use this for initialization
	void Start ()
	{
		playEnabled = false;
		myPoints = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Time.time - timeStart > 30) {
			TimeUp ();
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
		StartTimer ();
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
		DisablePlay ();
	}
}
