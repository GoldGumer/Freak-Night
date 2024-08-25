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
        GameScene currentScene = GameScene.RoomScene;

        //Font
        SpriteFont font;
        const int fontHeight = 40;
        const int fontWidth = 32;

        //Keyboards
        KeyboardState previousKeyboard;
        KeyboardState currentKeyboard;

        //Comands
        static readonly string[] gameplayCommands = { "i", "inspect","x", "eximine", "p", "pickup", "d", "drop", "h", "hide", "m", "map", "u", "use", "ping" };
        static readonly string[] editingCommands = { "add", "remove", "pickup", "place", "change" };

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

            roomID = 0;

            player = new Player(Vector2.One, centrePoint, new List<Item>() { });

            sideTextArea = new Queue<string>();
            inputTextArea = new string[2] { "", ""};

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("IBM Plex Mono Light");

            //Building Creation

            string path = Path.Combine(Content.RootDirectory, "ExampleBuilding.json");

            building = new Building(path);

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

        private void TextInputHandler(object sender, TextInputEventArgs args)
        {
            var pressedKey = args.Key;
            var character = args.Character;
            if (!isEditing)
            {
                if (pressedKey == Keys.Enter)
                {
                    Room currentRoom = building.GetRoomByID(roomID);
                    string command = Array.Find<string>(gameplayCommands, command => command == inputTextArea[0].Split(' ')[0]);
                    if (command == "x" || command == "eximine")
                    {
                        Item item;
                        if (inputTextArea[0].Split(' ').Length > 1 && currentRoom.FindItem(inputTextArea[0].Split(' ')[1], out item))
                        {
                            inputTextArea[0] = "";
                            inputTextArea[1] = item.description;
                        }
                        else
                        {
                            inputTextArea[0] = "";
                            inputTextArea[1] = "Couldn't find an item by that name.";
                        }
                    }
                    else if (inputTextArea[0].Split(' ').Length > 1 && command == "p" || command == "pickup")
                    {
                        Item item;
                        if (currentRoom.FindItem(inputTextArea[0].Split(' ')[1], out item))
                        {
                            inputTextArea[0] = "";
                            inputTextArea[1] = "Picked up " + item.name.ToLower() + ".";

                            player.AddItem(item);
                            currentRoom.RemoveItem(item);
                        }
                        else
                        {
                            inputTextArea[0] = "";
                            inputTextArea[1] = "Couldn't find an item by that name.";
                        }
                    }
                    else if (inputTextArea[0].Split(' ').Length > 1 && command == "d" || command == "drop")
                    {
                        Item item;
                        if (player.FindItem(inputTextArea[0].Split(' ')[1], out item))
                        {
                            inputTextArea[0] = "";
                            inputTextArea[1] = "Dropped " + item.name + ".";

                            currentRoom.AddItem(item, player.GetRoomPosition());
                            player.RemoveItem(item);
                        }
                    }
                    else if (command == "h" || command == "hide")
                    {
                        Interactable interactable;
                        if (currentRoom.FindInteractable("Cabin", out interactable))
                        {

                        }
                        else
                        {
                            inputTextArea[0] = "";
                            inputTextArea[1] = "Couldn't find a place to hide.";
                        }
                    }
                    else if (command == "m" || command == "map")
                    {

                    }
                    else if ((inputTextArea[0].Split(' ').Length > 2 && inputTextArea[0].Split(' ')[2].ToLower() != "in") ||
                        (inputTextArea[0].Split(' ').Length > 3) &&
                        command == "u" || command == "use")
                    {

                    }
                    else if (command == "ping")
                    {

                    }
                    else
                    {
                        inputTextArea[0] = "";
                        inputTextArea[1] = "Couldn't find a command to match what you are trying to do.";
                    }
                }
                else if (pressedKey == Keys.Back)
                {
                    inputTextArea[0] = inputTextArea[0].Remove(inputTextArea[0].Length - 1);
                }
                else if (pressedKey <= (Keys)31)
                {

                }
                else
                {
                    inputTextArea[0] += character.ToString();
                }
            }
            else
            {
                if (pressedKey == Keys.Enter)
                {
                    string command = Array.Find<string>(gameplayCommands, command => command == inputTextArea[0].Split(' ')[0]);

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

        //Main Menu Functions

        private void MainMenuUpdate()
        {

        }

        private void MainMenuDraw()
        {
            GraphicsDevice.Clear(Color.Black);


            _spriteBatch.Begin();

            _spriteBatch.End();
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
                if (building.FindRoom((player.GetMapPosition() - centrePoint) / 2, out room))
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
                else if (!building.FindRoom((player.GetMapPosition() - centrePoint) / 2))
                {
                    sideTextArea.Clear();
                }
            }
        }

        private void QueueStringToSide(string textToQueue)
        {
            for (int i = 0; i < textToQueue.Length; i += (screenWidth / fontWidth) - horizontalDivider - 1)
            {
                sideTextArea.Enqueue(textToQueue.Substring(i, (int)MathF.Min((screenWidth / fontWidth) - horizontalDivider - 1, textToQueue.Length - i)));
            }
        }

        //Gameplay Functions

        private void GameplayUpdate()
        {
            if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.RightAlt) && currentKeyboard.IsKeyDown(Keys.E))
            {
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
                        roomID = currentRoom.GetConnectedRooms()[0];
                        if (building.GetRoomByID(roomID).FindInteractable("SouthDoor", out interactable))
                        {
                            player.SetRoomPosition(interactable.GetPosition() + new Vector2(0, -1));
                        }
                        break;
                    case 1:
                        roomID = currentRoom.GetConnectedRooms()[1];
                        if (building.GetRoomByID(roomID).FindInteractable("WestDoor", out interactable))
                        {
                            player.SetRoomPosition(interactable.GetPosition() + new Vector2(1, 0));
                        }
                        break;
                    case 2:
                        roomID = currentRoom.GetConnectedRooms()[2];
                        if (building.GetRoomByID(roomID).FindInteractable("NorthDoor", out interactable))
                        {
                            player.SetRoomPosition(interactable.GetPosition() + new Vector2(0, 1));
                        }
                        break;
                    case 3:
                        roomID = currentRoom.GetConnectedRooms()[3];
                        if (building.GetRoomByID(roomID).FindInteractable("EastDoor", out interactable))
                        {
                            player.SetRoomPosition(interactable.GetPosition() + new Vector2(-1, 0));
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
                if ((player.GetRoomPosition() + direction).X < currentRoom.GetRoomSize().X &&
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
                            sideTextArea.Enqueue(item.name);
                        }
                    }
                }
            }
            else if (currentScene == GameScene.MapScene)
            {
                player.SetMapPosition(player.GetMapPosition() + direction);
                Room room;
                if (building.FindRoom((player.GetMapPosition() - centrePoint) / 2, out room) && sideTextArea.Count != 1 + room.GetItemCount())
                {
                    sideTextArea.Clear();
                    sideTextArea.Enqueue(room.name);
                    sideTextArea.Enqueue("");
                    foreach (Item item in room.GetItems())
                    {
                        if (!sideTextArea.Contains(item.name))
                        {
                            sideTextArea.Enqueue(item.name);
                        }
                    }
                }
                else if (!building.FindRoom((player.GetMapPosition() - centrePoint) / 2))
                {
                    sideTextArea.Clear();
                }
            }
        }

        //Display Options

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


            _spriteBatch.Begin();
            roomPosition = centrePoint - (building.GetRoomByID(roomID).GetRoomSize() / 2);
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

                    if (currentScene == GameScene.MapScene)
                    {
                        displayText = DisplayMapText(i, j, displayText);
                    }
                    else if (currentScene == GameScene.RoomScene && isMapDisplaying)
                    {
                        displayText = DisplayRoomText(i, j, displayText);
                    }


                    displayText = DisplayUIText(i, j, displayText);

                    displayText = DisplayPlayerText(i, j, displayText);

                    _spriteBatch.DrawString(font, displayText.text, textPosition, displayText.color);
                }
            }
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
                    if (i - verticalDivider < 3 && j < inputTextArea[1].Length)
                    {
                        displayText.text = inputTextArea[1].ToCharArray()[j].ToString();
                    }
                    else if (i - verticalDivider == 3 && j < inputTextArea[0].Length)
                    {
                        displayText.text = inputTextArea[0].ToCharArray()[j].ToString();
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
