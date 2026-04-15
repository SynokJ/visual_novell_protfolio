using UnityEngine;

[CreateAssetMenu(fileName = nameof(Identifier), menuName = "SOs/ID/" + nameof(Identifier))]
public class Identifier : ScriptableObject
{
    public string Id => id;

    [SerializeField] protected string id = default;

    public static bool operator ==(Identifier idA, Identifier idB)
        => idA.id.Equals(idB.id);

    public static bool operator !=(Identifier idA, Identifier idB)
        => !(idA == idB);

    public override int GetHashCode()
        => id.GetHashCode();

    public override bool Equals(object other)
    {
        if (other is Identifier otherId) return id.Equals(otherId.id);
        else return false;
    }

    public override string ToString()
        => $"{nameof(Identifier)} => {id}";
}
