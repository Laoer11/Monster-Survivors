using UnityEngine;

namespace MonsterSurvivors
{
    public class TestSpawner : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyData;

        private void Start()
        {
            Vector2 pos = (Vector2)PlayerBehavior.Instance.transform.position 
                + Random.insideUnitCircle.normalized * 5f;

            GameObject go = Instantiate(enemyData.prefab, pos, Quaternion.identity);
            go.GetComponent<EnemyBehavior>().Init(enemyData);
            
        }
    }
}
