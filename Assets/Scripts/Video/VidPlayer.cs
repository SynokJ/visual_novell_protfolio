using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField] protected string videoFileName = default;

    private void Start()
        => PlayVideo();

    public virtual void PlayVideo()
    {
        if (TryGetComponent(out VideoPlayer videoPlayer))
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
    }
}
