using System;
using System.Windows;
using WindowsPoint = System.Windows.Point;

namespace Warchief
{
    internal class OpponentNavigator : BoardRegionNavigation
    {
        private static WindowsPoint opponentHeroLocation = new WindowsPoint(0, 63);

        public WindowsPoint SwitchTo()
        {
            return opponentHeroLocation;
        }

        public WindowsPoint Navigate(bool toTheRight)
        {
            return opponentHeroLocation;
        }
        public CommandModule Select(CommandModule current)
        {
            return null;
        }

        public CommandModule Unselect(CommandModule current)
        {
            return null;
        }
    }
}