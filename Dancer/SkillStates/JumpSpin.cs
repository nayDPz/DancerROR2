using RoR2;
using UnityEngine;
using Dancer.Modules.Components;
using EntityStates;
using System;
using RoR2.Audio;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Dancer.SkillStates
{
    public class JumpSpin : BaseSkillState
    {

        private float jumpSpeedCoefficient = 1.5f;
        private List<HealthComponent> hits;

        protected string hitboxName = "NAir";
        protected DamageType damageType = DamageType.Generic;
        public static float damageCoefficient = 3f;
        protected float procCoefficient = 1f;
        protected float pushForce = 2600f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseDuration = 0.45f;
        protected float attackStartTime = 0.0f;
        protected float attackEndTime = 0.9f;
        protected float baseEarlyExitTime = 0.2f;
        protected float hitStopDuration = 0.025f;
        protected float attackRecoil = 0f;
        protected float hitHopVelocity = 1f;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound;


        private float speed;
        private float startSpeed;
        private float endSpeed;
        private Vector3 direction;

        public float duration;
        private bool hasFired;
        private float hitPauseTimer;
        protected OverlapAttack attack;
        protected bool inHitPause;
        private bool hasHopped;
        protected float stopwatch;
        protected Animator animator;
        private BaseState.HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.animator.SetBool("attacking", true);

            base.characterBody.SetAimTimer(2f);
            this.duration = this.baseDuration / this.attackSpeedStat;

            this.hits = new List<HealthComponent>();

            this.direction = base.GetAimRay().direction + Vector3.up * 5f;
            this.direction = this.direction.normalized;

            this.speed = this.moveSpeedStat * this.jumpSpeedCoefficient;
            this.startSpeed = this.speed * 2.2f;
            this.endSpeed = this.speed * 0.75f;

            this.hitSoundString = EntityStates.Merc.Uppercut.hitSoundString;
            this.hitEffectPrefab = EntityStates.Merc.GroundLight.finisherHitEffectPrefab;

            this.impactSound = Modules.Assets.jab1HitSoundEvent.index;

            Util.PlayAttackSpeedSound("Jab1", base.gameObject, base.attackSpeedStat);

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
            }

            this.attack = new OverlapAttack();
            this.attack.damageType = this.damageType;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = JumpSpin.damageCoefficient * this.damageStat;
            this.attack.procCoefficient = this.procCoefficient;
            this.attack.hitEffectPrefab = this.hitEffectPrefab;
            this.attack.forceVector = this.bonusForce;
            this.attack.pushAwayForce = this.pushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;


            PlayAttackAnimation();           
        }

        private void PlayAttackAnimation()
        {
            base.PlayCrossfade("Gesture, Override", "JumpSpin", "Slash.playbackRate", this.duration, 0.05f);
        }


        private void OnHitEnemyAuthority()
        {
            Util.PlaySound("JabHit1", base.gameObject);
            
            if (base.fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (!this.inHitPause && this.hitStopDuration > 0f)
            {
                this.storedVelocity = base.characterMotor.velocity;
                this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Slash.playbackRate");
                this.hitPauseTimer = this.hitStopDuration / this.attackSpeedStat;
                this.inHitPause = true;
            }
        }

        private void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(this.swingEffectPrefab, base.gameObject, this.muzzleString, true);
        }

        private void FireAttack()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);

                if (base.isAuthority)
                {
                    this.PlaySwingEffect();
                    base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                }
            }

            if (base.isAuthority)
            {
                if (this.attack.Fire())
                {
                    this.OnHitEnemyAuthority();
                }
            }
            if (NetworkServer.active)
            {
                Transform t = base.FindModelChild("DAirGround");
                Vector3 position = t.position;
                Vector3 vector = t.localScale * 0.5f;
                Quaternion rot = t.rotation;
                Collider[] hits = Physics.OverlapBox(position, vector, rot, LayerIndex.entityPrecise.mask);
                for (int i = 0; i < hits.Length; i++)
                {
                    HurtBox hurtBox = hits[i].GetComponent<HurtBox>();
                    if (hurtBox)
                    {
                        HealthComponent healthComponent = hurtBox.healthComponent;
                        if (healthComponent)
                        {
                            TeamComponent team = healthComponent.GetComponent<TeamComponent>();

                            bool enemy = team.teamIndex != base.teamComponent.teamIndex;
                            if (enemy)
                            {

                                if (!this.hits.Contains(healthComponent))
                                {
                                    this.hits.Add(healthComponent);
                                    HealthComponent h = healthComponent;
                                    if (h)
                                    {
                                        if (h.body && h.body.characterMotor)
                                        {
                                            h.body.characterMotor.velocity = Vector3.zero;
                                        }
                                        if (!h.body.isChampion || (h.gameObject.name.Contains("Brother") && h.gameObject.name.Contains("Body")))
                                        {
                                            this.LaunchEnemy(h.body);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

            }
        }

        public void LaunchEnemy(CharacterBody body)
        {
            Vector3 launchVector = Vector3.up;
            launchVector *= this.pushForce;

            if (body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>())
            {
                body.GetComponent<KinematicCharacterController.KinematicCharacterMotor>().ForceUnground();
            }

            CharacterMotor m = body.characterMotor;

            float force = 0.25f;
            if (m)
            {
                float f = Mathf.Max(100f, m.mass);
                force = f / 100f;
                launchVector *= force;
                m.ApplyForce(launchVector);
            }
            else if (body.rigidbody)
            {
                float f = Mathf.Max(50f, body.rigidbody.mass);
                force = f / 200f;
                launchVector *= force;
                body.rigidbody.AddForce(launchVector, ForceMode.Impulse);
            }

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.hitPauseTimer -= Time.fixedDeltaTime;

            if (this.hitPauseTimer <= 0f && this.inHitPause)
            {
                base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                this.inHitPause = false;
                base.characterMotor.velocity = this.storedVelocity;
            }

            if (!this.inHitPause)
            {
                this.stopwatch += Time.fixedDeltaTime;

                this.speed = Mathf.Lerp(this.startSpeed, this.endSpeed, base.fixedAge / this.duration);
                base.characterMotor.velocity = this.direction * this.speed;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                if (this.animator) this.animator.SetFloat("Slash.playbackRate", 0f);
            }

            if (this.stopwatch >= (this.duration * this.attackStartTime) && this.stopwatch <= (this.duration * this.attackEndTime))
            {
                this.FireAttack();
            }
            if (this.stopwatch >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.velocity = this.direction * this.speed;
            this.animator.SetFloat("Slash.playbackRate", 1f);
            base.OnExit();
        }
    }
}