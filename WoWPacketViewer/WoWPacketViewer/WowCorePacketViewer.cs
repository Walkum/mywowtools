using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WoWPacketViewer
{
	public class WowCorePacketViewer : PacketViewerBase
	{
		public override IEnumerable<Packet> ReadPackets(string file)
		{
			var packets = new List<Packet>();

			var gr = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read), Encoding.ASCII);
			gr.ReadBytes(3); // PKT
			gr.ReadBytes(2); // 0x02, 0x02
			gr.ReadByte(); // 0x06
			build = gr.ReadUInt16(); // build
			gr.ReadBytes(4); // client locale
			gr.ReadBytes(20); // packet key
			gr.ReadBytes(64); // realm name

			while (gr.PeekChar() >= 0)
			{
				Direction direction = gr.ReadByte() == 0xff ? Direction.Server : Direction.Client;
				uint unixtime = gr.ReadUInt32();
				uint tickcount = gr.ReadUInt32();
				uint size = gr.ReadUInt32();
				OpCodes opcode = (direction == Direction.Client) ? (OpCodes) gr.ReadUInt32() : (OpCodes) gr.ReadUInt16();
				byte[] data = gr.ReadBytes((int) size
				                           - ((direction == Direction.Client) ? 4 : 2)
					);

				packets.Add(new Packet(direction, opcode, data, unixtime, tickcount));
			}
			gr.Close();
			return packets;
		}
	}
}