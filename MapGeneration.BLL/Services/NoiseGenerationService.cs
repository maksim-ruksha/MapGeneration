namespace MapGeneration.BLL.Services;

public class NoiseGenerationService : INoiseGenerationService
{
    private long _seed = 0;
    private float _scale = 1.0f;

    private readonly float[][] _vertexVectors =
    {
        new float[] { 1, 0 },
        new float[] { 0, 1 },
        new float[] { -1, 0 },
        new float[] { 0, -1 },
    };

    public void SetSeed(long seed)
    {
        _seed = seed;
    }

    public void SetScale(float scale)
    {
        _scale = scale;
    }

    public float GetValue(float x, float y)
    {
        return GetSimpleNoise(x, y);
    }

    public float GetValue(float x, float y, int octaves)
    {
        return GetValue(x, y, octaves, 0.5f);
    }

    public float GetValue(float x, float y, int octaves, float persistence)
    {
        float noise = 0.0f;

        float totalWeight = 0.0f;
        float scaledX = x;
        float scaledY = y;
        float currentNoiseWeight = 1.0f;

        for (int i = 0; i < octaves; i++) {
            // add octave noise
            noise += GetSimpleNoise(scaledX, scaledY) * currentNoiseWeight;

            // collect weight
            totalWeight += currentNoiseWeight;

            // make next octave weaker
            currentNoiseWeight *= persistence;

            // increase noise scale
            scaledX *= 2;
            scaledY *= 2;
        }

        // normalize noise value
        noise /= totalWeight;

        return noise;
    }

    private float GetSimpleNoise(float x, float y)
    {
        // scale coords
        x *= _scale;
        y *= _scale;

        // calculate cell vertices coords
        int cellStartX = (int)Math.Floor(x);
        int cellStartY = (int)Math.Floor(y);
        int cellEndX = cellStartX + 1;
        int cellEndY = cellStartY + 1;

        // calculate cell-space coords
        float localX = x - cellStartX;
        float localY = y - cellStartY;

        // get pseudo random vectors to calculate dot product with
        // x is pointing right
        // y is pointing up
        float[] downLeftVector = GetVertexVector(cellStartX, cellStartY);
        float[] downRightVector = GetVertexVector(cellEndX, cellStartY);
        float[] upLeftVector = GetVertexVector(cellStartX, cellEndY);
        float[] upRightVector = GetVertexVector(cellEndX, cellEndY);

        // calculate vectors pointing to point inside cell from vertices
        float[] fromDownLeft = { localX, localY };
        float[] fromDownRight = { localX - 1, localY };
        float[] fromUpLeft = { localX, localY - 1 };
        float[] fromUpRight = { localX - 1, localY - 1 };

        // get dot products of vertices
        float downLeftDot = Dot(downLeftVector, fromDownLeft);
        float downRightDot = Dot(downRightVector, fromDownRight);
        float upLeftDot = Dot(upLeftVector, fromUpLeft);
        float upRightDot = Dot(upRightVector, fromUpRight);

        // interpolate between dot products based on cell-space coords
        float downDot = Lerp(downLeftDot, downRightDot, localX);
        float upDot = Lerp(upLeftDot, upRightDot, localX);

        // final result
        float noise = Lerp(downDot, upDot, localY);
        // arrange noise from 0 to 1
        noise = noise * 0.5f + 0.5f;

        return noise;
    }

    // linear interpolation
    private float Lerp(float a, float b, float t)
    {
        return a + (b - a) * CosineCurve(t);
    }

    // makes noise less blocky
    private float CosineCurve(float t)
    {
        return (float)((1 - Math.Cos(t * Math.PI)) * 0.5);
    }

    private float[] Normalize(float[] v)
    {
        float magnitude = (float)Math.Sqrt(v[0] * v[0] + v[1] * v[1]);
        return new float[] { v[0] / magnitude, v[1] / magnitude };
    }

    // dot product of two-dimensional vectors
    private float Dot(float[] v1, float[] v2)
    {
        return v1[0] * v2[0] + v1[1] * v2[1];
    }

    // get random vector based on coords
    private float[] GetVertexVector(int x, int y)
    {
        // inserting seed here makes output vectors seed-dependent
        int vectorIndex = (int)Math.Abs(Hash(Hash(x) + Hash(y) + Hash(_seed + x * y)) & 3);
        return _vertexVectors[vectorIndex];
    }

    // hash
    private long Hash(long a)
    {
        a ^= (a << 13);
        a ^= (a >>> 17);
        a ^= (a << 5);
        return a;
    }
}