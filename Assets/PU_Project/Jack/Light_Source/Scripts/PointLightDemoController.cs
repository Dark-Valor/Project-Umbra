using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PointLightDemoController : MonoBehaviour
{
    [SerializeField] [Range(1,18)] int sector = 1;
    [SerializeField] PointLightDemoModel pointLightDemoModel;
    public PointLightDemoModel PointLightDemoModel
    {
        get { return pointLightDemoModel; }
        private set { pointLightDemoModel=value; }
    }
    PointLightDemoView pointLightDemoView;
    public PointLightDemoView PointLightDemoView
    {
        get { return pointLightDemoView; }
        private set { pointLightDemoView=value; }
    }

    void Start()
    {
        pointLightDemoView = GetComponent<PointLightDemoView>();
    }

    void FixedUpdate()
    {
        CalculateLoResRenderMesh(pointLightDemoModel); // LoRes
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Detected: " + other.name);
    }

    void Update()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = new Color(1,1,1,PointLightDemoModel.ColorOpacity);
        DetectObjects(pointLightDemoModel);
    }

    void LateUpdate()
    {
        // CalculateRenderMesh(pointLightModel);
        RefineRenderMesh(pointLightDemoModel); // HiRes
        // Light Visuals
        pointLightDemoView.UpdateView(pointLightDemoModel);
    }

    void CalculateLoResRenderMesh(PointLightDemoModel pointLightDemoModel)
    {
        // Render Mesh Assignments
        pointLightDemoModel.LightPosition = transform.position;
        pointLightDemoModel.ViewPoints = new List<Vector3>();                                       // A list of all the points that our raycast hits; for mesh construction/rendering

        /// Render Mesh RayCast Generation Logic ///
        // 1.) For each raycast
        float angle = 0;
        int loStepCount = Mathf.RoundToInt(pointLightDemoModel.ViewAngle*pointLightDemoModel.MesRes/10);     // Number of Rays that will be cast
        float lostepAngleSize = pointLightDemoModel.ViewAngle/loStepCount;                              // Angle b/w ea. raycast
        

        while(angle < pointLightDemoModel.ViewAngle+.001f)
        {
            // 1.1) Get the the angle in the proper orientation (xy-plane) and rotation (CCW)
            // The Global Angle for each Sector of the view angle
            // This causes the rays to "scan" (!)
            float globalAngle=
                -transform.eulerAngles.z        // Offsets the "center" of the ViewAngle CCW of the z-axis rotation
                -pointLightDemoModel.ViewAngle/2    // Offsets to the "left" bound of the ViewAngle 
                // +stepAngleSize*i;               // Increments the number of raycast angle steps CW from the "left" ViewAngle side (to the right)
                +angle;               // Increments the number of raycast angle steps CW from the "left" ViewAngle side (to the right)
            float nextGlobalAngle =-transform.eulerAngles.z-pointLightDemoModel.ViewAngle/2+Math.Clamp(angle+lostepAngleSize, 0, pointLightDemoModel.ViewAngle);
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
            // SL_ViewCastInfo newViewCast = ViewCast(globalAngle,pointLightDemoModel.ViewRadius,pointLightDemoModel.TargetMask); 
            // SL_ViewCastInfo newViewCast = PU_Utilities.ViewCast(globalAngle,pointLightDemoModel); 
            // SL_ViewCastInfo nextViewCast = PU_Utilities.ViewCast ( nextGlobalAngle,pointLightDemoModel);

            // Adds the actual raycast hit point (!) and establish the last raycast hit
            // pointLightDemoModel.ViewPoints.Add(newViewCast._point);

            // Ensures that the last point is at the end of the View (Light) Cone
            angle += lostepAngleSize;
            // if(angle>pointLightDemoModel.ViewAngle) pointLightDemoModel.ViewPoints.Add(nextViewCast._point);
        }
    }

    void DetectObjects(PointLightDemoModel pointLightDemoModel)
    {
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position,pointLightDemoModel.ViewRadius,pointLightDemoModel.TargetMask);

        foreach(Collider2D target in targetsInViewRadius)
        {
            if(target.GetComponent<Tilemap>()!=null)
            {
                IlluminatedTiles(target.transform);
            }
        }
    }

    
    List<Vector3> IlluminatedTiles(Transform tilemapTransform)
    {
        // Debug.Log("Entered VisibleTile!");
        // Vector3 result = tilemapTransform.position;
        List<Vector3> result = new List<Vector3>();
        Tilemap tilemap = tilemapTransform.GetComponent<Tilemap>();
        int stepCount = Mathf.RoundToInt(pointLightDemoModel.ViewAngle*pointLightDemoModel.MesRes);
        float stepAngleSize = pointLightDemoModel.ViewAngle/stepCount;
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
            float angle=-transform.eulerAngles.z-pointLightDemoModel.ViewAngle/2+stepAngleSize*i; // The Angle for each Sector of the view angle(!)
            Vector3 dir = PU_Utilities.DirFromAngle(angle);
            hit2D = Physics2D.Raycast(transform.position,dir, pointLightDemoModel.ViewRadius,pointLightDemoModel.TargetMask);


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

    void RefineRenderMesh(PointLightDemoModel pointLightDemoModel)
    {

    }
}
