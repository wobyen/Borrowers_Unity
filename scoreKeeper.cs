using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class scoreKeeper : MonoBehaviour
{
    // Start is called before the first frame update    

    public EventsManager eventsManager;
    private TextMeshProUGUI score;


    private void Awake()
    {
        eventsManager = GameObject.Find("EventsManager").GetComponent<EventsManager>();
        score = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        string chickensKilled = eventsManager.chickensKilled.ToString();
        score.text = chickensKilled;
    }

    private void Update()
    {
        string chickensKilled = eventsManager.chickensKilled.ToString();
        score.text = chickensKilled;

    }
}
