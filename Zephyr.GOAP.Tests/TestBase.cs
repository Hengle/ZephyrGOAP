using NUnit.Framework;
using Unity.Entities;

namespace Zephyr.GOAP.Test
{
    public class TestBase
    {
        protected static World World;
        protected EntityManager EntityManager;
        protected EntityManager.EntityManagerDebug ManagerDebug;
    
        [SetUp]
        public virtual void SetUp()
        {
            World = new World("Test World");

            EntityManager = World.EntityManager;
            ManagerDebug = new EntityManager.EntityManagerDebug(EntityManager);
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (EntityManager != null)
            {
                World.Dispose();
                EntityManager = null;
            }
        }
    }
}