using RoR2;
using UnityEngine;

public class DancerComponent : MonoBehaviour
{
    private Animator animator;

    private ModelLocator modelLocator;

    private Transform modelTransform;

    public Transform weaponBase;

    public Transform root;

    private Vector3 weaponPointOverride;

    private Vector3 bodyDirectionOverride;

    public Vector3 vector = new Vector3(90f, 0f, 0f);

    private void Start()
    {
        modelLocator = GetComponent<ModelLocator>();
        modelTransform = modelLocator.modelTransform;
        GetTransforms();
    }

    private void GetTransforms()
    {
        if ((bool)modelTransform)
        {
            weaponBase = modelTransform.Find("DancerArmature/ROOT/HipsControl/Hips/Spine/Chest/Shoulder.L/UpperArm.L/LowerArm.L/Hand.L/Lance");
            root = modelTransform.Find("DancerArmature/ROOT");
        }
    }

    public void BodyRotationOverride(Vector3 direction)
    {
        bodyDirectionOverride = direction;
    }

    public void StopBodyOverride()
    {
        bodyDirectionOverride = Vector3.zero;
    }

    public void WeaponRotationOverride(Vector3 pointToHit)
    {
        weaponPointOverride = pointToHit;
    }

    public void StopWeaponOverride()
    {
        weaponPointOverride = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (weaponPointOverride != Vector3.zero)
        {
            Vector3 forward = weaponPointOverride - weaponBase.position;
            Quaternion rotation = Util.QuaternionSafeLookRotation(forward) * Quaternion.Euler(vector);
            weaponBase.rotation = rotation;
        }
        if (bodyDirectionOverride != Vector3.zero)
        {
            Quaternion rotation2 = Util.QuaternionSafeLookRotation(bodyDirectionOverride) * Quaternion.Euler(vector);
            root.rotation = rotation2;
        }
    }
}
