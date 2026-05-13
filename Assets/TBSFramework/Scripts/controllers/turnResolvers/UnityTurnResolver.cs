using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.TurnResolvers;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Controllers
{
    /// <summary>
    /// An abstract Unity-specific implementation of <see cref="ITurnResolver"/>.
    /// </summary>
    public abstract class UnityTurnResolver : MonoBehaviour, ITurnResolver
    {
        public abstract TurnContext ResolveStart(GridController gridController);
        public abstract TurnContext ResolveTurn(GridController gridController);
    }
}