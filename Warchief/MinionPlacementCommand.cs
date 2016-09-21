using System;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using CoreAPI = Hearthstone_Deck_Tracker.API.Core;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DrawingPoint = System.Drawing.Point;
using WindowsPoint = System.Windows.Point;
using Hearthstone_Deck_Tracker;

namespace Warchief
{
    internal class MinionPlacementCommand : CommandModule
    {
        private Entity minionEntity;
        private CommandModule parent;

        int minionIndex = 0;

        private static WindowsPoint playerMinionRowLocation = new WindowsPoint(0, -20);

        public CommandModule init(Entity card, CommandModule parent)
        {
            this.minionEntity = card;
            this.parent = parent;
            minionIndex = getMinionCount() / 2;

            return this;
        }

        public void SwitchTo()
        {
            Cursor.Position = getAbsolutePos(minionLocation(minionIndex));
        }

        public CommandModule Command(InputCommand input)
        {
            switch (input)
            {
                case InputCommand.Up:
                case InputCommand.Down:
                    break;
                case InputCommand.Left:
                case InputCommand.Right:
                    Navigate(input == InputCommand.Right);
                    break;
                case InputCommand.Select:
                    //TODO kiil urself
                    MinionNavigator.HUGE_HACK = getMinionCount();

                    click();
                    if (parent is TargetingDummy)
                    {
                        ((TargetingDummy)parent).setRegion(2);
                    }
                    return parent;
                case InputCommand.Unselect:
                    rightClick();

                    if (parent is TargetingDummy)
                    {
                        ((TargetingDummy)parent).setRegion(4);
                    }
                    return parent;
            }
            return this;
        }

        public void Navigate(bool toTheRight)
        {
            int minionCount = getMinionCount();

            minionIndex += (toTheRight ? 1 : -1);
            if (minionIndex < 0)
            {
                minionIndex = 0;
            }
            else if (minionIndex >= minionCount)
            {
                minionIndex = minionCount - 1;
            }

            WindowsPoint location = minionLocation(minionIndex);
            Cursor.Position = getAbsolutePos(location);

            return;
        }

        private static double MINION_OFFSET = 26.0;

        private WindowsPoint minionLocation(int minionIndex)
        {
            int minionCount = getMinionCount();

            double minion0Location = -1 * (minionCount - 1) * MINION_OFFSET / 2;
            return new WindowsPoint(playerMinionRowLocation.X + minion0Location + minionIndex * MINION_OFFSET,
                playerMinionRowLocation.Y);
        }

        private int getMinionCount()
        {
            return CoreAPI.Game.PlayerMinionCount + 1;
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

        private const int CLICK_SLEEP_TIME_MS = 30;
        private const int CLICK_SLEEP_TIME_AFTER_MS = 30;

        private void click()
        {
            User32.mouse_event((uint)User32.MouseEventFlags.LeftDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(CLICK_SLEEP_TIME_MS);
            User32.mouse_event((uint)User32.MouseEventFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(CLICK_SLEEP_TIME_AFTER_MS);
        }

        private void rightClick()
        {
            User32.mouse_event((uint)User32.MouseEventFlags.RightDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(CLICK_SLEEP_TIME_MS);
            User32.mouse_event((uint)User32.MouseEventFlags.RightUp, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(CLICK_SLEEP_TIME_AFTER_MS);
        }
    }
}