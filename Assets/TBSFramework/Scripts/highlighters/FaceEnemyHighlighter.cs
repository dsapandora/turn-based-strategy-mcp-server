using System.Threading.Tasks;
using TurnBasedStrategyFramework.Unity.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Highlighters
{
    /// <summary>
    /// Rotates a Transform to face an enemy unit during combat.
    /// </summary>
    public class FaceEnemyHighlighter : BaseRotationHighlighter
    {
        public override async Task Apply(IHighlightParams @params)
        {
            var combatHighlightParams = (CombatHighlightParams)@params;
            Vector3 directionToFace = (combatHighlightParams.SecondaryUnit.transform.position - _transform.position).normalized;
            await RotateTowards(directionToFace);
        }
    }
}
