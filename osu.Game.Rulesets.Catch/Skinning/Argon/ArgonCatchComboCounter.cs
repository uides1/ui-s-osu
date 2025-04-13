// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.


using osu.Framework.Graphics;
using osu.Game.Rulesets.Catch.UI;
using osu.Game.Screens.Play;
using osu.Game.Screens.Play.HUD;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Catch.Skinning.Argon
{
    public partial class ArgonCatchComboCounter : ArgonComboCounter, ICatchComboCounter
    {
        private int lastDisplayedCombo;

        public ArgonCatchComboCounter()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Scale = new Vector2(0.8f);
        }

        public void UpdateCombo(int combo, Color4? hitObjectColour = null)
        {
            if (combo == lastDisplayedCombo)
                return;

            lastDisplayedCombo = combo;

            if ((Clock as IGameplayClock)?.IsRewinding == true)
            {
                Hide();
                return;
            }

            // Combo fell to zero, fade out the counter
            if (combo == 0)
            {
                this.FadeOut(400, Easing.Out);
            }
            else
            {
                this.FadeInFromZero().Then().Delay(1000).FadeOut(300);
            }

            Current.Value = combo;
        }
    }
}
