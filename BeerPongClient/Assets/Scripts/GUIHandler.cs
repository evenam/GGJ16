using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class GUIHandler : MonoBehaviour {

    public InputField userField;
    public InputField opfoField;
    public Button connectButton;

	// Use this for initialization
	void Start ()
    {
        userField.ActivateInputField();
        opfoField.ActivateInputField();
        userField.Select();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
            SwitchField();
        if (Input.GetKeyDown(KeyCode.Return))
            ConnectToServer();
	}

    void SwitchField()
    {
        if (userField.isFocused)
            opfoField.Select();
        else
            userField.Select();
    }

    void ConnectToServer()
    {
        connectButton.onClick.Invoke();
        this.enabled = false;
    }
}
