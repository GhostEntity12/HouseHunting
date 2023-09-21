using UnityEngine;

public class HuntingInputManager : Singleton<HuntingInputManager>
{
	[SerializeField] WeaponWheel weaponWheelController;

	private PlayerInput playerInput;

	public PlayerInput PlayerInput => playerInput;

	protected override void Awake()
	{
        base.Awake();

		playerInput = GeneralInputManager.Instance.PlayerInput;

		// weapon wheel
		playerInput.Hunting.OpenWeaponWheel.started += ctx => OpenWeaponWheel();
		playerInput.Hunting.OpenWeaponWheel.canceled += ctx => CloseWeaponWheel();

		//removed for alpha
		//playerInput.Hunting.OpenInventory.performed += ctx => ShopUIManager.Instance.ToggleShop();

		// shoot
		playerInput.Hunting.UsePrimary.performed += ctx => WeaponManager.Instance.CurrentGun.Shoot();

		// ADS
		playerInput.Hunting.UseSecondary.performed += ctx => WeaponManager.Instance.CurrentGun.ToggleADS();

		// reload
		playerInput.Hunting.Reload.performed += ctx => WeaponManager.Instance.CurrentGun.Reload();

		// select weapon
		playerInput.Hunting.Quick1.performed += ctx => WeaponManager.Instance.SelectItem(0);
		playerInput.Hunting.Quick2.performed += ctx => WeaponManager.Instance.SelectItem(1);
        playerInput.Hunting.Quick3.performed += ctx => WeaponManager.Instance.SelectItem(2);
        playerInput.Hunting.Quick4.performed += ctx => WeaponManager.Instance.SelectItem(3);
        playerInput.Hunting.Quick5.performed += ctx => WeaponManager.Instance.SelectItem(4);
        playerInput.Hunting.Quick6.performed += ctx => WeaponManager.Instance.SelectItem(5);
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
		playerInput.Hunting.UseSecondary.Disable();
		weaponWheelController.OpenWeaponWheel();
    }

	private void CloseWeaponWheel()
	{
		playerInput.General.Look.Enable();
		playerInput.Hunting.UsePrimary.Enable();
		playerInput.Hunting.UseSecondary.Enable();
		weaponWheelController.CloseWeaponWheel();
	}

	public bool WeaponWheelIsOpen()
	{
		return weaponWheelController.GetOpen();
	}

	/// <summary>
	/// Enables firing of the gun. Not done in awake to allow for setup (campfires) without firing weapon.
	/// </summary>
	public void EnableShooting() => playerInput.Hunting.UsePrimary.performed += ctx => WeaponManager.Instance.CurrentGun.Shoot();
}
