using System;

namespace ProjectEternity.Core.Online
{
    public class PlayerPOCO
    {
        public string ID;
        public string Name;
        public byte[] Info;

        public PlayerPOCO()
        {
            ID = string.Empty;
            Name = string.Empty;
            Info = new byte[0];
        }
    }
}
