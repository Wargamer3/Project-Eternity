float4x4 WorldViewProj;
float3 Origin;
float3 Eye;
float3 Sizes = float3(1024, 256, 48);
float3 SunDir = float3(0.35, 0.86, 0.25);
float3 SunColor = float3(1, 0.95, 0.8);
float3 FogColor = float3(0.76, 0.81, 0.84);
float DepthBias;

texture Disp0;
sampler DispSampler0 = sampler_state
{
    Texture = (Disp0);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Disp1;
sampler DispSampler1 = sampler_state
{
    Texture = (Disp1);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture Disp2;
sampler DispSampler2 = sampler_state
{
    Texture = (Disp2);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = None;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VSIn
{
    float3 Position : POSITION0;
};

struct VSOut
{
    float4 Clip : SV_Position;
    float3 World : TEXCOORD0;
    float2 Flat : TEXCOORD1;
};

float3 Tap(sampler Map, float2 Xz, float Size)
{
    float E = Size * (1.0 / 64.0);
    float2 Uv = Xz / Size;
    float4 Center = tex2D(Map, Uv);
    float Hx = tex2D(Map, Uv + float2(E, 0) / Size).y;
    float Hz = tex2D(Map, Uv + float2(0, E) / Size).y;
    return float3((Hx - Center.y) / E, (Hz - Center.y) / E, Center.w);
}

VSOut VS(VSIn input)
{
    VSOut output;
    float3 World = input.Position + Origin;
    float2 Point = World.xz;
    float3 Moved = tex2Dlod(DispSampler0, float4(Point / Sizes.x, 0, 0)).xyz;
    Moved += tex2Dlod(DispSampler1, float4(Point / Sizes.y, 0, 0)).xyz;
    Moved += tex2Dlod(DispSampler2, float4(Point / Sizes.z, 0, 0)).xyz;
    World.xz += Moved.xz;
    World.y += Moved.y;
    output.World = World;
    output.Flat = Point;
    output.Clip = mul(float4(World, 1), WorldViewProj);
    output.Clip.z += DepthBias * output.Clip.w;
    return output;
}

float4 PS(VSOut input) : SV_Target0
{
    float Away = length(input.World - Eye);
    float Detail = saturate(1.0 - (Away - 80.0) / 270.0);
    float3 A = Tap(DispSampler0, input.Flat, Sizes.x);
    float3 B = Tap(DispSampler1, input.Flat, Sizes.y);
    float3 C = Tap(DispSampler2, input.Flat, Sizes.z);
    float3 Slope = A * 1.4 + B * 2.2 + C * 1.6 * Detail;
    float3 Normal = normalize(float3(-Slope.x, 1, -Slope.y));
    float3 View = normalize(Eye - input.World);
    float3 Sun = normalize(SunDir);
    float Day = saturate(Sun.y * 1.5 + 0.06);
    float Face = saturate(dot(Normal, Sun) * 0.55 + 0.45);
    float3 Deep = float3(0.02, 0.09, 0.20);
    float3 Shallow = float3(0.04, 0.20, 0.28);
    float3 Body = lerp(Deep, Shallow, Face * 0.35) * Day;
    float Fresnel = pow(1.0 - saturate(dot(Normal, View)), 5.0) * 0.72;
    float3 Color = lerp(Body, FogColor, Fresnel);
    float3 Half = normalize(Sun + View);
    float Align = saturate(dot(Normal, Half));
    float Glint = pow(Align, 80.0);
    Color += SunColor * Glint * 0.55 * saturate(Sun.y * 3.0);
    float Foam = saturate(C.z) * Detail;
    Color = lerp(Color, float3(0.86, 0.91, 0.93), Foam);
    float Haze = saturate((Away - 700.0) / 1800.0);
    Color = lerp(Color, FogColor, Haze * Haze);
    return float4(Color, 1);
}

technique Ocean
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PS();
    }
}
