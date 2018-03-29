using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace TESUnity
{
    public class GameSettings
    {
        private static readonly string ConfigFile = "config.ini";
        private static readonly string MWDataPathName = "MorrowindDataPath";

        public static void SaveValue(string parameter, string value)
        {
            var lines = File.ReadAllLines(ConfigFile);

            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(parameter))
                {
                    lines[i] = string.Format("{0} = {1}", parameter, value);
                    break;
                }
            }

            File.WriteAllLines(ConfigFile, lines);
        }

        public static bool IsValidPath(string path)
        {
            return File.Exists(Path.Combine(path, "Morrowind.esm"));
        }

        public static void SetDataPath(string dataPath)
        {
            SaveValue(MWDataPathName, dataPath);
        }

        public static string GetDataPath()
        {
            if (!File.Exists("config.ini"))
                return string.Empty;

            var lines = File.ReadAllLines(ConfigFile);
            foreach (var line in lines)
            {
                if (line.Contains(MWDataPathName))
                {
                    var tmp = line.Split('=');
                    if (tmp.Length == 2)
                        return tmp[1].Trim();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Checks if a file named Config.ini is located left to the main executable.
        /// Open/Parse it and configure default values.
        /// </summary>
        public static string CheckSettings(TESManager tes)
        {
            var path = string.Empty;
            var lines = File.ReadAllLines(ConfigFile);
            var temp = new string[2];
            var value = string.Empty;

            foreach (var line in lines)
            {
                temp = line.Split('=');

                if (temp.Length == 2)
                {
                    value = temp[1].Trim();

                    switch (temp[0].Trim())
                    {
                        case "AntiAliasing":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result >= 0 && result < 4)
                                        tes.antiAliasing = (PostProcessLayer.Antialiasing)result;
                                }
                            }
                            break;
                        case "PostProcessQuality":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result >= 0 && result < 4)
                                        tes.postProcessingQuality = (TESManager.PostProcessingQuality)result;
                                }
                            }
                            break;
                        case "AnimateLights": ParseBool(value, ref tes.animateLights); break;
                        case "MorrowindDataPath": path = value; break;
                        case "FollowHeadDirection": ParseBool(value, ref tes.followHeadDirection); break;
                        case "SunShadows":ParseBool(value, ref tes.renderSunShadows); break;
                        case "LightShadows": ParseBool(value, ref tes.renderLightShadows); break;
                        case "PlayMusic": ParseBool(value, ref tes.playMusic); break;
                        case "RenderExteriorCellLights": ParseBool(value, ref tes.renderExteriorCellLights); break;
                        case "WaterBackSideTransparent": ParseBool(value, ref tes.waterBackSideTransparent); break;
                        case "RenderPath":
                            if (value == "Forward")
                                tes.renderPath = TESManager.RendererType.Forward;
                            else if (value == "Deferred")
                                tes.renderPath = TESManager.RendererType.Deferred;
                            else if (value == "Lightweight")
                                tes.renderPath = TESManager.RendererType.LightweightSRP;
                            break;
                        case "Shader":
                            switch (value)
                            {
                                case "Default": tes.materialType = TESManager.MWMaterialType.Default; break;
                                case "PBR": tes.materialType = TESManager.MWMaterialType.PBR; break;
                                case "Unlit": tes.materialType = TESManager.MWMaterialType.Unlit; break;
                                default: tes.materialType = TESManager.MWMaterialType.StandardLighting; break;
                            }
                            break;
                        case "RoomScale": ParseBool(value, ref tes.roomScale); break;
                        case "ForceControllers": ParseBool(value, ref tes.forceControllers); break;
                        case "CreaturesEnabled": ParseBool(value, ref tes.creaturesEnabled); break;
                        case "CameraFarClip": ParseFloat(value, ref tes.cameraFarClip, 5); break;
                        case "XRVignette": ParseBool(value, ref tes.useXRVignette); break;
                        case "WaterQuality":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result > -1 && result < 3)
                                        tes.waterQuality = (UnityStandardAssets.Water.Water.WaterMode)result;
                                }
                            }
                            break;

                        case "DayNightCycle": ParseBool(value, ref tes.dayNightCycle); break;
                        case "GenerateNormalMap": ParseBool(value, ref tes.generateNormalMap); break;
                        case "NormalGeneratorIntensity": ParseFloat(value, ref tes.normalGeneratorIntensity); break;
                        case "RenderScale": ParseFloat(value, ref tes.renderScale, 0.1f, 4.0f); break;
                        case "SRPQuality":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result > -1 && result < 3)
                                        tes.srpQuality = (TESManager.SRPQuality)result;
                                }
                            }
                            break;

                        case "CellRadius": ParseInt(value, ref tes.cellRadius, 1); break;
                        case "CellDetailRadius": ParseInt(value, ref tes.cellDetailRadius, 1); break;
                        case "CellRadiusOnLoad": ParseInt(value, ref tes.cellRadiusOnLoad, 1); break;
                        default: break;
                    }
                }
            }

            return path;
        }

        public static void CreateConfigFile()
        {
            var sb = new StringBuilder();
            sb.Append("# TESUnity Configuration File\r\n");
            sb.Append("\r\n");

            sb.Append("[Global]\r\n");
            sb.Append("PlayMusic = True\r\n");
            sb.Append(string.Format("{0} = \r\n", MWDataPathName));
            sb.Append("\r\n");

            sb.Append("[Rendering]\r\n");
            sb.Append("RenderPath = Deferred\r\n");
            sb.Append("Shader = PBR\r\n");
            sb.Append("CameraFarClip = 500\r\n");
            sb.Append("WaterQuality = 0\r\n");
            sb.Append("\r\n");

            sb.Append("[Lighting]\r\n");
            sb.Append("AnimateLights = True\r\n");
            sb.Append("SunShadows = True\r\n");
            sb.Append("LightShadows = False\r\n");
            sb.Append("RenderExteriorCellLights = True\r\n");
            sb.Append("DayNightCycle = True\r\n");
            sb.Append("GenerateNormalMap = True\r\n");
            sb.Append("NormalGeneratorIntensity = 0.75\r\n");
            sb.Append("\r\n");

            sb.Append("[Effects]\r\n");
            sb.Append("AntiAliasing = True\r\n");
            sb.Append("PostProcessQuality = 3\r\n");
            sb.Append("WaterBackSideTransparent = False\r\n");
            sb.Append("\r\n");

            sb.Append("[VR]\r\n");
            sb.Append("FollowHeadDirection = True\r\n");
            sb.Append("RoomScale = False\r\n");
            sb.Append("ForceControllers = True\r\n");
            sb.Append("XRVignette = False\r\n");
            sb.Append("RenderScale = 1.0\r\n");
            sb.Append("\r\n");

            sb.Append("[Debug]\r\n");
            sb.Append("CreaturesEnabled = False\r\n");

            File.WriteAllText(ConfigFile, sb.ToString());
        }

        private static bool ParseBool(string value, bool defaultValue)
        {
            var val = value.ToLower();
            if (val == "true" || val == "1")
                return true;
            else if (val == "false" || value == "0")
                return false;

            return defaultValue;
        }

        private static void ParseBool(string key, ref bool value)
        {
            var val = key.ToLower();
            if (val == "true" || val == "1")
                value = true;
            else if (val == "false" || key == "0")
                value = false;
        }

        private static int ParseInt(string value, int defaultValue)
        {
            int result;

            if (int.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        private static void ParseInt(string key, ref int value, int? min = null, int? max = null)
        {
            int result;

            if (int.TryParse(key, out result))
            {
                var valid = true;

                if (min.HasValue)
                    valid &= result >= min;

                if (max.HasValue)
                    valid &= result <= max;

                if (valid)
                    value = result;
            }
        }

        private static float ParseFloat(string value, float defaultValue)
        {
            float result;

            if (float.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        private static void ParseFloat(string key, ref float value, float? min = null, float? max = null)
        {
            float result;

            if (float.TryParse(key, out result))
            {
                var valid = true;

                if (min.HasValue)
                    valid &= result >= min;

                if (max.HasValue)
                    valid &= result <= max;

                if (valid)
                    value = result;
            }
        }
    }
}
