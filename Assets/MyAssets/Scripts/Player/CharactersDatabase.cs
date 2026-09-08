using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    [CreateAssetMenu(fileName = "CharactersDatabase", menuName = "Scriptable Objects/CharactersDatabase")]
    public class CharactersDatabase : ScriptableObject
    {
        public List<CharacterData> characters = new List<CharacterData>();

        public CharacterData GetById(string id)
        {
            return characters.Find(c => c.characterId == id);
        }

        public int Count => characters.Count;
    }
}

