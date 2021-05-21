using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

enum Actions
{
    start,
    attack,
    dodge,
    jump,
    hardLeft,
    hardRight,
    hardDown,
    softLeft,
    softRight,
    horizontalNeutral,
    leftBumper,
    rightBumper,
    hardUp,
    softUp,
    softDown,
    holdSoft
}

namespace Nair.Classes.Managers
{

    public struct GGPOInputPackage //size of needs to fit in a uint
    {
        //32 bits


        public bool softLeft;
        public bool hardLeft;
        public bool softRight;
        public bool hardRight;
        public bool softUp;
        public bool hardUp;
        public bool softDown;
        public bool hardDown;

        public bool attack;   //1 bit
        public bool dodge;    //1 bit
        public bool jump;     //1 bit
        public bool start;    //1 bit
    }

    class ControllerManager
    {

        private static ControllerManager instance;
        private List<GamePadState> gamePads;
        private List<GamePadState> oldGamePads;
        private KeyboardState keyBoardState;
        private KeyboardState oldKeyboardState;

        private static PlayerIndex p1Controller;
        private static PlayerIndex p2Controller;

        private bool ggpo = false;
        private GGPOInputPackage currentLocalInput;
        private GGPOInputPackage oldLocalInput;

        private List<ControlProfile> profiles;

        public static ControllerManager Instance { get => instance; }

        private ControllerManager(ControlProfile profile1, ControlProfile profile2, ControlProfile profile3)
        {
            profiles = new List<ControlProfile>();
            profiles.Add(profile1);
            profiles.Add(profile2);
            profiles.Add(profile3);
            
            gamePads = new List<GamePadState>();
            oldGamePads = new List<GamePadState>();
        }

