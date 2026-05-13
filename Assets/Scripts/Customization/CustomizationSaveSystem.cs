using System;
using UnityEngine;

public static class CustomizationSaveSystem
{
    private const string SAVE_KEY = "CUSTOMIZATION_SAVE_DATA";

    public static void Save(CustomizationSaveData saveData)
    {
        if (saveData == null)
        {
            Debug.LogWarning("[CustomizationSaveSystem] Save failed: saveData is null.");
            return;
        }

        string json = JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"[CustomizationSaveSystem] Saved: {json}");
    }

    public static bool TryLoad(out CustomizationSaveData saveData)
    {
        saveData = null;

        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[CustomizationSaveSystem] No saved customization data found.");
            return false;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("[CustomizationSaveSystem] Save exists but JSON is empty.");
            return false;
        }

        try
        {
            saveData = JsonUtility.FromJson<CustomizationSaveData>(json);
        }
        catch
        {
            Debug.LogWarning($"[CustomizationSaveSystem] Failed to parse JSON: {json}");
            return false;
        }

        if (saveData == null)
        {
            Debug.LogWarning("[CustomizationSaveSystem] Loaded data is null.");
            return false;
        }

        Debug.Log($"[CustomizationSaveSystem] Loaded: {json}");
        return true;
    }

    public static void DeleteSave()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
            return;

        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();

        Debug.Log("[CustomizationSaveSystem] Save deleted.");
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }
}

[Serializable]
public class CustomizationSaveData
{
    public string hairFrontAttachmentName;
    public string hairBackAttachmentName;
    public string bodyAttachmentName;

    public CustomizationSaveData(
        string hairFrontAttachmentName,
        string hairBackAttachmentName,
        string bodyAttachmentName)
    {
        this.hairFrontAttachmentName = hairFrontAttachmentName;
        this.hairBackAttachmentName = hairBackAttachmentName;
        this.bodyAttachmentName = bodyAttachmentName;
    }
}