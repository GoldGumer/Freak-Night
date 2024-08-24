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

        //Display
        Vector2 roomPosition;
        Vector2 centrePoint;
        int horizontalDivider;
        int verticalDivider;
        string defaultBackground;
        bool isEditing;
        string sideAreaText;
        string inputAreaText;

        //Player
        Vector2 playerRoomPosition;
        Vector2 playerMapPosition;

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

            horizontalDivider = 46;
            verticalDivider = 22;
            centrePoint = new Vector2(horizontalDivider / 2, verticalDivider / 2);
            defaultBackground = " ";

            isEditing = false;

            roomID = 0;

            playerRoomPosition = Vector2.One;
            playerMapPosition = centrePoint;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("IBM Plex Mono Light");

            //Building Creation

            string path = Path.Combine(Content.RootDirectory, "ExampleBuilding.json");

            building = new Building(path);

            playerRoomPosition = building.GetRoomByID(roomID).GetRoomSize() / 2;
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

        private void TextInputHandler(object sender, TextInputEventArgs args)
        {
            var pressedKey = args.Key;
            var character = args.Character;

            inputAreaText += character.ToString();
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

            if (IsKeyPressed(Keys.M) && currentKeyboard.IsKeyDown(Keys.RightAlt))
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
                playerRoomPosition += direction;


            }
            else if (currentScene == GameScene.MapScene)
            {
                playerMapPosition += direction;
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
                if (currentScene == GameScene.RoomScene)
                {
                    currentScene = GameScene.MapScene;
                }
                else if (currentScene == GameScene.MapScene)
                {
                    currentScene = GameScene.RoomScene;
                }
            }

            Room currentRoom = building.GetRoomByID(roomID);

            Interactable interactable;
            if (currentRoom.FindInteractable(playerRoomPosition + direction, out interactable))
            {
                switch (interactable.GetID())
                {
                    case 0:
                        roomID = currentRoom.GetConnectedRooms()[0];
                        if (building.GetRoomByID(roomID).FindInteractable("SouthDoor", out interactable))
                        {
                            playerRoomPosition = interactable.GetPosition() + new Vector2(0, -1);
                        }
                        break;
                    case 1:
                        roomID = currentRoom.GetConnectedRooms()[1];
                        if (building.GetRoomByID(roomID).FindInteractable("WestDoor", out interactable))
                        {
                            playerRoomPosition = interactable.GetPosition() + new Vector2(1, 0);
                        }
                        break;
                    case 2:
                        roomID = currentRoom.GetConnectedRooms()[2];
                        if (building.GetRoomByID(roomID).FindInteractable("NorthDoor", out interactable))
                        {
                            playerRoomPosition = interactable.GetPosition() + new Vector2(0, 1);
                        }
                        break;
                    case 3:
                        roomID = currentRoom.GetConnectedRooms()[3];
                        if (building.GetRoomByID(roomID).FindInteractable("EastDoor", out interactable))
                        {
                            playerRoomPosition = interactable.GetPosition() + new Vector2(-1, 0);
                        }
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
            }

            if (currentScene == GameScene.RoomScene)
            {
                if ((playerRoomPosition + direction).X < currentRoom.GetRoomSize().X &&
                    (playerRoomPosition + direction).X > 0 &&
                    (playerRoomPosition + direction).Y < currentRoom.GetRoomSize().Y &&
                    (playerRoomPosition + direction).Y > 0
                    )
                {
                    playerRoomPosition += direction;
                }
            }
            else if (currentScene == GameScene.MapScene)
            {
                playerMapPosition += direction;
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
                    else if (currentScene == GameScene.RoomScene)
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
                    displayText = new DisplayText(UIColor, "-");
                }
            }
            else if (j >= horizontalDivider)
            {
                displayText = new DisplayText(UIColor, " ");

                if (j == horizontalDivider)
                {
                    displayText = new DisplayText(UIColor, "|");
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
            if ((currentScene == GameScene.MapScene && playerMapPosition == new Vector2(j, i)) ||
                (currentScene == GameScene.RoomScene && playerRoomPosition == new Vector2(j - (int)(roomPosition.X), i - (int)(roomPosition.Y))))
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
