using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public DialogueRunner dialogueRunner;
    public GameObject shutdownUI;
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        ColourCheck();
    }
    public void SetHealth(int health)
    {
        slider.value = health;
        ColourCheck();
    }

    private void ColourCheck()
    {
        if (slider.value <= 10)
            this.transform.Find("Fill").GetComponent<Image>().color = Color.red;
        else
            this.transform.Find("Fill").GetComponent<Image>().color = Color.cyan;

        if (slider.value <= 0)
            Shutdown();

        if (slider.value == 30)
        {
            var player = GameObject.Find("Player");
            if (player)
            {
                player.GetComponent<PlayerController>().SetMoveSpeed(4);
            }
        }
    }

    private void Shutdown()
    {
        // Any running dialogue should be stopped
        //if (dialogueRunner.IsDialogueRunning)
        //    dialogueRunner.Stop();

        // Show shut-down screen
        shutdownUI.SetActive(true);

        // Transfer player back home
        var player = GameObject.Find("Player");
        if (player)
        {
            player.transform.position = new Vector3(-7, 8, 0);
            player.GetComponent<PlayerController>().SetMoveSpeed(1);
        }

        //Give player 5 energy back
        var dialogUI = GameObject.Find("Dialog UI");
        string[] energy = new string[] {"5"};
        if (dialogUI)
            dialogUI.GetComponent<DialogUI>().SetEnergyLevel(energy);
    }
}
