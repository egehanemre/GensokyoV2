using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxObject;

    [Header("Fairy SFX")]
    public AudioClip moveSFX;
    public AudioClip attackSFX;
    public AudioClip blockSFX;
    public AudioClip projectileAttackSFX;

    public AudioClip[] damagedSFX;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, Transform spawnTransform, float volume)
    {
     
        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, spawnTransform.rotation);

        audioSource.clip = clip;
        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length; 
        Destroy(audioSource.gameObject, clipLength); 
    }
    public AudioClip PlayRandomDamagedSFX()
    {
        if (damagedSFX != null && damagedSFX.Length > 0)
        {
            int index = Random.Range(0, damagedSFX.Length);
            AudioClip chosenClip = damagedSFX[index];
            return chosenClip;
        }
        return null;
    }
}
