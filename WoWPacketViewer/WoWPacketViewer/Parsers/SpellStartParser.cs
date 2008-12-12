﻿using System;
using System.Text;
using System.IO;

namespace WoWPacketViewer.Parsers
{
    [Parser(OpCodes.SMSG_SPELL_START)]
    internal class SpellStartParser : Parser
    {
        [Flags]
        public enum CastFlags : uint
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

        [Flags]
        public enum TargetFlags : uint
        {
            TARGET_FLAG_SELF = 0x00000000,
            TARGET_FLAG_UNUSED1 = 0x00000001,               // not used in any spells as of 3.0.3 (can be set dynamically)
            TARGET_FLAG_UNIT = 0x00000002,               // pguid
            TARGET_FLAG_UNUSED2 = 0x00000004,               // not used in any spells as of 3.0.3 (can be set dynamically)
            TARGET_FLAG_UNUSED3 = 0x00000008,               // not used in any spells as of 3.0.3 (can be set dynamically)
            TARGET_FLAG_ITEM = 0x00000010,               // pguid
            TARGET_FLAG_SOURCE_LOCATION = 0x00000020,               // 3 float
            TARGET_FLAG_DEST_LOCATION = 0x00000040,               // 3 float
            TARGET_FLAG_OBJECT_UNK = 0x00000080,               // used in 7 spells only
            TARGET_FLAG_UNIT_UNK = 0x00000100,               // looks like self target (480 spells)
            TARGET_FLAG_PVP_CORPSE = 0x00000200,               // pguid
            TARGET_FLAG_UNIT_CORPSE = 0x00000400,               // 10 spells (gathering professions)
            TARGET_FLAG_OBJECT = 0x00000800,               // pguid, 2 spells
            TARGET_FLAG_TRADE_ITEM = 0x00001000,               // pguid, 0 spells
            TARGET_FLAG_STRING = 0x00002000,               // string, 0 spells
            TARGET_FLAG_UNK1 = 0x00004000,               // 199 spells, opening object/lock
            TARGET_FLAG_CORPSE = 0x00008000,               // pguid, resurrection spells
            TARGET_FLAG_UNK2 = 0x00010000,               // pguid, not used in any spells as of 3.0.3 (can be set dynamically)
            TARGET_FLAG_GLYPH = 0x00020000                // used in glyph spells
        }

        [Flags]
        public enum CooldownMask : byte
        {
            RUNE_NONE = 0,
            RUNE_BLOOD_0 = 1,
            RUNE_BLOOD_1 = 2,
            RUNE_UNHOLY_0 = 4,
            RUNE_UNHOLY_1 = 8,
            RUNE_FROST_0 = 16,
            RUNE_FROST_1 = 32
        }

        public SpellStartParser(Packet packet)
            : base(packet)
        {
        }

        public override string Parse()
        {
            var gr = Packet.CreateReader();

            var sb = new StringBuilder();

            sb.AppendFormat("Caster: 0x{0:X16}", gr.ReadPackedGuid()).AppendLine();
            sb.AppendFormat("Target: 0x{0:X16}", gr.ReadPackedGuid()).AppendLine();
            sb.AppendFormat("Pending Cast: {0}", gr.ReadByte()).AppendLine();
            sb.AppendFormat("Spell Id: {0}", gr.ReadUInt32()).AppendLine();
            CastFlags cf = (CastFlags)gr.ReadUInt32();
            sb.AppendFormat("Cast Flags: {0}", cf).AppendLine();
            sb.AppendFormat("Timer: {0}", gr.ReadUInt32()).AppendLine();

            ReadTargets(gr, sb);

            if ((cf & CastFlags.CAST_FLAG_12) != CastFlags.CAST_FLAG_00)
            {
                sb.AppendFormat("PredictedPower: {0}", gr.ReadUInt32()).AppendLine();
            }

            if ((cf & CastFlags.CAST_FLAG_22) != CastFlags.CAST_FLAG_00)
            {
                var v1 = gr.ReadByte();
                sb.AppendFormat("RuneState Before: {0}", (CooldownMask)v1).AppendLine();
                var v2 = gr.ReadByte();
                sb.AppendFormat("RuneState Now: {0}", (CooldownMask)v2).AppendLine();

                for (int i = 0; i < 6; ++i)
                {
                    var v3 = (i << i);

                    if ((v3 & v1) != 0)
                    {
                        if (!((v3 & v2) != 0))
                        {
                            var v4 = gr.ReadByte();
                            sb.AppendFormat("Cooldown for {0} is {1}", (CooldownMask)v3, v4).AppendLine();
                        }
                    }
                }
            }

            if ((cf & CastFlags.CAST_FLAG_06) != CastFlags.CAST_FLAG_00)
            {
                sb.AppendFormat("Projectile displayid {0}, inventoryType {1}", gr.ReadUInt32(), gr.ReadUInt32()).AppendLine();
            }

            if (gr.BaseStream.Position != gr.BaseStream.Length)
            {
                throw new Exception("Packet structure changed!");
            }

            return sb.ToString();
        }

        public static TargetFlags ReadTargets(BinaryReader br, StringBuilder sb)
        {
            TargetFlags tf = (TargetFlags)br.ReadUInt32();
            sb.AppendFormat("TargetFlags: {0}", tf).AppendLine();

            if ((tf & (TargetFlags.TARGET_FLAG_UNIT |
                TargetFlags.TARGET_FLAG_PVP_CORPSE |
                TargetFlags.TARGET_FLAG_OBJECT |
                TargetFlags.TARGET_FLAG_OBJECT_UNK |
                TargetFlags.TARGET_FLAG_CORPSE |
                TargetFlags.TARGET_FLAG_UNK2)) != TargetFlags.TARGET_FLAG_SELF)
            {
                sb.AppendFormat("ObjectTarget: 0x{0:X16}", br.ReadPackedGuid()).AppendLine();
            }

            if ((tf & (TargetFlags.TARGET_FLAG_ITEM |
                TargetFlags.TARGET_FLAG_TRADE_ITEM)) != TargetFlags.TARGET_FLAG_SELF)
            {
                sb.AppendFormat("ItemTarget: 0x{0:X16}", br.ReadPackedGuid()).AppendLine();
            }

            if ((tf & TargetFlags.TARGET_FLAG_SOURCE_LOCATION) != TargetFlags.TARGET_FLAG_SELF)
            {
                sb.AppendFormat("SrcTarget: {0} {1} {2}", br.ReadSingle(), br.ReadSingle(), br.ReadSingle()).AppendLine();
            }

            if ((tf & TargetFlags.TARGET_FLAG_DEST_LOCATION) != TargetFlags.TARGET_FLAG_SELF)
            {
                sb.AppendFormat("DstTarget: {0} {1} {2}", br.ReadSingle(), br.ReadSingle(), br.ReadSingle()).AppendLine();
            }

            if ((tf & TargetFlags.TARGET_FLAG_STRING) != TargetFlags.TARGET_FLAG_SELF)
            {
                sb.AppendFormat("StringTarget: {0}", br.ReadCString()).AppendLine();
            }

            return tf;
        }
    }
}
