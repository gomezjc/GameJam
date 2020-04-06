using Pathfinding;
using UnityEngine;

namespace Characters
{
    public class Patrol : MonoBehaviour
    {
        public float rotationSpeed;
        public float distance;
        public LineRenderer lineOfSight;
        public Gradient redColor;
        public Gradient greenColor;
        public Transform playerTarget;

        public float currentSpeed;
        public float speed;
        public float PersecutionSpeed;

        public float startWaitTime;
        public float startFollowTime;
        public Transform[] moveSpots;
        public float nextWayPointDistance = 3f;
        public Seeker seeker;
        public Rigidbody2D rgb;
        public Transform GFX;

        private bool following = false;
        private int currentWayPoint;
        private Path path;
        private float waitTime;
        private int randomSpot;
        public float followingTime;

        public bool canFollow;

        private Animator _animator;
        private CharacterMovement _characterMovement;

        public void StartPatrol()
        {
            //Debug.Log("start patrol");
            _animator = GetComponentInChildren<Animator>();
            currentSpeed = speed;
            canFollow = !GameControl.instance.Charity;
            Physics2D.queriesStartInColliders = false;
            waitTime = startWaitTime;
            followingTime = startFollowTime;
            updatePath();
        }

        void updatePath()
        {
            if (seeker.IsDone())
            {
                randomSpot = Random.Range(0, moveSpots.Length);
                seeker.StartPath(rgb.position, moveSpots[randomSpot].position, OnPathComplete);
            }
        }

        void followPlayer()
        {
            _characterMovement = playerTarget.GetComponent<CharacterMovement>();
            _characterMovement.StartPersecution();
            following = true;
            currentSpeed = PersecutionSpeed;
            InvokeRepeating("followingPlayer", 0, 0.5f);
        }

        void followingPlayer()
        {
            if (seeker.IsDone())
            {
                seeker.StartPath(rgb.position, playerTarget.transform.position, OnPathComplete);
            }
        }

        void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWayPoint = 0;
            }
        }

        public void cancelFollow()
        {
            _characterMovement.StopPersecution();
            _characterMovement = null;
            currentSpeed = speed;
            following = false;
            followingTime = startFollowTime;
            CancelInvoke("followingPlayer");
            updatePath();
        }

        void enemySearching()
        {
            if (following)
            {
                if (followingTime <= 0)
                {
                    cancelFollow();
                }

                followingTime -= Time.fixedDeltaTime;
            }

            RaycastHit2D hitInfo = Physics2D.Raycast(GFX.transform.position, GFX.transform.right, distance);
            if (hitInfo.collider != null)
            {
                lineOfSight.SetPosition(1, hitInfo.point);
                lineOfSight.colorGradient = redColor;
                if (hitInfo.collider.CompareTag("Player") && canFollow)
                {
                    followingTime = startFollowTime;
                    if (!following)
                    {
                        GameManager.instance.SetInteractText("La tomba!! corre!!!", true, 1);
                        followPlayer();
                    }
                }
            }
            else
            {
                lineOfSight.SetPosition(1, GFX.transform.position + GFX.transform.right * distance);
                lineOfSight.colorGradient = greenColor;
            }

            lineOfSight.SetPosition(0, GFX.position);
        }

        private void FixedUpdate()
        {
            if (path == null) return;

            this.enemySearching();
            if (currentWayPoint >= path.vectorPath.Count)
            {
                if (waitTime <= 0)
                {
                    updatePath();
                    waitTime = startWaitTime;
                }
                else
                {
                    _animator.SetBool("Walking", false);
                    GFX.transform.Rotate(Vector3.forward * rotationSpeed * Time.fixedDeltaTime);
                    waitTime -= Time.fixedDeltaTime;
                }

                return;
            }

            _animator.SetBool("Walking", true);
            Vector2 direction = ((Vector2) path.vectorPath[currentWayPoint] - rgb.position).normalized * currentSpeed;
            rgb.MovePosition(rgb.position + direction * Time.fixedDeltaTime);
            float distance = Vector2.Distance(rgb.position, path.vectorPath[currentWayPoint]);

            Vector3 dir = (Vector2) path.vectorPath[currentWayPoint] - rgb.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            GFX.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (distance < nextWayPointDistance)
            {
                currentWayPoint++;
            }
        }
    }
}