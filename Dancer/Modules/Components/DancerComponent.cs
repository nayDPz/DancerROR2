using UnityEngine;
using System.Collections;
using RoR2;
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

    void Start()
    {
        this.modelLocator = base.GetComponent<ModelLocator>();
        this.modelTransform = this.modelLocator.modelTransform;
        this.GetTransforms();
    }


    private void GetTransforms()
    {
        if (this.modelTransform)
        {
            this.weaponBase = modelTransform.Find("DancerArmature/ROOT/HipsControl/Hips/Spine/Chest/Shoulder.L/UpperArm.L/LowerArm.L/Hand.L/Lance 1");

            this.root = modelTransform.Find("DancerArmature/ROOT");

        }
    }


    public void BodyRotationOverride(Vector3 direction)
    {
        this.bodyDirectionOverride = direction;
    }

    public void StopBodyOverride()
    {
        this.bodyDirectionOverride = Vector3.zero;
    }

    public void WeaponRotationOverride(Vector3 pointToHit)
    {
        this.weaponPointOverride = pointToHit;
    }

    public void StopWeaponOverride()
    {
        this.weaponPointOverride = Vector3.zero;
    }

    void LateUpdate()
    {
        if(this.weaponPointOverride != Vector3.zero)
        {
            Vector3 weaponDirection = this.weaponPointOverride - this.weaponBase.position;
            Quaternion rotation = Util.QuaternionSafeLookRotation(weaponDirection) * Quaternion.Euler(vector);
            this.weaponBase.rotation = rotation;
        }

        if (this.bodyDirectionOverride != Vector3.zero)
        {
            Quaternion rotation = Util.QuaternionSafeLookRotation(this.bodyDirectionOverride) * Quaternion.Euler(vector);
            this.root.rotation = rotation;
        }

    }

}