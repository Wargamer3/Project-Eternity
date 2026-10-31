float Near = 1;
float Far = 10000;
float Radius = 28;
float2 UvPerUnit = float2(1, 1);
float2 Pixel = float2(0.001, 0.001);
float2 Direction = float2(1, 0);
float2 SunUv = float2(0.5, 0.2);
float3 SunColor = float3(1, 0.9, 0.7);
float Shaft = 0.2;
float Aspect = 1.6;

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

texture SceneMap;
sampler SceneSampler = sampler_state
{
    Texture = (SceneMap);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture AoMap;
sampler AoSampler = sampler_state
{
    Texture = (AoMap);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture ShaftMap;
sampler ShaftSampler = sampler_state
{
    Texture = (ShaftMap);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct QuadIn
{
    float4 Position : POSITION0;
    float2 Uv : TEXCOORD0;
};

struct QuadOut
{
    float4 Clip : SV_Position;
    float2 Uv : TEXCOORD0;
};

QuadOut QuadVS(QuadIn input)
{
    QuadOut output;
    output.Clip = float4(input.Position.xy, 0, 1);
    output.Uv = input.Uv;
    return output;
}

float EyeFrom(float Stored)
{
    return (Near * Far) / max(Far - Stored * (Far - Near), 0.001);
}

float EyeOf(float2 Uv)
{
    return EyeFrom(tex2D(DepthSampler, Uv).r);
}

float4 OccludePS(QuadOut input) : SV_Target0
{
    float Stored = tex2D(DepthSampler, input.Uv).r;
    if (Stored < 0.01)
    {
        return 1;
    }

    float Eye = EyeOf(input.Uv);
    float EyeX = ddx(Eye);
    float EyeY = ddy(Eye);
    float2 Span = UvPerUnit * (Radius / max(Eye, 1.0));
    float Turn = frac(sin(dot(input.Uv, float2(12.9898, 78.233))) * 43758.5453) * 6.28318;
    float2 Rotor = float2(cos(Turn), sin(Turn));
    float2 Kernel[12] = {
        float2(0.15, 0.05), float2(-0.22, 0.18), float2(0.28, -0.16), float2(-0.08, -0.32),
        float2(0.42, 0.22), float2(-0.48, 0.08), float2(0.12, 0.52), float2(-0.36, -0.44),
        float2(0.62, -0.28), float2(-0.58, 0.46), float2(0.33, 0.68), float2(-0.18, 0.72)
    };

    float Occlusion = 0.0;
    for (int i = 0; i < 12; i++)
    {
        float2 Offset = Kernel[i];
        float2 Turned = float2(Offset.x * Rotor.x - Offset.y * Rotor.y, Offset.x * Rotor.y + Offset.y * Rotor.x);
        float2 SampleUv = saturate(input.Uv + Turned * Span);
        float SampleStored = tex2Dlod(DepthSampler, float4(SampleUv, 0, 0)).r;
        float SampleEye = EyeFrom(max(SampleStored, 0.01));
        float2 Pixels = (SampleUv - input.Uv) / max(Pixel, float2(0.0001, 0.0001));
        float Expected = Eye + Pixels.x * EyeX + Pixels.y * EyeY;
        float Diff = Expected - SampleEye;
        float Hit = saturate((Diff - 2.0) * 0.12);
        Hit *= 1.0 - smoothstep(Radius * 0.2, Radius, abs(SampleEye - Eye));
        Hit *= SampleStored >= 0.01;
        Occlusion += Hit;
    }

    return 1.0 - Occlusion / 12.0;
}

void BlurTap(float2 Uv, float Center, float Gate, inout float Sum, inout float Weight)
{
    float Stored = tex2Dlod(DepthSampler, float4(Uv, 0, 0)).r;
    float Keep = Stored < 0.01 ? 0.0 : saturate(1.0 - abs(EyeFrom(Stored) - Center) / Gate);
    Sum += tex2Dlod(AoSampler, float4(Uv, 0, 0)).r * Keep;
    Weight += Keep;
}

float4 BlurPS(QuadOut input) : SV_Target0
{
    float CenterStored = tex2D(DepthSampler, input.Uv).r;
    if (CenterStored < 0.01)
    {
        return 1;
    }

    float Center = EyeFrom(CenterStored);
    float Sum = tex2D(AoSampler, input.Uv).r;
    float Weight = 1.0;
    float Gate = max(4.0, Center * 0.1);
    for (int i = 1; i <= 4; i++)
    {
        float2 Step = Direction * Pixel * i;
        BlurTap(input.Uv + Step, Center, Gate, Sum, Weight);
        BlurTap(input.Uv - Step, Center, Gate, Sum, Weight);
    }

    return Sum / Weight;
}

float4 PresentPS(QuadOut input) : SV_Target0
{
    float3 Color = tex2D(SceneSampler, input.Uv).rgb;
    float Open = saturate(tex2D(AoSampler, input.Uv).r);
    Color *= lerp(0.62, 1.0, Open);
    Color += SunColor * tex2D(ShaftSampler, input.Uv).r * Shaft;
    return float4(Color, 1);
}

float4 ShaftPS(QuadOut input) : SV_Target0
{
    float2 Step = (SunUv - input.Uv) / 16.0;
    float2 Perp = float2(-Step.y, Step.x) * float2(1.0, Aspect);
    float Acc = 0.0;
    float Weight = 0.0;
    for (int i = 0; i < 16; i++)
    {
        float Along = i + 1.0;
        float Spread = Along / 16.0;
        float2 Walk = input.Uv + Step * Along + Perp * (frac(Along * 0.37) - 0.5) * Spread * 6.0;
        float Sky = tex2D(DepthSampler, Walk).r < 0.01 ? 1.0 : 0.0;
        float Fade = lerp(1.0, 0.35, Spread);
        Acc += Sky * Fade;
        Weight += Fade;
    }

    float Glow = Acc / max(Weight, 0.001);
    float2 FromSun = (input.Uv - SunUv) * float2(1.0, Aspect);
    float Falloff = saturate(1.0 - length(FromSun) * 0.35);
    return Glow * Falloff;
}

technique Occlude
{
    pass
    {
        VertexShader = compile vs_4_0 QuadVS();
        PixelShader = compile ps_4_0 OccludePS();
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

technique Shafts
{
    pass
    {
        VertexShader = compile vs_4_0 QuadVS();
        PixelShader = compile ps_4_0 ShaftPS();
    }
}

technique Present
{
    pass
    {
        VertexShader = compile vs_4_0 QuadVS();
        PixelShader = compile ps_4_0 PresentPS();
    }
}
