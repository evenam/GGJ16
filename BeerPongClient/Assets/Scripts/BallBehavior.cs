using UnityEngine;
using System.Collections;

public class BallBehavior : MonoBehaviour
{
	private int points;
	private bool bounced;

	private Camera playerObj;
	private PlayerController myPlayer;

    private PlayerController app = null;

	// Use this for initialization
	void Start()
	{
		points = 0;
		bounced = false;

        playerObj = Camera.main;
        myPlayer  = playerObj.GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update()
	{
		/*
		 * When player runs out of time.
		 */
        if (!myPlayer.IsEnabled())
        {
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
            Debug.Log("bounce");
			bounced = true;
		}
        else if (other.name.Contains("TriCup"))
        {
			points = 6;
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
        if (other.name.Contains("OOB_DeathTrap"))
        {
            points = 0;
            DisablePlay();
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
		myPlayer.ReceivePoints(points);
		Destroy(gameObject);
        app.passTurn(0);
	}

    void DisablePlay()
    {
        myPlayer.DisablePlay();
        EndTurn();
    }

    public void setApp(PlayerController App)
    {
        app = App;
    }
}