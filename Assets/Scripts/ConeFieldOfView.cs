using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeFieldOfView : MonoBehaviour
{
    [SerializeField] public float visionRadius;
    [Range(0,360)]
    [SerializeField] public float visionAngle;


    public LayerMask targetMask;
    public LayerMask nonTargetMask;

    [HideInInspector]
    public List<Transform> visablesTargets = new List<Transform>();

    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisableTargets();
        }
    }

    public bool FindVisableTargets()
    {
        visablesTargets.Clear();
        Collider[] targetsInView = Physics.OverlapSphere(transform.position, visionRadius, targetMask);
        
        for(int i = 0; i < targetsInView.Length; i++)
        {
            Transform target = targetsInView[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, nonTargetMask))
                {
                    visablesTargets.Add(target);
                    return true;
                }
            }
        }
        return false;
    }


    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
