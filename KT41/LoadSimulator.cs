using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KT41
{
    public readonly record struct SimulationResult(
        string MethodName,
        double ElapsedMs,
        int Gen0Delta,
        int Gen1Delta,
        int Gen2Delta);

    public class LoadSimulator
    {
        public const int DefaultShortLivedIterations = 300_000;
        public const int ShortLivedArraySize = 200;

        public const int DefaultLongLivedCount = 5_000;
        public const int LongLivedArraySize = 512;

        public const int DefaultHeavyLohCount = 20;
        public const int HeavyLohArraySize = 100_000;

        private const int Gen0 = 0;
        private const int Gen1 = 1;
        private const int Gen2 = 2;

        private readonly List<byte[]> _longLivedStorage = new();
        private readonly List<byte[]> _heavyLohStorage = new();

        public SimulationResult ShortLived(int iterations = DefaultShortLivedIterations)
        {
            int g0Before = GC.CollectionCount(Gen0);
            int g1Before = GC.CollectionCount(Gen1);
            int g2Before = GC.CollectionCount(Gen2);

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                byte[] temp = new byte[ShortLivedArraySize];
                temp[0] = (byte)(i & 0xFF);
            }
            sw.Stop();

            return new SimulationResult(
                $"ShortLived ({iterations})",
                sw.Elapsed.TotalMilliseconds,
                GC.CollectionCount(Gen0) - g0Before,
                GC.CollectionCount(Gen1) - g1Before,
                GC.CollectionCount(Gen2) - g2Before);
        }

        public SimulationResult LongLived(int count = DefaultLongLivedCount)
        {
            int g0Before = GC.CollectionCount(Gen0);
            int g1Before = GC.CollectionCount(Gen1);
            int g2Before = GC.CollectionCount(Gen2);

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                byte[] item = new byte[LongLivedArraySize];
                item[0] = (byte)(i & 0xFF);
                _longLivedStorage.Add(item);
            }
            sw.Stop();

            return new SimulationResult(
                $"LongLived ({count})",
                sw.Elapsed.TotalMilliseconds,
                GC.CollectionCount(Gen0) - g0Before,
                GC.CollectionCount(Gen1) - g1Before,
                GC.CollectionCount(Gen2) - g2Before);
        }

        public SimulationResult HeavyLoh(int count = DefaultHeavyLohCount)
        {
            int g0Before = GC.CollectionCount(Gen0);
            int g1Before = GC.CollectionCount(Gen1);
            int g2Before = GC.CollectionCount(Gen2);

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                byte[] largeItem = new byte[HeavyLohArraySize];
                largeItem[0] = (byte)(i & 0xFF);
                _heavyLohStorage.Add(largeItem);
            }
            sw.Stop();

            return new SimulationResult(
                $"HeavyLoh ({count})",
                sw.Elapsed.TotalMilliseconds,
                GC.CollectionCount(Gen0) - g0Before,
                GC.CollectionCount(Gen1) - g1Before,
                GC.CollectionCount(Gen2) - g2Before);
        }

        public SimulationResult ForceBlockingCollect()
        {
            int g0Before = GC.CollectionCount(Gen0);
            int g1Before = GC.CollectionCount(Gen1);
            int g2Before = GC.CollectionCount(Gen2);

            Stopwatch sw = Stopwatch.StartNew();
            GC.Collect(Gen2, GCCollectionMode.Forced, blocking: true);
            sw.Stop();

            return new SimulationResult(
                "GC.Collect(2, blocking)",
                sw.Elapsed.TotalMilliseconds,
                GC.CollectionCount(Gen0) - g0Before,
                GC.CollectionCount(Gen1) - g1Before,
                GC.CollectionCount(Gen2) - g2Before);
        }

        public void ClearMemory()
        {
            _longLivedStorage.Clear();
            _heavyLohStorage.Clear();
        }
    }
}