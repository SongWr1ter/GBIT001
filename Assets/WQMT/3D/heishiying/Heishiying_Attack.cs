using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Heishiying_Attack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public BulletData bulletData;
    public Transform TargetTrans;
    private bool isFocus;
    private Transform bulletTargetTrans;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetMouseButtonDown(1))
        {
            isFocus = true;
            bulletTargetTrans = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isFocus = false;
            bulletTargetTrans = TargetTrans;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position,GetShotRotation());
        var bulletScript = bullet.GetComponent<Heshiying_Bullet>();
        bulletScript.SetBulletData(bulletData);
        bulletScript.SetTargetTrans(bulletTargetTrans);
    }

    Quaternion GetShotRotation()
    {
        if (isFocus)
        {
            return transform.rotation;
        }
        else
        {
            var dir = Vector3.Normalize(TargetTrans.position - transform.position);
            dir.z = 0;
            float angle = Vector3.SignedAngle(Vector3.up, dir,Vector3.forward);
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
