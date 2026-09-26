"""Draw the original robot icon and export PNG/ICO assets (requires Pillow)."""

from pathlib import Path

from PIL import Image, ImageDraw


OUT = Path(__file__).resolve().parent
image = Image.new("RGBA", (1024, 1024), (0, 0, 0, 0))
draw = ImageDraw.Draw(image)

outline = "#174457"
mint = "#E8F8F3"
teal = "#73D5C8"
screen = "#D5F5F2"
peach = "#FFAD91"

# Rounded tile and antenna give the icon a readable silhouette at 16 pixels.
draw.rounded_rectangle((64, 72, 960, 968), radius=222, fill=mint, outline=outline, width=35)
draw.line((512, 278, 512, 184), fill=outline, width=38)
draw.ellipse((459, 95, 565, 201), fill=peach, outline=outline, width=27)

# Ears sit behind the face shell.
draw.rounded_rectangle((118, 417, 251, 643), radius=65, fill=teal, outline=outline, width=30)
draw.rounded_rectangle((773, 417, 906, 643), radius=65, fill=teal, outline=outline, width=30)
draw.rounded_rectangle((201, 258, 823, 795), radius=160, fill="#FFFFFF", outline=outline, width=34)
draw.rounded_rectangle((260, 328, 764, 678), radius=116, fill=screen)

# Eyes, small highlights, cheeks, and a simple smile.
draw.ellipse((345, 418, 433, 534), fill=outline)
draw.ellipse((591, 418, 679, 534), fill=outline)
draw.ellipse((370, 439, 393, 469), fill="#FFFFFF")
draw.ellipse((616, 439, 639, 469), fill="#FFFFFF")
draw.ellipse((294, 539, 356, 583), fill=peach)
draw.ellipse((668, 539, 730, 583), fill=peach)
draw.arc((452, 488, 572, 607), 12, 168, fill=outline, width=24)

# A small fullwidth-comma badge connects the mascot to this utility.
draw.rounded_rectangle((367, 731, 657, 893), radius=78, fill=teal, outline=outline, width=30)
draw.ellipse((480, 765, 550, 835), fill="#FFFFFF")
draw.polygon(((524, 817), (555, 817), (533, 865), (495, 872)), fill="#FFFFFF")

image.save(OUT / "robot-icon.png")
image.save(
    OUT / "GrokBotChineseFix.ico",
    format="ICO",
    sizes=[(16, 16), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)],
)
