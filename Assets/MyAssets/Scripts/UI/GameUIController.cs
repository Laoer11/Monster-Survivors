using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 游戏UI控制器
    /// </summary>
    public class GameUIController : MonoBehaviour
    {
        [Header("引用")]
        [SerializeField] private SettingsPopup settingsPopup;

        private void Update()
        {
            // 检测 Esc 键
            bool escPressed = UnityEngine.InputSystem.Keyboard.current != null
                              && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;

            if (escPressed && settingsPopup != null)
            {
                if (settingsPopup.IsOpen)
                    settingsPopup.Hide();
                else
                    settingsPopup.Show();
            }
        }
    }
}