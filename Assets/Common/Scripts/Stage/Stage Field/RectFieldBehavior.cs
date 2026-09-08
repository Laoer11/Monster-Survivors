using OctoberStudio.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace OctoberStudio
{
    public class RectFieldBehavior : AbstractFieldBehavior
    {
        StageChunkBehavior chunk;

        List<Transform> borders = new List<Transform>();

        public override void Init(StageFieldData stageFieldData, bool spawnProp)
        {
            base.Init(stageFieldData, spawnProp);

            chunk = Object.Instantiate(stageFieldData.GetBackgroundPrefabs().Random()).GetComponent<StageChunkBehavior>();

            chunk.transform.position = Vector3.zero;
            chunk.transform.rotation = Quaternion.identity;
            chunk.transform.localScale = Vector3.one;

            if (stageFieldData.TopPrefab != null)
            {
                var topBorder = Object.Instantiate(stageFieldData.TopPrefab).GetComponent<Transform>();
                topBorder.transform.position = Vector3.up * chunk.Size.y;
                borders.Add(topBorder);
            }

            if (stageFieldData.BottomPrefab != null)
            {
                var bottomBorder = Object.Instantiate(stageFieldData.BottomPrefab).GetComponent<Transform>();
                bottomBorder.transform.position = Vector3.down * chunk.Size.y;
                borders.Add(bottomBorder);
            }

            if (stageFieldData.LeftPrefab != null)
            {
                var leftBorder = Object.Instantiate(stageFieldData.LeftPrefab).GetComponent<Transform>();
                leftBorder.transform.position = Vector3.left * chunk.Size.x;
                borders.Add(leftBorder);
            }

            if (stageFieldData.RightPrefab != null)
            {
                var rightBorder = Object.Instantiate(stageFieldData.RightPrefab).GetComponent<Transform>();
                rightBorder.transform.position = Vector3.right * chunk.Size.x;
                borders.Add(rightBorder);
            }

            if (stageFieldData.TopLeftPrefab != null)
            {
                var topLeftCorner = Object.Instantiate(stageFieldData.TopLeftPrefab).GetComponent<Transform>();
                topLeftCorner.transform.position = new Vector2(-chunk.Size.x, chunk.Size.y);
                borders.Add(topLeftCorner);
            }

            if (stageFieldData.TopRightPrefab != null)
            {
                var topRightCorner = Object.Instantiate(stageFieldData.TopRightPrefab).GetComponent<Transform>();
                topRightCorner.transform.position = new Vector2(chunk.Size.x, chunk.Size.y);
                borders.Add(topRightCorner);
            }

            if (stageFieldData.BottomLeftPrefab != null)
            {
                var bottomLeftCorner = Object.Instantiate(stageFieldData.BottomLeftPrefab).GetComponent<Transform>();
                bottomLeftCorner.transform.position = new Vector2(-chunk.Size.x, -chunk.Size.y);
                borders.Add(bottomLeftCorner);
            }

            if (stageFieldData.BottomRightPrefab != null)
            {
                var bottomRightCorner = Object.Instantiate(stageFieldData.BottomRightPrefab).GetComponent<Transform>();
                bottomRightCorner.transform.position = new Vector2(chunk.Size.x, -chunk.Size.y);
                borders.Add(bottomRightCorner);
            }

            SpawnProp(chunk);
        }

        public override void Update()
        {

        }

        public override bool ValidatePosition(Vector2 position)
        {
            if (position.x > chunk.transform.position.x + chunk.Size.x / 2) return false;
            if (position.x < chunk.transform.position.x - chunk.Size.x / 2) return false;

            if (position.y > chunk.transform.position.y + chunk.Size.y / 2) return false;
            if (position.y < chunk.transform.position.y - chunk.Size.y / 2) return false;

            return true;
        }

        public override Vector2 GetRandomPositionOnBorder()
        {
            Vector2 randomPoint = Random.insideUnitCircle.normalized * Mathf.Max(chunk.Size.x, chunk.Size.y);

            if (randomPoint.x > chunk.Size.x / 2) randomPoint.x = chunk.Size.x / 2;
            if (randomPoint.x < -chunk.Size.x / 2) randomPoint.x = -chunk.Size.x / 2;

            if (randomPoint.y > chunk.Size.y / 2) randomPoint.y = chunk.Size.y / 2;
            if (randomPoint.y < -chunk.Size.y / 2) randomPoint.y = -chunk.Size.y / 2;

            return randomPoint + chunk.transform.position.XY();
        }

        public override Vector2 GetBossSpawnPosition(BossFenceBehavior fence, Vector2 offset)
        {
            var playerPosition = PlayerBehavior.Player.transform.position.XY();
            var desiredPosition = playerPosition + offset;

            if (fence is CircleFenceBehavior circleFence)
            {
                bool tooCloseToLeftBorder = desiredPosition.x - circleFence.Radius < -chunk.Size.x / 2;
                bool tooCloseToRightBorder = desiredPosition.x + circleFence.Radius > chunk.Size.x / 2;

                if (tooCloseToLeftBorder && tooCloseToRightBorder) 
                {
                    desiredPosition.x = 0;
                } else if (tooCloseToLeftBorder)
                {
                    desiredPosition.x = -chunk.Size.x / 2 + circleFence.Radius;
                } else if (tooCloseToRightBorder)
                {
                    desiredPosition.x = chunk.Size.x / 2 - circleFence.Radius;
                }

                bool tooCloseToTopBorder = desiredPosition.y + circleFence.Radius > chunk.Size.y / 2;
                bool tooCloseToBottomBorder = desiredPosition.y - circleFence.Radius < -chunk.Size.y / 2;

                if (tooCloseToTopBorder && tooCloseToBottomBorder)
                {
                    desiredPosition.y = 0;
                }
                else if (tooCloseToTopBorder)
                {
                    desiredPosition.y = chunk.Size.y / 2 - circleFence.Radius;
                }
                else if (tooCloseToBottomBorder)
                {
                    desiredPosition.y = -chunk.Size.y / 2 + circleFence.Radius;
                }
            }
            else if (fence is RectFenceBehavior rectFence)
            {
                bool tooCloseToLeftBorder = desiredPosition.x - rectFence.Width / 2 < -chunk.Size.x / 2;
                bool tooCloseToRightBorder = desiredPosition.x + rectFence.Width / 2 > chunk.Size.x / 2;

                if (tooCloseToLeftBorder && tooCloseToRightBorder)
                {
                    desiredPosition.x = 0;
                }
                else if (tooCloseToLeftBorder)
                {
                    desiredPosition.x = -chunk.Size.x / 2 + rectFence.Width / 2;
                }
                else if (tooCloseToRightBorder)
                {
                    desiredPosition.x = chunk.Size.x / 2 - rectFence.Width / 2;
                }

                bool tooCloseToTopBorder = desiredPosition.y + rectFence.Height / 2 > chunk.Size.y / 2;
                bool tooCloseToBottomBorder = desiredPosition.y - rectFence.Height / 2 < -chunk.Size.y / 2;

                if (tooCloseToTopBorder && tooCloseToBottomBorder)
                {
                    desiredPosition.y = 0;
                }
                else if (tooCloseToTopBorder)
                {
                    desiredPosition.y = chunk.Size.y / 2 - rectFence.Height / 2;
                }
                else if (tooCloseToBottomBorder)
                {
                    desiredPosition.y = -chunk.Size.y / 2 + rectFence.Height / 2;
                }
            }

            return desiredPosition;
        }

        public override bool IsPointOutsideRight(Vector2 point, out float distance)
        {
            bool result = point.x > chunk.RightBound;
            distance = result ? point.x - chunk.RightBound : 0;
            return result;
        }

        public override bool IsPointOutsideLeft(Vector2 point, out float distance)
        {
            bool result = point.x < chunk.LeftBound;
            distance = result ? chunk.LeftBound - point.x : 0;
            return result;
        }

        public override bool IsPointOutsideTop(Vector2 point, out float distance)
        {
            bool result = point.y > chunk.TopBound;
            distance = result ? point.y - chunk.TopBound : 0;
            return result;
        }

        public override bool IsPointOutsideBottom(Vector2 point, out float distance)
        {
            bool result = point.y < chunk.BottomBound;
            distance = result ? chunk.BottomBound - point.y : 0;
            return result;
        }

        public override Vector2 GetIntersectionPoint(Vector2 start, Vector2 end, float offset)
        {
            var path = end - start;
            var distance = path.magnitude;

            // start == end
            if (distance < Mathf.Epsilon) return start;

            var direction = path.normalized;

            float tx1, tx2, ty1, ty2;

            // Create rectangle with offset
            var rect = new Rect(chunk.LeftBound + offset, chunk.BottomBound + offset, (chunk.Size.x - offset) * 2, (chunk.Size.y - offset) * 2);

            // X axis
            if (Mathf.Abs(path.x) > Mathf.Epsilon)
            {
                // Calculate intersection points
                tx1 = (rect.xMin - start.x) / path.x;
                tx2 = (rect.xMax - start.x) / path.x;

                // Swapping if needed
                if (tx1 > tx2) (tx1, tx2) = (tx2, tx1);
            }
            else
            {
                // No intersection
                tx1 = float.NegativeInfinity;
                tx2 = float.PositiveInfinity;
            }

            // Y axis
            if (Mathf.Abs(path.y) > Mathf.Epsilon)
            {
                // Calculate intersection points
                ty1 = (rect.yMin - start.y) / path.y;
                ty2 = (rect.yMax - start.y) / path.y;

                // Swapping if needed
                if (ty1 > ty2) (ty1, ty2) = (ty2, ty1);
            }
            else
            {
                // No intersection
                ty1 = float.NegativeInfinity;
                ty2 = float.PositiveInfinity;
            }

            var tEnter = Mathf.Max(tx1, ty1);
            var tExit = Mathf.Min(tx2, ty2);

            // No intersection
            if (tExit < 0f || tEnter > tExit) return start;

            return start + path * tExit;
        }

        public override void RemovePropFromBossFence(BossFenceBehavior fence)
        {
            chunk.RemovePropFromBossFence(fence);
        }

        public override void Clear()
        {
            Object.Destroy(chunk.gameObject);

            for(int i = 0; i < borders.Count; i++)
            {
                Object.Destroy(borders[i].gameObject);
            }

            borders.Clear();
        }
    }
}