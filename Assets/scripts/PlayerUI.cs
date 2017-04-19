﻿using UnityEngine;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    private RectTransform thrusterFuelFill;

    [SerializeField]
    private GameObject pauseMenu;

    private PlayerController controller;

    private void Start()
    {
        PauseMenu.isOn = false;
    }

    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }

    private void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }

    public void SetPlayerController(PlayerController _controller)
    {
        controller = _controller;
    }

}
