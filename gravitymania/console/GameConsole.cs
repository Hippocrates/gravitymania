using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using gravitymania.graphics;
using Microsoft.Xna.Framework.Content;
using gravitymania.math;

namespace gravitymania.console
{
    public class GameConsole
    {
        public SpriteFont Font { get; set; }
		public SpriteBatch SpriteDrawer { get; set; }
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public uint CaretBlinkRate { get; set; }
        public float TextScale { get; set; }

        public bool IsOpen { get; set; }
        public int NumVisibleLines 
        {
            get { return history.HistoryWindowSize; } 
            set { history.HistoryWindowSize = NumTools.Clamp(value, 0, 15); } 
        }

        public int ScrollAmount { get; set; }

        public String InputText { get { return this.inputLine.Text; } }
        public int CaretPosition { get { return this.inputLine.CaretPosition; } }

		// This is aparantly a trick to get around the 'null event' problem
		// I suspect that having 'null events' is useful to avoid having to calculate data
		// unneccessarily, but in this instance, I want to generate the string no matter what anyways
		public event Action<string> CommandEntered = delegate { };

        public GameConsole()
        {
            // n.b. if this is going to be a thing, we should have a global 1-pixel texture to use for all such drawing applications
            // (and should also probably have a seperate class to handle rendering such primitives like rectangles, lines, circles, et. al.
            BackgroundColor = DefaultBackgroundColor;
            TextColor = DefaultTextColor;
            CaretBlinkRate = DefaultCaretBlinkRate;
            TextScale = DefaultTextScale;

            IsOpen = false;
            history = new ConsoleHistory();
            inputLine = new ConsoleInputLine();
            NumVisibleLines = DefaultNumVisibleLines;
            keyReader = new ConsoleKeyReader();
            ScrollAmount = DefaultScrollAmount;
        }

        public void PrintLine(string line)
        {
            this.history.InsertEcho(line);
        }

        public void ClearScreen()
        {
            this.history.ClearEchoHistory();
        }

        // TODO: eventually, this should be replaced with an input handler event
        public void Update(KeyboardState keys)
        {
            this.keyReader.Update(keys);

            if (IsOpen)
            {
                switch (this.keyReader.GetCurrentKey())
                {
                    case CloseConsoleKey:
                        IsOpen = false;
                        break;
                    case IssueCommandKey:
                        this.history.InsertCommand(InputText);
                        this.CommandEntered(InputText);
                        this.inputLine.Text = "";
                        break;
                    case PreviousCommandKey:
                        --this.history.CurrentCommand;
                        this.inputLine.Text = this.history.GetCurrentCommand();
                        this.inputLine.CaretPosition = this.InputText.Length;
                        break;
                    case NextCommandKey:
                        ++this.history.CurrentCommand;
                        this.inputLine.Text = this.history.GetCurrentCommand();
                        this.inputLine.CaretPosition = this.InputText.Length;
                        break;
                    case ScrollUpKey:
                        this.history.HistoryLocation -= (int)ScrollAmount;
                        break;
                    case ScrollDownKey:
                        this.history.HistoryLocation += (int)ScrollAmount;
                        break;
                    default:
                        this.inputLine.Update(this.keyReader.GetModifiers(), this.keyReader.GetCurrentKey());
                        ++caretBlinkCounter;
                        if (caretBlinkCounter >= CaretBlinkRate)
                        {
                            this.caretOn = !this.caretOn;
                            caretBlinkCounter = 0;
                        }
                        break;
                }
            }
            else
            {
                if (this.keyReader.GetCurrentKey() == OpenConsoleKey)
                {
                    IsOpen = true;
                    history.ScrollToMostRecent();
                    this.caretOn = false;
                    this.caretBlinkCounter = 0;
                }
            }
        }

        public void LoadContent(GraphicsDevice device, ContentManager content)
        {
            onePixelTexture = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
            onePixelTexture.SetData(new[] { Color.White });

            Font = content.Load<SpriteFont>(DefaultFontName);

			SpriteDrawer = new SpriteBatch(device);
        }

        public void Draw()
        {
            if (IsOpen)
            {
				SpriteDrawer.Begin();

                // These should probably be extracted into a glyphimetrics library or something, 
                // since we'll need text sizing and output in other places as well
				float height = Math.Min(((Font.LineSpacing * TextScale) * (NumVisibleLines + 1)), SpriteDrawer.GraphicsDevice.Viewport.Height);
				float width = SpriteDrawer.GraphicsDevice.Viewport.Width;
                float inputLinePosition = height - (Font.LineSpacing * TextScale);
                // //////////////////////////////////////////////////////////////////////////////

                // draws the background using a cheap 1-pixel texture
				SpriteDrawer.Draw(onePixelTexture, new Rectangle(0, 0, (int)width, (int)height), BackgroundColor);

                int lineNumber = 0;
                foreach (var line in history.GetHistoryWindow())
                {
					SpriteDrawer.DrawString(Font, line, new Vector2(0.0f, lineNumber * (Font.LineSpacing * TextScale)), TextColor, 0.0f, new Vector2(), TextScale, SpriteEffects.None, 0);
                    ++lineNumber;
                }

                // The most important thing in this line is the 'scale' parameter, since that's what controls text size
				SpriteDrawer.DrawString(Font, "> " + InputText, new Vector2(0.0f, inputLinePosition), TextColor, 0.0f, new Vector2(), TextScale, SpriteEffects.None, 0);

                if (caretOn)
                {
                    // Similar glyphimetrics crap going on here
                    float caretPosition = (Font.MeasureString("> " + InputText.Substring(0, CaretPosition)).X * TextScale);

					SpriteDrawer.DrawString(Font, "_", new Vector2(caretPosition, inputLinePosition), TextColor, 0.0f, new Vector2(), TextScale, SpriteEffects.None, 0);
                }

				SpriteDrawer.End();
            }
        }

        private Texture2D onePixelTexture;
        private uint caretBlinkCounter;
        private bool caretOn;

        private ConsoleInputLine inputLine;
        private ConsoleKeyReader keyReader;
        private ConsoleHistory history;

        private const uint DefaultCaretBlinkRate = 30;
        private const float DefaultTextScale = 1.0f;
        private readonly Vector2 DefaultPosition = new Vector2(0.0f, 0.0f);
        private readonly Color DefaultTextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        private readonly Color DefaultBackgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

        // TODO: make the open and close keys settable
        private const Keys OpenConsoleKey = Keys.OemTilde;
        private const Keys CloseConsoleKey = Keys.Escape;
        private const Keys IssueCommandKey = Keys.Enter;
        private const Keys PreviousCommandKey = Keys.Up;
        private const Keys NextCommandKey = Keys.Down;
        private const Keys ScrollUpKey = Keys.PageUp;
        private const Keys ScrollDownKey = Keys.PageDown;
        private const int DefaultNumVisibleLines = 8;
        private const int DefaultScrollAmount = 1;
        private const string DefaultFontName = "Fonts/Default";
    }
}
