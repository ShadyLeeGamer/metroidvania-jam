using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{
    public int id;

    int damage;

    Rigidbody2D rb;

    bool isPlayer;

    ObjectPooler objectPooler;

    void Awake()
    {
        objectPooler = ObjectPooler.Instance;
    }

    public void Initialise(ObjectData data)
    {
        damage = data.damage;
        isPlayer = data.isPlayerProjectile;

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.right * data.speed * data.faceDir);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement otherCharacter = other.GetComponent<PlayerMovement>();
        if (otherCharacter)
        {
            if ((!isPlayer && other.GetComponent<Enemy>()) ||
                isPlayer && !other.GetComponent<Enemy>())
                return;

            otherCharacter.TakeDamage(damage);
            EndProjectile();
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Ground"))
            EndProjectile();
    }

    public void EndProjectile()
    {
        rb.velocity = Vector3.zero;
        objectPooler.RecycleProjectile(this);
    }
}