using ReunionMovementDLL.Dungeon.Random;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReunionMovementDLL.Dungeon.Util
{
    /// <summary>
    /// Perlin 噪声生成器封装，提供 2D/3D 噪声与倍频噪声（octave）方法。
    /// 内部使用 XorShift128 作为随机置换表的来源。
    /// </summary>
    public class PerlinNoise
    {
        // 内部随机数生成器（用于生成置换表初始种子）
        private XorShift128 rand;
        // 置换表（长度 512，前 256 为原始顺序或扰动后的排列，后 256 为重复）
        private int[] p = new int[512];

        /// <summary>
        /// Perlin 平滑函数（6t^5 - 15t^4 + 10t^3），用于计算插值权重。
        /// </summary>
        /// <param name="t">输入（通常为分数部分）</param>
        /// <returns>平滑权重</returns>
        private double GetFade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        /// <summary>
        /// 线性插值函数。
        /// </summary>
        /// <param name="t">插值因子</param>
        /// <param name="a">起始值</param>
        /// <param name="b">结束值</param>
        /// <returns>插值结果</returns>
        private double GetLerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }

        /// <summary>
        /// 计算 2 维梯度贡献（辅助），根据 hash 位选择正负符号。
        /// </summary>
        private double MakeGradUV(int hash, double u, double v)
        {
            return ((hash & 1) == 0 ? u : -u) + ((hash & 2) == 0 ? v : -v);
        }

        /// <summary>
        /// 根据 hash 选择不同的分量作为 2D 梯度，并返回贡献。
        /// </summary>
        private double MakeGrad(int hash, double x, double y)
        {
            return MakeGradUV(hash, (hash < 8) ? x : y, (hash < 4) ? y : (hash == 12 || hash == 14 ? x : 0.0));
        }

        /// <summary>
        /// 根据 hash 选择不同的分量作为 3D 梯度，并返回贡献。
        /// </summary>
        private double MakeGrad(int hash, double x, double y, double z)
        {
            return MakeGradUV(hash, (hash < 8) ? x : y, (hash < 4) ? y : (hash == 12 || hash == 14 ? x : z));
        }

        /// <summary>
        /// 获取 2D 梯度贡献（包装），将 hash 限制到 0-15。
        /// </summary>
        private double GetGrad(int hash, double x, double y)
        {
            return MakeGrad(hash & 15, x, y);
        }

        /// <summary>
        /// 获取 3D 梯度贡献（包装），将 hash 限制到 0-15。
        /// </summary>
        private double GetGrad(int hash, double x, double y, double z)
        {
            return MakeGrad(hash & 15, x, y, z);
        }

        /// <summary>
        /// 计算二维 Perlin 基础噪声（不做归一化）。
        /// 对输入做整/分数分离，并使用置换表进行混合与插值。
        /// </summary>
        private double SetNoise(double x = 0.0, double y = 0.0)
        {
            // 缓存置换表引用以加速访问
            var perm = this.p;

            double fx = Math.Floor(x);
            double fy = Math.Floor(y);
            uint xInt = (uint)fx & 255;
            uint yInt = (uint)fy & 255;
            double xr = x - fx;
            double yr = y - fy;

            double u = GetFade(xr);
            double v = GetFade(yr);

            uint a0 = (uint)(perm[xInt] + yInt);
            uint a1 = (uint)perm[a0];
            uint a2 = (uint)perm[a0 + 1];
            uint b0 = (uint)(perm[xInt + 1] + yInt);
            uint b1 = (uint)perm[b0];
            uint b2 = (uint)perm[b0 + 1];

            double g1 = GetGrad(perm[a1], xr, yr);
            double g2 = GetGrad(perm[b1], xr - 1, yr);
            double g3 = GetGrad(perm[a2], xr, yr - 1);
            double g4 = GetGrad(perm[b2], xr - 1, yr - 1);

            return GetLerp(v, GetLerp(u, g1, g2), GetLerp(u, g3, g4));
        }

        /// <summary>
        /// 计算三维 Perlin 基础噪声（不做归一化）。
        /// </summary>
        private double SetNoise(double x, double y, double z)
        {
            var perm = this.p;

            double fx = Math.Floor(x);
            double fy = Math.Floor(y);
            double fz = Math.Floor(z);
            uint xInt = (uint)fx & 255;
            uint yInt = (uint)fy & 255;
            uint zInt = (uint)fz & 255;
            double xr = x - fx;
            double yr = y - fy;
            double zr = z - fz;

            double u = GetFade(xr);
            double v = GetFade(yr);
            double w = GetFade(zr);

            uint a0 = (uint)perm[xInt] + yInt;
            uint a1 = (uint)perm[a0] + zInt;
            uint a2 = (uint)perm[a0 + 1] + zInt;
            uint b0 = (uint)perm[xInt + 1] + yInt;
            uint b1 = (uint)perm[b0] + zInt;
            uint b2 = (uint)perm[b0 + 1] + zInt;

            double g000 = GetGrad(perm[a1], xr, yr, zr);
            double g100 = GetGrad(perm[b1], xr - 1, yr, zr);
            double g010 = GetGrad(perm[a2], xr, yr - 1, zr);
            double g110 = GetGrad(perm[b2], xr - 1, yr - 1, zr);
            double g001 = GetGrad(perm[a1 + 1], xr, yr, zr - 1);
            double g101 = GetGrad(perm[b1 + 1], xr - 1, yr, zr - 1);
            double g011 = GetGrad(perm[a2 + 1], xr, yr - 1, zr - 1);
            double g111 = GetGrad(perm[b2 + 1], xr - 1, yr - 1, zr - 1);

            double lerpXY1 = GetLerp(u, g000, g100);
            double lerpXY2 = GetLerp(u, g010, g110);
            double lerpXY3 = GetLerp(u, g001, g101);
            double lerpXY4 = GetLerp(u, g011, g111);

            double lerpYZ1 = GetLerp(v, lerpXY1, lerpXY2);
            double lerpYZ2 = GetLerp(v, lerpXY3, lerpXY4);

            return GetLerp(w, lerpYZ1, lerpYZ2);
        }

        /// <summary>
        /// 计算单次 octave 的 1D 倍频噪声（不归一化）。
        /// </summary>
        private double SetOctaveNoise(uint octaves, double x)
        {
            double noiseValue = 0;
            double amp = 1.0;
            for (int i = 0; i < octaves; ++i)
            {
                noiseValue += SetNoise(x) * amp;
                x *= 2.0;
                amp *= 0.5;
            }

            return noiseValue;
        }

        /// <summary>
        /// 计算单次 octave 的 2D 倍频噪声（不归一化）。
        /// </summary>
        private double SetOctaveNoise(uint octaves, double x, double y)
        {
            double noiseValue = 0;
            double amp = 1.0;
            for (int i = 0; i < octaves; ++i)
            {
                noiseValue += SetNoise(x, y) * amp;
                x *= 2.0;
                y *= 2.0;
                amp *= 0.5;
            }

            return noiseValue;
        }

        /// <summary>
        /// 计算单次 octave 的 3D 倍频噪声（不归一化）。
        /// </summary>
        private double SetOctaveNoise(uint octaves, double x, double y, double z)
        {
            double noiseValue = 0;
            double amp = 1.0;
            for (int i = 0; i < octaves; ++i)
            {
                noiseValue += SetNoise(x, y, z) * amp;
                x *= 2.0;
                y *= 2.0;
                z *= 2.0;
                amp *= 0.5;
            }

            return noiseValue;
        }

        /// <summary>
        /// 2D 噪声，结果归一化到 [0,1]。
        /// </summary>
        /// <param name="x">X 坐标</param>
        /// <param name="y">Y 坐标</param>
        /// <returns>归一化噪声值</returns>
        public double Noise(double x, double y)
        {
            double noiseValue = SetNoise(x, y) * 0.5 + 0.5;
            return (noiseValue >= 1.0) ? 1.0 : (noiseValue <= 0.0) ? 0.0 : noiseValue;
        }

        /// <summary>
        /// 3D 噪声，结果归一化到 [0,1]。
        /// </summary>
        public double Noise(double x, double y, double z)
        {
            double noiseValue = SetNoise(x, y, z) * 0.5 + 0.5;
            return (noiseValue >= 1.0) ? 1.0 : (noiseValue <= 0.0) ? 0.0 : noiseValue;
        }

        /// <summary>
        /// 单次倍频噪声（octaves），1D。
        /// </summary>
        public double OctaveNoise(uint octaves_, double x)
        {
            double noiseValue = SetOctaveNoise(octaves_, x) * 0.5 + 0.5;
            return (noiseValue >= 1.0) ? 1.0 : (noiseValue <= 0.0) ? 0.0 : noiseValue;
        }

        /// <summary>
        /// 2D 倍频噪声（octaves）。
        /// </summary>
        public double OctaveNoise(uint octaves_, double x, double y)
        {
            double noiseValue = SetOctaveNoise(octaves_, x, y) * 0.5 + 0.5;
            return (noiseValue >= 1.0) ? 1.0 : (noiseValue <= 0.0) ? 0.0 : noiseValue;
        }

        /// <summary>
        /// 3D 倍频噪声（octaves）。
        /// </summary>
        public double OctaveNoise(uint octaves_, double x, double y, double z)
        {
            double noiseValue = SetOctaveNoise(octaves_, x, y, z) * 0.5 + 0.5;
            return (noiseValue >= 1.0) ? 1.0 : (noiseValue <= 0.0) ? 0.0 : noiseValue;
        }

        /// <summary>
        /// 根据给定的种子初始化置换表，并使用内部随机器乱序生成置换。
        /// </summary>
        /// <param name="seed">用于初始化置换表的种子</param>
        public void SetSeed(uint seed)
        {
            for (int i = 0; i < 256; ++i)
                this.p[i] = i;
            ArrayUtil.Shuffle(p, 256, rand);
            for (int i = 0; i < 256; ++i)
            {
                this.p[256 + i] = this.p[i];
            }
        }

        /// <summary>
        /// 默认构造函数，使用 XorShift128 作为内部随机器并随机初始化置换表。
        /// </summary>
        public PerlinNoise()
        {
            this.rand = new XorShift128();
            SetSeed(rand.Next());
        }

        /// <summary>
        /// 使用指定种子构造 PerlinNoise，保证可重复性。
        /// </summary>
        /// <param name="seed">有符号种子</param>
        public PerlinNoise(int seed)
        {
            this.rand = new XorShift128((uint)seed);
            SetSeed((uint)seed);
        }
    }
}