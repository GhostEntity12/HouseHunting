using UnityEngine;
using System.Collections;

public class HuntingInputManager : Singleton<HuntingInputManager>
{
	[SerializeField] WeaponWheel weaponWheelController;

	private PlayerInput playerInput;

	public PlayerInput PlayerInput => playerInput;

	public Transform cam;
	public GameObject objectToThrow;
	private bool lureReload;
	public float throwForce;
	public float throwUpwardForce;

	protected override void Awake()
	{
		base.Awake();

		playerInput = GeneralInputManager.Instance.PlayerInput;
		lureReload = true;

		// weapon wheel
		playerInput.Hunting.OpenWeaponWheel.started += ctx => OpenWeaponWheel();
		playerInput.Hunting.OpenWeaponWheel.canceled += ctx => CloseWeaponWheel();

		//removed for alpha
		//playerInput.Hunting.OpenInventory.performed += ctx => ShopUIManager.Instance.ToggleShop();

		// shoot
		if (!FindAnyObjectByType<CampfireManager>())
			playerInput.Hunting.Shoot.performed += ctx => EquipmentManager.Instance.EquippedItem.UsePrimary();

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
		playerInput.Hunting.Lure.performed += ctx => Lure();
	}


	private void Lure()
	{
		if (lureReload == true)
		{
			GameObject projectile = Instantiate(objectToThrow, cam.position, cam.rotation);
			Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
			Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up * throwUpwardForce;

			projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
			lureReload = false;
			StartCoroutine("LureTimer");
		}
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
		playerInput.Hunting.Shoot.Disable();
		weaponWheelController.OpenWeaponWheel();
	}

	private void CloseWeaponWheel()
	{
		playerInput.General.Look.Enable();
		playerInput.Hunting.Shoot.Enable();
		weaponWheelController.CloseWeaponWheel();
	}

	public bool WeaponWheelIsOpen()
	{
		return weaponWheelController.GetOpen();
	}

	private IEnumerator LureTimer()
    {
        yield return new WaitForSeconds(8);
		lureReload = true;
    }

	/// <summary>
	/// Enables firing of the gun. Not done in awake to allow for setup (campfires) without firing weapon.
	/// </summary>
	public void EnableShooting() => playerInput.Hunting.Shoot.performed += ctx => EquipmentManager.Instance.EquippedItem.UsePrimary();
}
