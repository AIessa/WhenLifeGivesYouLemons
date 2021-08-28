using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SaveLoad : MonoBehaviour
{
    public InMemoryVariableStorage variableStorage;
    public DialogueRunner dialogueRunner;
    public string externalJson = "";
    public InventoryManager inventoryManager;
    public SaveScreenVisuals saveScreenVisuals;

// Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save()
    {
        foreach (var variable in variableStorage)
        {
            string name = variable.Key;
            Yarn.Value value = variable.Value;
            Debug.LogFormat("Variable {0} saved with value {1}", name, value);
        }

        // Compile updated player info
        PlayerInfo saveData = new PlayerInfo();
        saveData.energyLevel = DialogUI.Instance.energyPoints;
        saveData.inventoryCards = DialogUI.Instance.collected_xpCards;
        saveData.collectedItems = new List<string>();
        saveData.eventsDone = new List<string>();
        NPC[] npcs = FindObjectsOfType<NPC>();
        saveData.relationships = new Dictionary<string, string>();
        for (int i = 0; i < npcs.Length; i++)
        {
            saveData.relationships[npcs[i].name] = npcs[i].relationship_to_player;
        }
        Dictionary<string, int> skillxps = new Dictionary<string, int>();
        skillxps["emotion"] = inventoryManager.emotion;
        skillxps["perspective"] = inventoryManager.perspective;
        skillxps["health"] = inventoryManager.health;
        skillxps["selfadvocacy"] = inventoryManager.selfadvocacy;
        saveData.skillxps = skillxps;

        //Save data from PlayerInfo to a file named players
        DataSaver.saveData(saveData, "gamedata");
        Debug.Log("Data Saved");

        //Send everything to savescreenvisuals for display to player
        saveScreenVisuals.DisplayStatsWhileSaving(inventoryManager.cardcount.text.ToString(), skillxps ,saveData.relationships);
    }

    public void Load()
    {
        PlayerInfo loadedData = DataSaver.loadData<PlayerInfo>("gamedata");
        if (loadedData == null)
        {
            return;
        }

        // set all variables to correct values
        DialogUI.Instance.energyPoints = loadedData.energyLevel;
        DialogUI.Instance.collected_xpCards = loadedData.inventoryCards;
        inventoryManager.emotion = loadedData.skillxps["emotion"];
        inventoryManager.perspective = loadedData.skillxps["perspective"];
        inventoryManager.health = loadedData.skillxps["health"];
        inventoryManager.selfadvocacy = loadedData.skillxps["selfadvocacy"];

        for (int i = 0; i < DialogUI.Instance.collected_xpCards.Count; i++)
        {
            Debug.Log("card: " + DialogUI.Instance.collected_xpCards[i]);
        }

        for (int i = 0; i < loadedData.ID.Count; i++)
        {
            Debug.Log("ID: " + loadedData.ID[i]);
        }

        // Fix UI stuff
        inventoryManager.DisplayXPs();
    }

    public void Delete()
    {
        DataSaver.deleteData("gamedata");
    }
}

[System.Serializable]
public class PlayerInfo
{
    public List<int> ID = new List<int>();
    public int energyLevel = 0;
    public List<string> inventoryCards = new List<string>();
    public List<string> collectedItems = new List<string>();
    public List<string> eventsDone = new List<string>();
    public Dictionary<string, string> relationships = new Dictionary<string, string>();
    public Dictionary<string, int> skillxps = new Dictionary<string, int>();
}
