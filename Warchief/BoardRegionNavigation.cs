using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsPoint = System.Windows.Point;

namespace Warchief
{
    /* Board regions and left/right navigation within */
    interface BoardRegionNavigation
    {
        // navigate onto the region
        WindowsPoint SwitchTo();

        // Navigate to the left or to the right
        WindowsPoint Navigate(bool toTheRight);

        // optionally, bubble a new CommandModule
        CommandModule Select(CommandModule current);
        CommandModule Unselect(CommandModule current);
    }
}
