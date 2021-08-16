using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float stoppingRange;

    Vector3 targetPos;
    Transform player;
    [SerializeField] Transform armPivot;
    [SerializeField] float armPivotRotationOffsetZ;
    [SerializeField] float armPivotRotationSpeed;

    [SerializeField] float lookRange;
    [SerializeField] Vector3 lookRaycastPosOffset;
    [SerializeField] LayerMask playerLayerMask;

    [SerializeField] bool drawGizmos;

    Vector3 LookRayPos => transform.position + lookRaycastPosOffset;
    Vector3 DiffWithTargetPos => targetPos - transform.position;

    bool TargetInRightDir => targetPos.x > transform.position.x;

    Movement movement;

    void Awake()
    {
        movement = GetComponent<Movement>();
    }

    void FixedUpdate()
    {
        CorrectFaceDir();

        float faceDir = TargetInRightDir ? 1 : -1;
        RaycastHit2D hit = Physics2D.Raycast(LookRayPos, armPivot.right, lookRange, playerLayerMask);
        player = hit ? hit.transform : null;
        targetPos = hit ? player.position : transform.position + (Vector3.right * faceDir);

        if (Mathf.Abs(DiffWithTargetPos.x) > stoppingRange)
            movement.SmoothMove(faceDir);
    }

    void CorrectFaceDir()
    {
        // FACE ENEMY IN RIGHT DIR
        if (TargetInRightDir)
        {
            if (transform.eulerAngles.y == 180)
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (transform.eulerAngles.y == 0f)
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        // FACE ARM IN RIGHT DIR
        Vector3 diff = DiffWithTargetPos;
        diff.Normalize(); // BETWEEN 0 AND 1
        float armPivotRotationZ = Mathf.Atan2(diff.y + armPivotRotationOffsetZ, diff.x) * Mathf.Rad2Deg;

        // KEEP ARM RIGHTSIDE UP
        Quaternion rotationDelta =
            Quaternion.Euler(TargetInRightDir ? 0f : 180f, 0f,
                             TargetInRightDir ? armPivotRotationZ : -armPivotRotationZ);
        // SMOOTHLY ROTATE ARM
        armPivot.rotation =
            Quaternion.Slerp(transform.rotation, rotationDelta,
                             armPivotRotationSpeed * Time.fixedDeltaTime);
    }

    void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            if (player)
                Gizmos.color = Color.green;
            else if (!player)
                Gizmos.color = Color.red;
            Gizmos.DrawRay(LookRayPos, armPivot.right * lookRange);
        }
    }
}