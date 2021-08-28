using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;
using System.Linq;

public class DialogUI : Singleton<DialogUI>
{
#pragma warning disable 0649
    [SerializeField] Image speakerPortrait;
    [SerializeField] Sprite invisible;
    [SerializeField] TextMeshProUGUI txt_Dialog, txt_SpeakerName;
    [SerializeField] TextMeshProUGUI txt_Helper;
    [SerializeField] GameObject helper_panel;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] List<Sprite> backgrounds;
    [SerializeField] List<GameObject> collectable_gameobjects= new List<GameObject>();
    [SerializeField] Image backgroundImage;
    public TextMeshProUGUI coping_strategy_note;
    public GameObject bgdarken;
    [SerializeField] GameObject darkener;
    [SerializeField] Image cardImage;
    [SerializeField] GameObject writtentext_panel;
    [SerializeField] TextMeshProUGUI namefield;
    [SerializeField] GameObject energysign;
    public AudioSource bg_sound;
    public AudioSource soundeffect;
    public GameObject RPGUI;

    #pragma warning restore 0649

    public DialogueRunner Runner { get; private set; }
    public DialogueRunner dialogueRunner;
    
    public HealthBar healthBar;
    public int energyPoints;
    private int maxEnergy = 30;
    public Text energydisplay;

    public List<string> collected_xpCards = new List<string>(); //tracks all cards that have been collected in the dialogue through yarn commands
    public List<string> events_done = new List<string>(); //events that have happened in the dialogue, singalled through yarn commands

    Dictionary<string, SpeakerData> speakerDatabase = new Dictionary<string, SpeakerData>();

    private string compositetext; //for whenever a text is built through option buttons within an interaction.
    private List<string> tasklist = new List<string>();
    public TextMeshProUGUI tasklist_display;

    private void Awake()
    {
        energydisplay.text = "Energy: " + energyPoints.ToString();
        Runner = GetComponent<DialogueRunner>();
        Runner.AddCommandHandler("SetSpeaker", SetSpeakerInfo);
        Runner.AddCommandHandler("OpenChat", OpenChat);
        Runner.AddCommandHandler("ChatMessage", ChatMessage);
        Runner.AddCommandHandler("CloseChat", CloseChat);
        Runner.AddCommandHandler("SetNameKnown", SetNameKnown);
        Runner.AddCommandHandler("NoSpeakerPic", RemoveSpeakerPic);
        Runner.AddCommandHandler("NoSpeakerName", RemoveSpeakerName);
        Runner.AddCommandHandler("HelperSays", SetHelperText);
        Runner.AddCommandHandler("Energy", SetEnergyLevel);
        Runner.AddCommandHandler("Relationship", SetRelationship);
        Runner.AddCommandHandler("WriteLong", WriteCompositeText);
        Runner.AddCommandHandler("ItemToInventory", AddItemToInventory);
        Runner.AddCommandHandler("ClearWriteLong", ClearCompositeText);
        Runner.AddCommandHandler("SetAudio", SetAudio);
        compositetext = " ";
        Runner.AddCommandHandler("AddXPCard", AddXPCard);
        Runner.AddCommandHandler("AddNewTask", AddNewTask);
        Runner.AddCommandHandler("AddCopingStrategy", AddCopingStrategy);
        Runner.AddCommandHandler("RemoveTask", RemoveTask);
        Runner.AddFunction("check_task", 1, delegate (Yarn.Value[] parameters)
        {
            string item = "";
            for (int i = 0; i < parameters.Length; i++)
            {
                item+=parameters[0].AsString+" ";
            }
            Debug.Log("Check task on list: " + item);
            return tasklist.Contains(item);
        });
        Runner.AddFunction("skill_check", 2, delegate (Yarn.Value[] parameters)
        {
            string required_skill = parameters[0].AsString;
            int difficulty_level = int.Parse(parameters[1].AsString);
            Debug.LogFormat("skill {0} and difficulty {1}",required_skill,difficulty_level);
            
            // get current energy for scaler & current xp for required_skill
            float scaler = (100 - (30 - energyPoints)) / 100;
            int skill_xp;
            if (required_skill == "emotion")
                skill_xp = inventoryManager.emotion;
            else if (required_skill == "perspective")
                skill_xp = inventoryManager.perspective;
            else if (required_skill == "health")
                skill_xp = inventoryManager.health;
            else
                skill_xp = inventoryManager.selfadvocacy;

            //calculate chance
            Debug.Log(energyPoints);
            float chance_of_success = (((100f - (30f - energyPoints)) / 100f) * skill_xp) / difficulty_level;
            Debug.Log("chance " + chance_of_success.ToString());

            //get success/failure with random int and chance
            float randNum = Random.value;
            Debug.Log("random was " + randNum.ToString());
            if (randNum <= chance_of_success)
            {
                Debug.Log("success!");
                return true;
            }
            else
                return false;
        });
        Runner.AddCommandHandler("TriggerScript", TriggerScript);
        Runner.AddCommandHandler("LogEvent", LogEvent);
        Runner.AddCommandHandler("SetBackground", SetBackgroundImage);
        Runner.AddCommandHandler("RemoveBackground", RemoveBackgroundImage);
        Runner.AddCommandHandler("BGFilter", DarkFilter);
        //Runner.AddFunction("get_name", 1, delegate (Yarn.Value[] parameters)
        //{
        //    return namefield.text;
        //});
        Runner.AddFunction("check_energy", 1, delegate (Yarn.Value[] parameters)
        {
            Debug.Log(parameters[0].AsString);
            return energyPoints;
        });
        Runner.AddFunction("check_relationship", 1, delegate (Yarn.Value[] parameters)
        {
            string npcname = parameters[0].AsString;
            NPC target = GameObject.Find(npcname).GetComponent<NPC>();
            return target.relationship_to_player;
        });
        
        Runner.AddFunction("check_card", 1, delegate (Yarn.Value[] parameters)
        {
            string cardinquestion = parameters[0].AsString;
            Debug.Log(parameters[0].AsString);
            if (collected_xpCards.Contains(cardinquestion))
                return true;
            else
                return false;
        });
        
        Runner.AddFunction("check_event", 1, delegate (Yarn.Value[] parameters)
        {
            string eventinquestion = parameters[0].AsString;
            Debug.Log(parameters[0].AsString);
            if (events_done.Contains(eventinquestion))
                return true;
            else
                return false;
        });
    }

    void OpenChat(string[] chatname)
    {
        string new_chatname = chatname[0];
        var chatui = GameObject.Find("Chatpanel");
        if (chatui)
        {
            chatui.GetComponent<ChatUI>().SetChatname(new_chatname);
            //chatui.SetActive(true);
        }
        var chatvisible = GameObject.Find("visible");
        if (chatvisible)
            chatvisible.SetActive(true);
    }

    void ChatMessage(string[] message)
    {
        string speaker = message[0];
        string chatmessage = string.Join(" ", message);
        Debug.Log(chatmessage);
        var chatui = GameObject.Find("Chatpanel");
        if (chatui)
        {
            chatui.GetComponent<ChatUI>().SendMessageToChat(speaker, chatmessage);
        }
    }

    void CloseChat(string[] info)
    {
        var chatui = GameObject.Find("visible");
        if (chatui)
            chatui.SetActive(false);
    }

    void DarkFilter(string[] on_off_description)
    {
        if (on_off_description[0] == "on")
        {
            RPGUI.SetActive(true);
            Debug.Log("rpg should be gone");
            bgdarken.SetActive(true);
        }
        if (on_off_description[0] == "off")
        {
            bgdarken.SetActive(false);
            RPGUI.SetActive(false);
        }
    }
    
    void AddNewTask(string[] description)
    {
        string newtask = string.Join(" ", description);
        tasklist.Add(newtask);
        tasklist_display.text = string.Join("\n", tasklist);
        Debug.Log("Added task: " + newtask);
    }

    void AddCopingStrategy(string[] description)
    {
        string newstrategy = string.Join(" ", description) + "\n-";
        coping_strategy_note.text += newstrategy;
    }

    void RemoveTask(string[] description)
    {
        tasklist.RemoveAll(x => ((string)x) == string.Join(" ", description));
        tasklist_display.text = string.Join("\n", tasklist);
    }

    
    private void MoveObjectAway(string objectname)
    {
        var thescript = GameObject.Find(objectname).GetComponent<EnableDisableObjects>();
        if (thescript)
        {
            thescript.MoveObjectFromScene();
            thescript.ActivateDeactivateObjects();
        }
        else
            Debug.LogFormat("Couldn't find EnableDisableObjects in {0}", objectname);
    }
    
    void TriggerScript(string[] description)
    {
        if (description[0] == "intro_end")
            MoveObjectAway("Ghostcat");

        if (description[0] == "kids_leave")
        {
            MoveObjectAway("Martin");
            MoveObjectAway("Amy");
        }

        if (description[0] == "lotte_leaves")
            MoveObjectAway("Lotte");

        if (description[0] == "jasmijn_leaves")
            MoveObjectAway("Jasmijn");

        if (description[0] == "darrel_leaves")
            MoveObjectAway("Darrel");

        if (description[0] == "exit_cave")
        {
            string[] audioinput = new string[] { "bg_house", "bg" };
            SetAudio(audioinput);

            var player = GameObject.Find("Player");
            if (player)
                player.transform.position = new Vector3(-7, 8, 0);
        }
        if (description[0] == "visit_guardians")
        {
            string[] audioinput = new string[] { "guardians", "bg" };
            SetAudio(audioinput);

            var player = GameObject.Find("Player");
            if (player)
                player.transform.position = new Vector3(-15, -21, 0);
        }
        if (description[0] == "visit_stimshop")
        {
            bg_sound.Stop();

            var player = GameObject.Find("Player");
            if (player)
                player.transform.position = new Vector3(-43, 14, 0);
        }
        if (description[0] == "exit_stimshop")
        {
            string[] audioinput = new string[] { "bg_island", "bg" };
            SetAudio(audioinput);

            var player = GameObject.Find("Player");
            Debug.Log("exited stimshop");
            if (player)
                player.transform.position = new Vector3(-2, -2, 0);
        }
    }

    void AddItemToInventory(string[] itemname)
    {
        Debug.Log("Letter should be gone from scene");
        //int index = collectable_gameobjects.FindIndex(0, s => s.name == itemname[0]);
        //collectable_gameobjects[index].SetActive(false);

        //get item from scene directly:
        GameObject.Find(itemname[0]).SetActive(false);
        //inventoryManager.collected_items.Add(collectable_gameobjects[index]);
    }

    void SetBackgroundImage(string[] imagename)
    {
        int index = backgrounds.FindIndex(0, s => s.name == imagename[0]);
        backgroundImage.sprite = backgrounds[index];
    }

    void WriteCompositeText(string[] textpiece)
    {
        compositetext += ' ' + string.Join(" ", textpiece);
        writtentext_panel.GetComponentInChildren<TextMeshProUGUI>().text = compositetext;
        writtentext_panel.SetActive(true);
        return;
    }
    void ClearCompositeText(string[] input)
    {
        compositetext = " ";
    }

    void RemoveBackgroundImage(string[] info)
    {
        backgroundImage.sprite = invisible;
    }

    int GetSkillLevel(string used_skill)
    {
        if (used_skill == "emotion")
            return inventoryManager.emotion;
        else if (used_skill == "perspective")
            return inventoryManager.perspective;
        else if (used_skill == "health")
            return inventoryManager.health;
        else if (used_skill == "selfadvocacy")
            return inventoryManager.selfadvocacy;
        else
            Debug.LogError("No recognized skill");
            return 0;
    }

    public void SetEnergyLevel(string[] energydiff)
    {
        int difference = int.Parse(energydiff[0]);
        energyPoints += difference;
        // There is an influence on energy reduction for an increasing skill level.
        // The second parameter in SetEnergyLevel is the skill to which the action is related, e.g. emotions.
        // The energy loss is moderated by the level of that skill

        if (energyPoints < 0)
        {
            string used_skill = energydiff[1];
            //check current skill points for that skill
            energyPoints = energyPoints - GetSkillLevel(used_skill);
            Debug.LogFormat("Tried to remove {0} energy points, after looking at skill level {1}", energyPoints, used_skill);
        }
        
        if (energyPoints > maxEnergy)
        {
            energyPoints = maxEnergy;
        }
        if (energyPoints < 0)
        {
            energyPoints = 0;
        }
        healthBar.SetHealth(energyPoints);
        Debug.LogFormat("Energy level is at {0}", energyPoints);
        energydisplay.text = "Energy: "+energyPoints.ToString();

        //show energy notif in dialogue
        if (difference > 0)
            energysign.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        else
            energysign.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        
        energysign.GetComponentInChildren<TextMeshProUGUI>().text = difference.ToString();
        energysign.SetActive(true);

    }

    void SetRelationship(string[] parameters)
    {
        //find npc whose relationship changed
        string npcname = parameters[1];
        NPC target = GameObject.Find(npcname).GetComponent<NPC>();
        //apply change
        target.relationship_to_player = parameters[0];

        Debug.LogFormat("Relationship with NPC {0} is {1}", npcname,target.relationship_to_player);
    }

    public void AddXPCard(string[] cardname)
    {
        inventoryManager.AddXPCard(cardname[0]);

        //show correct image
        //int index = inventoryManager.xpCards.FindIndex(0, s => s.cardname == cardname[0]);
        //cardImage.sprite = inventoryManager.xpCards[index].cardpic;
        cardImage.sprite = inventoryManager.all_possible_cards_dict[cardname[0]].cardpic;
        darkener.SetActive(true);

        //play sound effect
        soundeffect.GetComponent<AudioclipManager>().PlayOnce("friendly_open", soundeffect);
    }

    public void LogEvent(string[] eventname)
    {
        events_done.Add(eventname[0]);
        events_done.Distinct().ToList();
    }

    public void AddSpeaker(SpeakerData data)
    {
        if (speakerDatabase.ContainsKey(data.speakerName))
        {
            Debug.LogWarningFormat("Attempting to add {0} into speaker database, but it already exists!", data.speakerName);
            return;
        }
        //Add
        speakerDatabase.Add(data.speakerName, data);
    }

    void SetNameKnown(string[] speakername)
    {
        // set speaker pic and text if possible
        if (speakerDatabase.TryGetValue(speakername[0], out SpeakerData data))
        {
            data.player_knows_name = true;
        }
    }

    void SetSpeakerInfo(string[] info)
    {
        string speaker = info[0];

        // if info[0] is "none", set transparent speaker pic and no name field
        if (speaker == "none")
        {
            txt_SpeakerName.text = "Narrator";
            speakerPortrait.sprite = invisible;
        }
        else
        {
            // check if speaker pic is default or a different one
            string picturedescription;
            if (info.Length > 1)
                picturedescription = info[1].ToLower(); //currently only other is "alternative"
            else
                picturedescription = SpeakerData.DEFAULTPIC;

            // set speaker pic and text if possible
            if (speakerDatabase.TryGetValue(speaker, out SpeakerData data))
            {
                // don't set portrait if yarn command says "pictureless"
                speakerPortrait.sprite = data.GetCorrectPicture(picturedescription);
                if (data.player_knows_name)
                    txt_SpeakerName.text = data.speakerName;
                else
                    txt_SpeakerName.text = "???";
                return;
            }
            Debug.LogErrorFormat("Could not set speaker info for unknown speaker {0}", speaker);
        }
    }

    void RemoveSpeakerPic(string[] info)
    {
        speakerPortrait.sprite = invisible;
    }

    void SetAudio(string[] info)
    {
        // add list of audio sounds to bg and effect audio objects with manager
        string audioclip_name = info[0];
        if (info[1] == "bg")
        {
            bg_sound.GetComponent<AudioclipManager>().PlayOnLoop(audioclip_name, bg_sound);
        }
        else if (info[1] == "effect")
        {
            soundeffect.GetComponent<AudioclipManager>().PlayOnce(audioclip_name, soundeffect);
        }
    }

    void RemoveSpeakerName(string[] info)
    {
        txt_SpeakerName.text = info[0];
    }

    void SetHelperText(string[] helpertext)
    {
        helper_panel.SetActive(true);
        Debug.Log("Setting helper text");
        string content = string.Join(" ", helpertext);
        txt_Helper.text = content;
        return;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
