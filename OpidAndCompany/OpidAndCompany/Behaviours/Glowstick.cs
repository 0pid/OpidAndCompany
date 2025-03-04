using System;
using System.Collections.Generic;
using System.Text;

namespace OpidAndCompany.Behaviours
{
    internal class Glowstick : PhysicsProp
    {
        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (playerHeldBy != null)
            {
                //WIP
            }
        }
    }
}
