using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warchief
{
    /* swappable module that responds to input commands */
    interface CommandModule
    {
        // execute the specified command
        // returns: a command module to accept the next command (often `this`)
        // returns: null, if the command was not processed

        CommandModule Command(InputCommand input);
        void SwitchTo();
    }
}
