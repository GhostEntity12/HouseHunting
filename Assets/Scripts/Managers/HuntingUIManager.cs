using UnityEngine;

public class HuntingUIManager : Singleton<HuntingUIManager>
{
    //ammo
    [SerializeField] private AmmoUI AmmoUi;

    public AmmoUI AmmoUI => AmmoUi;
}
