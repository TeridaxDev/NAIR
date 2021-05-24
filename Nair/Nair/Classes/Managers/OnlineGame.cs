using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using GGPOSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Threading;

namespace Nair.Classes.Managers
{

    public class OnlineGame : IGGPOSession
    {

        private GGPO ggpo;
        //private GGPOSessionCallbacks callbacks;

        private GGPO.ErrorCode errorCode;

        private GGPOPlayer ggpoP1, ggpoP2;
        private GGPOSessionCallbacks callbacks;
        private int handleP1, handleP2;
        int port;

        private static IntPtr _beginGameCallback;
        private static IntPtr _advanceFrameCallback;
        private static IntPtr _loadGameStateCallback;
        private static IntPtr _logGameStateCallback;
        private static IntPtr _saveGameStateCallback;
        private static IntPtr _freeBufferCallback;
        private static IntPtr _onEventCallback;

        public OnlineGame()
        {
                ggpo = new GGPO();

                callbacks.BeginGame = Marshal.GetFunctionPointerForDelegate<BeginGame>(BeginGame);
                callbacks.AdvanceFrame = Marshal.GetFunctionPointerForDelegate<AdvanceFrame>(AdvanceFrame);
                callbacks.LoadGameState = Marshal.GetFunctionPointerForDelegate<LoadGameState>(LoadGameState);
                callbacks.LogGameState = Marshal.GetFunctionPointerForDelegate<LogGameState>(LogGameState);
                callbacks.SaveGameState = Marshal.GetFunctionPointerForDelegate<SaveGameState>(SaveGameState);
                callbacks.FreeBuffer = Marshal.GetFunctionPointerForDelegate<FreeBuffer>(FreeBuffer);
                callbacks.OnEvent = Marshal.GetFunctionPointerForDelegate<OnEvent>(OnEvent);
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
                ggpoP1.type = GGPO.PlayerType.GGPO_PLAYERTYPE_LOCAL;
                ggpoP1.ipAddress = "";
                ggpoP1.port = 0;
                ggpoP1.playerNum = 1;
            }
            else
            {
                ggpoP1.type = GGPO.PlayerType.GGPO_PLAYERTYPE_REMOTE;
                ggpoP1.ipAddress = p1ip;
                ggpoP1.port = opponentport;
                ggpoP1.playerNum = 1;
            }

            if (p2Local)
            {
                ggpoP2.type = GGPO.PlayerType.GGPO_PLAYERTYPE_LOCAL;
                ggpoP2.ipAddress = "";
                ggpoP2.port = 0;
                ggpoP2.playerNum = 2;
            }
            else
            {
                ggpoP2.type = GGPO.PlayerType.GGPO_PLAYERTYPE_REMOTE;
                ggpoP2.ipAddress = p2ip;
                ggpoP2.port = opponentport;
                ggpoP2.playerNum = 2;
            }

            errorCode = ggpo.StartSession(callbacks,
                "NAIR",
                2,
                sizeof(int),
                port
            );

            this.port = port;

            // automatically disconnect clients after 3000 ms and start our count-down timer
            // for disconnects after 1000 ms.   To completely disable disconnects, simply use
            // a value of 0 for ggpo_set_disconnect_timeout.
            ggpo.SetDisconnectTimeout(0);
            ggpo.SetDisconnectNotifyStart(0);

            errorCode = ggpo.AddPlayer(ref ggpoP1, out handleP1);
            errorCode = ggpo.AddPlayer(ref ggpoP2, out handleP2);

            if(ggpoP1.type == GGPO.PlayerType.GGPO_PLAYERTYPE_LOCAL)
                ggpo.SetFrameDelay(handleP1, 2);
            if(ggpoP2.type == GGPO.PlayerType.GGPO_PLAYERTYPE_LOCAL)
                ggpo.SetFrameDelay(handleP2, 2);

        }

        public void Close()
        {
            errorCode = ggpo.CloseSession();
            ControllerManager.ClearGGPO();
        }


        public void Update(GameTime gameTime)
        {
            //Call Idle with some time, to allow ggpo to do its thing
            ggpo.Idle(6);
            Thread.Sleep(6);

            //Update local player inputs
            ControllerManager.Update(true);
            uint data = ControllerManager.RequestGGPOLocalInput(false);


            if (ggpoP1.type == GGPO.PlayerType.GGPO_PLAYERTYPE_LOCAL)
                errorCode = ggpo.AddLocalInput(handleP1, (IntPtr)data, sizeof(uint));
            if(ggpoP2.type == GGPO.PlayerType.GGPO_PLAYERTYPE_LOCAL)
                errorCode = ggpo.AddLocalInput(handleP2, (IntPtr)data, sizeof(uint));

            //Synchronize inputs with GGPO
            if (errorCode == GGPO.ErrorCode.GGPO_ERRORCODE_SUCCESS)
            {
                IntPtr inputs = new IntPtr();
                int flags;
                errorCode = ggpo.SynchronizeInput(inputs, sizeof(uint) * 2, out flags);

                if (errorCode == GGPO.ErrorCode.GGPO_ERRORCODE_SUCCESS)
                {
                    //Update frame
                    PlayerManagerRollback.Instance.Update();
                    //Call AdvanceFrame to notify GGPO, it doesn't actually Update()
                    ggpo.AdvanceFrame();
                }
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
                ggpoP1.type == GGPO.PlayerType.GGPO_PLAYERTYPE_LOCAL? 4 : 5,
                ggpoP2.type == GGPO.PlayerType.GGPO_PLAYERTYPE_LOCAL? 4 : 5);

            return true;
        }

        public bool LoadGameState(byte[] buffer, int len)
        {
            Debug.WriteLine("Load Game State");
            return true;
        }

        public bool LogGameState(string filename, byte[] buffer, int len)
        {
            Debug.WriteLine("Log Game State");
            return true;
        }

        public bool SaveGameState(ref byte[] buffer, ref int len, ref int checksum, int frame)
        {
            Debug.WriteLine("Save Game State");
            return true;
        }

        public void FreeBuffer(IntPtr buffer)
        {
            Debug.WriteLine("Free Buffer");
            return;
        }

        public bool OnEvent(ref GGPOEvent info)
        {
            Debug.WriteLine("On Event");

            
            return true;
        }

    }
}
