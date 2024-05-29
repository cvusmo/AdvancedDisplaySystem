using System;
using System.Reflection;
using HarmonyLib;
using Sandbox.ModAPI;
using VRage.Plugins;
using VRage.Game.ModAPI;

namespace cvusmo.AdvancedDisplaySystem
{
    public class Plugin : IPlugin, IDisposable
    {
        public const string Name = "AdvancedDisplaySystem";
        public static Plugin Instance { get; private set; }

        private MonitorCore monitorCore;
        private IMyPlayer player;
        private IMyCockpit currentCockpit;
        private bool playerInCockpit = false;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Init(object gameInstance)
        {
            Instance = this;

            // TODO: Put your one time initialization code here.
            Harmony harmony = new Harmony(Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            monitorCore = new MonitorCore();
            player = MyAPIGateway.Session.LocalHumanPlayer;
            MyAPIGateway.Utilities.ShowMessage("AdvancedDisplaySystem", "Advanced Display System Initialized");
        }

        public void Update()
        {
            if (player?.Controller?.ControlledEntity?.Entity is IMyCockpit cockpit)
            {
                if (!playerInCockpit)
                {
                    playerInCockpit = true;
                    currentCockpit = cockpit;
                    OnPlayerEnterCockpit(cockpit);
                }
            }
            else if (playerInCockpit)
            {
                playerInCockpit = false;
                OnPlayerExitCockpit(currentCockpit);
                currentCockpit = null;
            }
        }
        internal void OnPlayerEnterCockpit(IMyCockpit cockpit)
        {
            MyAPIGateway.Utilities.ShowMessage(Name, "Player entered cockpit");
            monitorCore.UpdateCockpitScreens("Player entered cockpit");
        }

        internal void OnPlayerExitCockpit(IMyCockpit cockpit)
        {
            MyAPIGateway.Utilities.ShowMessage(Name, "Player exited cockpit");
            monitorCore.UpdateCockpitScreens("Player exited cockpit");
        }

        public void Dispose()
        {
            // Clean up resources
            monitorCore = null;
        }

        public void HandleInput()
        {
            // Handle input if needed
        }

        // TODO: Uncomment and use this method to create a plugin configuration dialog
        // ReSharper disable once UnusedMember.Global
        /*public void OpenConfigDialog()
        {
            MyGuiSandbox.AddScreen(new MyPluginConfigDialog());
        }*/

        //TODO: Uncomment and use this method to load asset files
        /*public void LoadAssets(string folder)
        {

        }*/
    }
}