using UnityEngine;

namespace Commons
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityModifier : MonoBehaviour
    {
        [SerializeField] private float _gravityMultiplier = 1f;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }

        private void FixedUpdate()
        {
            Vector3 gravity = Physics.gravity * _gravityMultiplier;
            _rigidbody.AddForce(gravity, ForceMode.Acceleration);
        }
    }
}
