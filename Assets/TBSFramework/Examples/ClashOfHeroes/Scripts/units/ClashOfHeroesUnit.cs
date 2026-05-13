using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Cells;
using TurnBasedStrategyFramework.Unity.Units;
using TurnBasedStrategyFramework.Unity.Utilities;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.Units
{
    /// <summary>
    /// Represents a unit in the Clash of Heroes demo
    /// </summary>
    public class ClashOfHeroesUnit : Unit, IUnitDetails, ITurnAbilityLimit
    {
        [SerializeField] private Transform _unitModel;
        [SerializeField] private string _unitName;
        [SerializeField] private Sprite _unitPortrait;
        [SerializeField] private int _maxAbilityUsesPerTurn;

        [SerializeField] private ScriptableObject _waterCellType;

        public string UnitName { get => _unitName; set => _unitName = value; }
        public Sprite UnitPortrait { get => _unitPortrait; set => _unitPortrait = value; }
        public int AbilityUsePoints { get; set; }

        public override bool IsCellTraversable(ICell source, ICell destination)
        {
            var sourceHeight = (source as Cell).GetComponent<IHeightComponent>().Height;
            var destinationHeight = (destination as Cell).GetComponent<IHeightComponent>().Height;

            var destinationType = (destination as Cell).GetComponent<ITypedCell>().CellType;

            return base.IsCellTraversable(source, destination) && (sourceHeight == destinationHeight || (Math.Abs(sourceHeight - destinationHeight) == 1)) && destinationType != _waterCellType;
        }

        public override bool IsCellMovableTo(ICell cell)
        {
            return base.IsCellMovableTo(cell) && !(cell as ITypedCell).CellType.Equals(_waterCellType);
        }

        public override bool IsUnitAttackable(IUnit otherUnit, ICell otherUnitCell, ICell attackSourceCell)
        {
            var attackSourceCellHeight = (attackSourceCell as Cell).GetComponent<IHeightComponent>().Height;
            var otherUnitCellHeight = (otherUnitCell as Cell).GetComponent<IHeightComponent>().Height;

            var isRangedAttack = AttackRange > 1;

            return base.IsUnitAttackable(otherUnit, otherUnitCell, attackSourceCell) && (isRangedAttack || Math.Abs(otherUnitCellHeight - attackSourceCellHeight) <= 1);
        }
        public override float CalculateDamageTaken(IUnit aggressor, float damageDealt, ICell aggressorCell, ICell defenderCell)
        {
            var agressorCellHeight = (aggressorCell as Cell).GetComponent<IHeightComponent>().Height;
            var defenderCellHeight = (defenderCell as Cell).GetComponent<IHeightComponent>().Height;

            return agressorCellHeight > defenderCellHeight ? (damageDealt * 2) - DefenceFactor : base.CalculateDamageTaken(aggressor, damageDealt, aggressorCell, defenderCell);
        }

        public override async Task MovementAnimation(IEnumerable<ICell> path, ICell destination)
        {
            var currentCell = CurrentCell;
            foreach (var cell in path)
            {
                InvokeUnitLeftCell(new UnitChangedGridPositionEventArgs(this, currentCell, cell));
                Vector3 direction = (cell.WorldPosition.ToVector3() - WorldPosition.ToVector3()).normalized;

                while (!WorldPosition.Equals(cell.WorldPosition))
                {
                    WorldPosition = Vector3.MoveTowards(
                        WorldPosition.ToVector3(),
                        cell.WorldPosition.ToVector3(),
                        Time.deltaTime * MovementAnimationSpeed
                    ).ToIVector3();

                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                        _unitModel.rotation = Quaternion.Slerp(
                            _unitModel.rotation,
                            targetRotation,
                            Time.deltaTime * MovementAnimationSpeed * 2
                        ); ;
                    }

                    await Awaitable.NextFrameAsync();
                }
                InvokeUnitEnteredCell(new UnitChangedGridPositionEventArgs(this, currentCell, cell));
            }
            WorldPosition = destination.WorldPosition;
        }

        public int GetMaxAbilityUsesPerTurn()
        {
            return _maxAbilityUsesPerTurn;
        }

        public int GetAbilityUsePoints()
        {
            return AbilityUsePoints;
        }

        public override void OnTurnStart(IGridController gridController)
        {
            base.OnTurnStart(gridController);
            AbilityUsePoints = _maxAbilityUsesPerTurn;
        }
    }
}