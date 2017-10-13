﻿using OpenTK.Graphics.OpenGL;
using Quad64.src.JSON;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BubblePony.Alloc;

namespace Quad64
{
    internal static class Globals
    {
		// this is just a kilobyte of stack memory set immediately within Main().
		// do not manipulate the address it is set to.
		internal static unsafe byte* _kilobyte;
		// 32 megabytes
		internal static Allocation _alloc64;
		public static unsafe byte* kilobyte => _kilobyte;
		public static unsafe byte* sixtyFourMB => _alloc64.Byte;
		public static Allocation sixtyFourMBAllocation => _alloc64;

        // Render Options
        public static bool doWireframe = false;
        public static bool drawObjectModels = true;
        public static bool renderCollisionMap = false;
        public static bool doBackfaceCulling = false;

        // Editor Options
        public static bool autoLoadROMOnStartup = false;
        public static string pathToAutoLoadROM = "";
        public static bool useHexadecimal = false;
        public static bool useSignedHex = false;

        // Advanced Options
        public static string pathToEmulator = "";
        public static bool autoSaveWhenClickEmulator = false;

        // Speed multipliers
        public static float camSpeedMultiplier = 1.0f;
        public static float objSpeedMultiplier = 1.0f;
        
        // TreeView selection
        public static int list_selected = -1;
        public static int item_selected = -1;

        // Keeps track if the user needs to save their changes.
        public static bool needToSave = false;

		// obj doesn't support vertex colors, but many applications treat extra positional values as vertex colors.
		// setting this to true does the above when exporting.
		public static bool wavefrontVertexColors=false;

		public static bool delayMID0Compression = true;
        // For the bounding boxes in the area
        public static Color ObjectColor = Color.Red;
        public static Color MacroObjectColor = Color.Blue;
        public static Color SpecialObjectColor = Color.Lime;
        public static Color SelectedObjectColor = Color.Yellow;

        public static bool DEBUG_PLG = false; // parsing level geometry flag
        public static uint DEBUG_PDL = 0x00000000; // parsing display list value
        
        // Locations in the Vanilla North American ROM (default)
        public static uint[] seg02_location = { 0x108A40, 0x114750 };
        public static uint[] seg15_location = { 0x2ABCA0, 0x2AC6B0 };
        public static uint macro_preset_table = 0xEC7E0;
        public static uint special_preset_table = 0xED350;

        // Ram to ROM conversion for assembly functions in each region
        public static uint RAMtoROM_JP = 0x80245000; // Japan
        public static uint RAMtoROM_NA = 0x80245000; // USA 
        public static uint RAMtoROM_EU = 0x80240800; // Europe
        public static uint RAMtoROM_JS = 0x80248000; // Japan (Shindou)

        // Function location that initalizes segment 0x02 for each region
        public static uint seg02_init_JP = 0x80248934; // Japan
        public static uint seg02_init_NA = 0x80248964; // USA 
        public static uint seg02_init_EU = 0x80244100; // Europe
        public static uint seg02_init_JS = 0x8024B958; // Japan (Shindou)

        // Actual function (UncIndexCopy) that is used to allocate segment 0x02 for each region
        public static uint seg02_alloc_JP = 0x80278228; // Japan
        public static uint seg02_alloc_NA = 0x802787D8; // USA 
        public static uint seg02_alloc_EU = 0x80269994; // Europe
        public static uint seg02_alloc_JS = 0x80271EF4; // Japan (Shindou)

        public static List<ObjectComboEntry> objectComboEntries = new List<ObjectComboEntry>();

		public static string getObjectComboPath(this ROM_Region region)
		{
			string str;
			if(ROM_Region.EUROPE==region)
				str= "./data/ObjectCombos_EU.json";
			else if(ROM_Region.JAPAN==region)
				str= "./data/ObjectCombos_JP.json";
			else if(ROM_Region.JAPAN_SHINDOU==region)
				str= "./data/ObjectCombos_JS.json";
			else
				str= "./data/ObjectCombos_NA.json";
			return str;
		}
		public static string getDefaultObjectComboPath(this ROM rom)
		{
			return rom.Region.getObjectComboPath();
		}
		public static string getDefaultObjectComboPath()
		{
			return ROM.Instance.getDefaultObjectComboPath();
		}

		public static void insertNewEntry(ObjectComboEntry newEntry)
        {
            for(int i = 0; i < objectComboEntries.Count; i++)
            {
                if (newEntry.ModelID == objectComboEntries[i].ModelID &&
                    newEntry.ModelSegmentAddress == objectComboEntries[i].ModelSegmentAddress &&
                    newEntry.Behavior == objectComboEntries[i].Behavior)
                {
                    objectComboEntries[i].Name = newEntry.Name; // Update name if it already exists
                    return;
                }
            }
            // Add new entry if it doesn't exist
            objectComboEntries.Add(newEntry);
        }
    }
}
