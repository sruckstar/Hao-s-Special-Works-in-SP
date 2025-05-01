using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using LemonUI;
using LemonUI.Elements;
using LemonUI.Menus;
using LemonUI.Extensions;

public class HaoCustom : Script
{

    private string HSW_VEHMOD;
    private string TOP_HSWUPGRADE;
    private ObjectPool pool;
    private NativeMenu menu;
    private NativeItem HaoBase;

    private bool checkMenuAfterDelay = false;
    private int checkMenuStartTime = 0;

    private int last_mod = -1;
    private int last_category = -1;
    private Dictionary<int, int> tempMods = new Dictionary<int, int>(); // Storing temporary mods
    private HashSet<int> confirmedMods = new HashSet<int>(); // Confirmed mods
    private Dictionary<int, int> originalMods = new Dictionary<int, int>(); // Storage of stocks mods

    public static Vector3 lastPos;
    public static float lastHeading;

    public static bool GarageMode = false;
    public static bool InGarageMode = false;

    public const int Spoiler = 0;
    public const int BumperFront = 1;
    public const int BumperRear = 2;
    public const int Skirt = 3;
    public const int Exhaust = 4;
    public const int Chassis = 5;
    public const int Grill = 6;
    public const int Bonnet = 7;
    public const int WingLeft = 8;
    public const int WingRight = 9;
    public const int Roof = 10;
    public const int Engine = 11;
    public const int Brakes = 12;
    public const int Gearbox = 13;
    public const int Horn = 14;
    public const int Suspension = 15;
    public const int Armour = 16;
    public const int Nitrous = 17;
    public const int Turbo = 18;
    public const int Subwoofer = 19;
    public const int TyreSmoke = 20;
    public const int Hydraulics = 21;
    public const int XenonLights = 22;
    public const int Wheels = 23;
    public const int WheelsRearOrHydraulics = 24;
    public const int PlateHolder = 25;
    public const int PlateVanity = 26;
    public const int Interior1 = 27;
    public const int Interior2 = 28;
    public const int Interior3 = 29;
    public const int Interior4 = 30;
    public const int Interior5 = 31;
    public const int Seats = 32;
    public const int Steering = 33;
    public const int Knob = 34;
    public const int Plaque = 35;
    public const int Ice = 36;
    public const int Trunk = 37;
    public const int Hydro = 38;
    public const int EngineBay1 = 39;
    public const int EngineBay2 = 40;
    public const int EngineBay3 = 41;
    public const int Chassis2 = 42;
    public const int Chassis3 = 43;
    public const int Chassis4 = 44;
    public const int Chassis5 = 45;
    public const int DoorLeft = 46;
    public const int DoorRight = 47;
    public const int LiveryMod = 48;
    public const int Lightbar = 49;

    List<string> parts = new List<string>
        {
            "Spoilers",               // 0
            "Front Bumper",           // 1
            "Rear Bumper",            // 2
            "Skirts",                 // 3
            "Exhaust",                // 4
            "Sunstrips",              // 5
            "Grille",                 // 6
            "Hoods",                  // 7
            "Left Fender",            // 8
            "Fenders",                // 9
            "Roof",                   // 10
            "Engine",                 // 11
            "Brakes",                 // 12
            "Transmission",           // 13
            "Horns",                  // 14
            "Suspension",             // 15
            "Armor",                  // 16
            "Nitrous",                // 17
            "Turbo",                  // 18
            "Subwoofer",              // 19
            "Tire Smoke",             // 20
            "Hydraulics",             // 21
            "Xenon Lights",           // 22
            "Wheels",                 // 23
            "Back Wheels",            // 24
            "Plate Holders",          // 25
            "Vanity Plates",          // 26
            "Trim Design",            // 27
            "Ornaments",              // 28
            "Dial Design",            // 29
            "Dashboard",              // 30
            "Interior Fifth",         // 31
            "Seats",                  // 32
            "Steering Wheel",         // 33
            "Turbo",                  // 34
            "Plaques",                // 35
            "HSW Upgrade",            // 36
            "Trunk",                  // 37
            "Hydraulics (Hydro)",      // 38
            "Engine Bay 1",            // 39
            "Engine Bay 2",            // 40
            "Engine Bay 3",            // 41
            "Chassis 2",              // 42
            "Chassis 3",              // 43
            "Imani Tech",              // 44
            "Skirts",                 // 45
            "Left Door",              // 46
            "Right Door",             // 47
            "Livery",                 // 48
            "Lightbar"                // 49
        };

