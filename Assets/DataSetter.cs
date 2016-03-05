using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DataSetter : MonoBehaviour {
    InputField inputTextField;
    public Keyboard keyboard;

    void Start()
    {
        inputTextField = GetComponent<InputField>();
    }

	public void SetData () {
        keyboard.data = inputTextField.text;
        keyboard.ParseData();
	}
}
