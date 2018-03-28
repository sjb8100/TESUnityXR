using UnityEngine;
using ur = UnityEngine.Rendering;

namespace TESUnity
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class HDMaterial : MWBaseMaterial
    {
        private Material m_HDMaterial;
        private Material m_CutoutMaterial;

        public HDMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            m_HDMaterial = Resources.Load<Material>("Materials/HD-Standard");
            m_CutoutMaterial = Resources.Load<Material>("Materials/HD-Cutout");
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

                if (mp.textures.mainFilePath != null)
                {
                    var texture = m_textureManager.LoadTexture(mp.textures.mainFilePath);
                    material.SetTexture("_BaseColorMap", texture);

                    if (TESManager.instance.generateNormalMap)
                        material.SetTexture("_NormalMap", GenerateNormalMap((Texture2D)texture, TESManager.instance.normalGeneratorIntensity));
                }

                //if (mp.textures.bumpFilePath != null)
                    //material.SetTexture("_NormalMap", m_textureManager.LoadTexture(mp.textures.bumpFilePath));

                m_existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            return new Material(Shader.Find("HDRenderPipeline/Lit"));
        }

        public override Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            Material material = BuildMaterialTested();
            //material.SetInt("_SrcBlend", (int)sourceBlendMode);
            //material.SetInt("_DstBlend", (int)destinationBlendMode);
            return material;
        }

        public override Material BuildMaterialTested(float cutoff = 0.5f)
        {
            Material material = BuildMaterial();
            material.CopyPropertiesFromMaterial(m_CutoutMaterial);
            //material.SetFloat("_AlphaCutoff", cutoff);
            return material;
        }
    }
}