    public HaoCustom()
    {
        HSW_VEHMOD = Game.GetLocalizedString("HSW_VEHMOD");
        TOP_HSWUPGRADE = Game.GetLocalizedString("TOP_HSWUPGRADE");

        pool = new ObjectPool();
        menu = new NativeMenu("", TOP_HSWUPGRADE, " ", new ScaledTexture(PointF.Empty, new SizeF(0, 108), "shopui_title_los_santos_car_meet", "shopui_title_los_santos_car_meet"));
        menu.Closed += (sender, e) =>
        {
            checkMenuAfterDelay = true;
            checkMenuStartTime = Game.GameTime;
        };

        HaoBase = new NativeItem(HSW_VEHMOD);
        HaoBase.AltTitle = "$1450000";
        menu.Add(HaoBase);
        pool.Add(menu);

        Tick += OnTick;
    }

    private void OnTick(object sender, EventArgs e)
    {
        pool.Process();

        if (GarageMode)
        {

            Vehicle vehicle = Game.Player.Character.CurrentVehicle;
            Vector3 TuningTeleport = new Vector3(-2170.26f, 1155.958f, 28.65768f);
            float TuningHeading = 183.3203f;
            lastPos = vehicle.Position;
            lastHeading = vehicle.Heading;

            GTA.UI.Screen.FadeOut(500);
            Wait(1000);
            vehicle.Position = TuningTeleport;
            vehicle.Heading = TuningHeading;
            Wait(1000);
            vehicle.IsPositionFrozen = true; 

            GTA.UI.Screen.FadeIn(500);
            OpenMenu();
            GarageMode = false;
        }

        if (checkMenuAfterDelay && Game.GameTime - checkMenuStartTime >= 10)
        {
            checkMenuAfterDelay = false;

            if (!pool.AreAnyVisible)
            {
                GTA.UI.Screen.FadeOut(500);
                Wait(1000);
                Game.Player.Character.CurrentVehicle.Position = lastPos;
                Game.Player.Character.CurrentVehicle.Heading = lastHeading;
                Wait(1000);
                Game.Player.Character.CurrentVehicle.IsPositionFrozen = true; 
                GTA.UI.Screen.FadeIn(500);
                InGarageMode = false;
                CameraControl.CameraActive = false;
            }
        }
    }

    private int GetVehicleMod(Vehicle veh, VehicleModType modType)
    {
        return Function.Call<int>(Hash.GET_VEHICLE_MOD, veh.Handle, (int)modType);
    }

    

    private BadgeSet CreateBadgeFromItem(string NormalDictionary, string NormalTexture, string HoveredDictionary, string HoveredTexture)
    {
        BadgeSet badge = new BadgeSet
        {
            NormalDictionary = NormalDictionary,
            NormalTexture = NormalTexture,
            HoveredDictionary = HoveredDictionary,
            HoveredTexture = HoveredTexture
        };

        return badge;
    }

    private void UpdateBadges(NativeMenu menu, Vehicle veh, int category, int selectedMod)
    {
        foreach (var item in menu.Items.OfType<NativeItem>())
        {
            if (item.Tag is int modIndex)
            {
                if (modIndex == selectedMod)
                {
                    item.RightBadgeSet = CreateBadgeFromItem("commonmenu", "shop_garage_icon_a", "commonmenu", "shop_garage_icon_b");
                    item.AltTitle = "";
                }
                else
                {
                    item.RightBadgeSet = null;
                }
            }
        }
    }

    private void SaveOriginalMods(Vehicle veh)
    {
        originalMods.Clear();
        for (int i = 0; i < 50; i++)
        {
            int currentMod = Function.Call<int>(Hash.GET_VEHICLE_MOD, veh, i);
            if (currentMod != -1)
            {
                originalMods[i] = currentMod;
            }
        }
    }

