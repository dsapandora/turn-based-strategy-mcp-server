using TurnBasedStrategyFramework.Common.AI.BehaviourTrees;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using UnityEngine;


namespace TurnBasedStrategyFramework.Unity.AI.BehaviourTrees
{
    /// <summary>
    /// Defines specific AI logic to be executed during the unit's turn.
    /// </summary>
    public abstract class BehaviourTreeResource : MonoBehaviour
    {
        /// <summary>
        /// The behavior tree associated with this unit, used to determine AI actions.
        /// </summary>
        public ITreeNode BehaviourTree { get; protected set; }

        /// <summary>
        /// Initializes the behavior tree for the specified unit, setting up the AI logic.
        /// This method is called when the unit is initialized, preparing the behavior tree for execution during the unit's turn.
        /// </summary>
        /// <param name="unit">The unit to which this behavior tree belongs.</param>
        /// <param name="gridController">The grid controller.</param>
        public abstract void Initialize(IUnit unit, IGridController gridController);
    }
}