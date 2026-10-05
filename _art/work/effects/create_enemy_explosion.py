from PIL import Image, ImageDraw
from pathlib import Path
import math

OUT=Path(__file__).parent
SIZE=128
SCALE=4
C=SIZE*SCALE//2
N=8


def p(x,y): return (round(x*SCALE),round(y*SCALE))
def box(cx,cy,r): return (*p(cx-r,cy-r),*p(cx+r,cy+r))
def poly_points(cx,cy,radii,rotation=0):
    pts=[]
    for i,r in enumerate(radii):
        a=rotation+2*math.pi*i/len(radii)
        pts.append(p(cx+math.cos(a)*r,cy+math.sin(a)*r))
    return pts

def burst(draw,cx,cy,outer,inner,fill,outline=None,width=2,rotation=0):
    radii=[outer if i%2==0 else inner for i in range(20)]
    points=poly_points(cx,cy,radii,rotation)
    draw.polygon(points,fill=fill)
    if outline: draw.line(points+[points[0]],fill=outline,width=width*SCALE,joint='curve')

def ring(draw,cx,cy,r,color,width):
    draw.ellipse(box(cx,cy,r),outline=color,width=round(width*SCALE))

def dot(draw,cx,cy,r,color): draw.ellipse(box(cx,cy,r),fill=color)

def spark(draw,cx,cy,r,angle,size,color):
    x=cx+math.cos(angle)*r; y=cy+math.sin(angle)*r
    a=angle
    points=[p(x+math.cos(a)*size*1.8,y+math.sin(a)*size*1.8),
            p(x+math.cos(a+2.25)*size,y+math.sin(a+2.25)*size),
            p(x-math.cos(a)*size*.7,y-math.sin(a)*size*.7),
            p(x+math.cos(a-2.25)*size,y+math.sin(a-2.25)*size)]
    draw.polygon(points,fill=color)


def frame(i):
    im=Image.new('RGBA',(SIZE*SCALE,SIZE*SCALE),(0,0,0,0))
    d=ImageDraw.Draw(im)
    if i==0:
        burst(d,64,64,17,10,(255,110,54,255),(67,46,48,255),2,0.1)
        burst(d,64,64,11,7,(255,245,190,255),None,0,0.1)
        for a in (0,math.pi/2,math.pi,3*math.pi/2): spark(d,64,64,25,a,3,(255,220,110,240))
    elif i==1:
        burst(d,64,64,36,24,(255,112,55,255),(67,46,48,255),3,0.02)
        burst(d,64,64,27,17,(255,190,75,255),None,0,0.15)
        burst(d,64,64,17,12,(255,251,220,255),None,0,0.25)
        for j in range(8): spark(d,64,64,46,j*math.tau/8+.1,3,(255,231,143,255))
    elif i==2:
        burst(d,64,64,51,35,(255,100,54,255),(67,46,48,255),3,0.12)
        burst(d,64,64,40,28,(255,181,72,255),None,0,0.2)
        burst(d,64,64,27,20,(255,248,187,255),None,0,0.06)
        burst(d,64,64,13,10,(255,255,245,255),None,0,0.12)
        for j in range(10): spark(d,64,64,58,j*math.tau/10+.2,2.5,(255,201,97,240))
    elif i==3:
        ring(d,64,64,45,(77,49,51,255),5)
        ring(d,64,64,43,(255,180,70,255),5)
        burst(d,64,64,31,24,(255,131,57,230),None,0,0.17)
        burst(d,64,64,20,15,(255,240,163,255),None,0,0.09)
        for j in range(8):
            a=j*math.tau/8+.18
            spark(d,64,64,55,a,4,(255,126,54,255))
            dot(d,64+math.cos(a)*61,64+math.sin(a)*61,2.5,(255,217,114,255))
    elif i==4:
        ring(d,64,64,54,(226,133,66,175),4)
        ring(d,64,64,50,(255,224,126,190),3)
        for j in range(7):
            a=j*math.tau/7+.18
            spark(d,64,64,57,a,3.6,(250,133,59,210))
            dot(d,64+math.cos(a)*38,64+math.sin(a)*38,4.2,(255,197,110,150))
        dot(d,64,64,13,(255,232,164,110))
    elif i==5:
        ring(d,64,64,59,(255,211,127,105),3)
        for j in range(8):
            a=j*math.tau/8+.1
            dot(d,64+math.cos(a)*46,64+math.sin(a)*46,4.6,(173,158,145,95))
            if j%2==0: spark(d,64,64,61,a,2.6,(255,160,76,150))
    elif i==6:
        for j in range(7):
            a=j*math.tau/7+.08
            dot(d,64+math.cos(a)*54,64+math.sin(a)*54,4,(173,158,145,65))
            if j%2==0: dot(d,64+math.cos(a)*60,64+math.sin(a)*60,2,(255,182,96,100))
    else:
        for j in range(5):
            a=j*math.tau/5+.12
            dot(d,64+math.cos(a)*58,64+math.sin(a)*58,2.5,(180,170,154,35))
    return im.resize((SIZE,SIZE),Image.Resampling.LANCZOS)

frames=[frame(i) for i in range(N)]
sheet=Image.new('RGBA',(SIZE*N,SIZE),(0,0,0,0))
for i,im in enumerate(frames): sheet.alpha_composite(im,(i*SIZE,0))
sheet.save(OUT/'enemy_explosion.png')

# Developer-facing visual storyboard on an office-floor color.
story=Image.new('RGB',(SIZE*4, (SIZE+20)*2),(91,104,105)); ds=ImageDraw.Draw(story)
for i,im in enumerate(frames):
    x=(i%4)*SIZE; y=(i//4)*(SIZE+20)
    story.paste(im,(x,y),im)
    ds.text((x+5,y+SIZE),f'{i}',fill=(245,246,239))
story.save(OUT/'enemy_explosion_storyboard.png')

# Motion preview only; Godot consumes the PNG sheet. Each frame 85 ms, one-shot in game.
gif_frames=[]
for im in frames:
    bg=Image.new('RGBA',(SIZE,SIZE),(91,104,105,255)); bg.alpha_composite(im)
    gif_frames.append(bg.convert('RGB'))
gif_frames[0].save(OUT/'enemy_explosion_preview.gif',save_all=True,append_images=gif_frames[1:],duration=85,loop=0,disposal=2)
print('sprite sheet',sheet.size,'frames',N,'frame size',SIZE,'preview fps',round(1000/85,1))
