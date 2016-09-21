using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using System;
using System.Collections.Generic;
using System.Windows;
using CoreAPI = Hearthstone_Deck_Tracker.API.Core;
using WindowsPoint = System.Windows.Point;

namespace Warchief
{
    internal class HandNavigator : BoardRegionNavigation
    {
        private static WindowsPoint handCenterDefault = new WindowsPoint(0, -90);
        private static List<List<WindowsPoint>> handLocations;
        MinionPlacementCommand minionPlacement = new MinionPlacementCommand();

        private int handIndex;

        public HandNavigator()
        {
            //TODO try using maths
            //TODO readjust all to (x,100) (?)
            handLocations = new List<List<WindowsPoint>>(10);

            // 1 card: ([-20,6],-90)
            handLocations.Add(new List<WindowsPoint> {
                new WindowsPoint(-7, -90)
            });


            //2: [-32,-8],[..18]
            handLocations.Add(new List<WindowsPoint> {
                new WindowsPoint(-20, -90),
                new WindowsPoint(5, -90)
            });

            //3: [-45,-20], [..5], [..30]
            handLocations.Add(new List<WindowsPoint> {
                new WindowsPoint(-32, -90),
                new WindowsPoint(-7, -90),
                new WindowsPoint(17, -90)
            });


            //4: ([-60,-33], --90), ([..., -7],...), [..,17], [..45]   => roughly every 30
            handLocations.Add(new List<WindowsPoint> {
                new WindowsPoint(-46, -90),
                new WindowsPoint(-20, -90),
                new WindowsPoint(5, -90),
                new WindowsPoint(31, -90),
            });

            //5: [-65,-42], [..-20], [..0], [..22], [..48]
            handLocations.Add(new List<WindowsPoint> {
                new WindowsPoint(-53, -90),
                new WindowsPoint(-31, -90),
                new WindowsPoint(-10, -90),
                new WindowsPoint(11, -90),
                new WindowsPoint(35, -90),
            });

            //6: [-61, -48], [..-29], [..-11], [..5], [24], 51
            handLocations.Add(new List<WindowsPoint> {
                new WindowsPoint(-54, -90),
                new WindowsPoint(-38, -90),
                new WindowsPoint(-20, -90),
                new WindowsPoint(-3, -90),
                new WindowsPoint(15, -90),
                new WindowsPoint(37, -90),
            });

            //7: [-64, -51], [..-35], [-20], -5, 10, 26, 46
            handLocations.Add(new List<WindowsPoint> {
                new WindowsPoint(-57, -90),
                new WindowsPoint(-43, -90),
                new WindowsPoint(-27, -90),
                new WindowsPoint(-12, -90),
                new WindowsPoint(2, -90),
                new WindowsPoint(18, -90),
                new WindowsPoint(36, -90),
            });

            //8: [-58..-55], -40, -26, -13, -1, 13, 27, 51
            handLocations.Add(new List<WindowsPoint> {
                new WindowsPoint(-57, -90),
                new WindowsPoint(-48, -90),
                new WindowsPoint(-33, -90),
                new WindowsPoint(-20, -90),
                new WindowsPoint(-7, -90),
                new WindowsPoint(6, -90),
                new WindowsPoint(20, -90),
                new WindowsPoint(39, -90),
            });

            //9: [-61..-58], -44, -32, -20, -9, 3, 15, 29, 43
            handLocations.Add(new List<WindowsPoint> {
                new WindowsPoint(-60, -90),
                new WindowsPoint(-52, -90),
                new WindowsPoint(-37, -90),
                new WindowsPoint(-26, -90),
                new WindowsPoint(-15, -90),
                new WindowsPoint(-3, -90),
                new WindowsPoint(9, -90),
                new WindowsPoint(22, -90),
                new WindowsPoint(36, -90),
            });

            //10: ([-69..-57], -100), -45, -34, -24, -15, -5, 4, 14, 25, 53   => NOTE: (x, 100) => ROUGHLY EVERY 10
            handLocations.Add(new List<WindowsPoint> {
                new WindowsPoint(-63, -100),
                new WindowsPoint(-51, -100),
                new WindowsPoint(-39, -100),
                new WindowsPoint(-29, -100),
                new WindowsPoint(-20, -100),
                new WindowsPoint(-10, -100),
                new WindowsPoint(0, -100),
                new WindowsPoint(9, -100),
                new WindowsPoint(20, -100),
                new WindowsPoint(39, -100),
            });
        }

        public WindowsPoint SwitchTo()
        {
            handIndex = getHandSize() / 2;
            return handLocation(handIndex);
        }

        public WindowsPoint Navigate(bool toTheRight)
        {
            handIndex = handIndex + (toTheRight ? 1 : -1);
            if (handIndex < 0)
            {
                handIndex = 0;
            }
            else if (handIndex >= getHandSize())
            {
                handIndex = getHandSize()-1;
            }
            return handLocation(handIndex);
        }

        private Entity currentHover()
        {
            IEnumerable<Entity> handCards = CoreAPI.Game.Player.Hand;

            foreach(Entity card in handCards)
            {
                if (card.GetTag(HearthDb.Enums.GameTag.ZONE_POSITION) == handIndex + 1)
                {
                    return card;
                }
            }
            return null;
        }


        private WindowsPoint handLocation(int handIndex)
        {
            int handSize = getHandSize();
            if (handSize == 0)
            {
                return handCenterDefault;
            }

            return handLocations[handSize - 1][handIndex];
        }

        private int getHandSize()
        {
            return CoreAPI.Game.Player.HandCount;
        }

        public CommandModule Select(CommandModule current)
        {
            //if currently selected card is a minion, switch to minion-placer
            Entity card = currentHover();
            if (card != null && card.IsMinion)
            {
                return minionPlacement.init(card, current);
            }
            return null;
        }

        public CommandModule Unselect(CommandModule current)
        {
            return null;
        }
    }
}