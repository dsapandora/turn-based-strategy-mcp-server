using System.Threading.Tasks;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// An abstract class for applying parameterized highlight effects to units or cells.
    /// </summary>
    public abstract class Highlighter : MonoBehaviour, IHighlighter
    {
        /// <summary>
        /// Asynchronously applies the highlight effect using the provided highlight parameters.
        /// </summary>
        /// <param name="@params">The parameters used to customize the highlight effect.</param>
        /// <returns>A task representing the asynchronous operation of applying the highlight effect.</returns>
        public abstract Task Apply(IHighlightParams @params);
    }

    public interface IHighlighter
    {
        Task Apply(IHighlightParams @params);
    }
}