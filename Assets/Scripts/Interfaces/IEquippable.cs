using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquippable
{
	string ID { get; }
	SoundAlertSO EquipSound { get; }

	void Equip();
	void Unequip();
	void UsePrimary();
	void UseSecondary();
	void Reload();
}
