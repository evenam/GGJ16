using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using WiimoteApi;

public class PlayerController : MonoBehaviour
{
	private bool  playEnabled;
	private float timeStart;
	
	private int   myPoints;
    private float oldVecz;
    private float newVecz;
    private float leftRightMotion;
    private float velocity;
    private float finalVel;
    private float fVelTime;
    private bool  throwSet;
    private bool  calibrated;
    private bool  isFirst;
    private bool  rayOn;
    private Vector3 myBallPos;
    private Vector3 myBallVel;
    
    private ButtonData wiiButton;
    private float[] accelData;

    private GameObject myBall;
	private Wiimote    wiiController;

    public bool usingWiimote;
	public GameObject pingPong;
    public GameObject ghostBall;
    public Network_Controller app;
    public AudioSource shotSound;
    public AudioSource music;
    public AudioClip[] makeShots;
    public AudioClip[] missShots;

    public GameObject cup1, cup2, cup3, cup4,
                       cup5, cup6, cup7, cup8,
                       cup9, cup10, cup11, cup12;
    private string cups;

	private Vector3 rotOffset = Vector3.zero;
    public GameObject flipper;
    public Text scoreBoard;
    private int curCups;

	// Use this for initialization
	void Start()
	{
        music.Play();
        cups = "UUUUUUUUUUUU";
        if (usingWiimote)
        {
            WiimoteManager.FindWiimotes();
            wiiController = WiimoteManager.Wiimotes[0];
            RumbleWii(true);
            iTween.ScaleBy(gameObject, iTween.Hash("time", 0.6f, "oncomplete", "RumbleWii", "oncompletetarget", gameObject, "oncompleteparams", false));
            wiiController.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL);
            wiiController.DeactivateWiiMotionPlus();
        }
        rayOn = true;
		playEnabled = true;
        throwSet = false;
        myBallPos = myBallVel = Vector3.zero;
        oldVecz = 0;
        newVecz = 0;
        leftRightMotion = 0;
		myPoints = 0;
        velocity = 0;
        finalVel = 0;
        fVelTime = 0;
        curCups = 0;

