float4x4 World;
float4x4 PartWorld;
float4x4 ViewProj;
float3 Pivot;
float WindTime;
float2 WindDir = float2(1, 0.2);
float WindStrength = 22;
float3 SunDir = float3(0.35, 0.86, 0.25);
float3 SunColor = float3(0.62, 0.58, 0.5);
float3 Ambient = float3(0.32, 0.34, 0.38);
float3 FillDir = float3(-0.2, 0.92, -0.15);
float3 FillColor = float3(0.14, 0.15, 0.17);
float AlphaCut = 0.4;
float TreeCast = 0;
float ShadowCast = 0;
float4x4 TreeViewProj;
float4x4 ShadowViewProj;
float CascadeCast = 0;
float4 CascadeSplit = float4(160, 640, 2560, 24);
float4 CascadeTexel = float4(1.0 / 1024.0, 1.0 / 512.0, 1.0 / 256.0, 0.0);
float4x4 CameraView;
float4x4 CascadeView0;
float4x4 CascadeView1;
float4x4 CascadeView2;

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

texture TreeMap;
sampler TreeSampler = sampler_state
{
    Texture = (TreeMap);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture ShadowMap;
sampler ShadowSampler = sampler_state
{
    Texture = (ShadowMap);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture ColorMap;
sampler ColorSampler = sampler_state
{
    Texture = (ColorMap);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct ColorIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Uv : TEXCOORD0;
    float4 Color : COLOR0;
};

struct ColorOut
{
    float4 Clip : SV_Position;
    float3 Normal : TEXCOORD0;
    float2 Uv : TEXCOORD1;
    float3 World : TEXCOORD2;
};

struct DepthIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Uv : TEXCOORD0;
    float4 Color : COLOR0;
};

struct DepthOut
{
    float4 Clip : SV_Position;
    float2 Uv : TEXCOORD0;
    float Depth : TEXCOORD1;
};

float3 Winded(float3 Position, float4x4 Bend, float3 Root)
{
    float3 WorldPos = mul(float4(Position, 1), Bend).xyz;
    float Above = saturate((WorldPos.y - Root.y) / 200.0);
    Above = Above * Above;
    float Phase = dot(WorldPos.xz, float2(0.012, 0.009));
    float Gust = sin(WindTime * 1.25 + Phase) * 0.65 + sin(WindTime * 2.6 + Phase * 1.8) * 0.35;
    float3 Push = float3(WindDir.x, 0, WindDir.y);
    float Flutter = sin(WindTime * 4.8 + Phase * 7.0);
    WorldPos += Push * Gust * Above * WindStrength;
    WorldPos.xz += float2(-Push.z, Push.x) * Flutter * Above * WindStrength * 0.28;
    return WorldPos;
}

ColorOut LeafVS(ColorIn input)
{
    ColorOut output;
    float3 WorldPos = Winded(input.Position, World, Pivot);
    output.Clip = mul(float4(WorldPos, 1), ViewProj);
    output.Normal = mul(input.Normal, (float3x3)World);
    output.Uv = input.Uv;
    output.World = WorldPos;
    return output;
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

float Blocked(sampler Map, float4x4 ViewProjShadow, float3 World, float TexelSize)
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

    return Grid(Map, Uv, Here, 0.00025, TexelSize);
}

float CascadeCover(float3 World)
{
    if (CascadeCast < 0.5)
    {
        return 1.0;
    }

    float Depth = -mul(float4(World, 1), CameraView).z;
    float Blend = max(CascadeSplit.w, 0.5);
    if (Depth < CascadeSplit.x)
    {
        float Near = Blocked(CascadeSampler0, CascadeView0, World, CascadeTexel.x);
        if (Depth < CascadeSplit.x - Blend)
        {
            return Near;
        }

        return lerp(Blocked(CascadeSampler1, CascadeView1, World, CascadeTexel.y), Near, saturate((CascadeSplit.x - Depth) / Blend));
    }

    if (Depth < CascadeSplit.y)
    {
        float Mid = Blocked(CascadeSampler1, CascadeView1, World, CascadeTexel.y);
        if (Depth < CascadeSplit.y - Blend)
        {
            return Mid;
        }

        return lerp(Blocked(CascadeSampler2, CascadeView2, World, CascadeTexel.z), Mid, saturate((CascadeSplit.y - Depth) / Blend));
    }

    return Blocked(CascadeSampler2, CascadeView2, World, CascadeTexel.z);
}

float3 Sunward(float3 WorldPos, float3 Normal)
{
    float3 Sun = SunDir;
    float Along = dot(Sun, Sun);
    if (Along < 0.0001)
    {
        return WorldPos;
    }

    Sun *= rsqrt(Along);
    float Grazing = 1.0 - saturate(abs(dot(normalize(Normal), Sun)));
    return WorldPos + Sun * (1.5 + Grazing * 3.0);
}

float4 LeafPS(ColorOut input) : SV_Target0
{
    float4 Texel = tex2D(ColorSampler, input.Uv);
    clip(Texel.a - AlphaCut);
    float3 Normal = normalize(input.Normal);
    float Cover = CascadeCover(Sunward(input.World, Normal));
    float Sun = saturate(dot(Normal, SunDir)) * Cover;
    float Through = saturate(dot(-Normal, SunDir));
    Through = Through * Through * Cover;
    return float4(Texel.rgb * (Ambient + SunColor * (Sun + Through * 0.65)), Texel.a);
}

struct BarkOut
{
    float4 Clip : SV_Position;
    float3 Normal : TEXCOORD0;
    float2 Uv : TEXCOORD1;
    float3 World : TEXCOORD2;
    float4 Color : TEXCOORD3;
};

BarkOut BarkVS(ColorIn input)
{
    BarkOut output;
    float3 WorldPos = mul(float4(input.Position, 1), World).xyz;
    output.Clip = mul(float4(WorldPos, 1), ViewProj);
    output.Normal = mul(input.Normal, (float3x3)World);
    output.Uv = input.Uv;
    output.World = WorldPos;
    output.Color = input.Color;
    return output;
}

float4 BarkPS(BarkOut input) : SV_Target0
{
    float4 Texel = tex2D(ColorSampler, input.Uv) * input.Color;
    float3 Normal = normalize(input.Normal);
    float Cover = CascadeCover(Sunward(input.World, Normal));
    float3 Sun = SunDir;
    float Along = dot(Sun, Sun);
    Sun = Along > 0.0001 ? Sun * rsqrt(Along) : float3(0, 1, 0);
    float3 Fill = FillDir;
    float FillAlong = dot(Fill, Fill);
    Fill = FillAlong > 0.0001 ? Fill * rsqrt(FillAlong) : float3(0, 1, 0);
    float Key = saturate(dot(Normal, Sun)) * Cover;
    float Sky = saturate(dot(Normal, Fill));
    float3 Lit = Ambient + SunColor * Key + FillColor * Sky;
    return float4(Texel.rgb * Lit, Texel.a);
}

DepthOut DepthVS(DepthIn input)
{
    DepthOut output;
    float3 WorldPos = Winded(input.Position, World, Pivot);
    float4 Clip = mul(float4(WorldPos, 1), ViewProj);
    output.Clip = Clip;
    output.Uv = input.Uv;
    output.Depth = Clip.z / Clip.w;
    return output;
}

float4 DepthPS(DepthOut input) : SV_Target0
{
    clip(tex2D(ColorSampler, input.Uv).a - AlphaCut);
    return input.Depth;
}

technique Leaf
{
    pass
    {
        VertexShader = compile vs_4_0 LeafVS();
        PixelShader = compile ps_4_0 LeafPS();
    }
}

technique Bark
{
    pass
    {
        VertexShader = compile vs_4_0 BarkVS();
        PixelShader = compile ps_4_0 BarkPS();
    }
}

technique LeafDepth
{
    pass
    {
        VertexShader = compile vs_4_0 DepthVS();
        PixelShader = compile ps_4_0 DepthPS();
    }
}

struct InstIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Uv : TEXCOORD0;
    float4 Color : COLOR0;
    float4 Row0 : TEXCOORD1;
    float4 Row1 : TEXCOORD2;
    float4 Row2 : TEXCOORD3;
    float4 Row3 : TEXCOORD4;
};

