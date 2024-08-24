using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

public class Building
{
    private List<Room> rooms { get; set; }

    public Building(List<Room> _rooms)
    {
        rooms = _rooms;
    }

    public Building(string path)
    {
        JSONToBuilding(path);
    }

    public void BuildingToJSON(string path)
    {
        using (StreamWriter file = File.CreateText(path))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, rooms);
        }
    }

    public void JSONToBuilding(string path)
    {
        using (StreamReader sr = new StreamReader(path))
        {
            string json = sr.ReadToEnd();
            rooms = JsonConvert.DeserializeObject<List<Room>>(json);
        }
    }

    public int GetRoomsCount()
    {
        return rooms.Count;
    }

    public Room GetRoomByID(int id)
    {
        foreach (Room room in rooms)
        {
            if (room.GetRoomID() == id)
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
    string name { get; set; }
    
    [JsonProperty("id")]
    int id { get; set; }

    [JsonProperty("north")]
    int northRoom { get; set; }

    [JsonProperty("east")]
    int eastRoom { get; set; }

    [JsonProperty("south")]
    int southRoom { get; set; }

    [JsonProperty("west")]
    int westRoom { get; set; }

    [JsonProperty("width")]
    int width { get; set; }
    
    [JsonProperty("height")]
    int height { get; set; }
    
    [JsonProperty("items")]
    List<Item> items { get; set; }
    
    [JsonProperty("interactables")]
    List<Interactable> interactables { get; set; }

    public Room(string _name, int _id, int _width, int _height, List<Item> _items, List<Interactable> _interactables)
    {
        name = _name;
        id = _id;
        width = _width;
        height = _height;
        items = _items;
        interactables = _interactables;
    }

    public int GetRoomID()
    {
        return id;
    }
    public int GetItemCount()
    {
        return items.Count;
    }
    public int GetInteractableCount()
    {
        return interactables.Count;
    }
    public Vector2 GetRoomSize() 
    { 
        return new Vector2(width, height);
    }

    public int[] GetConnectedRooms()
    {
        return new int[] { northRoom, eastRoom, southRoom, westRoom };
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

    public bool FindInteractable(Vector2 position)
    {
        Interactable interactable;
        return FindInteractable(position, out interactable);
    }
    public bool FindInteractable(Vector2 position, out Interactable interactableFound)
    {
        interactableFound = new Interactable();
        foreach (Interactable interactable in interactables)
        {
            if (interactable.GetPosition() == position)
            {
                interactableFound = interactable;
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
        return FindInteractable(new Vector2(xPos, yPos), out interactableFound);
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
            if (interactable.GetName() == name)
            {
                interactableFound = interactable;
                return true;
            }
        }
        return false;
    }
}

[JsonObject("Interactable")]
public class Interactable
{
    [JsonProperty("name")]
    string name { get; set; }

    [JsonProperty("id")]
    int id { get; set; }

    [JsonProperty("xPos")]
    int xPos { get; set; }

    [JsonProperty("yPos")]
    int yPos { get; set; }

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

    public Vector2 GetPosition()
    {
        return new Vector2(xPos, yPos);
    }

    public string GetName()
    {
        return name;
    }

    public int GetID()
    {
        return id;
    }
}

[JsonObject("Item")]
public class Item
{
    [JsonProperty("name")]
    public string name { get; set; }
    
    [JsonProperty("description")]
    public string description { get; set; }

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("xPos")]
    public int xPos { get; set; }

    [JsonProperty("yPos")]
    public int yPos { get; set; }

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

    public string GetName()
    {
        return name;
    }

    public int GetID()
    {
        return id;
    }
}