using Microsoft.Xna.Framework;

namespace Motorki.GameClasses
{
    public class Utils
    {
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
    }
}
