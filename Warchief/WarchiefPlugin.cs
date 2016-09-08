using System;
using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.API;
using System.Windows;
using Hearthstone_Deck_Tracker;
using CoreAPI = Hearthstone_Deck_Tracker.API.Core;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Forms;
using Cursor = System.Windows.Forms.Cursor;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using Hearthstone_Deck_Tracker.Utility.HotKeys;
using Hearthstone_Deck_Tracker.Exporting;
using System.Threading;

namespace Warchief
{
    public class Warchief : IPlugin
    {
        public string Name => "Warchief";
        public string Description
            =>
                "Lok-regar no'gall! Use your keyboard or gamepad to play Hearthstone.";

        public string ButtonText => "DO NOT PUSH THIS BUTTON!";
        public string Author => "realchriscasey";
        public Version Version => new Version(0, 0, 9);
        public System.Windows.Controls.MenuItem MenuItem => null;

        void IPlugin.OnButtonPress()
        {
            /*NOP*/
        }

        void IPlugin.OnLoad()
        {
            if (keyCommands.Count == 0)
            {
                // TODO it's probably possible to use delegates instead of simple function refs here
                keyCommands.Add(new HotKey(Keys.Up), this.Up);
                keyCommands.Add(new HotKey(Keys.Down), this.Down);
                keyCommands.Add(new HotKey(Keys.Left), this.Left);
                keyCommands.Add(new HotKey(Keys.Right), this.Right);

                keyCommands.Add(new HotKey(Keys.Z), this.Select);
                keyCommands.Add(new HotKey(Keys.X), this.Unselect);
            }
        }

        void IPlugin.OnUnload()
        {
            /*NOP*/
        }

        private bool areHotkeysRegistered = false;
        private Dictionary<HotKey, Action> keyCommands = new Dictionary<HotKey, Action>();
        private CommandModule currentModule = new TargetingDummy();

        void IPlugin.OnUpdate()
        {
            if (!this.areHotkeysRegistered && User32.IsHearthstoneInForeground())
            {
                foreach(KeyValuePair<HotKey, Action> keyCommand in this.keyCommands)
                {
                    HotKeyManager.RegisterHotkey(keyCommand.Key, keyCommand.Value, keyCommand.Key.Key.ToString());
                }

                this.areHotkeysRegistered = true;
            }
            else if (this.areHotkeysRegistered && !User32.IsHearthstoneInForeground())
            {
                //hotkeys are registered, but hearthstone isn't active. unregister them.
                
                foreach (KeyValuePair<HotKey, Action> keyCommand in this.keyCommands)
                {
                    HotKeyManager.RemovePredefinedHotkey(keyCommand.Key);
                }

                this.areHotkeysRegistered = false;
            }
        }

        private void SendCommand(InputCommand input)
        {
            CommandModule result = this.currentModule.Command(input);

            if (result != null)
            {
                this.currentModule = result;
            }
        }

        private void Up() { SendCommand(InputCommand.Up); }
        private void Down() { SendCommand(InputCommand.Down); }
        private void Left() { SendCommand(InputCommand.Left); }
        private void Right() { SendCommand(InputCommand.Right); }
        private void Select() { SendCommand(InputCommand.Select); }
        private void Unselect() { SendCommand(InputCommand.Unselect); }
    }
}