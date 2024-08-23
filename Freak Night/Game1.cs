using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        Building building;
        int roomID;

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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            RoomToScreen();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void RoomToScreen()
        {
            Room currentRoom = building.GetRoomByID(roomID);
            Color roomColor = Color.Gray;
            for (int i = -1; i <= currentRoom.width; i++) 
            {
                for(int j = -1; j <= currentRoom.height; j++)
                {
                    Vector2 textPosition = new Vector2((i + 1) * fontWidth, (j + 1) * fontHeight);
                    if ((i == -1 || i == currentRoom.width) && (j == -1 || j == currentRoom.height))
                    {
                        if (i == j)
                        {
                            _spriteBatch.DrawString(font, "/", textPosition, roomColor);
                        }
                        else
                        {
                            _spriteBatch.DrawString(font, "\\", textPosition, roomColor);
                        }
                    }
                    else if (i == -1 || i == currentRoom.width)
                    {
                        _spriteBatch.DrawString(font, "|", textPosition, roomColor);
                    }
                    else if (j == -1 || j == currentRoom.height)
                    {
                        _spriteBatch.DrawString(font, "-", textPosition, roomColor);
                    }
                    else
                    {
                        _spriteBatch.DrawString(font, ".", textPosition, roomColor);
                    }
                }
            }
        }
    }
}
