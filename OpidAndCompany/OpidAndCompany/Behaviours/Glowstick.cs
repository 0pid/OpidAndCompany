using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace OpidAndCompany.Behaviours
{
    internal class Glowstick : PhysicsProp
    {
        private Light? lightComponent;
        private float fadeDuration = 580f;
        private float glowDuration = 3f;
        private float elapsedFadeTime = 0f;
        private float elapsedGlowTime = 0f;

        private float currentIntensity;
        private float originalIntensity;
        private bool isGlowing = false;
        private bool isFading = true;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if(lightComponent == null)
            {
                lightComponent = FetchLightComponent();
            }

            if (lightComponent != null)
            {
                originalIntensity = Plugin.GlowStickIntensity.Value;
                currentIntensity = Plugin.GlowStickIntensity.Value;
                lightComponent.intensity = Plugin.GlowStickIntensity.Value;
            }
        }
        
        private bool hasBeenPickedUp;       
        

        public override void Update()
        {
            base.Update();
            if(lightComponent == null)
            {
                var lightComponent = FetchLightComponent();
            }

            if(lightComponent == null)
            {
                return;
            }

            if (isGlowing)
            {
                elapsedGlowTime += Time.deltaTime;
                currentIntensity = Mathf.Lerp(currentIntensity, originalIntensity, elapsedGlowTime / glowDuration);
                lightComponent.intensity = currentIntensity;

                if (elapsedGlowTime >= glowDuration) // Stop glowing after 3s
                {
                    isGlowing = false;
                    isFading = true;
                    elapsedFadeTime = 0f; // Reset fade timer
                }
            }
            else if (isFading)
            {
                elapsedFadeTime += Time.deltaTime;
                currentIntensity = Mathf.Lerp(lightComponent.intensity, 0f, elapsedFadeTime / fadeDuration);
                lightComponent.intensity = currentIntensity;

                if (lightComponent.intensity <= 0.01f) // Close enough to fully faded
                {
                    lightComponent.intensity = 0f;
                    isFading = false;
                }
            }           
        }

        public override void ItemInteractLeftRight(bool right)
        {            
            base.ItemInteractLeftRight(right);
            if (!right)
            {
                if (playerHeldBy != null)
                {
                    isFading = false;
                    isGlowing = true;
                    elapsedGlowTime = 0f;
                    this.playerHeldBy.playerBodyAnimator.SetTrigger("shakeItem");

                    //if (lightComponent == null)
                    //{
                    //    lightComponent = FetchLightComponent();
                    //}

                    //if (lightComponent != null)
                    //{
                        
                    //    elapsedTime += Time.deltaTime;
                    //    lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, initialIntensity, elapsedTime / glowDuration);
                    //}                
                }
            }
        }

        public override void PocketItem()
        {
            base.PocketItem();
            if(lightComponent == null)
            {
                lightComponent = FetchLightComponent();
            }

            if (lightComponent != null) 
            {
                lightComponent.enabled = false;
            }

            if (playerHeldBy != null)
            {
                this.playerHeldBy.equippedUsableItemQE = false;
            }
        }

        public override void EquipItem()
        {
            base.EquipItem();
            hasBeenPickedUp = true;
            if (lightComponent == null)
            {
                lightComponent = FetchLightComponent();
            }

            if (lightComponent != null)
            {
                lightComponent.enabled = true;
            }

            if (playerHeldBy != null)
            {
                this.playerHeldBy.equippedUsableItemQE = true;
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
