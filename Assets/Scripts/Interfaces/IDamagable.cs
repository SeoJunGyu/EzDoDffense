using UnityEngine;

public interface IDamagable
{
    public void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}