        /// <summary>
        /// Initialize a new controller manager
        /// </summary>
        public static void Initialize(ControlProfile profile1, ControlProfile profile2, ControlProfile profile3)
        {
            if(instance == null)
            {
                instance = new ControllerManager(profile1, profile2, profile3);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Change which controller is to be used for each player
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public void SetPlayerIndexes(PlayerIndex player1, PlayerIndex player2)
        {
            p1Controller = player1;
            p2Controller = player2;
        }

        /// <summary>
        /// Check the controller to see 
        /// </summary>
        /// <param name="playerIndex">Is this player one or player two?</param>
        /// <param name="action">Action to check for</param>
        /// <param name="old">Are we checking the old action?</param>
        /// <returns></returns>
        public static bool GetAction(int playerNumber, Actions action, bool old)
        {

            if (playerNumber == 4)
            {
                GGPOInputPackage refr;

                if (!old)
                    refr = instance.currentLocalInput;
                else
                    refr = instance.oldLocalInput;

                switch(action)
                {
                    case Actions.horizontalNeutral:
                        return !(refr.hardDown || refr.softDown ||
                            refr.hardUp || refr.softUp ||
                            refr.hardLeft || refr.softLeft ||
                            refr.hardRight || refr.softRight);
                    case Actions.hardRight:
                        return refr.hardRight;
                    case Actions.softRight:
                        return refr.softRight;
                    case Actions.hardLeft:
                        return refr.hardLeft;
                    case Actions.softLeft:
                        return refr.softLeft;
                    case Actions.hardUp:
                        return refr.hardUp;
                    case Actions.softUp:
                        return refr.softUp;
                    case Actions.hardDown:
                        return refr.hardDown;
                    case Actions.softDown:
                        return refr.softDown;
                    case Actions.start:
                        return refr.start;
                    case Actions.attack:
                        return refr.attack;
                    case Actions.jump:
                        return refr.jump;
                    case Actions.dodge:
                        return refr.dodge;
                    default:
                        return GetAction(1, action, old) || GetAction(2, action, old) || GetAction(3, action, old);
                }

            }
            else if (playerNumber == 5)
            {
                return true;
                throw new NotImplementedException();
            }

            if (instance.profiles[playerNumber - 1].UsingController)
            {
                //Fetch the gamepad
                GamePadState toBeChecked;
                if (!old)
                    toBeChecked = instance.gamePads[playerNumber - 1];
                else
                    toBeChecked = instance.oldGamePads[playerNumber - 1];
                if (action == Actions.start || action == Actions.attack || action == Actions.dodge || action == Actions.jump)
                {
                    List<Buttons> buttons = instance.profiles[playerNumber - 1].GetButtons(action);

                    foreach (Buttons button in buttons)
                    {
                        if (toBeChecked.IsButtonDown(button))
                            return true;
                    }
                    return false;
                }
                else
                {
                    switch (action)
                    {
                        //case Actions.start:
                        //    if (toBeChecked.IsButtonDown(Buttons.Start))

                        //        return true;
                        //    return false;
                        //case Actions.attack:
                        //    if (toBeChecked.IsButtonDown(Buttons.A))
                        //        return true;
                        //    return false;
                        //case Actions.dodge:
                        //    //if (toBeChecked.Triggers.Right >= 0.5)
                        //    if (toBeChecked.IsButtonDown(Buttons.RightTrigger))
                        //        return true;
                        //    return false;
                        //case Actions.jump:
                        //    if (toBeChecked.IsButtonDown(Buttons.X) || toBeChecked.IsButtonDown(Buttons.Y))
                        //        return true;
                        //    return false;

                        case Actions.hardLeft:
                            if (toBeChecked.ThumbSticks.Left.X <= -0.6f)
                                return true;
                            return false;
                        case Actions.hardRight:
                            if (toBeChecked.ThumbSticks.Left.X >= 0.6f)
                                return true;
                            return false;
                        case Actions.hardDown:
                            if (toBeChecked.ThumbSticks.Left.Y <= -0.5f)
                                return true;
                            return false;

                        case Actions.softLeft:
                            if (toBeChecked.ThumbSticks.Left.X < 0 && toBeChecked.ThumbSticks.Left.X > -0.6f)
                                return true;
                            return false;
                        case Actions.softRight:
                            if (toBeChecked.ThumbSticks.Left.X > 0 && toBeChecked.ThumbSticks.Left.X < 0.6f)
                                return true;
                            return false;
                        case Actions.horizontalNeutral:
                            if (toBeChecked.ThumbSticks.Left.X == 0)
                                return true;
                            return false;
                        case Actions.leftBumper:
                            if (toBeChecked.IsButtonUp(Buttons.LeftShoulder))
                                return true;
                            return false;
                        case Actions.rightBumper:
                            if (toBeChecked.IsButtonUp(Buttons.RightShoulder))
                                return true;
                            return false;
                    }

                    return false;
                }
            }
            else
            {
                KeyboardState toBeChecked = instance.keyBoardState;
                if (old)
                    toBeChecked = instance.oldKeyboardState;

                //Cheack for soft input
                List<Keys> buttons = instance.profiles[playerNumber - 1].GetKeys(Actions.holdSoft);
                bool isHoldsoft = false;
                foreach (Keys key in buttons)
                {
                    if (toBeChecked.IsKeyDown(key))
                        isHoldsoft = true;
                }
                if(isHoldsoft && (action == Actions.hardLeft || action == Actions.hardRight || action == Actions.hardUp || action == Actions.hardDown || action == Actions.softLeft || action == Actions.softRight || action == Actions.softUp || action == Actions.softDown))
                {
                    if (action == Actions.hardLeft || action == Actions.hardRight || action == Actions.hardUp || action == Actions.hardDown)
                        return false;
                    else if(action == Actions.softLeft)
                    {
                        buttons = instance.profiles[playerNumber - 1].GetKeys(Actions.hardLeft);
                        foreach (Keys key in buttons)
                        {
                            if (toBeChecked.IsKeyDown(key))
                                return true;
                        }
                        return false;
                    }
                    else if (action == Actions.softRight)
                    {
                        buttons = instance.profiles[playerNumber - 1].GetKeys(Actions.hardRight);
                        foreach (Keys key in buttons)
                        {
                            if (toBeChecked.IsKeyDown(key))
                                return true;
                        }
                        return false;
                    }
                    else if (action == Actions.softUp)
                    {
                        buttons = instance.profiles[playerNumber - 1].GetKeys(Actions.hardUp);
                        foreach (Keys key in buttons)
                        {
                            if (toBeChecked.IsKeyDown(key))
                                return true;
                        }
                        return false;
                    }
                    else if (action == Actions.softDown)
                    {
                        buttons = instance.profiles[playerNumber - 1].GetKeys(Actions.hardDown);
                        foreach (Keys key in buttons)
                        {
                            if (toBeChecked.IsKeyDown(key))
                                return true;
                        }
                        return false;
                    }
                }

                if (action != Actions.horizontalNeutral)
                {
                    buttons = instance.profiles[playerNumber - 1].GetKeys(action);

                    foreach (Keys key in buttons)
                    {
                        if (toBeChecked.IsKeyDown(key))
                            return true;
                    }
                    return false;
                }
                else
                {
                    if (GetAction(3, Actions.hardLeft, old) || GetAction(3, Actions.hardRight, old) || GetAction(3, Actions.softLeft, old) || GetAction(3, Actions.softRight, old))
                        return false;
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets the exact X coordinate of the gamepad for aerial drift
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static float GetDrift(int index)
        {

            if(index == 4)
            {
                if (instance.currentLocalInput.hardLeft)
                    return -1;
                else if (instance.currentLocalInput.softLeft)
                    return -0.5f;
                else if (instance.currentLocalInput.hardRight)
                    return 1;
                else if (instance.currentLocalInput.softRight)
                    return 0.5f;
                else
                    return 0;

            }
            else if(index == 5)
            {
                return 1;
            }

            else if(index != 3)
                return instance.gamePads[index - 1].ThumbSticks.Left.X;
            else
            {
                if (ControllerManager.GetAction(index, Actions.hardRight, false))
                    return 1f;
                else if (ControllerManager.GetAction(index, Actions.hardLeft, false))
                    return -1f;
                else if (ControllerManager.GetAction(index, Actions.softLeft, false))
                    return -0.5f;
                else if (ControllerManager.GetAction(index, Actions.softRight, false))
                    return 0.5f;
                else
                    return 0f;
            }
        }
        public static float GetHeight(int index)
        {
            if(index == 4)
            {
                if (instance.currentLocalInput.hardUp)
                    return 1;
                else if (instance.currentLocalInput.softUp)
                    return 0.5f;
                else if (instance.currentLocalInput.hardDown)
                    return -1;
                else if (instance.currentLocalInput.softDown)
                    return -0.5f;
                else
                    return 0;
            }
            else if(index == 5)
            {
                return 1;
            }

            if(index != 3)
                return instance.gamePads[index - 1].ThumbSticks.Left.Y;
            else
            {
                if (ControllerManager.GetAction(index, Actions.hardDown, false))
                    return -1f;
                else if (ControllerManager.GetAction(index, Actions.hardUp, false))
                    return 1f;
                else if (ControllerManager.GetAction(index, Actions.softDown, false))
                    return -0.5f;
                else if (ControllerManager.GetAction(index, Actions.softUp, false))
                    return 0.5f;
                else
                    return 0f;
            }
        }

        /// <summary>
        /// Check the controllers of both players
        /// </summary>
        public static void Update(bool ggpo = false)
        {
            //Move the old gamepadstates into the old list
            instance.oldGamePads.Clear();
            instance.oldGamePads.AddRange(instance.gamePads);
            instance.gamePads.Clear();
            instance.oldKeyboardState = instance.keyBoardState;
            
            //check the controller again
            instance.gamePads.Add(GamePad.GetState(p1Controller));
            instance.gamePads.Add(GamePad.GetState(p2Controller));
            instance.keyBoardState = Keyboard.GetState();

            if(ggpo)
            {
                RequestGGPOLocalInput(true);
            }

        }

        public static uint RequestGGPOLocalInput(bool old)
        {
            instance.ggpo = true;

            GGPOInputPackage pack = new GGPOInputPackage();

            pack.attack = GetAction(1, Actions.attack, old) || GetAction(2, Actions.attack, old) || GetAction(3, Actions.attack, old);
            pack.dodge = GetAction(1, Actions.dodge, old) || GetAction(2, Actions.dodge, old) || GetAction(3, Actions.dodge, old);
            pack.jump = GetAction(1, Actions.jump, old) || GetAction(2, Actions.jump, old) || GetAction(3, Actions.jump, old);
            pack.start = GetAction(1, Actions.start, old) || GetAction(2, Actions.start, old) || GetAction(3, Actions.start, old);

            if (ControllerManager.GetAction(3, Actions.hardRight, old))
                pack.hardRight = true;
            if (ControllerManager.GetAction(3, Actions.hardLeft, old))
                pack.hardLeft = true;
            if (ControllerManager.GetAction(3, Actions.softLeft, old))
                pack.softLeft = true;
            if (ControllerManager.GetAction(3, Actions.softRight, old))
                pack.softRight = true;
            if (ControllerManager.GetAction(3, Actions.hardUp, old))
                pack.hardUp = true;
            if (ControllerManager.GetAction(3, Actions.hardDown, old))
                pack.hardDown = true;
            if (ControllerManager.GetAction(3, Actions.softDown, old))
                pack.softDown = true;
            if (ControllerManager.GetAction(3, Actions.softUp, old))
                pack.softUp = true;

            if (!old)
            {
                if (instance.gamePads[0].ThumbSticks.Left.X > 0.5 || instance.gamePads[1].ThumbSticks.Left.X > 0.5)
                    pack.hardLeft = true;
                else if (instance.gamePads[0].ThumbSticks.Left.X > 0.2 || instance.gamePads[1].ThumbSticks.Left.X > 0.2)
                    pack.softLeft = true;
                else if (instance.gamePads[0].ThumbSticks.Left.X < -0.5 || instance.gamePads[1].ThumbSticks.Left.X < -0.5)
                    pack.hardRight = true;
                else if (instance.gamePads[0].ThumbSticks.Left.X < -0.2 || instance.gamePads[1].ThumbSticks.Left.X < -0.2)
                    pack.softRight = true;

                if (instance.gamePads[0].ThumbSticks.Left.Y > 0.5 || instance.gamePads[1].ThumbSticks.Left.Y > 0.5)
                    pack.hardUp = true;
                else if (instance.gamePads[0].ThumbSticks.Left.Y > 0.2 || instance.gamePads[1].ThumbSticks.Left.Y > 0.2)
                    pack.softUp = true;
                else if (instance.gamePads[0].ThumbSticks.Left.Y < -0.5 || instance.gamePads[1].ThumbSticks.Left.Y < -0.5)
                    pack.hardDown = true;
                else if (instance.gamePads[0].ThumbSticks.Left.Y < -0.2 || instance.gamePads[1].ThumbSticks.Left.Y < -0.2)
                    pack.softDown = true;

            }
            else
            {
                if (instance.oldGamePads[0].ThumbSticks.Left.X > 0.5 || instance.oldGamePads[1].ThumbSticks.Left.X > 0.5)
                    pack.hardLeft = true;
                else if (instance.oldGamePads[0].ThumbSticks.Left.X > 0.2 || instance.oldGamePads[1].ThumbSticks.Left.X > 0.2)
                    pack.softLeft = true;
                else if (instance.oldGamePads[0].ThumbSticks.Left.X < -0.5 || instance.oldGamePads[1].ThumbSticks.Left.X < -0.5)
                    pack.hardRight = true;
                else if (instance.oldGamePads[0].ThumbSticks.Left.X < -0.2 || instance.oldGamePads[1].ThumbSticks.Left.X < -0.2)
                    pack.softRight = true;

                if (instance.oldGamePads[0].ThumbSticks.Left.Y > 0.5 || instance.oldGamePads[1].ThumbSticks.Left.Y > 0.5)
                    pack.hardUp = true;
                else if (instance.oldGamePads[0].ThumbSticks.Left.Y > 0.2 || instance.oldGamePads[1].ThumbSticks.Left.Y > 0.2)
                    pack.softUp = true;
                else if (instance.oldGamePads[0].ThumbSticks.Left.Y < -0.5 || instance.oldGamePads[1].ThumbSticks.Left.Y < -0.5)
                    pack.hardDown = true;
                else if (instance.oldGamePads[0].ThumbSticks.Left.Y < -0.2 || instance.oldGamePads[1].ThumbSticks.Left.Y < -0.2)
                    pack.softDown = true;
            }

            if (old)
                Instance.oldLocalInput = pack;
            else
                Instance.currentLocalInput = pack;


            return instance.PackInput(pack);

        }

        private uint PackInput(GGPOInputPackage package)
        {
            uint value = 0;

            if(package.start)
                value = value & 1;
            value = value << 1;

            if (package.attack)
                value = value & 1;
            value = value << 1;

            if (package.jump)
                value = value & 1;
            value = value << 1;

            if (package.dodge)
                value = value & 1;
            value = value << 1;

            if (package.hardLeft)
                value = value & 1;
            value = value << 1;

            if (package.softLeft)
                value = value & 1;
            value = value << 1;

            if (package.hardRight)
                value = value & 1;
            value = value << 1;

            if (package.softRight)
                value = value & 1;
            value = value << 1;

            if (package.hardUp)
                value = value & 1;
            value = value << 1;

            if (package.softUp)
                value = value & 1;
            value = value << 1;

            if (package.hardDown)
                value = value & 1;
            value = value << 1;

            if (package.softDown)
                value = value & 1;

            return value;

        }


        public static void FeedLocalInput()
        {

        }

        public static void ClearGGPO()
        {
            instance.ggpo = false;
        }

    }
}
