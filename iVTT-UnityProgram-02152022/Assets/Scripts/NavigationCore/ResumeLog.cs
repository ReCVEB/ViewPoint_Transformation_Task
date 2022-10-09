using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class TrialOrderInfo {
    public List<int> sequence = new List<int>();
    public int pos = 0;
    public int getCurrentTrialNumber()
    {
        if (sequence != null && sequence.Count > pos)
            return sequence[pos];
        return 0;
    }
}

public class ResumeLog
{
    // Start is called before the first frame update

    public static void writeTrialOrderInfoToJson(string fileName, TrialOrderInfo target)
    {
        StreamWriter streamWriter = new StreamWriter(fileName, false);
        string json = JsonUtility.ToJson(target);
        Debug.Log(json);
        streamWriter.Write(json);
        streamWriter.Close();
    }

    public static TrialOrderInfo readTrialOrderInfoFromJSON(string fileName)
    {
        using (StreamReader streamReader = new StreamReader(fileName)){
            return JsonUtility.FromJson<TrialOrderInfo>(streamReader.ReadToEnd());
        }
        return null;
    }

}
