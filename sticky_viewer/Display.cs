using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace sticky_viewer
{
    /// <summary>
    /// Access functionality from user32.dll
    /// </summary>
    class User32
    {
        [DllImport("user32.dll")]
        public static extern void SetWindowPos(uint Hwnd, int Level, int X, int Y, int W, int H, uint Flags);

        public static readonly int HWND_TOPMOST = -1;
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Display : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D pressed;
        private Texture2D unpressed;

        private Texture2D left;
        private Texture2D neutral;
        private Texture2D right;

        private Texture2D stick;
        private Texture2D A;
        private Texture2D B;
        private Texture2D C;
        private Texture2D J;
        private Texture2D S;

        public Display()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 200;
            graphics.PreferredBackBufferHeight = 75;
            Window.IsBorderless = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Set window position and make it always on top
            uint hWnd = (uint)this.Window.Handle;
            int hWndInsertAfter = User32.HWND_TOPMOST;
            int w = graphics.PreferredBackBufferWidth;
            int h = graphics.PreferredBackBufferHeight;
            int x = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/2 - w/2;
            int y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - h;

            User32.SetWindowPos(hWnd, hWndInsertAfter, x, y, w, h, 0);

            // default images
            Assembly assembly = Assembly.GetExecutingAssembly();

            Stream dataStream = assembly.GetManifestResourceStream(assembly.GetName().Name.Replace(' ', '_') + ".Content.left.png");
            left = Texture2D.FromStream(GraphicsDevice, dataStream);
            dataStream.Dispose();

            dataStream = assembly.GetManifestResourceStream(assembly.GetName().Name.Replace(' ', '_') + ".Content.neutral.png");
            neutral = Texture2D.FromStream(GraphicsDevice, dataStream);
            dataStream.Dispose();

            dataStream = assembly.GetManifestResourceStream(assembly.GetName().Name.Replace(' ', '_') + ".Content.right.png");
            right = Texture2D.FromStream(GraphicsDevice, dataStream);
            dataStream.Dispose();

            dataStream = assembly.GetManifestResourceStream(assembly.GetName().Name.Replace(' ', '_') + ".Content.pressed.png");
            pressed = Texture2D.FromStream(GraphicsDevice, dataStream);
            dataStream.Dispose();

            dataStream = assembly.GetManifestResourceStream(assembly.GetName().Name.Replace(' ', '_') + ".Content.unpressed.png");
            unpressed = Texture2D.FromStream(GraphicsDevice, dataStream);
            dataStream.Dispose();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // If an image file exists, replace the one in memory with it
            FileStream fileStream;

            if (File.Exists("pressed.png"))
            {
                fileStream = new FileStream("pressed.png", FileMode.Open);
                pressed = Texture2D.FromStream(GraphicsDevice, fileStream);
                fileStream.Dispose();
            }

            if (File.Exists("unpressed.png"))
            {
                fileStream = new FileStream("unpressed.png", FileMode.Open);
                unpressed = Texture2D.FromStream(GraphicsDevice, fileStream);
                fileStream.Dispose();
            }

            if (File.Exists("left.png"))
            {
                fileStream = new FileStream("left.png", FileMode.Open);
                left = Texture2D.FromStream(GraphicsDevice, fileStream);
                fileStream.Dispose();
            }

            if (File.Exists("neutral.png"))
            {
                fileStream = new FileStream("neutral.png", FileMode.Open);
                neutral = Texture2D.FromStream(GraphicsDevice, fileStream);
                fileStream.Dispose();
            }

            if (File.Exists("right.png"))
            {
                fileStream = new FileStream("right.png", FileMode.Open);
                right = Texture2D.FromStream(GraphicsDevice, fileStream);
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
            {
                stick = left;
            } else if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
            {
                stick = right;
            } else
            {
                stick = neutral;
            }

            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            Dictionary<string, bool> inputs = new Dictionary<string, bool>()
            {
                {"A", currentState.Buttons.A == ButtonState.Pressed},
                {"B", currentState.Buttons.B == ButtonState.Pressed},
                {"X", currentState.Buttons.X == ButtonState.Pressed},
                {"Y", currentState.Buttons.Y == ButtonState.Pressed},
                {"RB", currentState.Buttons.RightShoulder == ButtonState.Pressed},
                {"RT", currentState.Triggers.Right >= 0.5 },
                {"LB", currentState.Buttons.LeftShoulder == ButtonState.Pressed},
                {"LT", currentState.Triggers.Left >= 0.5},
            };

            A = inputs[ConfigurationManager.AppSettings["A"] ?? "Y"] ? pressed : unpressed;
            B = inputs[ConfigurationManager.AppSettings["B"] ?? "X"] ? pressed : unpressed;
            C = inputs[ConfigurationManager.AppSettings["C"] ?? "RB"] ? pressed : unpressed;
            J = inputs[ConfigurationManager.AppSettings["J"] ?? "B"] ? pressed : unpressed;
            S = inputs[ConfigurationManager.AppSettings["S"] ?? "A"] ? pressed : unpressed;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(stick, new Vector2(16, 24), Color.White);
            spriteBatch.Draw(A, new Vector2(96, 11), Color.White);
            spriteBatch.Draw(B, new Vector2(128, 6), Color.White);
            spriteBatch.Draw(C, new Vector2(161, 6), Color.White);
            spriteBatch.Draw(J, new Vector2(86, 41), Color.White);
            spriteBatch.Draw(S, new Vector2(118, 36), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
