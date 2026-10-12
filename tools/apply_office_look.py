import os

root = r"D:\HUH\backrooms\Assets\_Project"
tex_dir = os.path.join(root, "Art", "Textures")
mat_dir = os.path.join(root, "Art", "Materials")
os.makedirs(mat_dir, exist_ok=True)

wall_tex_guid = "a1b2c3d4e5f64789a0b1c2d3e4f50601"
carpet_tex_guid = "a1b2c3d4e5f64789a0b1c2d3e4f50602"
wall_mat_guid = "b1b2c3d4e5f64789a0b1c2d3e4f50701"
carpet_mat_guid = "b1b2c3d4e5f64789a0b1c2d3e4f50702"


def write_tex_meta(path: str, guid: str) -> None:
    meta = f"""fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 1
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 2
    mipBias: 0
    wrapU: 0
    wrapV: 0
    wrapW: 0
  nPOTScale: 1
  lightmap: 0
  compressionQuality: 50
  spriteMode: 0
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 0
  spriteTessellationDetail: -1
  textureType: 0
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 3
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID:
    internalID: 0
    vertices: []
    indices:
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable: {{}}
  mipmapLimitGroupName:
  pSDRemoveMatte: 0
  userData:
  assetBundleName:
  assetBundleVariant:
"""
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write(meta)


def mat_yaml(name: str, tex_guid: str, tile_x: float, tile_y: float, smoothness: float) -> str:
    return f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &2100000
Material:
  serializedVersion: 8
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_Name: {name}
  m_Shader: {{fileID: 4800000, guid: 933532a4fcc9baf4fa0491de14d08ed7, type: 3}}
  m_Parent: {{fileID: 0}}
  m_ModifiedSerializedProperties: 0
  m_ValidKeywords: []
  m_InvalidKeywords: []
  m_LightmapFlags: 4
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap:
    RenderType: Opaque
  disabledShaderPasses:
  - MOTIONVECTORS
  m_LockedProperties:
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _BaseMap:
        m_Texture: {{fileID: 2800000, guid: {tex_guid}, type: 3}}
        m_Scale: {{x: {tile_x}, y: {tile_y}}}
        m_Offset: {{x: 0, y: 0}}
    - _BumpMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _DetailAlbedoMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _DetailMask:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _DetailNormalMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _EmissionMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _MainTex:
        m_Texture: {{fileID: 2800000, guid: {tex_guid}, type: 3}}
        m_Scale: {{x: {tile_x}, y: {tile_y}}}
        m_Offset: {{x: 0, y: 0}}
    - _MetallicGlossMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _OcclusionMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _ParallaxMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - _SpecGlossMap:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - unity_Lightmaps:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - unity_LightmapsInd:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    - unity_ShadowMasks:
        m_Texture: {{fileID: 0}}
        m_Scale: {{x: 1, y: 1}}
        m_Offset: {{x: 0, y: 0}}
    m_Ints: []
    m_Floats:
    - _AddPrecomputedVelocity: 0
    - _AlphaClip: 0
    - _AlphaToMask: 0
    - _Blend: 0
    - _BlendModePreserveSpecular: 1
    - _BumpScale: 1
    - _ClearCoatMask: 0
    - _ClearCoatSmoothness: 0
    - _Cull: 2
    - _Cutoff: 0.5
    - _DetailAlbedoMapScale: 1
    - _DetailNormalMapScale: 1
    - _DstBlend: 0
    - _DstBlendAlpha: 0
    - _EnvironmentReflections: 1
    - _GlossMapScale: 0
    - _Glossiness: 0
    - _GlossyReflections: 0
    - _Metallic: 0
    - _OcclusionStrength: 1
    - _Parallax: 0.005
    - _QueueOffset: 0
    - _ReceiveShadows: 1
    - _Smoothness: {smoothness}
    - _SmoothnessTextureChannel: 0
    - _SpecularHighlights: 1
    - _SrcBlend: 1
    - _SrcBlendAlpha: 1
    - _Surface: 0
    - _WorkflowMode: 1
    - _ZWrite: 1
    m_Colors:
    - _BaseColor: {{r: 1, g: 1, b: 1, a: 1}}
    - _Color: {{r: 1, g: 1, b: 1, a: 1}}
    - _EmissionColor: {{r: 0, g: 0, b: 0, a: 1}}
    - _SpecColor: {{r: 0.2, g: 0.2, b: 0.2, a: 1}}
  m_BuildTextureStacks: []
  m_AllowLocking: 1
