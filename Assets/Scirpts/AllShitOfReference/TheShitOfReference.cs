using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TheShitOfReference
{
    public static float extraAmmoAmount = 0;
    public const float MAX_EXTRA_SPEED = 5f;
    public const float MAX_EXTRA_AMMO = 3.0f;//最多提升??%的弹药量
    public static float extraSpeed = 0f;
    private static AllShitOfReference allShitOfReference;
    public static AllShitOfReference AllShitOfReference{
        get{
            if(allShitOfReference is null){
                allShitOfReference = Resources.Load<AllShitOfReference>("AllShitOfReferenceSO");
            }
            return allShitOfReference;
        }
    }
}
