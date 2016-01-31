using UnityEngine;
using System.Collections;

public class BallBehavior : MonoBehaviour
{
	private int points;
	private bool bounced;

	private Camera playerObj;
	private PlayerController myPlayer;
    float lifetime = 10.0f; // 10 seconds

    private PlayerController app = null;

    public AudioSource tableHit;
    public AudioSource cupHit;
    public AudioSource bodyHit;

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

        lifetime -= Time.deltaTime;
        if (lifetime <= 0.0f)
        {
            DisablePlay();
        }

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
            float tempPitch = cupHit.pitch;
            cupHit.pitch += Random.Range(-1f, 1f);
            cupHit.Play();
            cupHit.pitch = tempPitch;
            app.KillCup(other.GetComponent<CupBehavior>().cupNumber);
			if (bounced)
				points = 2;
			else
				points = 1;
			DisablePlay();
		}
        else if (other.name == "Table")
        {
            float tempPitch = tableHit.pitch;
            tableHit.pitch += Random.Range(-1f, 1f);
            tableHit.Play();
            tableHit.pitch = tempPitch;
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
        else if (other.name == "suitF01")
        {
            float tempPitch = bodyHit.pitch;
            bodyHit.pitch += Random.Range(-1f, 1f);
            bodyHit.Play();
            bodyHit.pitch = tempPitch;
        }
        else if (other.name.Contains("OOB_DeathTrap"))
        {
            myPlayer.PlayMiss();
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
        app.passTurn("");
        myPlayer.ReceivePoints(points);
		Destroy(gameObject);
	}

    void DisablePlay()
    {
        myPlayer.DisablePlay();
        EndTurn();
    }

    public void EnablePlay()
    {
        app.EnablePlay();
    }

    public void setApp(PlayerController App)
    {
        app = App;
    }
}