using System.Drawing;

namespace MapGeneration.BLL.Services;

public class MapGenerationService: IMapGenerationService
{
    // biome colors
    private static readonly Color _biomeColorDesert = ColorTranslator.FromHtml("#e0dd80"); // hot and dry
    private static readonly Color _biomeColorPlains = ColorTranslator.FromHtml("#368733"); // mid temperature, mid humidity
    private static readonly Color _biomeColorSnow = ColorTranslator.FromHtml("#cadbed"); // cold and wet AND also replaces beach in cold zones (index 0)
    private static readonly Color _biomeColorTaiga = ColorTranslator.FromHtml("#164236"); // cold with mid humidity
    private static readonly Color _biomeColorColdlands = ColorTranslator.FromHtml("#374057"); // cold and dry
    private static readonly Color _biomeColorWarmlands = ColorTranslator.FromHtml("#688733"); // mid tempearture with low humidity
    private static readonly Color _biomeColorForest = ColorTranslator.FromHtml("#144715"); // mid temperature with high humidity
    private static readonly Color _biomeColorJungle = ColorTranslator.FromHtml("#022602"); // high temperature, high humidity
    private static readonly Color _biomeColorSavanna = ColorTranslator.FromHtml("#d6712d"); // high temperature, mid humidity
    private static readonly Color _biomeColorBeach = ColorTranslator.FromHtml("#b8b333"); // spawns before water level (below BEACH_GLOBAL_HEIGHT)
    private static readonly Color _biomeColorWater = ColorTranslator.FromHtml("#2d46b5"); // spawns below WATER_GLOBAL_HEIGHT
    private static readonly Color _biomeColorIce = ColorTranslator.FromHtml("#3c6c91"); // replaces water in cold zones (index 0) (also spawns below WATER_GLOBAL_HEIGHT)
    
    // more weirder = more noisier
    private const int WeirdHeightOctaves = 8;

    // less weirder = more smooth
    private const int NormalHeightOctaves = 3;

    // less octaves = more smooth and straight rivers
    private const int RiversOctaves = 6;

    // controls index 0 (0 <= index 0 < NOISE_LEVEL_LOW_HEIGHT)
    private const float NoiseLevelLowHeight = 0.333f;

    // controls index 1 (NOISE_LEVEL_LOW_HEIGHT <= index 0 < NOISE_LEVEL_MID_HEIGHT), NOISE_LEVEL_MID_HEIGHT < index 2 < 1
    private const float NoiseLevelMidHeight = 0.5f;

    // index 0, 1, 2
    private const int NoiseLevelLow = 0;
    private const int NoiseLevelMid = 1;
    private const int NoiseLevelHigh = 2;

    // controls world ocean height
    private const float WaterGlobalHeight = 0.0734f;

    private const float RiversGlobalHeight = WaterGlobalHeight - 0.035f;

    // controls beach height
    private const float BeachGlobalHeight = WaterGlobalHeight + 0.015f;

    // temperature-humidity biome table
    // temperature -->
    // humidity \/
    private readonly Color[,] _biomeTemperatureHumidityTable = {
            {_biomeColorColdlands, _biomeColorWarmlands, _biomeColorDesert},
            {_biomeColorTaiga, _biomeColorPlains, _biomeColorSavanna},
            {_biomeColorSnow, _biomeColorForest, _biomeColorJungle},
    };

    // controls affection of height on temperature, higher = colder
    private const float HeightOnTemperatureAffection = 0.73f;

    // controls affection of height on humidity, higher = dryer
    private const float HeightOnHumidityAffection = 0.35f;

    // rivers noise scale, means how often rivers will spawn
    private const float RiversScale = 0.25f;

    // weirdness noise scale, means how often weirdness will change
    private const float WeirdnessScale = 0.35f;

    // humidity noise scale, means how often humidity will change
    private const float HumidityScale = 0.25f;

    // temperature noise scale, means how often temperature will change
    private const float TemperatureScale = 0.125f;

    // controls thiccness of rivers
    private const float RiversThickness = 0.035f;

    // minimum size of image
    private const int MinImageSize = 256;

    // max image size multiplier
    private const int MaxLod = 5;

    // how many space of landscape we want to capture
    private const float MapScale = 10.0f;

