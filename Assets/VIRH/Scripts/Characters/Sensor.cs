using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public enum SensorType { NearProximity, FarProximity };
    public PerceptionManager m_ActionManager;

    public SensorType m_MySensorType;
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void OnTriggerEnter(Collider other)
    {
        GameObject subject;
        if (other.tag == "Player")
        {
            subject = other.gameObject.transform.GetChild(0).gameObject;
        }
        else
        {
            subject = other.gameObject;
        }
        m_ActionManager.SensorInput(m_MySensorType, subject, true);
    }

    public void OnTriggerExit(Collider other)
    {
        GameObject subject;
        if (other.tag == "Player")
        {
            subject = other.gameObject.transform.GetChild(0).gameObject;
        }
        else
        {
            subject = other.gameObject;
        }
        m_ActionManager.SensorInput(m_MySensorType, subject, false);
    }
}
