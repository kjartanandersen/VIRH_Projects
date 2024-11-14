using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crosstales.RTVoice;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEditor.Rendering.Universal;
using Crosstales.RTVoice.Model;
using System;
using UMA.PoseTools;
using UMA.CharacterSystem;
using UMA;
using System.Globalization;
using CrazyMinnow.SALSA;

public class IntervieweeManager : MonoBehaviour
{
    public UIManager UIManager;
    public AudioSource audioSource;
    public string voiceName;

    [Header("Gaze Settings")]
    public Transform screenTarget;
    public Transform cameraTarget;
    public GameObject gazeTarget;

    private ExpressionPlayer expressionPlayer;
    private DynamicCharacterAvatar dynamicCharacterAvatar;
    private Emoter emoter;
    private Animator animator;
    

    private string intervieweeEducation;
    private string intervieweeCountry;
    private string intervieweeWorkExperience;
    private List<string> intervieweeSkills;
    
    private float fluency_score;
    private float sentiment_score;
    private float relevancy_score;
    private float intervieweeScore = 0.5f;

    private List<string> alreadyAskedQuestions = new List<string>();

    
    private bool startedInterview;

    private string fetchedString;
    private bool hasFetchedData;
    private bool isFetchingData;

    public int maxQuestions = 3;
    private int questionCount = 0;



    public enum Moods
    {
        Neutral,
        Happy,
        Surprised
    }

    public Moods mood;
    private Moods lastMood;
    private bool connected;


    public void OnEnable()
    {
        Speaker.Instance.OnSpeakComplete += SpeakComplete;
        dynamicCharacterAvatar = GetComponent<DynamicCharacterAvatar>();
        dynamicCharacterAvatar.CharacterUpdated.AddListener(OnCreated);

    }

    public void OnCreated(UMAData data)
    {
        expressionPlayer = GetComponent<ExpressionPlayer>();
        expressionPlayer.enableBlinking = true;
        expressionPlayer.enableSaccades = true;
        connected = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        startedInterview = false;
        emoter = GetComponent<Emoter>();
        animator = GetComponent<Animator>();
        

        
    }

    // Update is called once per frame
    void Update()
    {
        if (questionCount > maxQuestions)
        {
            UIManager.EndInterview();
            Speaker.Instance.OnSpeakComplete -= OnQuestionEnd;
            Speaker.Instance.Silence();
            hasFetchedData = false;
        }
        if (startedInterview)
        {
            
            Say("Thank you for joining us today. We’re excited to learn more about your background and experiences. This interview is an opportunity for us to better understand your technical expertise, problem-solving approach, and how you handle challenges in a collaborative environment. Let’s dive into some questions to help us get to know you better");
            // Thank you for joining us today. We’re excited to learn more about your background and experiences. This interview is an opportunity for us to better understand your technical expertise, problem-solving approach, and how you handle challenges in a collaborative environment. Let’s dive into some questions to help us get to know you better
            startedInterview = false;
            Speaker.Instance.OnSpeakComplete += AskFirstQuestion;
        }

        if (hasFetchedData)
        {
            UIManager.SetInterviewerQuestion(fetchedString);
            Say(fetchedString);
            hasFetchedData = false;
            questionCount++;
        }

        SetExpressions();

        

        
    }

    public void StartInterview()
    {
        startedInterview = true;
    }

    public void Say(string text)
    {
        UIManager.SetInterviewerQuestion(text);

        Speaker.Instance.Speak(text, audioSource, Speaker.Instance.VoiceForName(voiceName));
    }

    public void GetDataFromForm(string education, string country, string workExperience, List<string> skills)
    {
        intervieweeEducation = education;
        intervieweeCountry = country;
        intervieweeWorkExperience = workExperience;
        intervieweeSkills = skills;

        Debug.Log("Education: " + intervieweeEducation);
        Debug.Log("Country: " + intervieweeCountry);
        Debug.Log("Work Experience: " + intervieweeWorkExperience);
        Debug.Log("Skills: ");

        foreach (string skill in intervieweeSkills)
        {
            Debug.Log(skill);
        }
    }

    public void GetQuestion()
    {
        
        StartCoroutine(FetchQuestionData());
    }

    public void GetScore()
    {
        if (questionCount > maxQuestions)
        {
            return;
        }
        gazeTarget.transform.position = screenTarget.position;
        animator.SetBool("isTalking", false);
        animator.SetBool("isIdle", false);
        StartCoroutine(FetchScoreData());
    }

