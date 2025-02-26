using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightSourceController : MonoBehaviour
{
    [SerializeField]
    // float rayLength = 2f;
    float rayLength = 1000;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    List<Tilemap> tilemaps;
    [SerializeField]
    bool down;
    enum SourceDir
    {
        UP,
        DOWN,
        RIGHT,
        LEFT,
        NORTH_WEST,
        NORTH_EAST,
        SOUTH_WEST,
        SOUTH_EAST,
    }
    [SerializeField]
    SourceDir sourceDir;
    [SerializeField]
    float hitPosOffset = .5f;
    [SerializeField]
    float lightSourceAngle = 0f;



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Vector3 rayDir = Vector3.one;
        // Vector3 rayDir = Vector3.up;
        Vector3 rayDir = Vector3.right;

        

        // switch(sourceDir)
        // {
        //     case SourceDir.UP:
        //         rayDir = Vector3.up;
        //         break;
        //     case SourceDir.DOWN:
        //         rayDir = Vector3.down;
        //         break;
        //     case SourceDir.RIGHT:
        //         rayDir = Vector3.right;
        //         break;
        //     case SourceDir.LEFT:
        //         rayDir = Vector3.left;
        //         break;
        //     case SourceDir.NORTH_WEST:
        //         rayDir = Vector3.up;
        //         rayDir += Vector3.left;
        //         break;
        //     case SourceDir.NORTH_EAST:
        //         rayDir = Vector3.up;
        //         rayDir += Vector3.right;
        //         break;
        //     case SourceDir.SOUTH_WEST:
        //         rayDir = Vector3.down;
        //         rayDir += Vector3.left;
        //         break;
        //     case SourceDir.SOUTH_EAST:
        //         rayDir = Vector3.down;
        //         rayDir += Vector3.right;
        //         break;
        // }
        // lightSourceAngle+=6*Time.deltaTime;
        // lightSourceAngle+=6*Time.fixedDeltaTime;
        rayDir = Quaternion.Euler(0,0,lightSourceAngle)*rayDir;
        // rayDir = Quaternion.Euler(0,0,lightSourceAngle+=Time.deltaTime)*rayDir;
        // rayDir = Quaternion.Euler(0,0,6*(lightSourceAngle+=Time.deltaTime))*rayDir;
        // rayDir = rayDir.normalized;

        // RaycastHit2D hit2D = Physics2D.Raycast(transform.position,down ? Vector2.down : Vector2.up, rayLength,layerMask);
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position,rayDir, rayLength,layerMask);
        Vector3Int tilePos = Vector3Int.zero;
        TileBase hitTile = null;

        // Debug.DrawRay(transform.position, Vector2.down*rayLength, Color.red);

        if(hit2D.collider!=null)
        {
            Debug.Log("hit2D name: " + hit2D.collider.name);

            Vector3 hitPos = hit2D.point;
            rayLength = hit2D.distance;

            foreach(var tilemap in tilemaps)
            {
                // hitPos.y += down ? -.5f : .5f;

                // There is a case where the light is shining outside of a block...(!)
                // This ensures that light will only "hit" when it is not in the wall.
                if(rayDir.y>0)
                    hitPos.y += hitPosOffset;
                else
                    hitPos.y -= hitPosOffset;

                // if(rayDir.x>0)
                //     hitPos.x += hitPosOffset;
                // else
                //     hitPos.x -= hitPosOffset;


                // switch(sourceDir)
                // {
                //     case SourceDir.UP:
                //         hitPos.y += hitPosOffset;
                //         break;
                //     case SourceDir.DOWN:
                //         hitPos.y += -hitPosOffset;
                //         break;
                //     case SourceDir.RIGHT:
                //         hitPos.x += hitPosOffset;
                //         break;
                //     case SourceDir.LEFT:
                //         hitPos.x += -hitPosOffset;
                //         break;
                //     case SourceDir.NORTH_WEST:
                //         hitPos.y += hitPosOffset;
                //         hitPos.x += -hitPosOffset;
                //     break;
                //     case SourceDir.NORTH_EAST:
                //         hitPos.y += hitPosOffset;
                //         hitPos.x += hitPosOffset;
                //     break;
                //     case SourceDir.SOUTH_WEST:
                //         hitPos.y += -hitPosOffset;
                //         hitPos.x += -hitPosOffset;
                //     break;
                //     case SourceDir.SOUTH_EAST:
                //         hitPos.y += -hitPosOffset;
                //         hitPos.x += hitPosOffset;
                //     break;
                // }


                tilePos = tilemap.WorldToCell(hitPos);

                hitTile = tilemap.GetTile(tilePos);
                if(hitTile!=null) break;
            }

            if(hitTile!=null)
                Debug.Log($"Hit tile at {tilePos}: {hitTile.name}");
            else
                Debug.LogError("No tile at position");

            Debug.Log("Position: " + hitPos);
        }
        else
            rayLength = 1000;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Vector3 rayDir = Vector3.one;
        // Vector3 rayDir = Vector3.up;
        Vector3 rayDir = Vector3.right;

        // switch(sourceDir)
        // {
        //     case SourceDir.UP:
        //         rayDir = Vector3.up;
        //         break;
        //     case SourceDir.DOWN:
        //         rayDir = Vector3.down;
        //         break;
        //     case SourceDir.RIGHT:
        //         rayDir = Vector3.right;
        //         break;
        //     case SourceDir.LEFT:
        //         rayDir = Vector3.left;
        //         break;
        //     case SourceDir.NORTH_WEST:
        //         rayDir = Vector3.up;
        //         rayDir += Vector3.left;
        //         break;
        //     case SourceDir.NORTH_EAST:
        //         rayDir = Vector3.up;
        //         rayDir += Vector3.right;
        //         break;
        //     case SourceDir.SOUTH_WEST:
        //         rayDir = Vector3.down;
        //         rayDir += Vector3.left;
        //         break;
        //     case SourceDir.SOUTH_EAST:
        //         rayDir = Vector3.down;
        //         rayDir += Vector3.right;
        //         break;
        // }
        rayDir = Quaternion.Euler(0,0,lightSourceAngle)*rayDir;
        // rayDir = Quaternion.Euler(0,0,6*(lightSourceAngle+=Time.deltaTime))*rayDir;
        // rayDir = rayDir.normalized;

        // Gizmos.DrawRay(transform.position, (down? Vector3.down : Vector3.up )*rayLength);
        Gizmos.DrawRay(transform.position, rayDir*rayLength);
    }
}
