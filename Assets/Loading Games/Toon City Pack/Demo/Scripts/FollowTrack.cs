using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrack : MonoBehaviour
{
    Queue<GameObject> touchingTracks = new Queue<GameObject>();

    private Quaternion tempRotRaw;
    private Vector3 tempRot;
    Transform lastNavPoint;

    Vector3 tempPos;
    float speed = 0.1f;
    int navPointIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tempPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (Input.GetKey(KeyCode.Z))
        {
            if(touchingTracks.Count > 0)
			{
                GameObject nextTrack = touchingTracks.Peek();
                Debug.Log("Track children: " + nextTrack.gameObject.transform.childCount);
                
                var navPoint = nextTrack.transform.GetChild(navPointIndex);
                if (lastNavPoint != null && IsBetween(tempPos, lastNavPoint.position, navPoint.position)) //if we're still on the same nav point
                {
                    //transform.position = new Vector3(tempPos.x, tempPos.y, tempPos.z + speed);
                    Vector3 move = new Vector3(0, 0, 0 + speed);
                    //Debug.Log("move: " + move);
                    move = transform.TransformDirection(move);
                    //move *= speed;
                    transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z);
                }
                else //we need to move to the next point
                { 
                    navPointIndex += 1; //assume we're not moving backwards for now
                    transform.LookAt(navPoint);
                }
                //transform.position = new Vector3(nextNavPoint.position.x, tempPos.y, nextNavPoint.position.z);
                tempRot = transform.rotation.eulerAngles;
                tempRotRaw = transform.rotation;


                tempRotRaw.eulerAngles = tempRot;
                transform.rotation = tempRotRaw;
                Debug.Log("Looking at " + navPoint);
                lastNavPoint = navPoint;
			}
            else
			{
                //transform.position = new Vector3(tempPos.x, tempPos.y, tempPos.z + speed);
                Vector3 move = new Vector3(0, 0, 0 + speed);
                //Debug.Log("move: " + move);
                move = transform.TransformDirection(move);
                //move *= speed;
                transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z);
            }
		}
        else if (Input.GetKey(KeyCode.X))
		{
            transform.position = new Vector3(tempPos.x, tempPos.y, tempPos.z - speed);
        }
    }

	private void OnCollisionEnter(Collision collision)
	{
        Debug.Log("Collision Enter: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("railroad-track"))
		{
            touchingTracks.Enqueue(collision.gameObject);
            navPointIndex = 0;
        }
	}

	private void OnCollisionExit(Collision collision)
	{
        Debug.Log("Collision Exit: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("railroad-track") && touchingTracks.Contains(collision.gameObject))
        {
            touchingTracks.Dequeue();
        }
    }

    private bool IsBetween(Vector3 position, Vector3 start, Vector3 end, float tolerance = 0.5f)
	{
        Vector3 line = end - start;
        Vector3 vector = position - start;

        float projection = Vector3.Dot(vector, line / line.magnitude);

        return projection >= 0f
            && projection <= line.magnitude
            && Mathf.Abs(projection - line.magnitude) <= tolerance;
	}
}
