using UnityEngine;
using UnityEngine.UI;

namespace StoryNook
{
    public class StoryNookButtonHandler : MonoBehaviour
    {
        public Screen nextScreen;
        private Screen thisScreen;

        void Start()
        {
            Button button = this.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => HandleButton(button));
            thisScreen = this.transform.root.gameObject.GetComponent<BaseScreen>().thisScreen;
        }

        void HandleButton(Button button)
        {
            // If a story prefab has been tapped, update the current story (feature) to show
            if (button.gameObject.CompareTag("StoryPrefab"))
            {
                ScreenManager.instance.currentFeature = button.gameObject.GetComponent<StoryPrefabConstructor>().storyID;
            }

            ScreenManager.instance.TransitionScreens(thisScreen, nextScreen);
        }
    }
}
