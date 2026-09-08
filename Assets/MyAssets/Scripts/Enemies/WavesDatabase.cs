using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    [CreateAssetMenu(fileName = "WavesDatabase", menuName = "MonsterSurvivors/Waves Database")]
    public class WavesDatabase : ScriptableObject
    {
        public List<WaveData> waves = new List<WaveData>();

        public int Count => waves.Count;
        public WaveData GetWave(int index) => waves[index];
    }
}