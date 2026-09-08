using UnityEngine;

namespace MonsterSurvivors
{
    public class ToolkitTest : MonoBehaviour
    {
        [SerializeField] private Transform testCube;  // 拖一个测试物体

        // 测试 4
        private Vector2 lastMovement;
        private InputType lastType;


        private void Start()
        {
            Debug.Log($"[ToolkitTest] EasingManager 是否存在: {EasingManager.Instance != null}");

            // 测试 1：移动动画
            testCube.DoMove(new Vector3(5, 0, 0), 1f, EasingType.BounceOut)
                .SetOnFinish(() => Debug.Log("移动完成！"));

            // 测试 2：缩放弹入
            EasingManager.Instance.DoAfter(1.5f, () =>
            {
                testCube.localScale = Vector3.zero;
                testCube.DoScale(Vector3.one, 0.5f, EasingType.BackOut);
                Debug.Log("缩放弹入开始");
            });

            // 测试 3：对象池
            Pool<Transform> pool = new Pool<Transform>(testCube, 3, "TestPool");
            Transform obj = pool.Get();
            obj.position = new Vector3(-3, 0, 0);
            EasingManager.Instance.DoAfter(2f, () =>
            {
                pool.Return(obj);
                Debug.Log($"回收，池中空闲: {pool.AvailableCount}");
            });

            // 测试 4：输入管理器
            // 测试 5：测试音频
            AudioManager.PlayMusic("MainMenu");

            EasingManager.Instance.DoAfter(1f, () =>
            {
                AudioManager.PlaySFX("Click");     // 1秒后播个音效
            });

            EasingManager.Instance.DoAfter(2f, () =>
            {
                AudioManager.PlaySFX("LevelUp");   // 2秒后播升级音效
            });

        }

        private void Update()
        {
            Vector2 currentMove = InputManager.Movement;
            InputType currentType = InputManager.ActiveInputType;

            // 只在变化时输出
            if (currentMove != lastMovement || currentType != lastType)
            {
                Debug.Log($"[Input] 移动方向: {currentMove}, 设备: {currentType}");
                lastMovement = currentMove;
                lastType = currentType;
            }
            
            // 检查暂停按钮
            if(InputManager.PausePressed)
            {
                Debug.Log("[Input] 暂停按钮被按下");
                // 处理暂停逻辑

            }

        }

    }
}
