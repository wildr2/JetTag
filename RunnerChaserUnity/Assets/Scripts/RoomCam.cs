using UnityEngine;
using System.Collections;

public class RoomCam : MonoBehaviour
{
    public Transform[] rooms;
    public Transform agent;
    private int room_i = 0;


    private void Update()
    {
        if (room_i == 0 && agent.transform.position.y > 0)
        {
            room_i = 1;
            Vector3 pos = rooms[1].transform.position;
            pos.z = Camera.main.transform.position.z;
            Camera.main.transform.position = pos;
        }
        if (room_i == 1 && agent.transform.position.y < 0)
        {
            room_i = 0;
            Vector3 pos = rooms[0].transform.position;
            pos.z = Camera.main.transform.position.z;
            Camera.main.transform.position = pos;
        }
    }

}
