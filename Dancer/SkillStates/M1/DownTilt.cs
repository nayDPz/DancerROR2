using KinematicCharacterController;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates.M1
{

    public class DownTilt : BaseM1
    {
        public override void OnEnter()
        {
            anim = 2f;
            baseDuration = 0.4f;
            attackStartTime = 0f;
            attackEndTime = 0.4f;
            hitStopDuration = 0.025f;
            damageCoefficient = 2f;
            attackRecoil = 2f;
            hitHopVelocity = 2f;
            stackGainAmount = 7;
            hitStopDuration = 0.025f;
            bonusForce = Vector3.up * 1800f;
            pushForce = 1900f;
            launchVectorOverride = true;
            isFlinch = true;
            canMove = false;
            earlyExitJump = true;
            swingSoundString = "DownTilt";
            hitSoundString = "SwordHit";
            critHitSoundString = "SwordHit2";
            swingEffectPrefab = Modules.Assets.bigSwingEffect;
            hitEffectPrefab = Modules.Assets.hitEffect;
            impactSound = Modules.Assets.sword1HitSoundEvent.index;
            animString = "DownTilt";
            muzzleString = "eDownTilt";
            hitboxName = "DownTilt";
            hitHopVelocity = 11f;
            base.OnEnter();
        }

        public override void LaunchEnemy(CharacterBody body)
        {
            Vector3 vector = characterDirection.forward * 4f + Vector3.up * 10f;
            Vector3 normalized = (vector + transform.position - body.transform.position).normalized;
            normalized *= pushForce;
            if ((bool)body.GetComponent<KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterMotor>().ForceUnground();
            }
            CharacterMotor characterMotor = body.characterMotor;
            float num = 0.25f;
            if ((bool)characterMotor)
            {
                float num2 = Mathf.Max(150f, characterMotor.mass);
                num = num2 / 150f;
                normalized *= num;
                characterMotor.ApplyForce(normalized);
            }
            else if ((bool)body.rigidbody)
            {
                float num3 = Mathf.Max(50f, body.rigidbody.mass);
                num = num3 / 200f;
                normalized *= num;
                body.rigidbody.AddForce(normalized, ForceMode.Impulse);
            }
            DamageInfo damageInfo = new DamageInfo
            {
                attacker = gameObject,
                inflictor = gameObject,
                damage = 0f,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Generic,
                crit = false,
                dotIndex = DotController.DotIndex.None,
                force = normalized,
                position = transform.position,
                procChainMask = default,
                procCoefficient = 0f
            };
        }
    }
}