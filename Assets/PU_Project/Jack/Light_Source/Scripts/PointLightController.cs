using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[RequireComponent (typeof(PointLightView))]
public class PointLightController : MonoBehaviour
{
    [SerializeField] bool hiRes = false;
    [SerializeField] [Range(1,18)] int sector = 1;
    [SerializeField] PointLightModel pointLightModel;
    public PointLightModel PointLightModel
    {
        get { return pointLightModel; }
        private set { pointLightModel=value; }
    }
    PointLightView pointLightView;
    public PointLightView PointLightView
    {
        get { return pointLightView; }
        private set { pointLightView=value; }
    }
    PolygonCollider2D triangleCollider;



    void Start()
    {
        triangleCollider = GetComponent<PolygonCollider2D>();
        pointLightView = GetComponent<PointLightView>();
        StartCoroutine("FindTargetsWithDelay", pointLightModel.Delay);
    }

    void FixedUpdate()
    {
        CalculateRenderMesh(pointLightModel); // LoRes
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enter Detected: " + other.name);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exit Detected: " + other.name);
    }

    void Update()
    {
        // FindVisibleTargets();
        gameObject.GetComponent<MeshRenderer>().material.color = new Color(1,1,1,PointLightModel.ColorOpacity);
        // PointLightModel.UpdateModel();
    }


    void LateUpdate()
    {
        // CalculateRenderMesh(pointLightModel);
        RefineRenderMesh(pointLightModel); // HiRes
        // Light Visuals
        pointLightView.UpdateView(pointLightModel);
    }

    void CalculateRenderMesh(PointLightModel pointLightModel)
    {
        // Render Mesh Assignments
        pointLightModel.LightPosition = transform.position;
        int stepCount = Mathf.RoundToInt(pointLightModel.ViewAngle*pointLightModel.MesRes);     // Number of Rays that will be cast
        float stepAngleSize = pointLightModel.ViewAngle/stepCount;                              // Angle b/w ea. raycast
        pointLightModel.ViewPoints = new List<Vector3>();                                       // A list of all the points that our raycast hits; for mesh construction/rendering
        SL_ViewCastInfo oldViewCast = new SL_ViewCastInfo();                                    // Default information of a raycast (no information); for deter. if prev. viewcast hit something

        /// Render Mesh RayCast Generation Logic ///
        // 1.) For each raycast
        float angle = 0;
        int hiStepCount = Mathf.RoundToInt(pointLightModel.ViewAngle*pointLightModel.MesRes);     // Number of Rays that will be cast
        int loStepCount = Mathf.RoundToInt(pointLightModel.ViewAngle*pointLightModel.MesRes/10);     // Number of Rays that will be cast
        triangleCollider.pathCount = hiRes ? hiStepCount+1 : loStepCount+1;
        float histepAngleSize = pointLightModel.ViewAngle/hiStepCount;                              // Angle b/w ea. raycast
        float lostepAngleSize = pointLightModel.ViewAngle/loStepCount;                              // Angle b/w ea. raycast
        stepAngleSize = hiRes ? histepAngleSize : lostepAngleSize;
        int i =0;
        int polyIndex = 0;
        // for(int i=0;i<=stepCount;i++)
        while(angle < pointLightModel.ViewAngle+.001f)
        {
            // 1.1) Get the the angle in the proper orientation (xy-plane) and rotation (CCW)
            // The Global Angle for each Sector of the view angle
            // This causes the rays to "scan" (!)
            float globalAngle=
                -transform.eulerAngles.z        // Offsets the "center" of the ViewAngle CCW of the z-axis rotation
                -pointLightModel.ViewAngle/2    // Offsets to the "left" bound of the ViewAngle 
                // +stepAngleSize*i;               // Increments the number of raycast angle steps CW from the "left" ViewAngle side (to the right)
                +angle;               // Increments the number of raycast angle steps CW from the "left" ViewAngle side (to the right)
            float nextGlobalAngle =-transform.eulerAngles.z-pointLightModel.ViewAngle/2+Math.Clamp(angle+stepAngleSize, 0, pointLightModel.ViewAngle);
            /*
                Given our...
                    1.) global angle
                    2.) view radius
                    3.) mask

                We get information about a raycast relevant to our "View"...
                    1.) Whether or not it Hit our <Target>
                    2.) Where did it hit
                    3.) How far is the hit from the <gameObject>
                    4.) What's the angle of the hit from forward(?)
            */
            // SL_ViewCastInfo newViewCast = ViewCast(globalAngle,pointLightModel.ViewRadius,pointLightModel.TargetMask); 
            SL_ViewCastInfo newViewCast = PU_Utilities.ViewCast(globalAngle,pointLightModel); 
            SL_ViewCastInfo nextViewCast = PU_Utilities.ViewCast ( nextGlobalAngle,pointLightModel);


            // 1.2) EdgeDetection Logic; For each ray that's not the first, because the first oldViewCast is undef...
            if(i>0)
            {
                bool edgeDistThresholdExceeded = Mathf.Abs(oldViewCast._dist-newViewCast._dist)>pointLightModel.EdgeDistThresh;
                if
                (
                    // Checks if only one of the viewCasts hits something, then that means there's an edge somewhere,
                    // ATTN: How about checking if they both hit something that is the same, or check for different normals for edges/corners
                    oldViewCast._hit != newViewCast._hit
                    ||
                    // however, if both hits w/i a certain dist. from each other still run the edge algo.
                    // ATTN: This is a check because the raycasts don't differentiate distinct hits, so this is just a way to distinguish both hits w/o adding more info to the raycasts (!)
                    (
                        oldViewCast._hit
                        && newViewCast._hit
                        && edgeDistThresholdExceeded
                    )
                )
                {
                    // Review this logic later (!)
                    SL_EdgeInfo edge = PU_Utilities.FindEdge(oldViewCast, newViewCast,pointLightModel);
                    if(edge._pointA != Vector3.zero) pointLightModel.ViewPoints.Add(edge._pointA);
                    if(edge._pointB != Vector3.zero) pointLightModel.ViewPoints.Add(edge._pointB);
                    // hiRes = !hiRes;
                }
            }
            i++;

            // Adds the actual raycast hit point (!) and establish the last raycast hit
            pointLightModel.ViewPoints.Add(newViewCast._point);
            oldViewCast = newViewCast;

            stepAngleSize = hiRes ? histepAngleSize : lostepAngleSize; // (!)
            angle += stepAngleSize;
            if(angle>pointLightModel.ViewAngle) pointLightModel.ViewPoints.Add(nextViewCast._point);

            // if(i==sector)
            // {
                /// Creates the PolyCollider based on the Mesh ///
                // Is this really needed?
                triangleCollider.SetPath
                (
                    polyIndex++,
                    new Vector2[]
                    {
                        Vector2.zero,
                        transform.InverseTransformPoint(newViewCast._point),
                        transform.InverseTransformPoint(nextViewCast._point)
                    }
                );
            // }
        }
    }

    void RefineRenderMesh(PointLightModel pointLightModel)
    {

    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    // Light Logic (visible to the Editor)
    void FindVisibleTargets()
    {
        pointLightModel.VisibleTargets.Clear(); // Reset VisibleTargets List
        // 1.) Collect all the colliders w/i a given radius
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position,pointLightModel.ViewRadius,pointLightModel.TargetMask);

        // 2.) Loop thru all those colliders
        for(int i=0; i<targetsInViewRadius.Length;i++)
        {
            // Take a target and get it's position
            Transform target = targetsInViewRadius[i].transform;
            Vector3 targetPos = target.position;
            ContactPoint2D[] contacts = new ContactPoint2D[50];

            // This is where tiles need to be tracked!
            // Debug.Log("Target: " + target.name);

            // 2.1) If a Collider is a Tilemap
            if(target.GetComponent<Tilemap>()!=null)
            {
                int contactCount = GetComponent<PolygonCollider2D>().GetContacts(contacts);
                if(contactCount>0)
                for(int j=0;j<contactCount;j++)
                {
                    Debug.Log("Contact Point["+j+"]: "+contacts[j]);
                }
                // Add it to Visible targets (this is a lie!)
                pointLightModel.VisibleTargets.AddRange(VisibleTiles(target));
            }
            // 2.2) If a Collider is not a Tilemap
            else
            {
                // Vector3 dirToTarget = (target.position-transform.position).normalized;
                // 2.2.a) Figure out the unit vector to the collider
                Vector3 dirToTarget = (targetPos-transform.position).normalized;

                // 2.2.b) If it's w/i the effective view cone
                if(Vector3.Angle(transform.up,dirToTarget)<pointLightModel.ViewAngle/2)
                {
                    float distToTarget = Vector3.Distance(transform.position,target.position);
                    RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position,dirToTarget,distToTarget,pointLightModel.ObstacleMask); // This would be a kind of raycast all...(!)


                    // 2.2.b.i) Check if it has any obstacles in front of it; This could also be extended to check for other targets obstructing the line of sight (!)
                    if(obstacleHit.collider==null)
                        // visibleTargets.Add(target.position);
                        pointLightModel.VisibleTargets.Add(targetPos);
                }
            }
        }
    }

    List<Vector3> VisibleTiles(Transform tilemapTransform)
    {
        // Debug.Log("Entered VisibleTile!");
        // Vector3 result = tilemapTransform.position;
        List<Vector3> result = new List<Vector3>();
        Tilemap tilemap = tilemapTransform.GetComponent<Tilemap>();
        int stepCount = Mathf.RoundToInt(pointLightModel.ViewAngle*pointLightModel.MesRes);
        float stepAngleSize = pointLightModel.ViewAngle/stepCount;
        RaycastHit2D hit2D = new RaycastHit2D();
        // Vector3 dir = PU_Utilities.DirFromAngle(globalAngle);
        Vector3Int tilePos = Vector3Int.zero;
        TileBase hitTile = null;
        Vector3 original = Vector3.zero;
        Vector3 last = Vector3.zero;

        // Debug.DrawRay(transform.position, Vector2.down*rayLength, Color.red);

        /// This recreates the logic for casting all the rays (not DRY!) ///
        // 1.) Detect the tile within the view angle
        for(int i=0;i<=stepCount;i++)
        {
            float angle=-transform.eulerAngles.z-pointLightModel.ViewAngle/2+stepAngleSize*i; // The Angle for each Sector of the view angle(!)
            Vector3 dir = PU_Utilities.DirFromAngle(angle);
            hit2D = Physics2D.Raycast(transform.position,dir, pointLightModel.ViewRadius,pointLightModel.TargetMask);


            if
            (
                // 2.) Detect a hit
                hit2D.collider!=null
                && 
                // 3.) Filters the tilemaps for checks; this should be done better (!)
                (
                    hit2D.collider.name.Contains("64") && tilemap.name.Contains("64")
                    || hit2D.collider.name.Contains("32") && tilemap.name.Contains("32")
                    || hit2D.collider.name.Contains("16") && tilemap.name.Contains("16")
                )
            )
            {
                // Debug.Log("hit2D name: " + hit2D.collider.name);

                Vector3 hitPos = hit2D.point;
                last = hitPos;
                // Debug.Log("Tile Normal: " + hit2D.normal);
                if(original == (Vector3)hit2D.normal) continue;
                original = hit2D.normal;
                

                // 4.) Offset into the tiles that were hit
                hitPos.x += transform.position.x-hitPos.x < 0 ?  .0001f : -.0001f;
                hitPos.y += transform.position.y-hitPos.y < 0 ?  .0001f : -.0001f;

                // 5.) Get and alter the detected tile
                tilePos = tilemap.WorldToCell(hitPos);
                tilemap.SetTileFlags(tilePos,TileFlags.None);
                tilemap.GetComponent<GridInformation>().SetPositionProperty(tilePos,"Umbral State",1);
                tilemap.SetColor(tilePos, new Color(1,0,0,.5f));

                // 6.) Get the Detect tile and add it to "visible" tile(s)
                hitTile = tilemap.GetTile(tilePos);
                // if(hitTile!=null) result = tilePos;
                // if(hitTile!=null) result = hitPos;
                if(hitTile!=null) result.Add(hitPos);

                // if(hitTile!=null)
                //     Debug.Log($"Hit tile at {tilePos}: {hitTile.name}");
                // else
                //     Debug.LogError("No tile at position");

                // Debug.Log("Position: " + hitPos);
            }
            if(!result.Contains(last) && i==stepCount)result.Add(last);
        }

        return result;
    }
}
