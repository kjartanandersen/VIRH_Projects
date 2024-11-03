using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using System;

public class VoiceMovement : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    void Start()
    {
        actions.Add("forward", Forward);
        actions.Add("up", Up);
        actions.Add("down", Down);
        actions.Add("back", Back);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();

    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void Forward()
    {
        transform.Translate(Vector3.forward);
    }

    private void Up()
    {
        transform.Translate(Vector3.up);
    }

    private void Down()
    {
        transform.Translate(Vector3.down);
    }

    private void Back()
    {
        transform.Translate(Vector3.back);
    }




}
