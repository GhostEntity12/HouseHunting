public interface IEquippable
{
	public string ID { get; }
	public SoundAlertSO EquipSound { get; }

	public void Equip();
	public void Unequip();
	public void UsePrimary();
	public void UseSecondary();
	public void Reload();
}
