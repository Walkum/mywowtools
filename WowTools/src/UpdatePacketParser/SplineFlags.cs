﻿using System;

namespace WoWObjects
{
    [Flags]
    public enum SplineFlags : uint
    {
        NONE = 0x00000000,
        UNK01 = 0x00000001,
        UNK02 = 0x00000002,
        UNK03 = 0x00000004,
        UNK04 = 0x00000008,
        UNK05 = 0x00000010,
        UNK06 = 0x00000020,
        UNK07 = 0x00000040,
        POINT = 0x00008000,
        TARGET = 0x00010000,
        ORIENT = 0x00020000,
        UNK11 = 0x00000400,
        UNK12 = 0x00000800,
        UNK13 = 0x00001000,
        UNK14 = 0x00002000,
        UNK15 = 0x00004000,
        UNK16 = 0x00008000,
        UNK17 = 0x00010000,
        UNK18 = 0x00020000,
        UNK19 = 0x00040000,
        UNK20 = 0x00080000,
        UNK21 = 0x00100000,
        UNK22 = 0x00200000,
        UNK23 = 0x00400000,
        UNK24 = 0x00800000,
        UNK25 = 0x01000000,
        UNK26 = 0x02000000,
        UNK27 = 0x04000000,
        UNK28 = 0x08000000,
        UNK29 = 0x10000000,
        UNK30 = 0x20000000,
        UNK31 = 0x40000000,
        UNK32 = 0x80000000,
    };
}
