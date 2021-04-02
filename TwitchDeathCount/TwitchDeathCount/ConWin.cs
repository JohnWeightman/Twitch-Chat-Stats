using System;
using System.Timers;
using System.Collections.Generic;
using Debugger;

namespace TwitchDeathCount
{
    static class ConWin
    {

        static List<string> TwitchLog = new List<string>();
        static bool UDTwitchLog;
        static Timer Timer;

        static int time;
        #region Start

        public static void ConWinStart()
        {
            DrawUI();
            SetTimer();
        }

        #region Draw UI

        static void DrawUI()
        {
            DrawBorders();
            DrawOutputBoxes();
            StartVars();
        }

        static void DrawBorders()
        {
            Console.SetWindowSize(153, 51);
            Console.SetBufferSize(153, 51);
            Draw.RectangleFromTop(152, 48, 0, 0, ConsoleColor.Blue);
            Draw.RectangleFromTop(152, 14, 0, 0, ConsoleColor.Blue);
            Draw.RectangleFromTop(82, 33, 0, 15, ConsoleColor.Blue);
            Draw.RectangleFromTop(70, 17, 82, 15, ConsoleColor.Blue);
        }

        static void DrawOutputBoxes()
        {
            for (int x = -28; x < 120; x++)
            {
                x += 29;
                for (int y = -2; y < 10; y++)
                {
                    y += 3;
                    Draw.RectangleFromTop(30, 3, x, y, ConsoleColor.Green);
                }
            }
        }

        static void StartVars()
        {
            for (int x = 1; x < 16; x++)
                UpdateVarBoxes(x, "Var " + x);
        }

        #endregion

        #region Timer

        static void SetTimer()
        {
            Timer = new Timer(1000);
            Timer.Elapsed += UpdateDisplay;
            Timer.AutoReset = true;
            Timer.Enabled = true;
        }

        static void UpdateDisplay(Object source, ElapsedEventArgs e)
        {
            if (UDTwitchLog)
                DisplayTwitchLog();
            UpdateVarBoxes(14, "Time: " + time);
            UpdateVarBoxes(15, "Connected: " + Twitch.TwitchClient.Connected);
            time++;
        }

        #endregion

        #endregion

        #region Update Console Window

        #region Variable Boxes

        public static void UpdateVarBoxes(int VarBox, string VarString)
        {
            Tuple<int, int> Coords = GetCoords(VarBox);
            if (Coords.Item1 == -1)
                return;
            ClearVarBox(Coords);
            Console.Write(VarString);
        }

        static void ClearVarBox(Tuple<int, int> Coords)
        {
            Console.SetCursorPosition(Coords.Item1, Coords.Item2);
            for (int x = 0; x < 29; x++)
                Console.Write(" ");
            Console.SetCursorPosition(Coords.Item1, Coords.Item2);
        }

        static Tuple<int, int> GetCoords(int VarBox)
        {
            Tuple<int, int> Coords;
            switch (VarBox)
            {
                case 1:
                    Coords = new Tuple<int, int>(2, 3);
                    break;
                case 2:
                    Coords = new Tuple<int, int>(32, 3);
                    break;
                case 3:
                    Coords = new Tuple<int, int>(62, 3);
                    break;
                case 4:
                    Coords = new Tuple<int, int>(92, 3);
                    break;
                case 5:
                    Coords = new Tuple<int, int>(122, 3);
                    break;
                case 6:
                    Coords = new Tuple<int, int>(2, 7);
                    break;
                case 7:
                    Coords = new Tuple<int, int>(32, 7);
                    break;
                case 8:
                    Coords = new Tuple<int, int>(62, 7);
                    break;
                case 9:
                    Coords = new Tuple<int, int>(92, 7);
                    break;
                case 10:
                    Coords = new Tuple<int, int>(122, 7);
                    break;
                case 11:
                    Coords = new Tuple<int, int>(2, 11);
                    break;
                case 12:
                    Coords = new Tuple<int, int>(32, 11);
                    break;
                case 13:
                    Coords = new Tuple<int, int>(62, 11);
                    break;
                case 14:
                    Coords = new Tuple<int, int>(92, 11);
                    break;
                case 15:
                    Coords = new Tuple<int, int>(122, 11);
                    break;
                default:
                    Debug.Log("ConWin - GetCoords() -> Invalid VarBox Value!", 1);
                    Coords = new Tuple<int, int>(-1, -1);
                    break;
            }
            return Coords;
        }

