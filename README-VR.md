# TESUnity VR

TESUnity supports the Oculus Rift and OpenVR devices (HTC Vive, Windows Mixed Reality, etc.).

The game requires a lot of CPU and GPU to work at a good framerate. The following presets will help you to configure the game with your own hardware.

The Lightweight Scriptable Render Pipeline is the **best** option for VR.

### Maximum Performances

| Parameter | Values |
|-----------|---------|
| SunShadows  | `False` |
| LightShadows  | `False` |
| RenderExteriorCellLights | `False` |
| DayNightCycle | `False` |
| GenerateNormalMap | `False` |
|**Effects** | |
|AntiAliasing | `0` |
| PostProcessQuality | 0 |
|WaterBackSideTransparent | `False` |
|**Rendering** | |
| Shader  | `Unlit` |
| RenderPath  | `Lightweight` |
| SRPQuality | 0 |
| CameraFarClip | `150` |
| WaterQuality | `0` |
| RenderScale | `0.5` |

In the launcher start the game in `Fastest`.

### Mix between performances and quality
| Parameter | Values |
|-----------|---------|
| SunShadows  | `true` |
| LightShadows  | `False` |
| RenderExteriorCellLights | `true` |
| DayNightCycle | `true` |
| GenerateNormalMap | `true` |
|**Effects** | |
|AntiAliasing | `3` |
| PostProcessQuality | 2 |
|WaterBackSideTransparent | `False` |
|**Rendering** | |
| Shader  | `PBR` |
| RenderPath  | `Lightweight` |
| SRPQuality | 1 |
| CameraFarClip | `500` |
| WaterQuality | `0` |
| RenderScale | `1.0` |

In the launcher start the game in `Good` or `Beautiful`.