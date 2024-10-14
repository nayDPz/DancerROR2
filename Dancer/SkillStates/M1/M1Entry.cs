using EntityStates;

namespace Dancer.SkillStates.M1
{

    public class M1Entry : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (isAuthority)
            {
                float y = inputBank.aimDirection.y;
                if (y > 0.575f)
                {
                    outer.SetNextState(new UpAir());
                }
                else if (y < -0.425f)
                {
                    if (characterMotor.isGrounded)
                    {
                        outer.SetNextState(new DownAirLand());
                    }
                    else if (y < -0.74f)
                    {
                        outer.SetNextState(new DownAir());
                    }
                    else
                    {
                        outer.SetNextState(new FAir());
                    }
                }
                else if (!characterMotor.isGrounded)
                {
                    outer.SetNextState(new FAir());
                }
                else if (characterBody.isSprinting)
                {
                    outer.SetNextState(new DashAttack());
                }
                else
                {
                    outer.SetNextState(new Jab1());
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}