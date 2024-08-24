using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;

public class Building
{
    public List<Room> rooms { get; }

    public Building(string path)
    {
        using (StreamReader sr = new StreamReader(path))
        {
            string json = sr.ReadToEnd();
            rooms = JsonConvert.DeserializeObject<Building>(json).rooms;
        }
    }

    public Room GetRoomByID(int id)
    {
        foreach (Room room in rooms)
        {
            if (room.id == id)
            {
                return room;
            }
        }
        return null;
    }
}

[JsonObject("Room")]
public class Room
{
    [JsonProperty("name")]
    public string name { get; }
    
    [JsonProperty("id")]
    public int id { get; }

    [JsonProperty("width")]
    public int width { get; }
    
    [JsonProperty("height")]
    public int height { get; }
    
    [JsonProperty("items")]
    public List<Item> items { get; }
    
    [JsonProperty("interactables")]
    public List<Interactable> interactables { get; }

    public Room(string _name, int _id, int _width, int _height, List<Item> _items, List<Interactable> _interactables)
    {
        name = _name;
        id = _id;
        width = _width;
        height = _height;
        items = _items;
        interactables = _interactables;
    }

    //Finding things
    public bool FindItem(int xPos, int yPos)
    {
        Item discard;
        return FindItem(xPos, yPos, out discard);
    }
    public bool FindItem(int xPos, int yPos, out Item itemFound)
    {
        itemFound = new Item();
        foreach (Item item in items)
        {
            if (item.xPos == xPos && item.yPos == yPos)
            {
                itemFound = item;
                return true;
            }
        }
        return false;
    }

    public bool FindItem(string name)
    {
        Item discard;
        return FindItem(name, out discard);
    }
    public bool FindItem(string name, out Item itemFound)
    {
        itemFound = new Item();
        foreach (Item item in items)
        {
            if (item.name == name)
            {
                itemFound = item;
                return true;
            }
        }
        return false;
    }

    public bool FindInteractable(int xPos, int yPos)
    {
        Interactable interactable;
        return FindInteractable(xPos, yPos, out interactable);
    }
    public bool FindInteractable(int xPos, int yPos, out Interactable interactableFound)
    {
        interactableFound = new Interactable();
        foreach (Interactable interactable in interactables)
        {
            if (interactable.xPos == xPos && interactable.yPos == yPos)
            {
                interactableFound = interactable;
                return true;
            }
        }
        return false;
    }

    public bool FindInteractable(string name)
    {
        Interactable interactable;
        return FindInteractable(name, out interactable);
    }
    public bool FindInteractable(string name, out Interactable interactableFound)
    {
        interactableFound = new Interactable();
        foreach (Interactable interactable in interactables)
        {
            if (interactable.name == name)
            {
                interactableFound = interactable;
                return true;
            }
        }
        return false;
    }

    //Get room size in vector2
    public Vector2 GetSizeInVector2()
    {
        return new Vector2(width, height);
    }
}

[JsonObject("Interactable")]
public class Interactable
{
    [JsonProperty("name")]
    public string name { get; }

    [JsonProperty("id")]
    public int id { get; }

    [JsonProperty("xPos")]
    public int xPos { get; }

    [JsonProperty("yPos")]
    public int yPos { get; }

    public Interactable()
    {
        name = "Default";
        id = -1;
        xPos = 0;
        yPos = 0;
    }

    public Interactable(string _name, int _id, int _xPos, int _yPos)
    {
        name = _name;
        id = _id;
        xPos = _xPos;
        yPos = _yPos;
    }

    public void Interact()
    {
        switch (id)
        {
            case 0:

                break;
            default:
                break;
        }
    }
}

[JsonObject("Item")]
public class Item
{
    [JsonProperty("name")]
    public string name { get; }
    
    [JsonProperty("description")]
    public string description { get; }

    [JsonProperty("id")]
    public int id { get; }

    [JsonProperty("xPos")]
    public int xPos { get; }

    [JsonProperty("yPos")]
    public int yPos { get; }

    public Item()
    {
        name = "Default";
        description = "Default";
        id = -1;
        xPos = 0;
        yPos = 0;
    }

    public Item(string _name, string _description, int _id, int _xPos, int _yPos)
    {
        name = _name;
        description = _description;
        id = _id;
        xPos = _xPos;
        yPos = _yPos;
    }

    public void Use()
    {

    }
}