using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.AI.BehaviourTrees;
using TurnBasedStrategyFramework.Common.AI.Evaluators;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Common.Units;
using UnityEngine;

namespace TurnBasedStrategyFramework.Unity.AI.BehaviourTrees
{
    /// <summary>
    /// A basic behavior tree that evaluates positions and enemies to guide unit movement and attack actions.
    /// </summary>
    public class RegularBehaviourTreeResource : BehaviourTreeResource
    {
        [Space]
        [Header("General")]
        /// <summary>
        /// Delay between actions in milliseconds
        /// </summary>
        [SerializeField] private int _actionDelay = 100;
        /// <summary>
        /// Delay before finishing unit's turn in milliseconds
        /// </summary>
        [SerializeField] private int _turnFinishDelay = 100;

        [Space]
        [Header("High Health Actions")]
        [SerializeField] private float _highHealthThreshold = 0.5f;

        [Space]
        [Header("High Health - Damage Dealt Position Evaluator")]
        [SerializeField] private float _highHealthDamageDealtPositionEvaluatorWeight = 1f;
        [SerializeField] private float _highHealthDamageDealtPositionEvaluatorDecay = 0.5f;

        [Space]
        [Header("High Health - Damage Received Position Evaluator")]
        [SerializeField] private float _highHealthDamageReceivedPositionEvaluatorWeight = -0.1f;
        [SerializeField] private float _highHealthDamageReceivedPositionEvaluatorDecay = 0.5f;

        [Space]
        [Header("High Health - Distance Position Evaluator")]
        [SerializeField] private float _highHealthDistancePositionEvaluatorWeight = -0.1f;
        [SerializeField] private int _highHealthDistancePositionEvaluatorThreshold = 10;

        [Space]
        [Header("Low Health - Damage Dealt Position Evaluator")]
        [SerializeField] private float _lowHealthDamageDealtPositionEvaluatorWeight = 0.9f;
        [SerializeField] private float _lowHealthDamageDealtPositionEvaluatorDecay = 0.5f;

        [Space]
        [Header("Low Health - Damage Received Position Evaluator")]
        [SerializeField] private float _lowHealthDamageReceivedPositionEvaluatorWeight = -1f;
        [SerializeField] private float _lowHealthDamageReceivedPositionEvaluatorDecay = 0.5f;

        [Space]
        [Header("Low Health - Distance Position Evaluator")]
        [SerializeField] private float _lowHealthDistancePositionEvaluatorWeight = -0.1f;
        [SerializeField] private int _lowHealthDistancePositionEvaluatorThreshold = 10;

        [Space]
        [Header("Attack Sequence")]
        [SerializeField] private float _healthTargetEvaluatorWeight = 1f;
        [SerializeField] private float _damageGivenTargetEvaluatorWeight = 1f;

        public override void Initialize(IUnit unit, IGridController gridController)
        {
            BehaviourTree = new SequenceNode(new List<ITreeNode>
            {
                new SelectorNode(new List<ITreeNode>
                {
                    new SequenceNode(new List<ITreeNode> // High health actions
                    {
                        new HealthThresholdNode(unit, _highHealthThreshold),
                        new DebugMoveAction(unit, gridController, new List<IPositionEvaluator>
                        {
                            new DamageDealtPositionEvaluator(_highHealthDamageDealtPositionEvaluatorWeight, _highHealthDamageDealtPositionEvaluatorDecay),
                            new DamageReceivedPositionEvaluator(_highHealthDamageReceivedPositionEvaluatorWeight, _highHealthDamageReceivedPositionEvaluatorDecay),
                            new DistancePositionEvaluator(_highHealthDistancePositionEvaluatorWeight, _highHealthDistancePositionEvaluatorThreshold),
                        }),
                        new SuccederNode(new MoveActionNode(unit, gridController, new List<IPositionEvaluator>
                        {
                            new DamageDealtPositionEvaluator(_highHealthDamageDealtPositionEvaluatorWeight, _highHealthDamageDealtPositionEvaluatorDecay),
                            new DamageReceivedPositionEvaluator(_highHealthDamageReceivedPositionEvaluatorWeight, _highHealthDamageReceivedPositionEvaluatorDecay),
                            new DistancePositionEvaluator(_highHealthDistancePositionEvaluatorWeight, _highHealthDistancePositionEvaluatorThreshold),
                        })),
                    }),
                    new SequenceNode(new List<ITreeNode> // Low health actions
                    {
                        new InverterNode(
                            new HealthThresholdNode(unit, _highHealthThreshold)
                        ),

                        new SuccederNode(
                            new MoveActionNode(unit, gridController, new List<IPositionEvaluator>
                            {
                                new DamageDealtPositionEvaluator(_lowHealthDamageDealtPositionEvaluatorWeight, _lowHealthDamageDealtPositionEvaluatorDecay),
                                new DamageReceivedPositionEvaluator(_lowHealthDamageReceivedPositionEvaluatorWeight, _lowHealthDamageReceivedPositionEvaluatorDecay),
                                new DistancePositionEvaluator(_lowHealthDistancePositionEvaluatorWeight, _lowHealthDistancePositionEvaluatorThreshold),
                            })
                        )
                    }),
                }), // Movement
                new RealtimeDelayNode(_actionDelay),
                new SuccederNode(new AttackSequenceNode(unit, gridController, new List<ITargetEvaluator>
                {
                    new HealthTargetEvaluator(_healthTargetEvaluatorWeight),
                    new DamageDealtTargetEvaluator(_damageGivenTargetEvaluatorWeight),
                })), // Attack
                new RealtimeDelayNode(_turnFinishDelay)
            });
        }
    }
}