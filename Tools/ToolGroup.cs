using avaness.ToolSwitcherPlugin.Definitions;
using avaness.ToolSwitcherPlugin.Slot;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Screens.Helpers;
using System.Collections.Generic;
using VRage.Game;
using System.Linq;
using Sandbox.ModAPI;
using System;

namespace avaness.ToolSwitcherPlugin.Tools
{
    public class ToolGroup
    {
        private readonly Tool[] tools;
        private readonly ToolDefinitions defs;

        public ToolGroup(ToolDefinitions defs)
        {
            this.defs = defs;
            tools = defs.Select((d) => new Tool(d)).ToArray();
        }

        public void EquipNext(PlayerCharacter ch, bool dir)
        {
            ToolSlot hand = ch.Toolbar.GetHandSlot();

            if (hand == null)
                return;

            for(int i = 0; i < tools.Length; i++)
            {
                if(tools[i].Contains(hand.PhysicalId))
                {
                    EquipNext(i, dir, hand, ch);
                    return;
                }
            }
        }

        private void EquipNext(int index, bool dir, ToolSlot hand, PlayerCharacter ch)
        {
            if(dir)
            {
                for(int i = (index + 1) % tools.Length; i != index; i = (i + 1) % tools.Length)
                {
                    if (tools[i].Equip(hand, ch))
                        return;
                }
            }
            else
            {
                int i = index - 1;
                if (i < 0)
                    i = tools.Length - 1;

                while(i != index)
                {
                    if (tools[i].Equip(hand, ch))
                        return;

                    i--;
                    if (i < 0)
                        i = tools.Length - 1;
                }
            }
        }

        public void EquipAny(PlayerCharacter ch)
        {
            ToolSlot current = ch.Toolbar.GetToolSlot();
            if (current != null && TryFindTool(current.PhysicalId, out Tool currentTool) && currentTool.Equip(ch))
                return;

            foreach (Tool t in tools)
            {
                if (t.Equip(ch))
                    return;
            }
        }

        public void EquipUpgrade(PlayerCharacter ch)
        {
            ToolSlot slot = ch.Toolbar.GetToolSlot();
            if (slot == null)
                return;

            if (TryFindTool(slot.PhysicalId, out Tool tool))
                tool.EquipUpgrade(slot.PhysicalId, ch);
        }

        private bool TryFindTool(MyDefinitionId physicalId, out Tool tool)
        {
            foreach(Tool t in tools)
            {
                if (t.Contains(physicalId))
                {
                    tool = t;
                    return true;
                }
            }
            tool = null;
            return false;
        }

        /*private ToolSlot GetSlot(MyToolbar toolbar)
        {
            int currPageStart = toolbar.SlotToIndex(0);

            if (toolbar.SelectedSlot.HasValue)
            {
                if (toolbar.SelectedItem is MyToolbarItemDefinition item && defs.ContainsPhysical(item.Definition.Id))
                    return new ToolSlot(toolbar.CurrentPage, toolbar.SelectedSlot.Value);
            }

            int currPageEnd = currPageStart + 8;

            ToolSlot result = GetSlot(toolbar, currPageStart, currPageEnd);
            if (result != null)
                return result;

            if (currPageStart > 0)
            {
                result = GetSlot(toolbar, currPageStart - 1, 0);
                if (result != null)
                    return result;
            }

            int len = toolbar.PageCount * toolbar.SlotCount;
            if (currPageEnd < len - 1)
            {
                result = GetSlot(toolbar, currPageEnd + 1, len - 1);
                if (result != null)
                    return result;
            }

            return ToolSlot.GetSlot(toolbar);
        }

        private ToolSlot GetSlot(MyToolbar toolbar, int start, int end)
        {
            MyToolbarItemDefinition toolbarItem;
            if (start > end)
            {
                for (int i = start; i >= end; i--)
                {
                    toolbarItem = toolbar[i] as MyToolbarItemDefinition;
                    if (toolbarItem != null && defs.ContainsPhysical(toolbarItem.Definition.Id))
                        return new ToolSlot(i);
                }
            }
            else
            {
                for (int i = start; i <= end; i++)
                {
                    toolbarItem = toolbar[i] as MyToolbarItemDefinition;
                    if (toolbarItem != null && defs.ContainsPhysical(toolbarItem.Definition.Id))
                        return new ToolSlot(i);
                }
            }
            return null;
        }*/
    }
}
