﻿using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using MySql.Data.MySqlClient;

namespace character_convertor
{
    enum ObjectType
    {
        TYPE_OBJECT = 1,
        TYPE_ITEM = 2,
        TYPE_CONTAINER = 6,
        TYPE_UNIT = 8,
        TYPE_PLAYER = 16,
        TYPE_GAMEOBJECT = 32,
        TYPE_DYNAMICOBJECT = 64,
        TYPE_CORPSE = 128,
        TYPE_AIGROUP = 256,
        TYPE_AREATRIGGER = 512
    };

    enum ObjectTypeId
    {
        TYPEID_OBJECT = 0,
        TYPEID_ITEM = 1,
        TYPEID_CONTAINER = 2,
        TYPEID_UNIT = 3,
        TYPEID_PLAYER = 4,
        TYPEID_GAMEOBJECT = 5,
        TYPEID_DYNAMICOBJECT = 6,
        TYPEID_CORPSE = 7,
        TYPEID_AIGROUP = 8,
        TYPEID_AREATRIGGER = 9
    };

    class Program
    {
        static string cs = String.Empty;
        static List<Object> objects = new List<Object>();

        static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(TopLevelErrorHandler);

            Console.WriteLine("Character convertor for MaNGoS");
            Console.WriteLine("Client version 2.2.3->2.3.2");

            Console.Write("Enter DB host: ");
            string dbServer = Console.ReadLine();
            Console.Write("Enter DB port: ");
            string dbPort = Console.ReadLine();
            Console.Write("Enter DB name: ");
            string dbName = Console.ReadLine();
            Console.Write("Enter DB user name: ");
            string dbUser = Console.ReadLine();
            Console.Write("Enter DB password: ");
            string dbPass = Console.ReadLine();

            // connection string
            cs = String.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", dbServer, dbPort, dbName, dbUser, dbPass);

            // main query
            string query = "SELECT `data` FROM `character`";

            MySqlConnection connection = new MySqlConnection(cs);

            try
            {
                connection.Open();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                Console.ReadKey();
                return;
            }

            MySqlCommand command;

            try
            {
                command = connection.CreateCommand();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                Console.ReadKey();
                return;
            }

            command.CommandText = query;

            MySqlDataReader reader;

            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                Console.ReadKey();
                return;
            }

            while (reader.Read())
            {
                string data = reader.GetString(0);

                Object src = new Object((ushort)UpdateFieldsOld.PLAYER_END);
                Object dst = new Object((ushort)UpdateFieldsNew.PLAYER_END);

                // load values
                src.LoadValues(data);
                // update fields
                UpdatePlayerFields(src, dst);
                // add to list
                objects.Add(dst);
            }

            // close reader
            reader.Close();

            // now save all converted characters
            List<Object>.Enumerator eobjects = objects.GetEnumerator();
            while (eobjects.MoveNext())
            {
                eobjects.Current.Save(command);
            }

