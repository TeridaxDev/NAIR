using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Nair.Classes
{
    class ControlProfile
    {
        private bool usingController;
        private Dictionary<Actions, List<Buttons>> controller;
        private Dictionary<Actions, List<Keys>> keyboard;


        public bool UsingController { get => usingController; }



        public List<Buttons> GetButtons(Actions action)
        {
            if (controller.ContainsKey(action))
                return controller[action];

            throw new InvalidOperationException();
        }
        public List<Keys> GetKeys(Actions action)
        {
            if (keyboard.ContainsKey(action))
                return keyboard[action];

            throw new InvalidOperationException();
        }

        public ControlProfile(bool usingController)
        {
            this.usingController = usingController;

            if (usingController)
            {
                controller = new Dictionary<Actions, List<Buttons>>();

                controller[Actions.start] = new List<Buttons>();
                controller[Actions.attack] = new List<Buttons>();
                controller[Actions.dodge] = new List<Buttons>();
                controller[Actions.jump] = new List<Buttons>();
            }
            else
            {
                keyboard = new Dictionary<Actions, List<Keys>>();

                keyboard[Actions.start] = new List<Keys>();
                keyboard[Actions.attack] = new List<Keys>();
                keyboard[Actions.dodge] = new List<Keys>();
                keyboard[Actions.jump] = new List<Keys>();

                keyboard[Actions.hardLeft] = new List<Keys>();
                keyboard[Actions.hardRight] = new List<Keys>();
                keyboard[Actions.hardDown] = new List<Keys>();
                keyboard[Actions.hardUp] = new List<Keys>();
                keyboard[Actions.softLeft] = new List<Keys>();
                keyboard[Actions.softRight] = new List<Keys>();
                keyboard[Actions.softUp] = new List<Keys>();
                keyboard[Actions.softDown] = new List<Keys>();
                keyboard[Actions.softLeft] = new List<Keys>();
                keyboard[Actions.holdSoft] = new List<Keys>();

                keyboard[Actions.leftBumper] = new List<Keys>();
                keyboard[Actions.rightBumper] = new List<Keys>();
            }
        }

        public void SetDefaults()
        {
            if(usingController)
            {

                controller[Actions.start].Clear();
                controller[Actions.attack].Clear();
                controller[Actions.dodge].Clear();
                controller[Actions.jump].Clear();

                controller[Actions.start].Add(Buttons.Start);
                controller[Actions.attack].Add(Buttons.A);
                controller[Actions.dodge].Add(Buttons.RightTrigger);
                controller[Actions.dodge].Add(Buttons.RightShoulder);
                controller[Actions.jump].Add(Buttons.Y);
                controller[Actions.jump].Add(Buttons.X);
                controller[Actions.jump].Add(Buttons.LeftTrigger);
                controller[Actions.jump].Add(Buttons.LeftShoulder);
            }
            else
            {
                keyboard[Actions.start].Clear();
                keyboard[Actions.hardLeft].Clear();
                keyboard[Actions.hardRight].Clear();
                keyboard[Actions.hardDown].Clear();
                keyboard[Actions.hardUp].Clear();

                keyboard[Actions.softLeft].Clear();
                keyboard[Actions.softRight].Clear();
                keyboard[Actions.softDown].Clear();
                keyboard[Actions.softUp].Clear();

                keyboard[Actions.holdSoft].Clear();

                keyboard[Actions.jump].Clear();
                keyboard[Actions.attack].Clear();
                keyboard[Actions.dodge].Clear();

                keyboard[Actions.leftBumper].Clear();
                keyboard[Actions.rightBumper].Clear();



                keyboard[Actions.start].Add(Keys.Escape);

                keyboard[Actions.hardLeft].Add(Keys.Left);
                keyboard[Actions.hardRight].Add(Keys.Right);
                keyboard[Actions.hardDown].Add(Keys.Down);
                keyboard[Actions.hardUp].Add(Keys.Up);

                keyboard[Actions.softLeft].Add(Keys.A);
                keyboard[Actions.softRight].Add(Keys.D);
                keyboard[Actions.softDown].Add(Keys.S);
                keyboard[Actions.softUp].Add(Keys.W);

                keyboard[Actions.holdSoft].Add(Keys.LeftShift);

                keyboard[Actions.jump].Add(Keys.Z);
                keyboard[Actions.attack].Add(Keys.X);
                keyboard[Actions.dodge].Add(Keys.C);

                keyboard[Actions.leftBumper].Add(Keys.Q);
                keyboard[Actions.rightBumper].Add(Keys.E);
            }
        }
        public void Clear()
        {
            if(usingController)
            {
                controller[Actions.start].Clear();
                controller[Actions.attack].Clear();
                controller[Actions.dodge].Clear();
                controller[Actions.jump].Clear();
            }
            else
            {
                keyboard[Actions.start].Clear();
                keyboard[Actions.attack].Clear();
                keyboard[Actions.dodge].Clear();
                keyboard[Actions.jump].Clear();

                keyboard[Actions.hardLeft].Clear();
                keyboard[Actions.hardRight].Clear();
                keyboard[Actions.hardDown].Clear();
                keyboard[Actions.softLeft].Clear();
                keyboard[Actions.softRight].Clear();
                keyboard[Actions.hardUp].Clear();
                keyboard[Actions.softUp].Clear();
                keyboard[Actions.softDown].Clear();

                keyboard[Actions.holdSoft].Clear();

                keyboard[Actions.leftBumper].Clear();
                keyboard[Actions.rightBumper].Clear();
            }
        }

        public void AddButton(int intAction, Buttons button)
        {
            if (!usingController)
                throw new InvalidOperationException();

            Actions action = Actions.start;
            switch(intAction)
            {
                case 0:
                    action = Actions.attack;
                    break;
                case 1:
                    action = Actions.jump;
                    break;
                case 2:
                    action = Actions.dodge;
                    break;
                case 3:
                    return;
            }

            if (controller.ContainsKey(action))
            {
                controller[action].Add(button);
            }

            //Save the actual file

        }
        public void AddButton(Actions action, Buttons button)
        {
            if (!usingController)
                throw new InvalidOperationException();

            if (controller.ContainsKey(action))
                controller[action].Add(button);
        }
        public void AddButton(Actions action, Keys key)
        {
            if (usingController)
                throw new InvalidOperationException();

            if (keyboard.ContainsKey(action))
                keyboard[action].Add(key);
        }

        public void SaveControllerProfile(int controllerNum)
        {
            BinaryWriter writer = null;
            try
            {
                Stream streamOut = File.OpenWrite("ControlProfile" + controllerNum.ToString() + ".cfg");
                writer = new BinaryWriter(streamOut);

                writer.Write(controller[Actions.start].Count);
                foreach (Buttons button in controller[Actions.start])
                {
                    writer.Write((int)button);
                }
                writer.Write(controller[Actions.attack].Count);
                foreach (Buttons button in controller[Actions.attack])
                {
                    writer.Write((int)button);
                }
                writer.Write(controller[Actions.jump].Count);
                foreach (Buttons button in controller[Actions.jump])
                {
                    writer.Write((int)button);
                }
                writer.Write(controller[Actions.dodge].Count);
                foreach (Buttons button in controller[Actions.dodge])
                {
                    writer.Write((int)button);
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                writer.Close();
            }
        }
        public void LoadControllerProfile(int controllerNum)
        {
            this.Clear();

            BinaryReader reader = null;
            try
            {
                Stream streamIn = File.OpenRead("ControlProfile" + controllerNum.ToString() + ".cfg");
                reader = new BinaryReader(streamIn);

                Buttons button;
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    button = (Buttons)reader.ReadInt32();
                    this.AddButton(Actions.start, button);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    button = (Buttons)reader.ReadInt32();
                    this.AddButton(Actions.attack, button);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    button = (Buttons)reader.ReadInt32();
                    this.AddButton(Actions.jump, button);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    button = (Buttons)reader.ReadInt32();
                    this.AddButton(Actions.dodge, button);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if(reader != null)
                    reader.Close();
            }
        }

        public void LoadKeyProfile()
        {
            this.Clear();

            BinaryReader reader = null;

            try
            {
                Stream streamIn = File.OpenRead("ControlProfile3.cfg");
                reader = new BinaryReader(streamIn);

                Keys key;
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.start, key);
                }

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.hardUp, key);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.hardDown, key);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.hardLeft, key);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.hardRight, key);
                }

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.attack, key);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.jump, key);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.dodge, key);
                }

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.holdSoft, key);
                }

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.softUp, key);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.softDown, key);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.softLeft, key);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.softRight, key);
                }

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.leftBumper, key);
                }
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    key = (Keys)reader.ReadInt32();
                    this.AddButton(Actions.rightBumper, key);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if(reader != null)
                    reader.Close();
            }
        }

        public void SaveKeyProfile()
        {
            BinaryWriter writer = null;
            try
            {
                Stream streamOut = File.OpenWrite("ControlProfile3.cfg");
                writer = new BinaryWriter(streamOut);

                writer.Write(keyboard[Actions.start].Count);
                foreach (Keys key in keyboard[Actions.start])
                {
                    writer.Write((int)key);
                }

                writer.Write(keyboard[Actions.hardUp].Count);
                foreach (Keys key in keyboard[Actions.hardUp])
                {
                    writer.Write((int)key);
                }
                writer.Write(keyboard[Actions.hardDown].Count);
                foreach (Keys key in keyboard[Actions.hardDown])
                {
                    writer.Write((int)key);
                }
                writer.Write(keyboard[Actions.hardLeft].Count);
                foreach (Keys key in keyboard[Actions.hardLeft])
                {
                    writer.Write((int)key);
                }
                writer.Write(keyboard[Actions.hardRight].Count);
                foreach (Keys key in keyboard[Actions.hardRight])
                {
                    writer.Write((int)key);
                }

                writer.Write(keyboard[Actions.attack].Count);
                foreach (Keys key in keyboard[Actions.attack])
                {
                    writer.Write((int)key);
                }
                writer.Write(keyboard[Actions.jump].Count);
                foreach (Keys key in keyboard[Actions.jump])
                {
                    writer.Write((int)key);
                }
                writer.Write(keyboard[Actions.dodge].Count);
                foreach (Keys key in keyboard[Actions.dodge])
                {
                    writer.Write((int)key);
                }

                writer.Write(keyboard[Actions.holdSoft].Count);
                foreach (Keys key in keyboard[Actions.holdSoft])
                {
                    writer.Write((int)key);
                }

                writer.Write(keyboard[Actions.softUp].Count);
                foreach (Keys key in keyboard[Actions.softUp])
                {
                    writer.Write((int)key);
                }
                writer.Write(keyboard[Actions.softDown].Count);
                foreach (Keys key in keyboard[Actions.softDown])
                {
                    writer.Write((int)key);
                }
                writer.Write(keyboard[Actions.softLeft].Count);
                foreach (Keys key in keyboard[Actions.softLeft])
                {
                    writer.Write((int)key);
                }
                writer.Write(keyboard[Actions.softRight].Count);
                foreach (Keys key in keyboard[Actions.softRight])
                {
                    writer.Write((int)key);
                }

                writer.Write(keyboard[Actions.leftBumper].Count);
                foreach (Keys key in keyboard[Actions.leftBumper])
                {
                    writer.Write((int)key);
                }
                writer.Write(keyboard[Actions.rightBumper].Count);
                foreach (Keys key in keyboard[Actions.rightBumper])
                {
                    writer.Write((int)key);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                writer.Close();
            }
        }

    }
}
