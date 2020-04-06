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

        [Header("Sound")] 
        public AudioClip walkSound;
        public AudioClip walkWithCar;

        private Vector2 _velocity;
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private bool inPersecution = false;
        private AudioSource sfx;
        private float redColorTime = 0f;
        private float BlueColorTime = 0f;

        private bool changeColorTime = false;

        private void Start()
        {
            sfx = GetComponent<AudioSource>();
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
            SoundManager.instance.PlayBackground(SoundManager.instance.persecutionBackground);
            changeColorTime = true;
            inPersecution = true;
        }

        public void StopPersecution()
        {
            if (GameControl.instance.Charity)
            {
                SoundManager.instance.PlayBackground(SoundManager.instance.sadBackground);
            }
            else
            {
                SoundManager.instance.PlayBackground(SoundManager.instance.gameplayBackground);
            }

            inPersecution = false;
            PersecutionImage.color = Color.clear;
        }

        private void Update()
        {
            _animator.SetBool("Running", Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);

            if (_animator.GetBool("Running"))
            {
                if (!sfx.isPlaying)
                {
                    playSteps();
                }
            }else if (sfx.isPlaying)
            {
                sfx.Stop();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.instance.PauseGame();
            }

            currentSpeed = Input.GetButton("Sprint") ? _sprintSpeed : _normalSpeed;
            _velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized *
                        currentSpeed;
        }

        private void playSteps()
        {
            sfx.clip = null;
            if (GameControl.instance.Charity)
            {
                sfx.clip = walkSound;
            }
            else
            {
                sfx.clip = walkWithCar;
            }
            sfx.volume = 0.4f;
            sfx.Play();
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
                    changeColorTime = true;
                    PersecutionImage.color = ColorRed;
                }
                else if (BlueColorTime <= 0)
                {
                    Debug.Log("azul");
                    BlueColorTime = 0.25f;
                    changeColorTime = false;
                    PersecutionImage.color = ColorBlue;
                }

                if (changeColorTime)
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