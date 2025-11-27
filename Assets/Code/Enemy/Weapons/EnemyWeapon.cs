using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    protected EnemyWeaponData data;
   
    public virtual void Initialise(EnemyWeaponData data)
    {
        this.data = data;
    }

    public virtual void Fire()
    {
        
    }
}
