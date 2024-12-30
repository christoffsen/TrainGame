using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 tempPos;
    private Quaternion tempRotRaw;
    private Vector3 tempRot;
    private float speed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        tempPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        tempRot = transform.rotation.eulerAngles;
        tempRotRaw = transform.rotation;
        float leftMove = 0.0f;
        float rightMove = 0.0f;
        float forwardMove = 0.0f;
        float backwardMove = 0.0f;

        if (Input.GetKey(KeyCode.UpArrow))
		{
            forwardMove += speed;
		}
        else if (Input.GetKey(KeyCode.DownArrow))
		{
            backwardMove += speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            leftMove += speed;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rightMove += speed;
        }
        if (Input.GetKey(KeyCode.Backspace))
		{
            tempRot.y += 1f * speed;
            if(tempRot.y >= 360f)
			{
                tempRot.y = 0;
			}
            tempRotRaw.eulerAngles = tempRot;
        }
        else if (Input.GetKey(KeyCode.Backslash))
        {
            tempRot.y -= 1f * speed;
            if (tempRot.y <= 0)
            {
                tempRot.y = 360f;
            }
            tempRotRaw.eulerAngles = tempRot;
        }
        Vector3 move = new Vector3(0 + rightMove - leftMove, 0, 0 + forwardMove - backwardMove);
        //Debug.Log("move: " + move);
        move = transform.TransformDirection(move);
        //move *= speed;
        transform.position = new Vector3(tempPos.x + move.x, tempPos.y + 0, tempPos.z + move.z);
        transform.rotation = tempRotRaw;
    }
}
