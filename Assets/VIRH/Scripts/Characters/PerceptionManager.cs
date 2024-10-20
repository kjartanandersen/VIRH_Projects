using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionManager : MonoBehaviour
{
    public PerceptionActionManager m_CharacterActionManager;
    public ActionManager m_ActionManager;
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void SensorInput(Sensor.SensorType type, GameObject subject, bool isEntering)
    {
        switch (type)
        {
            case Sensor.SensorType.FarProximity:
                if (isEntering)
                {
                    m_CharacterActionManager.GlanceAt(subject);
                }
                else
                {
                    m_CharacterActionManager.StopGazeTracking();
                }
                break;
            case Sensor.SensorType.NearProximity:
                if (isEntering)
                {
                    m_CharacterActionManager.GazeTrack(subject);

                }
                else
                {
                    m_CharacterActionManager.StopGazeTracking();

                }
                break;
        }
    }
}
