using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PerceptionActionManager : MonoBehaviour
{
    Animator m_Animator;

    public GameObject m_FocusTarget;        // Head should rest on this target
    public GameObject m_GlanceTarget;       // Eyes should glance at this target

    bool m_GazeTracking = false;            // Should a target be followed by head and eyes
    GameObject m_TrackingTarget;            // If tracking, this is the target being tracked



    float time_passed = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_GazeTracking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_GazeTracking)
        {
            GazeAt(m_TrackingTarget);
        }
    }



    // ================== Gaze Tracking ==================
    public void GlanceAt(GameObject target)
    {
        // Slowly move the glance target to the target


        m_GlanceTarget.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
        Invoke("StopGlancing", 1.0f);
    }

    public void StopGlancing()
    {
        m_GlanceTarget.transform.SetPositionAndRotation(m_FocusTarget.transform.position, m_FocusTarget.transform.rotation);
    }

    public void GazeAt(GameObject target)
    {
        m_FocusTarget.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
        m_GlanceTarget.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
    }

    public void GazeTrack(GameObject target)
    {
        m_TrackingTarget = target;
        m_GazeTracking = true;
    }

    public void StopGazeTracking()
    {
        m_GazeTracking = false;
        m_TrackingTarget = m_FocusTarget;
    }
}
