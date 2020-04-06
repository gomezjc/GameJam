using System;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Characters
{
    public class PatrolPeople : MonoBehaviour
    {
        public float speed;
        public Transform[] moveSpots;
        public float nextWayPointDistance = 3f;
        public Seeker seeker;
        public Rigidbody2D rgb;
        public Transform GFX;
        public float startWaitTime;

        private float waitTime;
        private int currentWayPoint;
        private Path actualPath;
        private Path path;
        private int randomSpot;
        
        private Animator _animator;
        
        public void startPatrol()
        {
            _animator = GetComponentInChildren<Animator>();
            waitTime = startWaitTime;
            updatePath();
        }

        public void stopPath()
        {
            _animator.SetBool("Walking",false);
            actualPath = path;
            path = null;
        }

        public void continuePath()
        {
            path = actualPath;
            actualPath = null;
        }

        void updatePath()
        {
            Debug.Log("done");
            if (seeker.IsDone())
            {
                Debug.Log("done2");
                randomSpot = Random.Range(0, moveSpots.Length);
                Debug.Log(moveSpots[randomSpot].gameObject.name);
                seeker.StartPath(rgb.position, moveSpots[randomSpot].position, OnPathComplete);
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


        private void FixedUpdate()
        {
            if (path == null) return;

            if (currentWayPoint >= path.vectorPath.Count)
            {
                if (waitTime <= 0)
                {
                    updatePath();
                    waitTime = startWaitTime;
                }
                else
                {
                    _animator.SetBool("Walking",false);
                    waitTime -= Time.fixedDeltaTime;
                }

                return;
            }

            _animator.SetBool("Walking",true);
            Vector2 direction = ((Vector2) path.vectorPath[currentWayPoint] - rgb.position).normalized * speed;
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