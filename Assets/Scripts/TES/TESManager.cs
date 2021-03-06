﻿using System;
using System.IO;
using TESUnity.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Experimental.Rendering.LightweightPipeline;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.Water;

namespace TESUnity
{
    public class TESManager : MonoBehaviour
    {
        public static TESManager instance;

        public enum MWMaterialType
        {
            PBR, StandardLighting, Unlit
        }

        public enum PostProcessingQuality
        {
            None = 0, Low, Medium, High
        }

        public enum SRPQuality
        {
            Low, Medium, High
        }

        public enum RendererType
        {
            Forward, Deferred, LightweightRP, HDRP
        }

        public const string Version = "0.9.0";

        #region Inspector-set Members

#if UNITY_EDITOR
        [Header("Editor Only")]
        [SerializeField]
        public bool _bypassINIConfig = false;
        [SerializeField]
        private bool _allowChangeCellInRealtime = false;
#endif

        [Header("Global")]
        public string dataPath;
        public string alternativeDataPath;
        public bool useKinematicRigidbodies = true;
        public bool playMusic = false;
        public bool enableLog = false;
        public Water.WaterMode waterQuality = Water.WaterMode.Simple;
        public bool useStaticBatching = false;

        [Header("Optimizations")]
        public int cellRadius = 4;
        public int cellDetailRadius = 3;
        public int cellRadiusOnLoad = 2;

        [Header("Rendering")]
        public MWMaterialType materialType = MWMaterialType.StandardLighting;
        public RendererType renderPath = RendererType.Forward;
        public float cameraFarClip = 500.0f;
        public SRPQuality srpQuality = SRPQuality.Medium;
        public LightweightRenderPipelineAsset[] lightweightAssets;
        public float renderScale = 1.0f;

        [Header("HDRP")]
        public HDRenderPipelineAsset[] hdrpAssets;
        public GameObject volumeSettings;

        [Header("Lighting")]
        public float ambientIntensity = 1.5f;
        public bool renderSunShadows = false;
        public bool renderLightShadows = false;
        public bool renderExteriorCellLights = false;
        public bool animateLights = false;
        public bool dayNightCycle = false;
        public bool generateNormalMap = true;
        public float normalGeneratorIntensity = 0.75f;

        [Header("Effects")]
        public PostProcessingQuality postProcessingQuality = PostProcessingQuality.High;
        public PostProcessLayer.Antialiasing antiAliasing = PostProcessLayer.Antialiasing.TemporalAntialiasing;
        public bool waterBackSideTransparent = false;

        [Header("VR")]
        public bool followHeadDirection = false;
        public bool directModePreview = true;
        public bool roomScale = true;
        public bool forceControllers = false;
        public bool useXRVignette = false;

        [Header("UI")]
        public UIManager UIManager;
        public Sprite UIBackgroundImg;
        public Sprite UICheckmarkImg;
        public Sprite UIDropdownArrowImg;
        public Sprite UIInputFieldBackgroundImg;
        public Sprite UIKnobImg;
        public Sprite UIMaskImg;
        public Sprite UISpriteImg;

        [Header("Prefabs")]
        public GameObject playerPrefab;
        public GameObject waterPrefab;

        [Header("Debug")]
        public bool creaturesEnabled = false;
        public bool npcsEnabled = false;

        #endregion

        private MorrowindDataReader MWDataReader = null;
        private MorrowindEngine MWEngine = null;
        private MusicPlayer musicPlayer = null;

        public MorrowindEngine Engine
        {
            get { return MWEngine; }
        }

        public TextureManager TextureManager
        {
            get { return MWEngine.textureManager; }
        }

