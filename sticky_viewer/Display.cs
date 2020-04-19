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

        private Texture2D upleft;
        private Texture2D up;
        private Texture2D upright;
        private Texture2D downleft;
        private Texture2D down;
        private Texture2D downright;
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

        Stream GetStreamFromFileOrAssembly(Assembly assembly, string s)
        {
            try
            {
                return new FileStream($"{s}.png", FileMode.Open);
            }
            catch
            {
                // uh oh,! time to panic
                return assembly.GetManifestResourceStream(assembly.GetName().Name.Replace(' ', '_') + $".Content.{s}.png");
            }
        }

        Texture2D GetTexture(Assembly assembly, string s)
        {
            using (Stream stream = GetStreamFromFileOrAssembly(assembly, s))
            {
                return Texture2D.FromStream(GraphicsDevice, stream);
            }
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
            int x = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - w / 2;
            int y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - h;

            User32.SetWindowPos(hWnd, hWndInsertAfter, x, y, w, h, 0);

            // default images
            Assembly assembly = Assembly.GetExecutingAssembly();

            pressed = GetTexture(assembly, "pressed");
            unpressed = GetTexture(assembly, "unpressed");
            upleft = GetTexture(assembly, "upleft");
            up = GetTexture(assembly, "up");
            upright = GetTexture(assembly, "upright");
            left = GetTexture(assembly, "left");
            neutral = GetTexture(assembly, "neutral");
            right = GetTexture(assembly, "right");
            downleft = GetTexture(assembly, "downleft");
            down = GetTexture(assembly, "down");
            downright = GetTexture(assembly, "downright");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
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

            GamePadDPad dpad = GamePad.GetState(PlayerIndex.One).DPad;

            if (dpad.Up == ButtonState.Pressed && dpad.Left == ButtonState.Pressed)
            {
                stick = upleft;
            }
            else if (dpad.Up == ButtonState.Pressed && dpad.Right == ButtonState.Pressed)
            {
                stick = upright;
            }
            else if (dpad.Up == ButtonState.Pressed)
            {
                stick = up;
            }
            else if (dpad.Down == ButtonState.Pressed && dpad.Left == ButtonState.Pressed)
            {
                stick = downleft;
            }
            else if (dpad.Down == ButtonState.Pressed && dpad.Right == ButtonState.Pressed)
            {
                stick = downright;
            }
            else if (dpad.Down == ButtonState.Pressed)
            {
                stick = down;
            }
            else if (dpad.Left == ButtonState.Pressed)
            {
                stick = left;
            }
            else if (dpad.Right == ButtonState.Pressed)
            {
                stick = right;
            }
            else
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

            spriteBatch.Draw(stick, new Vector2(21, 16), Color.White);
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
