using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crosstales.RTVoice;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class IntervieweeManager : MonoBehaviour
{
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetData()
    {
        StartCoroutine(FetchData());
    }

    public IEnumerator FetchData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://localhost:5000/"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                string text = request.downloadHandler.text;
                Speaker.Instance.Speak(text, audioSource);
            }
        }
    }
}
