using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.Game.GUI.TextPanel;
using VRageMath;
using Sandbox.ModAPI.Ingame;
using System;
using System.Net.Mime;
using cvusmo.AdvancedDisplaySystem;

namespace cvusmo.AdvancedDisplaySystem
{
    public class Core
    {
        private Sandbox.ModAPI.IMyCameraBlock camera;
        private Sandbox.ModAPI.IMyTextSurface lcd;
        private Sandbox.ModAPI.IMyCockpit cockpit;

        public Core()
        {
            try
            {
                if (MyAPIGateway.Session == null)
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "Session is null");
                    return;
                }

                if (MyAPIGateway.Session.LocalHumanPlayer == null)
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "LocalHumanPlayer is null");
                    return;
                }

                var controller = MyAPIGateway.Session.LocalHumanPlayer.Controller;
                if (controller == null)
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "Controller is null");
                    return;
                }

                var controlledEntity = controller.ControlledEntity;
                if (controlledEntity == null)
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "ControlledEntity is null");
                    return;
                }

                var grid = controlledEntity.Entity.GetTopMostParent() as IMyCubeGrid;
                if (grid == null)
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "Grid is null");
                    return;
                }

                var gridTerminalSystem = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(grid);
                if (gridTerminalSystem == null)
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "GridTerminalSystem is null");
                    return;
                }

                camera = gridTerminalSystem.GetBlockWithName("Camera") as Sandbox.ModAPI.IMyCameraBlock;
                lcd = gridTerminalSystem.GetBlockWithName("LCD") as Sandbox.ModAPI.IMyTextSurface;

                // Debug: Check if camera and LCD were found
                if (camera == null)
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "Camera not found!");
                }
                else
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "Camera found!");
                }

                if (lcd == null)
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "LCD not found!");
                }
                else
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "LCD found!");
                }

                cockpit = gridTerminalSystem.GetBlockWithName("MV-100 Cockpit") as Sandbox.ModAPI.IMyCockpit;
                if (cockpit == null)
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "Cockpit not found!");
                    return;
                }
                else
                {
                    MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "Cockpit found!");
                }

                SetupCockpitScreens();
            }
            catch (Exception ex)
            {
                MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", $"core Constructor Exception: {ex.Message}");
            }
        }

        private void SetupCockpitScreens()
        {
            // Left Screen
            string leftOutput = GetLeftScreenInfo(cockpit);
            Sandbox.ModAPI.IMyTextSurface leftSurface = cockpit.GetSurface(1) as Sandbox.ModAPI.IMyTextSurface;
            SetSurfaceProperties(leftSurface, leftOutput, 1f);

            // Middle Screen
            string middleOutput = GetMiddleScreenInfo(cockpit);
            Sandbox.ModAPI.IMyTextSurface middleSurface = cockpit.GetSurface(0) as Sandbox.ModAPI.IMyTextSurface;
            SetSurfaceProperties(middleSurface, middleOutput, 1f);

            // Right Screen
            string rightOutput = GetRightScreenInfo(cockpit);
            Sandbox.ModAPI.IMyTextSurface rightSurface = cockpit.GetSurface(2) as Sandbox.ModAPI.IMyTextSurface;
            SetSurfaceProperties(rightSurface, rightOutput, 2f);
        }

        private string GetLeftScreenInfo(Sandbox.ModAPI.IMyCockpit cockpit)
        {
            return "LEFTSCREEN";
        }

        private string GetMiddleScreenInfo(Sandbox.ModAPI.IMyCockpit cockpit)
        {
            return "MIDDLESCREEN";
        }

        private string GetRightScreenInfo(Sandbox.ModAPI.IMyCockpit cockpit)
        {
            return "RIGHTSCREEN";
        }

        internal void SetSurfaceProperties(Sandbox.ModAPI.IMyTextSurface surface, string text, float fontSize = -1f)
        {
            surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
            surface.Font = "Monospace";
            surface.Alignment = TextAlignment.CENTER;

            if (fontSize > 0)
            {
                surface.FontSize = fontSize;
            }
            else
            {
                float calculatedFontSize = CalculateFontSize(text, surface);
                surface.FontSize = calculatedFontSize;
            }

            surface.WriteText(text, false);
        }

        internal float CalculateFontSize(string text, Sandbox.ModAPI.IMyTextSurface surface)
        {
            float surfaceWidth = surface.SurfaceSize.X;
            float surfaceHeight = surface.SurfaceSize.Y;
            int textLength = text.Length;

            float scalingFactor = 0.3f;
            float fontSize = Math.Min(surfaceWidth / (textLength * 1.5f), surfaceHeight / 1) * scalingFactor;

            return Math.Max(fontSize, 0.4f);
        }

        public void Execute(string argument, UpdateType updateSource)
        {
            if (camera != null)
            {
                MyDetectedEntityInfo info = camera.Raycast(1000);
                string output = info.IsEmpty() ? "No entity detected." : $"Detected: {info.Name}";

                if (lcd != null)
                {
                    lcd.WriteText(output);
                }

                UpdateCockpitScreens(output);
            }
        }

        internal void UpdateCockpitScreens(string info)
        {
            if (cockpit != null)
            {
                // Left Screen
                Sandbox.ModAPI.IMyTextSurface leftSurface = cockpit.GetSurface(1) as Sandbox.ModAPI.IMyTextSurface;
                SetSurfaceProperties(leftSurface, info, 1f);

                // Middle Screen
                Sandbox.ModAPI.IMyTextSurface middleSurface = cockpit.GetSurface(0) as Sandbox.ModAPI.IMyTextSurface;
                SetSurfaceProperties(middleSurface, info, 1f);

                // Right Screen
                Sandbox.ModAPI.IMyTextSurface rightSurface = cockpit.GetSurface(2) as Sandbox.ModAPI.IMyTextSurface;
                SetSurfaceProperties(rightSurface, info, 2f);
            }
        }
    }
}
