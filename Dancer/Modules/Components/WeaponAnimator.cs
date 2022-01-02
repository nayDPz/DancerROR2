using UnityEngine;
using System.Collections;
using RoR2;
public class WeaponAnimator : MonoBehaviour
{
    float leftFootPositionWeight;
    float leftFootRotationWeight;
    Transform leftFootObj;


    private Animator animator;
    private ModelLocator modelLocator;
    private Transform modelTransform;
    private Transform weaponBase;
    private Transform weaponExtender;

    private bool overrideRotation;
    private Vector3 weaponPointOverride;

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
            if(this.weaponBase)
                Debug.Log("found dancer weapon");
            this.weaponExtender = modelTransform.Find("DancerArmature/ROOT/HipsControl/Hips/Spine/Chest/Shoulder.L/UpperArm.L/LowerArm.L/Hand.L/Lance 1/Lance.001/Lance.002/Lance.003/Lance.004/Lance.005/Lance.006/Lance.007/Lance.008/LanceExtension");
            if (this.weaponExtender)
                Debug.Log("found dancer weapon extender");
        }
    }

    public void RotationOverride(Vector3 pointToHit)
    {
        this.overrideRotation = true;
        this.weaponPointOverride = pointToHit;
    }

    public void StopRotationOverride()
    {
        this.overrideRotation = false;
        this.weaponPointOverride = Vector3.zero;
    }

    void LateUpdate()
    {
        if(this.overrideRotation)
        {
            Vector3 weaponDirection = this.weaponPointOverride - this.weaponBase.position;
            Quaternion rotation = Util.QuaternionSafeLookRotation(weaponDirection) * Quaternion.Euler(vector);
            this.weaponBase.rotation = rotation;
        }
    }

}