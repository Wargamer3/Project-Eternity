float4x4 ViewProj;
float WindTime;
float2 WindDir = float2(1, 0.2);
float WindStrength = 8;
float3 SunDir = float3(0.35, 0.86, 0.25);
float3 SunColor = float3(1.0, 0.93, 0.80);
float3 Ambient = float3(0.32, 0.34, 0.38);
float3 Eye;
float FadeStart = 860;
float FadeEnd = 1250;
int StepCount;
float4 Steps[8];
float4 StepMove[8];

texture Clump;
sampler ClumpSampler = sampler_state
{
    Texture = (Clump);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VSIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Uv : TEXCOORD0;
    float2 Bend : TEXCOORD1;
};

struct VSOut
{
    float4 Clip : SV_Position;
    float3 Normal : TEXCOORD0;
    float2 Uv : TEXCOORD1;
    float Bend : TEXCOORD2;
    float Fade : TEXCOORD3;
};

VSOut VS(VSIn input)
{
    VSOut output;
    float3 Place = input.Position;
    float Above = saturate(input.Bend.x);
    Above = Above * Above;
    float Phase = dot(Place.xz, float2(0.012, 0.009)) + input.Bend.y;
    float Gust = sin(WindTime * 1.25 + Phase) * 0.65 + sin(WindTime * 2.6 + Phase * 1.8) * 0.35;
    float3 Push = float3(WindDir.x, 0, WindDir.y);
    float Flutter = sin(WindTime * 4.8 + Phase * 7.0);
    Place += Push * Gust * Above * WindStrength;
    Place.xz += float2(-Push.z, Push.x) * Flutter * Above * WindStrength * 0.32;
    float Press = 0;
    if (StepCount > 0)
    {
    [loop]
    for (int i = 0; i < 8; i++)
    {
        if (i >= StepCount)
        {
            break;
        }
        float3 Foot = Steps[i].xyz;
        float Weight = Steps[i].w;
        float2 Delta = input.Position.xz - Foot.xz;
        float Dist = length(Delta);
        float Reach = max(StepMove[i].z, 1);
        float Near = saturate(1 - Dist / Reach);
        if (abs(input.Position.y - Foot.y) > 42)
        {
            Near = 0;
        }

        Near = Near * Near * Weight;
        float2 Outward = Dist > 0.35 ? Delta / Dist : float2(0, 1);
        float2 Travel = StepMove[i].xy;
        float Speed = length(Travel);
        float2 Trail = Speed > 2 ? Travel / Speed : Outward;
        float2 Dir = Outward * 0.7 + Trail * saturate(Speed / 14.0) * 1.2;
        Place.xz += Dir * Near * Above * 14;
        Press = max(Press, Near);
    }
    }

    Place.y -= Press * Above * 2.4;
    output.Clip = mul(float4(Place, 1), ViewProj);
    output.Normal = input.Normal;
    output.Uv = input.Uv;
    output.Bend = input.Bend.x;
    float Far = distance(Place.xz, Eye.xz);
    output.Fade = saturate((FadeEnd - Far) / max(FadeEnd - FadeStart, 1));
    if (output.Fade < 0.5)
    {
        output.Clip = 0;
    }

    return output;
}

float4 PS(VSOut input) : SV_Target0
{
    float4 Tex = tex2D(ClumpSampler, input.Uv);
    clip(Tex.a * input.Fade - 0.5);
    float3 N = normalize(input.Normal);
    float Wrap = abs(dot(N, normalize(SunDir))) * 0.55 + 0.45;
    float Tip = lerp(0.72, 1.08, saturate(input.Bend));
    float3 Light = Ambient + SunColor * Wrap;
    return float4(Tex.rgb * Light * Tip, 1);
}

technique Grass
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PS();
    }
}
