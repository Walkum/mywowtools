using System.IO;
using System.Text;
using Defines;
using WoWReader;

namespace OpcodeParsers
{
    public class OpcodeParsers
    {
        /// <summary>
        /// Monster move opcode parser method.
        /// </summary>
        /// <param name="gr">Main stream reader.</param>
        /// <param name="gr2">Packet stream reader.</param>
        /// <param name="sb">Logger string builder.</param>
        /// <param name="swe">Error logger writer.</param>
        /// <returns>Successful</returns>
        public static bool ParseMonsterMoveOpcode(GenericReader gr, GenericReader gr2, StringBuilder sb, StreamWriter swe)
        {
            sb.AppendLine("Packet offset " + gr.BaseStream.Position.ToString("X2"));
            sb.AppendLine("Opcode SMSG_MONSTER_MOVE (0x00DD)");

            ulong guid = gr2.ReadPackedGuid();
            sb.AppendLine("GUID " + guid.ToString("X16"));

            Coords3 coords = gr2.ReadCoords3();
            sb.AppendLine("Start point " + coords.GetCoords());

            uint time = gr2.ReadUInt32();
            sb.AppendLine("Time " + time);

            byte unk = gr2.ReadByte();
            sb.AppendLine("unk_byte " + unk);

            switch (unk)
            {
                case 0: // ������� �����
                    break;
                case 1: // ����, ����� ������...
                    sb.AppendLine("stop");
                    return true;
                case 3: // ���-�� ����, ������ ����� �������...
                    ulong target_guid = gr2.ReadUInt64();
                    sb.AppendLine("GUID unknown " + target_guid.ToString("X16"));
                    break;
                case 4: // ������ �� ����������...
                    float orientation = gr2.ReadSingle();
                    sb.AppendLine("Orientation " + orientation.ToString().Replace(",", "."));
                    break;
                default:
                    swe.WriteLine("Error in position " + gr.BaseStream.Position.ToString("X2"));
                    swe.WriteLine("unknown unk " + unk);
                    break;
            }

            Flags flags = (Flags)gr2.ReadUInt32();
            sb.AppendLine("Flags " + flags);

            uint movetime = gr2.ReadUInt32();
            sb.AppendLine("MoveTime " + movetime);

            uint points = gr2.ReadUInt32();
            sb.AppendLine("Points " + points);

            if ((flags & Flags.flag10) != 0) // 0x200
            {
                sb.AppendLine("Taxi");
                for (uint i = 0; i < points; i++)
                {
                    Coords3 path = gr2.ReadCoords3();
                    sb.AppendLine("Path point" + i + ": " + path.GetCoords());
                }
            }
            else
            {
                if ((flags & Flags.flag09) == 0 && (flags & Flags.flag10) == 0 && flags != 0)
                {
                    swe.WriteLine("Unknown flags " + flags);
                }

                if ((flags & Flags.flag09) != 0)
                    sb.AppendLine("Running");

                Coords3 end = gr2.ReadCoords3();
                sb.AppendLine("End point " + end.GetCoords());

                for (uint i = 0; i < (points - 1); i++)
                {
                    uint unk2 = gr2.ReadUInt32();
                    sb.AppendLine("vector" + i + " " + unk2.ToString("X8"));
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="gr2"></param>
        /// <param name="sb"></param>
        /// <param name="swe"></param>
        /// <returns></returns>
        public static bool ParseInitialSpellsOpcode(GenericReader gr, GenericReader gr2, StringBuilder sb, StreamWriter swe)
        {
            sb.AppendLine("Packet offset " + gr.BaseStream.Position.ToString("X2"));
            sb.AppendLine("Opcode SMSG_INITIAL_SPELLS (0x012A)");

            byte unk1 = gr2.ReadByte();

            sb.AppendLine("unk byte " + unk1);

            ushort spells_count = gr2.ReadUInt16();
            sb.AppendLine("Spells count " + spells_count);
            for (ushort i = 0; i < spells_count; i++)
            {
                ushort spellid = gr2.ReadUInt16();
                ushort slotid = gr2.ReadUInt16();
                sb.AppendLine("Spell ID " + spellid + ", slot " + slotid.ToString("X2"));
            }

            ushort cooldowns_count = gr2.ReadUInt16();
            sb.AppendLine("Cooldowns count " + cooldowns_count);
            for (ushort i = 0; i < cooldowns_count; i++)
            {
                ushort spellid = gr2.ReadUInt16();
                ushort itemid = gr2.ReadUInt16();
                ushort spellcategory = gr2.ReadUInt16();
                uint cooldown1 = gr2.ReadUInt32();
                uint cooldown2 = gr2.ReadUInt32();
                sb.AppendLine("Spell Cooldown: spell id " + spellid + ", itemid " + itemid + ", spellcategory " + spellcategory + ", cooldown1 " + cooldown1 + ", cooldown2 " + cooldown2);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="gr2"></param>
        /// <param name="sb"></param>
        /// <param name="swe"></param>
        /// <returns></returns>
        public static bool ParseAuctionListResultOpcode(GenericReader gr, GenericReader gr2, StringBuilder sb, StreamWriter swe)
        {
            sb.AppendLine("Packet offset ");
            sb.AppendLine(gr.BaseStream.Position.ToString("X2"));
            sb.AppendLine("Opcode SMSG_AUCTION_LIST_RESULT (0x025C)");

            uint count = gr2.ReadUInt32();

            sb.AppendLine("count ");
            sb.AppendLine(count.ToString());

            for (uint i = 0; i < count; i++)
            {
                uint auction_id = gr2.ReadUInt32();
                sb.AppendLine("auction_id " + auction_id);
                uint item_entry = gr2.ReadUInt32();
                sb.AppendLine("item_entry " + item_entry);

                uint ench1_1 = gr2.ReadUInt32();
                uint ench1_2 = gr2.ReadUInt32();
                uint ench1_3 = gr2.ReadUInt32();
                sb.AppendLine("enchant1 " + ench1_1 + " " + ench1_2 + " " + ench1_3);

                uint ench2_1 = gr2.ReadUInt32();
                uint ench2_2 = gr2.ReadUInt32();
                uint ench2_3 = gr2.ReadUInt32();
                sb.AppendLine("enchant2 " + ench2_1 + " " + ench2_2 + " " + ench2_3);

                uint socket1_1 = gr2.ReadUInt32();
                uint socket1_2 = gr2.ReadUInt32();
                uint socket1_3 = gr2.ReadUInt32();
                sb.AppendLine("socket1 " + socket1_1 + " " + socket1_2 + " " + socket1_3);

                uint socket2_1 = gr2.ReadUInt32();
                uint socket2_2 = gr2.ReadUInt32();
                uint socket2_3 = gr2.ReadUInt32();
                sb.AppendLine("socket2 " + socket2_1 + " " + socket2_2 + " " + socket2_3);

                uint socket3_1 = gr2.ReadUInt32();
                uint socket3_2 = gr2.ReadUInt32();
                uint socket3_3 = gr2.ReadUInt32();
                sb.AppendLine("socket3 " + socket3_1 + " " + socket3_2 + " " + socket3_3);

                uint bonus_1 = gr2.ReadUInt32();
                uint bonus_2 = gr2.ReadUInt32();
                uint bonus_3 = gr2.ReadUInt32();
                sb.AppendLine("bonus " + bonus_1 + " " + bonus_2 + " " + bonus_3);

                uint rand = gr2.ReadUInt32();
                sb.AppendLine("random property " + rand);

                uint unk1 = gr2.ReadUInt32();
                sb.AppendLine("unk1 " + unk1);

                uint itemcount = gr2.ReadUInt32();
                sb.AppendLine("item count " + itemcount);

                uint charges = gr2.ReadUInt32();
                sb.AppendLine("charges " + charges);
                uint unk2 = gr2.ReadUInt32();
                sb.AppendLine("unk2 " + unk2);

                if (unk2 != 0)
                    swe.WriteLine("unk2 " + unk2);

                ulong owner = gr2.ReadUInt64();
                uint startbid = gr2.ReadUInt32();
                uint outbid = gr2.ReadUInt32();
                uint buyout = gr2.ReadUInt32();
                uint time = gr2.ReadUInt32();
                sb.AppendLine("owner: " + owner + " " + startbid + " " + outbid + " " + buyout + " " + time);

                ulong bidder = gr2.ReadUInt64();
                uint bid = gr2.ReadUInt32();
                sb.AppendLine("bidder " + bidder + " " + bid);
                sb.AppendLine();
            }

            uint totalcount = gr2.ReadUInt32();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gr"></param>
        /// <param name="gr2"></param>
        /// <param name="sb"></param>
        /// <param name="swe"></param>
        /// <returns></returns>
        public static bool ParsePartyMemberStatsOpcode(GenericReader gr, GenericReader gr2, StringBuilder sb, StreamWriter swe)
        {
            sb.AppendLine("Packet offset " + gr.BaseStream.Position.ToString("X2"));
            sb.AppendLine("Opcode SMSG_PARTY_MEMBER_STATS (0x007E)");

            ulong guid = gr2.ReadPackedGuid();
            sb.AppendLine("GUID " + guid.ToString("X16"));

            Flags flags = (Flags)gr2.ReadUInt32();
            sb.AppendLine("Flags " + flags);

            if ((flags & Flags.flag01) != 0)
            {
                byte unk1 = gr2.ReadByte(); // flag
                sb.AppendLine("flag 0x00000001, value " + unk1);
            }
            if ((flags & Flags.flag02) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00000002, value " + unk1);
            }
            if ((flags & Flags.flag03) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00000004, value " + unk1);
            }
            if ((flags & Flags.flag04) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00000008, value " + unk1);
            }
            if ((flags & Flags.flag05) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00000010, value " + unk1);
            }
            if ((flags & Flags.flag06) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00000020, value " + unk1);
            }
            if ((flags & Flags.flag07) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00000040, value " + unk1);
            }
            if ((flags & Flags.flag08) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00000080, value " + unk1);
            }
            if ((flags & Flags.flag09) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00000100, value1 " + unk1);
                ushort unk2 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00000100, value2 " + unk1);
            }
            if ((flags & Flags.flag10) != 0)
            {
                swe.Write("flag10 there: ");
                swe.WriteLine(gr.BaseStream.Position);
                ulong unk1 = gr2.ReadUInt64();
                sb.AppendLine("flag 0x00000200, value " + unk1.ToString("X16"));
                for (uint i = 0; i < 0; i++)    // unknown
                {

                }
            }
            if ((flags & Flags.flag11) != 0)
            {
                ulong unk1 = gr2.ReadUInt64();
                sb.AppendLine("flag 0x00000400, value " + unk1);
            }
            if ((flags & Flags.flag12) != 0)
            {
                string str = gr2.ReadStringNull();
                sb.AppendLine("flag 0x00000800, value " + str);
            }
            if ((flags & Flags.flag13) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00001000, value " + unk1);
            }
            if ((flags & Flags.flag14) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00002000, value " + unk1);
            }
            if ((flags & Flags.flag15) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00004000, value " + unk1);
            }
            if ((flags & Flags.flag16) != 0)
            {
                byte unk1 = gr2.ReadByte();
                sb.AppendLine("flag 0x00008000, value " + unk1);
            }
            if ((flags & Flags.flag17) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00010000, value " + unk1);
            }
            if ((flags & Flags.flag18) != 0)
            {
                ushort unk1 = gr2.ReadUInt16();
                sb.AppendLine("flag 0x00020000, value " + unk1);
            }
            if ((flags & Flags.flag19) != 0)
            {
                swe.Write("flag19 there: ");
                swe.WriteLine(gr.BaseStream.Position);
                ulong unk1 = gr2.ReadUInt64();
                sb.AppendLine("flag 0x00040000, value " + unk1.ToString("X16"));
                for (uint i = 0; i < 0; i++)    // unknown
                {

                }
            }

            if (gr2.BaseStream.Position == gr2.BaseStream.Length)
                sb.AppendLine("parsed: ok...");
            else
                sb.AppendLine("parsed: error...");

            return true;
        }

        public static bool ParseTrainerListOpcode(GenericReader gr, GenericReader gr2, StringBuilder sb, StreamWriter swe)
        {
            sb.AppendLine("Packet offset " + gr.BaseStream.Position.ToString("X2"));
            sb.AppendLine("Opcode SMSG_TRAINER_LIST (0x01B1)");

            StreamWriter sw = new StreamWriter("trainer.log", true, Encoding.ASCII);

            ulong guid = gr2.ReadUInt64();
            TrainerType trainer_type = (TrainerType)gr2.ReadUInt32();
            uint spells_count = gr2.ReadUInt32();

            sw.WriteLine("Trainer {0}, type {1}, spells_count {2}", guid.ToString("X16"), trainer_type, spells_count);

            for (uint i = 0; i < spells_count; i++)
            {
                uint spellid = gr2.ReadUInt32();
                TrainerSpellState state = (TrainerSpellState)gr2.ReadByte();
                uint spellcost = gr2.ReadUInt32();
                uint unk1 = gr2.ReadUInt32();   // isProfession?
                uint unk2 = gr2.ReadUInt32();
                byte reqlevel = gr2.ReadByte();
                uint reqskill = gr2.ReadUInt32();
                uint reqskillvalue = gr2.ReadUInt32();
                uint reqspell = gr2.ReadUInt32();
                uint unk3 = gr2.ReadUInt32();
                uint unk4 = gr2.ReadUInt32();

                sw.WriteLine("Spell {0}, state {1}, cost {2}, unk1 {3}, unk2 {4}, reqlevel {5}, reqskill {6}, reqskillvalue {7}, reqspell {8}, unk3 {9} unk4 {10}", spellid, state, spellcost, unk1, unk2, reqlevel, reqskill, reqskillvalue, reqspell, unk3, unk4);
            }

            string title = gr2.ReadStringNull();
            sw.WriteLine("title {0}", title);

            sw.Flush();
            sw.Close();

            if (gr2.BaseStream.Position == gr2.BaseStream.Length)
                sb.AppendLine("parsed: ok...");
            else
                sb.AppendLine("parsed: error...");

            return true;
        }

        public static bool ParseAttackerStateUpdateOpcode(GenericReader gr, GenericReader gr2, StringBuilder sb, StreamWriter swe)
        {
            sb.AppendLine("Packet offset " + gr.BaseStream.Position.ToString("X2"));
            sb.AppendLine("Opcode SMSG_ATTACKERSTATEUPDATE (0x01B1)");

            StreamWriter sw = new StreamWriter("attacker_state.log", true, Encoding.ASCII);

            HitInfo hi = (HitInfo)gr2.ReadUInt32();
            ulong attacker = gr2.ReadPackedGuid();
            ulong target = gr2.ReadPackedGuid();

            uint damage = gr2.ReadUInt32();

            sw.WriteLine("HitInfo {0}", hi);
            sw.WriteLine("attacker {0}", attacker.ToString("X16"));
            sw.WriteLine("target {0}", target.ToString("X16"));
            sw.WriteLine("damage {0}", damage);

            byte count = gr2.ReadByte();
            sw.WriteLine("count {0}", count);

            for (byte i = 0; i < count; i++)
            {
                ITEM_DAMAGE_TYPE damagetype = (ITEM_DAMAGE_TYPE)gr2.ReadUInt32();
                float damage2 = gr2.ReadSingle();
                uint damage3 = gr2.ReadUInt32();
                uint adsorb = gr2.ReadUInt32();
                uint resist = gr2.ReadUInt32();

                sw.WriteLine("damagetype {0}", damagetype);
                sw.WriteLine("damage2 {0}", damage2);
                sw.WriteLine("damage3 {0}", damage3);
                sw.WriteLine("adsorb {0}", adsorb);
                sw.WriteLine("resist {0}", resist);
            }

            VictimState targetstate = (VictimState)gr2.ReadUInt32();
            uint unk1 = gr2.ReadUInt32();
            uint unk2 = gr2.ReadUInt32();
            uint blocked = gr2.ReadUInt32();

            sw.WriteLine("targetstate {0}", targetstate);
            sw.WriteLine("unk1 {0}", unk1);
            sw.WriteLine("unk2 {0}", unk2);
            sw.WriteLine("blocked {0}", blocked);
            sw.WriteLine();

            sw.Flush();
            sw.Close();

            if (gr2.BaseStream.Position == gr2.BaseStream.Length)
                sb.AppendLine("parsed: ok...");
            else
                sb.AppendLine("parsed: error...");

            return true;
        }
    }
}
