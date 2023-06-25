using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : MonoBehaviour
{
    public static void Create(TowerTypeSO towerType, Vector3 spawnPosition, Vector3 targetPosition)
    {
        Transform arrowTransform = Instantiate(towerType.projectile, spawnPosition, Quaternion.Euler(90, 0, 0));

        ProjectileArrow projectileArrow = arrowTransform.GetComponent<ProjectileArrow>();
        projectileArrow.Setup(targetPosition);
    }

    private Vector3 targetPosition;
    private float moveSpeed = 10f;
    //private float turnSpeed = 100f;
    private float destroySelfDistance = 1f;
    private void Setup(Vector3 targetPosition) 
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        transform.LookAt(targetPosition);
        transform.eulerAngles += new Vector3(90, 0, 0);
        //var angle = Vector3.Angle(transform.position, moveDir);

        //transform.eulerAngles = new Vector3(angle, angle, angle);
        /*float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        transform.eulerAngles = new Vector3(90, angle, angle);*/

        if (Vector3.Distance(transform.position, targetPosition) < destroySelfDistance)
        {
            Destroy(gameObject);
        }
    }
}
