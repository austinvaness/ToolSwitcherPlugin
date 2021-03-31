using avaness.ToolSwitcherPlugin.Definitions;
using avaness.ToolSwitcherPlugin.Slot;
using Sandbox.Game.Screens.Helpers;
using System;
using VRage.Game;

namespace avaness.ToolSwitcherPlugin
{
    public class PlayerToolbar
    {
        private readonly MyToolbar toolbar;
        private readonly ToolDefinitions defs;
        private int? activeToolPage;
        private readonly ToolSlot[] toolPages = new ToolSlot[9];

        public bool NeedsTool { get; private set; }

        public PlayerToolbar(MyToolbar toolbar, ToolDefinitions defs)
        {
            this.toolbar = toolbar;
            this.defs = defs;

            ToolSlot firstTool = null;
            for(int i = 80; i >= 0; i--)
            {
                ToolSlot slot = CreateSlot(toolbar[i], i);
                if (slot != null)
                {
                    if (firstTool == null)
                        firstTool = slot;

                    ToolSlot current = toolPages[slot.Page];
                    if (current != null)
                    {
                        current.Clear(toolbar);
                        if (firstTool == current)
                            firstTool = slot;
                    }
                    toolPages[slot.Page] = slot;
                }
            }

            ToolSlot initialTool = toolPages[toolbar.CurrentPage];
            if (initialTool != null)
                activeToolPage = toolbar.CurrentPage;

            if (initialTool == null)
            {
                if (firstTool == null)
                    return;
                initialTool = firstTool;
            }

            CopyFrom(initialTool);

            toolbar.SlotActivated += Toolbar_SlotActivated;
            toolbar.Unselected += Toolbar_Unselected;
            //toolbar.ItemChanged += Toolbar_ItemChanged; Handled by CopyFrom
            toolbar.ItemUpdated += Toolbar_ItemUpdated;
        }

        public void Unload()
        {
            toolbar.SlotActivated -= Toolbar_SlotActivated;
            toolbar.Unselected -= Toolbar_Unselected;
            toolbar.ItemChanged -= Toolbar_ItemChanged;
            toolbar.ItemUpdated -= Toolbar_ItemUpdated;
        }
        
        private void Toolbar_ItemUpdated(MyToolbar toolbar, MyToolbar.IndexArgs index, MyToolbarItem.ChangeInfo change)
        {
            if(change == MyToolbarItem.ChangeInfo.Enabled)
            {
                MyToolbarItemDefinition itemDef = toolbar[index.ItemIndex] as MyToolbarItemDefinition;
                if (itemDef?.Definition != null && !itemDef.Enabled && defs.ContainsPhysical(itemDef.Definition.Id))
                    NeedsTool = true;
            }
        }

        private void Toolbar_ItemChanged(MyToolbar toolbar, MyToolbar.IndexArgs index, bool arg3)
        {
            ToolSlot slot = CreateSlot(toolbar.GetItemAtIndex(index.ItemIndex), index.ItemIndex);
            if (slot != null)
            {
                ToolSlot current = toolPages[slot.Page];
                if (current != null && current.Slot != slot.Slot)
                    current.Clear(toolbar);
                toolPages[slot.Page] = slot;
                CopyFrom(slot);
            }
        }

        private void CopyFrom(ToolSlot slot)
        {
            toolbar.ItemChanged -= Toolbar_ItemChanged;
            foreach(ToolSlot s in toolPages)
            {
                if(s != null && s != slot)
                    s.CopyFrom(toolbar, slot);
            }
            NeedsTool = false;
            toolbar.ItemChanged += Toolbar_ItemChanged;
        }

        private void Toolbar_SlotActivated(MyToolbar toolbar, MyToolbar.SlotArgs slot, bool arg3)
        {
            if (!slot.SlotNumber.HasValue)
                return;

            ToolSlot newSlot = CreateSlot(toolbar.GetSlotItem(slot.SlotNumber.Value), toolbar.CurrentPage, slot.SlotNumber.Value);
            if (newSlot != null)
            {
                ToolSlot current = toolPages[newSlot.Page];
                if (current == null || current.Slot != newSlot.Slot)
                    toolPages[newSlot.Page] = newSlot;
                activeToolPage = newSlot.Page;
                NeedsTool = false;
            }
        }

        private void Toolbar_Unselected(MyToolbar toolbar)
        {
            activeToolPage = null;
        }

        public ToolSlot GetHandSlot()
        {
            if (!activeToolPage.HasValue)
                return null;

            int currentPage = toolbar.CurrentPage;
            ToolSlot currentSlot = toolPages[currentPage];
            if (currentSlot != null)
                return currentSlot;

            return toolPages[activeToolPage.Value];
        }

        public ToolSlot GetToolSlot()
        {
            ToolSlot pageSlot = toolPages[toolbar.CurrentPage];
            if (pageSlot != null)
                return pageSlot;
            if (activeToolPage.HasValue)
                return toolPages[activeToolPage.Value];
            foreach (ToolSlot s in toolPages)
            {
                if (s != null)
                    return s;
            }
            return null;
        }

        private ToolSlot CreateSlot(MyToolbarItem item, int page, int slot)
        {
            MyToolbarItemDefinition itemDef = item as MyToolbarItemDefinition;
            if (itemDef?.Definition != null && defs.ContainsPhysical(itemDef.Definition.Id))
                return new ToolSlot(page, slot, itemDef);
            return null;
        }

        private ToolSlot CreateSlot(MyToolbarItem item, int index)
        {
            MyToolbarItemDefinition itemDef = item as MyToolbarItemDefinition;
            if (itemDef?.Definition != null && defs.ContainsPhysical(itemDef.Definition.Id))
                return new ToolSlot(index, itemDef);
            return null;
        }

        public void SetTool(MyDefinitionId physicalId)
        {
            ToolSlot refer = GetToolSlot();
            if (refer == null)
                return;

            toolbar.Unselect(false);

            refer.SetTo(toolbar, physicalId);

            CopyFrom(refer);

            refer.Activate(toolbar);
            activeToolPage = refer.Page;
            NeedsTool = false;
        }
    }
}
