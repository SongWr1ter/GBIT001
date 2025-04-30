using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heshiying_Bullet : Bullet
{
    private Transform targetTrans;
    protected override void Update()
    {
        Vector3 oriPos = transform.position; //记录原来的位置
        if (Input.GetKey(KeyCode.Tab))
        {
            if (targetTrans != null)
            {
                MoveTowards(targetTrans.position);
            }
            else
            {
                transform.Translate(transform.up * (speed * time.deltaTime), Space.World);
            }
        }
        else
        {
            transform.Translate(transform.up * (speed * time.deltaTime), Space.World);
        }

        float length = (transform.position - oriPos).magnitude; //射线的长度
        Vector3 direction = transform.position - oriPos; //方向
        float deltaLength = length / iteration;
        RaycastHit2D hitinfo;
        for (int i = 0; i < iteration; i++)
        {

            oriPos = oriPos + direction.normalized * (deltaLength * i);
            hitinfo = Physics2D.Raycast(oriPos, direction, deltaLength, layer); //在两个位置之间发起一条射线，然后通过这条射线去检测有没有发生碰撞
            //check
            if (hitinfo)
            {
                Collider2D other = hitinfo.collider;
                if (other.CompareTag("Breakable")) //子弹会穿透
                {
                    if (other.TryGetComponent(out Breakable breakableObject))
                    {
                        GameObject g = InstantiateVFX(_vfxName, hitinfo.point, Quaternion.identity);
                        ParticleSystem.MainModule m = g.GetComponent<ParticleSystem>().main;
                        m.stopAction = ParticleSystemStopAction.Callback;
                        breakableObject.GetHurt(oriPos, hitinfo.point);
                        //Destroy(gameObject);
                    }

                    return;
                }

                if (other.CompareTag("Obstacle")) //子弹不会穿透 阻挡视野
                {
                    GameObject g = InstantiateVFX(_vfxName, hitinfo.point, Quaternion.identity);
                    ParticleSystem.MainModule m = g.GetComponent<ParticleSystem>().main;
                    m.stopAction = ParticleSystemStopAction.Callback;
                    OnHitObstacle(hitinfo.point);
                    Destroy(gameObject);
                    return;
                }

                if (other.CompareTag("Shield") && !canDamagePlayer) //子弹不会穿透 不阻挡视野
                {
                    GameObject g = InstantiateVFX(_vfxName, hitinfo.point, Quaternion.identity);
                    ParticleSystem.MainModule m = g.GetComponent<ParticleSystem>().main;
                    m.stopAction = ParticleSystemStopAction.Callback;
                    if (other.TryGetComponent(out Breakable breakableObject))
                    {
                        breakableObject.GetHurt(oriPos, hitinfo.point);
                    }

                    Destroy(gameObject);
                    return;
                }

                if (other.CompareTag("Enemy") && !canDamagePlayer) //子弹不会穿透 不阻挡视野
                {
                    other.GetComponent<simpleFSM>().GetDamaged(transform.up, _damage);
                    Destroy(gameObject);
                    return;
                }

                if (other.CompareTag("Player") && canDamagePlayer) //子弹不会穿透
                {
                    other.GetComponent<PlayerControl>().GetDamaged(null);
                    Destroy(gameObject);
                    return;
                }
            }

        }

        deadtime += time.deltaTime;
        if (deadtime >= deadTime)
        {
            Destroy(gameObject);
            GameObject g = InstantiateVFX(_vfxName, transform.position, Quaternion.identity);
            deadtime -= deadTime;
        }
    }

    public void SetTargetTrans(Transform targetTrans)
    {
        this.targetTrans = targetTrans;
    }
    //向目标点旋转并移动
    private void MoveTowards(Vector3 targetPos)
    {
        transform.Translate(transform.up * (speed/7 * time.deltaTime), Space.World);
        Quaternion currentRotation = transform.rotation;
        Vector3 dir = targetPos - transform.position;
        dir.z = 0;
        dir.Normalize();
        float angle = Vector3.SignedAngle(transform.up, dir, Vector3.forward);
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle * time.deltaTime * 5);
        
        transform.rotation = targetRotation * currentRotation;
    }
    //原地旋转
    private void MoveTowards3(Vector3 targetPos)
    {
        transform.Translate(transform.up * (speed/7 * time.deltaTime), Space.World);
        Quaternion currentRotation = transform.rotation;
        Vector3 dir = targetPos - transform.position;
        dir.z = 0;
        dir.Normalize();
        float angle = Vector3.SignedAngle(transform.up, dir, Vector3.forward);
        Quaternion targetRotation = Quaternion.Euler(0, 0, time.deltaTime * 90f);
        
        transform.rotation = targetRotation * currentRotation;
    }
    //移动到目标绝对上方开始绕y轴旋转
    private void MoveTowards1(Vector3 targetPos)
    {
        transform.Translate(transform.up * (speed/7 * time.deltaTime), Space.World);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetPos - transform.position), speed/2 * time.deltaTime);
    }
    //旋转到y朝向正上方
    private void MoveTowards2(Vector3 targetPos)
    {
        transform.Translate(transform.up * (speed/7 * time.deltaTime), Space.World);
        Vector3 dir = targetPos - transform.position;
        dir.z = 0;
        dir.Normalize();
        Quaternion targetRot = Quaternion.Euler(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * speed * 10f);
    }
}
