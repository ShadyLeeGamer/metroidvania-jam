using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{
    public int id;

    int damage;

    Rigidbody2D rb;

    bool isPlayerProjectile;

    ObjectPooler objectPooler;

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    public void Initialise(ObjectData projectileData)
    {
        damage = projectileData.damage;
        isPlayerProjectile = projectileData.isPlayerProjectile;

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.position + (Vector3.right * projectileData.speed * projectileData.faceDir));
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerMovement otherCharacter = other.GetComponent<PlayerMovement>();
        if (otherCharacter)
        {
            if ((!isPlayerProjectile && other.GetComponent<Enemy>()) ||
                isPlayerProjectile && !other.GetComponent<Enemy>())
                return;

            //otherCharacter.TakeDamage(damage);
            EndProjectile();
        }
    }

    public void EndProjectile()
    {
        rb.velocity = Vector3.zero;
        objectPooler.RecycleProjectile(this);
    }
}