using UnityEngine;
using TMPro;

public class ArmoryUI : MonoBehaviour
{
    [Header("Loadout 1")]
    [SerializeField] private TMP_Dropdown primaryDropdown1;
    [SerializeField] private TMP_Dropdown secondaryDropdown1;
    [SerializeField] private TMP_Dropdown utilityDropdown1;

    [Header("Loadout 2")]
    [SerializeField] private TMP_Dropdown primaryDropdown2;
    [SerializeField] private TMP_Dropdown secondaryDropdown2;
    [SerializeField] private TMP_Dropdown utilityDropdown2;

    [Header("Loadout 3")]
    [SerializeField] private TMP_Dropdown primaryDropdown3;
    [SerializeField] private TMP_Dropdown secondaryDropdown3;
    [SerializeField] private TMP_Dropdown utilityDropdown3;

    [Header("Progression UI")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private UnityEngine.UI.Image xpFillImage;
    [SerializeField] private TextMeshProUGUI nextUnlockText;

    [System.Serializable]
    private class UnlockHint
    {
        public int level;
        [TextArea] public string description;
    }

    [Header("Unlock Hints")]
    [SerializeField] private UnlockHint[] unlockHints;

    private void Start()
    {
        // Load saved dropdowns
        LoadFromManager();
    }

    // Save all loadout button call
    public void SaveAllLoadouts()
    {
        if (ArmoryManager.Instance == null)
        {
            Debug.LogError("ArmoryManager.Instance not found!");
            return;
        }

        // Loadout 1 (index 0)
        ArmoryManager.Instance.SetPrimary(0, primaryDropdown1.value);
        ArmoryManager.Instance.SetSecondary(0, secondaryDropdown1.value);
        ArmoryManager.Instance.SetUtility(0, utilityDropdown1.value);

        // Loadout 2 (index 1)
        ArmoryManager.Instance.SetPrimary(1, primaryDropdown2.value);
        ArmoryManager.Instance.SetSecondary(1, secondaryDropdown2.value);
        ArmoryManager.Instance.SetUtility(1, utilityDropdown2.value);

        // Loadout 3 (index 2)
        ArmoryManager.Instance.SetPrimary(2, primaryDropdown3.value);
        ArmoryManager.Instance.SetSecondary(2, secondaryDropdown3.value);
        ArmoryManager.Instance.SetUtility(2, utilityDropdown3.value);

        ArmoryManager.Instance.SaveToPrefs();
        Debug.Log("All loadouts saved.");
    }

    private void LoadFromManager()
    {
        if (ArmoryManager.Instance == null)
        {
            return;
        }

        var l0 = ArmoryManager.Instance.loadouts[0];
        var l1 = ArmoryManager.Instance.loadouts[1];
        var l2 = ArmoryManager.Instance.loadouts[2];

        // Clamp so we don't go out of range if arrays changed
        primaryDropdown1.value = Mathf.Clamp(l0.primaryIndex, 0, primaryDropdown1.options.Count - 1);
        secondaryDropdown1.value = Mathf.Clamp(l0.secondaryIndex, 0, secondaryDropdown1.options.Count - 1);
        utilityDropdown1.value = Mathf.Clamp(l0.utilityIndex, 0, utilityDropdown1.options.Count - 1);

        primaryDropdown2.value = Mathf.Clamp(l1.primaryIndex, 0, primaryDropdown2.options.Count - 1);
        secondaryDropdown2.value = Mathf.Clamp(l1.secondaryIndex, 0, secondaryDropdown2.options.Count - 1);
        utilityDropdown2.value = Mathf.Clamp(l1.utilityIndex, 0, utilityDropdown2.options.Count - 1);

        primaryDropdown3.value = Mathf.Clamp(l2.primaryIndex, 0, primaryDropdown3.options.Count - 1);
        secondaryDropdown3.value = Mathf.Clamp(l2.secondaryIndex, 0, secondaryDropdown3.options.Count - 1);
        utilityDropdown3.value = Mathf.Clamp(l2.utilityIndex, 0, utilityDropdown3.options.Count - 1);

        UpdateProgressionUI();
    }

    private void UpdateProgressionUI()
    {        
        int level = PlayerProgression.Instance.CurrentLevel;    
        int currentXP = PlayerProgression.Instance.CurrentXP;     
        int xpToNext = PlayerProgression.Instance.XPToNextLevel;
   
        if (levelText != null)  
        {
            levelText.text = $"Level {level}";
        }
        
        if (xpText != null)
        {
            xpText.text = $"{currentXP} / {xpToNext} XP";
        }

        if (xpFillImage != null)
        {
            float fill = xpToNext > 0 ? (float)currentXP / xpToNext : 0f;
            xpFillImage.fillAmount = Mathf.Clamp01(fill);
        }

        if (nextUnlockText != null)
        {
            nextUnlockText.text = BuildNextUnlockDescription(level);
        }
    }

    private string BuildNextUnlockDescription(int currentLevel)
    {
        if (unlockHints == null || unlockHints.Length == 0)
        {
            return "No unlock data.";
        }

        UnlockHint next = null;

        foreach(var hint in unlockHints)
        {
            if (hint.level > currentLevel && (next == null || hint.level < next.level))
            {
                next = hint;
            }
        }

        if(next == null)
        {
            return "All rewards unlocked.";
        }

        return $"Next unlock at Level {next.level}: {next.description}";
    }

    private void OnDisable()
    {
        // Auto-save
        if(ArmoryManager.Instance != null)
        {
            SaveAllLoadouts();
        }
    }
}
