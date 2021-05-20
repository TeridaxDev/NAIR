using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using UnityGGPO;
//using GGPOSharp;
using System.Diagnostics;

namespace Nair.Classes.Managers
{

    class OnlineGame
    {

        private IntPtr ggpo;
        //private GGPOSessionCallbacks callbacks;

        private string errorCode;

        private GGPOPlayer ggpoP1, ggpoP2;
        private int handleP1, handleP2;

        private static IntPtr _beginGameCallback;
        private static IntPtr _advanceFrameCallback;
        private static IntPtr _loadGameStateCallback;
        private static IntPtr _logGameStateCallback;
        private static IntPtr _saveGameStateCallback;
        private static IntPtr _freeBufferCallback;
        private static IntPtr _onEventCallback;

        public OnlineGame()
        {

            unsafe
            {
                _beginGameCallback = Marshal.GetFunctionPointerForDelegate<GGPO.BeginGameDelegate>(BeginGame);
                _advanceFrameCallback = Marshal.GetFunctionPointerForDelegate<GGPO.AdvanceFrameDelegate>(AdvanceFrame);
                _loadGameStateCallback = Marshal.GetFunctionPointerForDelegate<GGPO.LoadGameStateDelegate>(LoadGameState);
                _logGameStateCallback = Marshal.GetFunctionPointerForDelegate<GGPO.LogGameStateDelegate>(LogGameState);
                _saveGameStateCallback = Marshal.GetFunctionPointerForDelegate<GGPO.SaveGameStateDelegate>(SaveGameState);
                _freeBufferCallback = Marshal.GetFunctionPointerForDelegate<GGPO.FreeBufferDelegate>(FreeBuffer);
                _onEventCallback = Marshal.GetFunctionPointerForDelegate<GGPO.OnEventDelegate>(OnEvent);
            }

            //callbacks.BeginGame = Marshal.GetFunctionPointerForDelegate(new BeginGame(BeginGame));
            //callbacks.SaveGameState = Marshal.GetFunctionPointerForDelegate(new SaveGameState(SaveGameState));
            //callbacks.LoadGameState = Marshal.GetFunctionPointerForDelegate(new LoadGameState(LoadGameState));
            //callbacks.LogGameState = Marshal.GetFunctionPointerForDelegate(new LogGameState(LogGameState));
            //callbacks.FreeBuffer = Marshal.GetFunctionPointerForDelegate(new FreeBuffer(FreeBuffer));
            //callbacks.AdvanceFrame = Marshal.GetFunctionPointerForDelegate(new AdvanceFrame(AdvanceFrame));
            //callbacks.OnEvent = Marshal.GetFunctionPointerForDelegate(new OnEvent(OnEvent));

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


            errorCode = GGPO.GetErrorCodeMessage(GGPO.StartSession(out ggpo,
                _beginGameCallback,
                _advanceFrameCallback,
                _loadGameStateCallback,
                _logGameStateCallback,
                _saveGameStateCallback,
                _freeBufferCallback,
                _onEventCallback,
                "NAIR",
                2,
                port
            ));

            // automatically disconnect clients after 3000 ms and start our count-down timer
            // for disconnects after 1000 ms.   To completely disable disconnects, simply use
            // a value of 0 for ggpo_set_disconnect_timeout.
            GGPO.SetDisconnectTimeout(ggpo, 3000);
            GGPO.SetDisconnectNotifyStart(ggpo, 1000);



            if (p1Local)
                errorCode = GGPO.GetErrorCodeMessage(GGPO.AddPlayer(ggpo, (int)GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL, 1, "", 0, out handleP1));
            else
                errorCode = GGPO.GetErrorCodeMessage(GGPO.AddPlayer(ggpo, (int)GGPOPlayerType.GGPO_PLAYERTYPE_REMOTE, 1, p1ip, (ushort)opponentport, out handleP1));

            if (p2Local)
                errorCode = GGPO.GetErrorCodeMessage(GGPO.AddPlayer(ggpo, (int)GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL, 2, "", 0, out handleP2));
            else
                errorCode = GGPO.GetErrorCodeMessage(GGPO.AddPlayer(ggpo, (int)GGPOPlayerType.GGPO_PLAYERTYPE_REMOTE, 2, p2ip, (ushort)opponentport, out handleP2));






        }

        public void Close()
        {
            errorCode = GGPO.GetErrorCodeMessage(GGPO.CloseSession(ggpo));
        }

        public void Draw()
        {

        }

        
        public bool AdvanceFrame(int flags)
        {

            return true;
        }

        public bool BeginGame(string text)
        {

            return true;
        }

        public unsafe bool LoadGameState(void* data, int length)
        {

            return true;
        }

        public unsafe bool LogGameState(string filename, void* buffer, int length)
        {

            return true;
        }

        public unsafe bool SaveGameState(void** buffer, int* len, int* checksum, int frame)
        {

            return true;
        }

        public unsafe void FreeBuffer(void* buffer)
        {

            return;
        }

        public unsafe bool OnEvent(IntPtr evt)
        {

            return true;
        }

    }
}
