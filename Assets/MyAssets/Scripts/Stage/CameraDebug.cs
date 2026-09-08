using UnityEngine;

namespace MonsterSurvivors
{
    public class CameraDebug : MonoBehaviour
    {
        private int frame;

        private void Update()
        {
            // 每 30 帧打印一次，避免刷屏
            frame++;
            if (frame % 30 == 0)
            {
                Debug.Log($"[Debug] Camera.main={Camera.main != null}, 位置={Camera.main.transform.position}");
            }
        }
    }
}