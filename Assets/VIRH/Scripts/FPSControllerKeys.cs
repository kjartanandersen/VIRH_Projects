using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class FPSControllerKeys : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NPCKey")
        {
            GameManager.instance.SetHasItem(true);
            GameManager.instance.PlayAudio();
            Destroy(other.gameObject);
        }
    }
}
