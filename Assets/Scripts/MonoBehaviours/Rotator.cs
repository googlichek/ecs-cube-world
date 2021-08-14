using UnityEngine;

namespace Game.Scripts
{
    public class Rotator : MonoBehaviour
    {
        private const float RotationSpeed = 25;

        private Quaternion _initialLocalRotaion = Quaternion.identity;

        void Awake()
        {
            _initialLocalRotaion = transform.localRotation;
        }

        void OnEnable()
        {
            transform.localRotation = _initialLocalRotaion;
        }

        void Update()
        {
            var rotation = transform.localRotation.eulerAngles.y - RotationSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.AngleAxis(rotation, Vector3.up);
        }
    }
}
