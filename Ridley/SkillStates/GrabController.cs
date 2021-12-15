using System;
using RoR2;
using UnityEngine;

namespace Ridley.SkillStates
{
	// Token: 0x02000006 RID: 6
	public class GrabController : MonoBehaviour
	{
		// Token: 0x0600000A RID: 10 RVA: 0x0000236C File Offset: 0x0000056C
		private void Awake()
		{
			this.body = base.GetComponent<CharacterBody>();
			this.motor = base.GetComponent<CharacterMotor>();
			this.direction = base.GetComponent<CharacterDirection>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			bool flag = this.modelLocator;
			if (flag)
			{
				Transform transform = base.transform.Find("Model Base/mdlGreaterWisp/GreaterWispArmature/HurtBox");
				bool flag2 = transform;
				if (flag2)
				{
					this.extraLayer = transform.gameObject.layer;
					transform.gameObject.layer = LayerIndex.noCollision.intVal;
				}
				transform = base.transform.Find("Model Base/mdlGreaterWisp/GreaterWispArmature/ROOT/Mask/StandableSurfacePosition/StandableSurface");
				bool flag3 = transform;
				if (flag3)
				{
					this.extraLayer2 = transform.gameObject.layer;
					transform.gameObject.layer = LayerIndex.noCollision.intVal;
				}
			}
			base.gameObject.layer = LayerIndex.noCollision.intVal;
			bool flag4 = this.direction;
			bool flag5 = flag4;
			if (flag5)
			{
				this.direction.enabled = false;
			}
			bool flag6 = this.modelLocator;
			bool flag7 = flag6;
			if (flag7)
			{
				bool flag8 = !this.modelLocator.enabled;
				if (flag8)
				{
					this.modelLocatorStartedDisabled = true;
				}
				bool flag9 = this.modelLocator.modelTransform;
				bool flag10 = flag9;
				if (flag10)
				{
					this.modelTransform = this.modelLocator.modelTransform;
					this.originalRotation = this.modelTransform.rotation;
					this.modelLocator.enabled = false;
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002500 File Offset: 0x00000700
		private void FixedUpdate()
		{
			bool flag = this.motor;
			bool flag2 = flag;
			if (flag2)
			{
				this.motor.disableAirControlUntilCollision = true;
				this.motor.velocity = Vector3.zero;
				this.motor.rootMotion = Vector3.zero;
				this.motor.Motor.SetPosition(this.pivotTransform.position, true);
			}
			bool flag3 = this.pivotTransform;
			bool flag4 = flag3;
			if (flag4)
			{
				base.transform.position = this.pivotTransform.position;
			}
			bool flag5 = this.modelTransform;
			bool flag6 = flag5;
			if (flag6)
			{
				this.modelTransform.position = this.pivotTransform.position;
				this.modelTransform.rotation = this.pivotTransform.rotation;
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(base.transform.position + Vector3.up * 2f, Vector3.down), out raycastHit, 6f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
            {
				this.lastGroundPosition = raycastHit.point;
            }
		}

		private Vector3 lastGroundPosition;
		// Token: 0x0600000C RID: 12 RVA: 0x000025DC File Offset: 0x000007DC
		public void Launch(Vector3 launchVector)
		{
			bool flag = this.modelLocator;
			bool flag2 = flag && !this.modelLocatorStartedDisabled;
			if (flag2)
			{
				this.modelLocator.enabled = true;
			}
			bool flag3 = this.modelTransform;
			bool flag4 = flag3;
			if (flag4)
			{
				this.modelTransform.rotation = this.originalRotation;
			}
			bool flag5 = this.direction;
			bool flag6 = flag5;
			if (flag6)
			{
				this.direction.enabled = true;
			}
			if(this.body.healthComponent && this.body.healthComponent.alive)
            {
				bool flag7 = this.modelLocator;
				if (flag7)
				{
					Transform transform = base.transform.Find("Model Base/mdlGreaterWisp/GreaterWispArmature/HurtBox");
					bool flag8 = transform;
					if (flag8)
					{
						transform.gameObject.layer = this.extraLayer;
					}
					transform = base.transform.Find("Model Base/mdlGreaterWisp/GreaterWispArmature/ROOT/Mask/StandableSurfacePosition/StandableSurface");
					bool flag9 = transform;
					if (flag9)
					{
						transform.gameObject.layer = this.extraLayer2;
					}
				}
				base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			}
			RaycastHit raycastHit;
			if (!Physics.Raycast(new Ray(this.body.footPosition, Vector3.down), out raycastHit, 15f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
            {
				base.transform.position = this.lastGroundPosition;
            }
			bool flag10 = this.motor;
			if (flag10)
			{
				this.motor.ApplyForce(launchVector, false, false);
			}
			else
			{
				DamageInfo damageInfo = new DamageInfo
				{
					position = this.body.transform.position,
					attacker = null,
					inflictor = null,
					damage = 0f,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					crit = false,
					force = launchVector,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0f
				};
				this.body.healthComponent.TakeDamageForce(damageInfo, false, false);
			}
			GameObject.Destroy(this);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000027A0 File Offset: 0x000009A0
		public void Release()
		{
			bool flag = this.modelLocator;
			bool flag2 = flag && !this.modelLocatorStartedDisabled;
			if (flag2)
			{
				this.modelLocator.enabled = true;
			}
			bool flag3 = this.modelTransform;
			bool flag4 = flag3;
			if (flag4)
			{
				this.modelTransform.rotation = this.originalRotation;
			}
			bool flag5 = this.direction;
			bool flag6 = flag5;
			if (flag6)
			{
				this.direction.enabled = true;
			}
			bool flag7 = this.extraCollider;
			if (flag7)
			{
				this.extraCollider.layer = this.extraLayer;
			}
			bool flag8 = this.extraCollider2;
			if (flag8)
			{
				this.extraCollider2.layer = this.extraLayer2;
			}
			base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			GameObject.Destroy(this);
		}

		// Token: 0x0400003A RID: 58
		private GameObject extraCollider;

		// Token: 0x0400003B RID: 59
		private GameObject extraCollider2;

		// Token: 0x0400003C RID: 60
		private int extraLayer;

		// Token: 0x0400003D RID: 61
		private int extraLayer2;

		// Token: 0x0400003E RID: 62
		private bool modelLocatorStartedDisabled;

		// Token: 0x0400003F RID: 63
		public Transform pivotTransform;

		// Token: 0x04000040 RID: 64
		public CharacterBody body;

		// Token: 0x04000041 RID: 65
		public CharacterMotor motor;

		// Token: 0x04000042 RID: 66
		private CharacterDirection direction;

		// Token: 0x04000043 RID: 67
		private ModelLocator modelLocator;

		// Token: 0x04000044 RID: 68
		private Transform modelTransform;

		// Token: 0x04000045 RID: 69
		private Quaternion originalRotation;
	}
}
