using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace Freak_Night
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        int screenHeight = 1080;
        int screenWidth = 1920;

        SpriteFont font;
        const int fontHeight = 40;
        const int fontWidth = 32;

        Vector2 centrePoint = new Vector2(23,9);
        int horizontalDivider = 46;
        int verticalDivider = 18;

        Vector2 roomPosition;

        Building building;
        int roomID;
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

            building = new Building(Path.Combine(Content.RootDirectory, "ExampleBuilding.txt"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                roomID++;
                if (roomID >= building.rooms.Count)
                {
                    roomID = building.rooms.Count - 1;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                roomID--;
                if (roomID <= 0)
                {
                    roomID = 0;
                }
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            roomPosition = CentreMap(building.GetRoomByID(roomID), centrePoint);

            _spriteBatch.Begin();
            DisplayText(new Vector2(0.0f, -15.0f));
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public Vector2 CentreMap(Room currentRoom, Vector2 position)
        {
            return new Vector2(position.X - (currentRoom.width / 2), position.Y - (currentRoom.height / 2));
        }

        public Vector2 VectorTimesText(Vector2 vector2)
        {
            return new Vector2(vector2.X * fontWidth, vector2.Y * fontHeight);
        }

        public void DisplayText()
        {
            DisplayText(Vector2.Zero);
        }

        public void DisplayText(Vector2 Offset)
        {
            Room currentRoom = building.GetRoomByID(roomID);
            
            for (int i = 0; i < screenHeight / fontHeight; i++) 
            {
                for(int j = 0; j < screenWidth / fontWidth; j++)
                {
                    Vector2 textPosition = VectorTimesText(new Vector2(j, i)) + Offset;
                    Color textColor = Color.White;
                    string textString = " ";

                    if ((roomPosition.Y - 1) <= i && (roomPosition.Y + currentRoom.height) >= i && (roomPosition.X - 1) <= j && (roomPosition.X + currentRoom.width) >= j)
                    {
                        textColor = roomColor;
                        textString = ".";
                        if (i == (roomPosition.Y - 1) || i == (roomPosition.Y + currentRoom.height))
                        {
                            textString = "=";
                        }
                        else if (j == (roomPosition.X - 1) || j == (roomPosition.X + currentRoom.width))
                        {
                            textString = "H";
                        }

                        Item item;
                        if (currentRoom.items.Count > 0 && currentRoom.FindItem(j - (int)(roomPosition.X), i - (int)(roomPosition.Y), out item))
                        {
                            textColor = itemColor;
                            textString = "i";
                        }

                        Interactable interactable;
                        if (currentRoom.interactables.Count > 0 && currentRoom.FindInteractable(j - (int)(roomPosition.X), i - (int)(roomPosition.Y), out interactable))
                        {
                            textColor = interactableColor;
                            textString = "#";
                        }
                    }

                    if (i == verticalDivider)
                    {
                        textColor = UIColor;
                        textString = "-";
                    }
                    else if (j == horizontalDivider && i < verticalDivider)
                    {
                        textColor = UIColor;
                        textString = "|";
                    }

                    _spriteBatch.DrawString(font, textString, textPosition, textColor);
                }
            }
        }
    }
}
