using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol_NOFLIP : MonoBehaviour {
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    public bool isRight = true;
    public float speed = 0.3f;
    private Vector3 pointAPosition;
    private Vector3 pointBPosition;

    void Start()
    {
        pointAPosition = new Vector3(pointA.position.x, pointA.position.y, 0);
        pointBPosition = new Vector3(pointB.position.x, pointB.position.y, 0);
    }

    void Update()
    {
        Vector3 thisPosition = new(transform.position.x, transform.position.y, 0);
        if (isRight)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);

            if (Vector2.Distance(thisPosition, pointBPosition) < 0.05f)
            {
                isRight = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pointA.position, speed * Time.deltaTime);

            if (Vector2.Distance(thisPosition, pointAPosition) < 0.05f)
            {
                isRight = true;
            }
        }
    }
}