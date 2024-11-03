using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetAPI : MonoBehaviour
{
    public string url = "http://localhost:5000/";

    public void GetData()
    {
        StartCoroutine(FetchData());
    }

    public IEnumerator FetchData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
