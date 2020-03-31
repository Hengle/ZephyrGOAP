using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;
using Zephyr.GOAP.Component.Trait;
using Zephyr.GOAP.Game.ComponentData;

namespace Zephyr.GOAP.Game.Authoring
{
    [RequiresEntityConversion]
    [ConverterVersion("Zephyr", 1)]
    public class CampfireAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public string Name;
        [ValueDropdown("OutputNames")]
        public string[] Outputs;
        
        private static string[] OutputNames = { "roast_apple", "roast_peach" };
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
#if UNITY_EDITOR
            dstManager.SetName(entity, Name);
#endif
            dstManager.AddComponentData(entity, new CookerTrait());
            var buffer = dstManager.AddBuffer<ContainedOutput>(entity);
            foreach (var output in Outputs)
            {
                buffer.Add(new ContainedOutput {ItemOutput = output});
            }
            dstManager.AddComponentData(entity, new ItemContainer{IsTransferSource = true});
            dstManager.AddBuffer<ContainedItemRef>(entity);
        }
    }
}