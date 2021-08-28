using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public GameObject player;
    public GameObject dialog_bg_darken;
    public Cinemachine.CinemachineVirtualCamera camera;

    private Vector3 player_startingposition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartMaze()
    {
        player_startingposition = player.transform.position;
        player.transform.position = new Vector3(26, -24, 0);
        camera.m_Lens.OrthographicSize = 2;
        dialog_bg_darken.SetActive(false);
    }

    public void EndMaze()
    {
        player.transform.position = player_startingposition;
        camera.m_Lens.OrthographicSize = 5;
        dialog_bg_darken.SetActive(true);
    }

}
