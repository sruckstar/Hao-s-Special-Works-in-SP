using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Windows.Forms;

public class CameraControl : Script
{
    public static Camera camObj = null;
    public static Vehicle targetVehicle = null;

    public static readonly float mouseSensitivity = 9.5f;
    public static readonly float zoomSpeed = 1.5f;
    public static readonly float minZoom = 5.0f;
    public static readonly float maxZoom = 60.0f;
    public static readonly float minPitch = 0.0f;
    public static readonly float maxPitch = 45.0f;

    public static float distanceToVehicle = 4.5f;
    public static float currentYaw = 0f;
    public static float currentPitch = 10f;

    public static bool CameraActive = false;
    public static bool CanUseMouse = false;

    public static bool invertY = true;

    public static bool isTransitioning = false;
    public static float transitionStartYaw, transitionStartPitch;
    public static float transitionTargetYaw, transitionTargetPitch;
    public static float transitionProgress = 0f;
    public static float transitionSpeed = 3f;

    public CameraControl()
    {
        Tick += OnTick;
        Interval = 0;
    }

    public static void SetCameraDirection(float targetYaw, float targetPitch)
    {
        transitionStartYaw = currentYaw;
        transitionStartPitch = currentPitch;
        transitionTargetYaw = targetYaw;
        transitionTargetPitch = targetPitch;
        transitionProgress = 0f;
        isTransitioning = true;
    }

    public static void OnTick(object sender, EventArgs e)
    {
        if (CameraActive && camObj == null)
        {
            Ped player = Game.Player.Character;
            if (!player.IsInVehicle()) return;

            targetVehicle = player.CurrentVehicle;
            Vector3 offset = GetOffsetFromAngles(currentYaw, currentPitch, distanceToVehicle);
            camObj = World.CreateCamera(targetVehicle.Position + offset, Vector3.Zero, 60f);
            camObj.PointAt(targetVehicle);
            camObj.IsActive = true;
            World.RenderingCamera = camObj;

        }
        else
        {
            if (!CameraActive && camObj != null)
            {
                camObj.IsActive = false;
                World.RenderingCamera = null;
                camObj.Delete();
                camObj = null;
                targetVehicle = null;
            }
        }

        if (camObj != null && camObj.IsActive && targetVehicle != null && targetVehicle.Exists())
        {
            Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 2);

            float inputX = Function.Call<float>(Hash.GET_DISABLED_CONTROL_NORMAL, 2, (int)GTA.Control.LookLeftRight);
            float inputY = Function.Call<float>(Hash.GET_DISABLED_CONTROL_NORMAL, 2, (int)GTA.Control.LookUpDown);

            float zoomIn = Function.Call<float>(Hash.GET_DISABLED_CONTROL_NORMAL, 2, (int)GTA.Control.VehicleFlySelectNextWeapon) * zoomSpeed;
            float zoomOut = Function.Call<float>(Hash.GET_DISABLED_CONTROL_NORMAL, 2, (int)GTA.Control.VehicleFlySelectPrevWeapon) * zoomSpeed;

            bool isUsingMouse = Function.Call<bool>(Hash.IS_USING_KEYBOARD_AND_MOUSE, 2);
            bool mouseHeld = Function.Call<bool>(Hash.IS_DISABLED_CONTROL_PRESSED, 0, (int)GTA.Control.Attack);
            bool stickMoved = !isUsingMouse && (Math.Abs(inputX) > 0.01f || Math.Abs(inputY) > 0.01f);
            bool rotating = false;

            if (CanUseMouse)
            {
                rotating = (isUsingMouse && mouseHeld) || stickMoved;

                if (rotating && !isTransitioning)
                {
                    float x = inputX * mouseSensitivity;
                    float y = inputY * mouseSensitivity;

                    if (invertY)
                        y = -y;

                    currentYaw -= x;
                    currentPitch -= y;

                    currentPitch = Clamp(currentPitch, minPitch, maxPitch);
                }

                Function.Call(Hash.SET_MOUSE_CURSOR_STYLE, rotating ? 4 : 3);
                Function.Call(Hash.SET_MOUSE_CURSOR_THIS_FRAME);
            }

            if (isTransitioning)
            {
                transitionProgress += transitionSpeed * Game.LastFrameTime;
                float t = Math.Min(transitionProgress, 1f);

                currentYaw = LerpAngle(transitionStartYaw, transitionTargetYaw, t);
                currentPitch = Lerp(transitionStartPitch, transitionTargetPitch, t);

                if (t >= 1f)
                    isTransitioning = false;
            }

            Vector3 vehiclePos = targetVehicle.Position;
            Vector3 offset = GetOffsetFromAngles(currentYaw, currentPitch, distanceToVehicle);

            camObj.Position = vehiclePos + offset;
            camObj.PointAt(targetVehicle);
        }
    }

    public static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static float Lerp(float a, float b, float t) => a + (b - a) * t;

    public static float LerpAngle(float a, float b, float t)
    {
        float delta = (b - a + 540f) % 360f - 180f;
        return a + delta * t;
    }

    public static Vector3 GetOffsetFromAngles(float yawDeg, float pitchDeg, float distance)
    {
        float yaw = yawDeg * ((float)Math.PI / 180f);
        float pitch = pitchDeg * ((float)Math.PI / 180f);

        float x = distance * (float)(Math.Cos(pitch) * Math.Cos(yaw));
        float y = distance * (float)(Math.Cos(pitch) * Math.Sin(yaw));
        float z = distance * (float)Math.Sin(pitch);

        return new Vector3(-x, -y, z);
    }
}