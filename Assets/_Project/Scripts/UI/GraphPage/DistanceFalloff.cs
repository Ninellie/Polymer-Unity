using System;

namespace UI.DevicePage
{
    public static class DistanceFalloff
    {
        [Serializable]
        public enum Mode
        {
            Linear,
            Smoothstep,
            Perlin,
            Exp,
            Gaussian,
            SoftInvSquare
        }

        public static float Get(float R, float r, float F0, Mode mode)
        {
            return mode switch
            {
                Mode.Linear         => Linear(R, r, F0),
                Mode.Smoothstep     => Smoothstep(R, r, F0),
                Mode.Perlin         => Perlin(R, r, F0),
                Mode.Exp            => Exp(R, r, F0),
                Mode.Gaussian       => Gaussian(R, r, F0),
                Mode.SoftInvSquare  => SoftInvSquare(R, r, F0),
                _                   => 0f
            };
        }

        private static float Linear(float R, float r, float F0)
        {
            if (r >= R) return 0f;
            return F0 * (1f - r / R);
        }

        private static float Smoothstep(float R, float r, float F0)
        {
            if (r >= R) return 0f;
            float x = r / R;
            float f = 1f - (3f * x * x - 2f * x * x * x);
            return F0 * f;
        }

        private static float Perlin(float R, float r, float F0)
        {
            if (r >= R) return 0f;
            float x = r / R;
            float t = 6f * x * x * x * x * x - 15f * x * x * x * x + 10f * x * x * x;
            float f = 1f - t;
            return F0 * f;
        }

        private static float Exp(float R, float r, float F0)
        {
            if (r >= R) return 0f;
            float k = 3f / R;
            float f = UnityEngine.Mathf.Exp(-k * r);
            return F0 * f;
        }

        private static float Gaussian(float R, float r, float F0)
        {
            if (r >= R) return 0f;
            float sigma = R * 0.5f;
            float f = UnityEngine.Mathf.Exp(-(r * r) / (2f * sigma * sigma));
            return F0 * f;
        }

        private static float SoftInvSquare(float R, float r, float F0)
        {
            if (r >= R) return 0f;
            float eps = R * 0.2f;
            float f = 1f / (1f + (r * r) / (eps * eps));
            return F0 * f;
        }
    }
}