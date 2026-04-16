using UnityEngine;

public class AudioManagerController : AbstractDataComponentUploader<AudioManagerController>
{
    [SerializeField] protected AudioSource audioSource = default;
    [SerializeField] protected AudioManagerModel audioModel = default;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public virtual void PlayAudioById(Identifier id)
    {
        foreach (AudioParameter tempParameter in audioModel.Parameters)
            if (tempParameter.Identifier == id)
            {
                audioSource.PlayOneShot(tempParameter.Clip);
                break;
            }
    }
}
