using UnityEngine;

namespace Model
{
    public class SceneModel
    {
        private static SceneModel _instance = new SceneModel();

        public static SceneModel Instance => _instance;
        
        public GameObject BattleScene;
    }
}