    private void RestoreOriginalMods(Vehicle veh)
    {
        foreach (var mod in tempMods)
        {
            if (!confirmedMods.Contains(mod.Key)) // only roll back temporary mods
            {
                if (originalMods.TryGetValue(mod.Key, out int originalMod))
                {
                    Function.Call(Hash.SET_VEHICLE_MOD, veh, mod.Key, originalMod, 0);
                }
                else
                {
                    Function.Call(Hash.REMOVE_VEHICLE_MOD, veh, mod.Key);
                }
            }
        }
        tempMods.Clear();
    }

    private string GetModName(Vehicle veh, int modType, int modIndex)
    {
        List<string> armor = new List<string> { "CMOD_ARM_1", "CMOD_ARM_2", "CMOD_ARM_3", "CMOD_ARM_4", "CMOD_ARM_5" };
        List<string> brakes = new List<string> { "CMOD_BRA_0", "CMOD_BRA_1", "CMOD_BRA_2", "CMOD_BRA_3", "CMOD_BRA_4" };
        List<string> engine = new List<string> { "CMOD_ENG_2", "CMOD_ENG_3", "CMOD_ENG_4", "CMOD_ENG_5", "CMOD_ENG_6" };
        List<string> suspension = new List<string> { "CMOD_SUS_0", "CMOD_SUS_1", "CMOD_SUS_2", "CMOD_SUS_3", "CMOD_SUS_4", "CMOD_SUS_5" };
        List<string> transmission = new List<string> { "CMOD_GBX_0", "CMOD_GBX_1", "CMOD_GBX_2", "CMOD_GBX_3", "CMOD_GBX_4" };

        string ModName = null;

        ModName = Game.GetLocalizedString(Function.Call<string>(Hash.GET_MOD_TEXT_LABEL, veh, modType, modIndex));
        
        if (ModName.Length < 2)
        {

            switch (modType)
            {
                case 16:
                    ModName = Game.GetLocalizedString(armor[modIndex]);
                    break;

                case 12:
                    ModName = Game.GetLocalizedString(brakes[modIndex]);
                    break;

                case 11:
                    ModName = Game.GetLocalizedString(engine[modIndex]);

                    if (ModName.Length < 2) ModName = "HSW Engine Tune";

                    break;

                case 15:
                    ModName = Game.GetLocalizedString(suspension[modIndex]);
                    break;

                case 13:
                    ModName = Game.GetLocalizedString(transmission[modIndex]);
                    break;

                case 21:
                    if (ModName.Length < 2) ModName = "Hydraulics";
                    break;
            }
        }

        return ModName;
    }

