using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    
    public DialogueRunner dialogueRunner;

    // XP card related
    public List<XPcard> xpCards; //to manually add all xp cards that should be in the system
    public Dictionary<string, XPcard> all_possible_cards_dict = new Dictionary<string, XPcard>(); //for easier access and retrieval

    private List<string> collected_cards = new List<string>(); //stores names of currently collected cards
    private List<XPcard> current_xpCards = new List<XPcard>(); //stores card_object of currently collected cards
    public TextMeshProUGUI cardcount;

    [SerializeField] List<GameObject> cardlist = new List<GameObject>(); //not sure why this exists, used to show cards in inventory

    // UI stuff
    public GameObject card_content_field; //where cards should be displayed
    public GameObject card_container_prefab; //a new one will be instantiated for each card to display & added to card_content_field
    public Sprite placeholder;
    
    [SerializeField] TextMeshProUGUI txt_emotionxp, txt_perspectivexp, txt_healthxp, txt_selfadvocacyxp;
    [SerializeField] Text dialogtxt_emotionxp, dialogtxt_perspectivexp, dialogtxt_healthxp, dialogtxt_selfadvocacyxp;

    // emotion variables
    public int emotion;
    public int perspective;
    public int health;
    public int selfadvocacy;

    // other
    public List<GameObject> collected_items;

    public void LoadAllCards()
    {
        //load all added xpcards into dictionary for easier access
        for (int i = 0; i < xpCards.Count; i++)
        {
            all_possible_cards_dict[xpCards[i].name] = xpCards[i];
        }
        Debug.Log("Loaded all xp cards!");
    }

    //Add cards to inventory (no duplicates)
    public void AddXPCard(string new_xpcardname)
    {
        //if it's not already in the collection, add it and update XPs
        if (!collected_cards.Contains(new_xpcardname))
        {
            collected_cards.Add(new_xpcardname);
            XPcard new_xpcard = all_possible_cards_dict[new_xpcardname];
            current_xpCards.Add(new_xpcard);
            UpdateAndDisplayXPs(new_xpcard);
        }
    }
    private void UpdateAndDisplayXPs(XPcard new_xpcard) //Called by DialogUI whenever a new XP card is added
    {
        emotion += new_xpcard.xpEmotion;
        perspective += new_xpcard.xpPerspective;
        health += new_xpcard.xpHealth;
        selfadvocacy += new_xpcard.xpSelfAdvocacy;
        DisplayXPs();
    }

    public void AddXPs(string skill, int amount)
    {
        if (skill == "emotion")
            emotion += amount;
        else if (skill == "perspective")
            perspective += amount;
        else if (skill == "health")
            health += amount;
        else
            selfadvocacy += amount;
    }

    public void DisplayXPs()
    {
        txt_emotionxp.text = emotion.ToString();
        dialogtxt_emotionxp.text = "XP E: "+emotion.ToString();
        txt_perspectivexp.text = perspective.ToString();
        dialogtxt_perspectivexp.text = "XP P: " + perspective.ToString();
        txt_healthxp.text = health.ToString();
        dialogtxt_healthxp.text = "XP H: " + health.ToString();
        txt_selfadvocacyxp.text = selfadvocacy.ToString();
        dialogtxt_selfadvocacyxp.text = "XP S: " + selfadvocacy.ToString();
    }

    public void ShowXPCards() //Called whenever inventory is opened
    {
        //delete all contents from last load
        cardlist = new List<GameObject>();
        for (int i = card_content_field.transform.childCount - 1; i > 0; i--)
        {
            GameObject.Destroy(card_content_field.transform.GetChild(i).gameObject);
        }

        //For debugging
        //current_xpCards.Add(xpCards[0]);
        //current_xpCards.Add(xpCards[1]);
        //current_xpCards.Add(xpCards[2]);
        //for each item in all xpCards, show actual card if it is in current_xpCards, else show placeholder
        int gotcards = 0;
        for (int i = 0; i < xpCards.Count; i++)
        {
            Debug.Log(xpCards[i].cardname);
            // create new instance
            GameObject new_card = Instantiate(card_container_prefab, card_content_field.transform);

            if (current_xpCards.Contains(xpCards[i]))
            {
                new_card.GetComponent<Image>().sprite = xpCards[i].cardpic;
                gotcards += 1;
            }
            else
                new_card.GetComponent<Image>().sprite = placeholder;

            cardlist.Add(new_card);
        }

        Debug.LogFormat("Found {0} of {1} XPCards",gotcards, xpCards.Count);
        cardcount.text = "Found "+ gotcards.ToString()+" of "+ xpCards.Count.ToString() +" XPCards";

    }
}