        //UnityEngine.VR.VRSettings.enabled = !UnityEngine.VR.VRSettings.enabled;
        EnablePlay();
    }

    public void setState(string st, Vector3 pos, Vector3 vel)
    {
        cups = st;
        if (cups[0] == 'D') cup1.SetActive(false); else cup1.SetActive(true);
        if (cups[1] == 'D') cup2.SetActive(false); else cup2.SetActive(true);
        if (cups[2] == 'D') cup3.SetActive(false); else cup3.SetActive(true);
        if (cups[3] == 'D') cup4.SetActive(false); else cup4.SetActive(true);
        if (cups[4] == 'D') cup5.SetActive(false); else cup5.SetActive(true);
        if (cups[5] == 'D') cup6.SetActive(false); else cup6.SetActive(true);
        if (cups[6] == 'D') cup7.SetActive(false); else cup7.SetActive(true);
        if (cups[7] == 'D') cup8.SetActive(false); else cup8.SetActive(true);
        if (cups[8] == 'D') cup9.SetActive(false); else cup9.SetActive(true);
        if (cups[9] == 'D') cup10.SetActive(false); else cup10.SetActive(true);
        if (cups[10] == 'D') cup11.SetActive(false); else cup11.SetActive(true);
        if (cups[11] == 'D') cup12.SetActive(false); else cup12.SetActive(true);

        //GameObject hisBall = (GameObject)Instantiate(pingPong, new Vector3(-pos.x, pos.y, pos.z), Quaternion.identity);
        //myBall.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Min(finalVel, 0) * -2, 2f, newVecz / 100);
    }

    int calculateScore1(string theCups)
    {
        int score = 0;
        for (int i = 0; i < 6; i++)
        {
            if (theCups[i] == 'D')
            {
                score++;
                IncreaseDrunk();
            }
        }
        return score;
    }

    int calculateScore2(string theCups)
    {
        int score = 0;
        for (int i = 6; i < 12; i++)
        {
            if (theCups[i] == 'D')
                score++;
        }
        return score;
    }

    // Update is called once per frame
    void Update()
	{
        UpdateScore();
        if (rayOn)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit);
            //Vector3 cursorPosition = new Vector3(cursor.transform.position.x, hit.transform.position.y, hit.transform.position.z);
            if (Input.GetKeyDown(KeyCode.Tab))
                Debug.Log("hi");
            //cursor.transform.position = cursorPosition;

        }

        // void establish from state()

        if (app.RespectMyAutoritah() && !myBall)
        {
            ghostBall.SetActive(true);
            if (usingWiimote)
            {
                int ret;
                float velocity_old = 0.0f;
                ret = wiiController.ReadWiimoteData();
                wiiButton = wiiController.Button;
                if (ret > 0 && wiiButton.b)
                {
                    oldVecz = newVecz;
                    throwSet = true;
                    Vector3 accel = GetAccelVector();
                    Vector3 accelNormal = accel;
                    accelNormal.Normalize();

                    float sizeOfWidth = 2;
                    leftRightMotion = accelNormal.x * (sizeOfWidth / 2);

                    velocity_old = velocity;
                    Vector2 vVec = new Vector2(accel.y, accel.z);
                    velocity = Mathf.Sqrt(vVec.SqrMagnitude());
                    velocity *= Mathf.Sign(accel.y);
                    velocity -= velocity_old;
                    newVecz = leftRightMotion;

                    if (finalVel >= velocity)
                    {
                        finalVel = velocity;
                        fVelTime = Time.time;
                    }
                    iTween.ValueTo(gameObject, iTween.Hash("from", oldVecz, "to", newVecz, "time", Time.deltaTime * 4.0f, "onupdate", "ShowGhost"));

                    Debug.Log(finalVel);

                }
                else if (!wiiButton.b && throwSet)
                {
                    throwSet = false;
                    Destroy(myBall);
                    myBallPos = new Vector3(transform.position.x - 1, transform.position.y, newVecz);
                    myBallVel = new Vector3(Mathf.Min(finalVel, 0) * 2, 2f, newVecz / 100);
                    myBall = (GameObject)Instantiate(pingPong, myBallPos, Quaternion.identity);
                    myBall.GetComponent<Rigidbody>().velocity = myBallVel;
                    Debug.Log(finalVel);
                    myBall.GetComponent<BallBehavior>().setApp(this);
                    myBall.GetComponent<BallBehavior>().EnablePlay();

                    finalVel = 0;
                    velocity = 0;
                }

                if (Time.time - fVelTime > 1f)
                {
                    finalVel = velocity;
                    fVelTime = Time.time;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                myBall = (GameObject)Instantiate(pingPong, new Vector3(transform.position.x - 1, transform.position.y, 0), Quaternion.identity);
                myBall.GetComponent<Rigidbody>().velocity = new Vector3(-7.2f, 2f, 0f);
                myBall.GetComponent<BallBehavior>().setApp(this);
                myBall.GetComponent<BallBehavior>().EnablePlay();
            }


            if (Time.time - timeStart > 30f && playEnabled && false)
            {
                TimeUp();
            }
        }
        else
        {
            ghostBall.SetActive(false);
        }
	}

	/*
	 * Receive points from current ball.
	 */
	public void ReceivePoints(int points)
	{
		myPoints += points;
	}

    void ShowGhost(float gvec)
    {
        ghostBall.transform.position    = new Vector3(transform.position.x - 1, transform.position.y, gvec);
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
		DisablePlay();
	}

    public void SetPosition(bool b)
    {
        isFirst = b;
        /*
        if (b)
        {
            cup1.GetComponent<CupBehavior>().cupNumber = 6;
            cup2.GetComponent<CupBehavior>().cupNumber = 7;
            cup3.GetComponent<CupBehavior>().cupNumber = 8;
            cup4.GetComponent<CupBehavior>().cupNumber = 9;
            cup5.GetComponent<CupBehavior>().cupNumber = 10;
            cup6.GetComponent<CupBehavior>().cupNumber = 11;
            cup7.GetComponent<CupBehavior>().cupNumber = 0;
            cup8.GetComponent<CupBehavior>().cupNumber = 1;
            cup9.GetComponent<CupBehavior>().cupNumber = 2;
            cup10.GetComponent<CupBehavior>().cupNumber = 3;
            cup11.GetComponent<CupBehavior>().cupNumber = 4;
            cup12.GetComponent<CupBehavior>().cupNumber = 5;
        }*/
    }

    public void GameStatePush(short gdat, int xPos, int yPos, int zPos, int xVel, int yVel, int zVel)
    {

    }

    public void GameStateUpdate(short gdat, int xPos, int yPos, int zPos, int xVel, int yVel, int zVel)
    {
        // gdat
        Rigidbody rigidBalls = myBall.GetComponent<Rigidbody>();
        myBall.transform.position = new Vector3(xPos, yPos, zPos);
        rigidBalls.AddForce(xVel, yVel, zVel);
    }

    public void RayOff()
    {
        rayOn = false;
    }

    void RumbleWii(bool rum)
    {
        wiiController.RumbleOn = rum;
        wiiController.SendStatusInfoRequest();
    }

    private Vector3 GetAccelVector()
    {
        float accel_x;
        float accel_y;
        float accel_z;
        float[] accel = wiiController.Accel.GetCalibratedAccelData();
        accel_x = accel[0]-.35f;
        accel_y = accel[2]-.35f;
        accel_z = accel[1]-.35f;

        Vector3 ret = new Vector3(accel_x, accel_y, accel_z);
        //Debug.Log(ret);
        return ret;
    }

    public void passTurn(string shotType)
    {
        app.sendClientGameState(cups, myBallPos, myBallVel);
    }

    public void KillCup(int number)
    {
        shotSound.clip = makeShots[Random.Range(0, 3)];
        shotSound.Play();
        shotSound.clip = null;
        string newState = "";
        for (int i = 0; i < 12; i++)
        {
            if (number == i)
                newState += 'D';
            else
                newState += cups[i];
        }
        cups = newState;
    }

    public void PlayMiss()
    {
        shotSound.clip = missShots[Random.Range(0, 2)];
        shotSound.Play();
        shotSound.clip = null;
    }

    public void IncreaseDrunk()
    {
        Camera.main.GetComponent<Draaank>().Drunk += 10;
        Camera.main.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount += .01f;
    }

    public void resetDrunk(int reset)
    {
        int resetD = 10 * reset;
        float resetM = .01f * reset;

        Camera.main.GetComponent<Draaank>().Drunk -= resetD;
        Camera.main.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount -= resetM;
    }

    public void UpdateScore()
    {
        int op = 0;
        int user = 0;
        for (int i = 0; i < 6; i++)
            if (cups[i] == 'D')
            {
                op++;
                IncreaseDrunk();
                resetDrunk(curCups);
            }
        for (int i = 6; i < 12; i++)
            if (cups[i] == 'D')
                user++;
        curCups = op;
        scoreBoard.text = user +" : " + op;
    }
}