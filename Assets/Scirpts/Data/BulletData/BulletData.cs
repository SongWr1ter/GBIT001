using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BulletData", menuName = "Weapons/BulletData")]
public class BulletData : ScriptableObject
{
    public float speed = 10f;
    public float deadTime = 3f;
    public bool canBounceBack;
    public LayerMask layer;
    public int iteration;
    public int damage;
    public string VfxName = "Normal";
}
