using UnityEngine;

namespace ALine
{
    public class GameDebugger : MonoBehaviour
    {
        protected void Awake()
        {
            instance = this;
        }
        protected void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        static public GameDebugger instance;

        public bool debugResolutionCalculations;
        public bool debugAppLogic;
        public bool debugRoadGeneration;
        public bool debugUserInput;
    }
}