    // light direction multiplied by -1, used for lighting calculation
    /*   \
     *    \
     *    _\|
     */
    private readonly float[] _inverseLightDirection = Normalize(new float[]{-0.5f, 0.5f, -0.5f}); // LIGHT_DIRECTION = {0.5f, -0.5f, 0.5f}

    // maximum amount of direct light
    private readonly float _directLightIntensity = 1.0f;

    // constant amount of light which will be received in any condition
    private readonly float _ambientLightIntensity = 0.4f;

    private INoiseGenerationService _heightGenerationService;
    private INoiseGenerationService _riversGenerationService;
    private INoiseGenerationService _temperatureGenerationService;
    private INoiseGenerationService _humidityGenerationService;
    private INoiseGenerationService _weirdnessGenerationService;

    public MapGenerationService(
        INoiseGenerationService heightGenerationService,
        INoiseGenerationService riversGenerationService,
        INoiseGenerationService temperatureGenerationService,
        INoiseGenerationService humidityGenerationService,
        INoiseGenerationService weirdnessGenerationService
        )
    {
        _heightGenerationService = heightGenerationService;
        _riversGenerationService = riversGenerationService;
        _temperatureGenerationService = temperatureGenerationService;
        _humidityGenerationService = humidityGenerationService;
        _weirdnessGenerationService = weirdnessGenerationService;
    }
    
