using Unity.Collections;
using Unity.Entities;
using Zephyr.GOAP.Action;
using Zephyr.GOAP.Struct;

namespace Zephyr.GOAP.Test.Mock
{
    public struct MockAction : IComponentData, IAction
    {
        public NativeString64 GetName()
        {
            return "MockAction";
        }

        public State GetTargetGoalState(ref StateGroup targetStates, ref StackData stackData)
        {
            return default;
        }

        public StateGroup GetSettings(ref State targetState, ref StackData stackData, Allocator allocator)
        {
            return new StateGroup(1, allocator);
        }

        public void GetPreconditions(ref State targetState, ref State setting, ref StackData stackData,
            ref StateGroup preconditions)
        {
            
        }

        public void GetEffects(ref State targetState, ref State setting, ref StackData stackData,
            ref StateGroup effects)
        {
            
        }

        public float GetReward(ref State targetState, ref State setting, ref StackData stackData)
        {
            return 0;
        }

        public float GetExecuteTime(ref State targetState, ref State setting, ref StackData stackData)
        {
            return 0;
        }

        public void GetNavigatingSubjectInfo(ref State targetState, ref State setting, ref StackData stackData,
            ref StateGroup preconditions, out NodeNavigatingSubjectType subjectType, out byte subjectId)
        {
            subjectType = NodeNavigatingSubjectType.Null;
            subjectId = 0;
        }
    }
}