using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    
    [Header("Interviewer Object")]
    public IntervieweeManager interviewerManager;

    [Header("UI Elements")]
    public GameObject InterviewModeUI;      // UI elements for the interview mode
    public GameObject FormModeUI;           // UI elements for the form mode
    public GameObject ResetButtonMode;      // UI elements for the reset button

    [Header("User Inputs")]
    public GameObject EducationRadialButtons;
    public TextMeshProUGUI CountryTextInput;
    public TextMeshProUGUI WorkExperienceTextInput;
    public TextMeshProUGUI SkillsTextInput;
    public TextMeshProUGUI IntervieweeAnswerText;

    [Header("Interviewer Data")]
    public TextMeshProUGUI InterviewerQuestionText;

    private 

    // Start is called before the first frame update
    void Start()
    {
        FormModeUI.SetActive(true);
        InterviewModeUI.SetActive(false);
        ResetButtonMode.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public string GetIntervieweeAnswer()
    {
        return IntervieweeAnswerText.text;
    }

    public void SetInterviewerQuestion(string question)
    {
        InterviewerQuestionText.text = question;
    }

    public void OnStartInterviewButtonClicked()
    {

        if (CountryTextInput.text == "" || WorkExperienceTextInput.text == "" || SkillsTextInput.text == "")
        {
            Debug.Log("Please fill out all fields");
            return;
        }
        string education = "";
        string country;
        string workExperience;
        List<string> skills;

        
        
        for (int i = 0; i < EducationRadialButtons.transform.childCount; i++)
        {
            if (EducationRadialButtons.transform.GetChild(i).GetComponent<Toggle>().isOn)
            {
                education = EducationRadialButtons.transform.GetChild(i).GetChild(1).GetComponent<Text>().text;
            }
        }

        country = CountryTextInput.text;
        workExperience = WorkExperienceTextInput.text;
        skills = new List<string>(SkillsTextInput.text.Split(','));

        interviewerManager.GetDataFromForm(education, country, workExperience, skills);

        FormModeUI.SetActive(false);
        InterviewModeUI.SetActive(true);
        interviewerManager.StartInterview();

    }

    public void OnSubmitAnswerButtonClicked()
    {
        if (interviewerManager.IsFetchingData())
        {
            Debug.Log("Please wait for the interviewer to finish speaking");
            return;
        }
        interviewerManager.GetScore();
    }

    public void EndInterview()
    {
        InterviewModeUI.SetActive(false);
        ResetButtonMode.SetActive(true);
    }

    public void ResetInterview()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
