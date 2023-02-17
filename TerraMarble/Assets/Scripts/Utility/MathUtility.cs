using System;
using UnityEngine;

namespace MathUtility
{
    public static class MathU
    {
        public static Vector2 RadianToVector2(float radian)
        {
            return new(
                Mathf.Cos(radian),
                Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        public static float Vector2ToRadian(Vector2 direction)
        {
            return Mathf.Atan2(
                direction.y,
                direction.x);
        }

        public static float Vector2ToDegree(Vector2 direction)
        {
            return Vector2ToRadian(direction) * Mathf.Rad2Deg;
        }

        public static Vector2 RotatedByRadian(this Vector2 v, float radian)
        {
            var ca = Mathf.Cos(radian);
            var sa = Mathf.Sin(radian);
            var rx = v.x * ca - v.y * sa;

            return new Vector2(rx, v.x * sa + v.y * ca);
        }

        public static Vector2 RotatedByDegree(this Vector2 v, float degree)
        {
            return RotatedByRadian(v, degree * Mathf.Deg2Rad);
        }

        /// <summary>
        ///   <para>Same as Lerp but makes sure the values interpolate correctly when they wrap around 360 degrees.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static float LerpAngleUnclamped(float a, float b, float t)
        {
            var num = Mathf.Repeat(b - a, 360f);
            if ((double)num > 180.0)
                num -= 360f;
            return a + num * t;
        }


        public static float InverseLerpAngle(float a, float b, float t)
        {
            float angBetween =  DeltaRange(a, b, 360f);
            b = a + angBetween; // remove any a->b discontinuity
            float h = a + angBetween * 0.5f; // halfway angle
            t = h + DeltaRange(h, t, 360f); // get offset from h, and offset by h
            return Mathf.InverseLerp(a, b, t);
        }


        /// <summary>
        ///   <para>Calculates the shortest difference between two given numbers in a repeating range.</para>
        /// </summary>
        public static float DeltaRange(float current, float target, float max)
        {
            var num = Mathf.Repeat(target - current, max);
            if ((double)num > max * .5f)
                num -= max;
            return num;
        }

        public static int Repeat(int current, int min, int max)
        {
            int range = Math.Abs(min) + Math.Abs(max);
            while (current < min) current += range;
            while (current > max) current -= range;
            return current;
        }

        //  /// <summary>
        //  ///   <para>Calculates the shortest difference between two given numbers in a repeating range.</para>
        //  /// </summary>
        //  /// <param name="current"></param>
        //  /// <param name="target"></param>
        //  /// <param name="max"></param>
        //  public static int DeltaRange(int current, int target, int max)
        //  {
        //      float num = (target - current) % (max + 1);
        //      if ((float)num > max * 0.5f)
        //          num -= max;
        //      return Mathf.RoundToInt(num);
        //  }

        /// <summary>
        ///   <para>Same as MoveTowards but makes sure the values interpolate correctly when they wrap around 360 degrees.</para>
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDelta"></param>
        public static float MoveTowardsRange(float current, float target, float maxDelta, float maxRange)
        {
            var num = DeltaRange(current, target, maxRange);
            if (-(double)maxDelta < (double)num && (double)num < (double)maxDelta)
                return target;
            target = current + num;
            return Mathf.MoveTowards(current, target, maxDelta);
        }

        /// <returns>World-space vector starting from position to target's position</returns>
        public static Vector3 Towards(this Transform self, Transform target)
        {
            return target.position - self.position;
        }

        public static Vector3 Towards(this Vector3 self, Vector3 target)
        {
            return target - self;
        }

        public static Vector2 Towards(this Vector2 self, Vector2 target)
        {
            return target - self;
        }


        public static Vector2 RotatedAround(this Vector2 self, Vector2 pivotPoint, float degree)
        {
            return (Vector2)(Quaternion.AngleAxis(degree, Vector3.forward) * (self - pivotPoint))
                   + pivotPoint;
        }

        public static void RotateAround(this Transform self, Vector3 pivotPoint, Quaternion rot)
        {
            self.position = rot * (self.position - pivotPoint) + pivotPoint;
            self.rotation = rot * self.rotation;
        }

        public static void MoveRotateAround(this Rigidbody2D self, Vector2 pivotPoint, float rotation)
        {
            Quaternion rot = Quaternion.AngleAxis(rotation, Vector3.forward);
            Vector2 delta = (Vector2)(rot * (self.position - pivotPoint)) + pivotPoint;
            self.MovePosition(delta);
            self.MoveRotation(Mathf.Repeat(self.rotation + rotation, 360f));
        }

        public static void AddRotateAroundForce(this Rigidbody2D self, Vector2 pivotPoint, float rotation)
        {
            self.MoveRotation(Mathf.Repeat(self.rotation + rotation, 360f));
            Vector2 deltaV = self.position.RotatedAround(pivotPoint, rotation) - self.position;
            self.velocity += deltaV;
        }
        
        /// <param name="rotateSmoothTime">eg 0.1</param>
        public static Quaternion SmoothDampRotation(Quaternion current, Quaternion target, ref float velocity, float rotateSmoothTime)
        {
            float delta = Quaternion.Angle(current, target);
            if (delta > 0f)
            {
                float t = Mathf.SmoothDampAngle(delta, 0.0f, ref velocity, rotateSmoothTime);
                t = 1.0f - (t / delta);
                current = Quaternion.Slerp(current, target, t);
            }

            return current;
        }
    }
}