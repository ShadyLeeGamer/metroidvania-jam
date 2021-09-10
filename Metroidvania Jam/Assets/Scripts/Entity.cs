using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] int maxHealth;
    int health;

    public virtual void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}