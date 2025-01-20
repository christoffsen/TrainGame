using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrack : MonoBehaviour
{
    Queue<GameObject> touchingTracks = new Queue<GameObject>();

    private Quaternion tempRotRaw;
    private Vector3 tempRot;
    Transform lastNavPoint;
    Transform navPoint = null;

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
                    //go to next navPoint
                    //when we touch a navPoint, go to the next one
                    //Transform navPoint = null;
                    /*
                    if (navPointIndex >= navPointCount //if there's no next navPoint
                       || lastNavPoint.position == navPoint.position)
                    {
                        // just keep going
                        Debug.Log("same point: " + navPoint);
                    }*/
                    if (lastNavPoint != null)
                    {
                        Debug.LogWarning("lastNavPoint: " + lastNavPoint.transform.position);
                        Debug.LogWarning("bus: " + tempPos);
                    }
                    if (navPoint == null
                        || ( IsTouching(tempPos, lastNavPoint.transform.position, 1.0f))) //if we're starting the first nav point of the track, or if we're touching the last navPoint we were looking at
                    {
                        try
                        {
                            //get the next navPoint
                            navPoint = nextTrack.transform.GetChild(navPointIndex);
                            navPointIndex++;
                            Debug.Log("updating navPoint to " + navPoint);
                        }
                        catch (UnityException ue)
                        {
                            Debug.Log(ue);
                            //return;
                        }
                        transform.LookAt(navPoint);
                    }
                    /*
                    else if(IsBetween(tempPos, lastNavPoint.transform.position, navPoint.transform.position, 0.1f)) {// if we're still on the same nav point

                    }

                    else //we need to move to the next point
                    {
                        Debug.LogWarning("not between");
                        Debug.LogWarning(string.Format("tempPos: {0}, lastNavPoint.transform.position: {1}, navPoint.transform.position: {2}", tempPos, lastNavPoint.transform.position, navPoint.transform.position));
                    }
                    */
                    move = transform.TransformDirection(move);
                    transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z); // add move to position
                    //move *= speed;


                    //transform.position = new Vector3(nextNavPoint.transform.position.x, tempPos.y, nextNavPoint.transform.position.z);
                    tempRot = transform.rotation.eulerAngles;
                    tempRotRaw = transform.rotation;


                    tempRotRaw.eulerAngles = tempRot;
                    transform.rotation = tempRotRaw;
                    if (navPoint == null)
                    {
                        Debug.Log("Looking at null navPoint!");
                    }
                    else
                    {
                        Debug.Log("Looking at " + navPoint);
                    }
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

    private bool IsTouching(Vector3 position1, Vector3 position2, float tolerance = 0.5f)
    {
        return Math.Abs(position1.x - position2.x) <= tolerance
               //&& Math.Abs(position1.y - position2.y) <= tolerance
               && Math.Abs(position1.z - position2.z) <= tolerance;

    }
}
