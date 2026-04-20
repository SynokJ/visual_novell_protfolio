using Spine;
using Spine.Unity;
using UnityEngine;
using System.Collections.Generic;

public class CustomizationManager : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    protected List<Skin.SkinEntry> entries = new List<Skin.SkinEntry>();

    private const string HEAD_SLOT = "Head";
    protected int currentAttachmentId = 0;

    private void Awake()
    {
        var skeleton = skeletonAnimation.Skeleton;
        var slotData = skeleton.Data.FindSlot(HEAD_SLOT);

        if (slotData == null) return;

        var skins = skeleton.Data.Skins;
        for (int s = 0; s < skins.Count; s++)
        {
            var skin = skins.Items[s];
            entries = new List<Skin.SkinEntry>();
            skin.GetAttachments(slotData.Index, entries);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SetNextHead();
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            SetPreviousHead();
    }

    public virtual void SetNextHead()
    {
        currentAttachmentId = Mathf.Clamp(currentAttachmentId + 1, 0, entries.Count - 1);
        SetHead(entries[currentAttachmentId].Name);
    }

    public virtual void SetPreviousHead()
    {
        currentAttachmentId = Mathf.Clamp(currentAttachmentId - 1, 0, entries.Count - 1);
        SetHead(entries[currentAttachmentId].Name);
    }

    public void SetHead(string attachmentName)
    {
        var skeleton = skeletonAnimation.Skeleton;
        skeleton.SetAttachment(HEAD_SLOT, attachmentName);

        skeletonAnimation.AnimationState.Apply(skeleton);
        skeletonAnimation.LateUpdate();
    }

    public void HideHead()
    {
        var skeleton = skeletonAnimation.Skeleton;
        skeleton.FindSlot(HEAD_SLOT).Attachment = null;

        skeletonAnimation.AnimationState.Apply(skeleton);
        skeletonAnimation.LateUpdate();
    }
}