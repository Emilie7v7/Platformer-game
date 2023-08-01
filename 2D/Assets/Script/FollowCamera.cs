using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public GameObject player;
    public float offset;
    public float offsetSmoothing;
    private Vector3 playerPosition;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        if (player.transform.position.x > 0f)
        {
            playerPosition = new Vector3(playerPosition.x + offset, playerPosition.y + offset, playerPosition.z + offset);
        }
        else
        {
            playerPosition = new Vector3(playerPosition.x - offset, playerPosition.y + offset, playerPosition.z + offset);
        }
        transform.position = Vector3.Lerp(transform.position, playerPosition, offsetSmoothing * Time.deltaTime);
    }
}
