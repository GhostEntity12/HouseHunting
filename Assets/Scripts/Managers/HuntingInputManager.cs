using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class HuntingInputManager : Singleton<HuntingInputManager>
{
	[SerializeField] private WeaponWheel weaponWheelController;

	private PlayerInput playerInput;

	public PlayerInput PlayerInput => playerInput;

	protected override void Awake()
	{
		base.Awake();

		playerInput = GeneralInputManager.Instance.PlayerInput;

		// weapon wheel
		playerInput.Hunting.OpenWeaponWheel.started += ctx => OpenWeaponWheel();
		playerInput.Hunting.OpenWeaponWheel.canceled += ctx => CloseWeaponWheel();

		// shoot
		if (!FindAnyObjectByType<CampfireManager>())
			playerInput.Hunting.UsePrimary.performed += ctx => EquipmentManager.Instance.EquippedItem.UsePrimary();

		// reload
		playerInput.Hunting.Reload.performed += ctx => EquipmentManager.Instance.EquippedItem.Reload();

		// select weapon
		playerInput.Hunting.Quick1.performed += ctx => EquipmentManager.Instance.SelectItem(0);
		playerInput.Hunting.Quick2.performed += ctx => EquipmentManager.Instance.SelectItem(1);
		playerInput.Hunting.Quick3.performed += ctx => EquipmentManager.Instance.SelectItem(2);
		playerInput.Hunting.Quick4.performed += ctx => EquipmentManager.Instance.SelectItem(3);
		playerInput.Hunting.Quick5.performed += ctx => EquipmentManager.Instance.SelectItem(4);
		playerInput.Hunting.Quick6.performed += ctx => EquipmentManager.Instance.SelectItem(5);

		// ADS
		playerInput.Hunting.ADS.performed += ctx => EquipmentManager.Instance.EquippedItem.UseSecondary();

		// Lure
		playerInput.Hunting.Lure.performed += ctx => ThrowLure();
	}

	private void OnEnable()
	{
		playerInput.Hunting.Enable();
	}

	private void OnDisable()
	{
		playerInput.Hunting.Disable();
	}

	private void OpenWeaponWheel()
	{
		playerInput.General.Look.Disable();
		playerInput.Hunting.UsePrimary.Disable();
		weaponWheelController.OpenWeaponWheel();
	}

	private void CloseWeaponWheel()
	{
		playerInput.General.Look.Enable();
		playerInput.Hunting.UsePrimary.Enable();
		weaponWheelController.CloseWeaponWheel();
	}

	public bool WeaponWheelIsOpen()
	{
		return weaponWheelController.GetOpen();
	}

	public void ThrowLure()
	{
		if (Lure.lureNotOnCooldown)
		{
			Instantiate(HuntingManager.Instance.LurePrefab.gameObject, Camera.main.transform.position, Camera.main.transform.rotation);
		}
	}

	/// <summary>
	/// Enables firing of the gun. Not done in awake to allow for setup (campfires) without firing weapon.
	/// </summary>
	public void EnableShooting() => playerInput.Hunting.UsePrimary.performed += ctx => EquipmentManager.Instance.EquippedItem.UsePrimary();

	private void OnDestroy() => playerInput.Dispose();
}
