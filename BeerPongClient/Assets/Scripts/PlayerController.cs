using UnityEngine;
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
    
    private ButtonData wiiButton;
    private float[] accelData;

    private GameObject myBall;
	private Wiimote    wiiController;
	public  GameObject pingPong;
    public  GameObject ghostBall;

	private Vector3 rotOffset = Vector3.zero;

	// Use this for initialization
	void Start()
	{
        WiimoteManager.FindWiimotes();
        wiiController = WiimoteManager.Wiimotes[0];
        RumbleWii(true);
        iTween.ScaleBy(gameObject, iTween.Hash("time", 0.6f, "oncomplete", "RumbleWii", "oncompletetarget", gameObject, "oncompleteparams", false));
        wiiController.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL);
        wiiController.DeactivateWiiMotionPlus();
        rayOn = true;
		playEnabled = true;
        throwSet = false;
        oldVecz = 0;
        newVecz = 0;
        leftRightMotion = 0;
		myPoints = 0;
        velocity = 0;
        finalVel = 0;
        fVelTime = 0;
        

        //UnityEngine.VR.VRSettings.enabled = !UnityEngine.VR.VRSettings.enabled;
        EnablePlay();
	}
	
	// Update is called once per frame
	void Update()
	{

        if (rayOn)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit);
            if (Input.GetKeyDown(KeyCode.Tab))
                Debug.Log("hi");

        }

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
               
            if (finalVel <= velocity)
            {
                finalVel = velocity;
                fVelTime = Time.time;
            }
            iTween.ValueTo(gameObject, iTween.Hash("from", oldVecz, "to", newVecz, "time", Time.deltaTime, "onupdate", "ShowGhost"));

        }
        else if (!wiiButton.b && throwSet)
        {
            throwSet = false;
            Destroy(myBall);
            myBall = (GameObject)Instantiate(pingPong, new Vector3(transform.position.x - 1, transform.position.y, newVecz), Quaternion.identity);
            myBall.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Abs(finalVel) * 2, 2f, 0f);
            Debug.Log(finalVel);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            myBall = (GameObject)Instantiate(pingPong, new Vector3(transform.position.x - 1, transform.position.y, 0), Quaternion.identity);
            myBall.GetComponent<Rigidbody>().velocity = new Vector3(7 * -2, 2f, 0f);
        }

        if (Time.time - fVelTime > 1f)
        {
            finalVel = velocity;
        }

		if (Time.time - timeStart > 30f && playEnabled && false)
		{
			TimeUp();
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
        ghostBall.transform.position = new Vector3(transform.position.x - 1, transform.position.y, gvec);
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
}