    public Bitmap Generate(string seed, int resolution)
    {
        //new Color()
        SetSeeds(seed);
        Bitmap bitmap = new Bitmap(resolution, resolution);
        int[] pixels = GetPixels(resolution, resolution);
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                bitmap.SetPixel(x, y, Color.FromArgb(pixels[y * resolution + x]));
            }
        }
    }
    
    private void SetSeeds(String seed) {
        _riversGenerationService.SetSeed((seed + "_riversNoise").GetHashCode());
        _weirdnessGenerationService.SetSeed((seed + "_weirdnessNoise").GetHashCode());
        _heightGenerationService.SetSeed((seed + "_heightNoise").GetHashCode());
        _temperatureGenerationService.SetSeed((seed + "_temperatureNoise").GetHashCode());
        _humidityGenerationService.SetSeed((seed + "_humidityNoise").GetHashCode());
    }
    
    private int[] GetPixels(int width, int height) {
        int pixelsCount = width * height;
        int[] pixels = new int[pixelsCount];

        float deltaX = 1.0f / width;
        float deltaY = 1.0f / height;

        // iterate through pixels
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {

                // calculate float coords
                float u = x * deltaX * MapScale;
                float v = y * deltaY * MapScale;

                // get color
                int color = GetColor(u, v, width, height).ToArgb();
                pixels[x * height + y] = color;
            }
        }
        return pixels;
    }
    
    private Color GetColor(float u, float v, int width, int height) {
        // get height
        float mapHeight = GetHeight(u, v);

        // get temperature and apply height on temperature affection
        float temperature = GetTemperature(u, v) * (1 - mapHeight * HeightOnTemperatureAffection);

        // get humidity and apply height on humidity affection
        float humidity = GetHumidity(u, v) * (1 - mapHeight * HeightOnHumidityAffection);

        // get biome color
        Color biomeColor = GetBiomeColor(mapHeight, temperature, humidity);

        // don't apply light to water and ice beacuse it is flat
        if (biomeColor.Equals(_biomeColorIce) || biomeColor.Equals(_biomeColorWater)) {
            return biomeColor;
        }

        // calculate light
        float light = Saturate(GetLight(u, v, width, height));

        // apply light
        biomeColor = Color.FromArgb(
            (int) Math.Floor(biomeColor.R * light),
            (int) Math.Floor(biomeColor.G * light),
            (int) Math.Floor(biomeColor.B * light)
        );

        return biomeColor;
    }
    
    private float GetLight(float u, float v, int width, int height) {
        // calculate texel size
        float uOffset = 1.0f / width;
        float vOffset = 1.0f / height;

        // can be received from outside
        float center = GetHeight(u, v);

        // calculate neighbour heights
        float right = GetHeight(u + uOffset, v);
        float top = GetHeight(u, v + vOffset);

        // calculate height differences from center
        float rightDelta = right - center;
        float topDelta = top - center;

        // calculate normals
        float[] normalRight = Normalize(new[]{0, rightDelta, uOffset});
        float[] normalTop = Normalize(new[]{vOffset, topDelta, 0});

        // combine normals into one
        float[] normal = Normalize(new[]{
            normalRight[0] + normalTop[0],
            normalRight[1] + normalTop[1],
            normalRight[2] + normalTop[2],
        });

        // calculate amount of direct light
        // if normal equals INVERSE_LIGHT_DIRECTION then directLight will be DIRECT_LIGHT_INTENSITY
        // if normal equals -INVERSE_LIGHT_DIRECTION then directLight will be 0
        float directLight = (Dot(_inverseLightDirection, normal) * 0.5f + 0.5f) * _directLightIntensity;
        // add ambient light, this will prevent surface from going completly black
        float light = directLight + _ambientLightIntensity;
        return light;
    }
    
    
     // dot product of 2 3d vectors
    private static float Dot(float[] v1, float[] v2) {
        return v1[0] * v2[0]
                + v1[1] * v2[1]
                + v1[2] * v2[2];
    }

    // normalizes 3d vector
    private static float[] Normalize(float[] v) {
        float magnitude = (float) Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
        float multiplier = 1.0f / magnitude;
        return new[]{v[0] * multiplier, v[1] * multiplier, v[2] * multiplier};
    }

    private Color GetBiomeColor(float height, float temperature, float humidity) {
        // get temperature index
        int temperatureLevel = GetNoiseLevel(temperature);

        // get humidity index
        int humidityLevel = GetNoiseLevel(humidity);

        // water checks
        if (temperatureLevel > 0) {
            // water and beach
            if (height < WaterGlobalHeight) {
                return _biomeColorWater;
            }
            if (height < BeachGlobalHeight) {
                return _biomeColorBeach;
            }
        } else {
            // ice and snow
            if (height < WaterGlobalHeight) {
                return _biomeColorIce;
            }
            if (height < BeachGlobalHeight) {
                return _biomeColorSnow;
            }
        }

        // get biome with corresponding indexes from table
        Color biomeColor = _biomeTemperatureHumidityTable[humidityLevel, temperatureLevel];
        return biomeColor;
    }

    // helper function to avoid overflowing the values
    private static float Saturate(float a) {
        if (a > 1)
            return 1;
        if (a < 0)
            return 0;
        return a;
    }
    
    // linear interpolation
    private static float Lerp(float a, float b, float t) {
        return a + (b - a) * t;
    }

    // cosine curve
    private static float Cosine(float t) {
        return (float) ((1 - Math.Cos(t * Math.PI)) * 0.5);
    }

    private float GetRivers(float u, float v) {
        u *= RiversScale;
        v *= RiversScale;
        // sample noise
        float noise = _riversGenerationService.GetValue(u, v, RiversOctaves);
        // calculate rivers factor
        float deltaToMid = Math.Abs(0.5f - noise) * 2 / RiversThickness;
        return 1 - Saturate(deltaToMid);
    }

    private float getWeirdness(float u, float v) {
        u *= WeirdnessScale;
        v *= WeirdnessScale;
        // sample noise
        return _weirdnessGenerationService.GetValue(u, v);
    }

    private float GetHeight(float u, float v) {
        // sample weird variant
        float weirdHeight = _heightGenerationService.GetValue(u, v, WeirdHeightOctaves);
        // sample normal variant
        float normalHeight = _heightGenerationService.GetValue(u, v, NormalHeightOctaves);
        // rivers factor
        float rivers = GetRivers(u, v);

        // combine weird and normal variant according to weirdness factor
        float height = Lerp(normalHeight, weirdHeight, Cosine(getWeirdness(u, v)));

        // apply rivers factor
        // cosines makes transitions nicer
        return Lerp(height * height * height, RiversGlobalHeight, Cosine(rivers));
    }

    //sample temperature noise
    private float GetTemperature(float u, float v) {
        u *= TemperatureScale;
        v *= TemperatureScale;
        float temperature = _temperatureGenerationService.GetValue(u, v);
        return temperature;
    }

    // sample humidity noise
    private float GetHumidity(float u, float v) {
        u *= HumidityScale;
        v *= HumidityScale;
        float humidity = _humidityGenerationService.GetValue(u, v);
        return humidity;
    }

    // calculate index
    private static int GetNoiseLevel(float height) {
        if (height < NoiseLevelLowHeight) {
            return NoiseLevelLow;
        }
        if (height < NoiseLevelMidHeight) {
            return NoiseLevelMid;
        }
        return NoiseLevelHigh;
    }
}