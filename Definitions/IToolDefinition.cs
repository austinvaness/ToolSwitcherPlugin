using Sandbox.Definitions;
using System.Collections.Generic;
using VRage.Game;

namespace avaness.ToolSwitcherPlugin.Definitions
{
    public interface IToolDefinition
    {
        MyEngineerToolBaseDefinition this[int index] { get; }

        int Length { get; }

        bool Contains(MyDefinitionId id);
        bool ContainsPhysical(MyDefinitionId id);
        bool TryGetIndex(MyDefinitionId id, out int index);
        bool TryGetIndexPhysical(MyDefinitionId id, out int index);
    }
}