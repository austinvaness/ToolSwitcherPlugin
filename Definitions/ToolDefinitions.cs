using System.Collections;
using System.Collections.Generic;
using VRage.Game;

namespace avaness.ToolSwitcherPlugin.Definitions
{
    public class ToolDefinitions : IEnumerable<IToolDefinition>
    {
        private readonly List<IToolDefinition> definitions = new List<IToolDefinition>();

        public IToolDefinition Add<T>() where T : MyObjectBuilder_EngineerToolBaseDefinition
        {
            IToolDefinition newTool = new ToolDefinition<T>();
            definitions.Add(newTool);
            return newTool;
        }

        public bool Contains(MyDefinitionId id)
        {
            foreach(IToolDefinition def in definitions)
            {
                if (def.Contains(id))
                    return true;
            }
            return false;
        }

        public bool ContainsPhysical(MyDefinitionId id)
        {
            foreach (IToolDefinition def in definitions)
            {
                if (def.ContainsPhysical(id))
                    return true;
            }
            return false;
        }

        public IEnumerator<IToolDefinition> GetEnumerator()
        {
            return definitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return definitions.GetEnumerator();
        }
    }
}
