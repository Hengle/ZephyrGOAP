using DOTS.Struct;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace DOTS.Action
{
    public class ActionScheduler
    {
        // Input
        [ReadOnly]
        public NativeList<Node> UnexpandedNodes;

        [ReadOnly]
        public StackData StackData;
        
        //Output
        public NodeGraph NodeGraph;

        public NativeList<Node> NewlyExpandedNodes;

        public int Iteration;

        public JobHandle Schedule(JobHandle inputDeps)
        {
            var entityManager = World.Active.EntityManager;
            
            if (entityManager.HasComponent<DropItemAction>(StackData.AgentEntity))
            {
                inputDeps = new DropItemActionJob(ref UnexpandedNodes, ref StackData,
                    ref NodeGraph, ref NewlyExpandedNodes, Iteration).Schedule(
                    UnexpandedNodes, 0, inputDeps);
            }

            if (entityManager.HasComponent<PickItemAction>(StackData.AgentEntity))
            {
                inputDeps = new PickItemActionJob(ref UnexpandedNodes, ref StackData,
                    ref NodeGraph, ref NewlyExpandedNodes, Iteration).Schedule(
                    UnexpandedNodes, 0, inputDeps);
            }

            return inputDeps;
        }
    }
}