using UnityEngine;

namespace Game.Scripts
{
    public class Drive : MonoBehaviour
    {
        [SerializeField] [Range(0, 100)]
        private float _speed = 0;

        [SerializeField] [Range(0, 1000)]
        private float _rotationSpeed = 0;

        void Update()
        {
            // Get the horizontal and vertical axis.
            // By default they are mapped to the arrow keys.
            // The value is in the range -1 to 1
            var translation = Input.GetAxis("Vertical") * _speed;
            var rotation = Input.GetAxis("Horizontal") * _rotationSpeed;

            // Make it move 10 meters per second instead of 10 meters per frame...
            translation *= Time.deltaTime;
            rotation *= Time.deltaTime;

            // Move translation along the object's z-axis
            transform.Translate(0, 0, translation);

            // Rotate around our y-axis
            transform.Rotate(0, rotation, 0);
        }
    }
}
