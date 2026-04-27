using UnityEngine;

[CreateAssetMenu(fileName = "New Slot", menuName = "SO/Slot")]
public class SlotSO : ItemSO
{
    public int nbSlotMax;
    public int nbSlotByDefault;
    public float costMultiplier;
    public override string Id => "slot";
}
