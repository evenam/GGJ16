using UnityEngine;
using System.Collections;

public class BallBehavior : MonoBehaviour
{
	private int points;
	private bool bounced;

	public  GameObject playerObj;
	private PlayerController myPlayer;

	// Use this for initialization
	void Start()
	{
		points = 0;
		bounced = false;

        myPlayer = playerObj.GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update()
	{
		/*
		 * When player runs out of time.
		 */
        Debug.Log(myPlayer.IsEnabled());
		if (!myPlayer.IsEnabled())
        {
            Debug.Log("disabled");
			points = 0;
			EndTurn();
		}
	}

	/*
	 * Handles ball outcomes.
	 * Landing in cup.
	 * Bouncing on the table.
	 * Landing between three cups.
	 * Landing in an island cup.
	 * Bouncing back to player's side.
	 */
	void OnTriggerEnter(Collider other)
	{
            Debug.Log("collide");
		if (other.name.Contains("Cup"))
        {
			if (bounced)
				points = 2;
			else
				points = 1;
			DisablePlay();
		}
        else if (other.name == "Table")
        {
			bounced = true;
		}
        else if (other.name.Contains("TriCup"))
        {
			points = 5;
            DisablePlay();
		} 
		else if (other.name.Contains("Island"))
		{
			points = 3;
            DisablePlay();
		}
		else if (other.name.Contains("ReturnTrigger"))
		{
			ResetTurn();
		}
	}

	/*
	 * Reset when ball returns to player's side.
	 */
	void ResetTurn()
	{
		myPlayer.StartTimer();
		Destroy(gameObject);
	}

	/*
	 * Allocate current points to player.
	 * Transfer active status to other player.
	 * Kill me.
	 */
	void EndTurn()
	{
        Debug.Log("end");
		myPlayer.ReceivePoints(points);
		Destroy(gameObject);
	}

    void DisablePlay()
    {
        myPlayer.DisablePlay();
        EndTurn();
    }
}