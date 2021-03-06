using System.Collections.Generic;
using UnityEngine;
using ur = UnityEngine.Rendering;

namespace TESUnity
{
    public enum MatTestMode { Always, Less, LEqual, Equal, GEqual, Greater, NotEqual, Never }

    public struct MWMaterialTextures
    {
        public string mainFilePath;
        public string darkFilePath;
        public string detailFilePath;
        public string glossFilePath;
        public string glowFilePath;
        public string bumpFilePath;
    }

    public struct MWMaterialProps
    {
        public MWMaterialTextures textures;
        public bool alphaBlended;
        public ur.BlendMode srcBlendMode;
        public ur.BlendMode dstBlendMode;
        public bool alphaTest;
        public float alphaCutoff;
        public bool zWrite;
    }

    /// <summary>
    /// Manages loading and instantiation of Morrowind materials.
    /// </summary>
    public class MaterialManager
    {
        private BaseMaterial _mwMaterial;
        private TextureManager _textureManager;

        public TextureManager TextureManager
        {
            get { return _textureManager; }
        }

        public MaterialManager(TextureManager textureManager)
        {
            _textureManager = textureManager;

            var tes = TESManager.instance;

            // Order is important
            if (tes.renderPath == TESManager.RendererType.LightweightRP && tes.materialType != TESManager.MWMaterialType.Unlit)
            {
                _mwMaterial = new LightweightMaterial(textureManager);
            }
            else if (tes.renderPath == TESManager.RendererType.HDRP && tes.materialType != TESManager.MWMaterialType.Unlit)
            {
                _mwMaterial = new HDMaterial(textureManager);
            }
            else
            {
                switch (tes.materialType)
                {
                    case TESManager.MWMaterialType.PBR:
                        _mwMaterial = new StandardMaterial(textureManager);
                        break;
                    case TESManager.MWMaterialType.Unlit:
                        _mwMaterial = new UnliteMaterial(textureManager);
                        break;
                    default:
                        _mwMaterial = new DiffuseMaterial(textureManager);
                        break;
                }
            }
        }

        public Material BuildMaterialFromProperties(MWMaterialProps mp)
        {
            return _mwMaterial.BuildMaterialFromProperties(mp);
        }
    }
}