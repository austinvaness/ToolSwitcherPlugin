using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Screens.Helpers;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage.Game;

namespace avaness.ToolSwitcherPlugin.Slot
{
    public class ToolSlot : IEquatable<ToolSlot>
    {
        public MyDefinitionId PhysicalId { get; private set; }
        public int Page { get; }
        public int Slot { get; }

        private readonly int index;
        private MyObjectBuilder_ToolbarItem ob;

        public ToolSlot(int page, int slot, MyToolbarItemDefinition item)
        {
            Page = page;
            Slot = slot;
            index = page * 9 + slot;
            PhysicalId = item.Definition.Id;
            ob = item.GetObjectBuilder();
        }

        public ToolSlot(int index, MyToolbarItemDefinition item)
        {
            Page = index / 9;
            Slot = index % 9;
            this.index = index;
            PhysicalId = item.Definition.Id;
            ob = item.GetObjectBuilder();
        }

        public void SetTo(MyToolbar toolbar, MyDefinitionId physicalId)
        {
            MyDefinitionBase defBase;
            if (!MyDefinitionManager.Static.TryGetDefinition(physicalId, out defBase))
                return;

            MyObjectBuilder_ToolbarItem ob = MyToolbarItemFactory.ObjectBuilderFromDefinition(defBase);
            MyToolbarItem toolbarItem = MyToolbarItemFactory.CreateToolbarItem(ob);
            toolbar.SetItemAtIndex(index, toolbarItem);

            this.ob = ob;
            PhysicalId = defBase.Id;
        }

        public void Clear(MyToolbar toolbar)
        {
            toolbar.SetItemAtIndex(index, null);
        }

        public void CopyFrom(MyToolbar toolbar, ToolSlot slot)
        {
            if (PhysicalId == slot.PhysicalId)
                return;

            MyToolbarItem toolbarItem = MyToolbarItemFactory.CreateToolbarItem(slot.ob);
            toolbar.SetItemAtIndex(index, toolbarItem);

            ob = slot.ob;
            PhysicalId = slot.PhysicalId;
        }

        public void Activate(MyToolbar toolbar)
        {
            if (toolbar.CurrentPage != Page)
                toolbar.SwitchToPage(Page);
            toolbar.ActivateItemAtIndex(index);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ToolSlot);
        }

        public bool Equals(ToolSlot other)
        {
            return other != null &&
                   index == other.index;
        }

        public override int GetHashCode()
        {
            return -1982729373 + index.GetHashCode();
        }

        public static bool operator ==(ToolSlot left, ToolSlot right)
        {
            return EqualityComparer<ToolSlot>.Default.Equals(left, right);
        }

        public static bool operator !=(ToolSlot left, ToolSlot right)
        {
            return !(left == right);
        }

        /*public static ToolSlot GetSlot(MyToolbar toolbar)
        {

            int currPageStart = toolbar.SlotToIndex(0);
            for(int i = currPageStart; i <= currPageStart + 8; i++)
            {
                var item = toolbar.GetItemAtIndex(i);
                if (item == null || !(item is MyToolbarItemDefinition))
                    return new ToolSlot(toolbar.CurrentPage, i - currPageStart);
            }
            return new ToolSlot(toolbar.CurrentPage, toolbar.SelectedSlot ?? 0);
        }*/

    }

}
