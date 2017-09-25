using System;
using System.Collections.Generic;
using System.Text;

namespace WiimoteVR
{
    class AffineTransformSolver
    {
        public static float computeAngle(float dx, float dy)
        {
            float angle = 0;
            if (dx == 0)
            {
                angle = (float)Math.PI / 2;
            }
            else
            {
                if (dx > 0)
                    return (float)Math.Atan(dy / dx);
                else
                    angle = (float)(Math.Atan(dy / dx) + Math.PI);
            }
            return angle;
        }

        public static float[,] solve2Dto3x3(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            float scale = distance(x3, x4, y3, y4) / distance(x1, x2, y1, y2);
            float theta = computeAngle((x4 - x3), (y4 - y3)) - computeAngle((x2 - x1), (y2 - y1));

            float tx1 = (x2 + x1) / 2;
            float ty1 = (y2 + y1) / 2;

            float tx2 = (x4 + x3) / 2;
            float ty2 = (y4 + y3) / 2;

            float[,] result = new float[3, 3];
            result[2, 2] = 1.0f;

            result[0, 0] = (float)(scale * Math.Cos(theta));
            result[1, 0] = -(float)(scale * Math.Sin(theta));
            result[2, 0] = -tx1 * result[0, 0] - ty1 * result[1, 0] + tx2;

            result[0, 1] = (float)(scale * Math.Sin(theta));
            result[1, 1] = (float)(scale * Math.Cos(theta));
            result[2, 1] = -tx1 * result[0, 1] - ty1 * result[1, 1] + ty2;
            return result;
        }

        public static float distance(float x1, float x2, float y1, float y2)
        {
            float dx = x1- x2;
            float dy = y1-y2;
            return (float)Math.Sqrt(dx*dx+dy*dy);
        }

        public static float[,] solve2Dto4x4(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            float scale = distance(x3,x4,y3,y4) / distance(x1,x2,y1,y2);
            float theta = computeAngle((x4 - x3), (y4 - y3)) - computeAngle((x2 - x1), (y2 - y1));

            float tx1 = (x2 + x1) / 2;
            float ty1 = (y2 + y1) / 2;

            float tx2 = (x4 + x3) / 2;
            float ty2 = (y4 + y3) / 2;

            float [,] result = new float[4,4];
            result[0, 0] = 1.0f;
            result[1, 1] = 1.0f;
            result[2, 2] = 1.0f;
            result[3, 3] = 1.0f;

            result[0,0] = (float)(scale * Math.Cos(theta));
            result[1,0] = -(float)(scale * Math.Sin(theta));
            result[3,0] = -tx1 * result[0,0] - ty1 * result[1,0] + tx2;

            result[0,1] = (float)(scale * Math.Sin(theta));
            result[1,1] = (float)(scale * Math.Cos(theta));
            result[3,1] = -tx1 * result[0,1] - ty1 * result[1,1] + ty2;
            return result;
        }
    }
}
