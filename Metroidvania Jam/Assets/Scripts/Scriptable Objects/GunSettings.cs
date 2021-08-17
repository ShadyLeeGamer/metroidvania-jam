using UnityEngine;

[CreateAssetMenu(fileName = "Gun Settings")]
public class GunSettings : ScriptableObject
{
    public float shootRange;
    public float fireRate;
    public float recoil;

    public Projectile projectilePrefab;
    public float projSpeed;
    public int projDamage;

    public Vector3 AddRecoil(Vector3 rot)
    {
        return rot + Vector3.forward * Random.Range(-recoil, recoil);
    }

    public void Shoot(ObjectPooler objectPooler, Vector3 shootPos, Quaternion projectileRot, int faceDir)
    {
        ObjectData projectileData = new ObjectData(projDamage, projSpeed, faceDir, false);
        objectPooler.GetProjectile(projectilePrefab.id, shootPos, projectileRot, projectileData);
    }
}