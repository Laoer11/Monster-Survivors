using UnityEngine;
using System.Collections.Generic;

namespace MonsterSurvivors
{
    public class InfiniteFieldManager : MonoBehaviour
    {
        public static InfiniteFieldManager Instance { get; private set; }

        [SerializeField] private Camera cam;            // ★ 直接拖 Main Camera
        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private float chunkSize = 10f;
        [SerializeField] private int margin = 2;

        private Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (cam == null) cam = Camera.main;   // 兜底

            if (chunkPrefab != null)
            {
                SpriteRenderer sr = chunkPrefab.GetComponent<SpriteRenderer>();
                if (sr != null)
                    chunkSize = sr.size.x;
            }
        }

        private void Update()
        {
            if (cam == null) return;

            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            Vector2 camPos = cam.transform.position;

            int minX = Mathf.FloorToInt((camPos.x - halfW) / chunkSize) - margin;
            int maxX = Mathf.FloorToInt((camPos.x + halfW) / chunkSize) + margin;
            int minY = Mathf.FloorToInt((camPos.y - halfH) / chunkSize) - margin;
            int maxY = Mathf.FloorToInt((camPos.y + halfH) / chunkSize) + margin;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2Int key = new Vector2Int(x, y);
                    if (!chunks.ContainsKey(key))
                        SpawnChunk(key);
                }
            }

            List<Vector2Int> toRemove = new List<Vector2Int>();
            foreach (var kvp in chunks)
            {
                Vector2Int key = kvp.Key;
                if (key.x < minX || key.x > maxX || key.y < minY || key.y > maxY)
                    toRemove.Add(key);
            }
            foreach (var key in toRemove)
            {
                Destroy(chunks[key]);
                chunks.Remove(key);
            }
        }

        private void SpawnChunk(Vector2Int key)
        {
            Vector3 pos = new Vector3(key.x * chunkSize, key.y * chunkSize, 0);
            GameObject chunk = Instantiate(chunkPrefab, pos, Quaternion.identity, transform);
            chunks.Add(key, chunk);
        }
    }
}
