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

public class HSWSP : Script
{
    public static Vector3 EnterGarage = new Vector3(779.6964f, -1867.664f, 28.29393f);
    public static Vector3 TeleportGarage = new Vector3(-2142.083f, 1160.495f, 27.65765f);
    public static Vector3 ExitGarage = new Vector3(-2142.083f, 1160.495f, 27.65765f);
    public static Vector3 TeleportExitGarage = new Vector3(786.6609f, -1869.35f, 28.19134f);
    public static Vector3 HswPropPos = new Vector3(-2168.33f, 1155.0f, 30.01f);
    public static Vector3 TuningTeleport = new Vector3(-2170.26f, 1155.958f, 28.65768f);
    public static float TeleportEnterHeading = 183.0179f;
    public static float TeleportExitHeading = 260.3245f;
    public static float TuningHeading = 183.3203f;
    public static Prop HswInterior;
    public static bool IsInGarage = false;

    public static bool checkFreezeAfterDelay = false;
    public static int checkFreezeStartTime = 0;

    public static Vector3 HSW_Pos_1 = new Vector3(-2145.59f, 1145.9f, 28.35852f);
    public static Vector3 HSW_Pos_2 = new Vector3(-2145.593f, 1148.94f, 28.35846f);
    public static float HSW_Heading_1 = 91.23364f;
    public static float HSW_Heading_2 = 90.96558f;
    public static Vehicle hsw_1;
    public static Vehicle hsw_2;
    public static Vehicle hsw_3;

    public static ObjectPool pool;
    public static NativeMenu menu;

    public static Blip lsmeet;

    public static List<NativeMenu> MenuComponents = new List<NativeMenu>();

    public static NativeItem HaoBase;

    public static string HSW_VEHMOD;
    public static string TOP_HSWUPGRADE;

    public static List<Model> vehicleModels = new List<Model>()
    {
        new Model("s95"), 
        new Model("astron2"), 
        new Model("firebolt"), 
        new Model("banshee3"),
        new Model("eurosx32"), 
        new Model("niobe"), 
        new Model("vigero3"), 
        new Model("cyclone2"),
        new Model("vivanite"), 
        new Model("coureur"), 
        new Model("buffalo5"),
        new Model("monstrociti"), 
        new Model("stingertt"),
        new Model("issi8"), 
        new Model("entity3"),
        new Model("vigero2"),
        new Model("arbitergt"),
        new Model("arbitergt"), //Ignus2 placeholder
        new Model("turismo2"),
        new Model("sentinel"), 
        new Model("banshee"),
        new Model("hakuchou2"),
        new Model("deveste"),
        new Model("brioso"), 
        new Model("feltzer3"),
        new Model("tampa4"),
        new Model("woodlander"),
    };

