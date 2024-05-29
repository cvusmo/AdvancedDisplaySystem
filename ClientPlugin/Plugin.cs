using HarmonyLib;
using Sandbox.ModAPI;
using System.Reflection;
using System;
using VRage.Plugins;
using cvusmo.AdvancedDisplaySystem;

public class Plugin : IPlugin, IDisposable
{
    public const string Name = "AdvancedDisplaySystem";
    public static Plugin Instance { get; private set; }

    private Core _core;
    private bool _coreInitialized = false;
    private bool _playerInCockpit;
    private IMyCockpit _currentCockpit;

    public void Init(object gameInstance)
    {
        try
        {
            Instance = this;

            // One-time initialization code
            var harmony = new Harmony(Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            MyAPIGateway.Utilities.ShowMessage(Name, "Plugin Init Start");

            if (MyAPIGateway.Session == null)
            {
                MyAPIGateway.Utilities.ShowMessage(Name, "Session is null");
                return;
            }

            MyAPIGateway.Session.OnSessionReady += OnSessionReady;
        }
        catch (Exception ex)
        {
            MyAPIGateway.Utilities.ShowMessage(Name, $"Init Exception: {ex.Message}");
            throw;
        }
    }

    private void OnSessionReady()
    {
        try
        {
            if (MyAPIGateway.Session?.LocalHumanPlayer == null)
            {
                MyAPIGateway.Utilities.ShowMessage(Name, "LocalHumanPlayer is null");
                return;
            }

            MyAPIGateway.Utilities.ShowMessage(Name, "AdvancedDisplaySystem Session Ready");
            _core = new Core();
            _coreInitialized = true;
            MyAPIGateway.Utilities.ShowMessage(Name, "AdvancedDisplaySystem Core Initialized");
        }
        catch (Exception ex)
        {
            MyAPIGateway.Utilities.ShowMessage(Name, $"AdvancedDisplaySystem Core Initialization Exception: {ex.Message}");
            throw;
        }
    }

    public void Update()
    {
        if (_coreInitialized)
        {
            var player = MyAPIGateway.Session.LocalHumanPlayer;
            if (player?.Controller?.ControlledEntity?.Entity is IMyCockpit cockpit)
            {
                if (!_playerInCockpit)
                {
                    _playerInCockpit = true;
                    _currentCockpit = cockpit;
                    OnPlayerEnterCockpit(cockpit);
                }
            }
            else if (_playerInCockpit)
            {
                _playerInCockpit = false;
                OnPlayerExitCockpit(_currentCockpit);
                _currentCockpit = null;
            }
        }
    }

    private void OnPlayerEnterCockpit(IMyCockpit cockpit)
    {
        MyAPIGateway.Utilities.ShowMessage(Name, "Player entered cockpit");
        _core.UpdateCockpitScreens("Player entered cockpit");
    }

    private void OnPlayerExitCockpit(IMyCockpit cockpit)
    {
        MyAPIGateway.Utilities.ShowMessage(Name, "Player exited cockpit");
        _core.UpdateCockpitScreens("Player exited cockpit");
    }

    public void Dispose()
    {
        // Clean up resources
        _core = null;
        if (MyAPIGateway.Session != null)
        {
            MyAPIGateway.Session.OnSessionReady -= OnSessionReady;
        }
    }

    public void HandleInput()
    {
        // Handle input if needed
    }

    // Uncomment and use this method to create a plugin configuration dialog
    // public void OpenConfigDialog()
    // {
    //     MyGuiSandbox.AddScreen(new MyPluginConfigDialog());
    // }

    // Uncomment and use this method to load asset files
    // public void LoadAssets(string folder)
    // {
    // }
}
