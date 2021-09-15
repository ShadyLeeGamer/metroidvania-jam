using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int health;

    public virtual void Start()
    {
        Heal();
    }
    public void Heal() {
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