using UnityEngine;

public class PingPong : MonoBehaviour
{

    [SerializeField] float pingPongSpeed=2f;
    [SerializeField] float min=2f;
	[SerializeField] float max=3f;

	
	// Update is called once per frame
	void Update ()
    {
		transform.position =new Vector3(Mathf.PingPong(pingPongSpeed*Time.time,max-min)+min, transform.position.y, transform.position.z);
	}

}
