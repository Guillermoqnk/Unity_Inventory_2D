using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject[,] Inventory_Slots = new GameObject[6, 3];
    public GameObject[,] Inventory_Objects = new GameObject[6, 3];
    bool[,] Inventory_Slots_Used = new bool[6, 3];

    public GameObject[] Items;
    public GameObject selector;
    private GameObject pickingUpItem;

    private int selectorX = 0;
    private int selectorY = 0;

    private Vector3 goingPoint = new Vector3(15, 15, 0);

    private Text selectedItemName;

    List<GameObject> ItemsOnInventory = new List<GameObject>();

    void Start()
    {
        int num = 0;

        for (int i = 0; i < 3; i++) //This fuction orders each slot into the matrice
        {
            for (int u = 0; u < 6; u++)
            {
                num++;
                Inventory_Slots[u, i] = GameObject.Find("Slot_" + num);
                Inventory_Slots_Used[u, i] = false;
            }
        }

        selectedItemName = GameObject.Find("SelectedItem_Name").GetComponent<Text>();
        NewRandomItems();
        pickingUpItem = null;
    }


    void Update()
    {
        //Since I want a clearer code here I only want the inputs
        //Moving the selector
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelector("right");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelector("left");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelector("up");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelector("down");
        }


        //Grab and move items
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ItemInteraction();
        }


        //Delete items
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DeleteItem();
        }


        //New items
        if (Input.GetKeyDown(KeyCode.C))
        {
            NewRandomItems();
        }
    }


    void NewRandomItems()
    {
        if (ItemsOnInventory.Count > 0)
        {
            for (int i = 0; i < 3; i++) //This fuction orders each slot into the matrice
            {
                for (int u = 0; u < 6; u++)
                {
                    if (Inventory_Slots_Used[u, i] == true)
                    {
                        ItemsOnInventory.Remove(Inventory_Objects[u, i]);
                        //Inventory_Objects[u, i].transform.Translate(goingPoint); //Cambiar por corutina o añadir
                        StartCoroutine(RemoveItem(Inventory_Objects[u, i]));
                        Inventory_Objects[u, i] = null;
                        Inventory_Slots_Used[u, i] = false;
                    }
                }
            }
        }
        while (ItemsOnInventory.Count < 5)
        {
            int random = Random.Range(0, Items.Length);

            if (!ItemsOnInventory.Contains(Items[random]))
            {
                ItemsOnInventory.Add(Items[random]);

                while (true)
                {
                    int randomPositionY = Random.Range(0, 2);
                    int randomPositionX = Random.Range(0, 5);

                    if (Inventory_Slots_Used[randomPositionX, randomPositionY] == false)
                    {
                        Inventory_Slots_Used[randomPositionX, randomPositionY] = true;
                        Inventory_Objects[randomPositionX, randomPositionY] = Items[random];
                        Items[random].transform.position = Inventory_Slots[randomPositionX, randomPositionY].transform.position;
                        break;
                    }
                }
            }
        }
        ChangeName();
    }

    //Movement of the selectioner
    void MoveSelector(string direction)
    {
        switch (direction)
        {
            case "right":
                {
                    if (selectorX < 5)
                    {
                        selectorX++;
                        selector.transform.position = Inventory_Slots[selectorX, selectorY].transform.position;
                    }
                    break;
                }
            case "left":
                {
                    if (selectorX > 0)
                    {
                        selectorX--;
                        selector.transform.position = Inventory_Slots[selectorX, selectorY].transform.position;
                    }
                    break;
                }
            case "up":
                {
                    if (selectorY > 0)
                    {
                        selectorY--;
                        selector.transform.position = Inventory_Slots[selectorX, selectorY].transform.position;
                    }
                    break;
                }
            case "down":
                {
                    if (selectorY < 2)
                    {
                        selectorY++;
                        selector.transform.position = Inventory_Slots[selectorX, selectorY].transform.position;
                    }
                    break;
                }
        }

        /*if (pickingUpItem != null)
        {
            pickingUpItem.transform.position = selector.transform.position;
        }*/

        ChangeName();
    }




    //Here we change the text at the bottom corner right into the name of the item at the slot
    void ChangeName()
    {
        if (Inventory_Slots_Used[selectorX, selectorY] == true)
        {
            selectedItemName.text = Inventory_Objects[selectorX, selectorY].name;
        }
        else
        {
            selectedItemName.text = "Empy";
        }
    }

    IEnumerator RemoveItem(GameObject Item)
    {

        while (Item.transform.position != goingPoint)
        {
            Item.transform.position = Vector3.MoveTowards(Item.transform.position, goingPoint, 0.5f);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

    }


    //Rotation of resolutions
    void ChangeResolution()
    {
        switch (Screen.height)
        {
            case 1280:
                {
                    Screen.SetResolution(1920, 1080, Screen.fullScreen);
                    break;
                }
            case 1920:
                {
                    Screen.SetResolution(3860, 2160, Screen.fullScreen);
                    break;
                }
            case 3860:
                {
                    Screen.SetResolution(1280, 720, Screen.fullScreen);
                    break;
                }
        }
    }


    //Pick up and down items
    void ItemInteraction()
    {
        if (pickingUpItem == null && Inventory_Slots_Used[selectorX, selectorY] == true)
        {
            pickingUpItem = Inventory_Objects[selectorX, selectorY];
            pickingUpItem.transform.SetParent(selector.transform);
            Inventory_Slots_Used[selectorX, selectorY] = false;
            Inventory_Objects[selectorX, selectorY] = null;
            selectedItemName.text = "Empy";
            pickingUpItem.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        else if (pickingUpItem != null && Inventory_Slots_Used[selectorX, selectorY] == true)
        {
            GameObject trans;
            trans = Inventory_Objects[selectorX, selectorY];
            Inventory_Objects[selectorX, selectorY] = pickingUpItem;
            pickingUpItem.GetComponent<SpriteRenderer>().sortingOrder = 0;
            pickingUpItem = trans;
            pickingUpItem.GetComponent<SpriteRenderer>().sortingOrder = 1;
            pickingUpItem.transform.SetParent(selector.transform);
            Inventory_Objects[selectorX, selectorY].transform.SetParent(Inventory_Slots[selectorX, selectorY].transform);
            Inventory_Objects[selectorX, selectorY].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            selectedItemName.text = Inventory_Objects[selectorX, selectorY].name;
        }

        else if (Inventory_Slots_Used[selectorX, selectorY] == false && pickingUpItem != null)
        {
            pickingUpItem.transform.SetParent(Inventory_Slots[selectorX, selectorY].transform);
            Inventory_Slots_Used[selectorX, selectorY] = true;
            Inventory_Objects[selectorX, selectorY] = pickingUpItem;
            pickingUpItem.GetComponent<SpriteRenderer>().sortingOrder = 0;
            Inventory_Objects[selectorX, selectorY].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            pickingUpItem = null;
            selectedItemName.text = Inventory_Objects[selectorX, selectorY].name;
        }
    }


    //Delete selected item
    void DeleteItem()
    {
        pickingUpItem.transform.position = goingPoint;
        pickingUpItem.transform.SetParent(Inventory_Slots[selectorX, selectorY].transform);
        ItemsOnInventory.Remove(pickingUpItem);
        pickingUpItem = null;
    }
}
