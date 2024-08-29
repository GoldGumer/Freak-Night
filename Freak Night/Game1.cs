using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace Freak_Night
{
    enum GameScene
    {
        MainMenuScene,
        RoomScene,
        MapScene
    }

    struct DisplayText
    {
        public Color color;
        public string text;

        public DisplayText(Color _color, string _text)
        {
            color = _color;
            text = _text;
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Screen
        int screenHeight;
        int screenWidth;

        //Scene
        GameScene currentScene = GameScene.MainMenuScene;

        //Font
        SpriteFont font;
        const int fontHeight = 40;
        const int fontWidth = 32;

        //Keyboards
        KeyboardState previousKeyboard;
        KeyboardState currentKeyboard;

        //Comands
        static readonly string[] gameplayCommands = { "i", "inspect","x", "examine", "p", "pickup", "d", "drop", "h", "hide", "m", "map", "u", "use", "ping" };
        static readonly string[] editingCommands = { "a", "add", "r", "remove", "pu", "pickup", "pa", "place", "c", "change" };

        //Display
        Vector2 roomPosition;
        Vector2 centrePoint;
        int horizontalDivider;
        int verticalDivider;
        string defaultBackground;
        bool isEditing;
        bool isMapDisplaying;
        Queue<string> sideTextArea;
        string[] inputTextArea;

        //Main Menu stuff
        int buttonSelected;
        string[] buttonsText;
        string[] displayButtonsText;
        float audioVolume;

        //Edit Mode Stuff
        Item pickedUpItem;
        Interactable pickedUpInteractable;
        Room pickedUpRoom;

        //Player
        Player player;

        //Building
        Building building;
        int roomID;

        //Colors
        Color roomColor = Color.Gray;
        Color itemColor = Color.White;
        Color interactableColor = Color.Yellow;
        Color playerColor = Color.Red;
        Color creeperColor = Color.Green;
        Color UIColor = Color.White;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            screenHeight = 1080;
            screenWidth = 1920;

            _graphics.PreferredBackBufferHeight = screenHeight;
            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.ApplyChanges();

            Window.TextInput += TextInputHandler;


            horizontalDivider = 46;
            verticalDivider = 22;
            centrePoint = new Vector2(horizontalDivider / 2, verticalDivider / 2);
            defaultBackground = " ";

            isEditing = false;
            isMapDisplaying = true;

            pickedUpItem = null;
            pickedUpInteractable = null;
            pickedUpRoom = null;

            roomID = 0;

            buttonSelected = 0;
            buttonsText = new string[3] { "Start", "Volume", "Quit" };
            displayButtonsText = new string[3] { "Start", "Volume", "Quit" };
            audioVolume = 0.5f;

            player = new Player(Vector2.One, centrePoint, new List<Item>() { });

            sideTextArea = new Queue<string>();
            inputTextArea = new string[3] { "", "", ""};

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("IBM Plex Mono Light");

            //Building Creation

            string path = Path.Combine(Content.RootDirectory, "ExampleBuilding.json");

            building = new Building(path);

            foreach (var room in building.GetRooms())
            {
                room.isExplored = false;
            }

            building.GetRoomByID(roomID).isExplored = true;

            player.SetRoomPosition(building.GetRoomByID(roomID).GetRoomSize() / 2);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();

            if (currentScene == GameScene.MainMenuScene)
            {
                MainMenuUpdate();
            }
            else if (!isEditing)
            {
                GameplayUpdate();
            }
            else if (isEditing)
            {
                EditModeUpdate();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (currentScene == GameScene.MainMenuScene)
            {
                MainMenuDraw();
            }
            else if (currentScene == GameScene.RoomScene)
            {
                RoomDraw();
            }
            else if (currentScene == GameScene.MapScene)
            { 
                MapDraw();
            }

            base.Draw(gameTime);
        }


        //"x", "eximine", "p", "pickup", "h", "hide", "m", "map", "u", "use", "ping"
        //"a", "add", "r", "remove", "pu", "pickup", "pa", "place", "c", "change"
        private void TextInputHandler(object sender, TextInputEventArgs args)
        {
            var pressedKey = args.Key;
            var character = args.Character;
            if (currentScene != GameScene.MainMenuScene)
            {
                if (!isEditing)
                {
                    if (pressedKey == Keys.Enter)
                    {
                        Room currentRoom = building.GetRoomByID(roomID);
                        string command = Array.Find<string>(gameplayCommands, command => command == inputTextArea[0].Split(' ')[0]);
                        if (command == "i" || command == "inspect")
                        {
                            Item item;
                            if (inputTextArea[0].Split(' ').Length > 1 && currentRoom.FindItem(inputTextArea[0].Split(' ')[1], out item))
                            {
                                AddStringToBottom(item.description);
                            }
                            else
                            {
                                AddStringToBottom("Couldn't find an item by that name.");
                            }
                        }
                        else if (command == "x" || command == "examine")
                        {
                            string stringToAdd = "There is a ";

                            for (int i = 0; i < currentRoom.GetItemCount(); i++)
                            {
                                if (i != 0 && currentRoom.GetItemCount() - 1 == i)
                                {
                                    stringToAdd += " and ";
                                }
                                else if (i != 0)
                                {
                                    stringToAdd += ", ";
                                }

                                stringToAdd += currentRoom.GetItems().ToArray()[i].name;
                            }

                            stringToAdd += " on the floor. There is also ";

                            for (int i = 0; i < currentRoom.GetInteractableCount(); i++)
                            {
                                if (i != 0 && currentRoom.GetInteractableCount() - 1 == i)
                                {
                                    stringToAdd += " and ";
                                }
                                else if (i != 0)
                                {
                                    stringToAdd += ", ";
                                }

                                stringToAdd += currentRoom.GetInteractables().ToArray()[i].name;
                            }

                            stringToAdd += " object ";
                            if (currentRoom.GetInteractableCount() > 1)
                            {
                                stringToAdd += "s ";
                            }

                            stringToAdd += "that you can interact with around you.";

                            AddStringToBottom(stringToAdd);
                        }
                        else if (command == "p" || command == "pickup")
                        {
                            Item item;
                            if (inputTextArea[0].Split(' ').Length > 1 && currentRoom.FindItem(inputTextArea[0].Split(' ')[1], out item))
                            {
                                AddStringToBottom("Picked up " + item.name.ToLower() + ".");

                                player.AddItem(item);
                                currentRoom.RemoveItem(item);
                            }
                            else
                            {
                                AddStringToBottom("Couldn't find an item by that name.");
                            }
                        }
                        else if (command == "d" || command == "drop")
                        {
                            Item item;
                            if (inputTextArea[0].Split(' ').Length > 1 && player.FindItem(inputTextArea[0].Split(' ')[1], out item))
                            {
                                AddStringToBottom("Dropped " + item.name + ".");

                                currentRoom.AddItem(item, player.GetRoomPosition());
                                player.RemoveItem(item);
                            }
                        }
                        else if (command == "h" || command == "hide")
                        {
                            Interactable interactable;
                            if (currentRoom.FindInteractable(4, out interactable))
                            {
                                player.ChangeHiding();
                                AddStringToBottom("You are hiding in a " + interactable.name + ".");
                            }
                            else
                            {
                                AddStringToBottom("Couldn't find a place to hide.");
                            }
                        }
                        else if (command == "m" || command == "map")
                        {
                            isMapDisplaying = !isMapDisplaying;
                            AddStringToBottom("Mapp off!");
                        }
                        else if (
                            (inputTextArea[0].Split(' ').Length > 2 &&
                            inputTextArea[0].Split(' ')[2].ToLower() != "on") ||
                            (inputTextArea[0].Split(' ').Length > 3) &&
                            command == "u" || command == "use")
                        {

                        }
                        else if (command == "ping")
                        {

                            AddStringToBottom("");
                        }
                        else
                        {
                            AddStringToBottom("Couldn't find a command to match what you are trying to do.");
                        }
                    }
                    else if (pressedKey == Keys.Back || pressedKey == Keys.Delete)
                    {
                        if (inputTextArea[0].Length > 0)
                        {
                            inputTextArea[0] = inputTextArea[0].Remove(inputTextArea[0].Length - 1);
                        }
                    }
                    else if (pressedKey == Keys.Tab || pressedKey == Keys.Insert)
                    {

                    }
                    else if (!currentKeyboard.IsKeyDown(Keys.LeftAlt) && !currentKeyboard.IsKeyDown(Keys.RightAlt))
                    {
                        if (currentKeyboard.IsKeyDown(Keys.LeftShift) || currentKeyboard.IsKeyDown(Keys.RightShift))
                        {
                            inputTextArea[0] += character.ToString().ToUpper();
                        }
                        else
                        {
                            inputTextArea[0] += character.ToString().ToLower();
                        }

                    }
                }
                else
                {
                    if (pressedKey == Keys.Enter)
                    {
                        Room currentRoom = building.GetRoomByID(roomID);
                        string command = Array.Find<string>(editingCommands, command => command == inputTextArea[0].Split(' ')[0]);

                        if (command == "a" || command == "add" && inputTextArea[0].Split(' ').Length > 1)
                        {
                            if (
                                inputTextArea[0].Split(' ')[1] == "item" &&
                                currentScene == GameScene.RoomScene &&
                                !currentRoom.FindItem(player.GetRoomPosition()))
                            {
                                currentRoom.AddItem(new Item(), player.GetRoomPosition());
                                AddStringToBottom("Item was added at " + player.GetRoomPosition().X + ", " + player.GetRoomPosition().Y);
                            }
                            else if (
                                (inputTextArea[0].Split(' ')[1] == "interactable" ||
                                inputTextArea[0].Split(' ')[1] == "intr") &&
                                currentScene == GameScene.RoomScene &&
                                !currentRoom.FindInteractable(player.GetRoomPosition()))
                            {
                                currentRoom.AddInteractable(new Interactable(), player.GetRoomPosition());
                                AddStringToBottom("Interactable was added at " + player.GetRoomPosition().X + ", " + player.GetRoomPosition().Y);
                            }
                            else if (
                                inputTextArea[0].Split(' ')[1] == "room" &&
                                currentScene == GameScene.MapScene &&
                                !building.FindRoom(PositionToMap(player.GetMapPosition())))
                            {
                                building.AddRoom(PositionToMap(player.GetMapPosition()));
                                AddStringToBottom("Room was added at " + PositionToMap(player.GetMapPosition()).X + ", " + PositionToMap(player.GetMapPosition()).Y);
                            }
                            else
                            {
                                AddStringToBottom("Couldn't find an object of that type to create.");
                            }
                        }
                        else if (inputTextArea[0].Split(' ').Length > 1 && (command == "r" || command == "remove"))
                        {
                            if (inputTextArea[0].Split(' ')[1] == "item")
                            {
                                currentRoom.RemoveItem(player.GetRoomPosition());
                                AddStringToBottom("Item was removed at " + player.GetRoomPosition().X + ", " + player.GetRoomPosition().Y);
                            }
                            else if (inputTextArea[0].Split(' ')[1] == "interactable" || inputTextArea[0].Split(' ')[1] == "intr")
                            {
                                currentRoom.RemoveInteractable(player.GetRoomPosition());
                                AddStringToBottom("Interactable was removed at " + player.GetRoomPosition().X + ", " + player.GetRoomPosition().Y);

                            }
                            else if (inputTextArea[0].Split(' ')[1] == "room")
                            {
                                Room room;

                                if (building.FindRoom(PositionToMap(player.GetMapPosition()), out room) && room.GetRoomID() != roomID)
                                {
                                    building.RemoveRoom(PositionToMap(player.GetMapPosition()));
                                    AddStringToBottom("Room was removed at " + PositionToMap(player.GetMapPosition()).X + ", " + PositionToMap(player.GetMapPosition()).Y);
                                }
                                else
                                {
                                    AddStringToBottom("Cannot remove the room you are in.");
                                }
                            }
                            else
                            {
                                Item item;
                                Interactable interactable;
                                Room room;
                                if (
                                    currentRoom.FindItem(inputTextArea[0].Split(' ')[1], out item) &&
                                    currentScene == GameScene.RoomScene)
                                {
                                    currentRoom.RemoveItem(item);
                                    AddStringToBottom(inputTextArea[0].Split(' ')[1] + " item removed.");

                                }
                                else if (
                                    currentRoom.FindInteractable(inputTextArea[0].Split(' ')[1], out interactable) &&
                                    currentScene == GameScene.RoomScene)
                                {
                                    currentRoom.RemoveInteractable(interactable);
                                    AddStringToBottom(inputTextArea[0].Split(' ')[1] + " interactable removed.");

                                }
                                else if (
                                    building.FindRoom(inputTextArea[0].Split(' ')[1], out room) &&
                                    currentScene == GameScene.MapScene)
                                {
                                    building.RemoveRoom(room);
                                    AddStringToBottom(inputTextArea[0].Split(' ')[1] + " room removed.");
                                }
                                else
                                {
                                    AddStringToBottom("Couldn't find an object of that name/type.");
                                }
                            }
                        }
                        else if (command == "pu" || command == "pickup")
                        {
                            Item item;
                            Interactable interactable;
                            Room room;
                            if (
                                currentRoom.FindItem(player.GetRoomPosition(), out item) &&
                                currentScene == GameScene.RoomScene &&
                                pickedUpItem == null &&
                                pickedUpInteractable == null &&
                                pickedUpRoom == null)
                            {
                                pickedUpItem = item;
                                AddStringToBottom("Picked up " + item.name);
                                currentRoom.RemoveItem(item);
                            }
                            else if (
                                currentRoom.FindInteractable(player.GetRoomPosition(), out interactable) &&
                                currentScene == GameScene.RoomScene &&
                                pickedUpItem == null &&
                                pickedUpInteractable == null &&
                                pickedUpRoom == null)
                            {
                                pickedUpInteractable = interactable;
                                AddStringToBottom("Picked up " + interactable.name);
                                currentRoom.RemoveInteractable(interactable);
                            }
                            else if (
                                building.FindRoom(PositionToMap(player.GetMapPosition()), out room) &&
                                currentScene == GameScene.MapScene &&
                                pickedUpItem == null &&
                                pickedUpInteractable == null &&
                                pickedUpRoom == null)
                            {
                                if (roomID != room.GetRoomID())
                                {
                                    pickedUpRoom = room;
                                    AddStringToBottom("Picked up " + room.name);
                                    building.RemoveRoom(room);
                                }
                                else
                                {
                                    AddStringToBottom("Cannot remove the room you are in.");
                                }
                            }
                        }
                        else if (command == "pa" || command == "place")
                        {
                            if (GameScene.RoomScene == currentScene)
                            {
                                if (pickedUpItem != null)
                                {
                                    currentRoom.AddItem(pickedUpItem, player.GetRoomPosition());
                                    AddStringToBottom("Picked up " + pickedUpItem.name);
                                    pickedUpItem = null;
                                }
                                else if (pickedUpInteractable != null)
                                {
                                    currentRoom.AddInteractable(pickedUpInteractable, player.GetRoomPosition());
                                    AddStringToBottom("Picked up " + pickedUpInteractable.name);
                                    pickedUpInteractable = null;
                                }
                            }
                            else if (pickedUpRoom != null)
                            {
                                building.AddRoom(pickedUpRoom, PositionToMap(player.GetMapPosition()));
                                AddStringToBottom("Picked up " + pickedUpRoom.name);
                                pickedUpRoom = null;
                            }
                        }
                        else if (inputTextArea[0].Split(' ').Length > 2 && (command == "c" || command == "change"))
                        {
                            Item item;
                            Interactable interactable;
                            Room room;
                            int result;
                            int[] array = new int[4] { 0, 0, 0, 0 };
                            if (
                                currentRoom.FindItem(player.GetRoomPosition(), out item) &&
                                currentScene == GameScene.RoomScene)
                            {

                                switch (inputTextArea[0].Split(' ')[1])
                                {
                                    case "name":
                                        item.name = "";
                                        for (int i = 2; i < inputTextArea[0].Split(' ').Length; i++)
                                        {
                                            item.name += inputTextArea[0].Split(' ')[i] + " ";
                                        }
                                        break;
                                    case "description":
                                        item.description = "";
                                        for (int i = 2; i < inputTextArea[0].Split(' ').Length; i++)
                                        {
                                            item.description += inputTextArea[0].Split(' ')[i] + " ";
                                        }
                                        break;
                                    case "desc":
                                        item.description = " ";
                                        for (int i = 2; i < inputTextArea[0].Split(' ').Length; i++)
                                        {
                                            item.description += inputTextArea[0].Split(' ')[i] + " ";
                                        }
                                        break;
                                    case "id":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            item.id = result;
                                        }
                                        break;
                                    case "xpos":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            item.xPos = result;
                                        }
                                        break;
                                    case "ypos":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            item.yPos = result;
                                        }
                                        break;
                                    case "textcolor":
                                        for (int i = 2; i < inputTextArea[0].Split(' ').Length; i++)
                                        {
                                            if (int.TryParse(inputTextArea[0].Split(" ")[i], out result))
                                            {
                                                array[i - 2] = result;
                                            }
                                        }
                                        item.textColor = new Color(array[0], array[1], array[2]);
                                        break;
                                    case "tc":
                                        for (int i = 2; i < inputTextArea[0].Split(' ').Length; i++)
                                        {
                                            if (int.TryParse(inputTextArea[0].Split(" ")[i], out result))
                                            {
                                                array[i - 2] = result;
                                            }
                                        }
                                        item.textColor = new Color(array[0], array[1], array[2]);
                                        break;
                                    case "textstring":
                                        item.textString = inputTextArea[0].Split(' ')[2].ToCharArray()[0].ToString().ToLower();
                                        break;
                                    case "ts":
                                        item.textString = inputTextArea[0].Split(' ')[2].ToCharArray()[0].ToString().ToLower();
                                        break;
                                    default:
                                        break;
                                }
                                AddStringToBottom("");
                            }
                            else if (
                                currentRoom.FindInteractable(player.GetRoomPosition(), out interactable) &&
                                currentScene == GameScene.RoomScene)
                            {
                                switch (inputTextArea[0].Split(' ')[1])
                                {
                                    case "name":
                                        interactable.name = inputTextArea[0].Split(' ')[2];
                                        break;
                                    case "id":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            interactable.id = result;
                                        }
                                        break;
                                    case "xpos":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            interactable.xPos = result;
                                        }
                                        break;
                                    case "ypos":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            interactable.yPos = result;
                                        }
                                        break;
                                    case "textcolor":
                                        for (int i = 2; i < inputTextArea[0].Split(' ').Length; i++)
                                        {
                                            if (int.TryParse(inputTextArea[0].Split(" ")[i], out result))
                                            {
                                                array[i - 2] = result;
                                            }
                                        }
                                        interactable.textColor = new Color(array[0], array[1], array[2]);
                                        break;
                                    case "tc":
                                        for (int i = 2; i < inputTextArea[0].Split(' ').Length; i++)
                                        {
                                            if (int.TryParse(inputTextArea[0].Split(" ")[i], out result))
                                            {
                                                array[i - 2] = result;
                                            }
                                        }
                                        interactable.textColor = new Color(array[0], array[1], array[2]);
                                        break;
                                    case "textstring":
                                        interactable.textString = inputTextArea[0].Split(' ')[2].ToCharArray()[0].ToString().ToUpper();
                                        break;
                                    case "ts":
                                        interactable.textString = inputTextArea[0].Split(' ')[2].ToCharArray()[0].ToString().ToUpper();
                                        break;
                                    default:
                                        break;
                                }
                                AddStringToBottom("");
                            }
                            else if (
                                building.FindRoom(PositionToMap(player.GetMapPosition()), out room) &&
                                currentScene == GameScene.MapScene)
                            {
                                switch (inputTextArea[0].Split(' ')[1])
                                {
                                    case "name":
                                        room.name = inputTextArea[0].Split(' ')[2];
                                        break;
                                    case "id":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            room.id = result;
                                        }
                                        break;
                                    case "xpos":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            room.xPos = result;
                                        }
                                        break;
                                    case "ypos":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            room.yPos = result;
                                        }
                                        break;
                                    case "width":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            room.width = result;
                                        }
                                        break;
                                    case "w":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            room.width = result;
                                        }
                                        break;
                                    case "height":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            room.height = result;
                                        }
                                        break;
                                    case "h":
                                        if (int.TryParse(inputTextArea[0].Split(' ')[2], out result))
                                        {
                                            room.height = result;
                                        }
                                        break;
                                    case "connectedrooms":
                                        for (int i = 2; i < inputTextArea[0].Split(' ').Length; i++)
                                        {
                                            if (int.TryParse(inputTextArea[0].Split(" ")[i], out result))
                                            {
                                                array[i - 2] = result;
                                            }
                                        }
                                        room.connectedRooms = new int[4] { (int)array[0], (int)array[1], (int)array[2], (int)array[3] };
                                        break;
                                    case "connections":
                                        for (int i = 2; i < inputTextArea[0].Split(' ').Length; i++)
                                        {
                                            if (int.TryParse(inputTextArea[0].Split(" ")[i], out result))
                                            {
                                                array[i - 2] = result;
                                            }
                                        }
                                        room.connectedRooms = new int[4] { (int)array[0], (int)array[1], (int)array[2], (int)array[3] };
                                        break;
                                    case "con":
                                        for (int i = 2; i < inputTextArea[0].Split(' ').Length; i++)
                                        {
                                            if (int.TryParse(inputTextArea[0].Split(" ")[i], out result))
                                            {
                                                array[i - 2] = result;
                                            }
                                        }
                                        room.connectedRooms = new int[4] { (int)array[0], (int)array[1], (int)array[2], (int)array[3] };
                                        break;
                                    case "isexplored":
                                        if (inputTextArea[0].Split(' ')[2] == "false")
                                        {
                                            room.isExplored = false;
                                        }
                                        else
                                        {
                                            room.isExplored = true;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                AddStringToBottom("");
                            }
                            else
                            {
                                AddStringToBottom("Couldn't find an attribute by that name.");
                            }
                        }
                        else
                        {
                            AddStringToBottom("Couldn't find a command to match what you are trying to do.");
                        }
                    }
                    else if (pressedKey == Keys.Back || pressedKey == Keys.Delete)
                    {
                        if (inputTextArea[0].Length > 0)
                        {
                            inputTextArea[0] = inputTextArea[0].Remove(inputTextArea[0].Length - 1);
                        }
                    }
                    else if (pressedKey == Keys.Tab || pressedKey == Keys.Insert)
                    {

                    }
                    else if (!currentKeyboard.IsKeyDown(Keys.LeftAlt) && !currentKeyboard.IsKeyDown(Keys.RightAlt))
                    {
                        if (currentKeyboard.IsKeyDown(Keys.LeftShift) || currentKeyboard.IsKeyDown(Keys.RightShift))
                        {
                            inputTextArea[0] += character.ToString().ToUpper();
                        }
                        else
                        {
                            inputTextArea[0] += character.ToString().ToLower();
                        }

                    }
                }
            }
            else 
            {
                if (pressedKey == Keys.Enter)
                {
                    switch (buttonsText[buttonSelected])
                    {
                        case "Start":
                            currentScene = GameScene.RoomScene;
                            break;
                        case "Options":
                            break;
                        case "Quit":
                            Exit();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private bool IsKeyPressed(Keys key)
        {
            if (currentKeyboard.IsKeyDown(key) && (currentKeyboard.IsKeyDown(key) ^ previousKeyboard.IsKeyDown(key)))
            {
                return true;
            }
            else 
            {
                return false; 
            }
        }

        private Vector2 PositionToMap(Vector2 position)
        {
            return (position - centrePoint) / 2;
        }

        private Vector2 MapToPosition(Vector2 position)
        {
            return (position + centrePoint) * 2;
        }

        //UI functions

        private void QueueStringToSide(string textToQueue)
        {
            for (int i = 0; i < textToQueue.Length; i += (screenWidth / fontWidth) - horizontalDivider - 1)
            {
                sideTextArea.Enqueue(textToQueue.Substring(i, (int)MathF.Min((screenWidth / fontWidth) - horizontalDivider - 1, textToQueue.Length - i)));
            }
        }

        private void AddStringToBottom(string textToAdd)
        {
            for (int i = 0; i < inputTextArea.Length; i++)
            {
                inputTextArea[i] = "";
            }

            for (int i = 0; i < textToAdd.Length; i += (screenWidth / fontWidth))
            {
                if (i == 0)
                {
                    inputTextArea[1] = (textToAdd.Substring(i, (int)MathF.Min((screenWidth / fontWidth), textToAdd.Length - i)));
                }
                else if (i == (screenWidth / fontWidth)) 
                {
                    inputTextArea[2] = (textToAdd.Substring(i, (int)MathF.Min((screenWidth / fontWidth), textToAdd.Length - i)));
                }
                else
                {

                }
            }

            inputTextArea[0] = "";
        }

        //Main Menu Functions

        private void MainMenuUpdate()
        {
            if (IsKeyPressed(Keys.Up))
            {
                buttonSelected--;
            }
            else if (IsKeyPressed(Keys.Down))
            {
                buttonSelected++;
            }

            buttonSelected = Math.Clamp(buttonSelected, 0, 2);

            displayButtonsText[0] = buttonsText[0];
            displayButtonsText[1] = buttonsText[1];
            displayButtonsText[2] = buttonsText[2];

            displayButtonsText[buttonSelected] = ("-< " + buttonsText[buttonSelected] + " >-");

        }

        //Edit Mode Functions

        private void EditModeUpdate()
        {
            Vector2 direction = new Vector2(
                (IsKeyPressed(Keys.Right) ? 1 : 0) - 
                (IsKeyPressed(Keys.Left) ? 1 : 0),
                (IsKeyPressed(Keys.Down) ? 1 : 0) - 
                (IsKeyPressed(Keys.Up) ? 1 : 0));

            if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.RightAlt) && currentKeyboard.IsKeyDown(Keys.G))
            {
                string path = Path.Combine(Content.RootDirectory, "ExampleBuilding.json");
                building.BuildingToJSON(path);


                defaultBackground = " ";
                isEditing = false;
            }

            if (IsKeyPressed(Keys.M) && (currentKeyboard.IsKeyDown(Keys.LeftShift) || currentKeyboard.IsKeyDown(Keys.RightShift)))
            {
                if (currentScene == GameScene.RoomScene)
                {
                    currentScene = GameScene.MapScene;
                }
                else if (currentScene == GameScene.MapScene)
                {
                    currentScene = GameScene.RoomScene;
                }
            }

            if (currentScene == GameScene.RoomScene)
            {
                player.SetRoomPosition(player.GetRoomPosition() + direction);

                Item item;
                Interactable interactable;
                if (building.GetRoomByID(roomID).FindItem(player.GetRoomPosition(), out item))
                {
                    sideTextArea.Clear();

                    QueueStringToSide("Name: " + item.name);
                    QueueStringToSide("Description: " + item.description);
                    QueueStringToSide("ID: " + item.id);
                    QueueStringToSide("xPos: " + item.xPos);
                    QueueStringToSide("yPos: " + item.yPos);
                    QueueStringToSide("textColor: " + item.GetColor());
                    QueueStringToSide("textString: " + item.GetTextString());

                }
                else if (building.GetRoomByID(roomID).FindInteractable(player.GetRoomPosition(), out interactable))
                {
                    sideTextArea.Clear();

                    QueueStringToSide("Name: " + interactable.name);
                    QueueStringToSide("ID: " + interactable.GetID());
                    QueueStringToSide("xPos: " + interactable.GetPosition().X);
                    QueueStringToSide("yPos: " + interactable.GetPosition().Y);
                    QueueStringToSide("textColor: " + interactable.GetColor());
                    QueueStringToSide("textString: " + interactable.GetTextString());
                }
                else if (!building.GetRoomByID(roomID).FindItem(player.GetRoomPosition()) &&
                    !building.GetRoomByID(roomID).FindInteractable(player.GetRoomPosition()))
                {
                    sideTextArea.Clear();
                }
            }
            else if (currentScene == GameScene.MapScene)
            {
                player.SetMapPosition(player.GetMapPosition() + direction);

                Room room;
                if (building.FindRoom(PositionToMap(player.GetMapPosition()), out room))
                {
                    sideTextArea.Clear();

                    QueueStringToSide("Name: " + room.name);
                    QueueStringToSide("ID: " + room.GetRoomID());
                    string textToDisplay = "connectedRooms: ";
                    foreach (int roomID in room.GetConnectedRooms())
                    {
                        if (roomID != -1)
                        {
                            if (textToDisplay != "connectedRooms: ")
                            {
                                textToDisplay += ", ";
                            }
                            textToDisplay += roomID;
                        }
                    }
                    QueueStringToSide(textToDisplay);
                    QueueStringToSide("xPos: " + room.GetPosition().X);
                    QueueStringToSide("yPos: " + room.GetPosition().Y);
                    QueueStringToSide("width: " + room.GetRoomSize().X);
                    QueueStringToSide("height: " + room.GetRoomSize().Y);

                }
                else if (!building.FindRoom(PositionToMap(player.GetMapPosition())))
                {
                    sideTextArea.Clear();
                }
            }
        }

        //Gameplay Functions

        private void GameplayUpdate()
        {
            if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.RightAlt) && currentKeyboard.IsKeyDown(Keys.E))
            {
                foreach (var room in building.GetRooms())
                {
                    room.isExplored = true;
                }

                defaultBackground = ".";
                isEditing = true;
            }

            Vector2 direction = new Vector2(
                (IsKeyPressed(Keys.Right) ? 1 : 0) -
                (IsKeyPressed(Keys.Left) ? 1 : 0),
                (IsKeyPressed(Keys.Down) ? 1 : 0) -
                (IsKeyPressed(Keys.Up) ? 1 : 0));

            if ((currentKeyboard.IsKeyDown(Keys.LeftShift) || currentKeyboard.IsKeyDown(Keys.RightShift)) && IsKeyPressed(Keys.M))
            {
                sideTextArea.Clear();
                if (currentScene == GameScene.RoomScene)
                {
                    player.SetMapPosition(centrePoint + building.GetRoomByID(roomID).GetPosition() * 2);
                    currentScene = GameScene.MapScene;
                }
                else if (currentScene == GameScene.MapScene)
                {
                    currentScene = GameScene.RoomScene;
                }
            }

            Room currentRoom = building.GetRoomByID(roomID);

            Interactable interactable;
            if (currentRoom.FindInteractable(player.GetRoomPosition() + direction, out interactable))
            {
                switch (interactable.GetID())
                {
                    case 0:
                        if (currentRoom.GetConnectedRooms()[0] != -1)
                        {
                            roomID = currentRoom.GetConnectedRooms()[0];
                            building.GetRoomByID(roomID).isExplored = true;
                            if (building.GetRoomByID(roomID).FindInteractable(2, out interactable))
                            {
                                player.SetRoomPosition(interactable.GetPosition() + new Vector2(0, -1));
                            }
                        }
                        break;
                    case 1:
                        if (currentRoom.GetConnectedRooms()[1] != -1)
                        {
                            roomID = currentRoom.GetConnectedRooms()[1];
                            building.GetRoomByID(roomID).isExplored = true;
                            if (building.GetRoomByID(roomID).FindInteractable(3, out interactable))
                            {
                                player.SetRoomPosition(interactable.GetPosition() + new Vector2(1, 0));
                            }
                        }
                        break;
                    case 2:
                        if (currentRoom.GetConnectedRooms()[2] != -1)
                        {
                            roomID = currentRoom.GetConnectedRooms()[2];
                            building.GetRoomByID(roomID).isExplored = true;
                            if (building.GetRoomByID(roomID).FindInteractable(0, out interactable))
                            {
                                player.SetRoomPosition(interactable.GetPosition() + new Vector2(0, 1));
                            }
                        }
                        break;
                    case 3:
                        if (currentRoom.GetConnectedRooms()[3] != -1)
                        {
                            roomID = currentRoom.GetConnectedRooms()[3];
                            building.GetRoomByID(roomID).isExplored = true;
                            if (building.GetRoomByID(roomID).FindInteractable(1, out interactable))
                            {
                                player.SetRoomPosition(interactable.GetPosition() + new Vector2(-1, 0));
                            }
                        }
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
            }
            else if (currentScene == GameScene.RoomScene)
            {
                if (!player.GetHiding() &&
                    (player.GetRoomPosition() + direction).X < currentRoom.GetRoomSize().X &&
                    (player.GetRoomPosition() + direction).X > 0 &&
                    (player.GetRoomPosition() + direction).Y < currentRoom.GetRoomSize().Y &&
                    (player.GetRoomPosition() + direction).Y > 0
                    )
                {
                    player.SetRoomPosition(player.GetRoomPosition() + direction);
                }

                if (sideTextArea.Count != player.GetItems().Count)
                {
                    sideTextArea.Clear();
                    foreach (Item item in player.GetItems())
                    {
                        if (!sideTextArea.Contains(item.name))
                        {
                            QueueStringToSide(item.name);
                        }
                    }
                }
            }
            else if (currentScene == GameScene.MapScene)
            {
                player.SetMapPosition(player.GetMapPosition() + direction);
                Room room;
                if (building.FindRoom(PositionToMap(player.GetMapPosition()), out room) &&
                    room.isExplored &&
                    sideTextArea.Count != 1 + room.GetItemCount())
                {
                    sideTextArea.Clear();
                    QueueStringToSide(room.name);
                    QueueStringToSide("");
                    foreach (Item item in room.GetItems())
                    {
                        if (!sideTextArea.Contains(item.name))
                        {
                            QueueStringToSide(item.name);
                        }
                    }
                }
                else if (!building.FindRoom(PositionToMap(player.GetMapPosition())))
                {
                    sideTextArea.Clear();
                }
            }
        }

        //Display Options

        private void MainMenuDraw()
        {
            GraphicsDevice.Clear(Color.Black);

            centrePoint = new Vector2(((screenWidth / fontWidth) / 2), ((screenHeight / fontHeight) / 2));

            _spriteBatch.Begin();
            Display(new Vector2(0.0f, -15.0f));
            _spriteBatch.End();
        }

        private void MapDraw()
        {
            GraphicsDevice.Clear(Color.Black);


            _spriteBatch.Begin();
            Display(new Vector2(0.0f, -15.0f));
            _spriteBatch.End();
        }

        private void RoomDraw()
        {
            GraphicsDevice.Clear(Color.Black);
            centrePoint = new Vector2(horizontalDivider / 2, verticalDivider / 2);

            _spriteBatch.Begin();
            roomPosition = centrePoint - new Vector2(MathF.Floor(building.GetRoomByID(roomID).GetRoomSize().X / 2), MathF.Floor(building.GetRoomByID(roomID).GetRoomSize().Y / 2));
            Display(new Vector2(0.0f, -15.0f));
            _spriteBatch.End();
        }

        private void Display(Vector2 offset)
        {
            int upperVertical = (screenHeight / fontHeight) - 1;
            int upperHorizontal = (screenWidth / fontWidth);

            for (int i = 0; i < upperVertical; i++)
            {
                for (int j = 0; j < upperHorizontal; j++)
                {
                    Vector2 textPosition = new Vector2(j * fontWidth, i * fontHeight) + offset;
                    DisplayText displayText = new DisplayText(Color.White, defaultBackground);

                    if (currentScene != GameScene.MainMenuScene)
                    {
                        if (currentScene == GameScene.MapScene)
                        {
                            displayText = DisplayMapText(i, j, displayText);
                        }
                        else if (currentScene == GameScene.RoomScene && isMapDisplaying)
                        {
                            displayText = DisplayRoomText(i, j, displayText);
                        }


                        displayText = DisplayUIText(i, j, displayText);

                        if (!player.GetHiding())
                        {
                            displayText = DisplayPlayerText(i, j, displayText);
                        }
                    }
                    else
                    {
                        displayText = DisplayMenuText(i, j, displayText);
                    }

                    _spriteBatch.DrawString(font, displayText.text, textPosition, displayText.color);
                }
            }
        }

        private DisplayText DisplayMenuText(int i, int j, DisplayText displayText)
        {
            string title = "Freak Night";
            if (j <= (centrePoint.X + (title.Length / 2)) && j >= (centrePoint.X - (title.Length / 2)) && i == 2)
            {
                displayText = new DisplayText(Color.White, title.ToCharArray()[j - (int)(((screenWidth / fontWidth) / 2) - (title.Length / 2))].ToString());
            }
            else if (j <= (centrePoint.X + (displayButtonsText[0].Length / 2)) && j >= (centrePoint.X - (displayButtonsText[0].Length / 2)) && i == 8)
            {
                displayText = new DisplayText(Color.White, displayButtonsText[0].ToCharArray()[j - (int)(centrePoint.X - (displayButtonsText[0].Length / 2))].ToString());
            }
            else if (j < (centrePoint.X + (displayButtonsText[1].Length / 2)) && j >= (centrePoint.X - (displayButtonsText[1].Length / 2)) && i == 10)
            {
                displayText = new DisplayText(Color.White, displayButtonsText[1].ToCharArray()[j - (int)(centrePoint.X - (displayButtonsText[1].Length / 2))].ToString());
            }
            else if (j < (centrePoint.X + (displayButtonsText[2].Length / 2)) && j >= (centrePoint.X - (displayButtonsText[2].Length / 2)) && i == 20)
            {
                displayText = new DisplayText(Color.White, displayButtonsText[2].ToCharArray()[j - (int)(centrePoint.X - (displayButtonsText[2].Length / 2))].ToString());
            }

            return displayText;
        }

        private DisplayText DisplayUIText(int i, int j, DisplayText displayText)
        {
            if (i >= verticalDivider)
            {
                displayText = new DisplayText(UIColor, " ");

                if (i == verticalDivider)
                {
                    displayText.text = "-";
                }
                else if (i > verticalDivider)
                {
                    if (j < inputTextArea[(i - verticalDivider) % 3].Length)
                    {
                        displayText.text = inputTextArea[(i - verticalDivider) % 3].ToCharArray()[j].ToString();
                    }
                }
            }
            else if (j >= horizontalDivider)
            {
                displayText.text = " ";

                if (j == horizontalDivider)
                {
                    displayText.text = "|";
                }
                else if (i < sideTextArea.Count && ((j - horizontalDivider) - 1) < sideTextArea.ToArray()[i].Length)
                {
                    displayText.text = sideTextArea.ToArray()[i].ToCharArray()[(j - horizontalDivider) - 1].ToString();
                }
            }

            return displayText;
        }

        private DisplayText DisplayMapText(int i, int j, DisplayText displayText)
        {
            List<Room> rooms = building.GetRooms();

            if (rooms.Count > 0)
            {
                foreach (Room room in rooms)
                {
                    if (room.GetIsExplored() && (centrePoint + (room.GetPosition() * 2)) == new Vector2(j, i))
                    {
                        displayText = new DisplayText(roomColor, "o");
                    }
                }
            }

            return displayText;
        }

        private DisplayText DisplayPlayerText(int i, int j, DisplayText displayText)
        {
            if ((currentScene == GameScene.MapScene && player.GetMapPosition() == new Vector2(j, i)) ||
                (currentScene == GameScene.RoomScene && player.GetRoomPosition() == new Vector2(j - (int)(roomPosition.X), i - (int)(roomPosition.Y))))
            {
                displayText = new DisplayText(playerColor, "@");
            }

            return displayText;
        }

        private DisplayText DisplayRoomText(int i, int j, DisplayText displayText)
        {
            Room currentRoom = building.GetRoomByID(roomID);

            if (roomPosition.Y <= i && (roomPosition.Y + currentRoom.GetRoomSize().Y) >= i && roomPosition.X <= j && (roomPosition.X + currentRoom.GetRoomSize().X) >= j)
            {
                displayText = new DisplayText(roomColor, ".");
                if (i == roomPosition.Y || i == (roomPosition.Y + currentRoom.GetRoomSize().Y))
                {
                    displayText.text = "=";
                }
                else if (j == roomPosition.X || j == (roomPosition.X + currentRoom.GetRoomSize().X))
                {
                    displayText.text = "H";
                }

                Item item;
                if (currentRoom.GetItemCount() > 0 && currentRoom.FindItem(j - (int)(roomPosition.X), i - (int)(roomPosition.Y), out item))
                {
                    displayText = new DisplayText(item.GetColor(), item.GetTextString());
                }

                Interactable interactable;
                if (currentRoom.GetInteractableCount() > 0 && currentRoom.FindInteractable(j - (int)(roomPosition.X), i - (int)(roomPosition.Y), out interactable))
                {
                    displayText = new DisplayText(interactable.GetColor(), interactable.GetTextString());
                }
            }

            return displayText;
        }
    }
}
