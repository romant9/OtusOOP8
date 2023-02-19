using UnityEngine;

namespace Homework.Movement
{
    public abstract class Move
    {
        protected MonoBehaviour Owner;

        protected Move(MonoBehaviour owner)
        {
            Owner = owner;
        }

        public abstract void Execute();
    }
}