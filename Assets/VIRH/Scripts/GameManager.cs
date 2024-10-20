using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using StarterAssets;

public class GameManager : MonoBehaviour
{
    bool isInConversation = false;
    bool hasItem = false;
    bool gameDone = false;
    public Animator m_RemyAnimator;
    public FirstPersonController m_FirstPersonController;
    public AudioSource m_AudioSource;
    public AudioClip m_AudioClip;
    public AudioSource m_RemysAudioSource;
    public AudioClip m_RemysSadAudioClip;
    public AudioClip m_RemysHappyAudioClip;



    public static GameManager instance;

    [Tooltip("Typically leave unticked so temporary Dialogue Managers don't unregister your functions.")]
    public bool unregisterOnDisable = false;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }




    }

    // Update is called once per frame
    void Update()
    {
        // Keep mouse cursor locked in the center of the screen
        if (!isInConversation)
        {
            Cursor.lockState = CursorLockMode.Locked;
            m_FirstPersonController.EnablePlayerInput();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            m_FirstPersonController.DisablePlayerInput();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Exit the game
            Application.Quit();
        }
    }

    public void StartConversation()
    {
        isInConversation = true;
        m_RemyAnimator.SetBool("IsTalking", true);
    }

    public void EndConversation()
    {
        isInConversation = false;
        m_RemyAnimator.SetBool("IsTalking", false);
    }

    public void SetHasItem(bool hasItem)
    {
        this.hasItem = hasItem;
    }

    public void SetGameDone(bool gameDone)
    {
        this.gameDone = gameDone;

    }

    public bool GetGameDone()
    {
        return gameDone;
    }

    public bool GetHasItem()
    {
        return hasItem;
    }

    public void MakeRemySad()
    {
        m_RemyAnimator.SetBool("IsSad", true);
        m_RemyAnimator.SetBool("IsHappy", false);

        m_RemysAudioSource.clip = m_RemysSadAudioClip;
        m_RemysAudioSource.Play();
    }

    public void MakeRemyHappy()
    {
        m_RemyAnimator.SetBool("IsSad", false);
        m_RemyAnimator.SetBool("IsHappy", true);

        m_RemysAudioSource.clip = m_RemysHappyAudioClip;
        m_RemysAudioSource.Play();
    }

    public void PlayAudio()
    {
        m_AudioSource.clip = m_AudioClip;
        m_AudioSource.Play();
    }



    #region Regsiter with Lua

    void OnEnable()
    {
        Lua.RegisterFunction("StartConversation", this, SymbolExtensions.GetMethodInfo(() => StartConversation()));
        Lua.RegisterFunction("EndConversation", this, SymbolExtensions.GetMethodInfo(() => EndConversation()));
        Lua.RegisterFunction("GetHasItem", this, SymbolExtensions.GetMethodInfo(() => GetHasItem()));
        Lua.RegisterFunction("MakeRemySad", this, SymbolExtensions.GetMethodInfo(() => MakeRemySad()));
        Lua.RegisterFunction("MakeRemyHappy", this, SymbolExtensions.GetMethodInfo(() => MakeRemyHappy()));
        Lua.RegisterFunction("GetGameDone", this, SymbolExtensions.GetMethodInfo(() => GetGameDone()));
        Lua.RegisterFunction("SetGameDone", this, SymbolExtensions.GetMethodInfo(() => SetGameDone(true)));
    }

    void OnDisable()
    {
        Lua.UnregisterFunction("StartConversation");
        Lua.UnregisterFunction("EndConversation");
        Lua.UnregisterFunction("GetHasItem");
        Lua.UnregisterFunction("MakeRemySad");
        Lua.UnregisterFunction("MakeRemyHappy");
        Lua.UnregisterFunction("GetGameDone");
        Lua.UnregisterFunction("SetGameDone");
    }

    #endregion
}
