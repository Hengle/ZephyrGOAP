using Classic.AI.GOAP;
using UnityEngine;

namespace Classic.Game.Action
{
    public class DropOffOreAction : GoapAction
    {
        private bool _droppedOffOre = false;
        private SupplyPileComponent _targetSupplyPile;

        public DropOffOreAction()
        {
            AddPrecondition("hasOre", true);
            AddEffect("hasOre", false);
            AddEffect("collectOre", true);
        }
        
        public override void Reset()
        {
            _droppedOffOre = false;
            _targetSupplyPile = null;
        }

        public override bool IsDone()
        {
            return _droppedOffOre;
        }

        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            var supplyPiles = FindObjectsOfType<SupplyPileComponent>();
            SupplyPileComponent closest = null;
            float closestDist = 0;

            foreach (var pile in supplyPiles)
            {
                var distance = Vector3.Distance(
                    transform.position, pile.transform.position);
                if (closest == null)
                {
                    closest = pile;
                    closestDist = distance;
                }else if (distance < closestDist)
                {
                    closest = pile;
                    closestDist = distance;
                }
            }

            if (closest == null) return false;

            _targetSupplyPile = closest;
            target = _targetSupplyPile.gameObject;

            return closest != null;
        }

        public override bool Perform(GameObject agent)
        {
            var backpack = agent.GetComponent<BackpackComponent>();
            _targetSupplyPile.numOre += backpack.numOre;
            _droppedOffOre = true;
            backpack.numOre = 0;

            return true;
        }

        public override bool RequiresInRange()
        {
            return true;
        }
    }
}