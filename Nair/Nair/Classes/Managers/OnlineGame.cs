using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using UnityGGPO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Threading;

namespace Nair.Classes.Managers
{

    public class OnlineGame
    {

        private IntPtr ggpo;
        //private GGPOSessionCallbacks callbacks;

        private int errorCode;

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

        }

        public void Initialize()
        {

            short port = 0;
            short opponentport = 0;
            string p2ip = "";
            string p1ip = "";
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

            if (p1Local)
            {
                ggpoP1.type = GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL;
                ggpoP1.ip_address = "";
                ggpoP1.port = 0;
                ggpoP1.player_num = 1;
            }
            else
            {
                ggpoP1.type = GGPOPlayerType.GGPO_PLAYERTYPE_REMOTE;
                ggpoP1.ip_address = p1ip;
                ggpoP1.port = (ushort)opponentport;
                ggpoP1.player_num = 1;
            }

            if (p2Local)
            {
                ggpoP2.type = GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL;
                ggpoP2.ip_address = "";
                ggpoP2.port = 0;
                ggpoP2.player_num = 2;
            }
            else
            {
                ggpoP2.type = GGPOPlayerType.GGPO_PLAYERTYPE_REMOTE;
                ggpoP2.ip_address = p2ip;
                ggpoP2.port = (ushort)opponentport;
                ggpoP2.player_num = 2;
            }

            errorCode = GGPO.StartSession(out ggpo,
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
            );

            // automatically disconnect clients after 3000 ms and start our count-down timer
            // for disconnects after 1000 ms.   To completely disable disconnects, simply use
            // a value of 0 for ggpo_set_disconnect_timeout.
            GGPO.SetDisconnectTimeout(ggpo, 0);
            GGPO.SetDisconnectNotifyStart(ggpo, 0);

            errorCode = GGPO.AddPlayer(ggpo, (int)ggpoP1.type, ggpoP1.player_num, ggpoP1.ip_address, ggpoP1.port, out handleP1);
            errorCode = GGPO.AddPlayer(ggpo, (int)ggpoP2.type, ggpoP2.player_num, ggpoP2.ip_address, ggpoP2.port, out handleP2);

            GGPO.SetFrameDelay(ggpo, handleP1, 2);
            GGPO.SetFrameDelay(ggpo, handleP2, 2);

        }

        public void Close()
        {
            errorCode = GGPO.CloseSession(ggpo);
            ControllerManager.ClearGGPO();
        }


        public void Update(GameTime gameTime)
        {
            //Call Idle with some time, to allow ggpo to do its thing
            GGPO.Idle(ggpo, 6);
            Thread.Sleep(6);

            //Update local player inputs
            ControllerManager.Update(true);

            if (ggpoP1.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL)
                errorCode = GGPO.AddLocalInput(ggpo, handleP1, ControllerManager.RequestGGPOLocalInput(false));
            else if(ggpoP2.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL)
                errorCode = GGPO.AddLocalInput(ggpo, handleP2, ControllerManager.RequestGGPOLocalInput(false));

            //Synchronize inputs with GGPO
            if (GGPO.SUCCEEDED(errorCode))
            {
                long[] inputs;
                inputs = GGPO.SynchronizeInput(ggpo, sizeof(uint) * 2, out errorCode);

                if (GGPO.SUCCEEDED(errorCode))
                {
                    //Update frame
                    PlayerManagerRollback.Instance.Update();
                    //Call AdvanceFrame to notify GGPO, it doesn't actually Update()
                    GGPO.AdvanceFrame(ggpo);
                }
            }
            else
            {
                int queuelen;
                int recqueue;
                int ping;
                int kbpssent;
                int localbehind;
                int remotebehind;
                GGPO.GetNetworkStats(ggpo, 2, out queuelen, out recqueue, out ping, out kbpssent, out localbehind, out remotebehind);
            }

            AnimationManager.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerManagerRollback.Instance.Draw(spriteBatch);
        }

        
        public bool AdvanceFrame(int flags)
        {
            Debug.WriteLine("Advance Frame");
            return true;
        }

        public bool BeginGame(string text)
        {
            Debug.WriteLine("Begin Game");

            PlayerManagerRollback.Instance.ResetGame(true,
                ggpoP1.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL? 4 : 5,
                ggpoP2.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL? 4 : 5);

            return true;
        }

        public unsafe bool LoadGameState(void* data, int length)
        {
            Debug.WriteLine("Load Game State");
            return true;
        }

        public unsafe bool LogGameState(string filename, void* buffer, int length)
        {
            Debug.WriteLine("Log Game State");
            return true;
        }

        public unsafe bool SaveGameState(void** buffer, int* len, int* checksum, int frame)
        {
            Debug.WriteLine("Save Game State");
            return true;
        }

        public unsafe void FreeBuffer(void* buffer)
        {
            Debug.WriteLine("Free Buffer");
            return;
        }

        public unsafe bool OnEvent(IntPtr evt)
        {
            Debug.WriteLine("On Event");

            int[] data = new int[4];
            Marshal.Copy(evt, data, 0, 4);
            switch (data[0])
            {
                case GGPO.EVENTCODE_CONNECTED_TO_PEER:
                    Debug.WriteLine("CONNECTED_TO_PEER");
                    return true;
                    //return onEventConnectedToPeer(data[1]);

                case GGPO.EVENTCODE_SYNCHRONIZING_WITH_PEER:
                    Debug.WriteLine("SYNCHRONIZING_WITH_PEER");
                    return true;
                    //return onEventSynchronizingWithPeer(data[1], data[2], data[3]);

                case GGPO.EVENTCODE_SYNCHRONIZED_WITH_PEER:
                    Debug.WriteLine("SYNCHRONIZED_WITH_PEER");
                    return true;
                    //return onEventSynchronizedWithPeer(data[1]);

                case GGPO.EVENTCODE_RUNNING:
                    Debug.WriteLine("RUNNING");
                    return true;
                    //return onEventRunning();

                case GGPO.EVENTCODE_DISCONNECTED_FROM_PEER:
                    Debug.WriteLine("DISCONNECTED_FROM_PEER");
                    return true;
                    //return onEventDisconnectedFromPeer(data[1]);

                case GGPO.EVENTCODE_TIMESYNC:
                    Debug.WriteLine("TIMESYNC");
                    return true;
                    //return onEventTimesync(data[1]);

                case GGPO.EVENTCODE_CONNECTION_INTERRUPTED:
                    Debug.WriteLine("CONNECTION_INTERRUPTED");
                    return true;
                    //return onEventConnectionInterrupted(data[1], data[2]);

                case GGPO.EVENTCODE_CONNECTION_RESUMED:
                    Debug.WriteLine("CONNECTION_RESUMED");
                    return true;
                    //return onEventConnectionResumed(data[1]);
            }
            return false;
        }

    }
}
