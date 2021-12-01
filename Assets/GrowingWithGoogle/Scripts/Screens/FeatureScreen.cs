using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

namespace StoryNook
{
    public class FeatureScreen : BaseScreen
    {
        [SerializeField] private DebugFeatureScreen debug;
        public Slider progressBar;
        public Slider volumeSlider;
        public TextMeshProUGUI company;

        private int currentFeature;
        private VideoPlayer player;
        private AudioSource audioSource;
        private bool transitionDone;
        private bool isDone;
        private Color highlight;
        [SerializeField] private GameObject caption;
        public Image ccButtonImage;
        [SerializeField] private string srtFileName;
        private SubtitleController subtitleController;

        [SerializeField] private GameObject loadingImage;

        protected override void AwakeActions()
        {
            currentFeature = screenManager.currentFeature;
            subtitleController = this.gameObject.GetComponent<SubtitleController>(); // need this reference to pass thru the SRT file name in OnEnable
        }

        private void Start()
        {
            if (player == null) { player = GetComponent<VideoPlayer>(); }
            if (player != null)
            {
                currentFeature = screenManager.currentFeature;

                player.targetTexture.Release();
                player.clip = screenManager.clips[currentFeature];
                volumeSlider.value = screenManager.volume;
                UpdateVolume();

                highlight = new Color(0.9176f, 0.2627f, 0.2078f); // reddish color for CC button on
            }

            debug.gameObject.SetActive(screenManager.debugOn);
        }

        private void UpdateDebug()
        {
            debug.prepared.text = player.isPrepared.ToString();
            debug.paused.text = player.isPaused.ToString();
            debug.playing.text = player.isPlaying.ToString();
            debug.frame.text = player.frame.ToString();
            debug.timeSec.text = player.time.ToString("f1");
        }

        public void PlayIfStuck()
        {
            if (!player.isPlaying)
            {
                player.Play();
            }
        }

        public void Update()
        {
            if (screenManager.debugOn) { UpdateDebug(); }  //TODO: set up as a callback

            if (!player.isPrepared) { return; }
            if (!transitionDone) { return; }

            if (!player.isPlaying && !isDone)
            {
                player.Play();
                subtitleController.SetFileName(srtFileName);
            }

            UpdateProgressBar();

            if (isDone)
            {
                player.Pause();
                progressBar.value = 1.0f;
                StartCoroutine(WaitThenCTA(1.0f)); // hold the last frame briefly before trasition to CTA
            }
        }

        public void ToggleCaptionState()
        {
            screenManager.captionOn = !screenManager.captionOn;
            SetCaptionState(screenManager.captionOn);
        }

        private void SetCaptionState(bool captionOn)
        {
            caption.SetActive(captionOn);
            ccButtonImage.color = captionOn ? highlight : Color.white;
        }


        private IEnumerator WaitThenCTA(float duration)
        {
            yield return new WaitForSeconds(duration);
            ScreenManager.instance.TransitionScreens(thisScreen, Screen.CTA);
        }

        private void OnEnable()
        {
            if (screenManager == null)
            {
                screenManager = ScreenManager.instance;
            }
            screenManager.currentScreen = (int)Screen.Feature;

            if (player == null) { player = GetComponent<VideoPlayer>(); }

            // Video player callbacks
            player.prepareCompleted += PrepareCompleted;
            player.errorReceived += ErrorReceived;
            player.loopPointReached += LoopPointReached;

            progressBar.value = 0.0f;
            transitionDone = false;
            isDone = false;
            loadingImage.SetActive(true);

            currentFeature = screenManager.currentFeature;
            company.text = storyData[currentFeature].Company;
            srtFileName = storyData[currentFeature].SRTFileName;

            // Update the feature Played bool upon starting to play. Could be done when play is finished instead.
            screenManager.storySettings[currentFeature].Played = true;

            //TODO: Change Feature to Story where appropriate e.g. currentStory in place of currentFeature

            if (player != null)
            {
                player.targetTexture.Release();
                player.clip = screenManager.clips[currentFeature];
                player.Prepare();

                audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    player.SetTargetAudioSource(0, audioSource);
                    audioSource.volume = screenManager.volume;
                }
                screenManager.storySettings[currentFeature].Played = true;
            }

            volumeSlider.value = screenManager.volume; // keep the volume consistent across videos to what the user has set
            SetCaptionState(screenManager.captionOn);
        }

        void LoopPointReached(VideoPlayer source)
        {
            isDone = true;
            player.Pause();
        }


        void ErrorReceived(VideoPlayer source, string message)
        {
            if (screenManager.debugOn) { debug.error.text = message; }
        }


        void PrepareCompleted(VideoPlayer source)
        {
            if (loadingImage != null)
            {
                loadingImage.SetActive(false);
                isDone = false;
            }
            if (screenManager.debugOn)
            {
                debug.frameCount.text = player.clip.frameCount.ToString();
                debug.lengthSec.text = player.clip.length.ToString("f1");
            }
        }


        protected override void AfterTransitionActions()
        {
            canvasGroup.interactable = true;
            transitionDone = true;

        }

        private void UpdateProgressBar()
        {
            progressBar.value = (float)(player.time / player.clip.length);
        }

        public void UpdateVolume()
        {
            if (audioSource != null)
            {
                audioSource.volume = volumeSlider.value;
                // update the volume in screen manager so that the volume is consistent across videos (unless changed by user)
                screenManager.volume = volumeSlider.value;
            }
        }

        private void OnDisable()
        {
            this.gameObject.GetComponent<SubtitleController>().Stop();
            player.prepareCompleted -= PrepareCompleted;
            player.errorReceived -= ErrorReceived;
            player.loopPointReached -= LoopPointReached;
            player.targetTexture.Release();
        }
    }
}
