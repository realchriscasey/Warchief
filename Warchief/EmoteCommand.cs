using Hearthstone_Deck_Tracker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DrawingPoint = System.Drawing.Point;
using WindowsPoint = System.Windows.Point;

namespace Warchief
{
    internal class EmoteCommand : CommandModule
    {
        private CommandModule parent;

        public EmoteCommand(CommandModule parent)
        {
            this.parent = parent;
        }
        private int emoteIndex = 1;
        private bool emotesOnRight = false;

        private List<WindowsPoint> leftEmotes = 
            new List<WindowsPoint> {
                new WindowsPoint(-11, -36),
                new WindowsPoint(-20, -52),
                new WindowsPoint(-19, -69)
            };

        private List<WindowsPoint> rightEmotes =
            new List<WindowsPoint> {
                new WindowsPoint(45, -36),
                new WindowsPoint(53, -52),
                new WindowsPoint(52, -69)
            };

        public CommandModule Command(InputCommand input)
        {
            switch (input)
            {
                case InputCommand.Up:
                    navigate(emoteIndex - 1);
                    break;
                case InputCommand.Down:
                    navigate(emoteIndex + 1);
                    break;
                case InputCommand.Left:
                    emotesOnRight = false;
                    break;
                case InputCommand.Right:
                    emotesOnRight = true;                    
                    break;
                case InputCommand.Select:
                    click();
                    return parent;
                    break;
                case InputCommand.Unselect:
                    rightClick();
                    return parent;
                    break;
            }

            updatePosition();
            return this;
        }

        private void navigate(int newIndex)
        {
            if (newIndex < 0 || newIndex >= 3)
            {
                return;
            }
            emoteIndex = newIndex;
        }

        private void updatePosition()
        {
            List<WindowsPoint> currentEmotes = emotesOnRight ? rightEmotes : leftEmotes;

            Cursor.Position = getAbsolutePos(currentEmotes[emoteIndex]);
        }

        //TODO move all cursor navigation to helper library

        /* `ALGALON` coordinate system for hearthstone */

        /* define the center of board as (0,0). */
        /* define one unit as one percent of the distance from the center of the board to the top of the screen */
        /* (precision is approximately three digits) */

        /* top of screen is (0,100) */
        /* left side of board is (-133,0).  the board is roughly 4:3 shape. */
        /* in 16:9 displays, the padding around the board extends to (177,0) */

        //TODO: replace most references to `WindowsPoint` with `AlgalonPoint`

        static double GLOBAL_SCALE = 100.0;
        static double EMPIRICAL_X_OFFSET = 0;
        static double EMPIRICAL_Y_OFFSET = -0.07 * GLOBAL_SCALE;

        //get an algalon coordinate from a game client-relative coordinate
        private WindowsPoint getLocalPos(WindowsPoint clientPos)
        {
            Rectangle wrecked = User32.GetHearthstoneRect(false);
            double yCenter = wrecked.Height / 2;
            double xCenter = wrecked.Width / 2;
            double scale = GLOBAL_SCALE / yCenter;

            double xOffset = EMPIRICAL_X_OFFSET;
            double yOffset = EMPIRICAL_Y_OFFSET;

            WindowsPoint local = new WindowsPoint(
                (clientPos.X - xCenter) * scale + xOffset,
                (clientPos.Y - yCenter) * scale * -1 + yOffset);  //invert y axis (up should be positive, dammit)

            return local;
        }

        //get a client-relative coordinate from an algalon coordinate
        private DrawingPoint getWindowPos(WindowsPoint boardPos)
        {
            Rectangle wrecked = User32.GetHearthstoneRect(false);
            double yCenter = wrecked.Height / 2;
            double xCenter = wrecked.Width / 2;
            double scale = GLOBAL_SCALE / yCenter;

            double xOffset = EMPIRICAL_X_OFFSET;
            double yOffset = EMPIRICAL_Y_OFFSET;

            DrawingPoint windowPos = new DrawingPoint(
                (int)((boardPos.X - xOffset) / scale + xCenter),
                (int)((boardPos.Y - yOffset) * -1 / scale + yCenter)
                );

            return windowPos;
        }

        //get a windows absolute coordinate from an algalon coordinate
        private DrawingPoint getAbsolutePos(WindowsPoint boardPos)
        {
            DrawingPoint position = getWindowPos(boardPos);
            User32.ClientToScreen(User32.GetHearthstoneWindow(), ref position);
            return position;
        }

        /*
        private WindowsPoint getCanvasPos(DrawingPoint absolutePos)
        {
            Rectangle wrecked = User32.GetHearthstoneRect(false);

            double y = absolutePos.Y - wrecked.Top;
            double x = absolutePos.X - wrecked.Left;

            return new WindowsPoint(x, y);
        }
        */

        private const int CLICK_SLEEP_TIME_MS = 5;

        private void click()
        {
            User32.mouse_event((uint)User32.MouseEventFlags.LeftDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(CLICK_SLEEP_TIME_MS);
            User32.mouse_event((uint)User32.MouseEventFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        private void rightClick()
        {
            User32.mouse_event((uint)User32.MouseEventFlags.RightDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(CLICK_SLEEP_TIME_MS);
            User32.mouse_event((uint)User32.MouseEventFlags.RightUp, 0, 0, 0, UIntPtr.Zero);
        }

        public void SwitchTo()
        {
            return;
        }
    }
}