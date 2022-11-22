using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
public class InventoryDisplay : InputManager
{

    public InventoryObject inventory;

    [SerializeField] GameObject pauseMenuUI;

    [SerializeField] GameObject menuTilesPrefab;

    public List<GameObject> menuTiles = new List<GameObject>();

    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

    int inventorySize = 10;

    public bool inventoryBuilt = false;

    [SerializeField] public TextMeshProUGUI itemName;
    [SerializeField] public TextMeshProUGUI itemDesc;

    private void Awake()

    {
        //Create the prefabs in the grid
        for (int i = inventorySize; i > 0; i--)
        {
            menuTiles.Add(Instantiate(menuTilesPrefab, Vector3.zero, Quaternion.identity, transform));
        }

        CreateDisplay();

        inventoryBuilt = true;


    }



    private void Start()
    {
        //        pauseMenuUI.SetActive(false);

    }

    private void Update()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {

        for (int i = 0; i < inventory.Container.Count; i++)
        {
            if (itemsDisplayed.ContainsKey(inventory.Container[i]))
            {
                itemsDisplayed[inventory.Container[i]].GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
            }

            else
            {
                var obj = Instantiate(inventory.Container[i].item.itemInventoryDisplay, Vector3.zero, Quaternion.identity, menuTiles[i].transform);
                obj.GetComponent<RectTransform>().localPosition = Vector3.zero;
                obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.Container[i].amount.ToString("n0");
                itemsDisplayed.Add(inventory.Container[i], obj);
            }
        }
    }

    public void CreateDisplay()
    {



    }











}
