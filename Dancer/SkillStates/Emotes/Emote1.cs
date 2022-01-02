using System;
using System.Collections.Generic;
using System.Text;
using RoR2.Audio;
using RoR2;
namespace Dancer.SkillStates.Emotes
{
    public class Emote1 : BaseEmote
    {

        public override void OnEnter()
        {
            Util.PlaySound("EmoteScream", base.gameObject);
            this.animString = "Emote1";
            this.animDuration = 1f;
            this.duration = 1.75f;
            this.normalizeModel = false;
            base.OnEnter();
            
        }
    }
}
