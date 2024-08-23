using System;
using System.Collection.Generic

public class Room
{
    string name { get; };
    int width { get; };
    int height { get; };
    List<Item> items { get; }

    public Room(string _name, int _width, int _height, List<Item> _items)
	{
        name = _name;
        width = _width;
        height = _height;
        items = _items;
	}
}

public class Item
{
    string name { get; }
    

    public Item()
    {

    }
}