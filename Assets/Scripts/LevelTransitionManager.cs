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
        }


        bool transitioning = false;
        public void LoadLevel(string levelName)
        {
        }

        private IEnumerator TransitionOutOfLevel(string levelName)
        {

            var async = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);

            while(!async.isDone && !transitioning)
            {
                yield return null;
            }
        }
    }
}
