using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowTrack : MonoBehaviour
{
	ArrayList touchingTracks = new ArrayList();

	private Quaternion tempRotRaw;
	private Vector3 tempRot;
	Transform lastNavPoint;
	Transform navPoint = null;

	Vector3 tempPos;
	readonly float MAX_SPEED = 0.30f;
	float speed = 0f;
	int nextNavPointIndex = 0;
	int navPointCount = 0;
	GameObject currentTrack = null;

	bool moving = false;
	bool stopping = false;

	bool paused = false;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (!paused)
		{
			if (Input.GetKey(KeyCode.Z)) //means moving forward
			{
				moving = true;
			}
			else if (Input.GetKey(KeyCode.X))
			{
				stopping = true;
			}
			else if (Input.GetKeyUp(KeyCode.Space))
			{
				Pause();
				return;
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
					move = transform.TransformDirection(move);
					transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z); // add move to position


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
					move = transform.TransformDirection(move);
					transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z);
				}
			}

			/*
			 * no moving backwards right now
			 * we don't have the technology
			 */
		}
		//unpause if Esc is pressed again
		else if (Input.GetKeyUp(KeyCode.Space))
		{
			Unpause();
		}
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
			   && Math.Abs(position1.z - position2.z) <= tolerance;

	}

	public void Pause() {
		paused = true;
		GameObject canvas = GameObject.FindWithTag("pause-menu");
		for(int i = 0; i < canvas.transform.childCount; i++)
		{
			GameObject button = canvas.transform.GetChild(i).gameObject;
			button.SetActive(true);
		}
	}
	
	public void Unpause()
	{
		paused = false;
		GameObject canvas = GameObject.FindWithTag("pause-menu");
		for (int i = 0; i < canvas.transform.childCount; i++)
		{
			GameObject button = canvas.transform.GetChild(i).gameObject;
			button.SetActive(false);
		}
	}

	public void QuitToMenu()
	{
		SceneManager.LoadSceneAsync(0);
	}
}
