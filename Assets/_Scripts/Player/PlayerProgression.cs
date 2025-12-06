using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression Instance { get; private set; }

    [Header("XP Settings")]
    [SerializeField] private int baseXPToLevelUp = 100;
    [SerializeField] private int xpPerLevelIncrease = 25;

    public int CurrentLevel { get; private set; }
    public int CurrentXP { get; private set; }
    public int XPToNextLevel => baseXPToLevelUp + (CurrentLevel * xpPerLevelIncrease);


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadFromPrefs();
    }

    public void AddXP(int amount)
    {
        if(amount <= 0)
        {
            return;
        }

        CurrentXP += amount;

        while (CurrentXP >= XPToNextLevel)
        {
            CurrentXP -= XPToNextLevel;
            CurrentLevel++;
        }

        SaveToPrefs();
    }

    private void LoadFromPrefs()
    {
        CurrentLevel = PlayerPrefs.GetInt("XP_Level", 0);
        CurrentXP = PlayerPrefs.GetInt("XP_Current", 0);
    }

    private void SaveToPrefs()
    {
        PlayerPrefs.SetInt("XP_Level", CurrentLevel);
        PlayerPrefs.SetInt("XP_Current", CurrentXP);
        PlayerPrefs.Save();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
