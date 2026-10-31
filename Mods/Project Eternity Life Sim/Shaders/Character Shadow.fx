float4x4 WorldViewProj;
float4x4 World;
float4x4 PartWorld;
float4x4 ViewProj;
float3 Pivot;
float WindTime;
float2 WindDir = float2(1, 0.2);
float WindStrength = 22;
float AlphaCut = 0.4;
float4x4 Bones[256];

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

struct VSInput
{
    float3 Position : POSITION0;
};

struct VSOutput
{
    float4 Clip : SV_Position;
    float Depth : TEXCOORD0;
};

struct LeafIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Uv : TEXCOORD0;
    float4 Color : COLOR0;
};

struct LeafOut
{
    float4 Clip : SV_Position;
    float2 Uv : TEXCOORD0;
    float Depth : TEXCOORD1;
};

VSOutput VS(VSInput input)
{
    VSOutput output;
    float4 Clip = mul(float4(input.Position, 1), WorldViewProj);
    output.Clip = Clip;
    output.Depth = Clip.z / Clip.w;
    return output;
}

float4 PS(VSOutput input) : SV_Target0
{
    return input.Depth;
}

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

LeafOut LeafVS(LeafIn input)
{
    LeafOut output;
    float3 WorldPos = Winded(input.Position, World, Pivot);
    float4 Clip = mul(float4(WorldPos, 1), ViewProj);
    output.Clip = Clip;
    output.Uv = input.Uv;
    output.Depth = Clip.z / Clip.w;
    return output;
}

float4 LeafPS(LeafOut input) : SV_Target0
{
    clip(tex2D(ColorSampler, input.Uv).a - AlphaCut);
    return input.Depth;
}

technique Depth
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PS();
    }
}

technique Foliage
{
    pass
    {
        VertexShader = compile vs_4_0 LeafVS();
        PixelShader = compile ps_4_0 LeafPS();
    }
}

struct RigidIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Uv : TEXCOORD0;
    float4 Color : COLOR0;
};

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

VSOutput RigidVS(RigidIn input)
{
    VSOutput output;
    float3 WorldPos = mul(float4(input.Position, 1), World).xyz;
    float4 Clip = mul(float4(WorldPos, 1), ViewProj);
    output.Clip = Clip;
    output.Depth = Clip.z / Clip.w;
    return output;
}

VSOutput RigidInstVS(InstIn input)
{
    VSOutput output;
    float3 WorldPos = mul(float4(input.Position, 1), Planted(input)).xyz;
    float4 Clip = mul(float4(WorldPos, 1), ViewProj);
    output.Clip = Clip;
    output.Depth = Clip.z / Clip.w;
    return output;
}

LeafOut LeafInstVS(InstIn input)
{
    LeafOut output;
    float4x4 Bend = Planted(input);
    float3 WorldPos = Winded(input.Position, Bend, float3(Bend._41, Bend._42, Bend._43));
    float4 Clip = mul(float4(WorldPos, 1), ViewProj);
    output.Clip = Clip;
    output.Uv = input.Uv;
    output.Depth = Clip.z / Clip.w;
    return output;
}

technique Rigid
{
    pass
    {
        VertexShader = compile vs_4_0 RigidVS();
        PixelShader = compile ps_4_0 PS();
    }
}

technique RigidInst
{
    pass
    {
        VertexShader = compile vs_4_0 RigidInstVS();
        PixelShader = compile ps_4_0 PS();
    }
}

technique FoliageInst
{
    pass
    {
        VertexShader = compile vs_4_0 LeafInstVS();
        PixelShader = compile ps_4_0 LeafPS();
    }
}

struct SkinIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Uv : TEXCOORD0;
    int4 Indices : BLENDINDICES0;
    float4 Weights : BLENDWEIGHT0;
};

VSOutput SkinVS(SkinIn input)
{
    VSOutput output;
    float4x4 Skinning = 0;
    Skinning += Bones[input.Indices.x] * input.Weights.x;
    Skinning += Bones[input.Indices.y] * input.Weights.y;
    Skinning += Bones[input.Indices.z] * input.Weights.z;
    Skinning += Bones[input.Indices.w] * input.Weights.w;
    float4 Position = mul(float4(input.Position, 1), Skinning);
    float4 Clip = mul(Position, WorldViewProj);
    output.Clip = Clip;
    output.Depth = Clip.z / Clip.w;
    return output;
}

technique Skin
{
    pass
    {
        VertexShader = compile vs_4_0 SkinVS();
        PixelShader = compile ps_4_0 PS();
    }
}
