using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dancer.Modules.Components
{

    public class MaterialControllerComponents
    {
        public class HGControllerFinder : MonoBehaviour
        {
            public Renderer Renderer;

            public Material[] Materials;

            public void Start()
            {
                Renderer = base.gameObject.GetComponent<Renderer>();
                if ((bool)Renderer)
                {
                    Materials = Renderer.materials;
                    Material[] materials = Materials;
                    foreach (Material material in materials)
                    {
                        if ((bool)material)
                        {
                            switch (material.shader.name)
                            {
                                case "Hopoo Games/Deferred/Standard":
                                    {
                                        HGStandardController hGStandardController = base.gameObject.AddComponent<HGStandardController>();
                                        hGStandardController.Material = material;
                                        hGStandardController.Renderer = Renderer;
                                        break;
                                    }
                                case "Hopoo Games/Deferred/Snow Topped":
                                    {
                                        HGSnowToppedController hGSnowToppedController = base.gameObject.AddComponent<HGSnowToppedController>();
                                        hGSnowToppedController.Material = material;
                                        hGSnowToppedController.Renderer = Renderer;
                                        hGSnowToppedController.name = material.name + "(HGSnowTopped) Controller";
                                        break;
                                    }
                                case "Hopoo Games/Deferred/Triplanar Terrain Blend":
                                    {
                                        HGTriplanarTerrainBlend hGTriplanarTerrainBlend = base.gameObject.AddComponent<HGTriplanarTerrainBlend>();
                                        hGTriplanarTerrainBlend.Material = material;
                                        hGTriplanarTerrainBlend.Renderer = Renderer;
                                        break;
                                    }
                                case "Hopoo Games/FX/Distortion":
                                    {
                                        HGDistortionController hGDistortionController = base.gameObject.AddComponent<HGDistortionController>();
                                        hGDistortionController.Material = material;
                                        hGDistortionController.Renderer = Renderer;
                                        break;
                                    }
                                case "Hopoo Games/FX/Solid Parallax":
                                    {
                                        HGSolidParallaxController hGSolidParallaxController = base.gameObject.AddComponent<HGSolidParallaxController>();
                                        hGSolidParallaxController.Material = material;
                                        hGSolidParallaxController.Renderer = Renderer;
                                        break;
                                    }
                                case "Hopoo Games/FX/Cloud Remap":
                                    {
                                        HGCloudRemapController hGCloudRemapController = base.gameObject.AddComponent<HGCloudRemapController>();
                                        hGCloudRemapController.Material = material;
                                        hGCloudRemapController.Renderer = Renderer;
                                        break;
                                    }
                                case "Hopoo Games/FX/Cloud Intersection Remap":
                                    {
                                        HGIntersectionController hGIntersectionController = base.gameObject.AddComponent<HGIntersectionController>();
                                        hGIntersectionController.Material = material;
                                        hGIntersectionController.Renderer = Renderer;
                                        break;
                                    }
                            }
                        }
                    }
                }
                UnityEngine.Object.Destroy(this);
            }
        }

        public class HGBaseController : MonoBehaviour
        {
            public Material Material;

            public Renderer Renderer;

            public GameObject OwnerGameObject;

            public virtual string ShaderName { get; set; }

            public void Start()
            {
                OwnerGameObject = base.gameObject;
            }

            public void Update()
            {
                if (!Material || !Renderer)
                {
                    UnityEngine.Object.Destroy(this);
                }
                if (((bool)Renderer && (bool)Material && !Material.shader.name.Contains(ShaderName)) || ((bool)Renderer && Renderer.gameObject != OwnerGameObject))
                {
                    HGControllerFinder hGControllerFinder = Renderer.gameObject.AddComponent<HGControllerFinder>();
                    hGControllerFinder.Renderer = Renderer;
                    UnityEngine.Object.Destroy(this);
                }
            }
        }

        public class HGStandardController : HGBaseController
        {
            public enum _RampInfoEnum
            {
                TwoTone = 0,
                SmoothedTwoTone = 1,
                Unlitish = 3,
                Subsurface = 4,
                Grass = 5
            }

            public enum _DecalLayerEnum
            {
                Default,
                Environment,
                Character,
                Misc
            }

            public enum _CullEnum
            {
                Off,
                Front,
                Back
            }

            public enum _PrintDirectionEnum
            {
                BottomUp = 0,
                TopDown = 1,
                BackToFront = 3
            }

            public override string ShaderName => "Hopoo Games/Deferred/Standard";

            public bool _EnableCutout
            {
                get
                {
                    return Material?.IsKeywordEnabled("CUTOUT") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "CUTOUT");
                }
            }

            public Color _Color
            {
                get
                {
                    return Material?.GetColor("_Color") ?? default(Color);
                }
                set
                {
                    Material?.SetColor("_Color", value);
                }
            }

            public Texture _MainTex
            {
                get
                {
                    return Material?.GetTexture("_MainTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_MainTex", value);
                }
            }

            public Vector2 _MainTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_MainTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_MainTex", value);
                }
            }

            public Vector2 _MainTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_MainTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_MainTex", value);
                }
            }

            public float _NormalStrength
            {
                get
                {
                    return Material?.GetFloat("_NormalStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_NormalStrength", Mathf.Clamp(value, 0f, 5f));
                }
            }

            public Texture _NormalTex
            {
                get
                {
                    return Material?.GetTexture("_NormalTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_NormalTex", value);
                }
            }

            public Vector2 _NormalTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_NormalTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_NormalTex", value);
                }
            }

            public Vector2 _NormalTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_NormalTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_NormalTex", value);
                }
            }

            public Color _EmColor
            {
                get
                {
                    return Material?.GetColor("_EmColor") ?? default(Color);
                }
                set
                {
                    Material?.SetColor("_EmColor", value);
                }
            }

            public Texture _EmTex
            {
                get
                {
                    return Material?.GetTexture("_EmTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_EmTex", value);
                }
            }

            public float _EmPower
            {
                get
                {
                    return Material?.GetFloat("_EmPower") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_EmPower", Mathf.Clamp(value, 0f, 10f));
                }
            }

            public float _Smoothness
            {
                get
                {
                    return Material?.GetFloat("_Smoothness") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Smoothness", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public bool _IgnoreDiffuseAlphaForSpeculars
            {
                get
                {
                    return Material?.IsKeywordEnabled("FORCE_SPEC") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "FORCE_SPEC");
                }
            }

            public _RampInfoEnum _RampChoice
            {
                get
                {
                    return (_RampInfoEnum)(Material?.GetFloat("_RampInfo") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_RampInfo", Convert.ToSingle(value));
                }
            }

            public _DecalLayerEnum _DecalLayer
            {
                get
                {
                    return (_DecalLayerEnum)(Material?.GetFloat("_DecalLayer") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_DecalLayer", Convert.ToSingle(value));
                }
            }

            public float _SpecularStrength
            {
                get
                {
                    return Material?.GetFloat("_SpecularStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SpecularStrength", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _SpecularExponent
            {
                get
                {
                    return Material?.GetFloat("_SpecularExponent") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SpecularExponent", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public _CullEnum _Cull_Mode
            {
                get
                {
                    return (_CullEnum)(Material?.GetFloat("_Cull") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_Cull", Convert.ToSingle(value));
                }
            }

            public bool _EnableDither
            {
                get
                {
                    return Material?.IsKeywordEnabled("DITHER") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "DITHER");
                }
            }

            public float _FadeBias
            {
                get
                {
                    return Material?.GetFloat("_FadeBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FadeBias", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public bool _EnableFresnelEmission
            {
                get
                {
                    return Material?.IsKeywordEnabled("FRESNEL_EMISSION") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "FRESNEL_EMISSION");
                }
            }

            public Texture _FresnelRamp
            {
                get
                {
                    return Material?.GetTexture("_FresnelRamp") ?? null;
                }
                set
                {
                    Material?.SetTexture("_FresnelRamp", value);
                }
            }

            public float _FresnelPower
            {
                get
                {
                    return Material?.GetFloat("_FresnelPower") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FresnelPower", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public Texture _FresnelMask
            {
                get
                {
                    return Material?.GetTexture("_FresnelMask") ?? null;
                }
                set
                {
                    Material?.SetTexture("_FresnelMask", value);
                }
            }

            public float _FresnelBoost
            {
                get
                {
                    return Material?.GetFloat("_FresnelBoost") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FresnelBoost", Mathf.Clamp(value, 0f, 20f));
                }
            }

            public bool _EnablePrinting
            {
                get
                {
                    return Material?.IsKeywordEnabled("PRINT_CUTOFF") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "PRINT_CUTOFF");
                }
            }

            public float _SliceHeight
            {
                get
                {
                    return Material?.GetFloat("_SliceHeight") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SliceHeight", Mathf.Clamp(value, -25f, 25f));
                }
            }

            public float _PrintBandHeight
            {
                get
                {
                    return Material?.GetFloat("_SliceBandHeight") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SliceBandHeight", Mathf.Clamp(value, 0f, 10f));
                }
            }

            public float _PrintAlphaDepth
            {
                get
                {
                    return Material?.GetFloat("_SliceAlphaDepth") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SliceAlphaDepth", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public Texture _PrintAlphaTexture
            {
                get
                {
                    return Material?.GetTexture("_SliceAlphaTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_SliceAlphaTex", value);
                }
            }

            public Vector2 _PrintAlphaTextureScale
            {
                get
                {
                    return Material?.GetTextureScale("_SliceAlphaTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_SliceAlphaTex", value);
                }
            }

            public Vector2 _PrintAlphaTextureOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_SliceAlphaTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_SliceAlphaTex", value);
                }
            }

            public float _PrintColorBoost
            {
                get
                {
                    return Material?.GetFloat("_PrintBoost") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_PrintBoost", Mathf.Clamp(value, 0f, 10f));
                }
            }

            public float _PrintAlphaBias
            {
                get
                {
                    return Material?.GetFloat("_PrintBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_PrintBias", Mathf.Clamp(value, 0f, 4f));
                }
            }

            public float _PrintEmissionToAlbedoLerp
            {
                get
                {
                    return Material?.GetFloat("_PrintEmissionToAlbedoLerp") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_PrintEmissionToAlbedoLerp", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public _PrintDirectionEnum _PrintDirection
            {
                get
                {
                    return (_PrintDirectionEnum)(Material?.GetFloat("_PrintDirection") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_PrintDirection", Convert.ToSingle(value));
                }
            }

            public Texture _PrintRamp
            {
                get
                {
                    return Material?.GetTexture("_PrintRamp") ?? null;
                }
                set
                {
                    Material?.SetTexture("_PrintRamp", value);
                }
            }

            public float _EliteIndex
            {
                get
                {
                    return Material?.GetFloat("_EliteIndex") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_EliteIndex", value);
                }
            }

            public float _EliteBrightnessMin
            {
                get
                {
                    return Material?.GetFloat("_EliteBrightnessMin") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_EliteBrightnessMin", Mathf.Clamp(value, -10f, 10f));
                }
            }

            public float _EliteBrightnessMax
            {
                get
                {
                    return Material?.GetFloat("_EliteBrightnessMax") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_EliteBrightnessMax", Mathf.Clamp(value, -10f, 10f));
                }
            }

            public bool _EnableSplatmap
            {
                get
                {
                    return Material?.IsKeywordEnabled("SPLATMAP") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "SPLATMAP");
                }
            }

            public bool _UseVertexColorsInstead
            {
                get
                {
                    return Material?.IsKeywordEnabled("USE_VERTEX_COLORS") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "USE_VERTEX_COLORS");
                }
            }

            public float _BlendDepth
            {
                get
                {
                    return Material?.GetFloat("_Depth") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Depth", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public Texture _SplatmapTex
            {
                get
                {
                    return Material?.GetTexture("_SplatmapTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_SplatmapTex", value);
                }
            }

            public Vector2 _SplatmapTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_SplatmapTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_SplatmapTex", value);
                }
            }

            public Vector2 _SplatmapTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_SplatmapTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_SplatmapTex", value);
                }
            }

            public float _SplatmapTileScale
            {
                get
                {
                    return Material?.GetFloat("_SplatmapTileScale") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SplatmapTileScale", Mathf.Clamp(value, 0f, 20f));
                }
            }

            public Texture _GreenChannelTex
            {
                get
                {
                    return Material?.GetTexture("_GreenChannelTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_GreenChannelTex", value);
                }
            }

            public Texture _GreenChannelNormalTex
            {
                get
                {
                    return Material?.GetTexture("_GreenChannelNormalTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_GreenChannelNormalTex", value);
                }
            }

            public float _GreenChannelSmoothness
            {
                get
                {
                    return Material?.GetFloat("_GreenChannelSmoothness") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_GreenChannelSmoothness", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _GreenChannelBias
            {
                get
                {
                    return Material?.GetFloat("_GreenChannelBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_GreenChannelBias", Mathf.Clamp(value, -2f, 5f));
                }
            }

            public Texture _BlueChannelTex
            {
                get
                {
                    return Material?.GetTexture("_BlueChannelTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_BlueChannelTex", value);
                }
            }

            public Texture _BlueChannelNormalTex
            {
                get
                {
                    return Material?.GetTexture("_BlueChannelNormalTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_BlueChannelNormalTex", value);
                }
            }

            public float _BlueChannelSmoothness
            {
                get
                {
                    return Material?.GetFloat("_BlueChannelSmoothness") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_BlueChannelSmoothness", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _BlueChannelBias
            {
                get
                {
                    return Material?.GetFloat("_BlueChannelBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_BlueChannelBias", Mathf.Clamp(value, -2f, 5f));
                }
            }

            public bool _EnableFlowmap
            {
                get
                {
                    return Material?.IsKeywordEnabled("FLOWMAP") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "FLOWMAP");
                }
            }

            public Texture _FlowTexture
            {
                get
                {
                    return Material?.GetTexture("_FlowTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_FlowTex", value);
                }
            }

            public Texture _FlowHeightmap
            {
                get
                {
                    return Material?.GetTexture("_FlowHeightmap") ?? null;
                }
                set
                {
                    Material?.SetTexture("_FlowHeightmap", value);
                }
            }

            public Vector2 _FlowHeightmapScale
            {
                get
                {
                    return Material?.GetTextureScale("_FlowHeightmap") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_FlowHeightmap", value);
                }
            }

            public Vector2 _FlowHeightmapOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_FlowHeightmap") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_FlowHeightmap", value);
                }
            }

            public Texture _FlowHeightRamp
            {
                get
                {
                    return Material?.GetTexture("_FlowHeightRamp") ?? null;
                }
                set
                {
                    Material?.SetTexture("_FlowHeightRamp", value);
                }
            }

            public Vector2 _FlowHeightRampScale
            {
                get
                {
                    return Material?.GetTextureScale("_FlowHeightRamp") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_FlowHeightRamp", value);
                }
            }

            public Vector2 _FlowHeightRampOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_FlowHeightRamp") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_FlowHeightRamp", value);
                }
            }

            public float _FlowHeightBias
            {
                get
                {
                    return Material?.GetFloat("_FlowHeightBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FlowHeightBias", Mathf.Clamp(value, -1f, 1f));
                }
            }

            public float _FlowHeightPower
            {
                get
                {
                    return Material?.GetFloat("_FlowHeightPower") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FlowHeightPower", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public float _FlowEmissionStrength
            {
                get
                {
                    return Material?.GetFloat("_FlowEmissionStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FlowEmissionStrength", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public float _FlowSpeed
            {
                get
                {
                    return Material?.GetFloat("_FlowSpeed") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FlowSpeed", Mathf.Clamp(value, 0f, 15f));
                }
            }

            public float _MaskFlowStrength
            {
                get
                {
                    return Material?.GetFloat("_FlowMaskStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FlowMaskStrength", Mathf.Clamp(value, 0f, 5f));
                }
            }

            public float _NormalFlowStrength
            {
                get
                {
                    return Material?.GetFloat("_FlowNormalStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FlowNormalStrength", Mathf.Clamp(value, 0f, 5f));
                }
            }

            public float _FlowTextureScaleFactor
            {
                get
                {
                    return Material?.GetFloat("_FlowTextureScaleFactor") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FlowTextureScaleFactor", Mathf.Clamp(value, 0f, 10f));
                }
            }

            public bool _EnableLimbRemoval
            {
                get
                {
                    return Material?.IsKeywordEnabled("LIMBREMOVAL") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "LIMBREMOVAL");
                }
            }

            public float _LimbPrimeMask
            {
                get
                {
                    return Material?.GetFloat("_LimbPrimeMask") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_LimbPrimeMask", Mathf.Clamp(value, 1f, 10000f));
                }
            }

            public Color _FlashColor
            {
                get
                {
                    return Material?.GetColor("_FlashColor") ?? default(Color);
                }
                set
                {
                    Material?.SetColor("_FlashColor", value);
                }
            }

            public float _Fade
            {
                get
                {
                    return Material?.GetFloat("_Fade") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Fade", Mathf.Clamp(value, 0f, 1f));
                }
            }
        }

        public class HGSnowToppedController : HGBaseController
        {
            public enum _RampInfoEnum
            {
                TwoTone = 0,
                SmoothedTwoTone = 1,
                Unlitish = 3,
                Subsurface = 4,
                Grass = 5
            }

            public override string ShaderName => "Hopoo Games/Deferred/Snow Topped";

            public Color _Color
            {
                get
                {
                    return Material?.GetColor("_Color") ?? default(Color);
                }
                set
                {
                    Material?.SetColor("_Color", value);
                }
            }

            public Texture _MainTex
            {
                get
                {
                    return Material?.GetTexture("_MainTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_MainTex", value);
                }
            }

            public Vector2 _MainTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_MainTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_MainTex", value);
                }
            }

            public Vector2 _MainTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_MainTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_MainTex", value);
                }
            }

            public float _NormalStrength
            {
                get
                {
                    return Material?.GetFloat("_NormalStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_NormalStrength", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public Texture _NormalTex
            {
                get
                {
                    return Material?.GetTexture("_NormalTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_NormalTex", value);
                }
            }

            public Vector2 _NormalTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_NormalTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_NormalTex", value);
                }
            }

            public Vector2 _NormalTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_NormalTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_NormalTex", value);
                }
            }

            public Texture _SnowTex
            {
                get
                {
                    return Material?.GetTexture("_SnowTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_SnowTex", value);
                }
            }

            public Vector2 _SnowTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_SnowTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_SnowTex", value);
                }
            }

            public Vector2 _SnowTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_SnowTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_SnowTex", value);
                }
            }

            public Texture _SnowNormalTex
            {
                get
                {
                    return Material?.GetTexture("_SnowNormalTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_SnowNormalTex", value);
                }
            }

            public Vector2 _SnowNormalTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_SnowNormalTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_SnowNormalTex", value);
                }
            }

            public Vector2 _SnowNormalTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_SnowNormalTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_SnowNormalTex", value);
                }
            }

            public float _SnowBias
            {
                get
                {
                    return Material?.GetFloat("_SnowBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SnowBias", Mathf.Clamp(value, -1f, 1f));
                }
            }

            public float _Depth
            {
                get
                {
                    return Material?.GetFloat("_Depth") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Depth", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public bool _IgnoreAlphaWeights
            {
                get
                {
                    return Material?.IsKeywordEnabled("IGNORE_BIAS") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "IGNORE_BIAS");
                }
            }

            public bool _BlendWeightsBinarily
            {
                get
                {
                    return Material?.IsKeywordEnabled("BINARYBLEND") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "BINARYBLEND");
                }
            }

            public _RampInfoEnum _RampChoice
            {
                get
                {
                    return (_RampInfoEnum)(Material?.GetFloat("_RampInfo") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_RampInfo", Convert.ToSingle(value));
                }
            }

            public bool _IgnoreDiffuseAlphaForSpeculars
            {
                get
                {
                    return Material?.IsKeywordEnabled("FORCE_SPEC") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "FORCE_SPEC");
                }
            }

            public float _SpecularStrength
            {
                get
                {
                    return Material?.GetFloat("_SpecularStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SpecularStrength", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _SpecularExponent
            {
                get
                {
                    return Material?.GetFloat("_SpecularExponent") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SpecularExponent", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public float _Smoothness
            {
                get
                {
                    return Material?.GetFloat("_Smoothness") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Smoothness", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _SnowSpecularStrength
            {
                get
                {
                    return Material?.GetFloat("_SnowSpecularStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SnowSpecularStrength", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _SnowSpecularExponent
            {
                get
                {
                    return Material?.GetFloat("_SnowSpecularExponent") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SnowSpecularExponent", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public float _SnowSmoothness
            {
                get
                {
                    return Material?.GetFloat("_SnowSmoothness") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SnowSmoothness", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _Fade
            {
                get
                {
                    return Material?.GetFloat("_Fade") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Fade", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public bool _DitherOn
            {
                get
                {
                    return Material?.IsKeywordEnabled("DITHER") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "DITHER");
                }
            }

            public bool _TriplanarOn
            {
                get
                {
                    return Material?.IsKeywordEnabled("TRIPLANAR") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "TRIPLANAR");
                }
            }

            public float _TriplanarTextureFactor
            {
                get
                {
                    return Material?.GetFloat("_TriplanarTextureFactor") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_TriplanarTextureFactor", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public bool _SnowOn
            {
                get
                {
                    return Material?.IsKeywordEnabled("MICROFACET_SNOW") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "MICROFACET_SNOW");
                }
            }

            public bool _GradientBiasOn
            {
                get
                {
                    return Material?.IsKeywordEnabled("GRADIENTBIAS") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "GRADIENTBIAS");
                }
            }

            public Vector4 _GradientBiasVector
            {
                get
                {
                    return Material?.GetVector("_GradientBiasVector") ?? Vector4.zero;
                }
                set
                {
                    Material?.SetVector("_GradientBiasVector", value);
                }
            }

            public bool _DirtOn
            {
                get
                {
                    return Material?.IsKeywordEnabled("DIRTON") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "DIRTON");
                }
            }

            public Texture _DirtTex
            {
                get
                {
                    return Material?.GetTexture("_DirtTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_DirtTex", value);
                }
            }

            public Vector2 _DirtTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_DirtTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_DirtTex", value);
                }
            }

            public Vector2 _DirtTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_DirtTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_DirtTex", value);
                }
            }

            public Texture _DirtNormalTex
            {
                get
                {
                    return Material?.GetTexture("_DirtNormalTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_DirtNormalTex", value);
                }
            }

            public Vector2 _DirtNormalTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_DirtNormalTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_DirtNormalTex", value);
                }
            }

            public Vector2 _DirtNormalTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_DirtNormalTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_DirtNormalTex", value);
                }
            }

            public float _DirtBias
            {
                get
                {
                    return Material?.GetFloat("_DirtBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_DirtBias", Mathf.Clamp(value, -2f, 2f));
                }
            }

            public float _DirtSpecularStrength
            {
                get
                {
                    return Material?.GetFloat("_DirtSpecularStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_DirtSpecularStrength", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _DirtSpecularExponent
            {
                get
                {
                    return Material?.GetFloat("_DirtSpecularExponent") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_DirtSpecularExponent", Mathf.Clamp(value, 0f, 20f));
                }
            }

            public float _DirtSmoothness
            {
                get
                {
                    return Material?.GetFloat("_DirtSmoothness") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_DirtSmoothness", Mathf.Clamp(value, 0f, 1f));
                }
            }
        }

        public class HGTriplanarTerrainBlend : HGBaseController
        {
            public enum _RampInfoEnum
            {
                TwoTone = 0,
                SmoothedTwoTone = 1,
                Unlitish = 3,
                Subsurface = 4,
                Grass = 5
            }

            public enum _DecalLayerEnum
            {
                Default,
                Environment,
                Character,
                Misc
            }

            public enum _CullEnum
            {
                Off,
                Front,
                Back
            }

            public override string ShaderName => "Hopoo Games/Deferred/Triplanar Terrain Blend";

            public bool _Use_Vertex_Colors
            {
                get
                {
                    return Material?.IsKeywordEnabled("USE_VERTEX_COLORS") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "USE_VERTEX_COLORS");
                }
            }

            public bool _Mix_Vertex_Colors
            {
                get
                {
                    return Material?.IsKeywordEnabled("MIX_VERTEX_COLORS") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "MIX_VERTEX_COLORS");
                }
            }

            public bool _Use_Alpha_As_Mask
            {
                get
                {
                    return Material?.IsKeywordEnabled("USE_ALPHA_AS_MASK") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "USE_ALPHA_AS_MASK");
                }
            }

            public bool _Vertical_Bias_On
            {
                get
                {
                    return Material?.IsKeywordEnabled("USE_VERTICAL_BIAS") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "USE_VERTICAL_BIAS");
                }
            }

            public bool _Double_Sample_On
            {
                get
                {
                    return Material?.IsKeywordEnabled("DOUBLESAMPLE") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "DOUBLESAMPLE");
                }
            }

            public Color _Color
            {
                get
                {
                    return Material?.GetColor("_Color") ?? default(Color);
                }
                set
                {
                    Material?.SetColor("_Color", value);
                }
            }

            public Texture _NormalTex
            {
                get
                {
                    return Material?.GetTexture("_NormalTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_NormalTex", value);
                }
            }

            public Vector2 _NormalTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_NormalTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_NormalTex", value);
                }
            }

            public Vector2 _NormalTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_NormalTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_NormalTex", value);
                }
            }

            public float _NormalStrength
            {
                get
                {
                    return Material?.GetFloat("_NormalStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_NormalStrength", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public _RampInfoEnum _RampChoice
            {
                get
                {
                    return (_RampInfoEnum)(Material?.GetFloat("_RampInfo") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_RampInfo", Convert.ToSingle(value));
                }
            }

            public _DecalLayerEnum _DecalLayer
            {
                get
                {
                    return (_DecalLayerEnum)(Material?.GetFloat("_DecalLayer") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_DecalLayer", Convert.ToSingle(value));
                }
            }

            public _CullEnum _Cull_Mode
            {
                get
                {
                    return (_CullEnum)(Material?.GetFloat("_Cull") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_Cull", Convert.ToSingle(value));
                }
            }

            public float _TextureFactor
            {
                get
                {
                    return Material?.GetFloat("_TextureFactor") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_TextureFactor", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _Depth
            {
                get
                {
                    return Material?.GetFloat("_Depth") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Depth", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public Texture _SplatmapTex
            {
                get
                {
                    return Material?.GetTexture("_SplatmapTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_SplatmapTex", value);
                }
            }

            public Vector2 _SplatmapTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_SplatmapTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_SplatmapTex", value);
                }
            }

            public Vector2 _SplatmapTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_SplatmapTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_SplatmapTex", value);
                }
            }

            public Texture _RedChannelTopTex
            {
                get
                {
                    return Material?.GetTexture("_RedChannelTopTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_RedChannelTopTex", value);
                }
            }

            public Vector2 _RedChannelTopTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_RedChannelTopTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_RedChannelTopTex", value);
                }
            }

            public Vector2 _RedChannelTopTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_RedChannelTopTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_RedChannelTopTex", value);
                }
            }

            public Texture _RedChannelSideTex
            {
                get
                {
                    return Material?.GetTexture("_RedChannelSideTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_RedChannelSideTex", value);
                }
            }

            public Vector2 _RedChannelSideTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_RedChannelSideTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_RedChannelSideTex", value);
                }
            }

            public Vector2 _RedChannelSideTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_RedChannelSideTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_RedChannelSideTex", value);
                }
            }

            public float _RedChannelSmoothness
            {
                get
                {
                    return Material?.GetFloat("_RedChannelSmoothness") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_RedChannelSmoothness", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _RedChannelSpecularStrength
            {
                get
                {
                    return Material?.GetFloat("_RedChannelSpecularStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_RedChannelSpecularStrength", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _RedChannelSpecularExponent
            {
                get
                {
                    return Material?.GetFloat("_RedChannelSpecularExponent") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_RedChannelSpecularExponent", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public float _RedChannelBias
            {
                get
                {
                    return Material?.GetFloat("_RedChannelBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_RedChannelBias", Mathf.Clamp(value, -2f, 5f));
                }
            }

            public Texture _GreenChannelTex
            {
                get
                {
                    return Material?.GetTexture("_GreenChannelTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_GreenChannelTex", value);
                }
            }

            public Vector2 _GreenChannelTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_GreenChannelTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_GreenChannelTex", value);
                }
            }

            public Vector2 _GreenChannelTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_GreenChannelTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_GreenChannelTex", value);
                }
            }

            public float _GreenChannelSmoothness
            {
                get
                {
                    return Material?.GetFloat("_GreenChannelSmoothness") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_GreenChannelSmoothness", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _GreenChannelSpecularStrength
            {
                get
                {
                    return Material?.GetFloat("_GreenChannelSpecularStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_GreenChannelSpecularStrength", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _GreenChannelSpecularExponent
            {
                get
                {
                    return Material?.GetFloat("_GreenChannelSpecularExponent") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_GreenChannelSpecularExponent", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public float _GreenChannelBias
            {
                get
                {
                    return Material?.GetFloat("_GreenChannelBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_GreenChannelBias", Mathf.Clamp(value, -2f, 5f));
                }
            }

            public Texture _BlueChannelTex
            {
                get
                {
                    return Material?.GetTexture("_RedChannelTopTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_BlueChannelTex", value);
                }
            }

            public Vector2 _BlueChannelTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_BlueChannelTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_BlueChannelTex", value);
                }
            }

            public Vector2 _BlueChannelTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_BlueChannelTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_BlueChannelTex", value);
                }
            }

            public float _BlueChannelSmoothness
            {
                get
                {
                    return Material?.GetFloat("_BlueChannelSmoothness") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_BlueChannelSmoothness", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _BlueChannelSpecularStrength
            {
                get
                {
                    return Material?.GetFloat("_BlueChannelSpecularStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_BlueChannelSpecularStrength", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _BlueChannelSpecularExponent
            {
                get
                {
                    return Material?.GetFloat("_BlueChannelSpecularExponent") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_BlueChannelSpecularExponent", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public float _BlueChannelBias
            {
                get
                {
                    return Material?.GetFloat("_BlueChannelBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_BlueChannelBias", Mathf.Clamp(value, -2f, 5f));
                }
            }

            public bool _Treat_Green_Channel_As_Snow
            {
                get
                {
                    return Material?.IsKeywordEnabled("MICROFACET_SNOW") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "MICROFACET_SNOW");
                }
            }
        }

        public class HGDistortionController : HGBaseController
        {
            public Texture _BumpMap
            {
                get
                {
                    return Material?.GetTexture("_BumpMap") ?? null;
                }
                set
                {
                    Material?.SetTexture("_BumpMap", value);
                }
            }

            public Vector2 _BumpMapScale
            {
                get
                {
                    return Material?.GetTextureScale("_BumpMap") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_BumpMap", value);
                }
            }

            public Vector2 _BumpMapOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_BumpMap") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_BumpMap", value);
                }
            }

            public Texture _MaskTex
            {
                get
                {
                    return Material?.GetTexture("_MaskTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_MaskTex", value);
                }
            }

            public Vector2 _MaskTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_MaskTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_MaskTex", value);
                }
            }

            public Vector2 _MaskTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_MaskTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_MaskTex", value);
                }
            }

            public float _Magnitude
            {
                get
                {
                    return Material?.GetFloat("_Magnitude") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Magnitude", Mathf.Clamp(value, 0f, 10f));
                }
            }

            public float _NearFadeZeroDistance
            {
                get
                {
                    return Material?.GetFloat("_NearFadeZeroDistance") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_NearFadeZeroDistance", value);
                }
            }

            public float _NearFadeOneDistance
            {
                get
                {
                    return Material?.GetFloat("_NearFadeOneDistance") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_NearFadeOneDistance", value);
                }
            }

            public float _FarFadeOneDistance
            {
                get
                {
                    return Material?.GetFloat("_FarFadeOneDistance") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FarFadeOneDistance", value);
                }
            }

            public float _FarFadeZeroDistance
            {
                get
                {
                    return Material?.GetFloat("_FarFadeZeroDistance") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FarFadeZeroDistance", value);
                }
            }

            public bool _DistanceModulationOn
            {
                get
                {
                    return Material?.IsKeywordEnabled("DISTANCEMODULATION") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "DISTANCEMODULATION");
                }
            }

            public float _DistanceModulationMagnitude
            {
                get
                {
                    return Material?.GetFloat("_DistanceModulationMagnitude") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_DistanceModulationMagnitude", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _InvFade
            {
                get
                {
                    return Material?.GetFloat("_InvFade") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_InvFade", Mathf.Clamp(value, 0f, 2f));
                }
            }
        }

        public class HGSolidParallaxController : HGBaseController
        {
            public enum _RampInfoEnum
            {
                TwoTone = 0,
                SmoothedTwoTone = 1,
                Unlitish = 3,
                Subsurface = 4,
                Grass = 5
            }

            public enum _CullEnum
            {
                Off,
                Front,
                Back
            }

            public override string ShaderName => "Hopoo Games/FX/Solid Parallax";

            public string MaterialName => Material?.name ?? "";

            public Color _Color
            {
                get
                {
                    return Material?.GetColor("_Color") ?? default(Color);
                }
                set
                {
                    Material?.SetColor("_Color", value);
                }
            }

            public Texture _MainTex
            {
                get
                {
                    return Material?.GetTexture("_MainTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_MainTex", value);
                }
            }

            public Vector2 _MainTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_MainTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_MainTex", value);
                }
            }

            public Vector2 _MainTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_MainTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_MainTex", value);
                }
            }

            public Texture _EmissionTex
            {
                get
                {
                    return Material?.GetTexture("_EmissionTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_EmissionTex", value);
                }
            }

            public Vector2 _EmissionTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_EmissionTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_EmissionTex", value);
                }
            }

            public Vector2 _EmissionTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_EmissionTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_EmissionTex", value);
                }
            }

            public float _EmissionPower
            {
                get
                {
                    return Material?.GetFloat("_EmissionPower") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_EmissionPower", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public Texture _Normal
            {
                get
                {
                    return Material?.GetTexture("_Normal") ?? null;
                }
                set
                {
                    Material?.SetTexture("_Normal", value);
                }
            }

            public Vector2 _NormalScale
            {
                get
                {
                    return Material?.GetTextureScale("_Normal") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_Normal", value);
                }
            }

            public Vector2 _NormalOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_Normal") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_Normal", value);
                }
            }

            public float _SpecularStrength
            {
                get
                {
                    return Material?.GetFloat("_SpecularStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SpecularStrength", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _SpecularExponent
            {
                get
                {
                    return Material?.GetFloat("_SpecularExponent") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SpecularExponent", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public float _Smoothness
            {
                get
                {
                    return Material?.GetFloat("_Smoothness") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Smoothness", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public Texture _Height1
            {
                get
                {
                    return Material?.GetTexture("_Height1") ?? null;
                }
                set
                {
                    Material?.SetTexture("_Height1", value);
                }
            }

            public Vector2 _Height1Scale
            {
                get
                {
                    return Material?.GetTextureScale("_Height1") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_Height1", value);
                }
            }

            public Vector2 _Height1Offset
            {
                get
                {
                    return Material?.GetTextureOffset("_Height1") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_Height1", value);
                }
            }

            public Texture _Height2
            {
                get
                {
                    return Material?.GetTexture("_Height2") ?? null;
                }
                set
                {
                    Material?.SetTexture("_Height2", value);
                }
            }

            public Vector2 _Height2Scale
            {
                get
                {
                    return Material?.GetTextureScale("_Height2") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_Height2", value);
                }
            }

            public Vector2 _Height2Offset
            {
                get
                {
                    return Material?.GetTextureOffset("_Height2") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_Height2", value);
                }
            }

            public float _HeightStrength
            {
                get
                {
                    return Material?.GetFloat("_HeightStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_HeightStrength", Mathf.Clamp(value, 0f, 20f));
                }
            }

            public float _HeightBias
            {
                get
                {
                    return Material?.GetFloat("_HeightBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_HeightBias", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _Parallax
            {
                get
                {
                    return Material?.GetFloat("_Parallax") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Parallax", value);
                }
            }

            public Vector4 _ScrollSpeed
            {
                get
                {
                    return Material?.GetVector("_ScrollSpeed") ?? Vector4.zero;
                }
                set
                {
                    Material?.SetVector("_ScrollSpeed", value);
                }
            }

            public _RampInfoEnum _RampInfo
            {
                get
                {
                    return (_RampInfoEnum)(Material?.GetFloat("_RampInfo") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_RampInfo", Convert.ToSingle(value));
                }
            }

            public _CullEnum _Cull_Mode
            {
                get
                {
                    return (_CullEnum)(Material?.GetFloat("_Cull") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_Cull", Convert.ToSingle(value));
                }
            }

            public bool _ClipOn
            {
                get
                {
                    return Material?.IsKeywordEnabled("ALPHACLIP") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "ALPHACLIP");
                }
            }
        }

        public class HGCloudRemapController : HGBaseController
        {
            public enum _CullEnum
            {
                Off,
                Front,
                Back
            }

            public override string ShaderName => "Hopoo Games/FX/Cloud Remap";

            public string MaterialName => Material?.name ?? "";

            public BlendMode _Source_Blend_Mode
            {
                get
                {
                    return (BlendMode)(Material?.GetFloat("_SrcBlend") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_SrcBlend", Convert.ToSingle(value));
                }
            }

            public BlendMode _Destination_Blend_Mode
            {
                get
                {
                    return (BlendMode)(Material?.GetFloat("_DstBlend") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_DstBlend", Convert.ToSingle(value));
                }
            }

            public Color _Tint
            {
                get
                {
                    return Material?.GetColor("_TintColor") ?? default(Color);
                }
                set
                {
                    Material?.SetColor("_TintColor", value);
                }
            }

            public bool _DisableRemapping
            {
                get
                {
                    return Material?.IsKeywordEnabled("DISABLEREMAP") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "DISABLEREMAP");
                }
            }

            public Texture _MainTex
            {
                get
                {
                    return Material?.GetTexture("_MainTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_MainTex", value);
                }
            }

            public Vector2 _MainTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_MainTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_MainTex", value);
                }
            }

            public Vector2 _MainTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_MainTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_MainTex", value);
                }
            }

            public Texture _RemapTex
            {
                get
                {
                    return Material?.GetTexture("_RemapTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_RemapTex", value);
                }
            }

            public Vector2 _RemapTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_RemapTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_RemapTex", value);
                }
            }

            public Vector2 _RemapTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_RemapTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_RemapTex", value);
                }
            }

            public float _SoftFactor
            {
                get
                {
                    return Material?.GetFloat("_InvFade") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_InvFade", Mathf.Clamp(value, 0f, 2f));
                }
            }

            public float _BrightnessBoost
            {
                get
                {
                    return Material?.GetFloat("_Boost") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Boost", Mathf.Clamp(value, 1f, 20f));
                }
            }

            public float _AlphaBoost
            {
                get
                {
                    return Material?.GetFloat("_AlphaBoost") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_AlphaBoost", Mathf.Clamp(value, 0f, 20f));
                }
            }

            public float _AlphaBias
            {
                get
                {
                    return Material?.GetFloat("_AlphaBias") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_AlphaBias", Mathf.Clamp(value, 0f, 20f));
                }
            }

            public bool _UseUV1
            {
                get
                {
                    return Material?.IsKeywordEnabled("USE_UV1") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "USE_UV1");
                }
            }

            public bool _FadeWhenNearCamera
            {
                get
                {
                    return Material?.IsKeywordEnabled("FADECLOSE") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "FADECLOSE");
                }
            }

            public float _FadeCloseDistance
            {
                get
                {
                    return Material?.GetFloat("_FadeCloseDistance") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FadeCloseDistance", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public _CullEnum _Cull_Mode
            {
                get
                {
                    return (_CullEnum)(Material?.GetFloat("_Cull") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_Cull", Convert.ToSingle(value));
                }
            }

            public CompareFunction _ZTest_Mode
            {
                get
                {
                    return (CompareFunction)(Material?.GetFloat("_ZTest") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_ZTest", Convert.ToSingle(value));
                }
            }

            public float _DepthOffset
            {
                get
                {
                    return Material?.GetFloat("_DepthOffset") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_DepthOffset", Mathf.Clamp(value, -10f, 10f));
                }
            }

            public bool _CloudRemapping
            {
                get
                {
                    return Material?.IsKeywordEnabled("USE_CLOUDS") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "USE_CLOUDS");
                }
            }

            public bool _DistortionClouds
            {
                get
                {
                    return Material?.IsKeywordEnabled("CLOUDOFFSET") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "CLOUDOFFSET");
                }
            }

            public float _DistortionStrength
            {
                get
                {
                    return Material?.GetFloat("_DistortionStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_DistortionStrength", Mathf.Clamp(value, -2f, 2f));
                }
            }

            public Texture _Cloud1Tex
            {
                get
                {
                    return Material?.GetTexture("_Cloud1Tex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_Cloud1Tex", value);
                }
            }

            public Vector2 _Cloud1TexScale
            {
                get
                {
                    return Material?.GetTextureScale("_Cloud1Tex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_Cloud1Tex", value);
                }
            }

            public Vector2 _Cloud1TexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_Cloud1Tex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_Cloud1Tex", value);
                }
            }

            public Texture _Cloud2Tex
            {
                get
                {
                    return Material?.GetTexture("_Cloud2Tex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_Cloud2Tex", value);
                }
            }

            public Vector2 _Cloud2TexScale
            {
                get
                {
                    return Material?.GetTextureScale("_Cloud2Tex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_Cloud2Tex", value);
                }
            }

            public Vector2 _Cloud2TexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_Cloud2Tex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_Cloud2Tex", value);
                }
            }

            public Vector4 _CutoffScroll
            {
                get
                {
                    return Material?.GetVector("_CutoffScroll") ?? Vector4.zero;
                }
                set
                {
                    Material?.SetVector("_CutoffScroll", value);
                }
            }

            public bool _VertexColors
            {
                get
                {
                    return Material?.IsKeywordEnabled("VERTEXCOLOR") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "VERTEXCOLOR");
                }
            }

            public bool _LuminanceForVertexAlpha
            {
                get
                {
                    return Material?.IsKeywordEnabled("VERTEXALPHA") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "VERTEXALPHA");
                }
            }

            public bool _LuminanceForTextureAlpha
            {
                get
                {
                    return Material?.IsKeywordEnabled("CALCTEXTUREALPHA") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "CALCTEXTUREALPHA");
                }
            }

            public bool _VertexOffset
            {
                get
                {
                    return Material?.IsKeywordEnabled("VERTEXOFFSET") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "VERTEXOFFSET");
                }
            }

            public bool _FresnelFade
            {
                get
                {
                    return Material?.IsKeywordEnabled("FRESNEL") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "FRESNEL");
                }
            }

            public bool _SkyboxOnly
            {
                get
                {
                    return Material?.IsKeywordEnabled("SKYBOX_ONLY") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "SKYBOX_ONLY");
                }
            }

            public float _FresnelPower
            {
                get
                {
                    return Material?.GetFloat("_FresnelPower") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_FresnelPower", Mathf.Clamp(value, -20f, 20f));
                }
            }

            public float _VertexOffsetAmount
            {
                get
                {
                    return Material?.GetFloat("_OffsetAmount") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_OffsetAmount", Mathf.Clamp(value, 0f, 3f));
                }
            }

            public float _ExternalAlpha
            {
                get
                {
                    return Material?.GetFloat("_ExternalAlpha") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_ExternalAlpha", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public float _Fade
            {
                get
                {
                    return Material?.GetFloat("_Fade") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Fade", Mathf.Clamp(value, 0f, 1f));
                }
            }
        }

        public class HGIntersectionController : HGBaseController
        {
            public enum _CullEnum
            {
                Off,
                Front,
                Back
            }

            public override string ShaderName => "Hopoo Games/FX/Cloud Intersection Remap";

            public string MaterialName => Material?.name ?? "";

            public BlendMode _Source_Blend_Mode
            {
                get
                {
                    return (BlendMode)(Material?.GetFloat("_SrcBlendFloat") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_SrcBlendFloat", Convert.ToSingle(value));
                }
            }

            public BlendMode _Destination_Blend_Mode
            {
                get
                {
                    return (BlendMode)(Material?.GetFloat("_DstBlendFloat") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_DstBlendFloat", Convert.ToSingle(value));
                }
            }

            public Color _Tint
            {
                get
                {
                    return Material?.GetColor("_TintColor") ?? default(Color);
                }
                set
                {
                    Material?.SetColor("_TintColor", value);
                }
            }

            public Texture _MainTex
            {
                get
                {
                    return Material?.GetTexture("_MainTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_MainTex", value);
                }
            }

            public Vector2 _MainTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_MainTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_MainTex", value);
                }
            }

            public Vector2 _MainTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_MainTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_MainTex", value);
                }
            }

            public Texture _Cloud1Tex
            {
                get
                {
                    return Material?.GetTexture("_Cloud1Tex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_Cloud1Tex", value);
                }
            }

            public Vector2 _Cloud1TexScale
            {
                get
                {
                    return Material?.GetTextureScale("_Cloud1Tex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_Cloud1Tex", value);
                }
            }

            public Vector2 _Cloud1TexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_Cloud1Tex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_Cloud1Tex", value);
                }
            }

            public Texture _Cloud2Tex
            {
                get
                {
                    return Material?.GetTexture("_Cloud2Tex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_Cloud2Tex", value);
                }
            }

            public Vector2 _Cloud2TexScale
            {
                get
                {
                    return Material?.GetTextureScale("_Cloud2Tex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_Cloud2Tex", value);
                }
            }

            public Vector2 _Cloud2TexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_Cloud2Tex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_Cloud2Tex", value);
                }
            }

            public Texture _RemapTex
            {
                get
                {
                    return Material?.GetTexture("_RemapTex") ?? null;
                }
                set
                {
                    Material?.SetTexture("_RemapTex", value);
                }
            }

            public Vector2 _RemapTexScale
            {
                get
                {
                    return Material?.GetTextureScale("_RemapTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureScale("_RemapTex", value);
                }
            }

            public Vector2 _RemapTexOffset
            {
                get
                {
                    return Material?.GetTextureOffset("_RemapTex") ?? Vector2.zero;
                }
                set
                {
                    Material?.SetTextureOffset("_RemapTex", value);
                }
            }

            public Vector4 _CutoffScroll
            {
                get
                {
                    return Material?.GetVector("_CutoffScroll") ?? Vector4.zero;
                }
                set
                {
                    Material?.SetVector("_CutoffScroll", value);
                }
            }

            public float _SoftFactor
            {
                get
                {
                    return Material?.GetFloat("_InvFade") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_InvFade", Mathf.Clamp(value, 0f, 30f));
                }
            }

            public float _SoftPower
            {
                get
                {
                    return Material?.GetFloat("_SoftPower") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_SoftPower", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public float _BrightnessBoost
            {
                get
                {
                    return Material?.GetFloat("_Boost") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_Boost", Mathf.Clamp(value, 0f, 5f));
                }
            }

            public float _RimPower
            {
                get
                {
                    return Material?.GetFloat("_RimPower") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_RimPower", Mathf.Clamp(value, 0.1f, 20f));
                }
            }

            public float _RimStrength
            {
                get
                {
                    return Material?.GetFloat("_RimStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_RimStrength", Mathf.Clamp(value, 0f, 5f));
                }
            }

            public float _AlphaBoost
            {
                get
                {
                    return Material?.GetFloat("_AlphaBoost") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_AlphaBoost", Mathf.Clamp(value, 0f, 20f));
                }
            }

            public float _IntersectionStrength
            {
                get
                {
                    return Material?.GetFloat("_IntersectionStrength") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_IntersectionStrength", Mathf.Clamp(value, 0f, 20f));
                }
            }

            public _CullEnum _Cull_Mode
            {
                get
                {
                    return (_CullEnum)(Material?.GetFloat("_Cull") ?? 1f);
                }
                set
                {
                    Material?.SetFloat("_Cull", Convert.ToSingle(value));
                }
            }

            public float _ExternalAlpha
            {
                get
                {
                    return Material?.GetFloat("_ExternalAlpha") ?? 0f;
                }
                set
                {
                    Material?.SetFloat("_ExternalAlpha", Mathf.Clamp(value, 0f, 1f));
                }
            }

            public bool _FadeFromVertexColorsOn
            {
                get
                {
                    return Material?.IsKeywordEnabled("FADE_FROM_VERTEX_COLORS") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "FADE_FROM_VERTEX_COLORS");
                }
            }

            public bool _EnableTriplanarProjectionsForClouds
            {
                get
                {
                    return Material?.IsKeywordEnabled("TRIPLANAR") ?? false;
                }
                set
                {
                    SetShaderKeywordBasedOnBool(value, Material, "TRIPLANAR");
                }
            }
        }

        public static void SetShaderKeywordBasedOnBool(bool enabled, Material material, string keyword)
        {
            if (!material)
            {
                return;
            }
            if (enabled)
            {
                if (!material.IsKeywordEnabled(keyword))
                {
                    material.EnableKeyword(keyword);
                }
            }
            else if (material.IsKeywordEnabled(keyword))
            {
                material.DisableKeyword(keyword);
            }
        }

        public static void PutMaterialIntoMeshRenderer(Renderer meshRenderer, Material material)
        {
            if ((bool)material && (bool)meshRenderer)
            {
                meshRenderer.materials[0] = material;
            }
        }
    }
}
