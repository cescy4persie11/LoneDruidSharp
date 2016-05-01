using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common;

namespace LoneDruidSharpRewrite
{
    class Bootstrap
    {
        private readonly LoneDruidSharp lonedruidsharp;

        public Bootstrap()
        {
            this.lonedruidsharp = new LoneDruidSharp();
        }

        public void SubscribeEvents()
        {
            Events.OnLoad += this.Events_Onload;
            Events.OnClose += this.Events_OnClose;
            Game.OnUpdate += this.Game_OnUpdate;
            //Game.OnWndProc += this.Game_OnWndProc;
            Drawing.OnDraw += this.Drawing_OnDraw;
            Player.OnExecuteOrder += this.Player_OnExecuteOrder;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            this.lonedruidsharp.OnDraw();
        }

        private void Events_Onload(object sender, EventArgs e)
        {
            this.lonedruidsharp.OnLoad();
        }

        private void Events_OnClose(object sender, EventArgs e)
        {
            this.lonedruidsharp.OnClose();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            this.lonedruidsharp.OnUpdate_IronTalon();
            this.lonedruidsharp.OnUpdate_AutoMidas();
            this.lonedruidsharp.OnUpdate_LastHit();
            this.lonedruidsharp.OnUpdate_bearChase();
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            this.lonedruidsharp.OnWndProc(args);
        }

        private void Player_OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (sender.Equals(ObjectManager.LocalPlayer))
            {
                this.lonedruidsharp.Player_OnExecuteOrder(args);
            }
        }


    }
}
