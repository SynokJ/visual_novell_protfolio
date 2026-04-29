using Spine;
using Spine.Unity;
using UnityEngine;
using System.Collections.Generic;

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
        if(currentHairFrontAttachmentId >= headEntriesFront.Count)
            currentHairFrontAttachmentId = 0;
        SetHead(headEntriesFront[currentHairFrontAttachmentId].Name, HAIR_SLOT_F);

        currentHairBackAttachmentId = currentHairBackAttachmentId + 1;
        if(currentHairBackAttachmentId >= headEntriesBack.Count)
            currentHairBackAttachmentId = 0;
        SetHead(headEntriesBack[currentHairBackAttachmentId].Name, HAIR_SLOT_B);
    }

    public virtual void SetPreviousHead()
    {
        currentHairFrontAttachmentId = currentHairFrontAttachmentId - 1;
        if(currentHairFrontAttachmentId < 0)
            currentHairFrontAttachmentId = headEntriesFront.Count - 1;
        SetHead(headEntriesFront[currentHairFrontAttachmentId].Name, HAIR_SLOT_F);

        currentHairBackAttachmentId = currentHairBackAttachmentId - 1;
        if(currentHairBackAttachmentId < 0)
            currentHairBackAttachmentId = headEntriesBack.Count - 1;
        SetHead(headEntriesBack[currentHairBackAttachmentId].Name, HAIR_SLOT_B);
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
        currentBodyAttachmentId = Mathf.Clamp(currentBodyAttachmentId + 1, 0, bodyEntries.Count - 1);
        SetBody(bodyEntries[currentBodyAttachmentId].Name, BODY_SLOT);
    }

    public virtual void SetPreviousBody()
    {
        currentBodyAttachmentId = Mathf.Clamp(currentBodyAttachmentId - 1, 0, bodyEntries.Count - 1);
        SetBody(bodyEntries[currentBodyAttachmentId].Name, BODY_SLOT);
    }

    public void SetBody(string attachmentName, string slotName)
    {
        var skeleton = skeletonAnimation.Skeleton;
        skeleton.SetAttachment(slotName, attachmentName);

        skeletonAnimation.AnimationState.Apply(skeleton);
        skeletonAnimation.LateUpdate();
    }
}