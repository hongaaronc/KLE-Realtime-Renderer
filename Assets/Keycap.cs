using UnityEngine;
using System.Collections;

public class Keycap : MonoBehaviour {
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
    public string c = "#FFFFFF";

	void Start () {
        //Flip horizontally
        actualX *= -1;
        w *= -1;

        //Position
        transform.position = new Vector3(0.5f * (w - 1) + actualX, 0, 0.5f * (h - 1) + actualY);

        //Scale
        //transform.localScale = new Vector3(w, 1, h);
        GetComponentInChildren<KeycapSizer>().Resize(-w, h);

        //Color
        c = c.TrimStart("#\" ".ToCharArray());
        byte r = byte.Parse(c.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
	    byte g = byte.Parse(c.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
	    byte b = byte.Parse(c.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        Color newColor = new Color32(r,g,b,255);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = newColor;
        }
	}

    public void Calculate()
    {
        actualX = defaultX + x;
        actualY = defaultY + y;
    }
}
