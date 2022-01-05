using System;
using EntityStates;
using UnityEngine;

namespace Dancer.SkillStates
{
	public class EasyM1Entry : BaseSkillState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			float y = base.inputBank.aimDirection.y;

			if (y > 0.575f)
			{
				this.outer.SetNextState(new UpAir());
			}
			else if(y < -0.6f && !base.isGrounded)
			{
				this.outer.SetNextState(new EasyFAir());
			}
			else
            {
				this.outer.SetNextState(new EasyJab1());
            }
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}
}
