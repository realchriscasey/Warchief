using System;
using System.Windows;
using WindowsPoint = System.Windows.Point;


namespace Warchief
{
    internal class HeroNavigator : BoardRegionNavigation
    {
        private static WindowsPoint playerHeroLocation = new WindowsPoint(0, -60);
        private static WindowsPoint playerHeroPowerLocation = new WindowsPoint(33, -60);

        private bool targetIsHero;

        public WindowsPoint SwitchTo()
        {
            targetIsHero = true;
            return playerHeroLocation;
        }

        public WindowsPoint Navigate(bool toTheRight)
        {
            targetIsHero = !toTheRight;
            return (targetIsHero ? playerHeroLocation : playerHeroPowerLocation);
        }

        public CommandModule Select(CommandModule current)
        {
            return null;
        }

        public CommandModule Unselect(CommandModule current)
        {
            if (targetIsHero)
            {
                return new EmoteCommand(current);
            }
            return null;
        }
    }
}