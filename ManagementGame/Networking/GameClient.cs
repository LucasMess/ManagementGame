using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Networking
{
    class GameClient
    {
        Socket socket;

        public void Connect()
        {
            socket = IO.Socket("http://localhost:3000");
            socket.On(Socket.EVENT_CONNECT, OnConnect);

            //socket.On("session", (data) => OnSessionUpdate(((JObject)data).ToObject<Session>()));
            On<Session>("session", OnSessionUpdate);
            socket.On(Socket.EVENT_ERROR, () => Console.WriteLine("Error!"));

        }

        private void OnConnect()
        {
            Console.WriteLine("Connected!!");
        }

        public void CreateGame()
        {
            Console.WriteLine("Creating game...");
            socket.Emit("createSession");
        }

        public void JoinGame()
        {
            Console.WriteLine("Joining game...");
            socket.Emit("joinSession");
        }

        public void On<T>(string eventName, Action<T> callback)
        {
            socket.On(eventName, (data) => {
                var jsonObject = (JObject)data;
                T typedObject = jsonObject.ToObject<T>();
                callback.Invoke(typedObject);
            });
        }

        private void OnSessionUpdate(Session session)
        {
            Console.WriteLine("Session update");
            Console.WriteLine(session.OwnerId);
        }
    }
}
