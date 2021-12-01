using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace StoryNook
{
    public class StartupScreen : BaseScreen
    {
        public Dropdown dropdown;
        public InputField welcomeTimerInput;
        public Slider transitionTimeSlider;
        [SerializeField] private Text transitionTimeDisplay;

        [SerializeField] private GameObject settings;
        [SerializeField] private Toggle debugToggle;

        private void Start()
        {
            // This is the first screen, so it needs to start out visible
            canvasGroup.alpha = 1f;
            if (material != null)
            {
                material.color = Color.white;
            }
        }

        private void OnEnable()
        {
            if (screenManager == null)
            {
                screenManager = ScreenManager.instance;
            }
            screenManager.currentScreen = (int)Screen.Startup;
        }

        // Unhides settings, and also turns on diagnostics on Feature screen
        public void TurnOnDiagnostics()
        {

            settings.SetActive(debugToggle.isOn);
            screenManager.debugOn = debugToggle.isOn;
        }

        public void AdjustTransitionTime()
        {
            screenManager.transitionTime = transitionTimeSlider.value;
            transitionTimeDisplay.text = transitionTimeSlider.value.ToString("f1");
        }

        public void SetStartingFeature()
        {
            if (dropdown != null)
            {
                int startingFeature = dropdown.value;
                screenManager.startingFeature = startingFeature;
                screenManager.currentFeature = startingFeature;
            }
        }

        public void SetReturnToWelcomeTimer()
        {
            if (welcomeTimerInput != null)
            {
                float welcomeTime = float.Parse(welcomeTimerInput.text);
                screenManager.returnToWelcomeTime = welcomeTime;
            }

        }
    }
}
