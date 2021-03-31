using Sandbox.Game.World;
using System;
using VRage.Plugins;
using VRage.Utils;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.Game.Components;
using Sandbox.ModAPI;
using Sandbox.Game.Entities.Character;
using avaness.ToolSwitcherPlugin.Tools;
using Sandbox.Common.ObjectBuilders.Definitions;
using avaness.ToolSwitcherPlugin.Slot;
using VRage.Input;
using DarkHelmet.BuildVision2;
using avaness.ToolSwitcherPlugin.Definitions;

namespace avaness.ToolSwitcherPlugin
{
    public class ToolSwitcherPlugin : IPlugin, IDisposable
    {
        [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
        public class ToolPluginSession : MySessionComponentBase
        {
            public static ToolPluginSession Instance;
            public ToolDefinitions Definitions { get; } = new ToolDefinitions();

            private bool start;
            private ToolGroup group;
            private PlayerCharacter inv;

            public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
            {
                Instance = this;
                new BvApiClient();
                BvApiClient.Init("ToolSwitcherPlugin");
                MyLog.Default.WriteLineAndConsole("Tool Plugin Session loaded.");
            }

            protected override void UnloadData()
            {
                if (inv != null)
                {
                    inv.Unload();
                }
                Instance = null;
            }

            public override void UpdateAfterSimulation()
            {
                if (MySession.Static == null)
                {
                    MyAPIGateway.Utilities.ShowNotification($"No session!", 16);
                    return;
                }

                MyCharacter ch = MySession.Static.LocalCharacter;
                if (ch == null)
                {
                    MyAPIGateway.Utilities.ShowNotification($"No character!", 16);
                    return;
                }

                if (!start)
                    Start();

                int input = MyInput.Static.DeltaMouseScrollWheelValue();
                if (ch.ToolbarType == MyToolbarType.Character && inv.Toolbar != null && inv.Inventory != null && IsEnabled())
                {
                    if(inv.Toolbar.NeedsTool)
                    {
                        inv.CheckForUpgrade = false;
                        group.EquipAny(inv);
                    }
                    else if(inv.CheckForUpgrade)
                    {
                        inv.CheckForUpgrade = false;
                        group.EquipUpgrade(inv);
                    }

                    if(input != 0)
                    {
                        group.EquipNext(inv, input > 0);
                    }
                }

                MyAPIGateway.Utilities.ShowNotification($"Working.", 16);
            }


            private void Start()
            {
                Definitions.Add<MyObjectBuilder_WelderDefinition>();
                Definitions.Add<MyObjectBuilder_AngleGrinderDefinition>();
                Definitions.Add<MyObjectBuilder_HandDrillDefinition>();
                group = new ToolGroup(Definitions);
                inv = new PlayerCharacter(MySession.Static.LocalHumanPlayer, Definitions);

                DisableModVersion();
                start = true;
            }

            private bool IsEnabled()
            {
                return  MyAPIGateway.Gui.GetCurrentScreen == MyTerminalPageEnum.None && !MyAPIGateway.Gui.IsCursorVisible && !MyAPIGateway.Gui.ChatEntryVisible
                    && !MyAPIGateway.Session.IsCameraUserControlledSpectator && string.IsNullOrWhiteSpace(MyAPIGateway.Gui.ActiveGamePlayScreen) && (!BvApiClient.Registered || !BvApiClient.Open);
            }

            private class ItemEvent
            {
                public HandItem Item { get; }
                public ToolSlot Slot { get; }

                public ItemEvent(HandItem item, ToolSlot slot)
                {
                    Item = item;
                    Slot = slot;
                }
            }

            private void DisableModVersion()
            {
                MyAPIGateway.Utilities.SendModMessage(2211605465, false);
            }
        }

        public void Dispose()
        {

        }

        public void Init(object gameInstance)
        {

        }

        public void Update()
        {
        }
    }
}
