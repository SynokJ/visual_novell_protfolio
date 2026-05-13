using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationManager : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    protected List<Skin.SkinEntry> headEntriesFront = new List<Skin.SkinEntry>();
    protected List<Skin.SkinEntry> headEntriesBack = new List<Skin.SkinEntry>();
    protected List<Skin.SkinEntry> bodyEntries = new List<Skin.SkinEntry>();

    private const string HAIR_SLOT_F = "Hair_Front";
    private const string HAIR_SLOT_B = "Hair_Back";
    private const string BODY_SLOT = "Dress";

    protected int currentHairFrontAttachmentId = 0;
    protected int currentHairBackAttachmentId = 0;
    protected int currentBodyAttachmentId = 0;

    private void Awake()
    {
        InitSlotData(HAIR_SLOT_F, ref headEntriesFront);
        InitSlotData(HAIR_SLOT_B, ref headEntriesBack);
        InitSlotData(BODY_SLOT, ref bodyEntries);

        LoadSavedCustomization();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SetNextHead();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            SetPreviousHead();
    }

    protected virtual void InitSlotData(string slotName, ref List<Skin.SkinEntry> entries)
    {
        var skeleton = skeletonAnimation.Skeleton;
        var slotData = skeleton.Data.FindSlot(slotName);

        if (slotData == null) return;

        var skins = skeleton.Data.Skins;
        for (int s = 0; s < skins.Count; s++)
        {
            entries.Clear();
            var skin = skins.Items[s];
            skin.GetAttachments(slotData.Index, entries);
        }
    }

    public virtual void SetNextHead()
    {
        currentHairFrontAttachmentId = currentHairFrontAttachmentId + 1;
        if (currentHairFrontAttachmentId >= headEntriesFront.Count)
            currentHairFrontAttachmentId = 0;
        SetHead(headEntriesFront[currentHairFrontAttachmentId].Name, HAIR_SLOT_F);

        currentHairBackAttachmentId = currentHairBackAttachmentId + 1;
        if (currentHairBackAttachmentId >= headEntriesBack.Count)
            currentHairBackAttachmentId = 0;
        SetHead(headEntriesBack[currentHairBackAttachmentId].Name, HAIR_SLOT_B);

        SaveCurrentCustomization();
    }

    public virtual void SetPreviousHead()
    {
        currentHairFrontAttachmentId = currentHairFrontAttachmentId - 1;
        if (currentHairFrontAttachmentId < 0)
            currentHairFrontAttachmentId = headEntriesFront.Count - 1;
        SetHead(headEntriesFront[currentHairFrontAttachmentId].Name, HAIR_SLOT_F);

        currentHairBackAttachmentId = currentHairBackAttachmentId - 1;
        if (currentHairBackAttachmentId < 0)
            currentHairBackAttachmentId = headEntriesBack.Count - 1;
        SetHead(headEntriesBack[currentHairBackAttachmentId].Name, HAIR_SLOT_B);

        SaveCurrentCustomization();
    }

    public void SetHead(string attachmentName, string slotName)
    {
        var skeleton = skeletonAnimation.Skeleton;
        skeleton.SetAttachment(slotName, attachmentName);

        skeletonAnimation.AnimationState.Apply(skeleton);
        skeletonAnimation.LateUpdate();
    }

    public void HideHead(string slotName)
    {
        var skeleton = skeletonAnimation.Skeleton;
        skeleton.FindSlot(slotName).Attachment = null;

        skeletonAnimation.AnimationState.Apply(skeleton);
        skeletonAnimation.LateUpdate();
    }

    public virtual void SetNextBody()
    {
        currentBodyAttachmentId = currentBodyAttachmentId + 1;
        if (currentBodyAttachmentId > bodyEntries.Count - 1)
            currentBodyAttachmentId = 0;

        SetBody(bodyEntries[currentBodyAttachmentId].Name, BODY_SLOT);

        SaveCurrentCustomization();
    }

    public virtual void SetPreviousBody()
    {
        currentBodyAttachmentId = currentBodyAttachmentId - 1;
        if (currentBodyAttachmentId < 0)
            currentBodyAttachmentId = bodyEntries.Count - 1;

        SetBody(bodyEntries[currentBodyAttachmentId].Name, BODY_SLOT);

        SaveCurrentCustomization();
    }

    public void SetBody(string attachmentName, string slotName)
    {
        var skeleton = skeletonAnimation.Skeleton;
        skeleton.SetAttachment(slotName, attachmentName);

        skeletonAnimation.AnimationState.Apply(skeleton);
        skeletonAnimation.LateUpdate();
    }

    public void SaveCurrentCustomization()
    {
        if (headEntriesFront.Count == 0 || headEntriesBack.Count == 0 || bodyEntries.Count == 0)
        {
            Debug.LogWarning("[CustomizationManager] Cannot save customization: one or more entry lists are empty.");
            return;
        }

        var saveData = new CustomizationSaveData(
            headEntriesFront[currentHairFrontAttachmentId].Name,
            headEntriesBack[currentHairBackAttachmentId].Name,
            bodyEntries[currentBodyAttachmentId].Name
        );

        CustomizationSaveSystem.Save(saveData);
    }

    public void LoadSavedCustomization()
    {
        if (!CustomizationSaveSystem.TryLoad(out CustomizationSaveData saveData))
            return;

        ApplySavedHead(
            saveData.hairFrontAttachmentName,
            headEntriesFront,
            ref currentHairFrontAttachmentId,
            HAIR_SLOT_F
        );

        ApplySavedHead(
            saveData.hairBackAttachmentName,
            headEntriesBack,
            ref currentHairBackAttachmentId,
            HAIR_SLOT_B
        );

        ApplySavedBody(
            saveData.bodyAttachmentName,
            bodyEntries,
            ref currentBodyAttachmentId,
            BODY_SLOT
        );
    }

    private void ApplySavedHead(
        string savedAttachmentName,
        List<Skin.SkinEntry> entries,
        ref int currentId,
        string slotName)
    {
        if (string.IsNullOrEmpty(savedAttachmentName))
            return;

        int index = entries.FindIndex(entry => entry.Name == savedAttachmentName);

        if (index < 0)
        {
            Debug.LogWarning($"[CustomizationManager] Saved head attachment not found: {savedAttachmentName}");
            return;
        }

        currentId = index;
        SetHead(savedAttachmentName, slotName);
    }

    private void ApplySavedBody(
        string savedAttachmentName,
        List<Skin.SkinEntry> entries,
        ref int currentId,
        string slotName)
    {
        if (string.IsNullOrEmpty(savedAttachmentName))
            return;

        int index = entries.FindIndex(entry => entry.Name == savedAttachmentName);

        if (index < 0)
        {
            Debug.LogWarning($"[CustomizationManager] Saved body attachment not found: {savedAttachmentName}");
            return;
        }

        currentId = index;
        SetBody(savedAttachmentName, slotName);
    }
}