    public string CreateSendString()
    {
        string sendString = "Generate one unique and relevant interview question for a candidate applying for the role of Software Engineer. " +
        "Make sure the questions evaluate the candidate's skills, experience, problem-solving abilities, and knowledge specific to the role.\n" +
        "";
        
        sendString += "Education: " + intervieweeEducation + "\n";
        sendString += "Country: " + intervieweeCountry + "\n";
        sendString += "Work Experience: " + intervieweeWorkExperience + "\n";
        sendString += "Skills: \n";
        foreach (string skill in intervieweeSkills)
        {
            sendString += skill + "\n";
        }

        if (alreadyAskedQuestions.Count > 0)
        {
            sendString += "Already asked questions: \n";
            foreach (string question in alreadyAskedQuestions)
            {
                sendString += question + "\n";
            }
        }

        sendString += "Examples of topics to cover:\n" + 
       
        "1. Technical skills and knowledge relevant to Software Engineering.\n" +     
        "2. Situational or behavioral questions that assess how the candidate approaches challenges.\n" +
        "3. Questions that gauge the candidate’s experience and accomplishments in similar roles.\n" +

        "Please return only the question.\n";

        

        return sendString;
    }

    public IEnumerator FetchQuestionData()
    {
        if (isFetchingData)
        {
            yield return new WaitForSeconds(0);
        }
        else
        {
            isFetchingData = true;
            using (UnityWebRequest request = UnityWebRequest.Post("http://localhost:5000/send_interviewer_text", CreateSendString(), contentType: "application/raw"))
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
                    fetchedString = text;
                    alreadyAskedQuestions.Add(text);
                    hasFetchedData = true;
                    isFetchingData = false;
                    gazeTarget.transform.position = cameraTarget.position;
                    animator.SetBool("isTalking", true);
                    animator.SetBool("isIdle", false);
                }
            }    
        }
        
    }

    public IEnumerator FetchScoreData()
    {
        if (isFetchingData)
        {
            yield return new WaitForSeconds(0);
        }
        else
        {
            Debug.Log("Interviewee data" + UIManager.GetIntervieweeAnswer());
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(UIManager.GetIntervieweeAnswer());
            using (UnityWebRequest request = UnityWebRequest.Post("http://localhost:5000/fluency_sentiment_score", UIManager.GetIntervieweeAnswer(), contentType: "application/raw"))
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
                    string[] scores = text.Split(',');
                    Debug.Log("Scores: ");
                    for (int i = 0; i < scores.Length; i++)
                    {
                        Debug.Log(scores[i]);
                    }
                    fluency_score = float.Parse(scores[0], CultureInfo.InvariantCulture);
                    Debug.Log("Fluency Score: " + fluency_score);
                    sentiment_score = float.Parse(scores[1], CultureInfo.InvariantCulture);
                    Debug.Log("Sentiment Score: " + sentiment_score);
                    relevancy_score = float.Parse(scores[2], CultureInfo.InvariantCulture);
                    Debug.Log("Relevancy Score: " + relevancy_score);
                    intervieweeScore = (fluency_score * 0.2f) + sentiment_score * 0.3f + relevancy_score * 0.5f;
                    Debug.Log("Interview Score: " + intervieweeScore);
                    GetQuestion();

                }
            }   
        } 
    }

    public bool IsFetchingData()
    {
        return isFetchingData;
    }

    private void SpeakComplete(Wrapper wrapper)
    {
        Speaker.Instance.Silence();
        audioSource.Stop();
        Debug.Log("Finished speaking");
    }

    private void SetExpressions()
    {

        if (intervieweeScore < 0.5)
        {
            mood = Moods.Surprised;
        }
        else if (intervieweeScore >= 0.5 && intervieweeScore < 0.70)
        {
            mood = Moods.Neutral;
        }
        else
        {
            mood = Moods.Happy;
        }

        if (connected && lastMood != mood)
        {
            float delta = 10 * Time.deltaTime;
            lastMood = mood;
            switch (mood)
            {
                case Moods.Neutral:
                    emoter.ManualEmote("neutral", ExpressionComponent.ExpressionHandler.OneWay);

                    break;
                case Moods.Happy:
                    emoter.ManualEmote("happy", ExpressionComponent.ExpressionHandler.OneWay);
                    break;
                case Moods.Surprised:
                    emoter.ManualEmote("surprised", ExpressionComponent.ExpressionHandler.OneWay);
                    break;
                default:
                    break;
            }
        }
    }

    public void AskFirstQuestion(Wrapper wrapper)
    {
        isFetchingData = false;
        GetQuestion();
        Speaker.Instance.OnSpeakComplete += OnQuestionEnd;
        Speaker.Instance.OnSpeakComplete -= AskFirstQuestion;
    }

    public void OnQuestionEnd(Wrapper wrapper)
    {
        Debug.Log("Question ended");
        Debug.Log(animator.GetBool("isTalking"));
        animator.SetBool("isTalking", false);
        animator.SetBool("isIdle", true);
    }
}
