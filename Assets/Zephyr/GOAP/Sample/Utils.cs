using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Zephyr.GOAP.Component;
using Zephyr.GOAP.Sample.GoapImplement;
using Zephyr.GOAP.Sample.GoapImplement.Component.Trait;
using Zephyr.GOAP.Struct;

namespace Zephyr.GOAP.Sample
{
    public static class Utils
    {
        /// <summary>
        /// 根据传入的配方输出筛选，传出其对应的输入State组
        /// 能根据输出的数量要求给出成倍计算后的输入数量，如果出现配方产量超过需求，就产生富余
        /// </summary>
        /// <param name="stateGroup"></param>
        /// <param name="recipeOutFilter"></param>
        /// <param name="allocator"></param>
        /// <returns></returns>
        public static StateGroup GetRecipeInputInStateGroup(StateGroup stateGroup, State recipeOutFilter, Allocator allocator)
        {
            
            var result = new StateGroup(2, allocator);
            for (var i = 0; i < stateGroup.Length(); i++)
            {
                if (stateGroup[i].BelongTo(recipeOutFilter))
                {
                    
                    var multiply = math.ceil((float)recipeOutFilter.Amount / stateGroup[i].Amount);
                    var input1 = stateGroup[i + 1];
                    var input2 = stateGroup[i + 2];
                    input1.Amount *= (byte) multiply;
                    input2.Amount *= (byte) multiply;
                    result.Add(input1);
                    result.Add(input2);
                    break;
                }
            }

            return result;
        }
        
        /// <summary>
        /// 示例用的方法，根据trait获取所有对应物品名，实际应从define获取
        /// </summary>
        /// <param name="trait"></param>
        /// <param name="allocator"></param>
        /// <returns></returns>
        public static NativeList<NativeString32> GetItemNamesOfSpecificTrait(ComponentType trait,
            Allocator allocator)
        {
            var result = new NativeList<NativeString32>(allocator);
            
            if (trait.Equals(ComponentType.ReadOnly<FoodTrait>()))
            {
                result.Add(StringTable.Instance().RawPeachName);
                result.Add(StringTable.Instance().RoastPeachName);
                result.Add(StringTable.Instance().RawAppleName);
                result.Add(StringTable.Instance().RoastAppleName);
                result.Add(StringTable.Instance().FeastName);
            }

            return result;
        }

        public const float RawPeachStamina = 0.2f;
        public const float RawAppleStamina = 0.3f;
        public const float RoastPeachStamina = 0.4f;
        public const float RoastAppleStamina = 0.5f;
        public const float FeastStamina = 2;
        
        /// <summary>
        /// 示例用的方法，获取不同食物的食用reward
        /// </summary>
        /// <param name="foodName"></param>
        /// <returns></returns>
        public static float GetFoodReward(NativeString32 foodName)
        {
            var plus = 10;
            if (foodName.Equals(StringTable.Instance().RawPeachName))
            {
                return RawPeachStamina*plus;
            }
            if (foodName.Equals(StringTable.Instance().RoastPeachName))
            {
                return RoastPeachStamina*plus;
            }
            if (foodName.Equals(StringTable.Instance().RawAppleName))
            {
                return RawAppleStamina*plus;
            }
            if (foodName.Equals(StringTable.Instance().RoastAppleName))
            {
                return RoastAppleStamina*plus;
            }
            if (foodName.Equals(StringTable.Instance().FeastName))
            {
                return FeastStamina*plus;
            }
            return 0;
        }
        
        public static float GetFoodStamina(NativeString32 foodName)
        {
            if (foodName.Equals(StringTable.Instance().RawPeachName))
            {
                return RawPeachStamina;
            }
            if (foodName.Equals(StringTable.Instance().RoastPeachName))
            {
                return RoastPeachStamina;
            }
            if (foodName.Equals(StringTable.Instance().RawAppleName))
            {
                return RawAppleStamina;
            }
            if (foodName.Equals(StringTable.Instance().RoastAppleName))
            {
                return RoastAppleStamina;
            }
            if (foodName.Equals(StringTable.Instance().FeastName))
            {
                return FeastStamina;
            }

            return 0;
        }
    }
}