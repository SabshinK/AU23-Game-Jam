using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace King
{
    public class LevelTransitionManager : MonoBehaviour
    {

        [SerializeField] private Material trasitionMat;
        public static LevelTransitionManager Instance;
        //SINGLETOOOON
        private void Awake()
        {
            if (Instance)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            TransitionIntoLevel();
        }

        bool transitioning = false;
        public void LoadLevel(string levelName)
        {
            StartCoroutine(TransitionOutOfLevel(levelName));
        }

        private IEnumerator TransitionOutOfLevel(string levelName)
        {
            //Force initial state to be a transparent screen
            trasitionMat.SetFloat("_Opacity", 5);
            trasitionMat.SetInt("_Inverse", 0);
            transitioning = true;
            //Load level in the background
            var async = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
            async.allowSceneActivation = false;

            //Start the transition animation
            LeanTween.value(trasitionMat.GetFloat("_Opacity"), 0, 0.5f).setOnUpdate((float f) =>
            {
                trasitionMat.SetFloat("_Opacity", f);
            }).setOnComplete(
                delegate ()
                {
                    transitioning = false;
                });

            //Wait until both the animation and loading are done
            while(!async.isDone && !transitioning)
            {
                yield return null;
            }

            //Allow the scene to transition
            async.allowSceneActivation = true;


            TransitionIntoLevel();
        }
        private void TransitionIntoLevel()
        {
            trasitionMat.SetInt("_Inverse", 1);
            trasitionMat.SetFloat("_Opacity", 5);
            LeanTween.value(trasitionMat.GetFloat("_Opacity"), 0, 0.5f).setOnUpdate((float f) =>
            {
                trasitionMat.SetFloat("_Opacity", f);
            });
        }
    }

    
}
