

namespace shared
{
    public class InteractData : ASerializable
    {
        public bool interactPressed;

        public override void Serialize(Packet pPacket)
        {
            pPacket.Write(interactPressed.ToString());
        }

        public override void Deserialize(Packet pPacket)
        {
            string sInteractPressed = pPacket.ReadString();
            interactPressed = bool.Parse(sInteractPressed);
        }
    }
}