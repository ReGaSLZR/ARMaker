using UnityEngine;

namespace ARMarker
{
    /// <summary>
    /// NOTE: Please refine and revamp this code soon. 
    /// Right now, it's at this dirty/prototype state to save time for prototyping.
    /// </summary>
    public class LayerRotationCircleView : MonoBehaviour
    {
        public Axis axis;
        private LineRenderer lr;
        private int segments = 64;
        private float radius;
        private float thickness;

        public static LayerRotationCircleView Create(Axis axis, Color color, float radius, float thickness)
        {
            var obj = new GameObject($"RotationCircle_{axis}");
            var circle = obj.AddComponent<LayerRotationCircleView>();
            circle.axis = axis;
            circle.radius = radius;
            circle.thickness = thickness;

            var lr = obj.AddComponent<LineRenderer>();
            circle.lr = lr;
            lr.useWorldSpace = false;
            lr.loop = true;
            lr.positionCount = circle.segments;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lr.endColor = color;
            lr.startWidth = lr.endWidth = thickness;
            lr.sortingOrder = 100;

            return circle;
        }

        public void UpdatePosition(Vector3 worldPos, Transform spriteTransform)
        {
            transform.position = worldPos;

            Quaternion rotation = axis switch
            {
                Axis.X => Quaternion.Euler(0, 90, 0),
                Axis.Y => Quaternion.Euler(90, 0, 0),
                Axis.Z => Quaternion.identity,
                _ => Quaternion.identity,
            };
            transform.rotation = rotation;

            Vector3[] points = new Vector3[segments];
            for (int i = 0; i < segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                points[i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            }

            lr.SetPositions(points);
        }

        public float DistanceToScreen(Vector2 screenPos, Camera cam)
        {
            float closestDist = float.MaxValue;

            for (int i = 0; i < lr.positionCount; i++)
            {
                Vector3 worldPoint = transform.TransformPoint(lr.GetPosition(i));
                Vector2 screenPoint = cam.WorldToScreenPoint(worldPoint);
                float dist = Vector2.Distance(screenPos, screenPoint);
                if (dist < closestDist) closestDist = dist;
            }

            return closestDist;
        }
    }
}
