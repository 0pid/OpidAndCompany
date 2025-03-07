using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OpidAndCompany.Behaviours
{
    internal class Glowstick : PhysicsProp
    {
        //public override void ItemActivate(bool used, bool buttonDown = true)
        //{
        //    base.ItemActivate(used, buttonDown);
        //    if (buttonDown)
        //    {
        //        if (playerHeldBy != null)
        //        {
        //            var lightNode = this.transform.Find("Light_S");
        //            if (lightNode != null)
        //            {
        //                var lightComponent = lightNode.GetComponent<Light>();
        //                if (lightComponent != null)
        //                {
        //                    // Toggle light on or off
        //                    lightComponent.intensity += 1;
        //                    Debug.Log("Light intensity now: " + lightComponent.intensity);
        //                }
        //                else
        //                {
        //                    Debug.Log("Light component not found");
        //                }
        //            }
        //            else
        //            {
        //                Debug.Log("Light node not found");
        //            }
        //        }
        //    }
        //}

        public override void PocketItem()
        {
            base.PocketItem();
            var lightNode = this.transform.Find("Light_S");
            if (lightNode != null)
            {
                var lightComponent = lightNode.GetComponent<Light>();
                if (lightComponent != null)
                {
                    lightComponent.enabled = false;
                    Debug.Log("Light toggled: " + lightComponent.enabled);
                }
                else
                {
                    Debug.Log("Light component not found");
                }
            }
            else
            {
                Debug.Log("Light node not found");
            }
        }

        public override void EquipItem()
        {
            base.EquipItem();
            var lightNode = this.transform.Find("Light_S");
            if (lightNode != null)
            {
                var lightComponent = lightNode.GetComponent<Light>();
                if (lightComponent != null)
                {
                    if(Plugin.GlowStickIntensity != null)
                    {
                        lightComponent.intensity = Plugin.GlowStickIntensity.Value;
                    }

                    lightComponent.enabled = true;
                    Debug.Log("Light toggled: " + lightComponent.enabled);
                }
                else
                {
                    Debug.Log("Light component not found");
                }
            }
            else
            {
                Debug.Log("Light node not found");
            }
        }
    }
}
