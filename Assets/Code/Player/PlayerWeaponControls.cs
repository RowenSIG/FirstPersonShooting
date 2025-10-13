using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponControls : PlayerComponentControls
{
    [SerializeField]
    private PlayerWeaponZeroSword zeroSwordWeapon;

    [SerializeField]
    private PlayerWeaponHitScan pistolWeapon;
    [SerializeField]
    private PlayerWeaponHitScan machineGunWeapon;

    private PlayerWeapon currentWeapon;
    private List<PlayerWeapon> allWeapons = new List<PlayerWeapon>();

    public override void Setup(PlayerConfiguration config, Player player)
    {
        base.Setup(config, player);

        allWeapons.Add(zeroSwordWeapon);
        allWeapons.Add(pistolWeapon);
        allWeapons.Add(machineGunWeapon);
        
        foreach(var weapon in allWeapons)
        {
            weapon.Setup(player);
        }

        SetCurrentWeapon(zeroSwordWeapon);
    }

    private void SetCurrentWeapon(PlayerWeapon zWeapon)
    {
        foreach(var weapon in allWeapons)
        {
            weapon.IsVisible = weapon == zWeapon;
        }

        currentWeapon = zWeapon;
    }

    public override void UpdateFireInput(bool leftInput, bool rightInput, bool reloadButton)
    {
        //bang
        currentWeapon.UpdateWeapon(PlayerDT, leftInput, rightInput, reloadButton);
    }

    public override void UpdatePrevNextInput(bool prev, bool next, float scrollInput)
    {
        if (scrollInput < 0)
            prev |= true;
        if (scrollInput > 0)
            next |= true;

        var currentWeaponIndex = allWeapons.IndexOf(currentWeapon);
        if (prev)
            currentWeaponIndex -= 1;
        else if (next)
            currentWeaponIndex += 1;
        currentWeaponIndex = allWeapons.WrapIndex(currentWeaponIndex);
        SetCurrentWeapon(allWeapons[currentWeaponIndex]);
    }

}