        #endregion

        static void DisplayTwitchLog()
        {
            UDTwitchLog = false;
            int y = 17;
            for (int i = 0; i < TwitchLog.Count; i++)
            {
                Console.SetCursorPosition(1, y);
                Console.Write(TwitchLog[i]);
                if (i - 1 >= 0)
                    if (TwitchLog[i - 1].Length > TwitchLog[i].Length)
                    {
                        int Count = TwitchLog[i - 1].Length - TwitchLog[i].Length;
                        for (int l = 0; l < Count; l++)
                        {
                            Console.SetCursorPosition(1 + TwitchLog[i].Length + l, y);
                            Console.Write(" ");
                        }
                    }
                y += 1;
            }
        }

        #endregion

        #region Update Logs

        public static void UpdateTwitchLog(string Log)
        {
            if (Log.Length >= 81)
            {
                bool TooLong = true;
                while (TooLong)
                {
                    string Temp = Log.Substring(0, 80);
                    TwitchLog.Add(Temp);
                    if (TwitchLog.Count > 32)
                        TwitchLog.RemoveAt(0);
                    Log = Log.Substring(80, Log.Length - 80);
                    if (Log.Length < 81)
                    {
                        TwitchLog.Add(Log);
                        if (TwitchLog.Count > 32)
                            TwitchLog.RemoveAt(0);
                        TooLong = !TooLong;
                    }
                }
            }
            else
            {
                TwitchLog.Add(Log);
                if (TwitchLog.Count > 32)
                    TwitchLog.RemoveAt(0);
            }
            if (!UDTwitchLog)
                UDTwitchLog = true;
        }

        #endregion
    }

    public static class Draw///Externally Sourced Class
    {
        /// <summary>
        /// Draws a rectangle in the console using several WriteLine() calls.
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The right of the rectangle.</param>
        /// <param name="xLocation">The left side position.</param>
        /// <param name="yLocation">The top position.</param>
        /// <param name="keepOriginalCursorLocation">If true, 
        /// the cursor will return back to the starting location.</param>
        /// <param name="color">The color to use. null=uses current color Default: null</param>
        /// <param name="useDoubleLines">Enables double line boarders. Default: false</param>
        public static void RectangleFromCursor(int width,
            int height,
            int xLocation = 0,
            int yLocation = 0,
            bool keepOriginalCursorLocation = false,
            ConsoleColor? color = null,
            bool useDoubleLines = false)
        {
            {
                // Save original cursor location
                int savedCursorTop = Console.CursorTop;
                int savedCursorLeft = Console.CursorLeft;

                // if the size is smaller then 1 then don't do anything
                if (width < 1 || height < 1)
                {
                    return;
                }

                // Save and then set cursor color
                ConsoleColor savedColor = Console.ForegroundColor;
                if (color.HasValue)
                {
                    Console.ForegroundColor = color.Value;
                }

                char tl, tt, tr, mm, bl, br;

                if (useDoubleLines)
                {
                    tl = '+'; tt = '-'; tr = '+'; mm = '¦'; bl = '+'; br = '+';
                }
                else
                {
                    tl = '+'; tt = '-'; tr = '+'; mm = '¦'; bl = '+'; br = '+';
                }

                for (int i = 0; i < yLocation; i++)
                {
                    Console.WriteLine();
                }

                Console.WriteLine(
                    string.Empty.PadLeft(xLocation, ' ')
                    + tl
                    + string.Empty.PadLeft(width - 1, tt)
                    + tr);

                for (int i = 0; i < height; i++)
                {
                    Console.WriteLine(
                        string.Empty.PadLeft(xLocation, ' ')
                        + mm
                        + string.Empty.PadLeft(width - 1, ' ')
                        + mm);
                }

                Console.WriteLine(
                    string.Empty.PadLeft(xLocation, ' ')
                    + bl
                    + string.Empty.PadLeft(width - 1, tt)
                    + br);


                if (color.HasValue)
                {
                    Console.ForegroundColor = savedColor;
                }

                if (keepOriginalCursorLocation)
                {
                    Console.SetCursorPosition(savedCursorLeft, savedCursorTop);
                }
            }
        }

