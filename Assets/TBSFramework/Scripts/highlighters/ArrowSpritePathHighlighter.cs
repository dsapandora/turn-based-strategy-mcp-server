using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Utilities;
using TurnBasedStrategyFramework.Unity.Cells;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// A highlighter that visualize a path using directional arrow and line sprites.
    /// Depending on the position in the path, the appropriate arrowhead or segment is selected to visually represent the direction of movement.
    /// </summary>
    public class ArrowSpritePathHighlighter : Highlighter
    {
        /// <summary>
        /// Sprites representing the various arrow directions and path segments.
        /// </summary>
        [SerializeField] SpriteRenderer _arrowLeft;
        [SerializeField] SpriteRenderer _arrowRight;
        [SerializeField] SpriteRenderer _arrowUp;
        [SerializeField] SpriteRenderer _arrowDown;
        [SerializeField] SpriteRenderer _lineHorizontal;
        [SerializeField] SpriteRenderer _lineVertical;
        [SerializeField] SpriteRenderer _curlUpperRight;
        [SerializeField] SpriteRenderer _curlLowerRight;
        [SerializeField] SpriteRenderer _curlLowerLeft;
        [SerializeField] SpriteRenderer _curlUpperLeft;

        /// <summary>
        /// Applies the highlight to the path using directional arrow and line sprites based on the position of the cell in the path.
        /// </summary>
        /// <param name="target">The target node (not used in this implementation).</param>
        /// <param name="params">The parameters used to define the path and its segments.</param>
        /// <returns>A completed task representing the synchronous application of the highlight.</returns>
        public override Task Apply(IHighlightParams @params)
        {
            var pathHighlightParams = (PathHighlightParams)@params;

            var path = pathHighlightParams.Path;
            var index = pathHighlightParams.CellIndex;
            var originCell = pathHighlightParams.OriginCell;
            var currentPosition = path[index].GridCoordinates;

            Vector2IntImpl prevPosition = index > 0 ? path[index - 1].GridCoordinates : originCell.GridCoordinates;
            Vector2IntImpl nextPosition = index < path.Count - 1 ? path[index + 1].GridCoordinates : currentPosition;

            SpriteRenderer selectedSprite = index == path.Count - 1
                ? GetArrowHeadSprite(prevPosition, currentPosition)
                : GetArrowSegmentSprite(prevPosition, currentPosition, nextPosition);

            selectedSprite.gameObject.SetActive(true);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Determines the appropriate arrow segment sprite to use for a given segment of the path based on the relative positions of the previous, current, and next cells.
        /// </summary>
        /// <param name="origin">The origin point of the previous cell.</param>
        /// <param name="first">The current cell position.</param>
        /// <param name="second">The next cell position.</param>
        /// <returns>The sprite corresponding to the correct directional segment of the path.</returns>
        private SpriteRenderer GetArrowSegmentSprite(Vector2IntImpl origin, Vector2IntImpl first, Vector2IntImpl second)
        {
            if (second.y == origin.y)
                return _lineHorizontal;
            if (second.x == origin.x)
                return _lineVertical;

            if (origin.y > second.y)
            {
                return origin.x > second.x
                    ? (origin.y != first.y ? _curlLowerRight : _curlUpperLeft)
                    : (origin.y != first.y ? _curlLowerLeft : _curlUpperRight);
            }
            else
            {
                return origin.x > second.x
                    ? (origin.y != first.y ? _curlUpperRight : _curlLowerLeft)
                    : (origin.y != first.y ? _curlUpperLeft : _curlLowerRight);
            }
        }

        /// <summary>
        /// Determines the appropriate arrowhead sprite to use at the end of the path based on the direction between the last two cells.
        /// </summary>
        /// <param name="from">The position of the second-to-last cell in the path.</param>
        /// <param name="to">The position of the last cell in the path.</param>
        /// <returns>The sprite corresponding to the correct arrowhead direction.</returns>
        private SpriteRenderer GetArrowHeadSprite(Vector2IntImpl from, Vector2IntImpl to)
        {
            if (to.x != from.x)
                return to.x < from.x ? _arrowLeft : _arrowRight;
            return to.y <= from.y ? _arrowDown : _arrowUp;
        }
    }
}