using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;

public class ActionManager : MonoBehaviour
{

    Animator m_animator;

    public TwoBoneIKConstraint m_RightArmIK;
    public GameObject m_RightArmIKTarget;

    bool m_IsReadyingRightArm;
    float m_TimePassed;
    public float m_RightArmMoveDuration;

    NavMeshAgent m_NavMeshAgent;            // NavMeshAgent for movement

    public GameObject[] m_Destinations;      // List of destinations to walk to

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_IsReadyingRightArm = false;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }


    // Update is called once per frame
    void Update()
    {
        if (m_IsReadyingRightArm)
        {
            m_TimePassed += Time.deltaTime;
            m_RightArmIK.weight = Mathf.Clamp(m_TimePassed / m_RightArmMoveDuration, 0.0f, 1.0f);
            if (m_RightArmIK.weight == 1.0f)
            {
                m_IsReadyingRightArm = false;
                m_TimePassed = 0.0f;
            }
        }
    }

    // ================== Locomotion ==================

    public void WalkTo(GameObject target)
    {
        m_NavMeshAgent.destination = target.transform.position;
    }

    public void WalkAway()
    {
        foreach (GameObject destination in m_Destinations)
        {
            if (Vector3.Distance(transform.position, destination.transform.position) > 1.0f)
            {
                WalkTo(destination);
                break;
            }
        }
    }

    public void Stand()
    {
        m_animator.SetBool("IsPraying", false);
    }

    public void Pray()
    {
        m_animator.SetBool("IsPraying", true);
        Invoke("Stand", 5.0f);
    }

    public void Jump()
    {
        m_animator.SetBool("IsJumping", true);
        Invoke("StopJump", 3.0f);
    }

    public void StopJump()
    {
        m_animator.SetBool("IsJumping", false);
    }

    public void RightArmObjectReach(GameObject target)
    {
        m_RightArmIKTarget.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);
        m_IsReadyingRightArm = true;
    }

    public void RightArmRelax()
    {
        m_RightArmIK.weight = 0.0f;
    }
}
