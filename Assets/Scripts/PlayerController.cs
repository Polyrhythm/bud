using UnityEngine;
using HoloToolkit.Unity;

public partial class PlayerController : Singleton<PlayerController> {
    private UAudioManager audioManager;

    void Awake()
    {
        audioManager = GetComponent<UAudioManager>();
    }

    void Start()
    {
        PlayIntroMusic();
    }

    public void PlayEvent(string evt)
    {
        audioManager.PlayEvent(evt);
    }

    public void PlayIntroMusic()
    {
        audioManager.PlayEvent("IntroMusic");
    }

    public void StopIntroMusic()
    {
        audioManager.StopEvent("IntroMusic", 5f);
    }
}
