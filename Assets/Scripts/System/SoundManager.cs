//sound manager.cs handles playing sounds and background music
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{

    [System.Serializable]
    public class GameSounds
    {
        public string soundName;
        public AudioClip sound;
    }

    public static SoundManager instance;
    public List<GameSounds> gameSounds = new List<GameSounds>();

    [Header("Background Music")]
    public AudioClip[] backgroundMusic;
    [Range(0, 1)]
    public float musicVolume = 0.5f;
    public bool randomize;
    private AudioSource bgmAudio;
    private int trackIndex;
    private int lastIndex;

    void Awake()
    {
        instance = this;
        SetVolume();
    }

    void Start()
    {

        if (backgroundMusic.Length > 0)
        {
            //Set up bgm audio source
            GameObject bgm = new GameObject("Background Music");
            bgm.AddComponent<AudioSource>();
            bgmAudio = bgm.GetComponent<AudioSource>();
            bgmAudio.GetComponent<AudioSource>().loop = (backgroundMusic.Length == 1); //loop if only 1 track is assigned
            bgmAudio.GetComponent<AudioSource>().spatialBlend = 0;
            int trackIndex = (!randomize) ? 0 : Random.Range(0, backgroundMusic.Length);
            PlayMusicTrack(trackIndex);
        }
    }

    //Plays a sound in the list with 2 parameters - it's name and whether it's 2D/3D
    public void PlaySound(string name, bool sound2D)
    {
        if (sound2D)
        {
            GetComponent<AudioSource>().spatialBlend = 0;
        }
        else {
            GetComponent<AudioSource>().spatialBlend = 1;
        }

        for (int i = 0; i < gameSounds.Count; i++)
        {
            if (name == gameSounds[i].soundName)
            {
                GetComponent<AudioSource>().PlayOneShot(gameSounds[i].sound);
            }
        }
    }

    //Optional if you want to play sound in the list at a certain location
    public void PlaySoundAtLocation(string name, Vector3 location)
    {
        GetComponent<AudioSource>().spatialBlend = 1;

        for (int i = 0; i < gameSounds.Count; i++)
        {
            if (name == gameSounds[i].soundName)
            {
                AudioSource.PlayClipAtPoint(gameSounds[i].sound, location);
            }
        }
    }

    //Optional if you want to play a clip located in a different class at a certain location
    public void PlayClip(AudioClip clip, Vector3 position, float volume, float minDistance)
    {
        GameObject go = new GameObject("One shot audio");
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>() as AudioSource;
        source.spatialBlend = 1.0f;
        source.clip = clip;
        source.volume = volume;
        source.minDistance = minDistance;
        source.Play();
        Destroy(go, clip.length);
    }


    //Music to the ears :)
    void PlayMusicTrack(int index)
    {
        bgmAudio.clip = backgroundMusic[index];
        bgmAudio.Play();
        lastIndex = index;
    }


    void Update()
    {

        if (bgmAudio)
        {
            //Switch track when finishes
            if (!bgmAudio.isPlaying)
            {
                if (randomize)
                {
                    //Play a new random track
                    NewRandomTrack();
                }
                else
                {
                    trackIndex++;
                    if (trackIndex >= backgroundMusic.Length) { trackIndex = 0; }
                    PlayMusicTrack(trackIndex);
                }
            }

            //Handle music volume
            bgmAudio.volume = musicVolume;
        }
    }


    void NewRandomTrack()
    {
        int val = 0;

        Init:
        while (true)
        {
            val = Random.Range(0, backgroundMusic.Length);
            for (int i = 0; i < backgroundMusic.Length; i++)
            {
                if (val == lastIndex) goto Init;
            }
            goto Done;
        }

        Done:
        PlayMusicTrack(val);
    }


    //Sets saved volume
    public void SetVolume()
    {
        if (PlayerPrefs.HasKey("Sound"))
            AudioListener.volume = PlayerPrefs.GetFloat("Sound");
        else
            AudioListener.volume = 1.0f;
    }
}
