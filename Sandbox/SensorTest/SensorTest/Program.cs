namespace SensorTest
{
    using System;
    using System.Buffers.Binary;
    using System.IO.Ports;
    using System.Threading;

    public static class Program
    {
        public static void Main(string[] args)
        {
            // Open
            var port = new SerialPort(args.Length == 0 ? "/dev/ttyUSB0" : args[0])
            {
                BaudRate = 115200,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None,
                Handshake = Handshake.None
            };

            var wait = new object();

            port.DataReceived += (sender, eventArgs) =>
            {
                lock (wait)
                {
                    Monitor.PulseAll(wait);
                }
            };

            port.Open();
            port.DiscardOutBuffer();
            port.DiscardInBuffer();

            // Send
            var command = new byte[]
            {
                0x52, 0x42,         // Header
                0x05, 0x00,         // Length 5
                0x01, 0x21, 0x50,   // Read 0x5021
                0x00, 0x00          // CRC Area
            };

            var crc = CalcCrc(command.AsSpan(0, command.Length - 2));
            BinaryPrimitives.WriteUInt16LittleEndian(command.AsSpan(command.Length - 2, 2), crc);

            Console.WriteLine("Send: " + BitConverter.ToString(command, 0, command.Length));

            port.Write(command, 0, command.Length);

            // Wait
            lock (wait)
            {
                if (Monitor.Wait(wait, 5000))
                {
                    // Read
                    var buffer = new byte[256];
                    var read = port.Read(buffer, 0, buffer.Length);

                    Console.WriteLine("Recv: " + BitConverter.ToString(buffer, 0, read));

                    var temperature = (decimal)BinaryPrimitives.ReadInt16LittleEndian(buffer.AsSpan(8, 2)) / 100;
                    var humidity = (decimal)BinaryPrimitives.ReadInt16LittleEndian(buffer.AsSpan(10, 2)) / 100;
                    var light = BinaryPrimitives.ReadInt16LittleEndian(buffer.AsSpan(12, 2));

                    // TODO other

                    Console.WriteLine($"Temperature(C): {temperature}");
                    Console.WriteLine($"Humidity(%): {humidity}");
                    Console.WriteLine($"Light(lx): {light}");
                }
            }
        }

        private static ushort CalcCrc(Span<byte> span)
        {
            var crc = (ushort)0xFFFF;
            for (var i = 0; i < span.Length; i++)
            {
                crc = (ushort)(crc ^ span[i]);
                for (var j = 0; j < 8; j++)
                {
                    var carry = crc & 1;
                    if (carry != 0)
                    {
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }

            return crc;
        }
    }
}
