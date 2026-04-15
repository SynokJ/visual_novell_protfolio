using UnityEngine;

[CreateAssetMenu(fileName = nameof(AudioManagerModel), menuName = "SOs/Audio/" + nameof(AudioManagerModel))]
public class AudioManagerModel : ScriptableObject
{
    public AudioParameter[] Parameters => parameters;

    [SerializeField] protected AudioParameter[] parameters = default;
}

[System.Serializable]
public class AudioParameter
{
    public AudioClip Clip => clip;
    public Identifier Identifier => identifier;

    [SerializeField] protected AudioClip clip = default;
    [SerializeField] protected Identifier identifier = default;
}
