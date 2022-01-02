using System;
using RoR2.Audio;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.Networking;
using RoR2;
namespace Dancer.Modules.Components
{
	[RequireComponent(typeof(ProjectileController))]
	public class DancerFireballBouncer : ProjectileExplosion, IProjectileImpactBehavior
	{
		private Vector3 previousVelocity;
		private Rigidbody rigidbody;
		public override void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			base.Awake();
		}

		protected void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (NetworkServer.active || this.projectileController.isPrediction)
			{
				if (this.stopwatch >= this.lifetime || this.projectileHealthComponent && !this.projectileHealthComponent.alive)
				{
					this.alive = false;
				}
				if (!this.alive)
				{
					base.Detonate();
				}
			}
			this.previousVelocity = this.rigidbody.velocity;
		}


		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
			this.impactNormal = impactInfo.estimatedImpactNormal;
			if (collider)
			{
				DamageInfo damageInfo = new DamageInfo();
				if (this.projectileDamage)
				{
					damageInfo.damage = this.projectileDamage.damage;
					damageInfo.crit = this.projectileDamage.crit;
					damageInfo.attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null);
					damageInfo.inflictor = base.gameObject;
					damageInfo.position = impactInfo.estimatedPointOfImpact;
					damageInfo.force = this.projectileDamage.force * base.transform.forward;
					damageInfo.procChainMask = this.projectileController.procChainMask;
					damageInfo.procCoefficient = this.projectileController.procCoefficient;
				}
				else
				{
					Debug.Log("No projectile damage component!");
				}
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					if (this.destroyOnEnemy)
					{
						HealthComponent healthComponent = component.healthComponent;
						if (healthComponent)
						{
							if (healthComponent.gameObject == this.projectileController.owner)
							{
								return;
							}
							if (this.projectileHealthComponent && healthComponent == this.projectileHealthComponent)
							{
								return;
							}
							this.alive = false;
						}
					}
				}
				else if (this.timesBounced < this.numBounces)
				{
					this.timesBounced++;
					Rigidbody r = base.GetComponent<Rigidbody>();
					if(r)
                    {
						Vector3 velocity = r.velocity;
						velocity.y = -Physics.gravity.y * 1.25f;
						r.velocity = velocity;
						//r.velocity = Vector3.Reflect(r.velocity, this.impactNormal);
                    }
				}
				else
                {
					this.alive = false;
                }
				this.hasImpact = true;
				if (NetworkServer.active)
				{
					GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
				}
			}
		}

		public override void OnValidate()
		{
			if (Application.IsPlaying(this))
			{
				return;
			}
			base.OnValidate();
			if (!string.IsNullOrEmpty(this.lifetimeExpiredSoundString))
			{
				Debug.LogWarningFormat(base.gameObject, "{0} ProjectileImpactExplosion component supplies a value in the lifetimeExpiredSoundString field. This will not play correctly over the network. Please use lifetimeExpiredSound instead.", new object[]
				{
					Util.GetGameObjectHierarchyName(base.gameObject)
				});
			}
		}

		private Vector3 impactNormal = Vector3.up;

		public GameObject impactEffect;

		public string lifetimeExpiredSoundString;

		public bool destroyOnEnemy = true;

		private int timesBounced;
		public int numBounces = 1;

		public float lifetime;


		private float stopwatch;

		private bool hasImpact;


		public ProjectileImpactExplosion.TransformSpace transformSpace;


		public enum TransformSpace
		{
			World,
			Local,
			Normal
		}
	}
}
