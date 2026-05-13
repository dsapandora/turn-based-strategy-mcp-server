using System.Threading.Tasks;

namespace TurnBasedStrategyFramework.Common.AI.BehaviourTrees
{
    /// <summary>
    /// Introduces a realtime delay to AI decision making.
    /// </summary>
    public readonly struct RealtimeDelayNode : ITreeNode
    {
        /// <summary>
        /// A behavior tree node that introduces a real-time delay.
        /// 
        /// !! Not supported in WebGL builds:
        /// WebGL does not support multithreading or the .NET task scheduler required for Task.Delay,
        /// causing this node to hang or crash at runtime.
        /// </summary>
        private readonly int _delay;

        public RealtimeDelayNode(int delay)
        {
            _delay = delay;
        }

        public async Task<bool> Execute(bool debugMode)
        {
            await Task.Delay(_delay);
            return true;
        }
    }
}