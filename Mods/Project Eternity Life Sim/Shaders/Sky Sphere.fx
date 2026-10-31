matrix WorldViewProj;
float3 Zenith;
float3 Horizon;
float3 SunDir;
float3 SunColor;
float Cloud;
float Time;

struct VSInput
{
    float3 Position : POSITION0;
};

struct VSOutput
{
    float4 Clip : SV_Position;
    float3 Dir : TEXCOORD0;
};

VSOutput VS(VSInput input)
{
    VSOutput output;
    output.Clip = mul(float4(input.Position, 1), WorldViewProj);
    output.Dir = input.Position;
    return output;
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

float4 PS(VSOutput input) : SV_Target0
{
    float3 Dir = normalize(input.Dir);
    float Up = max(Dir.y, 0.0);
    float Band = pow(saturate(1.0 - Up), 3.2);
    float3 Color = lerp(Zenith, Horizon, Band);

    float3 Sun = normalize(SunDir);
    float Facing = saturate(dot(Dir, Sun));
    float Disc = smoothstep(0.9986, 0.9994, Facing);
    float Glow = pow(Facing, 6.0) * 0.45;
    float SunUp = saturate(Sun.y * 4.0 + 0.15) * (1.0 - Cloud * 0.92);
    Color += SunColor * (Disc * 1.6 + Glow) * SunUp;

    float2 CloudUv = Dir.xz / max(Up, 0.12);
    float Drift = Time * 0.008;
    float Soft = FadeNoise(CloudUv * 0.32 + Drift);
    float Puff = FadeNoise(CloudUv * 0.85 - Drift * 1.7);
    float Mask = smoothstep(0.46, 0.78, Soft * 0.62 + Puff * 0.38);
    Mask *= saturate(Up * 5.0) * lerp(0.22, 1.0, Cloud);
    float3 CloudColor = lerp(float3(0.86, 0.89, 0.92), float3(0.52, 0.56, 0.60), Cloud);
    Color = lerp(Color, CloudColor, Mask * lerp(0.28, 0.9, Cloud));

    float Night = saturate(-Sun.y);
    float Spark = step(0.993, Hash12(floor(Dir.xy * 96.0)));
    Color += Spark * Night * (1.0 - Cloud) * saturate(Up * 2.0) * 0.85;

    return float4(Color, 1);
}

technique Sky
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PS();
    }
}
