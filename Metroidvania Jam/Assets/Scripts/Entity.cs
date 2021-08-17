using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] int maxHealth;
    int health;

    void Start()
    {
        maxHealth = health;
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