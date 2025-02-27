using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class SL_PointLight : MonoBehaviour
{
    [SerializeField] float viewRadius;
    public float ViewRadius
    {
        get{return viewRadius;}
        set{viewRadius = value;}
    }
    [SerializeField] [Range (0,360)] float viewAngle;
    public float ViewAngle
    {
        get{return viewAngle;}
        set{viewAngle=value;}
    }
    [SerializeField] float meshResolution;
    public float MesRes
    {
        get{return meshResolution;}
        set{meshResolution=value;}
    }
    [SerializeField] int edgeResolution;
    public int EdgeRes
    {
        get{return edgeResolution;}
        set{edgeResolution=value;}
    }
    [SerializeField] float edgeDistThreshold;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask;
    MeshFilter viewMeshFilter;
    Mesh viewMesh;
    // List<Transform> visibleTargets = new List<Transform>();
    List<Vector3> visibleTargets = new List<Vector3>();
    public List<Vector3> VisibleTargets
    {
        get{return visibleTargets;}
        set{visibleTargets=value;}
    }
    [SerializeField]
    List<Tilemap> tilemaps;



    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "GenMesh";
        viewMeshFilter = GetComponent<MeshFilter>();
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    void LateUpdate()
    {
        DrawFieldofView();
    }


    /// BEHAVIOUR(S) ///

    void DrawFieldofView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle*meshResolution);
        float stepAngleSize = viewAngle/stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        SL_ViewCastInfo oldViewCast = new SL_ViewCastInfo();

        for(int i=0;i<=stepCount;i++)
        {
            float angle=-transform.eulerAngles.z-viewAngle/2+stepAngleSize*i; // The Angle for each Sector of the view angle(!)

            SL_ViewCastInfo newViewCast = ViewCast(angle);
            if(i>0)
            {
                bool edgeDistThresholdExceeded = Mathf.Abs(oldViewCast._dist-newViewCast._dist)>edgeDistThreshold;
                if
                (
                    oldViewCast._hit != newViewCast._hit
                    ||
                    (
                        oldViewCast._hit
                        && newViewCast._hit
                        && edgeDistThresholdExceeded
                    )
                )
                {
                    SL_EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge._pointA != Vector3.zero) viewPoints.Add(edge._pointA);
                    if(edge._pointB != Vector3.zero) viewPoints.Add(edge._pointB);
                }
            }

            viewPoints.Add(newViewCast._point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count+1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] tris = new int[(vertexCount-2)*3];

        vertices[0] = Vector3.zero;
        for(int i=0;i<vertexCount-1;i++)
        {
            vertices[i+1] = transform.InverseTransformPoint(viewPoints[i]);

            if(i<vertexCount-2)
            {
                tris[i*3] = 0;
                tris[i*3+1] = i+1;
                tris[i*3+2] = i+2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = tris;
        viewMesh.RecalculateNormals();
    }

    SL_ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = PU_Utilities.DirFromAngle(globalAngle);
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position,dir, viewRadius,targetMask);

        if(hit2D.collider!=null)
            // Hit
            return new SL_ViewCastInfo(true,hit2D.point,hit2D.distance,globalAngle);
        else
            // No Hit
            return new SL_ViewCastInfo(false,transform.position+dir*viewRadius,viewRadius,globalAngle);
    }
    
    SL_EdgeInfo FindEdge(SL_ViewCastInfo minViewCast, SL_ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast._angle;
        float maxAngle = maxViewCast._angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i=0;i<edgeResolution;i++)
        {
            float angle = (minAngle+maxAngle)/2;
            SL_ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistThresholdExceeded = Mathf.Abs(minViewCast._dist-newViewCast._dist)>edgeDistThreshold;
            if(newViewCast._hit==minViewCast._hit && !edgeDistThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast._point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast._point;
            }
        }

        return new SL_EdgeInfo(minPoint,maxPoint);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    // This logic needs to be altered or reworked for the tilemaps (!)
    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position,viewRadius,targetMask);

        for(int i=0; i<targetsInViewRadius.Length;i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 targetPos = target.position;
            // This is where tiles need to be tracked!
            Debug.Log("Target: " + target.name);
            if(target.GetComponent<Tilemap>()!=null)
            {
                visibleTargets.AddRange(VisibleTiles(target));
            }
            else
            {
                // Vector3 dirToTarget = (target.position-transform.position).normalized;
                Vector3 dirToTarget = (targetPos-transform.position).normalized;

                if(Vector3.Angle(transform.up,dirToTarget)<viewAngle/2)
                {
                    float distToTarget = Vector3.Distance(transform.position,target.position);
                    RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position,dirToTarget,distToTarget,obstacleMask);


                    if(obstacleHit.collider==null)
                        // visibleTargets.Add(target.position);
                        visibleTargets.Add(targetPos);
                }
            }


        }
    }

    // Vector3 VisibleTile(Transform tilemapTransform)
    List<Vector3> VisibleTiles(Transform tilemapTransform)
    {
        Debug.Log("Entered VisibleTile!");
        // Vector3 result = tilemapTransform.position;
        List<Vector3> result = new List<Vector3>();
        Tilemap tilemap = tilemapTransform.GetComponent<Tilemap>();
        int stepCount = Mathf.RoundToInt(viewAngle*meshResolution);
        float stepAngleSize = viewAngle/stepCount;
        RaycastHit2D hit2D = new RaycastHit2D();
        // Vector3 dir = PU_Utilities.DirFromAngle(globalAngle);
        Vector3Int tilePos = Vector3Int.zero;
        TileBase hitTile = null;

        // Debug.DrawRay(transform.position, Vector2.down*rayLength, Color.red);

        // Detect the tile within the view angle
        for(int i=0;i<=stepCount;i++)
        {
            float angle=-transform.eulerAngles.z-viewAngle/2+stepAngleSize*i; // The Angle for each Sector of the view angle(!)
            Vector3 dir = PU_Utilities.DirFromAngle(angle);
            hit2D = Physics2D.Raycast(transform.position,dir, viewRadius,targetMask);
            // if(hit2D.collider!=null) break;

            if
            (
                hit2D.collider!=null
                && 
                // Filters the tilemaps for checks; this should be done better (!)
                (
                    hit2D.collider.name.Contains("64") && tilemap.name.Contains("64")
                    || hit2D.collider.name.Contains("32") && tilemap.name.Contains("32")
                    || hit2D.collider.name.Contains("16") && tilemap.name.Contains("16")
                )
            )
            {
                Debug.Log("hit2D name: " + hit2D.collider.name);

                Vector3 hitPos = hit2D.point;

                // Offset into the tiles that were hit
                hitPos.x += transform.position.x-hitPos.x < 0 ?  .0001f : -.0001f;
                hitPos.y += transform.position.y-hitPos.y < 0 ?  .0001f : -.0001f;

                tilePos = tilemap.WorldToCell(hitPos);

                hitTile = tilemap.GetTile(tilePos);
                // if(hitTile!=null) result = tilePos;
                // if(hitTile!=null) result = hitPos;
                if(hitTile!=null) result.Add(hitPos);

                if(hitTile!=null)
                    Debug.Log($"Hit tile at {tilePos}: {hitTile.name}");
                else
                    Debug.LogError("No tile at position");

                Debug.Log("Position: " + hitPos);
            }
        }

        return result;
    }
}
