using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField] private GameObject source;

    [SerializeField] private float radius;

    // Update is called once per frame
    void Update()
    {
        run();
    }

    private void run()
    {
        Vector3 dir = target.transform.position - source.transform.position;
        dir.z = 0;
        dir.Normalize();
        transform.position = source.transform.position + dir * radius;//确定位置
        //确定旋转
        Quaternion q = Quaternion.FromToRotation(Vector3.right, dir);
        transform.rotation = q;
    }
}
