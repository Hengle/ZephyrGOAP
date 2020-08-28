using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Zephyr.GOAP.Component;
using Zephyr.GOAP.Component.AgentState;
using Zephyr.GOAP.Sample.Game.Component;
using Zephyr.GOAP.Sample.Game.Component.Order;
using Zephyr.GOAP.Sample.GoapImplement.Component.Action;
using Zephyr.GOAP.System;

namespace Zephyr.GOAP.Sample.GoapImplement.System.ActionExecuteSystem
{
    public class PickRawActionExecuteSystem : ActionExecuteSystemBase
    {
        protected override FixedString32 GetNameOfAction()
        {
            return nameof(PickRawAction);
        }

        protected override JobHandle ExecuteActionJob(FixedString32 nameOfAction,
            NativeArray<Entity> waitingNodeEntities,
            NativeArray<Node> waitingNodes, BufferFromEntity<State> waitingStates,
            EntityCommandBuffer.ParallelWriter ecb, JobHandle inputDeps)
        {
            return Entities.WithName("PickRawActionExecuteJob")
                .WithAll<Agent>()
                .WithAll<PickRawAction>()
                .WithAll<ReadyToAct>()
                .WithDisposeOnCompletion(waitingNodeEntities)
                .WithDisposeOnCompletion(waitingNodes)
                .WithReadOnly(waitingStates)
                .ForEach((Entity agentEntity, int entityInQueryIndex) =>
                {
                    for (var nodeId = 0; nodeId < waitingNodeEntities.Length; nodeId++)
                    {
                        var nodeEntity = waitingNodeEntities[nodeId];
                        var node = waitingNodes[nodeId];

                        if (!node.AgentExecutorEntity.Equals(agentEntity)) continue;
                        if (!node.Name.Equals(nameOfAction)) continue;

                        var states = waitingStates[nodeEntity];
                        //从precondition里找信息.
                        var rawEntity = Entity.Null;
                        var rawItemName = new FixedString32();
                        var rawAmount = 0;
                        for (var stateId = 0; stateId < states.Length; stateId++)
                        {
                            if ((node.PreconditionsBitmask & (ulong) 1 << stateId) <= 0) continue;
                            var precondition = states[stateId];
                            Assert.IsTrue(precondition.Target != Entity.Null);
                            
                            rawEntity = precondition.Target;
                            rawItemName = precondition.ValueString;
                            rawAmount = precondition.Amount;
                            break;
                        }
                        
                        //产生order
                        OrderWatchSystem.CreateOrderAndWatch<PickRawOrder>(ecb, entityInQueryIndex, agentEntity,
                            rawEntity, rawItemName, rawAmount, nodeEntity);
                        
                        //进入执行中状态
                        Zephyr.GOAP.Utils.NextAgentState<ReadyToAct, Acting>(agentEntity, entityInQueryIndex,
                            ecb, nodeEntity);
                        
                        break;
                    }
                }).Schedule(inputDeps);
        }
    }
}