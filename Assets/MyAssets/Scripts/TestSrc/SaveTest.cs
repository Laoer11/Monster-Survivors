using UnityEngine;

namespace MonsterSurvivors
{
    public class SaveTest : MonoBehaviour
    {
        private void Start()
        {
            // 旧测试
            PlayerSave save = GameController.SaveManager.GetSave<PlayerSave>("PlayerSave");
            Debug.Log($"[SaveTest] 金币: {save.gold}, 最高关卡: {save.maxReachedStage}");
            save.gold += 100;
            save.maxReachedStage += 1;

            // === 新作业 ===
            SettingsSave settings = GameController.SaveManager.GetSave<SettingsSave>("SettingsSave");
            Debug.Log($"[SaveTest] 音乐音量: {settings.musicVolume}, 音效音量: {settings.sfxVolume}");

            // 试试修改
            settings.musicVolume = 0.5f;
            settings.sfxVolume = 0.3f;

            // 保存
            GameController.SaveManager.SaveToDisk();
            Debug.Log("[SaveTest] 已保存！");
        }
    }
}