using UnityEngine;
using ur = UnityEngine.Rendering;

namespace TESUnity
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class LightweightMaterial : MWBaseMaterial
    {
        private Material m_StandardPBR;
        private Material m_StandardSimple;
        private Material m_CutoutPBRMaterial;
        private Material m_CutoutSimpleMaterial;

        public LightweightMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            m_StandardPBR = Resources.Load<Material>("Materials/Lightweight-PBR");
            m_StandardSimple = Resources.Load<Material>("Materials/Lightweight-Simple");
            m_CutoutPBRMaterial = Resources.Load<Material>("Materials/Lightweight-PBR-Cutout");
            m_CutoutSimpleMaterial = Resources.Load<Material>("Materials/Lightweight-Simple-Cutout");
        }

        public override Material BuildMaterialFromProperties(MWMaterialProps mp)
        {
            Material material;

            //check if the material is already cached
            if (!m_existingMaterials.TryGetValue(mp, out material))
            {
                //otherwise create a new material and cache it
                if (mp.alphaBlended)
                    material = BuildMaterialBlended(mp.srcBlendMode, mp.dstBlendMode);
                else if (mp.alphaTest)
                    material = BuildMaterialTested(mp.alphaCutoff);
                else
                    material = BuildMaterial();

                material.SetFloat("_Glossiness", 0.25f);

                if (mp.textures.mainFilePath != null)
                {
                    material.mainTexture = m_textureManager.LoadTexture(mp.textures.mainFilePath);

                    if (TESManager.instance.generateNormalMap)
                        material.SetTexture("_BumpMap", GenerateNormalMap((Texture2D)material.mainTexture, TESManager.instance.normalGeneratorIntensity));
                }

                if (mp.textures.bumpFilePath != null)
                    material.SetTexture("_BumpMap", m_textureManager.LoadTexture(mp.textures.bumpFilePath));

                m_existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            var pbr = TESManager.instance.materialType == TESManager.MWMaterialType.PBR;
            var material = new Material(Shader.Find(string.Format("LightweightPipeline/Standard ({0})", (pbr ? "Physically Based" : "Simple Lighting"))));
            material.CopyPropertiesFromMaterial(pbr ? m_StandardPBR : m_StandardSimple);
            return material;
        }

        public override Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            Material material = BuildMaterialTested();
            material.SetInt("_SrcBlend", (int)sourceBlendMode);
            material.SetInt("_DstBlend", (int)destinationBlendMode);
            return material;
        }

        public override Material BuildMaterialTested(float cutoff = 0.5f)
        {
            var material = BuildMaterial();
            var pbr = TESManager.instance.materialType == TESManager.MWMaterialType.PBR;
            material.CopyPropertiesFromMaterial(pbr ? m_CutoutPBRMaterial : m_CutoutSimpleMaterial);
            material.SetFloat("_Cutout", cutoff);
            return material;
        }
    }
}