        /// <summary>
        /// Draws a rectangle in a console window using the top line of the buffer as the offset.
        /// </summary>
        /// <param name="xLocation">The left side position.</param>
        /// <param name="yLocation">The top position.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The right of the rectangle.</param>
        /// <param name="color">The color to use. null=uses current color Default: null</param>
        public static void RectangleFromTop(
            int width,
            int height,
            int xLocation = 0,
            int yLocation = 0,
            ConsoleColor? color = null,
            bool useDoubleLines = false)
        {
            Rectangle(width, height, xLocation, yLocation, DrawKind.FromTop, color, useDoubleLines);
        }

        /// <summary>
        /// Specifies if the draw location should be based on the current cursor location or the
        /// top of the window.
        /// </summary>
        public enum DrawKind
        {
            BelowCursor,
            BelowCursorButKeepCursorLocation,
            FromTop,
        }

        /// <summary>
        /// Draws a rectangle in the console window.
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The right of the rectangle.</param>
        /// <param name="xLocation">The left side position.</param>
        /// <param name="yLocation">The top position.</param>
        /// <param name="drawKind">Where to draw the rectangle and 
        /// where to leave the cursor when finished.</param>
        /// <param name="color">The color to use. null=uses current color Default: null</param>
        /// <param name="useDoubleLines">Enables double line boarders. Default: false</param>
        public static void Rectangle(
            int width,
            int height,
            int xLocation = 0,
            int yLocation = 0,
            DrawKind drawKind = DrawKind.FromTop,
            ConsoleColor? color = null,
            bool useDoubleLines = false)
        {
            // if the size is smaller then 1 than don't do anything
            if (width < 1 || height < 1)
            {
                return;
            }

            // Save original cursor location
            int savedCursorTop = Console.CursorTop;
            int savedCursorLeft = Console.CursorLeft;

            if (drawKind == DrawKind.BelowCursor || drawKind == DrawKind.BelowCursorButKeepCursorLocation)
            {
                yLocation += Console.CursorTop;
            }

            // Save and then set cursor color
            ConsoleColor savedColor = Console.ForegroundColor;
            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }

            char tl, tt, tr, mm, bl, br;

            if (useDoubleLines)
            {
                tl = '+'; tt = '-'; tr = '+'; mm = '¦'; bl = '+'; br = '+';
            }
            else
            {
                tl = '+'; tt = '-'; tr = '+'; mm = '¦'; bl = '+'; br = '+';
            }

            SafeDraw(xLocation, yLocation, tl);
            for (int x = xLocation + 1; x < xLocation + width; x++)
            {
                SafeDraw(x, yLocation, tt);
            }
            SafeDraw(xLocation + width, yLocation, tr);

            for (int y = yLocation + height; y > yLocation; y--)
            {
                SafeDraw(xLocation, y, mm);
                SafeDraw(xLocation + width, y, mm);
            }

            SafeDraw(xLocation, yLocation + height + 1, bl);
            for (int x = xLocation + 1; x < xLocation + width; x++)
            {
                SafeDraw(x, yLocation + height + 1, tt);
            }
            SafeDraw(xLocation + width, yLocation + height + 1, br);

            // Restore cursor
            if (drawKind != DrawKind.BelowCursor)
            {
                Console.SetCursorPosition(savedCursorLeft, savedCursorTop);
            }

            if (color.HasValue)
            {
                Console.ForegroundColor = savedColor;
            }
        }

        private static void SafeDraw(int xLocation, int yLocation, char ch)
        {
            if (xLocation < Console.BufferWidth && yLocation < Console.BufferHeight)
            {
                Console.SetCursorPosition(xLocation, yLocation);
                Console.Write(ch);
            }
        }
    }
}
