using System;
using UnityEngine;
using Pathfinding;
using Random = UnityEngine.Random;

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
        public float speed;
        public float startWaitTime;
        public Transform[] moveSpots;
        public float nextWayPointDistance = 3f;
        public Seeker seeker;
        public Rigidbody2D rigidbody2D;
        public Transform GFX;

        private bool searching = false;
        private int currentWayPoint;
        private Path path;
        private float waitTime;
        private int randomSpot;

        private void Start()
        {
            Physics2D.queriesStartInColliders = false;
            waitTime = startWaitTime;
            updatePath();
        }

        void updatePath()
        {
            if (seeker.IsDone())
            {
                randomSpot = Random.Range(0, moveSpots.Length);
                seeker.StartPath(rigidbody2D.position, moveSpots[randomSpot].position, OnPathComplete);
            }
        }

        void followPlayer()
        {
            InvokeRepeating("followingPlayer",0,0.5f);
        }

        void followingPlayer()
        {
            if (seeker.IsDone())
            {
                seeker.StartPath(rigidbody2D.position, playerTarget.transform.position, OnPathComplete);
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

        void enemySearching()
        {
            if (searching)
            {
                lineOfSight.gameObject.SetActive(true);
                GFX.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

                RaycastHit2D hitInfo = Physics2D.Raycast(GFX.transform.position, GFX.transform.right, distance);

                if (hitInfo.collider != null)
                {
                    lineOfSight.SetPosition(1, hitInfo.point);
                    lineOfSight.colorGradient = redColor;
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        followPlayer();
                    }
                }
                else
                {
                    lineOfSight.SetPosition(1, GFX.transform.position + GFX.transform.right * distance);
                    lineOfSight.colorGradient = greenColor;
                    
                }

                lineOfSight.SetPosition(0, GFX.position);
            }
        }

        private void FixedUpdate()
        {
            if (path == null) return;

            if (currentWayPoint >= path.vectorPath.Count)
            {
                this.enemySearching();
                if (waitTime <= 0)
                {
                    updatePath();
                    waitTime = startWaitTime;
                    searching = false;
                    lineOfSight.gameObject.SetActive(false);
                }
                else
                {
                    waitTime -= Time.fixedDeltaTime;
                    searching = true;
                }

                return;
            }

            Vector2 direction = ((Vector2) path.vectorPath[currentWayPoint] - rigidbody2D.position).normalized * speed;
            rigidbody2D.MovePosition(rigidbody2D.position + direction * Time.fixedDeltaTime);
            float distance = Vector2.Distance(rigidbody2D.position, path.vectorPath[currentWayPoint]);

            if (distance < nextWayPointDistance)
            {
                currentWayPoint++;
            }
        }
    }
}