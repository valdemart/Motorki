using Microsoft.Xna.Framework;
using System;

namespace Motorki.GameClasses
{
    public static class Utils
    {
        public static Vector2 Perpendicular(this Vector2 vec)
        {
            return new Vector2(-vec.Y, vec.X);
        }

        public static float Dot(this Vector2 vec, Vector2 vector)
        {
            return vec.X * vector.X + vec.Y * vector.Y;
        }

        public static Vector2 Normalized(this Vector2 vec)
        {
            return new Vector2(vec.X / vec.Length(), vec.Y / vec.Length());
        }

        /// <summary>
        /// calculates coordinates of an image of a point on a line
        /// </summary>
        public static Vector2 Cast(Vector2 linePoint, Vector2 lineNormal, Vector2 point)
        {
            float Ck = -Vector2.Dot(lineNormal, linePoint);
            float Cl = -Vector2.Dot(new Vector2(lineNormal.Y, -lineNormal.X), point);

            float x = (-Ck * lineNormal.X - Cl * lineNormal.Y);
            float y = (-Ck * lineNormal.Y + Cl * lineNormal.X);

            return new Vector2(x, y);
        }

        /// <summary>
        /// calculates a distance between a line and a point in specified direction
        /// </summary>
        public static float DirectionalCastLength(Vector2 linePoint, Vector2 lineNormal, Vector2 point, Vector2 direction)
        {
            float Ck = -Vector2.Dot(lineNormal, linePoint);
            float Cl = -Vector2.Dot(new Vector2(direction.Y, -direction.X), point);

            float x = (-Ck * direction.X - Cl * lineNormal.Y) / Vector2.Dot(lineNormal, direction);
            float y = (-Ck * direction.Y + Cl * lineNormal.X) / Vector2.Dot(lineNormal, direction);
            return (point - (new Vector2(x, y))).Length();
        }

        /// <summary>
        /// calculates a distance between a line and a point
        /// </summary>
        public static float DistanceFromLine(Vector2 linePoint, Vector2 lineNormal, Vector2 point)
        {
            return (point - Cast(linePoint, lineNormal, point)).Length();
        }

        /// <summary>
        /// calculates a distance between a line segment and a point. If point image is not placed on the line segment, a distance to a closer end of a segment is returned
        /// </summary>
        public static float DistanceFromLineSegment(Vector2 segmentEnd1, Vector2 segmentEnd2, Vector2 lineNormal, Vector2 point)
        {
            Vector2 segmentCenter = (segmentEnd1 + segmentEnd2) / 2;

            float distCenterLimit = (segmentCenter - segmentEnd1).Length();
            float distCenter = (segmentCenter - Cast(segmentEnd1, lineNormal, point)).Length();

            float distEnd1 = (segmentEnd1 - point).Length();
            float distEnd2 = (segmentEnd2 - point).Length();

            float distSegment = DistanceFromLine(segmentEnd1, lineNormal, point);

            if (distCenter > distCenterLimit)
                return Math.Min(distEnd1, distEnd2);
            else
                return distSegment;
        }

        public static bool TestLineAndPointCollision(Vector2[] linePoints, Vector2 lineNormal, Vector2 point, Vector2 pointDirection, out Vector2 pointReverseDisplacement)
        {
            pointReverseDisplacement = Vector2.Zero;

            if (Vector2.Dot(pointDirection, lineNormal) >= 0)
                return false;
            Vector2 mid = (linePoints[0] + linePoints[1]) / 2;
            float range = (mid - linePoints[0]).Length();
            
            Vector2 cast = Utils.Cast(linePoints[0], lineNormal, point);

            if ((cast - mid).Length() > range)
                return false;
            Vector2 _ = point - cast;
            _.Normalize();
            if (Vector2.Dot(_, lineNormal) > 0)
                return false;
            pointReverseDisplacement = lineNormal * (point - cast).Length();
            if (pointReverseDisplacement.Length() > 0)
                return true;
            return false;
        }

        /// <summary>
        /// note: line facing vector will be rotated clockwise against vector starting in start point and pointing end point
        /// </summary>
        public static Vector2 CalculateLineFacing(Vector2 start, Vector2 end)
        {
            Vector2 _ = new Vector2(start.Y - end.Y, end.X - start.X);
            _.Normalize();
            return _;
        }

        public static Vector2 CalculateDirectionVector(float radians)
        {
            Vector2 dirVec = Vector2.Transform(new Vector2(0, -1), Matrix.CreateRotationZ(radians));
            dirVec.Normalize();
            return dirVec;
        }

        static float Deg2Rad = (float)(System.Math.PI / 180.0f);
        static float Rad2Deg = (float)(180.0f / System.Math.PI);

        public static float ToRadians(this float degrees)
        {
            return degrees * Deg2Rad;
        }

        public static float ToDegrees(this float radians)
        {
            return radians * Rad2Deg;
        }

        /// <summary>
        /// returns -1 if float is &lt;0, 1 if float is &gt;0, 0 if float is ==0
        /// </summary>
        public static int Sign(this float f)
        {
            return (f > 0 ? 1 : (f < 0 ? -1 : 0));
        }
    }
}
