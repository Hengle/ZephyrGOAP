using NUnit.Framework;
using Unity.Entities;
using Zephyr.GOAP.Component;
using Zephyr.GOAP.Component.AgentState;
using Zephyr.GOAP.Sample.Game.Component;
using Zephyr.GOAP.Sample.Game.Component.Order;
using Zephyr.GOAP.Sample.GoapImplement.Component.Action;
using Zephyr.GOAP.Sample.GoapImplement.Component.Trait;
using Zephyr.GOAP.Sample.GoapImplement.System.ActionExecuteSystem;
using Zephyr.GOAP.Struct;
using Zephyr.GOAP.Tests;

namespace Zephyr.GOAP.Sample.Tests.ActionExecute
{
    public class TestPickItemActionExecuteSystem : TestActionExecuteBase
    {
        private PickItemActionExecuteSystem _system;
        private Entity _containerEntity;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _system = World.GetOrCreateSystem<PickItemActionExecuteSystem>();
            
            _containerEntity = EntityManager.CreateEntity();
            
            //container预先存好物品
            var itemBuffer = EntityManager.AddBuffer<ContainedItemRef>(_containerEntity);
            itemBuffer.Add(new ContainedItemRef
            {
                ItemName = "item",
                ItemEntity = new Entity {Index = 99, Version = 9}
            });
            
            EntityManager.AddComponentData(_agentEntity, new PickItemAction());
            EntityManager.AddBuffer<ContainedItemRef>(_agentEntity);
            
            //任务
            EntityManager.AddComponentData(_actionNodeEntity, new Node
            {
                AgentExecutorEntity = _agentEntity,
                Name = nameof(PickItemAction),
                PreconditionsBitmask = 1,
                EffectsBitmask = 1 << 1,
            });
            var bufferStates = EntityManager.AddBuffer<State>(_actionNodeEntity);
            bufferStates.Add(new State
            {
                Target = _containerEntity,
                Trait = TypeManager.GetTypeIndex<ItemSourceTrait>(),
                ValueString = "item",
                Amount = 1
            });
            bufferStates.Add(new State
            {
                Target = _agentEntity,
                Trait = TypeManager.GetTypeIndex<ItemTransferTrait>(),
                ValueString = "item",
                Amount = 1
            });
        }

        [Test]
        public void CreateOrder()
        {
            _system.Update();
            _system.EcbSystem.Update();
            EntityManager.CompleteAllJobs();

            var orderQuery =
                EntityManager.CreateEntityQuery(typeof(Order), typeof(OrderWatchSystem.OrderWatch));
            Assert.AreEqual(1, orderQuery.CalculateEntityCount());
        }

        [Test]
        public void AgentState_To_Acting()
        {
            _system.Update();
            _system.EcbSystem.Update();
            EntityManager.CompleteAllJobs();
            
            Assert.IsTrue(EntityManager.HasComponent<Acting>(_agentEntity));
            Assert.IsFalse(EntityManager.HasComponent<ReadyToAct>(_agentEntity));
        }
    }
}