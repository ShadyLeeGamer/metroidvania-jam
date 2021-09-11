using UnityEngine;

public class Enemy : Movement
{
    [Header("Follow")]
    [SerializeField] float stoppingRange;
    [SerializeField] float updateRate;
    Vector3 targetPos;
    Transform player;

    [Header("Arm Pivot")]
    [SerializeField] Transform armPivot;
    [SerializeField] float armPivotRotationOffsetZ;
    [SerializeField] float armPivotRotationSpeed;

    [Header("Look Raycast")]
    [SerializeField] float lookRange;
    [SerializeField] Vector3 lookRaycastPosOffset;
    [SerializeField] LayerMask playerLayerMask;

    [Header("Shooting")]
    [SerializeField] GunSettings gun;
    [SerializeField] Transform shootPoint;
    float shootCountdown;

    [Header("Testing")]
    [SerializeField] bool drawGizmos;

    Vector2 LookRayPos => transform.position
                        + new Vector3(lookRaycastPosOffset.x * (transform.rotation.y == 0 ? 1 : -1),
                                      lookRaycastPosOffset.y);
    Vector3 DiffWithTargetPos => targetPos - transform.position;

    bool TargetInRightDir => targetPos.x > transform.position.x;

    Movement movement;

    ObjectPooler objectPooler;

    void Awake()
    {
        movement = GetComponent<Movement>();
    }

    public override void Start()
    {
        base.Start();
        objectPooler = ObjectPooler.Instance;
    }

    void FixedUpdate()
    {
        CorrectFaceDir();

        int faceDir = TargetInRightDir ? 1 : -1;
        RaycastHit2D hit = Physics2D.Raycast(LookRayPos, armPivot.right, lookRange, playerLayerMask);
        player = hit ? hit.transform : null;
        targetPos = hit ? player.position : transform.position + (Vector3.right * faceDir);

        Debug.Log(faceDir + " " + DiffWithTargetPos);
        if (Mathf.Abs(DiffWithTargetPos.x) > stoppingRange)
            SmoothMove(faceDir);

        if (player && Mathf.Abs(DiffWithTargetPos.x) <= gun.shootRange)
        {
            shootCountdown -= gun.fireRate * Time.deltaTime;
            if (shootCountdown <= 0)
            {
                Vector3 projRot = gun.AddRecoil(armPivot.localEulerAngles);
                Quaternion projRotWithDir = Quaternion.Euler(projRot.x, projRot.y, projRot.z * faceDir);
                gun.Shoot(objectPooler, shootPoint.position, projRotWithDir, faceDir);
                shootCountdown = 1;
            }
        }
        else if (shootCountdown != 1)
            shootCountdown = 1;
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

    Vector3 LocalisePos(Vector3 pos)
    {
        return transform.position + pos;
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