--- !u!114 &4431363231931934953
MonoBehaviour:
  m_ObjectHideFlags: 11
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: d0353a89b1f911e48b9e16bdc9f2e058, type: 3}}
  m_Name:
  m_EditorClassIdentifier:
  version: 10
"""


def write_mat(path: str, guid: str, name: str, tex_guid: str, tile_x: float, tile_y: float, smoothness: float) -> None:
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write(mat_yaml(name, tex_guid, tile_x, tile_y, smoothness))
    with open(path + ".meta", "w", encoding="utf-8", newline="\n") as f:
        f.write(
            f"""fileFormatVersion: 2
guid: {guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 2100000
  userData:
  assetBundleName:
  assetBundleVariant:
"""
        )


write_tex_meta(os.path.join(tex_dir, "tex_wallpaper_office.png.meta"), wall_tex_guid)
write_tex_meta(os.path.join(tex_dir, "tex_carpet_office.png.meta"), carpet_tex_guid)
write_mat(
    os.path.join(mat_dir, "Mat_Wallpaper_Office.mat"),
    wall_mat_guid,
    "Mat_Wallpaper_Office",
    wall_tex_guid,
    4,
    2,
    0.15,
)
write_mat(
    os.path.join(mat_dir, "Mat_Carpet_Office.mat"),
    carpet_mat_guid,
    "Mat_Carpet_Office",
    carpet_tex_guid,
    8,
    8,
    0.05,
)

with open(os.path.join(root, "Art", "Textures.meta"), "w", encoding="utf-8", newline="\n") as f:
    f.write(
        """fileFormatVersion: 2
guid: c1b2c3d4e5f64789a0b1c2d3e4f50801
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData:
  assetBundleName:
  assetBundleVariant:
"""
    )

scene = os.path.join(root, "Level0_Greybox.unity")
with open(scene, encoding="utf-8") as f:
    text = f.read()

replacements = [
    (
        "m_LocalPosition: {x: 10, y: 1.5, z: 0}\n  m_LocalScale: {x: 0.2, y: 3, z: 20}",
        "m_LocalPosition: {x: 10, y: 3, z: 0}\n  m_LocalScale: {x: 0.2, y: 6, z: 20}",
    ),
    (
        "m_LocalPosition: {x: 0, y: 1.5, z: 10}\n  m_LocalScale: {x: 20, y: 3, z: 0.2}",
        "m_LocalPosition: {x: 0, y: 3, z: 10}\n  m_LocalScale: {x: 20, y: 6, z: 0.2}",
    ),
    (
        "m_LocalPosition: {x: 0, y: 1.5, z: -10}\n  m_LocalScale: {x: 20, y: 3, z: 0.2}",
        "m_LocalPosition: {x: 0, y: 3, z: -10}\n  m_LocalScale: {x: 20, y: 6, z: 0.2}",
    ),
    (
        "m_LocalPosition: {x: -10, y: 1.5, z: 0}\n  m_LocalScale: {x: 0.2, y: 3, z: 20}",
        "m_LocalPosition: {x: -10, y: 3, z: 0}\n  m_LocalScale: {x: 0.2, y: 6, z: 20}",
    ),
    (
        "m_LocalPosition: {x: 1.75545, y: 3, z: 1.11996}",
        "m_LocalPosition: {x: 0, y: 5.2, z: 0}",
    ),
]

for old, new in replacements:
    if old not in text:
        print("MISSING:", old[:70])
    else:
        text = text.replace(old, new)
        print("OK:", old[:40])

default_mat = "- {fileID: 10303, guid: 0000000000000000f000000000000000, type: 0}"
wall_mat_ref = f"- {{fileID: 2100000, guid: {wall_mat_guid}, type: 2}}"
carpet_mat_ref = f"- {{fileID: 2100000, guid: {carpet_mat_guid}, type: 2}}"

print("default mat count", text.count(default_mat))
text = text.replace(default_mat, wall_mat_ref)

idx = text.find("m_Name: Plane")
m = text.find("m_Materials:", idx)
line_start = text.find(wall_mat_ref, m)
if 0 < line_start < idx + 2500:
    text = text[:line_start] + carpet_mat_ref + text[line_start + len(wall_mat_ref) :]
    print("plane carpet ok")
else:
    print("plane mat swap failed", line_start)

with open(scene, "w", encoding="utf-8", newline="\n") as f:
    f.write(text)

print("done")
