using avaness.ToolSwitcherPlugin.Definitions;
using avaness.ToolSwitcherPlugin.Slot;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Game.Weapons;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage;
using VRage.Game;

namespace avaness.ToolSwitcherPlugin.Tools
{
    public class Tool
    {
        private readonly IToolDefinition def;

        public Tool(IToolDefinition def)
        {
            this.def = def;
        }

        public bool Contains(MyDefinitionId physicalId)
        {
            return def.ContainsPhysical(physicalId);
        }

        public bool Equip(ToolSlot hand, PlayerCharacter ch)
        {
            if (hand == null)
                return false;

            if (def.TryGetIndexPhysical(hand.PhysicalId, out int i))
                return EquipBest(ch, i);
            return EquipBest(ch);
        }

        public bool Equip(PlayerCharacter ch)
        {
            return EquipBest(ch);
        }

        private bool EquipBest(PlayerCharacter ch, int min = -1)
        {
            MyFixedPoint one = 1;
            for (int i = def.Length - 1; i > min; i--)
            {
                if (MyAPIGateway.Session.CreativeMode || ch.Inventory.ContainItems(one, def[i].PhysicalItemId))
                {
                    ch.Toolbar.SetTool(def[i].PhysicalItemId);
                    return true;
                }
            }
            return false;
        }

        public bool EquipUpgrade(MyDefinitionId physicalId, PlayerCharacter ch)
        {
            if (def.TryGetIndexPhysical(physicalId, out int i))
                return EquipBest(ch, i);
            return false;
        }
    }
}
