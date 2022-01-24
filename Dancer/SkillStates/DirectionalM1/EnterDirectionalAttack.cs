using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using EntityStates;

namespace Dancer.SkillStates
{
    public class EnterDirectionalAttack : BaseInputEvaluation
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.EvaluateInput();

			if (this.inputVector.y < -0.5f)
			{
				this.nextState = new AttackLeft();
			}
			else if (this.inputVector.y > 0.5f)
			{
				this.nextState = new AttackRight();
			}
			else if (this.inputVector.x < -0.5f)
			{
				if (!base.isGrounded)
				{
					this.nextState = new AttackDown();
				}
				else
				{
					this.nextState = new AttackBackwards();
				}

			}
			else if (this.inputVector.x > 0.5f)
			{
				this.nextState = new AttackForward();
			}
			this.SetNextState();
        }

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}
	}
}
