using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionManager : MonoBehaviour
{

    public ActionManager m_CharacterActionManager;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public GameObject m_SocialTarget;   // Points to an entity that has entered our SocialZone

    // Returns True if we have anyone inside our SocialZone

    public bool HasSocialTarget()
    {
        return m_SocialTarget != null;
    }


    // Process and possibly react to input from our sensors
    public void SensorInput(Sensor.SensorType type, GameObject subject, bool state = true)
    {
        // Substitute the subject with the eyes of the subject if found - This ensure gaze toward subject's eyes
        Transform eye_object;
        eye_object = subject.transform.Find("PlayerCameraRoot");
        if (eye_object != null) { subject = eye_object.gameObject; };

        switch (type)
        {
            case Sensor.SensorType.FarProximity:
                m_CharacterActionManager.GlanceAt(subject);
                break;
            case Sensor.SensorType.NearProximity:
                m_CharacterActionManager.GazeTrack(subject, 3.0f);
                //m_CharacterActionManager.WalkAway();  // Lab 6
                break;

            case Sensor.SensorType.SocialZone:
                if (state == true)
                {
                    Debug.Log("PM: Someone Entered Social Zone");
                    m_SocialTarget = subject;       // Simply store the entering object as our SocialTarget
                }
                else
                {
                    Debug.Log("PM: Someone Left Social Zone ");
                    m_SocialTarget = null;          // Reset the SocialTarget
                }
                break;
        }




    }

}
