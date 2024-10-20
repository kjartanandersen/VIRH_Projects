using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjBackForth : MonoBehaviour
{
    public float speed = 1.0f;
    public float distance = 1.0f;

    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(startPos.x + Mathf.PingPong(Time.time * speed, distance), transform.position.y, transform.position.z);
    }
}
