using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CoreAPI = Hearthstone_Deck_Tracker.API.Core;
using WindowsPoint = System.Windows.Point;

namespace Warchief
{
    class MinionNavigator : BoardRegionNavigation
    {
        private WindowsPoint minionRowCenter;
        private bool isPlayer;

        private int minionIndex;
        private bool isEndTurn;

        private static WindowsPoint opponentMinionRowLocation = new WindowsPoint(0, 20);
        private static WindowsPoint playerMinionRowLocation = new WindowsPoint(0, -20);
        private static WindowsPoint endTurnLocation = new WindowsPoint(112, 0);

        private static double MINION_OFFSET = 26.0;

        public MinionNavigator(bool isPlayer)
        {
            this.isPlayer = isPlayer;
            this.minionRowCenter = isPlayer? playerMinionRowLocation : opponentMinionRowLocation;
        }

        public WindowsPoint SwitchTo()
        {
            minionIndex = getMinionCount() / 2;
            return minionLocation(minionIndex);
        }

        public WindowsPoint Navigate(bool toTheRight)
        {
            int minionCount = getMinionCount();

            minionIndex += 1 * (toTheRight ? 1 : -1);
            if (minionIndex < 0)
            {
                minionIndex = 0;
            } else if (minionCount > 0 && minionIndex >= minionCount) {
                minionIndex = minionCount;
            } else if (minionCount == 0 && minionIndex > 1) {
                minionIndex = 1;
            }
            return minionLocation(minionIndex);
        }

        private WindowsPoint minionLocation(int minionIndex)
        {
            int minionCount = getMinionCount();

            if (getMinionCount() == 0)
            {
                return minionIndex > 0 ? endTurnLocation : minionRowCenter;
            }

            if (minionIndex >= getMinionCount())
            {
                return endTurnLocation;
            }

            double minion0Location = -1 * (minionCount-1) * MINION_OFFSET / 2;
            return new WindowsPoint(minionRowCenter.X + minion0Location + minionIndex * MINION_OFFSET,
                minionRowCenter.Y);
        }

        private int getMinionCount()
        {
            return (isPlayer ? CoreAPI.Game.PlayerMinionCount : CoreAPI.Game.OpponentMinionCount);
        }
    }
}