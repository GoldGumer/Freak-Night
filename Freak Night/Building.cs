using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

public class Building
{
    public List<Room> rooms { get; }

    public Building(string path)
    {
        StreamReader sr = new StreamReader(path);

        List<Room> _rooms = new List<Room>();

        while (!sr.EndOfStream) 
        {
            string roomCount = sr.ReadLine();
            for (int i = 0; i < int.Parse(roomCount); i++) 
            { 
                string roomName = sr.ReadLine();
                int roomID = int.Parse(sr.ReadLine());
                int width = int.Parse(sr.ReadLine());
                int height = int.Parse(sr.ReadLine());
                
                int itemCount;
                List<Item> items = new List<Item>();

                if (int.TryParse(sr.ReadLine(), out itemCount))
                {
                    for (int j = 0; j < itemCount; j++)
                    {
                        string itemName = sr.ReadLine();
                        string itemDescription = sr.ReadLine();
                        int id = int.Parse(sr.ReadLine());
                        int xPos = int.Parse(sr.ReadLine());
                        int yPos = int.Parse(sr.ReadLine());
                        items.Add(new Item(itemName, itemDescription, id, xPos, yPos));
                    }
                }

                int interactableCount;
                List<Interactable> interactables = new List<Interactable>();

                if (int.TryParse(sr.ReadLine(), out interactableCount))
                {
                    for (int j = 0; j < interactableCount; j++)
                    {
                        string interactableName = sr.ReadLine();
                        int id = int.Parse(sr.ReadLine());
                        int xPos = int.Parse(sr.ReadLine());
                        int yPos = int.Parse(sr.ReadLine());
                        interactables.Add(new Interactable(interactableName, id, xPos, yPos));
                    }
                }
                

                _rooms.Add(new Room(roomName, roomID, width, height, items, interactables));
            }
        }
        sr.Close();

        rooms = _rooms;
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

public class Room
{
    public string name { get; }
    public int id { get; }
    public int width { get; }
    public int height { get; }
    public List<Item> items { get; }
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

public class Interactable
{
    public string name { get; }
    public int id { get; }
    public int xPos { get; }
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

public class Item
{
    public string name { get; }
    public string description { get; }
    public int id { get; }
    public int xPos { get; }
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