float4x4 Planted(InstIn input)
{
    return mul(PartWorld, float4x4(input.Row0, input.Row1, input.Row2, input.Row3));
}

ColorOut LeafInstVS(InstIn input)
{
    ColorOut output;
    float4x4 Bend = Planted(input);
    float3 WorldPos = Winded(input.Position, Bend, float3(Bend._41, Bend._42, Bend._43));
    output.Clip = mul(float4(WorldPos, 1), ViewProj);
    output.Normal = mul(input.Normal, (float3x3)Bend);
    output.Uv = input.Uv;
    output.World = WorldPos;
    return output;
}

BarkOut BarkInstVS(InstIn input)
{
    BarkOut output;
    float4x4 Bend = Planted(input);
    float3 WorldPos = mul(float4(input.Position, 1), Bend).xyz;
    output.Clip = mul(float4(WorldPos, 1), ViewProj);
    output.Normal = mul(input.Normal, (float3x3)Bend);
    output.Uv = input.Uv;
    output.World = WorldPos;
    output.Color = input.Color;
    return output;
}

technique LeafInst
{
    pass
    {
        VertexShader = compile vs_4_0 LeafInstVS();
        PixelShader = compile ps_4_0 LeafPS();
    }
}

technique BarkInst
{
    pass
    {
        VertexShader = compile vs_4_0 BarkInstVS();
        PixelShader = compile ps_4_0 BarkPS();
    }
}

struct CardIn
{
    float3 Position : POSITION0;
    float2 Uv : TEXCOORD0;
};

struct CardOut
{
    float4 Clip : SV_Position;
    float2 Uv : TEXCOORD0;
};

CardOut CardVS(CardIn input)
{
    CardOut output;
    output.Clip = mul(float4(input.Position, 1), ViewProj);
    output.Uv = input.Uv;
    return output;
}

float4 CardPS(CardOut input) : SV_Target0
{
    float4 Texel = tex2D(ColorSampler, input.Uv);
    clip(Texel.a - 0.35);
    return float4(Texel.rgb * (Ambient + SunColor * 0.7), 1);
}

technique Card
{
    pass
    {
        VertexShader = compile vs_4_0 CardVS();
        PixelShader = compile ps_4_0 CardPS();
    }
}
