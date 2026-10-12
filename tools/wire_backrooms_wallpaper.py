import os

dst = r"D:\HUH\backrooms\Assets\_Project\Art\Textures\BackroomsWallpaper"
guids = {
    "basecolor": "e1f2a3b4c5d64789a0b1c2d3e4f51001",
    "normal": "e1f2a3b4c5d64789a0b1c2d3e4f51002",
    "ao": "e1f2a3b4c5d64789a0b1c2d3e4f51003",
    "height": "e1f2a3b4c5d64789a0b1c2d3e4f51004",
    "roughness": "e1f2a3b4c5d64789a0b1c2d3e4f51005",
    "smoothness": "e1f2a3b4c5d64789a0b1c2d3e4f51006",
}


def tex_meta(guid: str, texture_type: int = 0, srgb: int = 1) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 1
    sRGBTexture: {srgb}
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
    aniso: 4
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
  textureType: {texture_type}
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


for key, guid in guids.items():
    path = os.path.join(dst, f"Wallpaper_Backrooms_001_{key}.png")
    ttype = 1 if key == "normal" else 0
    srgb = 0 if key != "basecolor" else 1
    with open(path + ".meta", "w", encoding="utf-8", newline="\n") as f:
        f.write(tex_meta(guid, ttype, srgb))
    print("meta", key)

with open(dst + ".meta", "w", encoding="utf-8", newline="\n") as f:
    f.write(
        """fileFormatVersion: 2
guid: e1f2a3b4c5d64789a0b1c2d3e4f51000
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData:
  assetBundleName:
  assetBundleVariant:
"""
    )

bg = guids["basecolor"]
ng = guids["normal"]
ag = guids["ao"]

mat_path = r"D:\HUH\backrooms\Assets\_Project\Art\Materials\Mat_Wallpaper_Office.mat"
with open(mat_path, encoding="utf-8") as f:
    lines = f.read().splitlines()

out = []
i = 0
while i < len(lines):
    line = lines[i]
    if line.strip() == "m_ValidKeywords: []":
        out.append("  m_ValidKeywords:")
        out.append("  - _NORMALMAP")
        i += 1
        continue

    if line.strip() in ("- _BaseMap:", "- _MainTex:"):
        out.append(line)
        i += 1
        out.append(f"        m_Texture: {{fileID: 2800000, guid: {bg}, type: 3}}")
        i += 1  # skip old texture line
        while i < len(lines) and not lines[i].lstrip().startswith("- "):
            if "m_Scale:" in lines[i]:
                out.append("        m_Scale: {x: 2, y: 2}")
            else:
                out.append(lines[i])
            i += 1
        continue

    if line.strip() == "- _BumpMap:":
        out.append(line)
        i += 1
        out.append(f"        m_Texture: {{fileID: 2800000, guid: {ng}, type: 3}}")
        i += 1
        while i < len(lines) and not lines[i].lstrip().startswith("- "):
            out.append(lines[i])
            i += 1
        continue

    if line.strip() == "- _OcclusionMap:":
        out.append(line)
        i += 1
        out.append(f"        m_Texture: {{fileID: 2800000, guid: {ag}, type: 3}}")
        i += 1
        while i < len(lines) and not lines[i].lstrip().startswith("- "):
            out.append(lines[i])
            i += 1
        continue

    if "- _Smoothness:" in line:
        out.append("    - _Smoothness: 0.22")
        i += 1
        continue
    if "- _BaseColor:" in line:
        out.append("    - _BaseColor: {r: 1, g: 1, b: 1, a: 1}")
        i += 1
        continue
    if line.strip().startswith("- _Color:") and "Emission" not in line:
        out.append("    - _Color: {r: 1, g: 1, b: 1, a: 1}")
        i += 1
        continue

    out.append(line)
    i += 1

with open(mat_path, "w", encoding="utf-8", newline="\n") as f:
    f.write("\n".join(out) + "\n")

print("material updated")
