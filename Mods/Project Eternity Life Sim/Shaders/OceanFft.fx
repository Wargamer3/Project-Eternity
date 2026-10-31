// 64x64 inverse FFT for three cascades. Complex values live in RG.
#define N 64
#define TWO_PI 6.28318530718
#define G 9.81

float Time;
float Tile;
float Len;
float Inv = 1;
float TargetRms;
float Fold;
float World;
float FoamOn;

texture Spec;
sampler SpecSampler = sampler_state
{
    Texture = (Spec);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

texture Src0;
texture Src1;
texture Src2;
texture ScaleMap;

sampler S0 = sampler_state
{
    Texture = (Src0);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler S1 = sampler_state
{
    Texture = (Src1);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler S2 = sampler_state
{
    Texture = (Src2);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler ScaleSampler = sampler_state
{
    Texture = (ScaleMap);
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = None;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VSOut
{
    float4 Clip : SV_Position;
};

struct Trio
{
    float4 A : SV_Target0;
    float4 B : SV_Target1;
    float4 C : SV_Target2;
};

VSOut VS(float4 Position : POSITION0)
{
    VSOut Output;
    Output.Clip = float4(Position.xy, 0, 1);
    return Output;
}

float4 At(sampler Map, int X, int Y, float Size)
{
    return tex2Dlod(Map, float4((float2(X, Y) + 0.5) / Size, 0, 0));
}

int Rev6(int X)
{
    return ((X & 1) << 5) | ((X & 2) << 3) | ((X & 4) << 1)
        | ((X & 8) >> 1) | ((X & 16) >> 3) | ((X & 32) >> 5);
}

float2 Mul(float2 B, float2 W)
{
    return float2(B.x * W.x - B.y * W.y, B.x * W.y + B.y * W.x);
}

float2 Butterfly(float2 Low, float2 HighValue, int Local, bool Upper, float Scale)
{
    float Ang = TWO_PI * Local / Len;
    float2 W = float2(cos(Ang), sin(Ang));
    float2 Mixed = Mul(HighValue, W);
    float2 Sum = Upper ? Low - Mixed : Low + Mixed;
    return Sum * Scale;
}

Trio PSEvolve(VSOut Input)
{
    int X = (int)Input.Clip.x;
    int Z = (int)Input.Clip.y;
    int Kx = X <= (N / 2) ? X : X - N;
    int Kz = Z <= (N / 2) ? Z : Z - N;
    int Nx = (N - X) & (N - 1);
    int Nz = (N - Z) & (N - 1);
    float Kxx = Kx * TWO_PI / Tile;
    float Kzz = Kz * TWO_PI / Tile;
    float K = sqrt(Kxx * Kxx + Kzz * Kzz);
    float2 Here = At(SpecSampler, X, Z, N).xy;
    float2 There = At(SpecSampler, Nx, Nz, N).xy;
    float Omega = sqrt(G * max(K, 0)) * Time;
    float C = cos(Omega);
    float S = sin(Omega);
    float Ar = Here.x * C - Here.y * S;
    float Ai = Here.x * S + Here.y * C;
    float Br = There.x * C - There.y * S;
    float Bi = -There.x * S - There.y * C;
    float Hr = Ar + Br;
    float Hi = Ai + Bi;
    Trio Output;
    Output.A = float4(Hr, Hi, 0, 0);
    if (K < 1e-6)
    {
        Output.B = Output.A;
        Output.C = Output.A;
        return Output;
    }

    float Sx = Kxx / K;
    float Sz = Kzz / K;
    Output.B = float4(Sx * Hi, -Sx * Hr, 0, 0);
    Output.C = float4(Sz * Hi, -Sz * Hr, 0, 0);
    return Output;
}

Trio PSBitH(VSOut Input)
{
    int X = (int)Input.Clip.x;
    int Y = (int)Input.Clip.y;
    int From = Rev6(X);
    Trio Output;
    Output.A = At(S0, From, Y, N);
    Output.B = At(S1, From, Y, N);
    Output.C = At(S2, From, Y, N);
    return Output;
}

Trio PSBitV(VSOut Input)
{
    int X = (int)Input.Clip.x;
    int Y = (int)Input.Clip.y;
    int From = Rev6(Y);
    Trio Output;
    Output.A = At(S0, X, From, N);
    Output.B = At(S1, X, From, N);
    Output.C = At(S2, X, From, N);
    return Output;
}

Trio PSButterH(VSOut Input)
{
    int X = (int)Input.Clip.x;
    int Y = (int)Input.Clip.y;
    int Step = (int)Len >> 1;
    int Group = X / (int)Len;
    int K = X - Group * (int)Len;
    bool Upper = K >= Step;
    int Local = Upper ? K - Step : K;
    int Lower = Group * (int)Len + Local;
    int UpperAt = Lower + Step;
    Trio Output;
    Output.A = float4(Butterfly(At(S0, Lower, Y, N).xy, At(S0, UpperAt, Y, N).xy, Local, Upper, Inv), 0, 0);
    Output.B = float4(Butterfly(At(S1, Lower, Y, N).xy, At(S1, UpperAt, Y, N).xy, Local, Upper, Inv), 0, 0);
    Output.C = float4(Butterfly(At(S2, Lower, Y, N).xy, At(S2, UpperAt, Y, N).xy, Local, Upper, Inv), 0, 0);
    return Output;
}

Trio PSButterV(VSOut Input)
{
    int X = (int)Input.Clip.x;
    int Y = (int)Input.Clip.y;
    int Step = (int)Len >> 1;
    int Group = Y / (int)Len;
    int K = Y - Group * (int)Len;
    bool Upper = K >= Step;
    int Local = Upper ? K - Step : K;
    int Lower = Group * (int)Len + Local;
    int UpperAt = Lower + Step;
    Trio Output;
    Output.A = float4(Butterfly(At(S0, X, Lower, N).xy, At(S0, X, UpperAt, N).xy, Local, Upper, Inv), 0, 0);
    Output.B = float4(Butterfly(At(S1, X, Lower, N).xy, At(S1, X, UpperAt, N).xy, Local, Upper, Inv), 0, 0);
    Output.C = float4(Butterfly(At(S2, X, Lower, N).xy, At(S2, X, UpperAt, N).xy, Local, Upper, Inv), 0, 0);
    return Output;
}

float4 PSReduce8(VSOut Input) : SV_Target0
{
    int2 Block = int2((int)Input.Clip.x, (int)Input.Clip.y);
    int2 Base = Block * 8;
    float Sum = 0;
    float Sum2 = 0;
    for (int Z = 0; Z < 8; Z++)
    {
        for (int X = 0; X < 8; X++)
        {
            float H = At(S0, Base.x + X, Base.y + Z, N).x;
            Sum += H;
            Sum2 += H * H;
        }
    }

    return float4(Sum, Sum2, 0, 0);
}

float4 PSReduce1(VSOut Input) : SV_Target0
{
    float Sum = 0;
    float Sum2 = 0;
    for (int Y = 0; Y < 8; Y++)
    {
        for (int X = 0; X < 8; X++)
        {
            float2 Block = At(S0, X, Y, 8).xy;
            Sum += Block.x;
            Sum2 += Block.y;
        }
    }

    float Mean = Sum / (N * N);
    float Rms = sqrt(max(0, Sum2 / (N * N) - Mean * Mean));
    float Scale = Rms > 1e-5 ? TargetRms / Rms : 0;
    return float4(Scale, Mean, 0, 0);
}

float4 PSFinish(VSOut Input) : SV_Target0
{
    int X = (int)Input.Clip.x;
    int Z = (int)Input.Clip.y;
    int Xp = (X + 1) & (N - 1);
    int Xm = (X - 1) & (N - 1);
    int Zp = (Z + 1) & (N - 1);
    int Zm = (Z - 1) & (N - 1);
    float2 Stats = At(ScaleSampler, 0, 0, 1).xy;
    float Scale = Stats.x;
    float Mean = Stats.y;
    float H = (At(S0, X, Z, N).x - Mean) * Scale;
    float DxR = At(S1, Xp, Z, N).x;
    float DxL = At(S1, Xm, Z, N).x;
    float DxU = At(S1, X, Zp, N).x;
    float DxD = At(S1, X, Zm, N).x;
    float DzR = At(S2, Xp, Z, N).x;
    float DzL = At(S2, Xm, Z, N).x;
    float DzU = At(S2, X, Zp, N).x;
    float DzD = At(S2, X, Zm, N).x;
    float Dx = At(S1, X, Z, N).x * Scale * Fold;
    float Dz = At(S2, X, Z, N).x * Scale * Fold;
    float Half = 0.5 * World;
    float Dxx = (DxR - DxL) * Scale * Fold * Half;
    float Dxz = (DxU - DxD) * Scale * Fold * Half;
    float Dzx = (DzR - DzL) * Scale * Fold * Half;
    float Dzz = (DzU - DzD) * Scale * Fold * Half;
    float Jacob = (1 + Dxx) * (1 + Dzz) - Dxz * Dzx;
    float Foam = 0;
    if (FoamOn > 0.5)
    {
        Foam = saturate((0.02 - Jacob) * 6);
    }

    return float4(Dx, H, Dz, Foam);
}

technique Evolve
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PSEvolve();
    }
}

technique BitH
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PSBitH();
    }
}

technique BitV
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PSBitV();
    }
}

technique ButterH
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PSButterH();
    }
}

technique ButterV
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PSButterV();
    }
}

technique Reduce8
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PSReduce8();
    }
}

technique Reduce1
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PSReduce1();
    }
}

technique Finish
{
    pass
    {
        VertexShader = compile vs_4_0 VS();
        PixelShader = compile ps_4_0 PSFinish();
    }
}
