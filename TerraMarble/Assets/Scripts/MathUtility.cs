using UnityEngine;

namespace MathUtility
{
    public static class MathU
    {
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(
                Mathf.Cos(radian),
                Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
            => RadianToVector2(degree * Mathf.Deg2Rad);

        public static float Vector2ToRadian(Vector2 direction)
        {
            return Mathf.Atan2(
                       direction.y,
                       direction.x);
        }

        public static float Vector2ToDegree(Vector2 direction)
            => Vector2ToRadian(direction) * Mathf.Rad2Deg;

        public static Vector2 RotatedByRadian(this Vector2 v, float radian)
        {
            float ca = Mathf.Cos(radian);
            float sa = Mathf.Sin(radian);
            float rx = v.x * ca - v.y * sa;

            return new Vector2(rx, v.x * sa + v.y * ca);
        }

        public static Vector2 RotatedByDegree(this Vector2 v, float degree)
            => RotatedByRadian(v, degree * Mathf.Deg2Rad);

        /// <summary>
        ///   <para>Same as Lerp but makes sure the values interpolate correctly when they wrap around 360 degrees.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static float LerpAngleUnclamped(float a, float b, float t)
        {
            float num = Mathf.Repeat(b - a, 360f);
            if ((double)num > 180.0)
                num -= 360f;
            return a + num * t;
        }
    }
}
