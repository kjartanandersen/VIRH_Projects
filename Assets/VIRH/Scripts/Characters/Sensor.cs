using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public enum SensorType
    {    // Our possible sensors types (you can add as many as you want)
        NearProximity,
        FarProximity,
        SocialZone
    };

    public PerceptionManager m_PerceptionManager;   // This sensor will report to this Perception Manager

    public SensorType m_MySensorType;               // Identifies this sensor

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    // Overrides this event callback
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Sensor Enter Triggered!");
        m_PerceptionManager.SensorInput(m_MySensorType, other.gameObject, true);
    }

    // Overrides this event callback
    public void OnTriggerExit(Collider other)
    {
        Debug.Log("Sensor Exit Triggered!");
        m_PerceptionManager.SensorInput(m_MySensorType, other.gameObject, false);
    }
}
