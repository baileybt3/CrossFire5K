using UnityEditor.Playables;
using UnityEngine;

[System.Serializable]
public class Loadout
{
    public int primaryIndex;
    public int secondaryIndex;
    public int utilityIndex;

}
public class ArmoryManager : MonoBehaviour
{
    public static ArmoryManager Instance { get; private set; }

    [Header("Available Weapon Prefabs")]
    public GameObject[] primaries;
    public GameObject[] secondaries;
    public GameObject[] utilities;

    [Header("Player Loadouts (1-3)")]
    public Loadout[] loadouts = new Loadout[3];

    public int ActiveLoadoutIndex { get; private set; } = 0; // 0 = Loadout 1

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < loadouts.Length; i++)
        {
            if (loadouts[i] == null)
            {
                loadouts[i] = new Loadout();
            }
        }

        LoadFromPrefs();
    }

    // Set weapons
    public void SetPrimary(int loadoutIndex, int weaponIndex)
    {
        loadouts[loadoutIndex].primaryIndex = weaponIndex;
    }

    public void SetSecondary(int loadoutIndex, int weaponIndex)
    {
        loadouts[loadoutIndex].secondaryIndex = weaponIndex;
    }

    public void SetUtility(int loadoutIndex, int weaponIndex)
    {
        loadouts[loadoutIndex].utilityIndex = weaponIndex;
    }

    public void SetActiveLoadout(int loadoutIndex)
    {
        ActiveLoadoutIndex = Mathf.Clamp(loadoutIndex, 0, loadouts.Length - 1);
    }

    // Get weapon for weapon object prefab
    public GameObject GetActivePrimary()
    {
        Loadout l = loadouts[ActiveLoadoutIndex];

        if (l.primaryIndex < 0 || l.primaryIndex >= primaries.Length) return null;
        return primaries[l.primaryIndex];
    }

    public GameObject GetActiveSecondary()
    {
        Loadout l = loadouts[ActiveLoadoutIndex];

        if (l.secondaryIndex < 0 || l.secondaryIndex >= secondaries.Length) return null;
        return secondaries[l.secondaryIndex];
    }

    public GameObject GetActiveUtility()
    {
        Loadout l = loadouts[ActiveLoadoutIndex];

        if (l.utilityIndex < 0 || l.utilityIndex >= utilities.Length) return null;
        return utilities[l.utilityIndex];
    }

    public void SaveToPrefs()
    {
        for (int i = 0; i < loadouts.Length; i++)
        {
            PlayerPrefs.SetInt($"Loadout{i}_Primary", loadouts[i].primaryIndex);
            PlayerPrefs.SetInt($"Loadout{i}_Secondary", loadouts[i].secondaryIndex);
            PlayerPrefs.SetInt($"Loadout{i}_Utility", loadouts[i].utilityIndex);
        }
        PlayerPrefs.SetInt("ActiveLoadoutIndex", ActiveLoadoutIndex);
        PlayerPrefs.Save();
    }

    public void LoadFromPrefs()
    {
        for (int i = 0; i < loadouts.Length; i++)
        {
            if (loadouts[i] == null) loadouts[i] = new Loadout();

            loadouts[i].primaryIndex = PlayerPrefs.GetInt($"Loadout{i}_Primary", loadouts[i].primaryIndex);
            loadouts[i].secondaryIndex = PlayerPrefs.GetInt($"Loadout{i}_Secondary", loadouts[i].secondaryIndex);
            loadouts[i].utilityIndex = PlayerPrefs.GetInt($"Loadout{i}_Utility", loadouts[i].utilityIndex);
        }

        int savedActive = PlayerPrefs.GetInt("ActiveLoadoutIndex", ActiveLoadoutIndex);
        SetActiveLoadout(savedActive);
    }
}
