using UnityEngine;

// interface - SPECIFIES TYPES AND FUNCTIONS THAT ALL OBJECTS THAT DERIVES FROM THE INTERFACE HAS TO IMPLEMENT
public interface IPooledObject
{
    void Initialise(ObjectData objectData);
}

public struct ObjectData
{
    // ENEMY DATA
    public int maxHealth;

    // PROJECTILE DATA
    public int damage;
    public float speed;
    public int faceDir;
    public bool isPlayerProjectile;

    // COMBAT TEXT DATA
    public Color colour;
    public string text;

    public ObjectData(int maxHealth)
    {
        this.maxHealth = maxHealth;

        damage = default;
        speed = default;
        faceDir = default;
        isPlayerProjectile = default;

        colour = default;
        text = default;
    }

    public ObjectData(int damage, float speed, int faceDir, bool isDraculaProjectile)
    {
        maxHealth = default;

        this.damage = damage;
        this.speed = speed;
        this.faceDir = faceDir;
        this.isPlayerProjectile = isDraculaProjectile;

        colour = default;
        text = default;
    }

    public ObjectData(Color colour, string text)
    {
        maxHealth = default;

        damage = default;
        speed = default;
        faceDir = default;
        isPlayerProjectile = default;

        this.colour = colour;
        this.text = text;
    }
}