using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace xyslot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[] specieslist = { };
        string[] metXY_00000 = { };
        private string[] getStringList(string f, string l)
        {
            object txt = Properties.Resources.ResourceManager.GetObject("text_" + f + "_" + l); // Fetch File, \n to list.
            List<string> rawlist = ((string)txt).Split(new char[] { '\n' }).ToList();

            string[] stringdata = new string[rawlist.Count];
            for (int i = 0; i < rawlist.Count; i++)
                stringdata[i] = rawlist[i];

            return stringdata;
        }
        private void process(object sender, EventArgs e)
        {
            process("ja");
            process("en");
            process("fr");
            process("it");
            process("de");
            process("es");
            process("ko");
        }
        private void process(string l)
        {
            Directory.CreateDirectory("C:\\Users\\Kurt\\Desktop\\XY Slots");
            Directory.CreateDirectory("C:\\Users\\Kurt\\Desktop\\XY Slots\\" + l);
            string inFolder = "C:\\Users\\Kurt\\Desktop\\X";
            string destFolderAndPrefix = "C:\\Users\\Kurt\\Desktop\\XY Slots\\" + l + "\\X Slots - ";
            string destExt = ".csv";

            specieslist = getStringList("Species", l);
            specieslist[0] = "---";
            metXY_00000 = getStringList("xy_00000", l);

            byte[] zonedata = File.ReadAllBytes(inFolder + "\\360.bin");
            string[] filepaths = Directory.GetFiles(inFolder, "*.*", SearchOption.TopDirectoryOnly);

            Grass = "Location,0,,,1,,,2,,,3,,,4,,,5,,,6,,,7,,,8,,,9,,,10,,,11,,,\r\n";
            Yellow = "Location,0,,,1,,,2,,,3,,,4,,,5,,,6,,,7,,,8,,,9,,,10,,,11,,,\r\n";
            Purple = "Location,0,,,1,,,2,,,3,,,4,,,5,,,6,,,7,,,8,,,9,,,10,,,11,,,\r\n";
            Red = "Location,0,,,1,,,2,,,3,,,4,,,5,,,6,,,7,,,8,,,9,,,10,,,11,,,\r\n";
            Rough = "Location,0,,,1,,,2,,,3,,,4,,,5,,,6,,,7,,,8,,,9,,,10,,,11,,,\r\n";

            Water = "Location,0,,,1,,,2,,,3,,,4,,,\r\n";
            RockSmash = "Location,0,,,1,,,2,,,3,,,4,,,\r\n";
            OldRod = "Location,0,,,1,,,2,,,\r\n";
            GoodRod = "Location,0,,,1,,,2,,,\r\n";
            SuperRod = "Location,0,,,1,,,2,,,\r\n";
            
            Horde1 = "Location,0,,,1,,,2,,,3,,,4,,,\r\n";
            Horde2 = "Location,0,,,1,,,2,,,3,,,4,,,\r\n";
            Horde3 = "Location,0,,,1,,,2,,,3,,,4,,,\r\n";

            for (int f = 0; f < filepaths.Length; f++)
            {
                FileStream InStream = File.OpenRead(filepaths[f]);
                BinaryReader br = new BinaryReader(InStream);
                br.BaseStream.Seek(0x10, SeekOrigin.Begin);
                int offset = br.ReadInt32() + 0x10;
                int length = (int)br.BaseStream.Length - offset;

                if (length < 0x178)
                {
                    continue;
                }
                br.Close();

                byte[] filedata = File.ReadAllBytes(filepaths[f]);

                FileInfo fi = new FileInfo(filepaths[f]);
                string name = Path.GetFileNameWithoutExtension(filepaths[f]);
                ;
                int LocationNum = Convert.ToInt16(name.Substring(4, name.Length - 4));
                string LocationName = metXY_00000[zonedata[LocationNum * 56 + 0x1C]];

                byte[] encounterdata = new Byte[0x178];
                Array.Copy(filedata, offset, encounterdata, 0, 0x178);
                parse(encounterdata,LocationName);
            }
            File.WriteAllText(destFolderAndPrefix + "Grass" + destExt, Grass, Encoding.UTF8);
            File.WriteAllText(destFolderAndPrefix + "Yellow" + destExt, Yellow, Encoding.UTF8);
            File.WriteAllText(destFolderAndPrefix + "Purple" + destExt, Purple, Encoding.UTF8);
            File.WriteAllText(destFolderAndPrefix + "Red" + destExt, Red, Encoding.UTF8);
            File.WriteAllText(destFolderAndPrefix + "Rough" + destExt, Rough, Encoding.UTF8);

            File.WriteAllText(destFolderAndPrefix + "Water" + destExt, Water, Encoding.UTF8);
            File.WriteAllText(destFolderAndPrefix + "RockSmash" + destExt, RockSmash, Encoding.UTF8);
            File.WriteAllText(destFolderAndPrefix + "OldRod" + destExt, OldRod, Encoding.UTF8);
            File.WriteAllText(destFolderAndPrefix + "GoodRod" + destExt, GoodRod, Encoding.UTF8);
            File.WriteAllText(destFolderAndPrefix + "SuperRod" + destExt, SuperRod, Encoding.UTF8);

            File.WriteAllText(destFolderAndPrefix + "Horde1" + destExt, Horde1, Encoding.UTF8);
            File.WriteAllText(destFolderAndPrefix + "Horde2" + destExt, Horde2, Encoding.UTF8);
            File.WriteAllText(destFolderAndPrefix + "Horde3" + destExt, Horde3, Encoding.UTF8);
        }
        string Grass = "";
        string Yellow = "";
        string Purple = "";
        string Red = "";
        string Rough = "";

        string Water = "";
        string RockSmash = "";
        string OldRod = "";
        string GoodRod = "";
        string SuperRod = "";

        string Horde1 = "";
        string Horde2 = "";
        string Horde3 = "";
        private void parse(byte[] ed, string LocationName)
        {
            // 12,12,12,12,12
            // 5,5
            // 3,3,3
            // 5,5,5,
            byte[] slot = new Byte[4];
            int offset = 0;
            string result = "";
            #region 12
            for (int i = 0; i < 12; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") Grass += (LocationName + ',' + result + "\r\n"); result = "";
            for (int i = 0; i < 12; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") Yellow += (LocationName + ',' + result + "\r\n"); result = "";
            for (int i = 0; i < 12; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") Purple += (LocationName + ',' + result + "\r\n"); result = "";
            for (int i = 0; i < 12; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") Red += (LocationName + ',' + result + "\r\n"); result = "";
            for (int i = 0; i < 12; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") Rough += (LocationName + ',' + result + "\r\n"); result = "";
            #endregion
            #region 5
            for (int i = 0; i < 5; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") Water += (LocationName + ',' + result + "\r\n"); result = "";
            for (int i = 0; i < 5; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") RockSmash += (LocationName + ',' + result + "\r\n"); result = "";

            #endregion
            #region 3
            for (int i = 0; i < 3; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") OldRod += (LocationName + ',' + result + "\r\n"); result = "";
            for (int i = 0; i < 3; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") GoodRod += (LocationName + ',' + result + "\r\n"); result = "";
            for (int i = 0; i < 3; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") SuperRod += (LocationName + ',' + result + "\r\n"); result = "";
            #endregion
            #region 5
            for (int i = 0; i < 5; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") Horde1 += (LocationName + ',' + result + "\r\n"); result = "";
            for (int i = 0; i < 5; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") Horde2 += (LocationName + ',' + result + "\r\n"); result = "";
            for (int i = 0; i < 5; i++)
            {
                Array.Copy(ed, offset, slot, 0, 4);
                result += parseslot(slot);
                offset += 4;
            } if (result != "") Horde3 += (LocationName + ',' + result + "\r\n"); result = "";
            #endregion
        }
        private string parseslot(byte[] slot)
        {
            int index = BitConverter.ToUInt16(slot, 0) & 0x3FF;
            if (index == 0) return "";
            int form = BitConverter.ToUInt16(slot, 0) >> 11;
            int min = slot[2];
            int max = slot[3];
            string species = specieslist[index];
            if (form > 0) species += "-"+form.ToString();
            return species + ',' + min + ',' + max + ',';
        }
    }
}
