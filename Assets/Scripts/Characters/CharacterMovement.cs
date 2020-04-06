using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Gfx")] public RuntimeAnimatorController NoCar;
        public RuntimeAnimatorController WithCar;

        [Header("Persecution")] public Image PersecutionImage;
        public Color ColorRed;
        public Color ColorBlue;

        [Header("Movement")] public float currentSpeed;
        public float _normalSpeed;
        public float _sprintSpeed;

        private Vector2 _velocity;
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private bool inPersecution = false;
        private float vanishSpeed = 5f;
        
        public float redColorTime = 0f;
        public float BlueColorTime = 0f;

        private bool redTime = false;
        private bool blueTime = false;

        private void Start()
        {
            currentSpeed = _normalSpeed;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();

            if (GameControl.instance.Charity)
            {
                _animator.runtimeAnimatorController = NoCar;
            }
            else
            {
                _animator.runtimeAnimatorController = WithCar;
            }
        }

        public void StartPersecution()
        {
            redTime = true;
            inPersecution = true;
        }

        public void StopPersecution()
        {
            inPersecution = false;
            PersecutionImage.color = Color.clear;
        }

        private void Update()
        {
            _animator.SetBool("Running", Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
            currentSpeed = Input.GetButton("Sprint") ? _sprintSpeed : _normalSpeed;
            _velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized *
                        currentSpeed;
        }

        // Start is called before the first frame update
        private void FixedUpdate()
        {
            
            if (inPersecution)
            {
                if (redColorTime <= 0)
                {
                    Debug.Log("rojo");
                    redColorTime = 0.25f;
                    redTime = true;
                    blueTime = false;
                    PersecutionImage.color = ColorRed;
                }
                else if(BlueColorTime <= 0)
                {
                    Debug.Log("azul");
                    BlueColorTime = 0.25f;
                    blueTime = true;
                    redTime = false;
                    PersecutionImage.color = ColorBlue;
                }

                if (redTime)
                {
                    BlueColorTime -= Time.fixedDeltaTime;
                }
                else
                {
                    redColorTime -= Time.fixedDeltaTime;
                }

            }
            
            _rigidbody2D.MovePosition(_rigidbody2D.position + _velocity * Time.fixedDeltaTime);
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                float angle = Mathf.Atan2(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg;
                _rigidbody2D.rotation = angle;
            }
        }
    }
}