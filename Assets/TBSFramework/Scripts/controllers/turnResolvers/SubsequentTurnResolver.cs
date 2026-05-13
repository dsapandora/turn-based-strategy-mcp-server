using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Controllers.TurnResolvers;

namespace TurnBasedStrategyFramework.Unity.Controllers
{
    /// <summary>
    /// A concrete implementation of <see cref="UnityTurnResolver"/> that delegates turn resolution to <see cref="SubsequentTurnResolverImpl"/>.
    /// This resolver handles turns sequentially for all players, selecting the first player at the start and moving through players in order.
    /// </summary>
    public class SubsequentTurnResolver : UnityTurnResolver
    {
        public SubsequentTurnResolverImpl subsequentTurnResolverImpl = new SubsequentTurnResolverImpl();
        public override TurnContext ResolveStart(GridController gridController)
        {
            return subsequentTurnResolverImpl.ResolveStart(gridController);
        }

        public override TurnContext ResolveTurn(GridController gridController)
        {
            return subsequentTurnResolverImpl.ResolveTurn(gridController);
        }
    }
}