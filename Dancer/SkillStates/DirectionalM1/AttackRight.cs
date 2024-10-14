using UnityEngine;

namespace Dancer.SkillStates.DirectionalM1
{

    public class AttackRight : BaseDirectionalM1
    {
        public override void OnEnter()
        {
            anim = 1.1f;
            damageCoefficient = 2f;
            baseDuration = 0.55f;
            attackStartTime = 0.17f;
            attackEndTime = 0.26f;
            earlyExitTime = 0.8f;
            attackRecoil = 2f;
            hitHopVelocity = 2f;
            hitStopDuration = 0.06f;
            swingSoundString = "SwordSwing2";
            hitSoundString = "JabHit1";
            muzzleString = "eJab2";
            swingEffectPrefab = Modules.Assets.swingEffect;
            hitEffectPrefab = Modules.Assets.hitEffect;
            impactSound = Modules.Assets.jab1HitSoundEvent.index;
            isSus = true;
            isDash = true;
            dashSpeedCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.15f, 6f), new Keyframe(0.75f, 0f), new Keyframe(1f, 0f));
            animString = "AttackRight";
            hitboxName = "Jab";
            canRecieveInput = false;
            base.OnEnter();
        }

        public override void SetSlideVector()
        {
            if ((slideVector = inputBank.moveVector) == Vector3.zero)
            {
                Vector3 aimDirection = inputBank.aimDirection;
                slideVector = new Vector3(aimDirection.z, 0f, 0f - aimDirection.x);
            }
        }
    }
}
