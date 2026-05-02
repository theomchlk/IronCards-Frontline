using UnityEngine;

[CreateAssetMenu(menuName = "SO/MillSO")]
public class MillSO : ItemSO
{
    public int nbMillsByDefault;
    public float costMultiplier;
    public override string Id => "mill";

    public override IItem CreateItemInstance()
    {
        return new MillItemLogic(this);
    }
}
