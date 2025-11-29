using UnityEngine;
using TMPro;

public class MapSelectUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown mapDropdown;
    [SerializeField] private TMP_Dropdown loadoutDropdown;
    [SerializeField] private string[] arenaSceneNames;

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
