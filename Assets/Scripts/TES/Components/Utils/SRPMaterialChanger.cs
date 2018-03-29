using UnityEngine;

namespace TESUnity.Components.Utilities
{
    [RequireComponent(typeof(Renderer))]
    public sealed class SRPMaterialChanger : MonoBehaviour
    {
        private void Start()
        {
            var renderer = GetComponent<Renderer>();
            var material = renderer.sharedMaterial;
#if UNITY_EDITOR
            // To prevent the script to change the material in the editor.
            material = renderer.material;
#endif
            var tes = TESManager.instance;
            var shaderName = "Unlit/Texture";

            if (tes.materialType != TESManager.MWMaterialType.Unlit)
            {
                if (tes.renderPath == TESManager.RendererType.LightweightSRP)
                {
                    var pbr = TESManager.instance.materialType == TESManager.MWMaterialType.PBR;
                    shaderName = string.Format("LightweightPipeline/Standard ({0})", (pbr ? "Physically Based" : "Simple Lighting"));
                }
                else if (tes.renderPath == TESManager.RendererType.HDSRP)
                    shaderName = "HDRenderPipeline/Lit";
            }

            var shader = Shader.Find(shaderName);

            if (material.shader != shader)
                material.shader = shader;
        }

        private void OnDestroy()
        {
            
        }
    }
}
