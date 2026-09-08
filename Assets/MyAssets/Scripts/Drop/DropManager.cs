using UnityEngine;

namespace MonsterSurvivors
{
    public class DropManager : MonoBehaviour
    {
        [SerializeField] private DropDatabase database;

        public void Drop(DropType type,Vector2 position)
        {
            DropData data = database.GetByType(type);
            if(data == null || data.prefab == null) return;

            GameObject go = Instantiate(data.prefab, position, Quaternion.identity);
            go.GetComponent<DropBehavior>().Init(data);
        }
    }
}