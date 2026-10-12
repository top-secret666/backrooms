from PIL import Image, ImageDraw, ImageFilter, ImageEnhance
import random
import os

out = r"D:\HUH\backrooms\Assets\_Project\Art\Textures"
os.makedirs(out, exist_ok=True)
rng = random.Random(42)

W, H = 1024, 1024
wall = Image.new("RGB", (W, H), (232, 230, 224))
px = wall.load()
for x in range(0, W, 64):
    shade = 205 + rng.randint(-8, 8)
    for dx in range(-1, 2):
        xx = x + dx
        if 0 <= xx < W:
            for y in range(H):
                px[xx, y] = (shade, shade - 2, shade - 6)

for _ in range(12000):
    x, y = rng.randint(0, W - 1), rng.randint(0, H - 1)
    d = rng.randint(-12, 8)
    r, g, b = px[x, y]
    px[x, y] = (
        max(0, min(255, r + d)),
        max(0, min(255, g + d - 1)),
        max(0, min(255, b + d - 2)),
    )

draw = ImageDraw.Draw(wall, "RGBA")
for _ in range(18):
    x = rng.randint(0, W)
    y = rng.randint(H // 2, H)
    w = rng.randint(40, 140)
    h = rng.randint(80, 280)
    col = (170, 175, 160, rng.randint(25, 55))
    draw.ellipse([x - w, y - h, x + w, y + h], fill=col)

wall = wall.filter(ImageFilter.GaussianBlur(0.6))
wall = ImageEnhance.Color(wall).enhance(0.85)
wall_path = os.path.join(out, "tex_wallpaper_office.png")
wall.save(wall_path)

floor = Image.new("RGB", (W, H), (110, 108, 100))
fpx = floor.load()
for y in range(H):
    for x in range(W):
        n = rng.randint(-18, 18)
        weave = 6 if ((x // 2) ^ (y // 2)) & 1 else -4
        v = 110 + n + weave
        fpx[x, y] = (
            max(0, min(255, v)),
            max(0, min(255, v - 2)),
            max(0, min(255, v - 8)),
        )

draw = ImageDraw.Draw(floor, "RGBA")
for _ in range(8):
    y = rng.randint(0, H)
    draw.rectangle([0, y, W, y + rng.randint(8, 20)], fill=(90, 88, 82, 40))

floor = floor.filter(ImageFilter.GaussianBlur(0.4))
floor_path = os.path.join(out, "tex_carpet_office.png")
floor.save(floor_path)

# Drop-ceiling acoustic tiles (2x2 ft grid), warm off-white
ceiling = Image.new("RGB", (W, H), (238, 236, 228))
cpx = ceiling.load()
tile = 128
for ty in range(0, H, tile):
    for tx in range(0, W, tile):
        base = 228 + rng.randint(-6, 6)
        for y in range(ty, min(ty + tile, H)):
            for x in range(tx, min(tx + tile, W)):
                edge = min(x - tx, tx + tile - 1 - x, y - ty, ty + tile - 1 - y)
                v = base - (3 if edge < 2 else 0) + rng.randint(-4, 4)
                cpx[x, y] = (
                    max(0, min(255, v)),
                    max(0, min(255, v - 1)),
                    max(0, min(255, v - 5)),
                )
draw = ImageDraw.Draw(ceiling)
for x in range(0, W, tile):
    draw.line([(x, 0), (x, H)], fill=(200, 198, 188), width=2)
for y in range(0, H, tile):
    draw.line([(0, y), (W, y)], fill=(200, 198, 188), width=2)
ceiling = ceiling.filter(ImageFilter.GaussianBlur(0.35))
ceiling_path = os.path.join(out, "tex_ceiling_office.png")
ceiling.save(ceiling_path)

print(wall_path)
print(floor_path)
print(ceiling_path)
