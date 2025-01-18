using BepInEx;
using BepInEx.Unity.Mono;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

using UnityEngine;

namespace TalesOfGracesFFix
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class ToGFFix : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        // Features
        public static ConfigEntry<bool> bFixAspectRatio;
        public static ConfigEntry<int> iMSAASamples;
        public static ConfigEntry<bool> bDepthOfField;

        // Aspect Ratio
        private const float fNativeAspect = (float)16 / 9;
        public static float fAspectRatio;
        public static float fAspectMultiplier;

        private void Awake()
        {
            // Plugin startup logic
            Log = base.Logger;
            Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} is loaded!");


            // Graphical tweaks
            iMSAASamples = Config.Bind("Graphical Tweaks",
                                "MSAA Samples",
                                1,
                                new ConfigDescription("Set number of MSAA samples. 1 = off. Note that enabling MSAA will disable depth of field.",
                                new AcceptableValueList<int>(1, 2, 4, 8, 16)));

            bDepthOfField = Config.Bind("Graphical Tweaks",
                                "Depth Of Field",
                                true,
                                "Enable or disable depth of field.");

            // Aspect ratio
            bFixAspectRatio = Config.Bind("Ultrawide/Narrower",
                                "Fix Aspect Ratio",
                                true,
                                "Removes pillarboxing/letterboxing and centers the UI at 16:9.");

            if (bFixAspectRatio.Value)
                Harmony.CreateAndPatchAll(typeof(AspectRatioPatches));

            if (!bDepthOfField.Value || iMSAASamples.Value >1)
                Harmony.CreateAndPatchAll(typeof(GraphicsPatches));
        }

        [HarmonyPatch]
        public class AspectRatioPatches
        {
            // Calculate aspect ratio
            [HarmonyPatch(typeof(Screen), nameof(Screen.SetResolution), [typeof(int), typeof(int), typeof(FullScreenMode)])]
            [HarmonyPostfix]
            public static void GetCurrentResolution(ref int __0, ref int __1, ref FullScreenMode __2)
            {
                fAspectRatio = (float)__0 / __1;
                fAspectMultiplier = fAspectRatio / fNativeAspect;

                Log.LogInfo($"Current Resolution: {__0}x{__1}");
                Log.LogInfo($"Current Resolution: Aspect Ratio: {fAspectRatio}");
                Log.LogInfo($"Current Resolution: Aspect Multiplier: {fAspectMultiplier}");
            }

            // Camera rect/aspect ratio
            [HarmonyPatch(typeof(Noble.CameraManager), nameof(Noble.CameraManager.SetCameraViewportRect))]
            [HarmonyPostfix]
            public static void ResetCameraRect(Noble.CameraManager __instance)
            {
                __instance.mCamera.rect = new Rect(0f, 0f, 1f, 1f);
                __instance.mCamera.ResetAspect();
            }

            // Set UI ortho matrix for 16:9
            [HarmonyPatch(typeof(Noble.PrimitiveManager), nameof(Noble.PrimitiveManager.CalcUIOrthoMatrix))]
            [HarmonyPostfix]
            public static void FixHUD(Noble.PrimitiveManager __instance)
            {
                if (fAspectRatio > fNativeAspect)
                    __instance.m_Viewport = new Rect(0f, 0f, (float)Screen.width / fAspectMultiplier, (float)Screen.height);
                else if (fAspectRatio < fNativeAspect)
                    __instance.m_Viewport = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height * fAspectMultiplier);
            }
        }

        [HarmonyPatch]
        public class GraphicsPatches
        {
            // Adjust graphical settings
            [HarmonyPatch(typeof(UnityEngine.Rendering.Volume), nameof(UnityEngine.Rendering.Volume.OnEnable))]
            [HarmonyPostfix]
            public static void GraphicalTweaks(UnityEngine.Rendering.Volume __instance)
            {
                // Get current render pipeline
                var urpAsset = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;

                // MSAA
                if (iMSAASamples.Value > 1 && urpAsset)
                {
                    urpAsset.msaaSampleCount = iMSAASamples.Value;
                    Log.LogInfo($"Graphical Tweaks: Set MSAA sample count to {iMSAASamples.Value}.");
                }

                // Depth of field
                __instance.profile.TryGet(out NoblePostEffectDepthOfFieldParam dof);
                if (dof && (!bDepthOfField.Value || iMSAASamples.Value > 1))
                {
                    dof.active = false;
                    Log.LogInfo($"Graphical Tweaks: Disabled depth of field on volume {__instance.gameObject.name}.");
                }
            }
        }
    }
}