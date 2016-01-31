using UnityEngine;
using System.Collections;
<<<<<<< Updated upstream
using WiimoteApi;

public class PlayerController : MonoBehaviour
{
	private bool  playEnabled;
	private float timeStart;
	
	private int   myPoints;
=======
using WiimoteApi;

public class PlayerController : MonoBehaviour
{
	private bool  playEnabled;
	private bool  isFirst;
	private float timeStart;
	
	private int   myPoints;
	private GameObject myBall;
>>>>>>> Stashed changes
    private bool  calibrated;
    
    private ButtonData wiiButton;
    private float[] accelData;

	private Wiimote wiiController;
	public GameObject pingPong;
	public GameObject testcup;

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
		playEnabled = true;
		myPoints = 0;

        
<<<<<<< Updated upstream

        UnityEngine.VR.VRSettings.enabled = !UnityEngine.VR.VRSettings.enabled;
=======

        //UnityEngine.VR.VRSettings.enabled = !UnityEngine.VR.VRSettings.enabled;
>>>>>>> Stashed changes
        EnablePlay();
	}
	
	// Update is called once per frame
	void Update()
	{

        int ret;
        float leftRightMotion = 0.0f, velocity = 0.0f, velocity_old = 0.0f;
        //do
        // {
        ret = wiiController.ReadWiimoteData();
        if (ret > 0)
        {
<<<<<<< Updated upstream
            //Vector3 offset = GetAccelVector();
            //rotOffset += offset;
=======
            ret = wiiController.ReadWiimoteData();
            if (ret > 0)
            {
                Vector3 offset = GetAccelVector();
				rotOffset += offset;
				new Quaternion(
>>>>>>> Stashed changes

            //testcup.transform.rotation = Quaternion.FromToRotation(offset, Vector3.down) * testcup.transform.rotation;

            Vector3 accel = GetAccelVector();
            Vector3 accelNormal = accel;
            accelNormal.Normalize();

            float sizeOfWidth = 200;
            leftRightMotion = accelNormal.x * (sizeOfWidth / 2);

            velocity_old = velocity;
            Vector2 vVec = new Vector2(accel.y, accel.z);
            velocity = Mathf.Sqrt(vVec.SqrMagnitude());
            velocity *= Mathf.Sign(accel.y);
            velocity -= velocity_old;

            Debug.Log("XPOS: " + leftRightMotion + "\t\tVEL: " + velocity);
        }

        //} while (ret > 0);
        
        wiiButton = wiiController.Button;

        if (wiiButton.d_down)
            for (int x = 0; x < 3; x++)
            {
                AccelCalibrationStep step = (AccelCalibrationStep)x;
                wiiController.Accel.CalibrateAccel(step);
            }

		if (Time.time - timeStart > 30f && playEnabled)
		{
			TimeUp();
		}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myBall = Instantiate(pingPong, new Vector3(transform.position.x - 3, transform.position.y, transform.position.z), Quaternion.identity);
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
		DisablePlay();
<<<<<<< Updated upstream
	}

=======
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
		myBall.transform = new Vector3 (xPos, yPos, zPos);
		rigidBalls.AddForce (xVel, yVel, zVel);
	}

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        accel_x = accel[0]-.35f;
        accel_y = accel[2]-.35f;
        accel_z = accel[1]-.35f;
=======
        accel_x = accel[0];
        accel_y = -accel[2];
        accel_z = -accel[1];
>>>>>>> Stashed changes

        Vector3 ret = new Vector3(accel_x, accel_y, accel_z);
        //Debug.Log(ret);
        return ret;
    }
}