            // close connection to database
            connection.Close();

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static void TopLevelErrorHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("Error Occured : " + e.ToString());
        }

        #region UpdatePlayerFields
        static void UpdatePlayerFields(Object srcobj, Object dstobj)
        {
            dstobj.SetUInt64Value(UpdateFieldsNew.OBJECT_FIELD_GUID,          srcobj.GetUInt64Value(UpdateFieldsOld.OBJECT_FIELD_GUID));
            dstobj.SetUInt32Value(UpdateFieldsNew.OBJECT_FIELD_TYPE,          srcobj.GetUInt32Value(UpdateFieldsOld.OBJECT_FIELD_TYPE));
            dstobj.SetUInt32Value(UpdateFieldsNew.OBJECT_FIELD_ENTRY,         srcobj.GetUInt32Value(UpdateFieldsOld.OBJECT_FIELD_ENTRY));
            dstobj.SetFloatValue(UpdateFieldsNew.OBJECT_FIELD_SCALE_X,        srcobj.GetFloatValue(UpdateFieldsOld.OBJECT_FIELD_SCALE_X));
            dstobj.SetUInt32Value(UpdateFieldsNew.OBJECT_FIELD_PADDING,       srcobj.GetUInt32Value(UpdateFieldsOld.OBJECT_FIELD_PADDING));
            dstobj.SetUInt64Value(UpdateFieldsNew.UNIT_FIELD_CHARM,           srcobj.GetUInt64Value(UpdateFieldsOld.UNIT_FIELD_CHARM));
            dstobj.SetUInt64Value(UpdateFieldsNew.UNIT_FIELD_SUMMON,          srcobj.GetUInt64Value(UpdateFieldsOld.UNIT_FIELD_SUMMON));
            dstobj.SetUInt64Value(UpdateFieldsNew.UNIT_FIELD_CHARMEDBY,       srcobj.GetUInt64Value(UpdateFieldsOld.UNIT_FIELD_CHARMEDBY));
            dstobj.SetUInt64Value(UpdateFieldsNew.UNIT_FIELD_SUMMONEDBY,      srcobj.GetUInt64Value(UpdateFieldsOld.UNIT_FIELD_SUMMONEDBY));
            dstobj.SetUInt64Value(UpdateFieldsNew.UNIT_FIELD_CREATEDBY,       srcobj.GetUInt64Value(UpdateFieldsOld.UNIT_FIELD_CREATEDBY));
            dstobj.SetUInt64Value(UpdateFieldsNew.UNIT_FIELD_TARGET,          srcobj.GetUInt64Value(UpdateFieldsOld.UNIT_FIELD_TARGET));
            dstobj.SetUInt64Value(UpdateFieldsNew.UNIT_FIELD_PERSUADED,       srcobj.GetUInt64Value(UpdateFieldsOld.UNIT_FIELD_PERSUADED));
            dstobj.SetUInt64Value(UpdateFieldsNew.UNIT_FIELD_CHANNEL_OBJECT,  srcobj.GetUInt64Value(UpdateFieldsOld.UNIT_FIELD_CHANNEL_OBJECT));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_HEALTH,          srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_HEALTH));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER1,          srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER2,          srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER3,          srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER3));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER4,          srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER4));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER5,          srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER5));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_MAXHEALTH,       srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_MAXHEALTH));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_MAXPOWER1,       srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_MAXPOWER1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_MAXPOWER2,       srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_MAXPOWER2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_MAXPOWER3,       srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_MAXPOWER3));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_MAXPOWER4,       srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_MAXPOWER4));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_MAXPOWER5,       srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_MAXPOWER5));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_LEVEL,            srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_LEVEL));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_FACTIONTEMPLATE,  srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_FACTIONTEMPLATE));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_BYTES_0,          srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_BYTES_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY_1, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY_2, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_VIRTUAL_ITEM_INFO,       srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_VIRTUAL_ITEM_INFO));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_VIRTUAL_ITEM_INFO_1,     srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_VIRTUAL_ITEM_INFO_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_VIRTUAL_ITEM_INFO_2,     srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_VIRTUAL_ITEM_INFO_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_VIRTUAL_ITEM_INFO_3,     srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_VIRTUAL_ITEM_INFO_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_VIRTUAL_ITEM_INFO_4,     srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_VIRTUAL_ITEM_INFO_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_VIRTUAL_ITEM_INFO_5,     srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_VIRTUAL_ITEM_INFO_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_FLAGS,             srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_FLAGS));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_FLAGS_2,           srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_FLAGS_2));

            //need cleanup Auras         
            for (ushort  i = (ushort)UpdateFieldsNew.UNIT_FIELD_AURA; i < (ushort)UpdateFieldsNew.UNIT_FIELD_BASEATTACKTIME; i++)
                dstobj.SetUInt32Value(i,0);


            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_BASEATTACKTIME,      srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_BASEATTACKTIME));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_BASEATTACKTIME + 1,  srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_BASEATTACKTIME + 1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RANGEDATTACKTIME,    srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RANGEDATTACKTIME));

            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_BOUNDINGRADIUS, srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_BOUNDINGRADIUS));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_COMBATREACH, srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_COMBATREACH));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_DISPLAYID, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_DISPLAYID));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_NATIVEDISPLAYID, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_NATIVEDISPLAYID));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_MOUNTDISPLAYID, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_MOUNTDISPLAYID));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_MINDAMAGE,         srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_MINDAMAGE));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_MAXDAMAGE,         srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_MAXDAMAGE));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_MINOFFHANDDAMAGE,  srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_MINOFFHANDDAMAGE));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_MAXOFFHANDDAMAGE,  srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_MAXOFFHANDDAMAGE));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_BYTES_1,            srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_BYTES_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_PETNUMBER,          srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_PETNUMBER));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_PET_NAME_TIMESTAMP, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_PET_NAME_TIMESTAMP));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_PETEXPERIENCE,   srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_PETEXPERIENCE));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_PETNEXTLEVELEXP, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_PETNEXTLEVELEXP));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_DYNAMIC_FLAGS,         srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_DYNAMIC_FLAGS));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_CHANNEL_SPELL,         srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_CHANNEL_SPELL));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_MOD_CAST_SPEED,         srcobj.GetFloatValue(UpdateFieldsOld.UNIT_MOD_CAST_SPEED));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_CREATED_BY_SPELL,      srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_CREATED_BY_SPELL));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_NPC_FLAGS,             srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_NPC_FLAGS));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_NPC_EMOTESTATE,        srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_NPC_EMOTESTATE));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_TRAINING_POINTS,       srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_TRAINING_POINTS));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_STAT0,           srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_STAT0));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_STAT1,           srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_STAT1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_STAT2,           srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_STAT2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_STAT3,           srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_STAT3));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_STAT4,           srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_STAT4));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POSSTAT0, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POSSTAT0));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POSSTAT1, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POSSTAT1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POSSTAT2, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POSSTAT2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POSSTAT3, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POSSTAT3));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POSSTAT4, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POSSTAT4));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_NEGSTAT0, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_NEGSTAT0));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_NEGSTAT1, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_NEGSTAT1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_NEGSTAT2, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_NEGSTAT2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_NEGSTAT3, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_NEGSTAT3));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_NEGSTAT4, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_NEGSTAT4));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCES, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCES));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCES_1, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCES_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCES_2, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCES_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCES_3, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCES_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCES_4, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCES_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCES_5, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCES_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCES_6, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCES_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_1, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_2, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_3, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_4, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_5, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_6, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSPOSITIVE_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE,   srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_1, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_2, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_3, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_4, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_5, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_6, srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RESISTANCEBUFFMODSNEGATIVE_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_BASE_MANA,                     srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_BASE_MANA));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_BASE_HEALTH,                   srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_BASE_HEALTH));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_BYTES_2,                       srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_BYTES_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_ATTACK_POWER,                  srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_ATTACK_POWER));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_ATTACK_POWER_MODS,             srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_ATTACK_POWER_MODS));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_ATTACK_POWER_MULTIPLIER,        srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_ATTACK_POWER_MULTIPLIER));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RANGED_ATTACK_POWER,           srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RANGED_ATTACK_POWER));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_RANGED_ATTACK_POWER_MODS,      srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_RANGED_ATTACK_POWER_MODS));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER, srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_RANGED_ATTACK_POWER_MULTIPLIER));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_MINRANGEDDAMAGE,                srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_MINRANGEDDAMAGE));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_MAXRANGEDDAMAGE,                srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_MAXRANGEDDAMAGE));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MODIFIER,           srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MODIFIER));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MODIFIER_1,         srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MODIFIER_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MODIFIER_2,         srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MODIFIER_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MODIFIER_3,         srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MODIFIER_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MODIFIER_4,         srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MODIFIER_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MODIFIER_5,         srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MODIFIER_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MODIFIER_6,         srcobj.GetUInt32Value(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MODIFIER_6));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MULTIPLIER,          srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MULTIPLIER));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MULTIPLIER_1,        srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MULTIPLIER_1));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MULTIPLIER_2,        srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MULTIPLIER_2));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MULTIPLIER_3,        srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MULTIPLIER_3));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MULTIPLIER_4,        srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MULTIPLIER_4));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MULTIPLIER_5,        srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MULTIPLIER_5));
            dstobj.SetFloatValue(UpdateFieldsNew.UNIT_FIELD_POWER_COST_MULTIPLIER_6,        srcobj.GetFloatValue(UpdateFieldsOld.UNIT_FIELD_POWER_COST_MULTIPLIER_6));

            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_DUEL_ARBITER                       ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_DUEL_ARBITER));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FLAGS                              ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FLAGS));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_GUILDID                            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_GUILDID));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_GUILDRANK                          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_GUILDRANK));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_BYTES                              ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_BYTES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_BYTES_2                            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_BYTES_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_BYTES_3                            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_BYTES_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_DUEL_TEAM                          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_DUEL_TEAM));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_GUILD_TIMESTAMP                    ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_GUILD_TIMESTAMP));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_1_1                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_1_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_1_2                      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_1_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_2_1                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_2_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_2_2                      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_2_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_3_1                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_3_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_3_2                      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_3_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_4_1                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_4_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_4_2                      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_4_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_5_1                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_5_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_5_2                      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_5_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_6_1                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_6_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_6_2                      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_6_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_7_1                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_7_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_7_2                      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_7_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_8_1                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_8_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_8_2                      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_8_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_9_1                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_9_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_9_2                      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_9_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_10_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_10_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_10_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_10_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_11_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_11_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_11_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_11_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_12_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_12_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_12_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_12_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_13_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_13_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_13_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_13_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_14_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_14_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_14_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_14_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_15_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_15_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_15_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_15_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_16_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_16_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_16_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_16_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_17_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_17_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_17_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_17_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_18_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_18_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_18_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_18_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_19_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_19_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_19_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_19_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_20_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_20_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_20_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_20_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_21_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_21_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_21_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_21_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_22_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_22_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_22_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_22_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_23_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_23_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_23_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_23_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_24_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_24_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_24_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_24_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_QUEST_LOG_25_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_QUEST_LOG_25_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_QUEST_LOG_25_2                     ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_QUEST_LOG_25_2));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_CREATOR             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_1                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_2                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_3                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_4                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_5                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_6                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_7                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_8                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_9                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_10                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_0_11                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_PROPERTIES          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_1_PAD                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_1_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_CREATOR             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_1                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_2                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_3                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_4                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_5                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_6                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_7                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_8                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_9                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_10                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_0_11                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_PROPERTIES          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_2_PAD                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_2_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_CREATOR             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_1                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_2                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_3                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_4                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_5                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_6                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_7                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_8                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_9                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_10                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_0_11                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_PROPERTIES          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_3_PAD                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_3_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_CREATOR             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_1                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_2                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_3                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_4                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_5                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_6                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_7                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_8                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_9                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_10                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_0_11                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_PROPERTIES          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_4_PAD                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_4_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_CREATOR             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_1                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_2                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_3                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_4                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_5                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_6                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_7                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_8                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_9                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_10                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_0_11                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_PROPERTIES          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_5_PAD                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_5_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_CREATOR             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_1                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_2                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_3                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_4                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_5                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_6                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_7                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_8                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_9                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_10                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_0_11                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_PROPERTIES          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_6_PAD                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_6_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_CREATOR             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_1                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_2                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_3                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_4                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_5                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_6                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_7                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_8                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_9                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_10                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_0_11                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_PROPERTIES          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_7_PAD                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_7_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_CREATOR             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_1                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_2                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_3                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_4                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_5                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_6                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_7                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_8                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_9                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_10                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_0_11                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_PROPERTIES          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_8_PAD                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_8_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_CREATOR             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_1                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_2                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_3                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_4                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_5                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_6                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_7                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_8                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_9                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_10                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_0_11                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_PROPERTIES          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_9_PAD                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_9_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_CREATOR            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_1                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_2                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_3                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_4                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_5                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_6                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_7                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_8                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_9                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_10               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_0_11               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_PROPERTIES         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_10_PAD                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_10_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_CREATOR            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_1                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_2                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_3                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_4                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_5                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_6                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_7                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_8                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_9                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_10               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_0_11               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_PROPERTIES         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_11_PAD                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_11_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_CREATOR            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_1                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_2                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_3                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_4                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_5                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_6                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_7                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_8                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_9                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_10               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_0_11               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_PROPERTIES         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_12_PAD                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_12_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_CREATOR            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_1                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_2                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_3                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_4                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_5                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_6                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_7                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_8                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_9                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_10               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_0_11               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_PROPERTIES         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_13_PAD                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_13_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_CREATOR            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_1                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_2                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_3                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_4                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_5                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_6                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_7                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_8                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_9                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_10               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_0_11               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_PROPERTIES         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_14_PAD                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_14_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_CREATOR            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_1                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_2                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_3                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_4                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_5                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_6                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_7                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_8                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_9                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_10               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_0_11               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_PROPERTIES         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_15_PAD                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_15_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_CREATOR            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_1                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_2                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_3                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_4                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_5                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_6                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_7                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_8                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_9                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_10               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_0_11               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_PROPERTIES         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_16_PAD                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_16_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_CREATOR            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_1                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_2                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_3                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_4                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_5                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_6                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_7                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_8                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_9                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_10               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_0_11               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_PROPERTIES         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_17_PAD                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_17_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_CREATOR            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_1                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_2                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_3                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_4                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_5                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_6                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_7                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_8                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_9                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_10               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_0_11               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_PROPERTIES         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_18_PAD                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_18_PAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_CREATOR            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_CREATOR));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_1                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_2                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_3                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_4                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_5                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_6                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_7                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_8                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_9                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_10               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_0_11               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_0_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_PROPERTIES         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_PROPERTIES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_VISIBLE_ITEM_19_PAD                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_VISIBLE_ITEM_19_PAD));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_CHOSEN_TITLE                       ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_CHOSEN_TITLE));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_1              ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_2              ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_2));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_3              ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_3));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_4              ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_4));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_5              ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_5));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_6              ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_6));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_7              ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_7));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_8              ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_8));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_9              ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_9));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_10             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_10));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_11             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_11));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_12             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_12));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_13             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_13));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_14             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_14));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_15             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_15));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_16             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_16));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_17             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_17));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_18             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_18));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_19             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_19));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_20             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_20));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_21             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_21));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_22             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_22));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_23             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_23));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_24             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_24));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_25             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_25));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_26             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_26));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_27             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_27));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_28             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_28));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_29             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_29));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_30             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_30));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_31             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_31));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_32             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_32));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_33             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_33));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_34             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_34));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_35             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_35));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_36             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_36));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_37             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_37));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_38             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_38));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_39             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_39));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_40             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_40));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_41             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_41));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_42             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_42));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_43             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_43));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_44             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_44));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_INV_SLOT_HEAD_45             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_INV_SLOT_HEAD_45));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1                  ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_1                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_2                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_2));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_3                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_3));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_4                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_4));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_5                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_5));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_6                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_6));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_7                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_7));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_8                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_8));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_9                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_9));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_10               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_10));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_11               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_11));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_12               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_12));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_13               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_13));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_14               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_14));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_15               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_15));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_16               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_16));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_17               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_17));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_18               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_18));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_19               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_19));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_20               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_20));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_21               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_21));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_22               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_22));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_23               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_23));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_24               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_24));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_25               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_25));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_26               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_26));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_27               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_27));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_28               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_28));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_29               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_29));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_30               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_30));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_PACK_SLOT_1_31               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_PACK_SLOT_1_31));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1                  ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_1                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_2                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_2));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_3                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_3));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_4                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_4));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_5                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_5));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_6                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_6));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_7                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_7));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_8                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_8));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_9                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_9));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_10               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_10));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_11               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_11));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_12               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_12));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_13               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_13));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_14               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_14));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_15               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_15));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_16               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_16));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_17               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_17));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_18               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_18));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_19               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_19));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_20               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_20));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_21               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_21));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_22               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_22));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_23               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_23));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_24               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_24));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_25               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_25));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_26               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_26));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_27               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_27));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_28               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_28));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_29               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_29));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_30               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_30));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_31               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_31));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_32               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_32));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_33               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_33));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_34               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_34));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_35               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_35));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_36               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_36));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_37               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_37));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_38               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_38));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_39               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_39));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_40               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_40));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_41               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_41));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_42               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_42));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_43               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_43));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_44               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_44));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_45               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_45));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_46               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_46));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_47               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_47));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_48               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_48));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_49               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_49));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_50               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_50));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_51               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_51));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_52               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_52));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_53               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_53));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_54               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_54));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANK_SLOT_1_55               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANK_SLOT_1_55));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_1             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_2             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_2));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_3             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_3));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_4             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_4));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_5             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_5));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_6             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_6));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_7             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_7));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_8             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_8));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_9             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_9));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_10            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_10));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_11            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_11));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_12            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_12));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_BANKBAG_SLOT_1_13            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_BANKBAG_SLOT_1_13));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1         ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_1       ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_2       ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_2));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_3       ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_3));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_4       ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_4));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_5       ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_5));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_6       ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_6));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_7       ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_7));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_8       ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_8));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_9       ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_9));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_10      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_10));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_11      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_11));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_12      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_12));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_13      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_13));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_14      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_14));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_15      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_15));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_16      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_16));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_17      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_17));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_18      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_18));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_19      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_19));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_20      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_20));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_21      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_21));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_22      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_22));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_23      ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_VENDORBUYBACK_SLOT_1_23));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1               ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_1             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_1));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_2             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_2));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_3             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_3));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_4             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_4));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_5             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_5));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_6             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_6));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_7             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_7));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_8             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_8));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_9             ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_9));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_10            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_10));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_11            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_11));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_12            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_12));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_13            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_13));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_14            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_14));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_15            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_15));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_16            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_16));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_17            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_17));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_18            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_18));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_19            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_19));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_20            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_20));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_21            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_21));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_22            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_22));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_23            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_23));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_24            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_24));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_25            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_25));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_26            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_26));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_27            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_27));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_28            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_28));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_29            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_29));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_30            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_30));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_31            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_31));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_32            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_32));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_33            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_33));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_34            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_34));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_35            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_35));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_36            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_36));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_37            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_37));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_38            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_38));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_39            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_39));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_40            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_40));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_41            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_41));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_42            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_42));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_43            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_43));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_44            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_44));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_45            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_45));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_46            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_46));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_47            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_47));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_48            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_48));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_49            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_49));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_50            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_50));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_51            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_51));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_52            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_52));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_53            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_53));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_54            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_54));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_55            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_55));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_56            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_56));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_57            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_57));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_58            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_58));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_59            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_59));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_60            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_60));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_61            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_61));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_62            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_62));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FIELD_KEYRING_SLOT_1_63            ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FIELD_KEYRING_SLOT_1_63));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER_FARSIGHT                           ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER_FARSIGHT));
            dstobj.SetUInt64Value(UpdateFieldsNew.PLAYER__FIELD_KNOWN_TITLES                ,srcobj.GetUInt64Value(UpdateFieldsOld.PLAYER__FIELD_KNOWN_TITLES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_XP                                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_XP));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_NEXT_LEVEL_XP                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_NEXT_LEVEL_XP));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_1                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_2                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_3                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_4                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_5                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_6                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_7                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_8                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_9                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_10                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_11                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_12                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_12));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_13                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_13));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_14                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_14));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_15                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_15));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_16                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_16));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_17                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_17));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_18                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_18));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_19                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_19));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_20                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_20));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_21                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_21));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_22                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_22));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_23                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_23));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_24                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_24));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_25                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_25));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_26                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_26));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_27                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_27));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_28                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_28));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_29                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_29));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_30                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_30));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_31                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_31));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_32                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_32));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_33                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_33));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_34                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_34));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_35                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_35));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_36                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_36));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_37                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_37));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_38                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_38));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_39                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_39));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_40                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_40));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_41                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_41));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_42                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_42));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_43                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_43));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_44                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_44));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_45                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_45));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_46                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_46));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_47                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_47));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_48                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_48));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_49                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_49));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_50                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_50));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_51                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_51));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_52                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_52));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_53                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_53));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_54                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_54));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_55                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_55));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_56                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_56));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_57                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_57));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_58                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_58));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_59                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_59));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_60                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_60));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_61                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_61));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_62                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_62));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_63                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_63));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_64                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_64));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_65                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_65));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_66                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_66));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_67                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_67));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_68                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_68));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_69                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_69));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_70                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_70));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_71                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_71));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_72                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_72));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_73                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_73));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_74                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_74));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_75                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_75));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_76                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_76));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_77                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_77));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_78                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_78));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_79                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_79));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_80                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_80));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_81                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_81));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_82                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_82));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_83                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_83));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_84                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_84));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_85                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_85));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_86                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_86));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_87                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_87));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_88                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_88));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_89                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_89));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_90                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_90));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_91                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_91));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_92                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_92));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_93                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_93));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_94                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_94));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_95                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_95));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_96                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_96));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_97                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_97));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_98                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_98));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_99                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_99));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_100                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_100));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_101                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_101));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_102                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_102));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_103                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_103));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_104                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_104));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_105                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_105));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_106                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_106));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_107                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_107));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_108                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_108));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_109                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_109));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_110                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_110));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_111                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_111));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_112                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_112));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_113                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_113));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_114                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_114));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_115                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_115));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_116                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_116));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_117                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_117));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_118                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_118));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_119                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_119));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_120                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_120));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_121                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_121));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_122                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_122));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_123                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_123));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_124                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_124));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_125                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_125));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_126                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_126));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_127                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_127));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_128                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_128));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_129                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_129));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_130                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_130));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_131                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_131));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_132                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_132));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_133                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_133));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_134                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_134));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_135                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_135));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_136                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_136));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_137                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_137));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_138                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_138));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_139                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_139));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_140                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_140));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_141                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_141));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_142                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_142));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_143                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_143));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_144                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_144));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_145                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_145));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_146                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_146));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_147                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_147));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_148                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_148));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_149                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_149));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_150                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_150));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_151                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_151));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_152                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_152));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_153                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_153));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_154                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_154));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_155                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_155));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_156                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_156));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_157                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_157));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_158                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_158));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_159                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_159));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_160                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_160));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_161                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_161));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_162                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_162));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_163                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_163));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_164                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_164));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_165                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_165));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_166                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_166));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_167                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_167));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_168                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_168));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_169                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_169));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_170                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_170));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_171                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_171));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_172                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_172));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_173                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_173));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_174                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_174));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_175                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_175));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_176                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_176));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_177                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_177));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_178                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_178));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_179                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_179));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_180                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_180));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_181                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_181));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_182                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_182));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_183                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_183));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_184                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_184));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_185                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_185));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_186                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_186));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_187                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_187));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_188                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_188));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_189                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_189));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_190                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_190));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_191                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_191));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_192                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_192));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_193                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_193));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_194                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_194));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_195                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_195));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_196                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_196));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_197                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_197));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_198                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_198));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_199                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_199));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_200                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_200));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_201                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_201));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_202                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_202));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_203                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_203));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_204                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_204));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_205                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_205));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_206                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_206));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_207                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_207));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_208                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_208));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_209                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_209));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_210                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_210));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_211                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_211));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_212                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_212));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_213                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_213));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_214                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_214));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_215                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_215));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_216                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_216));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_217                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_217));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_218                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_218));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_219                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_219));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_220                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_220));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_221                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_221));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_222                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_222));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_223                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_223));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_224                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_224));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_225                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_225));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_226                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_226));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_227                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_227));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_228                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_228));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_229                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_229));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_230                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_230));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_231                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_231));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_232                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_232));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_233                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_233));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_234                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_234));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_235                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_235));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_236                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_236));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_237                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_237));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_238                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_238));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_239                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_239));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_240                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_240));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_241                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_241));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_242                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_242));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_243                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_243));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_244                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_244));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_245                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_245));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_246                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_246));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_247                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_247));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_248                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_248));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_249                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_249));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_250                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_250));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_251                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_251));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_252                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_252));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_253                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_253));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_254                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_254));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_255                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_255));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_256                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_256));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_257                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_257));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_258                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_258));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_259                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_259));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_260                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_260));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_261                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_261));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_262                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_262));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_263                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_263));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_264                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_264));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_265                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_265));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_266                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_266));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_267                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_267));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_268                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_268));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_269                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_269));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_270                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_270));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_271                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_271));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_272                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_272));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_273                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_273));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_274                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_274));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_275                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_275));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_276                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_276));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_277                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_277));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_278                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_278));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_279                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_279));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_280                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_280));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_281                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_281));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_282                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_282));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_283                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_283));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_284                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_284));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_285                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_285));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_286                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_286));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_287                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_287));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_288                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_288));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_289                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_289));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_290                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_290));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_291                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_291));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_292                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_292));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_293                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_293));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_294                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_294));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_295                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_295));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_296                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_296));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_297                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_297));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_298                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_298));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_299                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_299));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_300                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_300));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_301                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_301));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_302                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_302));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_303                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_303));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_304                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_304));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_305                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_305));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_306                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_306));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_307                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_307));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_308                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_308));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_309                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_309));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_310                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_310));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_311                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_311));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_312                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_312));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_313                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_313));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_314                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_314));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_315                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_315));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_316                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_316));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_317                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_317));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_318                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_318));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_319                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_319));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_320                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_320));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_321                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_321));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_322                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_322));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_323                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_323));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_324                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_324));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_325                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_325));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_326                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_326));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_327                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_327));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_328                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_328));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_329                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_329));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_330                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_330));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_331                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_331));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_332                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_332));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_333                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_333));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_334                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_334));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_335                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_335));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_336                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_336));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_337                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_337));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_338                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_338));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_339                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_339));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_340                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_340));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_341                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_341));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_342                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_342));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_343                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_343));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_344                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_344));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_345                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_345));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_346                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_346));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_347                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_347));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_348                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_348));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_349                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_349));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_350                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_350));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_351                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_351));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_352                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_352));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_353                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_353));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_354                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_354));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_355                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_355));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_356                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_356));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_357                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_357));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_358                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_358));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_359                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_359));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_360                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_360));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_361                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_361));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_362                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_362));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_363                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_363));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_364                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_364));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_365                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_365));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_366                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_366));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_367                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_367));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_368                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_368));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_369                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_369));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_370                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_370));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_371                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_371));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_372                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_372));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_373                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_373));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_374                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_374));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_375                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_375));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_376                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_376));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_377                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_377));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_378                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_378));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_379                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_379));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_380                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_380));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_381                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_381));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_382                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_382));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SKILL_INFO_1_1_383                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SKILL_INFO_1_1_383));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_CHARACTER_POINTS1                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_CHARACTER_POINTS1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_CHARACTER_POINTS2                  ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_CHARACTER_POINTS2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_TRACK_CREATURES                    ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_TRACK_CREATURES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_TRACK_RESOURCES                    ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_TRACK_RESOURCES));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_BLOCK_PERCENTAGE                   ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_BLOCK_PERCENTAGE));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_DODGE_PERCENTAGE                   ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_DODGE_PERCENTAGE));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_PARRY_PERCENTAGE                   ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_PARRY_PERCENTAGE));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_EXPERTISE                          ,0);            
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_CRIT_PERCENTAGE                    ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_CRIT_PERCENTAGE));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_RANGED_CRIT_PERCENTAGE             ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_RANGED_CRIT_PERCENTAGE));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_OFFHAND_CRIT_PERCENTAGE            ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_OFFHAND_CRIT_PERCENTAGE));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_SPELL_CRIT_PERCENTAGE1             ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_SPELL_CRIT_PERCENTAGE1));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_SPELL_CRIT_PERCENTAGE1_1           ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_SPELL_CRIT_PERCENTAGE1_1));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_SPELL_CRIT_PERCENTAGE1_2           ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_SPELL_CRIT_PERCENTAGE1_2));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_SPELL_CRIT_PERCENTAGE1_3           ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_SPELL_CRIT_PERCENTAGE1_3));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_SPELL_CRIT_PERCENTAGE1_4           ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_SPELL_CRIT_PERCENTAGE1_4));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_SPELL_CRIT_PERCENTAGE1_5           ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_SPELL_CRIT_PERCENTAGE1_5));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_SPELL_CRIT_PERCENTAGE1_6           ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_SPELL_CRIT_PERCENTAGE1_6));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_SHIELD_BLOCK                       ,0);

            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_1                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_2                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_3                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_4                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_5                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_6                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_7                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_8                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_9                 ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_10                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_11                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_12                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_12));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_13                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_13));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_14                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_14));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_15                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_15));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_16                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_16));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_17                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_17));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_18                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_18));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_19                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_19));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_20                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_20));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_21                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_21));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_22                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_22));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_23                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_23));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_24                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_24));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_25                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_25));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_26                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_26));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_27                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_27));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_28                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_28));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_29                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_29));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_30                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_30));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_31                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_31));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_32                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_32));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_33                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_33));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_34                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_34));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_35                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_35));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_36                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_36));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_37                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_37));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_38                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_38));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_39                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_39));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_40                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_40));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_41                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_41));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_42                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_42));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_43                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_43));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_44                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_44));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_45                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_45));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_46                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_46));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_47                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_47));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_48                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_48));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_49                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_49));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_50                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_50));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_51                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_51));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_52                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_52));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_53                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_53));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_54                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_54));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_55                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_55));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_56                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_56));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_57                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_57));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_58                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_58));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_59                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_59));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_60                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_60));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_61                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_61));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_62                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_62));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_EXPLORED_ZONES_1_63                ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_EXPLORED_ZONES_1_63));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_REST_STATE_EXPERIENCE              ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_REST_STATE_EXPERIENCE));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COINAGE                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COINAGE));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_POS          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_POS));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_1        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_2        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_3        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_4        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_5        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_6        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_POS_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_1        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_2        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_3        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_4        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_5        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_6        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_NEG_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_1        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_2        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_3        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_4        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_5        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_6        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_DAMAGE_DONE_PCT_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_HEALING_DONE_POS         ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_HEALING_DONE_POS));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MOD_TARGET_RESISTANCE        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MOD_TARGET_RESISTANCE));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BYTES                        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BYTES));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_AMMO_ID                            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_AMMO_ID));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_SELF_RES_SPELL                     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_SELF_RES_SPELL));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_PVP_MEDALS                   ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_PVP_MEDALS));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1              ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_1            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_2            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_3            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_4            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_5            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_6            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_7            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_8            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_9            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_10           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_PRICE_1_11           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_PRICE_1_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1          ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_1        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_2        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_3        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_4        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_5        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_6        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_7        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_8        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_9        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_10       ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_11       ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BUYBACK_TIMESTAMP_1_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_KILLS                        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_KILLS));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_TODAY_CONTRIBUTION           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_TODAY_CONTRIBUTION));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_YESTERDAY_CONTRIBUTION       ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_YESTERDAY_CONTRIBUTION));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_LIFETIME_HONORBALE_KILLS     ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_LIFETIME_HONORBALE_KILLS));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_BYTES2                       ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_BYTES2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_WATCHED_FACTION_INDEX        ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_WATCHED_FACTION_INDEX));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1              ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_1            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_2            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_3            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_4            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_5            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_6            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_7            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_8            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_9            ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_10           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_10));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_11           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_11));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_12           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_12));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_13           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_13));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_14           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_14));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_15           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_15));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_16           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_16));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_17           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_17));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_18           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_18));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_19           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_19));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_20           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_20));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_21           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_21));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_22           ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_COMBAT_RATING_1_22));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_COMBAT_RATING_1_23           ,0);
                                                                

            //need cleanup Arena Team Info
            for (ushort  i = (ushort)UpdateFieldsNew.PLAYER_FIELD_ARENA_TEAM_INFO_1_1; i < (ushort)UpdateFieldsNew.PLAYER_FIELD_ARENA_TEAM_INFO_1_1_17; i++)
                dstobj.SetUInt32Value(i,0);

            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_HONOR_CURRENCY               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_HONOR_CURRENCY));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_ARENA_CURRENCY               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_ARENA_CURRENCY));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_FIELD_MOD_MANA_REGEN                ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_FIELD_MOD_MANA_REGEN));
            dstobj.SetFloatValue(UpdateFieldsNew.PLAYER_FIELD_MOD_MANA_REGEN_INTERRUPT      ,srcobj.GetFloatValue(UpdateFieldsOld.PLAYER_FIELD_MOD_MANA_REGEN_INTERRUPT));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_MAX_LEVEL                    ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_MAX_LEVEL));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_DAILY_QUESTS_1               ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_DAILY_QUESTS_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_DAILY_QUESTS_1_1             ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_DAILY_QUESTS_1_1));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_DAILY_QUESTS_1_2             ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_DAILY_QUESTS_1_2));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_DAILY_QUESTS_1_3             ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_DAILY_QUESTS_1_3));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_DAILY_QUESTS_1_4             ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_DAILY_QUESTS_1_4));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_DAILY_QUESTS_1_5             ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_DAILY_QUESTS_1_5));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_DAILY_QUESTS_1_6             ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_DAILY_QUESTS_1_6));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_DAILY_QUESTS_1_7             ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_DAILY_QUESTS_1_7));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_DAILY_QUESTS_1_8             ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_DAILY_QUESTS_1_8));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_DAILY_QUESTS_1_9             ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_DAILY_QUESTS_1_9));
            dstobj.SetUInt32Value(UpdateFieldsNew.PLAYER_FIELD_PADDING                      ,srcobj.GetUInt32Value(UpdateFieldsOld.PLAYER_FIELD_PADDING));
        }
        #endregion
    }

    class Object
    {
        ushort m_valuesCount;
        uint[] m_uint32Values;

        public Object(ushort valuescount)
        {
            m_valuesCount = valuescount;

            m_uint32Values = new uint[m_valuesCount];
        }

        ~Object()
        {

        }

        public void LoadValues(string data)
        {
            string[] values = data.Split(' ');

            for (ushort i = 0; i < m_valuesCount; ++i)
                m_uint32Values[i] = Convert.ToUInt32(values[i]);
        }

        public void Save(MySqlCommand cmd)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE `character` SET `data`='");

            for (ushort i = 0; i < m_valuesCount; i++)
            {
                sb.AppendFormat("{0} ", GetUInt32Value(i));
            }

            sb.Append("' WHERE `guid`='" + GetGUIDLow() + "'");

            cmd.CommandText = sb.ToString();

            int affected = cmd.ExecuteNonQuery();

            if (affected == 0)
                Console.WriteLine("Query failed for player {0}", GetGUIDLow());
            else
                Console.WriteLine("Player {0} saved", GetGUIDLow());
        }

        public uint GetGUIDLow()
        {
            return m_uint32Values[(int)UpdateFieldsNew.OBJECT_FIELD_GUID];
        }

        public uint GetGUIDHigh()
        {
            return m_uint32Values[(int)UpdateFieldsNew.OBJECT_FIELD_GUID + 1];
        }

        public uint GetUInt32Value(ushort index)
        {
            return m_uint32Values[index];
        }

        public ulong GetUInt64Value(ushort index)
        {
            uint low =  m_uint32Values[index];
            uint high = m_uint32Values[index + 1];
            return (ulong)(low | (high << 32));
        }

        public float GetFloatValue(ushort index)
        {
            byte[] temp = BitConverter.GetBytes(m_uint32Values[index]);
            return BitConverter.ToSingle(temp, 0);
        }

        public uint GetUInt32Value(UpdateFieldsOld index)
        {
            return m_uint32Values[(int)index];
        }

        public ulong GetUInt64Value(UpdateFieldsOld index)
        {
            uint low = m_uint32Values[(int)index];
            uint high = m_uint32Values[(int)index + 1];
            return (ulong)(low | (high << 32));
        }

        public float GetFloatValue(UpdateFieldsOld index)
        {
            byte[] temp = BitConverter.GetBytes(m_uint32Values[(int)index]);
            return BitConverter.ToSingle(temp, 0);
        }

        public void SetUInt32Value(ushort index, uint value)
        {
            m_uint32Values[index] = value;
        }

        public void SetUInt64Value(ushort index, ulong value)
        {
            m_uint32Values[index] = (uint)(value & 0xFFFFFFFF);
            m_uint32Values[index + 1] = (uint)(value >> 32);
        }

        public void SetFloatValue(ushort index, float value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            m_uint32Values[index] = BitConverter.ToUInt32(temp, 0);
        }

        public void SetUInt32Value(UpdateFieldsNew index, uint value)
        {
            m_uint32Values[(int)index] = value;
        }

        public void SetUInt64Value(UpdateFieldsNew index, ulong value)
        {
            m_uint32Values[(int)index] = (uint)(value & 0xFFFFFFFF);
            m_uint32Values[(int)index + 1] = (uint)(value >> 32);
        }

        public void SetFloatValue(UpdateFieldsNew index, float value)
        {
            byte[] temp = BitConverter.GetBytes(value);
            m_uint32Values[(int)index] = BitConverter.ToUInt32(temp, 0);
        }
    }
}
