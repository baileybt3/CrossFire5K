using UnityEngine;
using TMPro;

public class MapSelectUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown mapDropdown;
    [SerializeField] private TMP_Dropdown loadoutDropdown;
    [SerializeField] private string[] arenaSceneNames;

    [Header("Unlock")]
    [SerializeField] private int[] mapUnlockLevels; // Required levels per map
    [SerializeField] private TMP_Text lockedMessageText;

    public void OnPlayButtonPressed()
    {
        int index = mapDropdown.value;

        if (index < 0 || index >= arenaSceneNames.Length)
        {
            Debug.LogWarning($"No scene for dropdown index {index}");
            return;
        }

        string sceneToLoad = arenaSceneNames[index];

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader instance not found!");
            return;
        }

        // XP check for maps unlocked
        int requiredLevel = (mapUnlockLevels != null && index < mapUnlockLevels.Length) ? mapUnlockLevels[index] : 0;

        int playerLevel = PlayerProgression.Instance != null ? PlayerProgression.Instance.CurrentLevel : 0;

        if (playerLevel < requiredLevel)
        {
            if (lockedMessageText != null)
            {
                lockedMessageText.text = $"Locked - reach level {requiredLevel} to play this map.";
            }
            Debug.Log($"Map {sceneToLoad} locked. Need level {requiredLevel}, you are level {playerLevel}.");
            return;
        }

        // Loadout option
        if (ArmoryManager.Instance != null)
        {
            int loadoutIndex = Mathf.Clamp(loadoutDropdown.value, 0, ArmoryManager.Instance.loadouts.Length - 1);
            ArmoryManager.Instance.SetActiveLoadout(loadoutDropdown.value);
        }
        else
        {
            Debug.LogWarning("No ArmoryManger found, using default loadout.");
        }

        SceneLoader.Instance.LoadArena(sceneToLoad);
    }
}
