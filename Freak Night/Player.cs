using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Freak_Night
{
    public class Player
    {
        Vector2 roomPosition;
        Vector2 mapPosition;
        List<Item> items;
        bool isHiding;

        public Player(Vector2 _roomPosition, Vector2 _mapPosition, List<Item> _items)
        {
            roomPosition = _roomPosition;
            mapPosition = _mapPosition;
            items = _items;
            isHiding = false;
        }

        public Vector2 GetRoomPosition()
        {
            return roomPosition;
        }

        public void SetRoomPosition(Vector2 positon)
        {
            roomPosition = positon;
        }

        public Vector2 GetMapPosition()
        {
            return mapPosition;
        }

        public void SetMapPosition(Vector2 positon)
        {
            mapPosition = positon;
        }

        public List<Item> GetItems()
        {
            return items;
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

        public void AddItem(Item item) 
        {
            items.Add(item);
        }

        public void RemoveItem(Item item) 
        {
            items.Remove(item);
        }

        public bool GetHiding()
        {
            return isHiding;
        }

        public void SetHiding(bool hiding)
        {
            isHiding = hiding;
        }

        public void ChangeHiding()
        {
            isHiding = !isHiding;
        }
    }
}
