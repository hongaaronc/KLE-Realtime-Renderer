using UnityEngine;
using System.Collections;
using System.ComponentModel;

public class Keyboard : MonoBehaviour {

    public string data;
    public GameObject keycapPrefab;

    private Keycap lastKeycap;

	void Start () {
        string[] rows = data.Split("[".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
        string[] newRows = new string[rows.Length];
        for (int i=0; i<rows.Length; i++)
        {
            newRows[i] = rows[i].Replace("], ", "").Replace("]", "");
        }
        rows = newRows;
        for (int i=0; i<rows.Length; i++) {
            GameObject newRowGO = new GameObject("Row" + i);
            newRowGO.transform.parent = transform;
            string[] keys = rows[i].Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

            //Reformat keys
            ArrayList newKeys = new ArrayList();
            for (int j = 0; j < keys.Length; j++)
            {
                string newKey = keys[j];

                //Check for multiple properties
                if (keys[j].StartsWith("{") && !keys[j].EndsWith("}"))
                {
                    j++;
                    while (!keys[j].EndsWith("}"))
                    {
                        newKey += "," + keys[j];
                        j++;
                    }
                    newKey += "," + keys[j];
                }
                newKeys.Add(newKey);
            }
            keys = new string[newKeys.Count];
            newKeys.CopyTo(keys);
            for (int j = 0; j < keys.Length; j++)
            {
                if (keys[j].StartsWith("{") && keys[j].EndsWith("}"))
                {
                    //Create new keycap
                    GameObject newKeycap = (GameObject)Instantiate(keycapPrefab, Vector3.zero, Quaternion.identity);
                    newKeycap.transform.parent = newRowGO.transform;
                    Keycap newKeycapProperties = newKeycap.GetComponent<Keycap>();
                    newKeycapProperties.contents = keys[j+1].Replace("\"", "");
                    //Set default properties
                    SetDefaultProperties(newKeycapProperties, j == 0);

                    //Read in new properties
                    string newKeyString = keys[j].Replace("{", "").Replace("}", "");
                    string[] properties = newKeyString.Split(",".ToCharArray());
                    newKeycapProperties.rawData = properties;
                    
                    //Set new properties
                    foreach (string property in properties)
                    {
                        System.Reflection.FieldInfo member = newKeycapProperties.GetType().GetField(property.Split(":".ToCharArray())[0]);
                        if (member != null)
                        {
                            System.Type valueType = member.FieldType;
                            string valueString = property.Split(":".ToCharArray())[1];
                            TypeConverter tc = TypeDescriptor.GetConverter(valueType);
                            object valueObj = tc.ConvertFromString(valueString);
                            member.SetValue(newKeycapProperties, valueObj);
                        }
                        else
                        {
                            print("Could not find property: " + property);
                        }
                    }
                    lastKeycap = newKeycapProperties;
                    j++;
                }
                else if (keys[j].StartsWith("\"") && keys[j].EndsWith("\""))
                {
                    //Create new keycap
                    GameObject newKeycap = (GameObject)Instantiate(keycapPrefab, Vector3.zero, Quaternion.identity);
                    newKeycap.transform.parent = newRowGO.transform;
                    Keycap newKeycapProperties = newKeycap.GetComponent<Keycap>();
                    newKeycapProperties.contents = keys[j].Replace("\"", "");
                    //Set default properties
                    SetDefaultProperties(newKeycapProperties, j == 0);

                    lastKeycap = newKeycapProperties;
                }
            }
        }
	}

    void SetDefaultProperties(Keycap keycapProperties, bool newLine)
    {
        if (lastKeycap == null)
            return;

        lastKeycap.Calculate();

        if (newLine)
        {
            keycapProperties.defaultX = 0.0f;
            keycapProperties.defaultY = lastKeycap.y + 1.0f;
        }
        else
        {
            keycapProperties.defaultX = lastKeycap.x + lastKeycap.w;
            keycapProperties.defaultY = lastKeycap.y;
        }
        keycapProperties.c = lastKeycap.c;
    }
}
