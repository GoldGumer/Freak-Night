using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.Security.Cryptography;
using System.Xml.Linq;

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

    public List<Room> GetRooms()
    {
        return rooms;
    }

    public void AddRoom(Room room, Vector2 position)
    {
        room.xPos = (int)position.X;
        room.yPos = (int)position.Y;
        rooms.Add(room);
    }

    public void AddRoom(Vector2 position)
    {
        rooms.Add(new Room((int)position.X, (int)position.Y));
    }

    public void RemoveRoom(Vector2 position)
    {
        rooms.Remove(rooms.Find(room => room.GetPosition() == position));
    }

    public void RemoveRoom(string name)
    {
        rooms.Remove(rooms.Find(room => room.name == name));
    }

    public void RemoveRoom(int id)
    {
        rooms.Remove(rooms.Find(room => room.GetRoomID() == id));
    }

    public void RemoveRoom(Room room)
    {
        rooms.Remove(room);
    }

    public bool FindRoom(Vector2 position)
    {
        Room room;
        return FindRoom(position, out room);
    }
    public bool FindRoom(Vector2 position, out Room roomFound)
    {
        roomFound = new Room();
        foreach (Room room in rooms)
        {
            if (room.GetPosition() == position)
            {
                roomFound = room;
                return true;
            }
        }
        return false;
    }

    public bool FindRoom(int xPos, int yPos)
    {
        Room room;
        return FindRoom(xPos, yPos, out room);
    }
    public bool FindRoom(int xPos, int yPos, out Room roomFound)
    {
        return FindRoom(new Vector2(xPos, yPos), out roomFound);
    }

    public bool FindRoom(string name)
    {
        Room room;
        return FindRoom(name, out room);
    }
    public bool FindRoom(string name, out Room roomFound)
    {
        roomFound = new Room();
        foreach (Room room in rooms)
        {
            if (room.name.ToLower().Contains(name.ToLower()))
            {
                roomFound = room;
                return true;
            }
        }
        return false;
    }

    public bool FindRoom(int id)
    {
        Room room;
        return FindRoom(id, out room);
    }
    public bool FindRoom(int id, out Room roomFound)
    {
        roomFound = new Room();
        foreach (Room room in rooms)
        {
            if (room.GetRoomID() == id)
            {
                roomFound = room;
                return true;
            }
        }
        return false;
    }

    

    public Room GetRoomByID(int id)
    {
        Room room;
        if (FindRoom(id, out room))
        {
            return room;
        }
        return null;
    }
}

