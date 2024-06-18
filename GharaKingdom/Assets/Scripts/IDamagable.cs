using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public int CurrentHP { get; set; }
    public int MaxHP { get; }
    public int Defence { get; }
    public Vector2 Position { get; }
    public void TakeDamage(int damage);
    public void Kill();
}
