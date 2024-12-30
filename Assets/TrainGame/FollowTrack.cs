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

        if (Input.GetKey(KeyCode.Z)) //means moving forward
        {
            Vector3 move = new Vector3(0, 0, 0 + speed); // start from moving to the right
            transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z); // add move to position

            if (touchingTracks.Count > 0)
            {
                GameObject nextTrack = touchingTracks.Peek();
                int navPointCount = nextTrack.gameObject.transform.childCount;
                Debug.Log("Track children: " + navPointCount);

                if (navPointCount > 0)
                {

                    Transform navPoint = null;
                    try
                    {
                        navPoint = nextTrack.transform.GetChild(navPointIndex);
                        Debug.Log("first point - " + navPoint);
                    }
                    catch (UnityException ue)
                    {
                        Debug.Log(ue);
                        //return;
                    }

                    if (lastNavPoint == null) //if we're at the first nav point of the track 
                    {
                        transform.LookAt(navPoint);
                        Debug.LogWarning("looking at navPoint");
                    }
                    else if (navPoint == null //if there's no next navPoint
                           || (lastNavPoint != null && IsBetween(tempPos, lastNavPoint.position, navPoint.position, 0.1f))) //OR if we're still on the same nav point
                    {
                        // specifically DON'T look at the navPoint, because we might be on top of it
                        Debug.Log("same point: " + navPoint);
                    }
                    else //we need to move to the next point
                    {
                        navPointIndex += 1; //assume we're not moving backwards for now
                        Debug.Log("new point - navPoint " + navPoint);
                        try
                        {
                            navPoint = nextTrack.transform.GetChild(navPointIndex);
                            transform.LookAt(navPoint);
                        }
                        catch (UnityException ue)
                        {
                            Debug.Log("caught exception when getting next point: " + ue.Message);
                            //keep moving for now, since we're not handling the boundary condition of the end of the nav points
                            //return;
                        }
                    }

                    move = transform.TransformDirection(move);
                    transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z); // add move to position
                    //move *= speed;


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
                    //Vector3 move = new Vector3(0, 0, 0 + speed);
                    //Debug.Log("move: " + move);
                    move = transform.TransformDirection(move);
                    //move *= speed;
                    transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z);
                }
            }
		}
        else if (Input.GetKey(KeyCode.X))
		{
            Vector3 move = new Vector3(0, 0, 0 - speed); // start from moving to the left
            move = transform.TransformDirection(move);
            transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z); // add move to position
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
