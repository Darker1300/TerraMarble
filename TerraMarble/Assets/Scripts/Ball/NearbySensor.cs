using MathUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtility;

public class NearbySensor : MonoBehaviour
{
    [Serializable]
    public class CollisionSet<TC, T> : IEnumerable<TC>
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
            set => this[index] ??= value;
        }

        public int Length = 3;

        /// <param name="select">Ignores Null results</param>
        public void AddWhere<TD, D>(CollisionSet<TD, D> target, Func<D, T> select)
            where TD : class, ICollection<D>, new()
            where D : class
        {
            for (int index = 0; index < Length; index++)
            {
                var currentArray = this[index];
                var targetArray = target[index];

                foreach (var collider in targetArray)
                {
                    var selectResult = select.Invoke(collider);
                    if (selectResult == null)
                        continue;
                    currentArray.Add(selectResult);
                }
            }
        }
    }

    [Serializable] public class ColliderSet : CollisionSet<HashSet<Collider2D>, Collider2D> { }

    [Serializable] public class ColliderBuffer
    {
        public string Name = "Objects";
        public float Radius = 5f;
        public ContactFilter2D Filter;
        public ColliderSet ColliderSet = new();
    }

    [SerializeField] private bool drawGizmos = false;

    public List<ColliderBuffer> Buffers = new();

    private readonly List<Collider2D> tempDetected = new();
    private readonly HashSet<Collider2D> currentDetected = new();

    public event Action Updated;

    private void Update()
    {
        Vector2 startPos = transform.position;

        foreach (var objectBuffer in Buffers)
        {
            // Fill tempDetected List
            Physics2D.OverlapCircle(startPos, objectBuffer.Radius, objectBuffer.Filter, tempDetected);

            // Move data from List into currentDetected HashSet
            currentDetected.Clear();
            currentDetected.EnsureCapacity(tempDetected.Count);
            currentDetected.UnionWith(tempDetected);
            tempDetected.Clear();

            // Exit = in previous Stay but not in current
            objectBuffer.ColliderSet.Exit.Clear();
            foreach (var stayCollider in objectBuffer.ColliderSet.Stay)
                if (!currentDetected.Contains(stayCollider))
                    objectBuffer.ColliderSet.Exit.Add(stayCollider);

            // Enter = in current but not in previous Stay
            objectBuffer.ColliderSet.Enter.Clear();
            foreach (var currentCollider in currentDetected)
                if (!objectBuffer.ColliderSet.Stay.Contains(currentCollider))
                    objectBuffer.ColliderSet.Enter.Add(currentCollider);

            // Set StayColliders
            objectBuffer.ColliderSet.Stay.Clear();
            foreach (var collider in currentDetected)
                objectBuffer.ColliderSet.Stay.Add(collider);

            // Empty CurrentDetected
            currentDetected.Clear();
        }

        Updated?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        Vector3 startPos = transform.position;

        for (var index = 0; index < Buffers.Count; index++)
        {
            ColliderBuffer colliderBuffer = Buffers[index];

            // Color
            float t = (float)index / Buffers.Count;
            Gizmos.color = Color.HSVToRGB(t, .8f, .8f);

            // Range
            GizmosExtensions.DrawWireCircle(startPos, colliderBuffer.Radius,
                36, Quaternion.LookRotation(Vector3.up, Vector3.forward));

            // Lines
            foreach (var stayCollider in colliderBuffer.ColliderSet.Stay)
            {
                Gizmos.DrawLine(startPos,
                    stayCollider
                        .ClosestPoint(startPos)
                        .To3DXY(startPos.z));
            }
        }
    }
}