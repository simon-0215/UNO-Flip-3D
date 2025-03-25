using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetworkGame.TCPServer
{
    public class UNOCardRoom
    {
        public UNOCardRoom(ClientState c1, ClientState c2)
        {
            client1 = c1;
            client2 = c2;

            c1.room = this;
            c2.room = this;
        }

        public ClientState client1;
        public ClientState client2;
    }
}
