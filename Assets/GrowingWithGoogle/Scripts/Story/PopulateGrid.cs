// Can be used to populate the grid of story cards dynamically


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryNook
{
    public class PopulateGrid : MonoBehaviour
    {

        private GameObject prefab;
        private ScreenManager screenManager;

        void Start()
        {
            screenManager = ScreenManager.instance;

            prefab = screenManager.storyPrefab;

            if (prefab != null)
            {
                Populate();
            }
        }

        private void Populate()
        {
            GameObject newObj;

            for (int i = 0; i < screenManager.clips.Count; i++)
            {
                newObj = (GameObject)Instantiate(prefab, transform);
                newObj.GetComponent<StoryPrefabConstructor>().storyID = screenManager.storySettings[i].StoryID;
            }
        }
    }
}