        private void Awake()
        {
            Debug.unityLogger.logEnabled = enableLog;

            instance = this;

            var path = dataPath;

#if UNITY_EDITOR
            if (!_bypassINIConfig)
                path = GameSettings.CheckSettings(this);
#else
            path = GameSettings.CheckSettings(this);
#endif

            if (!GameSettings.IsValidPath(path))
            {
                if (GameSettings.IsValidPath(alternativeDataPath))
                {
                    dataPath = alternativeDataPath;
                    return;
                }

                GameSettings.SetDataPath(string.Empty);
                UnityEngine.SceneManagement.SceneManager.LoadScene("AskPathScene");
            }
            else
            {
                dataPath = path;
            }
        }

        private void Start()
        {
            if (renderPath == RendererType.LightweightRP)
            {
                var asset = lightweightAssets[(int)srpQuality];
                asset.renderScale = renderScale;
                GraphicsSettings.renderPipelineAsset = asset;

                // Only this mode is compatible with SRP.
                waterQuality = Water.WaterMode.Simple;
            }
            else if (renderPath == RendererType.HDRP)
            {
                var asset = hdrpAssets[(int)srpQuality];
                GraphicsSettings.renderPipelineAsset = asset;

                // Only this mode is compatible with SRP.
                waterQuality = Water.WaterMode.Simple;

                Instantiate(volumeSettings);
            }
            else
                GraphicsSettings.renderPipelineAsset = null;

            if (UIManager == null)
            {
                UIManager = FindObjectOfType<UIManager>();

                if (UIManager == null)
                    throw new UnityException("UI Manager is missing");
            }

            CellManager.cellRadius = cellRadius;
            CellManager.detailRadius = cellDetailRadius;
            MorrowindEngine.cellRadiusOnLoad = cellRadiusOnLoad;

            MWDataReader = new MorrowindDataReader(dataPath);
            MWEngine = new MorrowindEngine(MWDataReader, UIManager);

            if (playMusic)
            {
                // Start the music.
                musicPlayer = new MusicPlayer();

                foreach (var songFilePath in Directory.GetFiles(dataPath + "/Music/Explore"))
                {
                    if (!songFilePath.Contains("Morrowind Title"))
                    {
                        musicPlayer.AddSong(songFilePath);
                    }
                }

                musicPlayer.Play();
            }

            // Spawn the player.
            //MWEngine.SpawnPlayerInside(playerPrefab, new Vector2i(4537908, 1061158912), new Vector3(0.8f, -0.45f, -1.4f));

            MWEngine.SpawnPlayerOutside(playerPrefab, new Vector2i(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
        }

        private void OnDestroy()
        {
            if (MWDataReader != null)
            {
                MWDataReader.Close();
                MWDataReader = null;
            }

#if UNITY_EDITOR
            if (renderPath == RendererType.LightweightRP)
            {
                var asset = lightweightAssets[(int)srpQuality];
                asset.renderScale = 1.0f;
            }
#endif
        }

        private void Update()
        {
            MWEngine.Update();

            if (playMusic)
            {
                musicPlayer.Update();
            }

#if UNITY_EDITOR
            if (_allowChangeCellInRealtime)
            {
                CellManager.cellRadius = cellRadius;
                CellManager.detailRadius = cellDetailRadius;
                MorrowindEngine.cellRadiusOnLoad = cellRadiusOnLoad;
            }
#endif
        }

        private void TestAllCells(string resultsFilePath)
        {
            using (StreamWriter writer = new StreamWriter(resultsFilePath))
            {
                foreach (var record in MWDataReader.MorrowindESMFile.GetRecordsOfType<ESM.CELLRecord>())
                {
                    var CELL = (ESM.CELLRecord)record;

                    try
                    {
                        var cellInfo = MWEngine.cellManager.StartInstantiatingCell(CELL);
                        MWEngine.temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

                        DestroyImmediate(cellInfo.gameObject);

                        writer.Write("Pass: ");
                    }
                    catch (Exception)
                    {
                        writer.Write("Fail: ");
                    }

                    if (!CELL.isInterior)
                    {
                        writer.WriteLine(CELL.gridCoords.ToString());
                    }
                    else
                    {
                        writer.WriteLine(CELL.NAME.value);
                    }

                    writer.Flush();
                }
            }
        }
    }
}