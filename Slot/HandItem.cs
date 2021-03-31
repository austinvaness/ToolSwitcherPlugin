using Sandbox.Game.Entities;
using Sandbox.Game.Weapons;
using VRage.Game;

namespace avaness.ToolSwitcherPlugin.Slot
{
    public class HandItem
    {

        public MyDefinitionId Id { get; }
        public MyDefinitionId PhysicalId { get; }

        public HandItem(IMyHandheldGunObject<MyToolBase> hand)
        {
            Id = hand.DefinitionId;
            PhysicalId = hand.PhysicalItemDefinition.Id;
        }

        public HandItem(MyDefinitionId id, MyDefinitionId physicalId)
        {
            Id = id;
            PhysicalId = physicalId;
        }
    }
}
