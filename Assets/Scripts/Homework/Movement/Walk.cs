using UnityEngine;

namespace Homework.Movement
{
    public class Walk : Move
    {
        private readonly float _minX;
        private readonly float _maxX;
        private readonly float _speed;
        private int _direction = 1;


        public Walk(MonoBehaviour owner) : base(owner)
        {
        }

        public Walk(MonoBehaviour owner, float minX, float maxX, float speed) : base(owner)
        {
            _minX = minX;
            _maxX = maxX;
            _speed = speed;
        }

        public override void Execute()
        {
            var newPosition = Owner.transform.position;
            newPosition.x += _direction * Time.deltaTime * _speed;

            Owner.transform.position = newPosition;

            if (newPosition.x > _maxX)
                _direction = -1;
            else if (newPosition.x < _minX)
                _direction = 1;
        }
    }
}