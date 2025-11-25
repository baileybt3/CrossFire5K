using UnityEngine;
using TMPro;

public class MapSelectUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown mapDropdown;
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

        SceneLoader.Instance.LoadArena(sceneToLoad);
    }
}