    public static List<Tuple<Vector3, float>> spawnPoints = new List<Tuple<Vector3, float>>()
{
    Tuple.Create(new Vector3(-2145.5f, 1142.841f, 28.35852f), 89.99499f),
    Tuple.Create(new Vector3(-2145.521f, 1139.259f, 28.17075f), 90.08498f),
    Tuple.Create(new Vector3(-2145.532f, 1136.724f, 28.35852f), 90.93261f),
    Tuple.Create(new Vector3(-2145.955f, 1133.233f, 28.3586f), 90.02995f),
    Tuple.Create(new Vector3(-2158.724f, 1125.968f, 28.3586f), 90.01997f),
    Tuple.Create(new Vector3(-2158.659f, 1122.765f, 28.35871f), 89.29716f),
    Tuple.Create(new Vector3(-2158.658f, 1116.316f, 28.35996f), 89.97066f),
    Tuple.Create(new Vector3(-2158.752f, 1112.993f, 28.3591f), 90.65173f),
    Tuple.Create(new Vector3(-2164.586f, 1126.494f, 28.35859f), -89.98002f),
    Tuple.Create(new Vector3(-2164.9f, 1123.047f, 28.35871f), -92.0479f),
    Tuple.Create(new Vector3(-2164.699f, 1116.611f, 28.35895f), -89.98002f),
    Tuple.Create(new Vector3(-2164.529f, 1112.934f, 28.35909f), -90.89143f),
    Tuple.Create(new Vector3(-2177.36f, 1137.65f, 28.35886f), -88.79655f),
    Tuple.Create(new Vector3(-2177.673f, 1134.029f, 28.35758f), -88.79541f),
    Tuple.Create(new Vector3(-2177.254f, 1124.155f, 28.35825f), -89.51555f),
    Tuple.Create(new Vector3(-2177.105f, 1127.26f, 28.3581f), -90.00002f),
    Tuple.Create(new Vector3(-2177.039f, 1117.385f, 28.35863f), -90.01498f),
    Tuple.Create(new Vector3(-2177.105f, 1114.259f, 28.35874f), -88.50171f),
    Tuple.Create(new Vector3(-2176.937f, 1104.211f, 28.36308f), -90.46947f),
    Tuple.Create(new Vector3(-2177.031f, 1107.656f, 28.36089f), -88.97485f),
    Tuple.Create(new Vector3(-2186.816f, 1139.108f, 29.47238f), 89.13446f),
    Tuple.Create(new Vector3(-2186.94f, 1135.999f, 29.47057f), 90.30463f),
    Tuple.Create(new Vector3(-2186.972f, 1132.551f, 29.47061f), 90.00497f),
    Tuple.Create(new Vector3(-2186.948f, 1129.097f, 29.47065f), 89.98501f),
    Tuple.Create(new Vector3(-2187.01f, 1125.857f, 29.47066f), 90.00499f),
    Tuple.Create(new Vector3(-2177.629f, 1094.895f, 28.3651f), -90.006f),
    Tuple.Create(new Vector3(-2177.559f, 1087.234f, 27.95241f), -90.07346f),
    /*/Tuple.Create(new Vector3(-2177.548f, 1084.049f, 27.95219f), -89.94308f),
    Tuple.Create(new Vector3(-2164.692f, 1097.896f, 27.9547f), -90.02538f),
    Tuple.Create(new Vector3(-2164.852f, 1094.665f, 27.95362f), -89.78201f),
    Tuple.Create(new Vector3(-2164.837f, 1086.836f, 27.95459f), -90.00917f),
    Tuple.Create(new Vector3(-2165.031f, 1083.488f, 27.955f), -90.00914f),
    Tuple.Create(new Vector3(-2158.685f, 1083.448f, 27.95646f), 87.9808f),
    Tuple.Create(new Vector3(-2158.745f, 1086.82f, 27.95438f), 90.0687f),
    Tuple.Create(new Vector3(-2158.735f, 1094.486f, 27.95352f), 90.00913f),
    Tuple.Create(new Vector3(-2158.772f, 1097.707f, 27.95314f), 89.78201f),
    Tuple.Create(new Vector3(-2186.802f, 1122.698f, 29.05839f), 89.98816f),
    Tuple.Create(new Vector3(-2187.094f, 1119.657f, 29.05846f), 90.01183f),/*/

};

    public static List<Vehicle> spawnedVehicles = new List<Vehicle>();

