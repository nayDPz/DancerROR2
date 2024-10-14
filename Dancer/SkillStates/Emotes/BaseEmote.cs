using EntityStates;
using RoR2;
using UnityEngine;

namespace Dancer.SkillStates.Emotes
{
	public class BaseEmote : BaseState
	{
		public string soundString;

		public string animString;

		public float duration;

		public float animDuration;

		public bool normalizeModel;

		private uint activePlayID;

		private Animator animator;

		protected ChildLocator childLocator;

		private CharacterCameraParams originalCameraParams;

		public LocalUser localUser;

		public override void OnEnter()
		{
			base.OnEnter();
			animator = GetModelAnimator();
			childLocator = GetModelChildLocator();
			localUser = LocalUserManager.readOnlyLocalUsersList[0];
			base.characterBody.hideCrosshair = true;
			if ((bool)GetAimAnimator())
			{
				GetAimAnimator().enabled = false;
			}
			animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0f);
			animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0f);
			if (animDuration == 0f && duration != 0f)
			{
				animDuration = duration;
			}
			PlayAnimation("FullBody, Override", animString, "Emote.playbackRate", animDuration);
			activePlayID = Util.PlaySound(soundString, base.gameObject);
			if (normalizeModel && (bool)base.modelLocator)
			{
				base.modelLocator.normalizeToFloor = true;
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			base.characterBody.hideCrosshair = false;
			if ((bool)GetAimAnimator())
			{
				GetAimAnimator().enabled = true;
			}
			if ((bool)animator)
			{
				animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1f);
				animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1f);
			}
			if (normalizeModel && (bool)base.modelLocator)
			{
				base.modelLocator.normalizeToFloor = false;
			}
			base.PlayAnimation("FullBody, Override", "BufferEmpty");
			if (activePlayID != 0)
			{
				AkSoundEngine.StopPlayingID(activePlayID);
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			bool flag = false;
			if ((bool)base.characterMotor)
			{
				if (!base.characterMotor.isGrounded)
				{
					flag = true;
				}
				if (base.characterMotor.velocity != Vector3.zero)
				{
					flag = true;
				}
			}
			if ((bool)base.inputBank)
			{
				if (base.inputBank.skill1.down)
				{
					flag = true;
				}
				if (base.inputBank.skill2.down)
				{
					flag = true;
				}
				if (base.inputBank.skill3.down)
				{
					flag = true;
				}
				if (base.inputBank.skill4.down)
				{
					flag = true;
				}
				if (base.inputBank.jump.down)
				{
					flag = true;
				}
				if (base.inputBank.moveVector != Vector3.zero)
				{
					flag = true;
				}
			}
			if (!Util.HasEffectiveAuthority(base.gameObject) || !base.characterMotor.isGrounded || !localUser.isUIFocused)
			{
			}
			if (duration > 0f && base.fixedAge >= duration)
			{
				flag = true;
			}
			if (flag)
			{
				outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Any;
		}
	}
}