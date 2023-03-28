using MathUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtility;

public class NearbySensor : MonoBehaviour
{
    [Serializable] public class CollisionSet<TC, T> : IEnumerable<TC>
        where TC : class, ICollection<T>, new()
        where T : class
    {
        public TC Enter;
        public TC Stay;
        public TC Exit;

        public CollisionSet()
        {
            Enter = new TC();
            Stay = new TC();
            Exit = new TC();
        }

        public IEnumerator<TC> GetEnumerator()
        {
            yield return Enter;
            yield return Stay;
            yield return Exit;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TC this[int index]
        {
            get =>
                index switch
                {
                    0 => Enter,
                    1 => Stay,
                    2 => Exit,
                    _ => null
                };
            set => this[index] = value;
        }

        public int Length = 3;

        /// <param name="select">Ignores Null results</param>
        public void AddWhere<TD, D>(CollisionSet<TD, D> target, Func<D, T> select)
            where TD : class, ICollection<D>, new()
            where D : class
        {
            for (int index = 0; index < Length; index++)
            {
                var grabArray = this[index];
                var colliderArray = target[index];

                foreach (var collider in colliderArray)
                {
                    var grab = select.Invoke(collider);
                    if (grab == null)
                        continue;
                    grabArray.Add(grab);
                }
            }
        }
    }

    [Serializable] public class ColliderSet : CollisionSet<List<Collider2D>, Collider2D> { }

    [Serializable] public class ColliderBuffer
    {
        public string Name = "Objects";
        public float Radius = 5f;
        public ContactFilter2D Filter;
        public ColliderSet ColliderSet = new();
    }

    [SerializeField] private bool drawGizmos = false;

    public List<ColliderBuffer> buffers = new();

    private readonly List<Collider2D> CurrentDetected = new();

    public event EventHandler Updated;

    private void Update()
    {
        Vector2 startPos = transform.position;

        for (var index = 0; index < buffers.Count; index++)
        {
            ColliderBuffer objectBuffer = buffers[index];

            // Fill CurrentDetected
            Physics2D.OverlapCircle(startPos, objectBuffer.Radius, objectBuffer.Filter, CurrentDetected);

            // Exit = in Stay except in current
            objectBuffer.ColliderSet.Exit = objectBuffer.ColliderSet.Stay.Except(CurrentDetected).ToList();

            // Enter = in current but not in Stay
            objectBuffer.ColliderSet.Enter = CurrentDetected.Except(objectBuffer.ColliderSet.Stay).ToList();

            // Set StayColliders
            objectBuffer.ColliderSet.Stay = CurrentDetected.ToList();

            // Empty CurrentDetected
            CurrentDetected.Clear();
        }

        Updated?.Invoke(this, EventArgs.Empty);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        Vector3 startPos = transform.position;

        for (var index = 0; index < buffers.Count; index++)
        {
            ColliderBuffer colliderBuffer = buffers[index];

            // Color
            // float t = math.unlerp(0f, ColliderBuffers.Count - 1, index);
            float t = (float) index / buffers.Count;
            Gizmos.color = Color.HSVToRGB(t, .8f, .8f);

            // Range
            GizmosExtensions.DrawWireCircle(startPos, colliderBuffer.Radius,
                36, Quaternion.LookRotation(Vector3.up, Vector3.forward));

            // Lines
            for (var i = 0; i < colliderBuffer.ColliderSet.Stay.Count; i++)
            {
                Collider2D nearbyCollider = colliderBuffer.ColliderSet.Stay[i];
                Gizmos.DrawLine(startPos,
                    nearbyCollider
                        .ClosestPoint(startPos)
                        .To3DXY(startPos.z));
            }
        }
    }
}