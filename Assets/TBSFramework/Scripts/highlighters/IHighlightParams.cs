namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// An interface representing the parameters required for applying highlight effects.
    /// Implementations of this interface define the data needed to customize highlight behavior for units or cells.
    /// </summary>
    public interface IHighlightParams
    {
    }

    public readonly struct NoParam : IHighlightParams
    {
        public static readonly IHighlightParams Instance = NoParam.Instance;
    }
}
