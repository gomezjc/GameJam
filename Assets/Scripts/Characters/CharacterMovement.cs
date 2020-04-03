using UnityEngine;

namespace Characters
{
    public class CharacterMovement : MonoBehaviour
    {
        public float currentSpeed;
        public float _normalSpeed;
        public float _sprintSpeed;
        private Vector2 _velocity;
        private Rigidbody2D _rigidbody2D;
        private void Start()
        {
            currentSpeed = _normalSpeed;
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            currentSpeed = Input.GetButton("Sprint") ? _sprintSpeed : _normalSpeed;
            _velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * currentSpeed;
        }

        // Start is called before the first frame update
        private void FixedUpdate()
        {
            _rigidbody2D.MovePosition(_rigidbody2D.position + _velocity * Time.fixedDeltaTime);
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                float angle = Mathf.Atan2(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg;
                _rigidbody2D.rotation = angle;
            }
        }
    }
}