using System;

namespace WoWPacketViewer.Parsers.Spells
{
    [Flags]
    internal enum CastFlags : uint
    {
        CAST_FLAG_00 = 0x00000000,
        CAST_FLAG_01 = 0x00000001,
        CAST_FLAG_02 = 0x00000002,
        CAST_FLAG_03 = 0x00000004,
        CAST_FLAG_04 = 0x00000008,
        CAST_FLAG_05 = 0x00000010,
        CAST_FLAG_06 = 0x00000020,
        CAST_FLAG_07 = 0x00000040,
        CAST_FLAG_08 = 0x00000080,
        CAST_FLAG_09 = 0x00000100,
        CAST_FLAG_10 = 0x00000200,
        CAST_FLAG_11 = 0x00000400,
        CAST_FLAG_12 = 0x00000800,
        CAST_FLAG_13 = 0x00001000,
        CAST_FLAG_14 = 0x00002000,
        CAST_FLAG_15 = 0x00004000,
        CAST_FLAG_16 = 0x00008000,
        CAST_FLAG_17 = 0x00010000,
        CAST_FLAG_18 = 0x00020000,
        CAST_FLAG_19 = 0x00040000,
        CAST_FLAG_20 = 0x00080000,
        CAST_FLAG_21 = 0x00100000,
        CAST_FLAG_22 = 0x00200000,
        CAST_FLAG_23 = 0x00400000,
        CAST_FLAG_24 = 0x00800000,
        CAST_FLAG_25 = 0x01000000,
        CAST_FLAG_26 = 0x02000000,
        CAST_FLAG_27 = 0x04000000,
        CAST_FLAG_28 = 0x08000000,
        CAST_FLAG_29 = 0x10000000,
        CAST_FLAG_30 = 0x20000000,
        CAST_FLAG_31 = 0x40000000,
        CAST_FLAG_32 = 0x80000000
    }
}