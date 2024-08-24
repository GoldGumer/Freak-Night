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
        GameplayScene,
        EditScene
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Screen
        int screenHeight = 1080;
        int screenWidth = 1920;

        //Scene
        GameScene currentScene = GameScene.GameplayScene;

        //Font
        SpriteFont font;
        const int fontHeight = 40;
        const int fontWidth = 32;

        //Keyboards
        KeyboardState previousKeyboard;
        KeyboardState currentKeyboard;

        //Gameplay Display
        Vector2 centrePoint = new Vector2(23,9);
        int horizontalDivider = 46;
        int verticalDivider = 18;

        //Displaying Room
        Vector2 roomPosition;

        //Player
        Vector2 playerPosition = Vector2.One;

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
            _graphics.PreferredBackBufferHeight = screenHeight;
            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.ApplyChanges();

            roomID = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("IBM Plex Mono Light");

            //Building Creation

            string path = Path.Combine(Content.RootDirectory, "ExampleBuilding.json");

            building = new Building(path);
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
            else if (currentScene == GameScene.GameplayScene)
            {
                GameplayUpdate();
            }
            else if (currentScene == GameScene.EditScene)
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
            else if (currentScene == GameScene.GameplayScene)
            {
                GameplayDraw();
            }
            else if (currentScene == GameScene.EditScene)
            { 
                EditModeDraw();
            }

            base.Draw(gameTime);
        }

        private Vector2 CentreMap(Room currentRoom, Vector2 position)
        {
            return position - (currentRoom.GetRoomSize() / 2);
        }

        private Vector2 VectorTimesText(Vector2 vector2)
        {
            return new Vector2(vector2.X * fontWidth, vector2.Y * fontHeight);
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
                playerPosition = Vector2.One;
                currentScene = GameScene.GameplayScene;
            }
        }
        private void EditModeDraw()
        {
            GraphicsDevice.Clear(Color.Black);


            _spriteBatch.Begin();
            DisplayEditText(new Vector2(0.0f, -15.0f));
            _spriteBatch.End();
        }

        private void DisplayEditText()
        {
            DisplayEditText(Vector2.Zero);
        }

        private void DisplayEditText(Vector2 offset)
        {
            List<Room> rooms = new List<Room>();

            bool isInsideRoom = false;

            int upperVertical = (screenHeight / fontHeight) - 1;
            int upperHorizontal = (screenWidth / fontWidth);

            for (int i = 0; i < upperVertical; i++)
            {
                for (int j = 0; j < upperHorizontal; j++)
                {
                    Vector2 textPosition = VectorTimesText(new Vector2(j, i)) + offset;
                    Color textColor = Color.DarkGray;
                    string textString = ".";

                    if (isInsideRoom)
                    {

                    }
                    else
                    {
                        if (rooms.Count > 0)
                        {

                        }
                    }

                    _spriteBatch.DrawString(font, textString, textPosition, textColor);
                }
            }
        }


        //Gameplay Functions

        private void GameplayUpdate()
        {
            if (currentKeyboard.IsKeyDown(Keys.LeftControl) && currentKeyboard.IsKeyDown(Keys.RightAlt) && currentKeyboard.IsKeyDown(Keys.E))
            {
                playerPosition = new Vector2((screenWidth / fontWidth) / 2, (screenHeight / fontHeight) / 2);
                currentScene = GameScene.EditScene;
            }

            Vector2 direction = new Vector2(
                (IsKeyPressed(Keys.Right) ? 1 : 0) -
                (IsKeyPressed(Keys.Left) ? 1 : 0),
                (IsKeyPressed(Keys.Down) ? 1 : 0) -
                (IsKeyPressed(Keys.Up) ? 1 : 0));

            playerPosition += direction;

            Room currentRoom = building.GetRoomByID(roomID);

            Interactable interactable;
            if (currentRoom.FindInteractable(playerPosition, out interactable))
            {
                if (interactable.GetID() < 4)
                {
                }

                switch (interactable.GetID())
                {
                    case 0:
                        roomID = currentRoom.GetConnectedRooms()[0];                        
                        if (building.GetRoomByID(roomID).FindInteractable("SouthDoor", out interactable))
                        {
                            playerPosition = interactable.GetPosition() + new Vector2(0, -1);
                        }
                        break;
                    case 1:
                        roomID = currentRoom.GetConnectedRooms()[1];
                        if (building.GetRoomByID(roomID).FindInteractable("WestDoor", out interactable))
                        {
                            playerPosition = interactable.GetPosition() + new Vector2(1, 0);
                        }
                        break;
                    case 2:
                        roomID = currentRoom.GetConnectedRooms()[2];
                        if (building.GetRoomByID(roomID).FindInteractable("NorthDoor", out interactable))
                        {
                            playerPosition = interactable.GetPosition() + new Vector2(0, 1);
                        }
                        break;
                    case 3:
                        roomID = currentRoom.GetConnectedRooms()[3];
                        if (building.GetRoomByID(roomID).FindInteractable("EastDoor", out interactable))
                        {
                            playerPosition = interactable.GetPosition() + new Vector2(-1, 0);
                        }
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
            }
        }
        private void GameplayDraw()
        {
            GraphicsDevice.Clear(Color.Black);


            _spriteBatch.Begin();
            roomPosition = CentreMap(building.GetRoomByID(roomID), centrePoint);
            DisplayGameText(new Vector2(0.0f, -15.0f));
            _spriteBatch.End();
        }

        private void DisplayGameText()
        {
            DisplayGameText(Vector2.Zero);
        }

        private void DisplayGameText(Vector2 offset)
        {
            Room currentRoom = building.GetRoomByID(roomID);

            int upperVertical = (screenHeight / fontHeight) - 1;
            int upperHorizontal = (screenWidth / fontWidth);

            for (int i = 0; i < upperVertical; i++) 
            {
                for(int j = 0; j < upperHorizontal; j++)
                {
                    Vector2 textPosition = VectorTimesText(new Vector2(j, i)) + offset;
                    Color textColor = Color.White;
                    string textString = " ";

                    if (roomPosition.Y <= i && (roomPosition.Y + currentRoom.GetRoomSize().Y) >= i && roomPosition.X <= j && (roomPosition.X + currentRoom.GetRoomSize().X) >= j)
                    {
                        textColor = roomColor;
                        textString = ".";
                        if (i == roomPosition.Y || i == (roomPosition.Y + currentRoom.GetRoomSize().Y))
                        {
                            textString = "=";
                        }
                        else if (j == roomPosition.X || j == (roomPosition.X + currentRoom.GetRoomSize().X))
                        {
                            textString = "H";
                        }

                        Item item;
                        if (currentRoom.GetItemCount() > 0 && currentRoom.FindItem(j - (int)(roomPosition.X), i - (int)(roomPosition.Y), out item))
                        {
                            textColor = itemColor;
                            textString = "i";
                        }

                        Interactable interactable;
                        if (currentRoom.GetInteractableCount() > 0 && currentRoom.FindInteractable(j - (int)(roomPosition.X), i - (int)(roomPosition.Y), out interactable))
                        {
                            textColor = interactableColor;
                            textString = "#";
                        }
                    }

                    if (i >= verticalDivider)
                    {

                        if (i == verticalDivider || i != (upperVertical - 1))
                        {
                            textColor = UIColor;
                            textString = "-";
                        }
                        else if (i == (upperVertical - 1) && j == 0)
                        {
                            textColor= UIColor;
                            textString = "User Input";
                        }
                    }
                    else if (j >= horizontalDivider)
                    {
                        if (j == horizontalDivider)
                        {
                            textColor = UIColor;
                            textString = "|";
                        }
                    }

                    if (playerPosition == new Vector2(j - (int)(roomPosition.X), i - (int)(roomPosition.Y)))
                    {
                        textColor = playerColor;
                        textString = "@";
                    }

                    _spriteBatch.DrawString(font, textString, textPosition, textColor);
                }
            }
        }
    }
}
