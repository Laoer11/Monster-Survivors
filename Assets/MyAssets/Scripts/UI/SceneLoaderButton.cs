using UnityEngine;

namespace MonsterSurvivors
{
    public class SceneLoaderButton : MonoBehaviour
    {
        public void LoadStage()
        {
            GameController.LoadStage();
            Debug.Log("加载游戏场景");
        }

        public void LoadMainMenu()
        {
            GameController.LoadMainMenu();
            Debug.Log("加载主菜单场景");
        }
    }
}

