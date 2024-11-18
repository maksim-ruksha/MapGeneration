namespace MapGeneration.BLL.Services;

public interface INoiseGenerationService
{
    public void SetSeed(long seed);
    public void SetScale(float scale);
    public float GetValue(float u, float v);
    public float GetValue(float u, float v, int octaves);
    public float GetValue(float u, float v, int octaves, float persistence);
}