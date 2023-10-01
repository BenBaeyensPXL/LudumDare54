using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip musicClip;
    public AudioClip buttonBlip;
    public AudioClip pipePlace;
    public AudioClip pipeConnect;
    public AudioClip pipeDeleteSound;
    public AudioClip victorySound;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    public static AudioManager instance;

    private void OnEnable()
    {
        EventManager.OnButtonClick += PlayButtonBlip;
        EventManager.OnConnectNodes += PlayPipeConnectSound;
        EventManager.OnCancelConnection += PlayPipeDeleteSound;
        EventManager.OnPipePlace += PlayPipePlaceSound;
        EventManager.OnVictory += PlayVictorySound;
    }

    private void OnDisable()
    {
        EventManager.OnButtonClick -= PlayButtonBlip;
        EventManager.OnConnectNodes -= PlayPipeConnectSound;
        EventManager.OnCancelConnection -= PlayPipeDeleteSound;
        EventManager.OnPipePlace -= PlayPipePlaceSound;

    }

    private void Awake()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void PlayButtonBlip()
    {
        sfxSource.PlayOneShot(buttonBlip);
    }

    private void PlayPipePlaceSound()
    {
        sfxSource.PlayOneShot(pipePlace);
    }

    private void PlayPipeConnectSound()
    {
        sfxSource.PlayOneShot(pipeConnect);
    }

    private void PlayPipeDeleteSound()
    {
        sfxSource.PlayOneShot(pipeDeleteSound);
    }

    private void PlayVictorySound()
    {
        sfxSource.PlayOneShot(victorySound);
    }
}
