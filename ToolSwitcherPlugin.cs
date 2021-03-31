using Sandbox.Game.World;
using System;
using VRage.Plugins;
using VRage.Utils;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.Game.Components;
using Sandbox.ModAPI;
using Sandbox.Game.Entities;
using Sandbox.Game.Weapons;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Screens.Helpers;
using avaness.ToolSwitcherPlugin.Tools;
using Sandbox.Common.ObjectBuilders.Definitions;
using avaness.ToolSwitcherPlugin.Slot;
using Sandbox.ModAPI.Weapons;
using VRage.Input;
using DarkHelmet.BuildVision2;
using Sandbox.Game;
using VRage.Game.Entity;
using VRage;
using System.Collections.Generic;
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
            private BvApiClient client = new BvApiClient();
            //private readonly List<ItemEvent> itemRemoved = new List<ItemEvent>();
            //private readonly List<ItemEvent> itemAdded = new List<ItemEvent>();

            public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
            {
                Instance = this;
                BvApiClient.Init("ToolSwitcherPlugin");
                MyLog.Default.WriteLineAndConsole("Tool Plugin Session loaded.");
            }

            protected override void UnloadData()
            {
                if (inv != null)
                {
                    //inv.ItemRemoved -= Inv_ItemRemoved;
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
                    /*if(disabledItem != null)
                    {
                        MyToolbar toolbar = ch.Toolbar;
                        if (toolbar != null)
                            group.ReplaceItem(disabledItem, ch.GetInventory(), disabledSlot, toolbar);
                        disabledItem = null;
                    }*/
                    if(input != 0)
                    {
                        group.EquipNext(inv, input > 0);
                        /*var hand = GetHand(ch);
                        if (hand != null)
                        {
                            MyToolbar toolbar = ch.Toolbar;
                            if (toolbar != null)
                                group.EquipNext(hand, ch.GetInventory(), toolbar, input > 0);
                        }*/
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
                //inv.ItemRemoved += Inv_ItemRemoved;
                //inv.ItemAdded += Inv_ItemAdded;

                DisableModVersion();
                start = true;
            }

            /*private void Inv_ItemAdded(HandItem item, IEnumerable<ToolSlot> slots)
            {
                foreach (ToolSlot slot in slots)
                    itemAdded.Add(new ItemEvent(item, slot));
            }

            private void Inv_ItemRemoved(HandItem item, ToolSlot slot)
            {
                itemRemoved.Add(new ItemEvent(item, slot));
            }*/

            private bool IsEnabled()
            {
                return  MyAPIGateway.Gui.GetCurrentScreen == MyTerminalPageEnum.None && !MyAPIGateway.Gui.IsCursorVisible && !MyAPIGateway.Gui.ChatEntryVisible
                    && !MyAPIGateway.Session.IsCameraUserControlledSpectator && string.IsNullOrWhiteSpace(MyAPIGateway.Gui.ActiveGamePlayScreen) && (!BvApiClient.Registered || !BvApiClient.Open);
            }

            /*private HandItem GetHand(MyCharacter ch)
            {
                var temp = ch.EquippedTool as IMyHandheldGunObject<MyToolBase>;
                if (temp != null && !(temp is IMyBlockPlacerBase))
                    return new HandItem(temp);
                return null;
            }*/

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
