using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.AI.BehaviourTrees;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;

namespace TurnBasedStrategyFramework.Unity.AI.BehaviourTrees
{
    /// <summary>
    /// A behavior tree node that performs no operations.
    /// </summary>
    public partial class NoOpBehaviourTreeNode : BehaviourTreeResource
    {
        public override void Initialize(IUnit unit, IGridController gridController)
        {
            BehaviourTree = new SelectorNode(new List<ITreeNode>());
        }
    }
}