using UnityEngine;

[System.Serializable]
public class Sound{

    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    public bool loop = false;

    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    } 

    public void Play(){
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));;
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }

}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    Sound[] sounds;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);  // Удаляет дублирующий экземпляр AudioManager
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);  // Сохраняет единственный экземпляр между сценами

        // Удаление лишних AudioListener
        EnsureSingleAudioListener();
    }

    void EnsureSingleAudioListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            for (int i = 1; i < listeners.Length; i++)
            {
                Destroy(listeners[i]);
            }
        }
    }


    void Start() {
        for (int i = 0; i < sounds.Length; i++){
            GameObject _go = new GameObject("Sounds_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }

        PlaySound("Music");
    }

    public void PlaySound (string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }

        Debug.LogWarning("AudioManager: sound not found in list: " + _name);
    }

    public void StopSound (string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Stop();
                return;
            }
        }

        Debug.LogWarning("AudioManager: sound not found in list: " + _name);
    }
}
