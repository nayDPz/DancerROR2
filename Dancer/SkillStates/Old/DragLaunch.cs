using System;
using EntityStates.Merc;
using Dancer.Modules;
using System.Collections.Generic;
using System.Linq;
using EntityStates;
using EntityStates.Commando;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;
namespace Dancer.SkillStates
{
	public class DragLaunch : BaseSkillState
	{
		public float exitSpeed;
		public Vector3 lastSafeFootPosition;
		public List<GrabController> grabController;
		private float stopwatch;
		private float duration = 0.5f;
		public Vector3 direction;
		public override void OnEnter()
		{
			base.OnEnter();
			RaycastHit raycastHit;

			

			if (!Physics.Raycast(new Ray(base.characterBody.footPosition, Vector3.down), out raycastHit, 100f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
				base.transform.position = this.lastSafeFootPosition + Vector3.up * 5;
			Util.PlaySound("DragLaunch", base.gameObject);
			Util.PlaySound("DragLaunchVoice", base.gameObject);
			base.PlayAnimation("FullBody, Override", "DragEnd", "Slash.playbackRate", 0.4f);

			this.direction = base.characterMotor.moveDirection;
		}
        public override void FixedUpdate()
        {
            base.FixedUpdate();

			base.characterDirection.forward = this.direction;
			//base.characterMotor.moveDirection = this.direction * this.exitSpeed * Mathf.Lerp(1f, 0f, base.fixedAge / this.duration) / 4f;
			base.characterMotor.velocity = Vector3.zero; ////////delet
			base.characterMotor.moveDirection = Vector3.zero;
			if (base.fixedAge >= this.duration)
            {
				this.outer.SetNextStateToMain();
				return;
            }
		
		}
    }
}
