using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrack : MonoBehaviour
{
	ArrayList touchingTracks = new ArrayList();

	private Quaternion tempRotRaw;
	private Vector3 tempRot;
	Transform lastNavPoint;
	Transform navPoint = null;

	Vector3 tempPos;
	readonly float MAX_SPEED = 0.25f;
	float speed = 0f;
	int nextNavPointIndex = 0;
	int navPointCount = 0;
	GameObject currentTrack = null;

	bool moving = false;
	bool stopping = false;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.Z)) //means moving forward
		{
			moving = true;
		}
		else if (Input.GetKey(KeyCode.X))
		{
			stopping = true;
		}

		tempPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

		if (moving) //means moving forward
		{
			if (stopping)
			{
				if (speed > 0f)
				{
					speed -= 0.01f;
				}
				else
				{
					moving = false;
					stopping = false;
				}
			}
			else if (speed < MAX_SPEED)
			{
				speed += 0.001f;
			}
			Vector3 move = new Vector3(0, 0, 0 + speed); // start from moving to the right
			transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z); // add move to position

			if (currentTrack == null && touchingTracks.Count > 0)
			{
				currentTrack = (GameObject)touchingTracks[0];
				navPointCount = currentTrack.gameObject.transform.childCount;
				Debug.Log("Track children: " + navPointCount);
			}
			if (currentTrack != null && navPointCount > 0)
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
					//Debug.LogWarning("lastNavPoint: " + lastNavPoint.transform.position);
					//Debug.LogWarning("bus: " + tempPos);
				}
				if (navPoint == null
					|| (IsTouching(tempPos, lastNavPoint.transform.position, 1.0f))) //if we're starting the first nav point of the track, or if we're touching the last navPoint we were looking at
				{
					try
					{
						Debug.LogWarning(string.Format("1: index at {0} of count {1}", nextNavPointIndex, navPointCount));
						if (nextNavPointIndex > navPointCount - 1) //check if we're at the end of the current track
						{
							if (touchingTracks.Count > 1) //and make sure we have a new track to go to
							{
								//if so, discard it and look at the next one
								Debug.LogWarning(string.Format("2: index at {0} of count {1}", nextNavPointIndex, navPointCount));
								Debug.LogWarning("discarding current track");
								currentTrack = (GameObject)touchingTracks[1];
								navPointCount = currentTrack.gameObject.transform.childCount;
								Debug.Log("New track children: " + navPointCount);
								nextNavPointIndex = 0;
								navPoint = null;
								lastNavPoint = null;
								return; //exit early to move on to the next track
							}
							else
							{
								//do nothing - keep the same navPoint
							}
						}
						else
						{
							//get the next navPoint
							navPoint = currentTrack.transform.GetChild(nextNavPointIndex);
							nextNavPointIndex++;
							lastNavPoint = navPoint;
							Debug.Log("updating navPoint to " + navPoint);
						}

					}
					catch (UnityException ue)
					{
						Debug.Log(ue);
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

		/*
		 * no moving backwards right now
		 * we don't have the technology
		else if (Input.GetKey(KeyCode.X))
		{
			Vector3 move = new Vector3(0, 0, 0 - speed); // start from moving to the left
			move = transform.TransformDirection(move);
			transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z); // add move to position
		}
		*/
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Collision Enter: " + collision.gameObject.name);
		if (collision.gameObject.CompareTag("railroad-track"))
		{
			if (!touchingTracks.Contains(collision.gameObject))
			{
				touchingTracks.Insert(touchingTracks.Count, collision.gameObject);
			}
			for (short i = 0; i < touchingTracks.Count; i++)
			{
				Debug.LogWarning(string.Format("Track {0}: {1}", i, touchingTracks.ToArray()[i]));
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		Debug.Log("Collision Exit: " + collision.gameObject.name);
		if (collision.gameObject.CompareTag("railroad-track") && touchingTracks.Contains(collision.gameObject))
		{
			touchingTracks.Remove(collision.gameObject);
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
