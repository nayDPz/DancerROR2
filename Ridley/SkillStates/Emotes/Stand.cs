using RoR2;
namespace Ridley.SkillStates.Emotes
{
    public class Stand : BaseEmote
    {
        public override void OnEnter()
        {
            this.GetModelAnimator().SetBool("endStand", false);
            Util.PlaySound("EmoteStandStart", base.gameObject);
            this.animString = "StandStart";
            this.animDuration = 1f;
            this.duration = float.PositiveInfinity;
            this.normalizeModel = false;
            this.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
            base.OnEnter();
            
        }
        public override void OnExit()
        {
            this.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
            Util.PlaySound("EmoteStandEnd", base.gameObject);
            this.GetModelAnimator().SetBool("endStand", true);
            base.OnExit();
            
        }
    }
}