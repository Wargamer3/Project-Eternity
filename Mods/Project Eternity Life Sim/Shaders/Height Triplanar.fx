matrix WorldViewProj;
float TileMeters;
float3 ChunkOrigin;
float3 Eye;
float3 SunDir = float3(0.35, 0.86, 0.25);
float3 FogColor = float3(0.76, 0.81, 0.84);
float CascadeCast;
float4 CascadeSplit = float4(160, 640, 2560, 24);
float4 CascadeTexel = float4(1.0 / 1024.0, 1.0 / 512.0, 1.0 / 256.0, 0.0);
matrix CameraView;
matrix CascadeView0;
matrix CascadeView1;
matrix CascadeView2;
float4 Lamps[64];
int LampCount;
float2 LampGrid = float2(1.0, 1.0);
float2 LampOrigin = float2(0.0, 0.0);
float3 LampColor = float3(1.0, 0.78, 0.42);
float4 Contacts[24];
int ContactCount;
float Cloud = 0.22;
float CloudTime;
float RainWet = 0;
float Flash = 1;

texture LampTiles;
sampler LampTileSampler = sampler_state
{
    Texture = (LampTiles);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture Cascade0;
sampler CascadeSampler0 = sampler_state
{
    Texture = (Cascade0);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture Cascade1;
sampler CascadeSampler1 = sampler_state
{
    Texture = (Cascade1);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture Cascade2;
sampler CascadeSampler2 = sampler_state
{
    Texture = (Cascade2);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture ColorAtlas;
sampler ColorSampler = sampler_state
{
    Texture = (ColorAtlas);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture NormalAtlas;
sampler NormalSampler = sampler_state
{
    Texture = (NormalAtlas);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture RoughAtlas;
sampler RoughSampler = sampler_state
{
    Texture = (RoughAtlas);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

static const float2 GrassTile = float2(0, 0);
static const float2 DirtTile = float2(1, 0);
static const float2 RockTile = float2(2, 0);
static const float2 DampTile = float2(2, 1);
static const float CellPad = 32.0 / 1024.0;
static const float CellSpan = (1024.0 - 64.0) / 1024.0;

struct VSInput
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Blend : TEXCOORD0;
    float Wet : TEXCOORD1;
    float Shade : TEXCOORD2;
    float Open : TEXCOORD3;
};

struct VSOutput
{
    float4 Position : SV_Position;
    float3 World : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float4 Blend : TEXCOORD2;
    float Wet : TEXCOORD3;
    float Shade : TEXCOORD4;
    float Open : TEXCOORD5;
};

VSOutput VS(VSInput input)
{
    VSOutput output;
    float3 world = input.Position + ChunkOrigin;
    output.Position = mul(float4(input.Position + ChunkOrigin, 1), WorldViewProj);
    output.World = world;
    output.Normal = input.Normal;
    output.Blend = input.Blend;
    output.Wet = input.Wet;
    output.Shade = input.Shade;
    output.Open = input.Open;
    return output;
}

#define SAMPLE_ATLAS(Surface, WorldUv, Tile) tex2Dgrad( \
    Surface, \
    ((Tile) + frac(WorldUv) * CellSpan + CellPad) / float2(3.0, 2.0), \
    ddx(WorldUv) * float2(CellSpan / 3.0, CellSpan / 2.0), \
    ddy(WorldUv) * float2(CellSpan / 3.0, CellSpan / 2.0))

float Burley(float NdotL, float NdotV, float LdotH, float Rough)
{
    float Fd90 = 0.5 + 2.0 * Rough * LdotH * LdotH;
    float ScatterL = 1.0 + (Fd90 - 1.0) * pow(1.0 - NdotL, 5.0);
    float ScatterV = 1.0 + (Fd90 - 1.0) * pow(1.0 - NdotV, 5.0);
    return NdotL * ScatterL * ScatterV;
}

float3 LampGlow(float3 World, float3 Normal, float3 Albedo, float Rough, float Lift, float2 Screen)
{
    if (LampCount < 1)
    {
        return 0;
    }

    float3 Add = 0;
    float Night = lerp(1.0, 0.12, Lift);
    float3 View = normalize(Eye - World);
    float NdotV = saturate(dot(Normal, View));
    float R = saturate(Rough);
    float2 Tile = floor((Screen - LampOrigin) / 16.0);
    float4 Ids = tex2Dlod(LampTileSampler, float4((Tile + 0.5) / max(LampGrid, float2(1.0, 1.0)), 0.0, 0.0));
    [loop]
    for (int i = 0; i < 4; i++)
    {
        float Packed = i == 0 ? Ids.r : (i == 1 ? Ids.g : (i == 2 ? Ids.b : Ids.a));
        int Index = (int)round(Packed * 255.0) - 1;
        if (Index < 0 || Index >= LampCount)
        {
            continue;
        }

        float3 To = Lamps[Index].xyz - World;
        float Dist = length(To);
        float Fall = saturate(1.0 - Dist / max(Lamps[Index].w, 0.01));
        Fall *= Fall;
        float3 Light = To / max(Dist, 0.001);
        float NdotL = saturate(dot(Normal, Light));
        float3 Half = normalize(Light + View);
        float Body = Burley(NdotL, NdotV, saturate(dot(Light, Half)), R);
        Add += Albedo * LampColor * Fall * (0.4 + 0.6 * Body);
    }

    return Add * Night;
}

float Tap(sampler Map, float2 Uv, float Here, float Bias)
{
    return Here <= tex2D(Map, Uv).r + Bias ? 1.0 : 0.0;
}

// Tent filter across one shadow texel, so the edge fades instead of stepping.
float Grid(sampler Map, float2 Uv, float Here, float Bias, float TexelSize)
{
    float2 T = float2(TexelSize, TexelSize);
    float Lit = Tap(Map, Uv, Here, Bias) * 4.0;
    Lit += Tap(Map, Uv + T * float2(-1, 0), Here, Bias) * 2.0;
    Lit += Tap(Map, Uv + T * float2(1, 0), Here, Bias) * 2.0;
    Lit += Tap(Map, Uv + T * float2(0, -1), Here, Bias) * 2.0;
    Lit += Tap(Map, Uv + T * float2(0, 1), Here, Bias) * 2.0;
    Lit += Tap(Map, Uv + T * float2(-1, -1), Here, Bias);
    Lit += Tap(Map, Uv + T * float2(1, -1), Here, Bias);
    Lit += Tap(Map, Uv + T * float2(-1, 1), Here, Bias);
    Lit += Tap(Map, Uv + T * float2(1, 1), Here, Bias);
    return Lit * (1.0 / 16.0);
}

float LitTexels(sampler Map, float4x4 ViewProjShadow, float3 World, float TexelSize)
{
    float4 Clip = mul(float4(World, 1), ViewProjShadow);
    float2 Uv = Clip.xy / Clip.w * float2(0.5, -0.5) + 0.5;
    if (Uv.x < 0.0 || Uv.y < 0.0 || Uv.x > 1.0 || Uv.y > 1.0)
    {
        return 1.0;
    }

    float Here = Clip.z / Clip.w;
    if (Here < 0.0 || Here > 1.0)
    {
        return 1.0;
    }

    return Grid(Map, Uv, Here, 0.00015, TexelSize);
}

float CascadeLit(float3 World)
{
    if (CascadeCast < 0.5)
    {
        return 1.0;
    }

    float Depth = -mul(float4(World, 1), CameraView).z;
    float Blend = max(CascadeSplit.w, 0.5);
    if (Depth < CascadeSplit.x)
    {
        float Near = LitTexels(CascadeSampler0, CascadeView0, World, CascadeTexel.x);
        if (Depth < CascadeSplit.x - Blend)
        {
            return Near;
        }

        return lerp(LitTexels(CascadeSampler1, CascadeView1, World, CascadeTexel.y), Near, saturate((CascadeSplit.x - Depth) / Blend));
    }

    if (Depth < CascadeSplit.y)
    {
        float Mid = LitTexels(CascadeSampler1, CascadeView1, World, CascadeTexel.y);
        if (Depth < CascadeSplit.y - Blend)
        {
            return Mid;
        }

        return lerp(LitTexels(CascadeSampler2, CascadeView2, World, CascadeTexel.z), Mid, saturate((CascadeSplit.y - Depth) / Blend));
    }

    return LitTexels(CascadeSampler2, CascadeView2, World, CascadeTexel.z);
}

float Hash12(float2 p)
{
    return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
}

float FadeNoise(float2 p)
{
    float2 cell = floor(p);
    float2 f = frac(p);
    f = f * f * (3.0 - 2.0 * f);
    float a = Hash12(cell);
    float b = Hash12(cell + float2(1, 0));
    float c = Hash12(cell + float2(0, 1));
    float d = Hash12(cell + float2(1, 1));
    return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
}

float ContactOpen(float3 World)
{
    float Dark = 1.0;
    [loop]
    for (int i = 0; i < 24; i++)
    {
        if (i >= ContactCount)
        {
            break;
        }

        float Rise = World.y - Contacts[i].y;
        if (Rise > 4.0 || Rise < -8.0)
        {
            continue;
        }

        float Dist = length(World.xz - Contacts[i].xz);
        float Radius = max(Contacts[i].w, 0.01);
        float Along = saturate(Dist / Radius);
        float Shade = lerp(0.52, 1.0, smoothstep(0.45, 1.0, Along));
        Dark = min(Dark, Shade);
    }

    return Dark;
}

float CloudShade(float2 Ground)
{
    float Cover = saturate((Cloud - 0.28) * 1.5);
    float2 Uv = Ground * 0.0015 + float2(CloudTime, CloudTime * 0.37) * 0.02;
    float Soft = FadeNoise(Uv);
    float Puff = FadeNoise(Uv * 2.15 + 13.7);
    float Mask = smoothstep(0.40, 0.74, Soft * 0.62 + Puff * 0.38);
    return lerp(1.0, lerp(0.86, 0.52, Cover), Mask * Cover);
}

float2 Spin(float2 p)
{
    return float2(p.x * 0.8910065 - p.y * 0.4539905, p.x * 0.4539905 + p.y * 0.8910065);
}

float3 UnpackNormal(float4 Sample)
{
    float3 Packed = Sample.rgb * 2.0 - 1.0;
    Packed.xy *= 1.45;
    return Packed;
}

float3 FlatNormal(float3 Tangent, float3 Geo)
{
    float3 Bent = float3(Tangent.xy + Geo.xz, abs(Tangent.z) * Geo.y);
    return Bent.xzy;
}

float3 SteepNormal(float3 TX, float3 TY, float3 TZ, float3 Geo, float3 Blend)
{
    float3 BentX = float3(TX.xy + Geo.zy, abs(TX.z) * Geo.x);
    float3 BentY = float3(TY.xy + Geo.xz, abs(TY.z) * Geo.y);
    float3 BentZ = float3(TZ.xy + Geo.xy, abs(TZ.z) * Geo.z);
    return BentX.zyx * Blend.x + BentY.xzy * Blend.y + BentZ.xyz * Blend.z;
}

#define SURFACE_COLOR(Tile, Dest) \
    if (weight.y > 0.86) Dest = SAMPLE_ATLAS(ColorSampler, input.World.xz * scale, Tile).rgb; \
    else Dest = SAMPLE_ATLAS(ColorSampler, input.World.zy * scale, Tile).rgb * weight.x + SAMPLE_ATLAS(ColorSampler, input.World.xz * scale, Tile).rgb * weight.y + SAMPLE_ATLAS(ColorSampler, input.World.xy * scale, Tile).rgb * weight.z;

#define SURFACE_NORMAL(Tile, Dest) \
    if (weight.y > 0.86) Dest = normal; \
    else Dest = SteepNormal( \
        UnpackNormal(SAMPLE_ATLAS(NormalSampler, input.World.zy * scale, Tile)), \
        UnpackNormal(SAMPLE_ATLAS(NormalSampler, input.World.xz * scale, Tile)), \
        UnpackNormal(SAMPLE_ATLAS(NormalSampler, input.World.xy * scale, Tile)), \
        normal, weight);

#define SURFACE_ROUGH(Tile, Dest) \
    if (weight.y > 0.86) Dest = 0.82; \
    else Dest = SAMPLE_ATLAS(RoughSampler, input.World.zy * scale, Tile).r * weight.x + SAMPLE_ATLAS(RoughSampler, input.World.xz * scale, Tile).r * weight.y + SAMPLE_ATLAS(RoughSampler, input.World.xy * scale, Tile).r * weight.z;

float4 PS(VSOutput input) : COLOR0
{
    float3 normal = normalize(input.Normal);
    float3 weight = abs(normal);
    weight = pow(weight, 4);
    weight /= max(weight.x + weight.y + weight.z, 0.0001);

    float scale = TileMeters > 0.01 ? 1.0 / TileMeters : 0.06;

    float4 blend = input.Blend;
    float sum = blend.x + blend.y + blend.z + blend.w;
    if (sum < 0.001)
    {
        blend = float4(1, 0, 0, 0);
        sum = 1;
    }

    blend /= sum;
    float slope = saturate(normal.y);
    float puddle = lerp(0.42, 1.0, FadeNoise(input.World.xz * 0.05));
    float poured = input.Wet > 0.985 ? 1.0 : input.Wet * puddle;
    float wet = saturate(poured) * (1.0 - blend.z) * (1.0 - blend.w * 0.65);
    float dampMask = wet * (1.0 - blend.z) * (1.0 - blend.w);

    float3 grass = 0;
    float3 dirt = 0;
    float3 rock = 0;
    float3 damp = 0;
    float3 grassN = normal;
    float3 dirtN = normal;
    float3 rockN = normal;
    float3 dampN = normal;
    float grassR = 0.8;
    float dirtR = 0.8;
    float rockR = 0.8;
    float dampR = 0.4;
    if (blend.x > 0.02)
    {
        SURFACE_COLOR(GrassTile, grass)
        SURFACE_NORMAL(GrassTile, grassN)
        SURFACE_ROUGH(GrassTile, grassR)
    }

    if (blend.y > 0.02)
    {
        SURFACE_COLOR(DirtTile, dirt)
        SURFACE_NORMAL(DirtTile, dirtN)
        SURFACE_ROUGH(DirtTile, dirtR)
    }

    if (blend.z > 0.02)
    {
        SURFACE_COLOR(RockTile, rock)
        SURFACE_NORMAL(RockTile, rockN)
        SURFACE_ROUGH(RockTile, rockR)
    }

    if (dampMask > 0.02)
    {
        SURFACE_COLOR(DampTile, damp)
        SURFACE_NORMAL(DampTile, dampN)
        SURFACE_ROUGH(DampTile, dampR)
    }

    float3 albedo = grass * blend.x + dirt * blend.y + rock * blend.z;
    albedo = lerp(albedo, float3(0.90, 0.92, 0.94), blend.w);
    albedo = lerp(albedo, damp, dampMask);
    albedo *= lerp(0.9, 1.06, FadeNoise(input.World.xz * 0.007));
    if (weight.y > 0.86)
    {
        float lift = saturate(SunDir.y * 1.6 + 0.05);
        float3 sun = SunDir;
        float sunLen = length(sun);
        sun = sunLen > 0.001 ? sun / sunLen : float3(0.2, 1.0, 0.15);
        float ndotl = saturate(dot(normal, sun));
        float shade = 1.0;
        if (CascadeCast > 0.5)
        {
            shade = lerp(0.42, 1.0, CascadeLit(input.World));
        }

        float3 color = albedo * (lerp(0.22, 0.38, lift) + ndotl * lerp(0.25, 0.62, lift) * shade);
        color *= Flash;
        color = lerp(color, color * float3(0.40, 0.44, 0.46), wet);
        color += LampGlow(input.World, normal, albedo, 0.82, lift, input.Position.xy);
        float away = length(input.World - Eye);
        float haze = saturate((away - 520.0) / 1400.0);
        color = lerp(color, FogColor, haze * haze);
        return float4(color, 1);
    }

    float3 bumped = grassN * blend.x + dirtN * blend.y + rockN * blend.z;
    bumped = lerp(bumped, normal, blend.w);
    bumped = lerp(bumped, dampN, dampMask);
    bumped = normalize(bumped);

    float rough = grassR * blend.x + dirtR * blend.y + rockR * blend.z;
    rough = lerp(rough, 0.72, blend.w);
    rough = lerp(rough, dampR, dampMask);
    rough = lerp(rough, rough * 0.4, wet);

    float lift = saturate(SunDir.y * 1.6 + 0.05);
    float3 lightDir = normalize(lerp(float3(0.2, 1.0, 0.15), SunDir, lift));
    float3 sun = SunDir;
    float sunLen = length(sun);
    sun = sunLen > 0.001 ? sun / sunLen : lightDir;
    float sunShade = saturate(input.Shade);
    float3 viewDir = normalize(Eye - input.World);
    float NdotV = saturate(dot(bumped, viewDir));
    float roughLit = saturate(rough);
    float3 sunHalf = normalize(sun + viewDir);
    float sunBody = Burley(saturate(dot(bumped, sun)), NdotV, saturate(dot(sun, sunHalf)), roughLit);
    float fill = saturate(dot(bumped, lightDir));
    float clouds = CloudShade(input.World.xz);
    float light = lerp(fill, sunBody, lift) * sunShade * clouds;
    float3 halfDir = normalize(lightDir + viewDir);
    float shine = pow(saturate(dot(bumped, halfDir)), lerp(56.0, 5.0, saturate(rough)));
    float open = saturate(input.Open);
    float3 color = albedo * lerp(0.22, 0.38, lift) * lerp(0.9, 1.0, clouds) * open + albedo * light * lerp(0.25, 0.62, lift);
    color *= Flash;
    color += shine * (1.0 - saturate(rough)) * 0.32 * lift * sunShade * clouds;
    color = lerp(color, color * float3(0.40, 0.44, 0.46), wet);
    if (CascadeCast > 0.5)
    {
        color *= lerp(0.42, 1.0, CascadeLit(input.World));
    }

    color += LampGlow(input.World, bumped, albedo, rough, lift, input.Position.xy);
    float away = length(input.World - Eye);
    float haze = saturate((away - 520.0) / 1400.0);
    color = lerp(color, FogColor, haze * haze);
    return float4(color, 1);
}

float4 DepthPS(VSOutput input) : SV_Target0
{
    return input.Position.z;
}

float4 PlainPS(VSOutput input) : COLOR0
{
    float3 normal = normalize(input.Normal);
    float4 blend = input.Blend;
    float sum = blend.x + blend.y + blend.z + blend.w;
    if (sum < 0.001)
    {
        blend = float4(1, 0, 0, 0);
        sum = 1;
    }

    blend /= sum;
    float scale = TileMeters > 0.01 ? 1.0 / TileMeters : 0.06;
    float2 uv = input.World.xz * scale;
    float3 albedo = 0;
    if (blend.x > 0.02)
    {
        albedo += SAMPLE_ATLAS(ColorSampler, uv, GrassTile).rgb * blend.x;
    }

    if (blend.y > 0.02)
    {
        albedo += SAMPLE_ATLAS(ColorSampler, uv, DirtTile).rgb * blend.y;
    }

    if (blend.z > 0.02)
    {
        albedo += SAMPLE_ATLAS(ColorSampler, uv, RockTile).rgb * blend.z;
    }

    albedo = lerp(albedo, float3(0.90, 0.92, 0.94), blend.w);
    float lift = saturate(SunDir.y * 1.6 + 0.05);
    float3 sun = SunDir;
    float sunLen = length(sun);
    sun = sunLen > 0.001 ? sun / sunLen : float3(0.2, 1.0, 0.15);
    float ndotl = saturate(dot(normal, sun)) * saturate(input.Shade);
    float3 color = albedo * (lerp(0.22, 0.38, lift) + ndotl * lerp(0.25, 0.62, lift));
    float plainWet = saturate(input.Wet > 0.985 ? 1.0 : input.Wet);
    plainWet *= (1.0 - blend.z) * (1.0 - blend.w * 0.65);
    color = lerp(color, color * float3(0.40, 0.44, 0.46), plainWet);
    color *= Flash;
    float away = length(input.World - Eye);
    float haze = saturate((away - 520.0) / 1400.0);
    color = lerp(color, FogColor, haze * haze);
    return float4(color, 1);
}

technique Plain
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PlainPS();
    }
}

technique Height
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PS();
    }
}

technique Depth
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 DepthPS();
    }
}
