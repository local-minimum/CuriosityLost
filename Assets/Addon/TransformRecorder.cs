using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TransformData
{
    public string name;
    public Vector3 localPosition;
    public Vector3 localScale;
    public Vector3 eulerAngles;
}

public class TransformRecorder : MonoBehaviour {

    [SerializeField, HideInInspector]
    List<TransformData> recordedData = new List<TransformData>();

    public void RecordNew()
    {
        RecordNew(System.DateTime.UtcNow.ToString());
    }

    public void RecordNew(string name)
    {
        TransformData tData = new TransformData();
        tData.name = name;
        tData.localPosition = transform.localPosition;
        tData.localScale = transform.localScale;
        tData.eulerAngles = transform.localEulerAngles;
        recordedData.Add(tData);
    }

    public void UpdateState(int index)
    {
        TransformData tData = recordedData[index];
        tData.localPosition = transform.localPosition;
        tData.localScale = transform.localScale;
        tData.eulerAngles = transform.localEulerAngles;
        recordedData[index] = tData;
    }

    public void RemoveState(int index)
    {
        recordedData.RemoveAt(index);
    }

    public void ApplyState(int index)
    {
        TransformData tData = recordedData[index];
        transform.localPosition = tData.localPosition;
        transform.localScale = tData.localScale;
        transform.localEulerAngles = tData.eulerAngles;
    }

    public bool IsCurrentStateRecorded
    {
        get
        {
            Vector3 euAng = transform.localEulerAngles;
            Vector3 scale = transform.localScale;
            Vector3 pos = transform.localPosition;

            for (int i=0, l=recordedData.Count; i<l; i++)
            {
                if (recordedData[i].eulerAngles == euAng && recordedData[i].localPosition == pos && recordedData[i].localScale == scale)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
