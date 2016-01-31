using UnityEngine;
using System.Collections;
using WiimoteApi;

public class PlayerController : MonoBehaviour
{
	private bool  playEnabled;
	private float timeStart;
	
	private int   myPoints;
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

        

        UnityEngine.VR.VRSettings.enabled = !UnityEngine.VR.VRSettings.enabled;
        EnablePlay();
	}
	
	// Update is called once per frame
	void Update()
	{

        int ret;
        do
        {
            ret = wiiController.ReadWiimoteData();
            if (ret > 0)
            {
                Vector3 offset = GetAccelVector();
                rotOffset += offset;

                testcup.transform.rotation = Quaternion.FromToRotation(testcup.transform.rotation * GetAccelVector(), Vector3.down) * testcup.transform.rotation;
            }

        } while (ret > 0);
        
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
		DisablePlay();
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
        accel_x = accel[0];
        accel_y = 0;// -accel[2];
        accel_z = -accel[1];

        Debug.Log(new Vector3(accel_x, accel_y, accel_z));
        return new Vector3(accel_x, accel_y, accel_z);
    }
}