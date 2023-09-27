using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HuntingUIManager : Singleton<HuntingUIManager>
{
    //ammo
    [SerializeField] private AmmoUI AmmoUi;

    public AmmoUI AmmoUI => AmmoUi;
}
