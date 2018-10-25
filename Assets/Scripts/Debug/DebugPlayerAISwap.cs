using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlayerAISwap : MonoBehaviour {

    // When the player pressed
    public int playerNum;
    public Camera cam;
    public MonoBehaviour playerController;
    public MonoBehaviour aiController;
    public SimpleCarController carController;
    public GrapplerCharacterController grapplerController;
    public bool isStartingPlayer = false;

    private static DebugPlayerAISwap currentPlayer;

	// Use this for initialization
	void Awake () {
        if (isStartingPlayer)
        {
            currentPlayer = this;
            SwapToPlayer();
        } else
        {
            SwapToAI();
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(""+playerNum))
        {
            currentPlayer.SwapToAI();
            SwapToPlayer();
            currentPlayer = this;
        }
    }

    void SwapToPlayer()
    {
        if (playerController != null)
            playerController.enabled = true;
        if (aiController != null)
            aiController.enabled = false;
        if (carController != null)
        {
            carController.input = playerController.GetComponent<ICarControlInput>();
        }
        if (grapplerController != null)
            grapplerController.input = (GrapplerInput)playerController;
        cam.enabled = true;
    }

    void SwapToAI()
    {
        if (playerController != null)
            playerController.enabled = false;
        if (aiController != null)
            aiController.enabled = true;
        if(carController != null)
        {
            carController.input = aiController.GetComponent<ICarControlInput>();
        }
        if (grapplerController != null)
            grapplerController.input = (GrapplerInput)aiController;
        cam.enabled = false;
    }
}
