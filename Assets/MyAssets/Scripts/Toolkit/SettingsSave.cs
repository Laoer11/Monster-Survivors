using UnityEngine;
using System;

namespace MonsterSurvivors
{
    [Serializable]
    public class SettingsSave : ISave
    {
        public float musicVolume = 0.8f;
        public float sfxVolume = 1f;

        public void Init()
        {
            musicVolume = 0.8f;
            sfxVolume = 1f;
        }

        public void Flush()
        {

        }
    }
}
