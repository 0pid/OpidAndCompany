using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OpidAndCompany.Behaviours
{
    internal class Glowstick : PhysicsProp
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            var lightComponent = FetchLightComponent();
            if (lightComponent != null)
            {
                lightComponent.intensity = Plugin.GlowStickIntensity.Value;
                currentIntensity = lightComponent.intensity;
            }
        }
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
        //
        private bool hasBeenPickedUp;
        private float currentIntensity;
        public float decayRate = 0.1f; // Controls how fast the glowstick fades

        public override void Update()
        {
            base.Update();
            // Reduce intensity over time
            if (currentIntensity > 0)
            {
                var decayBy = decayRate * Time.deltaTime;
                currentIntensity -= decayBy;
                Debug.Log("Light decay: " + decayBy);
                currentIntensity = Mathf.Max(0, currentIntensity); // Ensure it doesn't go negative
            }

            var lightComponent = FetchLightComponent();
            if (lightComponent != null)
            {
                lightComponent.intensity = currentIntensity;
                Debug.Log("Light inensity: " + lightComponent.intensity);
            }
        }

        public override void InteractItem()
        {
            base.InteractItem();
            hasBeenPickedUp = true;
            Debug.Log("InteractItem");            
            Debug.Log((object)string.Format("interact {0}", (object)((UnityEngine.Object)this.playerHeldBy == (UnityEngine.Object)null)));
            if ((UnityEngine.Object)this.playerHeldBy == (UnityEngine.Object)null)
                return;

            this.playerHeldBy.playerBodyAnimator.SetTrigger("shakeItem");
        }

        public override void ItemInteractLeftRight(bool right)
        {            
            base.ItemInteractLeftRight(right);            
            if (playerHeldBy != null)
            {
                var lightComponent = FetchLightComponent();
                if (lightComponent != null)
                {
                    this.playerHeldBy.playerBodyAnimator.SetTrigger("shakeItem");
                    if (right)
                    {
                        currentIntensity += 5;                        
                    }
                    else
                    {
                        currentIntensity -= 5;                        
                    }
                    Debug.Log("Light inensity: " + lightComponent.intensity);
                }
            }
        }

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

            if (playerHeldBy != null)
            {
                this.playerHeldBy.equippedUsableItemQE = false;
            }
            else
            {
                Debug.Log("playerHeldBy is null");
            }
        }

        public override void EquipItem()
        {
            base.EquipItem();
            hasBeenPickedUp = true;
            var lightNode = this.transform.Find("Light_S");
            if (lightNode != null)
            {
                var lightComponent = lightNode.GetComponent<Light>();
                if (lightComponent != null)
                {
                    if(Plugin.GlowStickIntensity != null)
                    {
                        if(currentIntensity == null)
                        {
                            currentIntensity = Plugin.GlowStickIntensity.Value;
                        }

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

            if(playerHeldBy != null)
            {
                this.playerHeldBy.equippedUsableItemQE = true;                
            }
            else
            {
                Debug.Log("playerHeldBy is null");
            }
        }

        public override void DiscardItem()
        {
            base.DiscardItem();
            if (playerHeldBy != null)
            {
                this.playerHeldBy.equippedUsableItemQE = false;
            }
            else
            {
                Debug.Log("playerHeldBy is null");
            }
        }

        private Light? FetchLightComponent()
        {
            var lightNode = this.transform.Find("Light_S");
            if (lightNode != null)
            {
                return lightNode.GetComponent<Light>();                
            }
            return null;
        }
    }
}
