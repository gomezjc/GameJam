using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class FieldOfView : MonoBehaviour
    {
        public float viewRadius;
        [Range(0, 360)] public float viewAngle;

        public LayerMask targetMask;
        public LayerMask obstacleMask;
        public List<Transform> visibleTargets = new List<Transform>();

        public float meshResolution;
        public MeshFilter viewMeshFilter;
        private Mesh viewMesh;
        
        void Start()
        {
            viewMesh = new Mesh();
            viewMesh.name = "View Mesh";
            viewMeshFilter.mesh = viewMesh;
            StartCoroutine("FindTargetsWithDelay", .2f);
        }

        private void LateUpdate()
        {
            drawFieldOfView();
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
            visibleTargets.Clear();
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector2 dirToTarget = new Vector2(target.position.x - transform.position.x,
                    target.position.y - transform.position.y);
                if (Vector2.Angle(dirToTarget, transform.up) < viewAngle / 2)
                {
                    float dstToTarget = Vector2.Distance(transform.position, target.position);

                    if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        void drawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCastInfo = ViewCast(angle);
                viewPoints.Add(newViewCastInfo.point);
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            
            for (int i = 0; i <= vertexCount - 1; i++)
            {
                vertices[i + 1] = viewPoints[i];
                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                    
                }
            }
            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        public Vector3 DirFromAngle(float angleInDegress, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegress += transform.eulerAngles.z;
            }
            
            return new Vector3(Mathf.Sin(angleInDegress * Mathf.Deg2Rad), Mathf.Cos(angleInDegress * Mathf.Deg2Rad),0);
        }

        ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 dir = DirFromAngle(globalAngle, true);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);
            if (hit.collider != null)
            {
                return new ViewCastInfo(true,hit.point,hit.distance,globalAngle);
            }
            else
            {
                return new ViewCastInfo(false,transform.position + dir * viewRadius,viewRadius,globalAngle);
            }
        }
        
        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float dist;
            public float angle;

            public ViewCastInfo(bool hit, Vector3 point, float dist, float angle)
            {
                this.hit = hit;
                this.point = point;
                this.dist = dist;
                this.angle = angle;
            }
            
            
        }
    }
}