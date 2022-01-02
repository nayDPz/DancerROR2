using System;
using System.Collections.Generic;
using System.Text;

namespace Dancer.SkillStates.Emotes
{
    public class Emote2 : BaseEmote
    {

        public override void OnEnter()
        {
            this.animString = "Emote2";
            this.animDuration = 1f;
            this.duration = 1f;
            this.normalizeModel = false;
            base.OnEnter();
            
        }
    }
}
