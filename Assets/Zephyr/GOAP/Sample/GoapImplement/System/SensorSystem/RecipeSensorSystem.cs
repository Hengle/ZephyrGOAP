using Unity.Collections;
using Unity.Entities;
using Zephyr.GOAP.Component;
using Zephyr.GOAP.Component.Trait;
using Zephyr.GOAP.Struct;
using Zephyr.GOAP.System;

namespace Zephyr.GOAP.Sample.GoapImplement.System.SensorSystem
{
    /// <summary>
    /// 检测所有Recipe并存入CurrentState
    /// </summary>
    [UpdateInGroup(typeof(SensorSystemGroup))]
    public class RecipeSensorSystem : ComponentSystem
    {
        
        protected override void OnUpdate()
        {
            if (!EntityManager.Exists(CurrentStatesHelper.CurrentStatesEntity)) return;
            
            //范例只有一个recipe，直接手写，实际需要从define里批量写入
            var buffer = EntityManager.GetBuffer<State>(CurrentStatesHelper.CurrentStatesEntity);
            //存储recipe这样复杂state的折中方案：
            //每个recipe以1个output+2个input的方式保存，占用连续的3个state
            //对于不需要第二个原料的recipe,其第二个input为空state
            AddRecipeState(buffer, Sample.Utils.RoastPeachName, Sample.Utils.RawPeachName);
            AddRecipeState(buffer, "roast_apple", "raw_apple");
            AddRecipeState(buffer, "feast", "raw_apple", Sample.Utils.RawPeachName);
        }

        /// <summary>
        /// 存储recipe这样复杂state的折中方案
        /// 每个recipe以1个output+2个input的方式保存，占用连续的3个state
        /// 对于不需要第二个原料的recipe,其第二个input为空state
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="output"></param>
        /// <param name="input1"></param>
        /// <param name="input2"></param>
        private void AddRecipeState(DynamicBuffer<State> buffer, NativeString32 output,
            NativeString32 input1, NativeString32 input2 = default)
        {
            buffer.Add(new State
            {
                Trait = typeof(RecipeOutputTrait),
                ValueTrait = typeof(CookerTrait),    //以ValueTrait保存此recipe适用的生产设施
                ValueString = output,
            });
            buffer.Add(new State
            {
                Trait = typeof(RecipeInputTrait),
                ValueTrait = typeof(CookerTrait),
                ValueString = input1,
            });
            if(input2.Equals(default))
            {
                buffer.Add(State.Null);
            }else
            {
                buffer.Add(new State{
                    Trait = typeof(RecipeInputTrait),
                    ValueTrait = typeof(CookerTrait),
                    ValueString = input2,
                });
            }
        }
    }
}