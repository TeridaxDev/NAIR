using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using GGPOSharp;
using System.Diagnostics;

namespace Nair.Classes.Managers
{

    class OnlineGame : IGGPOSession
    {

        private GGPO session;
        private GGPOSessionCallbacks callbacks;
        private GGPO.ErrorCode errorCode;

        private GGPOPlayer ggpoP1, ggpoP2;
        private int handleP1, handleP2;


        public OnlineGame()
        {
            session = new GGPO();

            callbacks.BeginGame = Marshal.GetFunctionPointerForDelegate(new BeginGame(BeginGame));
            callbacks.SaveGameState = Marshal.GetFunctionPointerForDelegate(new SaveGameState(SaveGameState));
            callbacks.LoadGameState = Marshal.GetFunctionPointerForDelegate(new LoadGameState(LoadGameState));
            callbacks.LogGameState = Marshal.GetFunctionPointerForDelegate(new LogGameState(LogGameState));
            callbacks.FreeBuffer = Marshal.GetFunctionPointerForDelegate(new FreeBuffer(FreeBuffer));
            callbacks.AdvanceFrame = Marshal.GetFunctionPointerForDelegate(new AdvanceFrame(AdvanceFrame));
            callbacks.OnEvent = Marshal.GetFunctionPointerForDelegate(new OnEvent(OnEvent));

        }

        public void Update()
        {

        }

        public void Initialize()
        {

            short port = 0;
            short opponentport = 0;
            string p2ip = "0.0.0.0";
            string p1ip = "0.0.0.0";
            bool p1Local = false;
            bool p2Local = false;

            //Read IP data from file
            StreamReader reader = null;
            try
            {
                Stream streamIn = File.OpenRead("GGPOInfo.txt");
                reader = new StreamReader(streamIn);

                //Resolution
                string line = reader.ReadLine();

                port = short.Parse(line);

                line = reader.ReadLine();
                if (line.ToLower() == "local")
                {
                    p1Local = true;
                }
                else
                {
                    p1ip = line.Split(':')[0];
                    opponentport = short.Parse(line.Split(':')[1]);
                }

                line = reader.ReadLine();
                if (line.ToLower() == "local")
                {
                    p2Local = true;
                }
                else
                {
                    p2ip = line.Split(':')[0];
                    opponentport = short.Parse(line.Split(':')[1]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }


            unsafe
            {
                errorCode = session.StartSession(
                    callbacks,
                    "NAIR",
                    2,
                    sizeof(GGPOInputPackage),
                    port);
            }

            // automatically disconnect clients after 3000 ms and start our count-down timer
            // for disconnects after 1000 ms.   To completely disable disconnects, simply use
            // a value of 0 for ggpo_set_disconnect_timeout.
            session.SetDisconnectTimeout(3000);
            session.SetDisconnectNotifyStart(1000);

            if (p1Local)
                ggpoP1 = GGPO.CreateLocalPlayer(1);
            else
                ggpoP1 = GGPO.CreateRemotePlayer(1, p1ip, opponentport);

            if (p2Local)
                ggpoP2 = GGPO.CreateLocalPlayer(2);
            else
                ggpoP2 = GGPO.CreateRemotePlayer(2, p2ip, opponentport);

            errorCode = session.AddPlayer(ref ggpoP1, out handleP1);
            errorCode = session.AddPlayer(ref ggpoP2, out handleP2);



        }

        public void Close()
        {
            errorCode = session.CloseSession();
        }

        public void Draw()
        {

        }

        public bool BeginGame(string game)
        {
            Debug.WriteLine("BeginGame");
            PlayerManagerRollback.Instance.ResetGame(true);
            return true;
        }

        public bool SaveGameState(ref byte[] buffer, ref int len, ref int checksum, int frame)
        {
            Debug.WriteLine("Save");
            return true;
        }

        public bool LoadGameState(byte[] buffer, int len)
        {
            Debug.WriteLine("Load");
            return true;
        }

        public bool LogGameState(string filename, byte[] buffer, int len)
        {
            Debug.WriteLine("Log");
            return true;
        }

        public void FreeBuffer(IntPtr buffer)
        {
            Debug.WriteLine("Free");
            return;
        }

        public bool AdvanceFrame(int flags)
        {
            Debug.WriteLine("Advance");
            return true;
        }

        public bool OnEvent(ref GGPOEvent info)
        {
            Debug.WriteLine("OnEvent");
            return true;
        }
    }
}
