using UnityEngine;
using UnityEngine.Rendering;

namespace Ascendra.World
{
    /// Procedurally builds the explorable terrain the player spawns into.
    public static class WorldGenerator
    {
        public static Terrain Generate(WorldGenerationSettings settings)
        {
            TerrainData terrainData = new TerrainData
            {
                heightmapResolution = settings.HeightmapResolution,
                size = new Vector3(settings.WorldSize, settings.MaxHeight, settings.WorldSize)
            };

            terrainData.SetHeights(0, 0, BuildHeights(settings));

            GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
            terrainObject.name = "World Terrain";

            Terrain terrain = terrainObject.GetComponent<Terrain>();
            terrain.materialTemplate = CreateTerrainMaterial();

            return terrain;
        }

        // Terrain.CreateTerrainGameObject can pick a shader from the wrong render pipeline
        // (shows up as solid pink); pick the matching one explicitly instead of relying on auto-detect.
        private static Material CreateTerrainMaterial()
        {
            bool usingUrp = GraphicsSettings.currentRenderPipeline != null;
            string shaderName = usingUrp ? "Universal Render Pipeline/Terrain/Lit" : "Nature/Terrain/Standard";

            Shader shader = Shader.Find(shaderName) ?? Shader.Find("Standard") ?? Shader.Find("Diffuse");
            return shader != null ? new Material(shader) : null;
        }

        private static float[,] BuildHeights(WorldGenerationSettings settings)
        {
            int resolution = settings.HeightmapResolution;
            float[,] heights = new float[resolution, resolution];

            System.Random random = new System.Random(settings.Seed);
            Vector2 originOffset = new Vector2((float)random.NextDouble() * 10000f, (float)random.NextDouble() * 10000f);

            // Flat clearing radius (in heightmap cells) so the player spawns on level ground.
            float clearingRadius = resolution * settings.SpawnClearingRatio;
            Vector2 center = new Vector2(resolution * 0.5f, resolution * 0.5f);

            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float noise = FractalNoise(x, z, resolution, settings.Octaves, originOffset);

                    float distanceFromCenter = Vector2.Distance(new Vector2(x, z), center);
                    float clearingBlend = Mathf.Clamp01(distanceFromCenter / clearingRadius);

                    heights[z, x] = noise * clearingBlend;
                }
            }

            return heights;
        }

        private static float FractalNoise(int x, int z, int resolution, int octaves, Vector2 offset)
        {
            float amplitude = 1f;
            float frequency = 1f;
            float sum = 0f;
            float amplitudeSum = 0f;

            for (int i = 0; i < octaves; i++)
            {
                float sampleX = (x / (float)resolution) * frequency * 4f + offset.x;
                float sampleZ = (z / (float)resolution) * frequency * 4f + offset.y;

                sum += Mathf.PerlinNoise(sampleX, sampleZ) * amplitude;
                amplitudeSum += amplitude;

                amplitude *= 0.5f;
                frequency *= 2f;
            }

            return sum / amplitudeSum;
        }
    }

    [System.Serializable]
    public struct WorldGenerationSettings
    {
        public int Seed;
        public int HeightmapResolution;
        public float WorldSize;
        public float MaxHeight;
        public int Octaves;
        public float SpawnClearingRatio;

        public static WorldGenerationSettings Default => new WorldGenerationSettings
        {
            Seed = System.Environment.TickCount,
            HeightmapResolution = 513,
            WorldSize = 500f,
            MaxHeight = 60f,
            Octaves = 4,
            SpawnClearingRatio = 0.12f
        };
    }
}
