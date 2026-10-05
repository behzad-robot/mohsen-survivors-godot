from PIL import Image, ImageDraw
from pathlib import Path

ROOT=Path(__file__).parent
S=8
DARK='#202735'
STEEL='#9fb2bc'
LITE='#eaf4f1'
MID='#566976'

def canvas(size):
    im=Image.new('RGBA',(size[0]*S,size[1]*S),(0,0,0,0))
    return im,ImageDraw.Draw(im)

def pts(coords): return [(round(x*S),round(y*S)) for x,y in coords]
def rect(d,box,fill,outline=None,width=1):
    d.rectangle(tuple(round(v*S) for v in box),fill=fill,outline=outline,width=round(width*S))
def poly(d,coords,fill): d.polygon(pts(coords),fill=fill)
def line(d,coords,fill,width=1): d.line(pts(coords),fill=fill,width=round(width*S),joint='curve')
def ellipse(d,box,fill,outline=None,width=1):
    d.ellipse(tuple(round(v*S) for v in box),fill=fill,outline=outline,width=round(width*S))
def save(im,path,size): im.resize(size,Image.Resampling.LANCZOS).save(path)

# Right-facing floating knife, matching the existing 34x14 placeholder footprint.
im,d=canvas((36,16))
poly(d,[(1,5),(12,5),(12,11),(1,11)],DARK)
rect(d,(3,6,10,10),'#785645')
line(d,[(5,6),(5,10)],'#a98362',1)
line(d,[(8,6),(8,10)],'#a98362',1)
rect(d,(11,3,14,13),DARK)
rect(d,(12,4,13,12),'#d4b27a')
poly(d,[(14,3),(26,3),(35,8),(26,13),(14,13)],DARK)
poly(d,[(15,5),(25,5),(32,8),(25,11),(15,11)],STEEL)
poly(d,[(16,5),(25,5),(29,7),(16,7)],LITE)
line(d,[(17,10),(25,10),(31,8)],'#627b88',1)
save(im,ROOT/'weapons/knife.png',(36,16))

# Right-facing pistol: short barrel and a hooked grip.
im,d=canvas((36,24))
poly(d,[(2,5),(31,5),(34,8),(34,13),(21,13),(18,21),(9,21),(11,13),(2,13)],DARK)
poly(d,[(4,7),(29,7),(31,9),(31,11),(19,11),(16,19),(11,19),(13,11),(4,11)],MID)
rect(d,(6,7,25,8),'#8fa4ad')
rect(d,(27,8,33,10),'#37454e')
rect(d,(12,13,15,18),'#394650')
line(d,[(18,13),(21,13),(19,16)],'#c3d2d5',1)
rect(d,(6,4,10,5),DARK)
save(im,ROOT/'weapons/pistol.png',(36,24))

# Right-facing SMG: long muzzle, rear stock and distinct magazine.
im,d=canvas((44,24))
poly(d,[(1,7),(7,7),(9,5),(31,5),(33,7),(42,7),(43,11),(33,11),(32,14),(26,14),(24,22),(18,22),(19,14),(13,14),(11,21),(6,21),(7,13),(1,13)],DARK)
poly(d,[(3,9),(8,9),(10,7),(30,7),(31,9),(39,9),(39,10),(31,10),(30,12),(14,12),(11,19),(8,19),(9,11),(3,11)],'#627987')
poly(d,[(21,13),(26,13),(22,20),(19,20)],'#455866')
rect(d,(11,7,26,8),'#a9bdc3')
rect(d,(34,8,43,10),'#43545c')
for x in (17,20,23): line(d,[(x,10),(x+1,10)],'#29343d',1)
line(d,[(27,13),(29,13),(27,16)],'#b5c7c9',1)
save(im,ROOT/'weapons/smg.png',(44,24))

# Pistol round: warm bright bolt with a small orange tail.
im,d=canvas((14,12))
ellipse(d,(1,4,5,8),(255,156,44,100))
line(d,[(2,6),(6,6)],'#f7a12b',2)
ellipse(d,(5,2,13,10),'#422d29')
ellipse(d,(6,3,12,9),'#ffcd45')
ellipse(d,(8,4,11,7),'#fff8bb')
save(im,ROOT/'projectiles/pistol_bullet.png',(14,12))

# Faster SMG tracer: smaller gold dart with compact tail.
im,d=canvas((12,10))
line(d,[(1,5),(5,5)],'#ff9d2e',2)
poly(d,[(4,2),(9,2),(11,5),(9,8),(4,8),(2,5)],'#4a3029')
poly(d,[(5,3),(9,3),(10,5),(9,7),(5,7),(4,5)],'#ffe15b')
line(d,[(6,4),(8,4)],'#fff8c8',1)
save(im,ROOT/'projectiles/smg_bullet.png',(12,10))

# Ranged-enemy mail: red bullet-hell color, envelope silhouette, white fold.
im,d=canvas((26,19))
ellipse(d,(1,1,25,18),(255,34,65,65))
poly(d,[(3,3),(22,3),(25,9),(22,16),(3,16),(1,10)],'#421325')
poly(d,[(4,5),(21,5),(23,9),(21,14),(4,14),(3,10)],'#ff2347')
poly(d,[(4,6),(12,11),(21,6)],'#ff8293')
line(d,[(4,6),(12,11),(21,6)],'#fff0ed',1.4)
line(d,[(4,14),(10,9)],'#fcb7b4',1)
line(d,[(21,14),(15,9)],'#fcb7b4',1)
rect(d,(18,6,20,8),'#ffe9db')
save(im,ROOT/'projectiles/enemy_projectile.png',(26,19))
print('created six combat PNG candidates')
