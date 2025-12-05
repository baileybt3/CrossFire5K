using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private string[] menuSceneNames;

    [Header("UI SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip uiClickClip;
    [SerializeField] private AudioClip uiBackClip;

    [Header("Game SFX")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip healthPickupClip;
    [SerializeField] private AudioClip ammoPickupClip;
    [SerializeField] private AudioClip rifleClip;
    [SerializeField] private AudioClip smgClip;
    [SerializeField] private AudioClip pistolClip;
    [SerializeField] private AudioClip reloadClip;
    [SerializeField] private AudioClip explosionClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Add Sources
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.spatialBlend = 0f;

        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMenuScene = false;

        foreach (string name in menuSceneNames)
        {
            if (scene.name == name) {
                isMenuScene = true;
                break;
            }
        }

        if (isMenuScene)
        {
            PlayMusic(menuMusic);
        }
        else
        {
            PlayMusic(gameplayMusic);
        }

    }

    private void PlayMusic(AudioClip clip) {
        if (clip == null || musicSource == null)
        {
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }


    // UI
    public void PlayUIClick() => PlaySFX(uiClickClip);
    public void PlayUIBack() => PlaySFX(uiBackClip);

    // Player
    public void PlayFootstep() => PlaySFX(footstepClip);
    public void PlayHealthPickup() => PlaySFX(healthPickupClip);
    public void PlayAmmoPickup() => PlaySFX(ammoPickupClip);

    // Weapons
    public void PlayRifleShot() => PlaySFX(rifleClip);
    public void PlaySMGShot() => PlaySFX(smgClip);

    public void PlayPistolShot() => PlaySFX(pistolClip);

    public void PlayReload() => PlaySFX(reloadClip);

    public void PlayExplosionAt(Vector3 position)
    {
        if (explosionClip == null)
        {
            return;
        }
        AudioSource.PlayClipAtPoint(explosionClip, position);

    }

}