[JsonObject("Room")]
public class Room
{
    [JsonProperty("name")]
    public string name { get; set; }
    
    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("description")]
    public string description { get; set; }

    [JsonProperty("connectedRooms")]
    public int[] connectedRooms { get; set; }

    [JsonProperty("xPos")]
    public int xPos { get; set; }

    [JsonProperty("yPos")]
    public int yPos { get; set; }

    [JsonProperty("width")]
    public int width { get; set; }
    
    [JsonProperty("height")]
    public int height { get; set; }

    [JsonProperty("isExplored")]
    public bool isExplored { get; set; }

    [JsonProperty("items")]
    List<Item> items { get; set; }
    
    [JsonProperty("interactables")]
    List<Interactable> interactables { get; set; }

    public Room()
    {
        name = "Default";
        id = -1;
        connectedRooms = new int[] { -1, -1, -1, -1 };
        xPos = 0;
        yPos = 0;
        width = 1;
        height = 1;
        items = new List<Item>();
        interactables = new List<Interactable>();
    }

    public Room(int _xPos, int _yPos)
    {
        name = "Default";
        id = -1;
        connectedRooms = new int[]{ -1, -1, -1, -1};
        xPos = _xPos;
        yPos = _yPos;
        width = 1;
        height = 1;
        items = new List<Item>();
        interactables = new List<Interactable>();
    }

    public Room(string _name, int _id, int[] _connectedRooms, int _xPos, int _yPos, int _width, int _height, List<Item> _items, List<Interactable> _interactables)
    {
        name = _name;
        id = _id;
        connectedRooms = _connectedRooms;
        xPos = _xPos;
        yPos = _yPos;
        width = _width;
        height = _height;
        items = _items;
        interactables = _interactables;
    }

    public int GetRoomID()
    {
        return id;
    }
    public bool GetIsExplored() 
    {  
        return isExplored; 
    }
    public Vector2 GetPosition()
    {
        return new Vector2(xPos, yPos);
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
        return connectedRooms;
    }

    public List<Item> GetItems()
    {
        return items;
    }

    public List<Interactable> GetInteractables()
    {
        return interactables;
    }

    //Finding things
    public bool FindItem(Vector2 position)
    {
        Item item;
        return FindItem(position, out item);
    }
    public bool FindItem(Vector2 position, out Item itemFound)
    {
        itemFound = new Item();
        foreach (Item item in items)
        {
            if (item.GetPosition() == position)
            {
                itemFound = item;
                return true;
            }
        }
        return false;
    }

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
            if (item.name.ToLower().Contains(name.ToLower()))
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
            if (interactable.name.ToLower().Contains(name.ToLower()))
            {
                interactableFound = interactable;
                return true;
            }
        }
        return false;
    }

    public bool FindInteractable(int id)
    {
        Interactable interactable;
        return FindInteractable(id, out interactable);
    }
    public bool FindInteractable(int id, out Interactable interactableFound)
    {
        interactableFound = new Interactable();
        foreach (Interactable interactable in interactables)
        {
            if (interactable.id == id)
            {
                interactableFound = interactable;
                return true;
            }
        }
        return false;
    }

    //Removing Items

    public void RemoveItem(int xPos, int yPos)
    {
        Item item;
        if (FindItem(xPos, yPos, out item))
        {
            items.Remove(item);
        }

    }
    public void RemoveItem(Vector2 position)
    {
        RemoveItem((int)position.X, (int)position.Y);
    }
    public void RemoveItem(string name)
    {
        Item item;
        if (FindItem(name, out item))
        {
            items.Remove(item);
        }
    }
    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    //Adding Items

    public void AddItem(Item item, int xPos, int yPos)
    {
        item.xPos = xPos;
        item.yPos = yPos;
        items.Add(item);
    }
    
    public void AddItem(Item item, Vector2 position)
    {
        AddItem(item, (int)position.X, (int)position.Y);
    }

    //Removing Interactables

    public void RemoveInteractable(int xPos, int yPos)
    {
        Interactable interactable;
        if (FindInteractable(xPos, yPos, out interactable))
        {
            interactables.Remove(interactable);
        }
    }
    public void RemoveInteractable(Vector2 position)
    {
        RemoveInteractable((int)position.X, (int)position.Y);
    }
    public void RemoveInteractable(string name)
    {
        Interactable interactable;
        if (FindInteractable(name, out interactable))
        {
            interactables.Remove(interactable);
        }
    }
    public void RemoveInteractable(Interactable interactable)
    {
        interactables.Remove(interactable);
    }

    //Adding Interactables

    public void AddInteractable(Interactable interactable, int xPos, int yPos)
    {
        interactable.xPos = xPos;
        interactable.yPos = yPos;
        interactables.Add(interactable);
    }

    public void AddInteractable(Interactable interactable, Vector2 position)
    {
        AddInteractable(interactable, (int)position.X, (int)position.Y);
    }
}

[JsonObject("Interactable")]
public class Interactable
{
    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("xPos")]
    public int xPos { get; set; }

    [JsonProperty("yPos")]
    public int yPos { get; set; }

    [JsonProperty("textColor")]
    public Color textColor { get; set; }

    [JsonProperty("textString")]
    public string textString { get; set; }

    public Interactable()
    {
        name = "Default";
        id = -1;
        xPos = 0;
        yPos = 0;
        textColor = Color.Yellow;
        textString = "D";
    }

    public Interactable(string _name, int _id, int _xPos, int _yPos, Color _textColor, string _textString)
    {
        name = _name;
        id = _id;
        xPos = _xPos;
        yPos = _yPos;
        textColor = _textColor;
        textString = _textString;
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

    public Color GetColor() 
    {
        return textColor; 
    }

    public string GetTextString()
    {
        return textString;
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

    [JsonProperty("textColor")]
    public Color textColor { get; set; }

    [JsonProperty("textString")]
    public string textString { get; set; }

    public Item()
    {
        name = "Default";
        description = "Default";
        id = -1;
        xPos = 0;
        yPos = 0;
        textColor = Color.White;
        textString = "d";
    }

    public Item(string _name, string _description, int _id, int _xPos, int _yPos, Color _textColor, string _textString)
    {
        name = _name;
        description = _description;
        id = _id;
        xPos = _xPos;
        yPos = _yPos;
        textColor = _textColor;
        textString = _textString;
    }

    public string GetName()
    {
        return name;
    }

    public int GetID()
    {
        return id;
    }

    public Color GetColor()
    {
        return textColor;
    }

    public string GetTextString()
    {
        return textString;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(xPos, yPos);
    }
}