    public HSWSP()
    {
        //Loading MP Maps
        Function.Call(Hash.ON_ENTER_MP);
        Function.Call(Hash.SET_INSTANCE_PRIORITY_MODE, 1);
        Function.Call(Hash.REQUEST_IPL, "tr_tuner_meetup");
        Function.Call(Hash.REQUEST_IPL, "tr_tuner_race_line");

        HSW_VEHMOD = Game.GetLocalizedString("HSW_VEHMOD");
        TOP_HSWUPGRADE = Game.GetLocalizedString("TOP_HSWUPGRADE");

        pool = new ObjectPool();
        menu = new NativeMenu("", TOP_HSWUPGRADE, " ", new ScaledTexture(PointF.Empty, new SizeF(0, 108), "shopui_title_los_santos_car_meet", "shopui_title_los_santos_car_meet"));
        

        HaoBase = new NativeItem(HSW_VEHMOD);
        HaoBase.AltTitle = "$1450000";
        menu.Add(HaoBase);
        pool.Add(menu);

        Tick += OnTick;
        Aborted += OnAborted;

        CreateSimpleBlip();

        if (Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character.Handle) == 286209)
        {
            CreateInterior();
        }
    }

    private void OnAborted(object sender, EventArgs e)
    {
        ClearVehicles();
        DeleteSimpleBlip();
        if (HswInterior != null && HswInterior.Exists()) HswInterior.Delete();
        pool = null;
    }

    private static void CreateSimpleBlip()
    {
        Blip blip = World.CreateBlip(EnterGarage);
        blip.Sprite = BlipSprite.LSCarMeet;
        blip.IsShortRange = true;
    }

    private static void DeleteSimpleBlip()
    {
        if (lsmeet != null && lsmeet.Exists())
        {
            lsmeet.Delete();
        }
    }

    private void OnTick(object sender, EventArgs e)
    {
        pool.Process();

        if (checkFreezeAfterDelay && Game.GameTime - checkFreezeStartTime >= 2000)
        {
            checkFreezeAfterDelay = false;

            foreach (Vehicle veh in spawnedVehicles)
            {
                if (veh != null && veh.Exists())
                {
                    veh.IsPositionFrozen = true;
                }
            }

            hsw_1.IsPositionFrozen = true;
            hsw_2.IsPositionFrozen = true;
        }

        World.DrawMarker(MarkerType.VerticalCylinder, EnterGarage, Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 1), Color.LightBlue);
        World.DrawMarker(MarkerType.VerticalCylinder, ExitGarage, Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 1), Color.LightBlue);

        if (Game.Player.Character.Position.DistanceTo(EnterGarage) < 2)
        {
            GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_CONTEXT~ to enter garage");
            if (Game.IsControlJustPressed(GTA.Control.Context))
            {
                if (Game.Player.Character.CurrentVehicle == null)
                {
                    GTA.UI.Screen.FadeOut(500);
                    Wait(1000);
                    Game.Player.Character.Position = TeleportGarage;
                    Game.Player.Character.Heading = TeleportEnterHeading;
                    CreateInterior();
                    GTA.UI.Screen.FadeIn(500);
                }
                else
                {
                    if (vehicleModels.Contains(Game.Player.Character.CurrentVehicle.Model))
                    {
                        Vehicle personal = Game.Player.Character.CurrentVehicle;
                        if (personal != null && personal.Exists())
                        {
                            GTA.UI.Screen.FadeOut(500);
                            Wait(1000);

                            if (hsw_3 != null && hsw_3.Exists()) hsw_3.Delete();

                            hsw_3 = personal;

                            personal.Position = new Vector3(-2146.355f, 1129.296f, 28.65872f);
                            personal.Heading = 92.84504f;
                            CreateInterior();
                            Wait(1000);
                            personal.IsPositionFrozen = true;
                            GTA.UI.Screen.FadeIn(500);
                            Game.Player.Character.Task.LeaveVehicle();
                        }
                    }
                    else
                    {
                        GTA.UI.Screen.ShowHelpText("You need to be in a HSW vehicle to enter the garage.", 5000);
                    }
                }
            }
        }
        else
        {
            if (Game.Player.Character.Position.DistanceTo(ExitGarage) < 2)
            {
                GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_CONTEXT~ to exit garage");
                if (Game.IsControlJustPressed(GTA.Control.Context))
                {
                    GTA.UI.Screen.FadeOut(500);
                    Wait(1000);
                    Game.Player.Character.Position = TeleportExitGarage;
                    Game.Player.Character.Heading = TeleportExitHeading;

                    if (HswInterior != null && HswInterior.Exists())
                    {
                        HswInterior.Delete();
                    }

                    ClearVehicles();
                    CreateSimpleBlip();
                    IsInGarage = false;


                    GTA.UI.Screen.FadeIn(500);
                }
            }
            else
            {
                if (IsInGarage && !HaoCustom.InGarageMode)
                {
                    foreach (Vehicle veh in spawnedVehicles.ToArray())
                    {
                        if (veh != null && veh.Exists())
                        {

                            if (Game.Player.Character.CurrentVehicle == veh || Game.Player.Character.CurrentVehicle == hsw_1 || Game.Player.Character.CurrentVehicle == hsw_2 || Game.Player.Character.CurrentVehicle == hsw_3)
                            {
                                if (Game.Player.Character.CurrentVehicle != null)
                                {
                                    GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_CONTEXT~ to visit the Hao's Special Works");
                                    if (Game.IsControlJustPressed(GTA.Control.Context))
                                    {
                                        Game.Player.Character.CurrentVehicle.IsPositionFrozen = false;
                                        HaoCustom.GarageMode = true;
                                        HaoCustom.InGarageMode = true;
                                        HaoCustom.lastPos = Game.Player.Character.CurrentVehicle.Position;
                                        HaoCustom.lastHeading = Game.Player.Character.CurrentVehicle.Heading;
                                    }
                                }

                                if (Game.IsControlJustPressed(GTA.Control.VehicleAccelerate) && !HaoCustom.InGarageMode && IsInGarage)
                                {
                                    if (Game.Player.Character.CurrentVehicle != null)
                                    {
                                        Vehicle personal = Game.Player.Character.CurrentVehicle;
                                        GTA.UI.Screen.FadeOut(500);
                                        Wait(1000);
                                        personal.IsPositionFrozen = false; 
                                        personal.Position = new Vector3(787.0385f, -1868.826f, 29.18793f);
                                        personal.Heading = 262.5463f;

                                        if (HswInterior != null && HswInterior.Exists())
                                        {
                                            HswInterior.Delete();
                                        }

                                        if (personal == hsw_1) hsw_1 = null;
                                        else if (personal == hsw_2) hsw_2 = null;
                                        else if (personal == hsw_3) hsw_3 = null;
                                        else spawnedVehicles.Remove(personal);
                                        
                                        personal.MarkAsNoLongerNeeded();
                                        ClearVehicles();
                                        CreateSimpleBlip();
                                        IsInGarage = false;

                                        GTA.UI.Screen.FadeIn(500);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public static void CreateInterior()
    {
        int GarageMeet = Function.Call<int>(Hash.GET_INTERIOR_AT_COORDS, -2000.0, 1113.211, 27.66735);
        int GarageTest = Function.Call<int>(Hash.GET_INTERIOR_AT_COORDS, -2000.0, 1113.211, 27.66735);
        Function.Call(Hash.ACTIVATE_INTERIOR_ENTITY_SET, GarageMeet, "entity_set_meet_crew");
        Function.Call(Hash.ACTIVATE_INTERIOR_ENTITY_SET, GarageMeet, "entity_set_meet_lights");
        Function.Call(Hash.ACTIVATE_INTERIOR_ENTITY_SET, GarageMeet, "entity_set_player");
        Function.Call(Hash.ACTIVATE_INTERIOR_ENTITY_SET, GarageTest, "entity_set_test_crew");
        Function.Call(Hash.ACTIVATE_INTERIOR_ENTITY_SET, GarageTest, "entity_set_test_lights");
        Function.Call(Hash.ACTIVATE_INTERIOR_ENTITY_SET, GarageTest, "entity_set_time_trial");

        Function.Call(Hash.REFRESH_INTERIOR, GarageMeet);
        Function.Call(Hash.REFRESH_INTERIOR, GarageTest);

        HswInterior = World.CreatePropNoOffset(new Model("exc_prop_tr_overlay_meet"), HswPropPos, false);

        SpawnHSWCars();
        SpawnVehicles();
        DeleteSimpleBlip();
        IsInGarage = true;
        checkFreezeAfterDelay = true;
        checkFreezeStartTime = Game.GameTime;
    }

    public static void SpawnHSWCars()
    {

        Random random = new Random();

        VehicleHash car1Hash = vehicleModels[random.Next(vehicleModels.Count)];
        VehicleHash car2Hash;
        do
        {
            car2Hash = vehicleModels[random.Next(vehicleModels.Count)];
        } while (car2Hash == car1Hash);

        hsw_1 = World.CreateVehicle(car1Hash, HSW_Pos_1, HSW_Heading_1);
        while (hsw_1 == null) Script.Wait(0);
        hsw_2 = World.CreateVehicle(car2Hash, HSW_Pos_2, HSW_Heading_2);
        while (hsw_2 == null) Script.Wait(0);

        ApplyRandomTuning(hsw_1, 1, 1);
        ApplyRandomTuning(hsw_2, 1, 2);
    }

    public static void SpawnVehicles()
    {

        List<Model> shuffledVehicles = vehicleModels.OrderBy(x => Guid.NewGuid()).ToList();
        List<Tuple<Vector3, float>> shuffledPoints = spawnPoints.OrderBy(x => Guid.NewGuid()).ToList();

        for (int i = 0; i < 25; i++)
        {
            Vector3 position = shuffledPoints[i].Item1;
            float heading = shuffledPoints[i].Item2;
            Model model = shuffledVehicles[i];

            if (!model.IsLoaded)
            {
                model.Request(1000);
            }

            if (model.IsLoaded)
            {
                Vehicle vehicle = World.CreateVehicle(model, position, heading);
                if (vehicle != null)
                {
                    ApplyRandomTuning(vehicle, 0, 1);

                    vehicle.IsPersistent = true;
                    vehicle.PlaceOnGround();
                    spawnedVehicles.Add(vehicle);
                }
            }
        }
    }

    public static void ApplyRandomTuning(Vehicle vehicle, int IsHSW, int livery)
    {
        if (vehicle == null || !vehicle.Exists())
        {
            return;
        }
        Function.Call(Hash.SET_VEHICLE_MOD_KIT, vehicle, 0);
        vehicle.Mods[VehicleModType.Engine].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.Engine].Count);
        vehicle.Mods[VehicleModType.Transmission].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.Transmission].Count);
        vehicle.Mods[VehicleModType.Suspension].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.Suspension].Count);
        vehicle.Mods[VehicleModType.Brakes].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.Brakes].Count);
        vehicle.Mods[VehicleModType.Armor].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.Armor].Count);
        vehicle.Mods[VehicleModType.Horns].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.Horns].Count);
        vehicle.Mods[VehicleModType.Spoilers].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.Spoilers].Count);
        vehicle.Mods[VehicleModType.FrontWheel].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.FrontWheel].Count);
        vehicle.Mods[VehicleModType.RearWheel].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.RearWheel].Count);
        vehicle.Mods[VehicleModType.SideSkirt].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.SideSkirt].Count);
        vehicle.Mods[VehicleModType.Roof].Index = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, vehicle.Mods[VehicleModType.Livery].Count);
    
        if (IsHSW == 1)
        {
            Function.Call(Hash.SET_VEHICLE_MOD, vehicle, 36, 0, 0); //HSW Base
            Function.Call(Hash.SET_VEHICLE_MOD, vehicle, 34, 2, 0); //HSW Turbo

            int mods = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, vehicle, 48) - livery;
            Function.Call(Hash.SET_VEHICLE_MOD, vehicle, 48, mods, 0); //HSW Livery
        }
        else
        {
            int totalLiveries = Function.Call<int>(Hash.GET_NUM_VEHICLE_MODS, vehicle, 48);
            int mods = Math.Max(0, totalLiveries - 3);
            Random rnd = new Random();
            Function.Call(Hash.SET_VEHICLE_MOD, vehicle, 48, rnd.Next(0, mods + 1), 0);
        }
    }

    public static void ClearVehicles()
    {
        if (hsw_1 != null && hsw_1.Exists())
        {
            hsw_1.Delete();
        }

        if (hsw_2 != null && hsw_2.Exists())
        {
            hsw_2.Delete();
        }

        if (hsw_3 != null && hsw_3.Exists())
        {
            hsw_3.Delete();
        }

        foreach (var vehicle in spawnedVehicles)
        {
            if (vehicle != null && vehicle.Exists())
            {

                vehicle.Delete();
            }
        }
        spawnedVehicles.Clear();
    }
}