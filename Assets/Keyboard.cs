using UnityEngine;
using System.Collections;
using System.ComponentModel;

public class Keyboard : MonoBehaviour {

    public string data;
    public GameObject keycapPrefab;

    private Keycap lastKeycap;
    private ArrayList rows = new ArrayList();

	void Start () {
        Stack brackets = new Stack();
        bool firstKeyInRow = false;
        Keycap newPropertiesToSet = null;
        int endIndex = 0;
        int beginIndex = 0;
        while (endIndex < data.Length)
        {
            string checkChar = data.Substring(endIndex, 1);
            if (checkChar == "[")
            {
                if (brackets.Count == 0)
                {
                    brackets.Push(checkChar);
                    beginIndex = endIndex + 1;
                    //Create new row
                    GameObject newRow = new GameObject("Row" + rows.Count);
                    newRow.transform.parent = transform;
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
                    string propertiesString = data.Substring(beginIndex, endIndex-beginIndex);
                    string[] properties = propertiesString.Split(",".ToCharArray());

                    SetDefaultProperties(newPropertiesToSet, firstKeyInRow);
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
                    if (data.Substring(endIndex - 1, 1) != "\\" || data.Substring(endIndex - 2, 2) == "\\\\")
                    {
                        brackets.Pop();
                        if (newPropertiesToSet == null)
                        {
                            //Create new key
                            GameObject newKey = (GameObject)Instantiate(keycapPrefab, Vector3.zero, Quaternion.identity);
                            newKey.transform.parent = ((GameObject)rows[rows.Count - 1]).transform;
                            Keycap newKeycapProperties = newKey.GetComponent<Keycap>();

                            //Set default properties
                            SetDefaultProperties(newKeycapProperties, firstKeyInRow);
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
                    }
                }
                else if ((string)brackets.Peek() == "[")
                {
                    brackets.Push(checkChar);
                    beginIndex = endIndex + 1;
                }
            }
            endIndex++;
        }
	}

    void SetDefaultProperties(Keycap keycapProperties, bool newLine)
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
