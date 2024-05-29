using HarmonyLib;
using NLua;
using Sandbox.Game.World;
using System;
using System.Reflection;
using VRage.Plugins;
using VRage.Utils;

namespace cvusmo.AdvancedDisplaySystem
{
    public class Plugin : IPlugin, IDisposable
    {
        public const string Name = "AdvancedDisplaySystem";
        public static Plugin Instance { get; private set; }
        private bool _coreInitialized = false;
        private Lua _lua;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Init(object gameInstance)
        {
            MyLog.Default.WriteLine($"[{Name}] Init Start");
            Instance = this;

            try
            {
                Harmony harmony = new Harmony(Name);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                MyLog.Default.WriteLine($"[{Name}] Harmony patched");
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLine($"[{Name}] Harmony patching failed: {ex.Message}");
            }

            MyLog.Default.WriteLine($"[{Name}] Init End");
        }

        public void Update()
        {
            if (MySession.Static != null && !_coreInitialized)
            {
                _coreInitialized = true;
                MyLog.Default.WriteLine($"[{Name}] MySession.Static is not null, initializing core");
                InitializeCore();
            }
        }

        private void InitializeCore()
        {
            if (_coreInitialized)
            {
                try
                {
                    string pluginDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string luaFilePath = System.IO.Path.Combine(pluginDirectory, "core.lua");

                    _lua = new Lua();
                    _lua.DoFile(luaFilePath);
                    _lua["LogMessage"] = (Action<string>)LogMessage;
                    MyLog.Default.WriteLine($"[{Name}] Core initialized");

                    _lua.GetFunction("ShowPluginMessage").Call();
                }
                catch (Exception ex)
                {
                    MyLog.Default.WriteLine($"[{Name}] Lua initialization failed: {ex.Message}");
                }
            }
        }

        private void LogMessage(string message)
        {
            MyLog.Default.WriteLine($"[{Name}] {message}");
        }
        public void Dispose()
        {
            MyLog.Default.WriteLine($"[{Name}] Dispose called");
            _lua?.Dispose();
        }

        public void HandleInput()
        {
            // Handle input if needed
        }
    }
}
