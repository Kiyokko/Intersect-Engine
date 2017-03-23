﻿using System;
using System.IO;
using System.Xml;
using Intersect;
using MapZones = Intersect_Migration_Tool.UpgradeInstructions.Upgrade_7.Intersect_Convert_Lib.MapZones;
using Options = Intersect_Migration_Tool.UpgradeInstructions.Upgrade_7.Intersect_Convert_Lib.Options;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_7
{
    public static class ServerOptions
    {
        //Config XML String
        public static string ConfigXml = "";
        private static bool ConfigFailed = false;

        //Misc
        public static int ItemDespawnTime = 15000; //15 seconds
        public static int ItemRespawnTime = 15000; //15 seconds

        //Combat
        public static int RegenTime = 3000; //3 seconds

        //Options File
        public static bool LoadOptions()
        {
            if (!Directory.Exists("resources")) Directory.CreateDirectory("resources");
            if (!File.Exists("resources/config.xml"))
            {
                Console.WriteLine("Configuration file was missing!");
                return false;
            }
            else
            {
                var options = new XmlDocument();
                ConfigXml = File.ReadAllText("resources/config.xml");
                try
                {
                    options.LoadXml(ConfigXml);

                    //General Options
                    Options.Language = GetXmlStr(options, "//Config/Language", false);
                    if (Options.Language == "") Options.Language = "English";
                    Options.GameName = GetXmlStr(options, "//Config/GameName", false);
                    Options.ServerPort = GetXmlInt(options, "//Config/ServerPort");

                    //Player Options
                    Options.MaxStatValue = GetXmlInt(options, "//Config/Player/MaxStat");
                    Options.MaxLevel = GetXmlInt(options, "//Config/Player/MaxLevel");
                    Options.MaxInvItems = GetXmlInt(options, "//Config/Player/MaxInventory");
                    Options.MaxPlayerSkills = GetXmlInt(options, "//Config/Player/MaxSpells");
                    Options.MaxBankSlots = GetXmlInt(options, "//Config/Player/MaxBank");

                    //Equipment
                    int slot = 0;
                    while (!string.IsNullOrEmpty(GetXmlStr(options, "//Config/Equipment/Slot" + slot, false)))
                    {
                        if (
                            Options.EquipmentSlots.IndexOf(GetXmlStr(options, "//Config/Equipment/Slot" + slot, false)) >
                            -1)
                        {
                            Console.WriteLine(
                                "Tried to add the same piece of equipment twice, this is not permitted.  (Path: " +
                                "//Config/Equipment/Slot" + slot + ")");
                            return false;
                        }
                        else
                        {
                            Options.EquipmentSlots.Add(GetXmlStr(options, "//Config/Equipment/Slot" + slot, false));
                        }
                        slot++;
                    }
                    Options.WeaponIndex = GetXmlInt(options, "//Config/Equipment/WeaponSlot");
                    if (Options.WeaponIndex < -1 || Options.WeaponIndex > Options.EquipmentSlots.Count - 1)
                    {
                        Console.WriteLine(
                            "Weapon Slot is out of bounds! Make sure the slot exists and you are counting starting from zero! Use -1 if you do not wish to have equipable weapons in-game!  (Path: " +
                            "//Config/Equipment/WeaponSlot)");
                    }
                    Options.ShieldIndex = GetXmlInt(options, "//Config/Equipment/ShieldSlot");
                    if (Options.ShieldIndex < -1 || Options.ShieldIndex > Options.EquipmentSlots.Count - 1)
                    {
                        Console.WriteLine(
                            "Shield Slot is out of bounds! Make sure the slot exists and you are counting starting from zero! Use -1 if you do not wish to have equipable shields in-game!  (Path: " +
                            "//Config/Equipment/ShieldSlot)");
                    }

                    //Paperdoll
                    slot = 0;
                    while (!string.IsNullOrEmpty(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot, false)))
                    {
                        if (
                            Options.EquipmentSlots.IndexOf(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot, false)) >
                            -1)
                        {
                            if (
                                Options.PaperdollOrder.IndexOf(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot,
                                    false)) > -1)
                            {
                                Console.WriteLine(
                                    "Tried to add the same piece of equipment to the paperdoll render order twice, this is not permitted.  (Path: " +
                                    "//Config/Paperdoll/Slot" + slot + ")");
                                return false;
                            }
                            else
                            {
                                Options.PaperdollOrder.Add(GetXmlStr(options, "//Config/Paperdoll/Slot" + slot, false));
                            }
                        }
                        else
                        {
                            Console.WriteLine(
                                "Tried to add a paperdoll for a piece of equipment that does not exist!  (Path: " +
                                "//Config/Paperdoll/Slot" + slot + ")");
                            return false;
                        }
                        slot++;
                    }

                    //Tool Types
                    slot = 0;
                    while (!string.IsNullOrEmpty(GetXmlStr(options, "//Config/ToolTypes/Slot" + slot, false)))
                    {
                        if (Options.ToolTypes.IndexOf(GetXmlStr(options, "//Config/ToolTypes/Slot" + slot, false)) > -1)
                        {
                            Console.WriteLine(
                                "Tried to add the same type of tool twice, this is not permitted.  (Path: " +
                                "//Config/ToolTypes/Slot" + slot + ")");
                            return false;
                        }
                        else
                        {
                            Options.ToolTypes.Add(GetXmlStr(options, "//Config/ToolTypes/Slot" + slot, false));
                        }
                        slot++;
                    }

                    //Misc
                    ItemDespawnTime = GetXmlInt(options, "//Config/Misc/ItemDespawnTime");
                    ItemRespawnTime = GetXmlInt(options, "//Config/Misc/ItemSpawnTime");

                    //Combat
                    RegenTime = GetXmlInt(options, "//Config/Combat/RegenTime");
                    Options.MinAttackRate = GetXmlInt(options, "//Config/Combat/MinAttackRate");
                    Options.MaxAttackRate = GetXmlInt(options, "//Config/Combat/MaxAttackRate");
                    Options.BlockingSlow = GetXmlInt(options, "//Config/Combat/BlockingSlow") / 100;
                    Options.CritChance = GetXmlInt(options, "//Config/Combat/CritChance");
                    Options.BlockingSlow = GetXmlInt(options, "//Config/Combat/CritMultiplier") / 100;
                    Options.MaxDashSpeed = GetXmlInt(options, "//Config/Combat/MaxDashSpeed");

                    //Map
                    Options.GameBorderStyle = GetXmlInt(options, "//Config/Map/BorderStyle");
                    var zdimension = GetXmlStr(options, "//Config/Map/ZDimensionVisible", false);
                    if (zdimension != "")
                    {
                        Options.ZDimensionVisible = Convert.ToBoolean(zdimension);
                    }
                    Options.MapWidth = GetXmlInt(options, "//Config/Map/MapWidth");
                    Options.MapHeight = GetXmlInt(options, "//Config/Map/MapHeight");
                    if (Options.MapWidth < 10 || Options.MapWidth > 64 || Options.MapHeight < 10 ||
                        Options.MapHeight > 64)
                    {
                        Console.WriteLine(
                            "MapWidth and/or MapHeight are out of bounds. Must be between 10 and 64. The client loads 9 maps at a time, having large map sizes really hurts performance.");
                        ConfigFailed = true;
                    }
                    Options.TileWidth = GetXmlInt(options, "//Config/Map/TileWidth");
                    Options.TileHeight = GetXmlInt(options, "//Config/Map/TileHeight");

                    if (ConfigFailed)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to load config.xml. Exception info: " + ex.ToString());
                    return false;
                }
            }
            return !ConfigFailed;
        }

        public static byte[] GetServerConfig()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteString(Options.GameName);

            //Game Objects
            bf.WriteInteger(Options.MaxNpcDrops);

            //Player Objects
            bf.WriteInteger(Options.MaxStatValue);
            bf.WriteInteger(Options.MaxLevel);
            bf.WriteInteger(Options.MaxHotbar);
            bf.WriteInteger(Options.MaxInvItems);
            bf.WriteInteger(Options.MaxPlayerSkills);
            bf.WriteInteger(Options.MaxBankSlots);

            //Passability
            for (int i = 0; i < Enum.GetNames(typeof(MapZones)).Length; i++)
            {
                bf.WriteBoolean(Options.PlayerPassable[i]);
            }

            //Equipment
            bf.WriteInteger(Options.EquipmentSlots.Count);
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                bf.WriteString(Options.EquipmentSlots[i]);
            }
            bf.WriteInteger(Options.WeaponIndex);
            bf.WriteInteger(Options.ShieldIndex);

            //Paperdoll
            for (int i = 0; i < Options.NewPaperdollOrder.Length; i++)
            {
                bf.WriteInteger(Options.NewPaperdollOrder[i].Count);
                for (int x = 0; x < Options.NewPaperdollOrder[i].Count; x++)
                {
                    bf.WriteString(Options.NewPaperdollOrder[i][x]);
                }
            }

            //Tool Types
            bf.WriteInteger(Options.ToolTypes.Count);
            for (int i = 0; i < Options.ToolTypes.Count; i++)
            {
                bf.WriteString(Options.ToolTypes[i]);
            }

            //Combat
            bf.WriteInteger(Options.MinAttackRate);
            bf.WriteInteger(Options.MaxAttackRate);
            bf.WriteDouble(Options.BlockingSlow);
            bf.WriteInteger(Options.CritChance);
            bf.WriteDouble(Options.CritMultiplier);
            bf.WriteInteger(Options.MaxDashSpeed);

            //Map
            bf.WriteInteger(Options.GameBorderStyle);
            bf.WriteBoolean(Options.ZDimensionVisible);
            bf.WriteInteger(Options.MapWidth);
            bf.WriteInteger(Options.MapHeight);
            bf.WriteInteger(Options.TileWidth);
            bf.WriteInteger(Options.TileHeight);

            return bf.ToArray();
        }

        private static int GetXmlInt(XmlDocument xmlDoc, string xmlPath, bool required = true)
        {
            var selectSingleNode = xmlDoc.SelectSingleNode(xmlPath);
            int returnVal = 0;
            if (selectSingleNode == null)
            {
                if (required)
                {
                    Console.WriteLine("Path does not exist in config.xml  (Path: " + xmlPath + ")");
                    ConfigFailed = true;
                }
            }
            else if (!int.TryParse(selectSingleNode.InnerText, out returnVal))
            {
                if (required)
                {
                    Console.WriteLine("Failed to load value from config.xml  (Path: " + xmlPath + ")");
                    ConfigFailed = true;
                }
            }
            return returnVal;
        }

        private static string GetXmlStr(XmlDocument xmlDoc, string xmlPath, bool required = true)
        {
            var selectSingleNode = xmlDoc.SelectSingleNode(xmlPath);
            string returnVal = "";
            if (selectSingleNode == null)
            {
                if (required)
                {
                    Console.WriteLine("Path does not exist in config.xml  (Path: " + xmlPath + ")");
                    ConfigFailed = true;
                }
            }
            else
            {
                returnVal = selectSingleNode.InnerText;
            }
            return returnVal;
        }
    }
}