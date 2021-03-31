using Sandbox.Definitions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VRage.Game;

namespace avaness.ToolSwitcherPlugin.Definitions
{
    public class ToolDefinition<T> : IToolDefinition where T : MyObjectBuilder_EngineerToolBaseDefinition
    {
        private readonly MyEngineerToolBaseDefinition[] defs;
        private readonly Dictionary<MyDefinitionId, int> ids;
        private readonly Dictionary<MyDefinitionId, int> physicalIds;

        public ToolDefinition()
        {
            SortedDictionary<string, MyEngineerToolBaseDefinition> defs = new SortedDictionary<string, MyEngineerToolBaseDefinition>();
            foreach (var def in MyDefinitionManager.Static.GetHandItemDefinitions())
            {
                if (def.GetObjectBuilder() is T)
                    defs[def.Id.SubtypeName] = (MyEngineerToolBaseDefinition)def;
            }
            this.defs = defs.Values.ToArray();

            this.defs = new MyEngineerToolBaseDefinition[defs.Count];
            ids = new Dictionary<MyDefinitionId, int>();
            physicalIds = new Dictionary<MyDefinitionId, int>();
            int i = 0;
            foreach (MyEngineerToolBaseDefinition def in defs.Values)
            {
                ids[def.Id] = i;
                physicalIds[def.PhysicalItemId] = i;
                this.defs[i] = def;
                i++;
            }
        }

        public int Length => defs.Length;

        public MyEngineerToolBaseDefinition this[int index]
        {
            get
            {
                return defs[index];
            }
        }

        public bool TryGetIndex(MyDefinitionId id, out int index)
        {
            return ids.TryGetValue(id, out index);
        }

        public bool Contains(MyDefinitionId id)
        {
            return ids.ContainsKey(id);
        }

        public bool ContainsPhysical(MyDefinitionId id)
        {
            return physicalIds.ContainsKey(id);
        }

        public bool TryGetIndexPhysical(MyDefinitionId id, out int index)
        {
            return physicalIds.TryGetValue(id, out index);
        }
    }
}
