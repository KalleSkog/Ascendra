using UnityEngine;
using UnityEngine.Rendering;

namespace Ascendra.Core
{
    /// Picks a shader compatible with whichever render pipeline is currently active,
    /// so runtime-created objects never show up as pink (shader/pipeline mismatch).
    public static class PipelineMaterial
    {
        public static Material CreateLit(Color color)
        {
            bool usingUrp = GraphicsSettings.currentRenderPipeline != null;
            string shaderName = usingUrp ? "Universal Render Pipeline/Lit" : "Standard";

            Shader shader = Shader.Find(shaderName) ?? Shader.Find("Standard") ?? Shader.Find("Diffuse");
            Material material = shader != null ? new Material(shader) : null;
            if (material != null)
            {
                material.color = color;
            }

            return material;
        }
    }
}
