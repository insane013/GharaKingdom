using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageDealer
{
    public int Damage { get; }
    public IEnumerator DealDamage(IDamagable target);
}
