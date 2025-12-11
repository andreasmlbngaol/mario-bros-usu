using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int world { get; private set; }
    public int stage { get; private set; }
    public int lives { get; private set; }
    
    private AudioSource audioSource;
    public AudioClip backgroundMusic;
    
    private void Awake()
    {
        if (Instance != null) 
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void Start()
    {
        Application.targetFrameRate = 60;
        NewGame();
    }
    
    private void NewGame()
    {
        lives = 3;
        LoadLevel(1, 1);
    }
    
    private void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;
        
        SceneManager.LoadScene($"{world}-{stage}");
        PlayBackgroundMusic();
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !audioSource.isPlaying)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void Nextlevel()
    {
        // // Simulasi level berhenti di level 3
        // if (world == 1 && stage == 3)
        // {
        //     LoadLevel(world + 1, 1);
        // } else if (world == 2 && stage == 3)
        // {
        //     // Finish Game
        //     GameOver();
        // }
        // LoadLevel(this.world, this.stage + 1);

        // Karena hanya ada satu level, buat dia selesai
        if (world == 1 && stage == 1)
        { 
            // GameOver sementara pengganti Game Selesai dengan Menang
            GameOver();
        }
    }

    public void ResetLevel(float delay)
    {
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        lives--;
        if (lives > 0)
        {
            LoadLevel(this.world, this.stage);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        // SceneManager.LoadScene("GameOver");
        NewGame();
    }
}