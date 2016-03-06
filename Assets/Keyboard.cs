using UnityEngine;
using System.Collections;
using System.ComponentModel;

public class Keyboard : MonoBehaviour {

    public string data;
    public GameObject keycapPrefab;

    private GameObject container;
    private Vector3 lastMousePosition;
    private Vector3 mouseDeltaPosition;

    void Start()
    {
        lastMousePosition = Input.mousePosition;
        ParseData();
    }

    void Update()
    {
        mouseDeltaPosition = Input.mousePosition - lastMousePosition;
        if (Input.GetMouseButton(1))
        {
            transform.Rotate(Vector3.up, -mouseDeltaPosition.x * 0.2f);
            transform.RotateAround(Vector3.right, -mouseDeltaPosition.y * 0.002f);
        }
        lastMousePosition = Input.mousePosition;
    }

    public void SetData(string newData)
    {
        data = newData;
        ParseData();
    }

	public void ParseData () {
        Destroy(container);
        Keycap lastKeycap = null;
        ArrayList rows = new ArrayList();

        container = new GameObject("container");
        container.transform.parent = transform;
        container.transform.localPosition = Vector3.zero;
        Vector2 xBounds = Vector2.zero;
        Vector2 yBounds = Vector2.zero;

        Stack brackets = new Stack();
        bool firstKeyInRow = false;
        Keycap newPropertiesToSet = null;
        int endIndex = 0;
        int beginIndex = 0;
        while (endIndex < data.Length)
        {
            string checkChar = data.Substring(endIndex, 1);
            if (brackets.Count > 0 && (string)brackets.Peek() == "\\")
            {
                brackets.Pop();
                endIndex++;
                continue;
            }
            if (checkChar == "[")
            {
                if (brackets.Count == 0)
                {
                    brackets.Push(checkChar);
                    beginIndex = endIndex + 1;
                    //Create new row
                    GameObject newRow = new GameObject("Row" + rows.Count);
                    newRow.transform.parent = container.transform;
                    newRow.transform.localPosition = Vector3.zero;
                    rows.Add(newRow);

                    firstKeyInRow = true;
                }
            }
            else if (checkChar == "]")
            {
                if ((string)brackets.Peek() == "[")
                {
                    brackets.Pop();
                }
            }
            else if (checkChar == "{")
            {
                if ((string)brackets.Peek() == "[")
                {
                    brackets.Push(checkChar);
                    beginIndex = endIndex + 1;
                }
            }
            else if (checkChar == "}")
            {
                if ((string)brackets.Peek() == "{")
                {
                    brackets.Pop();
                    //Create new key
                    GameObject newKey = (GameObject)Instantiate(keycapPrefab, Vector3.zero, Quaternion.identity);
                    newKey.transform.parent = ((GameObject)rows[rows.Count - 1]).transform;
                    newPropertiesToSet = newKey.GetComponent<Keycap>();

                    //Create new properties
                    string propertiesString = data.Substring(beginIndex, endIndex - beginIndex);
                    string[] properties = propertiesString.Split(",".ToCharArray());

                    SetDefaultProperties(newPropertiesToSet, firstKeyInRow, lastKeycap);
                    firstKeyInRow = false;
                    newPropertiesToSet.rawData = propertiesString;
                    newPropertiesToSet.rawProperties = properties;

                    //Set new properties
                    foreach (string property in properties)
                    {
                        System.Reflection.FieldInfo member = newPropertiesToSet.GetType().GetField(property.Split(":".ToCharArray())[0]);
                        if (member != null)
                        {
                            System.Type valueType = member.FieldType;
                            string valueString = property.Split(":".ToCharArray())[1];
                            TypeConverter tc = TypeDescriptor.GetConverter(valueType);
                            object valueObj = tc.ConvertFromString(valueString);
                            member.SetValue(newPropertiesToSet, valueObj);
                        }
                        else
                        {
                            print("Could not find property: " + property);
                        }
                    }

                    lastKeycap = newPropertiesToSet;
                }
            }
            else if (checkChar == "\"")
            {
                if ((string)brackets.Peek() == "\"")
                {
                    brackets.Pop();
                    if (newPropertiesToSet == null)
                    {
                        //Create new key
                        GameObject newKey = (GameObject)Instantiate(keycapPrefab, Vector3.zero, Quaternion.identity);
                        newKey.transform.parent = ((GameObject)rows[rows.Count - 1]).transform;
                        Keycap newKeycapProperties = newKey.GetComponent<Keycap>();

                        //Set default properties
                        SetDefaultProperties(newKeycapProperties, firstKeyInRow, lastKeycap);
                        firstKeyInRow = false;

                        newKeycapProperties.rawData = data.Substring(beginIndex, endIndex - beginIndex);

                        lastKeycap = newKeycapProperties;
                    }
                    else
                    {
                        lastKeycap = newPropertiesToSet;
                        newPropertiesToSet = null;
                    }
                    lastKeycap.contents = data.Substring(beginIndex, endIndex - beginIndex);
                    lastKeycap.Calculate();
                    if (lastKeycap.actualX < xBounds.x)
                        xBounds.x = lastKeycap.actualX;
                    if (lastKeycap.actualX > xBounds.y)
                        xBounds.y = lastKeycap.actualX;
                    if (lastKeycap.actualY < yBounds.x)
                        yBounds.x = lastKeycap.actualY;
                    if (lastKeycap.actualY > yBounds.y)
                        yBounds.y = lastKeycap.actualY;
                }
                else if ((string)brackets.Peek() == "[")
                {
                    brackets.Push(checkChar);
                    beginIndex = endIndex + 1;
                }
            }
            else if (checkChar == "\\")
            {
                brackets.Push(checkChar);
                beginIndex = endIndex + 1;
            }
            endIndex++;
        }
        container.transform.localPosition = new Vector3(-xBounds.x + (xBounds.y - xBounds.x) / 2, 0, yBounds.x - (yBounds.y - yBounds.x) / 2);
	}

    void SetDefaultProperties(Keycap keycapProperties, bool newLine, Keycap lastKeycap)
    {
        if (lastKeycap == null)
            return;

        if (newLine)
        {
            keycapProperties.defaultX = 0.0f;
            keycapProperties.defaultY = lastKeycap.actualY + 1.0f;
        }
        else
        {
            keycapProperties.defaultX = lastKeycap.actualX + lastKeycap.w;
            keycapProperties.defaultY = lastKeycap.actualY;
        }
        keycapProperties.c = lastKeycap.c;
    }
}
