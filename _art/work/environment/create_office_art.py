from PIL import Image, ImageDraw
from pathlib import Path
import random

out=Path('_art/work/environment')
# Quiet 256 px repeatable carpet tile: four 128 px modular squares.
floor=Image.new('RGB',(256,256),'#697577')
d=ImageDraw.Draw(floor)
colors=['#697577','#6c7779']
for row in range(2):
    for col in range(2):
        x,y=col*128,row*128
        d.rectangle((x,y,x+127,y+127),fill=colors[(row+col)%2])
        d.line((x,y,x+127,y),fill='#637073',width=1)
        d.line((x,y,x,y+127),fill='#637073',width=1)
        d.line((x+1,y+1,x+126,y+1),fill='#6f7a7c',width=1)
        d.line((x+1,y+1,x+1,y+126),fill='#6f7a7c',width=1)
# Tiny low-contrast carpet fibers. Fixed seed keeps the tile reproducible.
rng=random.Random(1704)
for _ in range(370):
    x,y=rng.randrange(256),rng.randrange(256)
    color=rng.choice(['#707a7b','#667173','#718082','#657071'])
    d.point((x,y),fill=color)
floor.save(out/'office_floor.png')

# 3x3 atlas. Each 96x96 cell is a wall corner/edge/empty center.
S=96
atlas=Image.new('RGBA',(S*3,S*3),(0,0,0,0))
def tile(top=False,bottom=False,left=False,right=False):
    im=Image.new('RGBA',(S,S),(0,0,0,0)); p=im.load()
    for y in range(S):
        for x in range(S):
            dist=[]
            if top: dist.append(y)
            if bottom: dist.append(S-1-y)
            if left: dist.append(x)
            if right: dist.append(S-1-x)
            if not dist: continue
            z=min(dist)
            if z<4: color=(39,48,54,255)
            elif z<25: color=(82,96,102,255)
            elif z<28: color=(115,128,133,255)
            elif z<32: color=(29,40,47,255)
            elif z<36: color=(28,39,45,70)
            else: continue
            p[x,y]=color
    dr=ImageDraw.Draw(im)
    # Panel divisions follow the wall direction and align at tile boundaries.
    if top or bottom:
        y=15 if top else S-16
        dr.line((S-1,y,S-1,y+10),fill=(48,60,66,180),width=1)
    if left or right:
        x=15 if left else S-16
        dr.line((x,S-1,x+10,S-1),fill=(48,60,66,180),width=1)
    return im
layout=[
    [(True,False,True,False),(True,False,False,False),(True,False,False,True)],
    [(False,False,True,False),(False,False,False,False),(False,False,False,True)],
    [(False,True,True,False),(False,True,False,False),(False,True,False,True)],
]
for row,cells in enumerate(layout):
    for col,args in enumerate(cells):
        atlas.alpha_composite(tile(*args),(col*S,row*S))
atlas.save(out/'arena_wall.png')

# 1280x720 visual mockup. Walls bound this presentation only; game arena size is separate.
preview=Image.new('RGBA',(1280,720),(0,0,0,0))
for y in range(0,720,256):
    for x in range(0,1280,256):
        preview.paste(floor,(x,y))
# Build a continuous-looking border from the atlas cells.
def cell(col,row): return atlas.crop((col*S,row*S,(col+1)*S,(row+1)*S))
for x in range(0,1280,S):
    preview.alpha_composite(cell(1,0),(x,0))
    preview.alpha_composite(cell(1,2),(x,720-S))
for y in range(0,720,S):
    preview.alpha_composite(cell(0,1),(0,y))
    preview.alpha_composite(cell(2,1),(1280-S,y))
for col,row,x,y in [(0,0,0,0),(2,0,1280-S,0),(0,2,0,720-S),(2,2,1280-S,720-S)]:
    preview.alpha_composite(cell(col,row),(x,y))
# Scale figures to their reviewed display heights.
def figure(path,height):
    im=Image.open(path).convert('RGBA')
    box=im.getchannel('A').point(lambda a:255 if a>8 else 0).getbbox()
    im=im.crop(box)
    return im.resize((round(im.width*height/im.height),height),Image.Resampling.LANCZOS)
chars=Path('_art/work/characters')
enemies=Path('_art/work/enemies')
for path,height,x,y in [
    (chars/'mohsen_concept_v3.png',60,610,365),
    (enemies/'jira_ticket_concept_v2.png',42,735,385),
    (enemies/'mail_slime_concept_v1.png',46,465,280),
]:
    preview.alpha_composite(figure(path,height),(x,y))
preview.convert('RGB').save(out/'office_arena_preview_v1.png')
print('saved floor 256x256, wall atlas 288x288, preview 1280x720')
