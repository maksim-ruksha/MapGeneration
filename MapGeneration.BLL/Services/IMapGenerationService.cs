using System.Drawing;

namespace MapGeneration.BLL.Services;

public interface IMapGenerationService
{
    public Bitmap Generate(string seed, int resolution);
}