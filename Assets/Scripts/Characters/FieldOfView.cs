using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class FieldOfView : MonoBehaviour
    {
        public GameObject playerAttached;
        private Mesh mesh;
        public LayerMask blockingObjectMask;
        public LayerMask playerMask;

        private float fov;
        private float viewDistance;
        private float startingAngle;

        private Vector3 origin;

        void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            fov = 90f;
            viewDistance = 50f;
            StartCoroutine("FindTargetsWithDelay", .2f);
        }

        private void Update()
        {
            SetAimDirection(playerAttached.transform.right * -1);
            origin = playerAttached.transform.position;
        }

        private void LateUpdate()
        {
            int rayCount = 50;
            float angle = startingAngle;
            float angleIncrease = fov / rayCount;

            Vector3[] vertices = new Vector3[rayCount + 1 + 1];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[rayCount * 3];

            vertices[0] = origin;

            int vertexIndex = 1;
            int triangleIndex = 0;
            for (int i = 0; i <= rayCount; i++)
            {
                Vector3 vertex;
                RaycastHit2D raycastHit2D =
                    Physics2D.Raycast(origin, getVectorFromAngle(angle), viewDistance, blockingObjectMask);

                if (raycastHit2D.collider == null)
                {
                    vertex = origin + getVectorFromAngle(angle) * viewDistance;
                }
                else
                {
                    vertex = raycastHit2D.point;
                }
                
                vertices[vertexIndex] = vertex;
                if (i > 0)
                {
                    triangles[triangleIndex + 0] = 0;
                    triangles[triangleIndex + 1] = vertexIndex - 1;
                    triangles[triangleIndex + 2] = vertexIndex;
                    triangleIndex += 3;
                }

                vertexIndex++;
                angle -= angleIncrease;
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
        
        IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }
        
        void FindVisibleTargets()
        {
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewDistance, playerMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector2 dirToTarget = new Vector2(target.position.x - playerAttached.transform.position.x,
                    target.position.y - playerAttached.transform.position.y);
                
                if (Vector2.Angle(dirToTarget, transform.up) < startingAngle / 2)
                {
                    float dstToTarget = Vector2.Distance(transform.position, target.position);

                    if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, blockingObjectMask))
                    {
                        Debug.Log("encontre a player");
                    }
                    else
                    {
                        Debug.Log("no he encontrado ni mierda");
                    }
                }
            }
        }

        private Vector3 getVectorFromAngle(float angle)
        {
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public void SetAimDirection(Vector2 aimDirection)
        {
            startingAngle = getAngleFromVectorFloat(aimDirection) - fov / 2f;
        }

        private float getAngleFromVectorFloat(Vector2 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }
    }
}