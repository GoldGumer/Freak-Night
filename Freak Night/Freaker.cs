using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Freak_Night
{
    public struct RoomLink
    {
        public RoomLink(int _roomID, int _prevID)
        {
            roomID = _roomID;
            prevID = _prevID;
        }

        public int roomID;
        public int prevID;
    }

    class Freaker
    {
        Building building;
        float chanceToMove;
        int roomID;
        int roomIDOfInterest;

        Queue<RoomLink> queue;
        List<int> visited;

        public Freaker(float _chanceToMove, int _roomIDOfInterest, Building _building) 
        {
            chanceToMove = _chanceToMove;
            roomIDOfInterest = _roomIDOfInterest;
            building = _building;
        }

        public void PathFind()
        {

        }

        private bool CheckRoom(int currentID)
        {
            if (visited.Contains(currentID))
            {
                return false;
            }


            visited.Add(currentID);

            foreach (int roomID in building.GetRoomByID(currentID).GetConnectedRooms())
            {
                if (roomID == roomIDOfInterest) return true;
                else if (roomID != -1)
                {
                    queue.Enqueue(new RoomLink(roomID, currentID));
                }
            }
	        return false;
        }

        public void Move()
        {

        }

        public void SetChanceToMove(float _chanceToMove)
        {
            chanceToMove = _chanceToMove;
        }

        public float GetChanceToMove() 
        {
            return chanceToMove;
        }
    }
}
