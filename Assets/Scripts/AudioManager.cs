using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private GameObject audioUI;
    [SerializeField]
    private Slider masterSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private AudioSource backgroundMusic;
    [SerializeField]
    private AudioSource sellSound;
    [SerializeField]
    private AudioSource insertSound;
    [SerializeField]
    private AudioSource punchSound;
    [SerializeField]
    private AudioSource footSound;
    [SerializeField]
    private AudioSource headSound;
    [SerializeField]
    private AudioSource passSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowAudioUI()
    {
        if (audioUI.activeSelf)
        {
            audioUI.SetActive(false);
            GameplayManager.Instance.in_game_tutorial = false;
            GameplayManager.Instance.ToggleCards();
            GameplayManager.Instance.ToggleWBs();
            GameplayManager.Instance.cards_tmp_holder.Clear();
            GameplayManager.Instance.wbs_tmp_holder.Clear();
        }
        else
        {
            audioUI.SetActive(true);
            GameplayManager.Instance.in_game_tutorial = true;
            GameplayManager.Instance.ToggleCards();
            GameplayManager.Instance.ToggleWBs();
        }
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        backgroundMusic.volume = volume / 3.0f; // music too loud anyway, manually lower here
    }

    public void SetSFXVolume(float volume)
    {
        sellSound.volume = volume;
        insertSound.volume = volume / 1.8f;
        punchSound.volume = volume;
        footSound.volume = volume;
        headSound.volume = volume;
        passSound.volume = volume;
    }
}
