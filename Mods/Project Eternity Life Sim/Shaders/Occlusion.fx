float4x4 WorldViewProj;
float3 ChunkOrigin;
float ProjM33;
float ProjM43;
float PixelsPerUnit;
float Radius;
float Bias;
float2 BlurStep;
float2 TargetSize;

texture DepthMap;
sampler DepthSampler = sampler_state
{
    Texture = (DepthMap);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture WideMap;
sampler WideSampler = sampler_state
{
    Texture = (WideMap);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct MeshInput
{
    float3 Position : POSITION0;
};

struct HeightInput
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Blend : TEXCOORD0;
    float Wet : TEXCOORD1;
    float Shade : TEXCOORD2;
};

struct DepthOutput
{
    float4 Clip : SV_Position;
    float Depth : TEXCOORD0;
};

struct QuadInput
{
    float4 Position : POSITION0;
    float2 Uv : TEXCOORD0;
};

struct QuadOutput
{
    float4 Clip : SV_Position;
    float2 Uv : TEXCOORD0;
};

DepthOutput MeshVS(MeshInput input)
{
    DepthOutput output;
    float4 Clip = mul(float4(input.Position, 1), WorldViewProj);
    output.Clip = Clip;
    output.Depth = Clip.z / Clip.w;
    return output;
}

DepthOutput HeightVS(HeightInput input)
{
    DepthOutput output;
    float4 Clip = mul(float4(input.Position + ChunkOrigin, 1), WorldViewProj);
    output.Clip = Clip;
    output.Depth = Clip.z / Clip.w;
    return output;
}

float4 DepthPS(DepthOutput input) : SV_Target0
{
    return input.Depth;
}

QuadOutput QuadVS(QuadInput input)
{
    QuadOutput output;
    output.Clip = float4(input.Position.xy, 0, 1);
    output.Uv = input.Position.xy * float2(0.5, -0.5) + 0.5;
    return output;
}

float Linear(float Depth01)
{
    float Denom = Depth01 + ProjM33;
    if (abs(Denom) < 0.0000001)
    {
        return 10000.0;
    }

    return abs(ProjM43 / Denom);
}

float2 Disk(int Index, float Turn)
{
    float Ring = sqrt((Index + 0.5) / 12.0);
    float Angle = Index * 2.39996323 + Turn;
    return float2(cos(Angle), sin(Angle)) * Ring;
}

float TurnOf(float2 Uv)
{
    float2 Pixel = Uv * TargetSize;
    return frac(52.9829189 * frac(dot(Pixel, float2(0.06711056, 0.00583715)))) * 6.2831853;
}

float4 GatherPS(QuadOutput input) : SV_Target0
{
    float Center = Linear(tex2D(DepthSampler, input.Uv).r);
    float Turn = TurnOf(input.Uv);
    float Span = Radius * (PixelsPerUnit / max(Center, 0.5));
    Span = clamp(Span, 2.0, 36.0);
    float Blocked = 0.0;
    float Used = 0.0;
    [unroll]
    for (int i = 0; i < 12; i++)
    {
        float2 Uv = input.Uv + Disk(i, Turn) * Span / TargetSize;
        if (Uv.x < 0.0 || Uv.y < 0.0 || Uv.x > 1.0 || Uv.y > 1.0)
        {
            continue;
        }

        float Sample = Linear(tex2D(DepthSampler, Uv).r);
        float Closer = Center - Sample;
        Used += 1.0;
        if (Closer > Bias && Closer < Radius)
        {
            Blocked += 1.0;
        }
    }

    float Open = Used > 0.0 ? 1.0 - Blocked / Used : 1.0;
    // 1 is open sky. 0.35 is a crowded crease. Callers multiply this onto ambient only.
    return lerp(0.35, 1.0, saturate(Open));
}

float4 BlurPS(QuadOutput input) : SV_Target0
{
    float CenterZ = Linear(tex2D(DepthSampler, input.Uv).r);
    float Sum = tex2D(WideSampler, input.Uv).r;
    float Weight = 1.0;
    float2 Step = BlurStep;
    [unroll]
    for (int i = 1; i <= 3; i++)
    {
        float2 Uv = input.Uv + Step * i / TargetSize;
        float SampleZ = Linear(tex2D(DepthSampler, Uv).r);
        float Keep = saturate(1.0 - abs(SampleZ - CenterZ) / 1.6);
        Sum += tex2D(WideSampler, Uv).r * Keep;
        Weight += Keep;
    }

    return Sum / Weight;
}

technique Mesh
{
    pass
    {
        VertexShader = compile vs_4_0 MeshVS();
        PixelShader = compile ps_4_0 DepthPS();
    }
}

technique Height
{
    pass
    {
        VertexShader = compile vs_4_0 HeightVS();
        PixelShader = compile ps_4_0 DepthPS();
    }
}

technique Gather
{
    pass
    {
        VertexShader = compile vs_4_0 QuadVS();
        PixelShader = compile ps_4_0 GatherPS();
    }
}

technique Blur
{
    pass
    {
        VertexShader = compile vs_4_0 QuadVS();
        PixelShader = compile ps_4_0 BlurPS();
    }
}
