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

    // Handle xp and level ups
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

    // Load saved level and XP
    private void LoadFromPrefs()
    {
        CurrentLevel = PlayerPrefs.GetInt("XP_Level", 0);
        CurrentXP = PlayerPrefs.GetInt("XP_Current", 0);
    }

    // Save current level and XP
    private void SaveToPrefs()
    {
        PlayerPrefs.SetInt("XP_Level", CurrentLevel);
        PlayerPrefs.SetInt("XP_Current", CurrentXP);
        PlayerPrefs.Save();
    }
}
