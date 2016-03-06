using UnityEngine;
using System.Collections;

public class Keycap : MonoBehaviour {
    //Settings
    public float keyTravelTime = 0.025f;
    public float keyTravelDistance = 0.2f;
    public bool interactive = true;

    //Private variables
    private Vector3 velocity = Vector3.zero;
    private Vector3 restingPosition = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;

    //Properties
    public string rawData;
    public string[] rawProperties;

    public string contents;
    public bool d = false;
    public float defaultX = 0;
    public float defaultY = 0;
    public float actualX = 0;
    public float actualY = 0;
    public float x = 0;
    public float y = 0;
    public float w = 1;
    public float h = 1;
    public float r = 0;
    public float rx = 0;
    public float ry = 0;
    public string c = "#FFFFFF";

	void Start () {
        //Flip horizontally
        actualX *= -1;
        w *= -1;
        rx *= -1;

        //Position
        transform.Translate(new Vector3(0.5f * (w - 1) + actualX + rx, 0, 0.5f * (h - 1) + actualY + ry));

        //Rotation
        transform.RotateAround(transform.parent.position + new Vector3(rx - 0.5f, 0, ry - 0.5f), Vector3.up, r);

        //Scale
        //transform.localScale = new Vector3(w, 1, h);  //Simple scaling
        GetComponentInChildren<KeycapSizer>().Resize(-w, h);    //Procedural scaling

        //Color
        c = c.TrimStart("#\" ".ToCharArray());
        Color newColor = new Color32(byte.Parse(c.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(c.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(c.Substring(4, 2), System.Globalization.NumberStyles.HexNumber), 255);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = newColor;
            //Description
            if (d) {
                renderer.enabled = false;
            }
        }


        restingPosition = transform.localPosition;
	}

    void OnDrawGizmos()
    {
        rx *= -1;
        Gizmos.DrawCube(new Vector3(rx - 0.5f, 0, ry - 0.5f), Vector3.one * 1f);
        rx *= -1;
    }

    void Update()
    {
        if (interactive)
        {
            try
            {
                if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), contents)))
                {
                    targetPosition = restingPosition - keyTravelDistance * Vector3.up;
                }
                else
                {
                    targetPosition = restingPosition;
                }
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, keyTravelTime);
            }
            catch {
                interactive = false;
            };
        }
    }

    public void Calculate()
    {
        actualX = defaultX + x;
        actualY = defaultY + y;
        restingPosition = transform.localPosition;
        targetPosition = restingPosition;
    }
}
