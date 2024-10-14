using Dancer.Modules;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Dancer.SkillStates.InterruptStates
{

    internal class RibbonedState : BaseState
    {
        public float duration = 2f;

        public float pullDuration = 1f;

        public float timer;

        public Vector3 destination;

        public float deceleration = 0.5f;

        private GameObject ribbonVfxInstance;

        private float wait = 0.075f;

        public override void OnEnter()
        {
            base.OnEnter();
            Animator modelAnimator = GetModelAnimator();
            int layerIndex = modelAnimator.GetLayerIndex("Body");
            modelAnimator.CrossFadeInFixedTime(Random.Range(0, 2) == 0 ? "Hurt1" : "Hurt2", 0.1f);
            modelAnimator.Update(0f);
            timer = duration;
            float ribbonDebuffDuration = Buffs.ribbonDebuffDuration;
            foreach (CharacterBody.TimedBuff timedBuff in characterBody.timedBuffs)
            {
                if (timedBuff.buffIndex == Buffs.ribbonDebuff.buffIndex)
                {
                    ribbonDebuffDuration = timedBuff.timer;
                }
            }
            if (timer < ribbonDebuffDuration)
            {
                timer = ribbonDebuffDuration;
            }
            if (timer > Buffs.ribbonBossCCDuration && characterBody.isChampion)
            {
                timer = Buffs.ribbonBossCCDuration;
            }
            if ((bool)rigidbody && !rigidbody.isKinematic)
            {
                rigidbody.velocity = Vector3.zero;
                if ((bool)rigidbodyMotor)
                {
                    rigidbodyMotor.moveVector = Vector3.zero;
                }
            }
            EntityStateMachine[] components = gameObject.GetComponents<EntityStateMachine>();
            foreach (EntityStateMachine entityStateMachine in components)
            {
                if ((bool)entityStateMachine && entityStateMachine.customName.Equals("Weapon"))
                {
                    entityStateMachine.SetNextStateToMain();
                }
            }
        }

        public void SetNewTimer(float newDuration)
        {
            timer = newDuration;
            if (timer > Buffs.ribbonBossCCDuration && characterBody.isChampion)
            {
                timer = Buffs.ribbonBossCCDuration;
            }
            float num = 0f;
            foreach (CharacterBody.TimedBuff timedBuff in characterBody.timedBuffs)
            {
                if (timedBuff.buffIndex == Buffs.ribbonDebuff.buffIndex)
                {
                    num = timedBuff.timer;
                }
            }
            if (timer > num)
            {
                timer = num;
            }
        }

        public override void OnExit()
        {
            Animator modelAnimator = GetModelAnimator();
            if ((bool)modelAnimator)
            {
                modelAnimator.enabled = true;
            }
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer -= Time.fixedDeltaTime;
            Animator modelAnimator = GetModelAnimator();
            if ((bool)modelAnimator && fixedAge > wait)
            {
                modelAnimator.enabled = false;
            }
            if ((bool)characterMotor)
            {
                float magnitude = characterMotor.velocity.magnitude;
                magnitude -= deceleration * Time.fixedDeltaTime;
                characterMotor.velocity = characterMotor.velocity.normalized * magnitude;
                if (characterMotor.velocity.x == 0f && characterMotor.velocity.z == 0f)
                {
                    characterMotor.velocity.y = 0f;
                }
            }
            else if ((bool)rigidbody)
            {
                float magnitude2 = rigidbody.velocity.magnitude;
                magnitude2 -= deceleration * Time.fixedDeltaTime;
                rigidbody.velocity = rigidbody.velocity.normalized * magnitude2;
                if (rigidbody.velocity.x == 0f && rigidbody.velocity.z == 0f)
                {
                    rigidbody.velocity = Vector3.zero;
                }
            }
            if (timer <= 0f || NetworkServer.active && !characterBody.HasBuff(Buffs.ribbonDebuff))
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}