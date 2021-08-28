using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSelfDialogueImmediately : MonoBehaviour
{
    public string YarnStartNode { get { return yarnStartNode; } }
    [SerializeField] string yarnStartNode = "Start";
    [SerializeField] YarnProgram yarnDialog;
    // Start is called before the first frame update

    private void Start()
    {
        DialogUI.Instance.dialogueRunner.Add(yarnDialog);
    }
    public void StartThisDialogueNow()
    {
        DialogUI.Instance.dialogueRunner.StartDialogue(yarnStartNode);
    }
}