    private NativeMenu BuildComponentsMenu(Vehicle veh)
    {
        SaveOriginalMods(veh); // Save the original mods before opening the menu
        List<int> componentOrder = new List<int>();
        for (int i = 0; i < 50; i++)
        {
            if (i == 14) continue;

            componentOrder.Add(i);
        }

        List<NativeMenu> submenusList = new List<NativeMenu>();
        NativeMenu components = new NativeMenu("", TOP_HSWUPGRADE, " ", new ScaledTexture(PointF.Empty, new SizeF(0, 108), "shopui_title_los_santos_car_meet", "shopui_title_los_santos_car_meet"));

        int name_index = 0;
        foreach (int i in componentOrder)
        {

            int mods = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, veh, i);
            if (mods == 0) continue;
            string catName = null;

            if (veh.Model.Hash == new Model("astron2").Hash)
            {
                switch (i)
                {
                    case 8:
                        catName = "Mirrors";
                        break;
                    case 5:
                        catName = "Roof Spoilers";
                        break;
                    case 9:
                        catName = "Roof Rails";
                        break;
                    case 6:
                        catName = "Suntrips";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("firebolt").Hash)
            {
                switch (i)
                {
                    case 5:
                        catName = "Chassis";
                        break;
                    case 8:
                        catName = "Mirrors";
                        break;
                    case 10:
                        catName = "Sunstrips";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("banshee3").Hash)
            {
                switch (i)
                {
                    case 5:
                        catName = "Roll Cage";
                        break;
                    case 8:
                        catName = "Mirrors";
                        break;
                    case 9:
                        catName = "Trim";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("niobe").Hash)
            {
                switch (i)
                {
                    case 5:
                        catName = "Arch Covers";
                        break;
                    case 9:
                        catName = "Mirrors";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("vigero3").Hash)
            {
                switch (i)
                {
                    case 8:
                        catName = "Splitters";
                        break;
                    case 9:
                        catName = "Mirrors";
                        break;
                    case 10:
                        catName = "Rear Diffusers";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("vivanite").Hash)
            {
                switch (i)
                {
                    case 5:
                        catName = "Roll Cages";
                        break;
                    case 8:
                        catName = "Mirrors";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("coureur").Hash)
            {
                switch (i)
                {
                    case 8:
                        catName = "Mirrors";
                        break;
                    case 9:
                        catName = "Roll Cages";
                        break;
                    case 44:
                        catName = "Imani Tech";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("buffalo5").Hash)
            {
                switch (i)
                {
                    case 5:
                        catName = "Armor Plating";
                        break;
                    case 6:
                        catName = "Body Work";
                        break;
                    case 8:
                        catName = "Mirrors";
                        break;
                    case 9:
                        catName = "Proximity Mine";
                        break;
                    case 28:
                        catName = "Roll Cages";
                        break;
                    case 44:
                        catName = "Imani Tech";
                        break;
                    case 47:
                        catName = "Louvers";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("monstrociti").Hash)
            {
                switch (i)
                {
                    case 0:
                        catName = "Window Panels";
                        break;
                    case 3:
                        catName = "Mudguards";
                        break;
                    case 5:
                        catName = "Bullbars";
                        break;
                    case 8:
                        catName = "Snorkels";
                        break;
                    case 44:
                        catName = "Imani Tech";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("stingertt").Hash)
            {
                switch (i)
                {
                    case 5:
                        catName = "Armor Plating";
                        break;
                    case 8:
                        catName = "Splitters";
                        break;
                    case 9:
                        catName = "Proximity Mine";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("issi8").Hash)
            {
                switch (i)
                {
                    case 5:
                        catName = "Chassis";
                        break;
                    case 8:
                        catName = "Sunstrips";
                        break;
                    case 9:
                        catName = "Mirrors";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("entity3").Hash)
            {
                switch (i)
                {
                    case 8:
                        catName = "Mirrors";
                        break;
                    case 9:
                        catName = "Side panels";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("vigero2").Hash)
            {
                switch (i)
                {
                    case 8:
                        catName = "Louvers";
                        break;
                    case 9:
                        catName = "Mirrors";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("turismo2").Hash)
            {
                switch (i)
                {
                    case 8:
                        catName = "Engine Block";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("sentinel").Hash)
            {
                switch (i)
                {
                    case 5:
                        catName = "Roll Cages";
                        break;
                    case 8:
                        catName = "Sunstrips";
                        break;
                    case 9:
                        catName = "Mudguards";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("hakuchou2").Hash)
            {
                switch (i)
                {
                    case 5:
                        catName = "Fairings";
                        break;
                    case 10:
                        catName = "Fuel Tanks";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("feltzer3").Hash)
            {
                switch (i)
                {
                    case 5:
                        catName = "Roll Cages";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }
            else if (veh.Model.Hash == new Model("cyclone2").Hash)
            {
                switch (i)
                {
                    case 8:
                        catName = "Mirrors";
                        break;
                    default:
                        catName = parts[i];
                        break;
                }
            }

            else
            {
                catName = parts[i];
            }

            NativeMenu menu = new NativeMenu("", catName, " ", new ScaledTexture(PointF.Empty, new SizeF(0, 108), "shopui_title_los_santos_car_meet", "shopui_title_los_santos_car_meet"));
            menu.CloseOnInvalidClick = false;
            menu.UseMouse = false;
            menu.DisableControls = false;
            submenusList.Add(menu);
            name_index++;

            menu.Opening += (sender, e) => 
            {
                CameraControl.CanUseMouse = true;

                switch (i)
                {
                    case 0:
                    case 2:
                    case 4:
                        CameraControl.SetCameraDirection(-90f, 10f); // Back
                        break;
                    case 1:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        CameraControl.SetCameraDirection(90f, 10f);  // Forward
                        break;
                    case 3:
                    case 5:
                        CameraControl.SetCameraDirection(0f, 10f);   // Right
                        
                        break;
                    case 23:
                        CameraControl.SetCameraDirection(180f, 10f); // Left
                        break;
                    case 10:
                        CameraControl.SetCameraDirection(CameraControl.currentYaw, 45f); // Above
                        break;

                    default:
                        CameraControl.SetCameraDirection(48f, 6f); // Standard side
                        break;
                }
            };

            menu.Closed += (sender, e) => 
            {
                RestoreOriginalMods(veh);
                confirmedMods.Clear();
                checkMenuAfterDelay = true;
                checkMenuStartTime = Game.GameTime;
                CameraControl.CanUseMouse = false;
            };

            components.Closed += (sender, e) => 
            {

                confirmedMods.Clear();
                checkMenuAfterDelay = true;
                checkMenuStartTime = Game.GameTime;
            };

            for (int j = 0; j < mods; j++)
            {
                string modName = GetModName(veh, i, j);

                NativeItem item = new NativeItem(modName) { Tag = j };

                int category = i;
                int modIndex = j;

                if (originalMods.TryGetValue(category, out int originalMod) && originalMod == modIndex)
                {
                    item.RightBadgeSet = CreateBadgeFromItem("commonmenu", "shop_garage_icon_a", "commonmenu", "shop_garage_icon_b");
                    item.AltTitle = "";
                }

                item.Selected += (sender, e) => 
                {
                    if (!confirmedMods.Contains(category)) 
                    {
                        if (tempMods.ContainsKey(category))
                        {
                            Function.Call(Hash.REMOVE_VEHICLE_MOD, veh, category); 
                        }
                        Function.Call(Hash.SET_VEHICLE_MOD, veh, category, modIndex, 0);
                        tempMods[category] = modIndex;
                    }
                };

                item.Activated += (sender, e) => 
                {
                    if (Game.IsControlPressed(GTA.Control.Attack))
                    {
                        return;
                    }

                    if (!originalMods.ContainsKey(category)) originalMods.Add(category, -1);
                    if (originalMods[category] != modIndex)
                    {
                        confirmedMods.Add(category);
                        tempMods.Remove(category); 
                        originalMods[category] = modIndex; 
                        Function.Call(Hash.SET_VEHICLE_MOD, veh, category, modIndex, 0);
                        UpdateBadges(menu, veh, category, modIndex); 

                        RestoreOriginalMods(veh);
                        confirmedMods.Clear();
                    }
                };

                menu.Add(item);
            }
        }

        foreach (NativeMenu menu in submenusList)
        {
            menu.UseMouse = false;
            menu.CloseOnInvalidClick = false;
            menu.DisableControls = false;
            menu.RotateCamera = true;
            pool.Add(menu);
            components.AddSubMenu(menu);
        }
        return components;
    }


    private void OpenMenu()
    {
        Ped player = Game.Player.Character;


        CameraControl.CameraActive = true;
        CameraControl.SetCameraDirection(48f, 6f);

        if (player.CurrentVehicle != null)
        {
            Vehicle veh = player.CurrentVehicle;
            int mod = GetVehicleMod(veh, VehicleModType.Speakers);
            if (mod == -1)
            {
                menu.Visible = true;
                menu.ItemActivated += (sndrs, item) =>
                {
                    if (Game.Player.Money >= 1450000)
                    {
                        Game.Player.Money -= 1450000;
                        Function.Call(Hash.SET_VEHICLE_MOD_KIT, veh, 0);
                        Function.Call(Hash.SET_VEHICLE_MOD, veh, 36, 0, 0);
                        pool.HideAll();
                        NativeMenu components = BuildComponentsMenu(veh);
                        pool.Add(components);
                        components.Visible = true;
                        components.UseMouse = false;
                        components.CloseOnInvalidClick = false;
                    }
                    else
                    {
                        GTA.UI.Screen.ShowHelpText("You don't have enough money.");
                    }
                };
            }
            else
            {
                NativeMenu components = BuildComponentsMenu(veh);
                pool.Add(components);
                components.Visible = true;
                components.UseMouse = false;
                components.CloseOnInvalidClick = false;
            }
        }
    }
}