using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;


namespace STM32Cube_EmBlocks
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static XmlWriter xw = null;

        public static void writeXML(string[] strs, Boolean brk)
        {
            xw.WriteStartElement(strs[0]);
            for (int i = 1; i < strs.Count(); i += 2)
                xw.WriteAttributeString(strs[i], strs[i + 1]);
            if (brk)
                xw.WriteEndElement();
        }


        public static string readSrcFilePaths(string prj)
        {
            StreamReader sr = File.OpenText(prj); 
            String input;

            while ((input = sr.ReadLine()) != null)
            {
                if (input.Contains("[PreviousUsedTStudioFiles]"))
                {
                    input = sr.ReadLine();
                    if (input.Contains("HeaderPath="))
                    {
                        input = sr.ReadLine();
                        if (input.Contains("SourceFiles="))
                        {
                            int j, i = 11;
                            while (i < input.Length)
                            {
                                j = i;
                                i = input.IndexOf(';', i + 1);
                                if (i > 0)
                                {
                                    string fn = input.Substring(j, i - j).Substring(5).Replace("/", @"\").ToLower();
                                    writeXML(new string[] { "Unit", "filename", fn }, false);
                                    if (fn[fn.Length - 1] == 's')
                                        writeXML(new string[] { "Option", "compilerVar", "ASM" }, true);
                                    else
                                        writeXML(new string[] { "Option", "compilerVar", "CC" }, true);
                                    xw.WriteEndElement();
                                }
                                else
                                    return "OK";
                            }
                            return "OK";
                        }
                    }
                    else if (input.Contains("SourceFiles="))
                    {
                        int j, i = 0;
                        while (i < input.Length)
                        {
                            j = i;
                            i = input.IndexOf(';', i + 1);
                            if (i > 0)
                            {
                                string fn = input.Substring(j, i - j).Substring(5).Replace("/", @"\").ToLower();
                                writeXML(new string[] { "Unit", "filename", fn }, false);
                                if (fn[fn.Length - 1] == 's')
                                    writeXML(new string[] { "Option", "compilerVar", "ASM" }, true);
                                else
                                    writeXML(new string[] { "Option", "compilerVar", "CC" }, true);
                                xw.WriteEndElement();
                            }
                            else
                                return "OK";
                        }
                        return "OK";
                    }
                    else
                        return "ERROR: No Source file found!";
                }
            }
            return "ERROR: No Source file found!";
        }




        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = "e:\\WorkSpace";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string pt = folderBrowserDialog1.SelectedPath;
                string fn = Path.GetFileName(pt);
                string[] ldf = Directory.GetFiles(pt + @"\Projects\TrueSTUDIO\" + fn + " Configuration", "*.ld");
                foreach (string f in ldf)
                    File.Copy(f, pt + "\\" + Path.GetFileName(f), true);
                try
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.IndentChars = ("\t");
                    xw = XmlWriter.Create(pt + "\\" + fn + ".ebp", settings);
                    xw.WriteStartDocument(true);
                    xw.WriteStartElement("EmBlocks_project_file");
                    writeXML(new string[] { "EmBlocksVersion", "release", "2.10", "revision", "1" }, true);
                    writeXML(new string[] { "FileVersion", "major", "1", "minor", "0" }, true);

                    xw.WriteStartElement("Project");
                    writeXML(new string[] { "Option", "title", fn }, true);
                    writeXML(new string[] { "Option", "pch_mode", "2" }, true);
                    writeXML(new string[] { "Option", "compiler", "armgcc_eb" }, true);

                    xw.WriteStartElement("Build");
                    writeXML(new string[] { "Target", "title", "Debug" }, false);
                    writeXML(new string[] { "Option", "output", @".\Debug\" + fn + ".elf" }, true);
                    writeXML(new string[] { "Option", "object_output", @".\Debug" }, true);
                    writeXML(new string[] { "Option", "type", "0" }, true);
                    writeXML(new string[] { "Option", "compiler", "armgcc_eb" }, true);
                    writeXML(new string[] { "Option", "projectDeviceOptionsRelation", "0" }, true);

                    xw.WriteStartElement("Compiler");
                    writeXML(new string[] { "Add", "option", "-Wall" }, true);
                    writeXML(new string[] { "Add", "option", "-fdata-sections" }, true);
                    writeXML(new string[] { "Add", "option", "-ffunction-sections" }, true);
                    writeXML(new string[] { "Add", "option", "-O0" }, true);
                    writeXML(new string[] { "Add", "option", "-g3" }, true);
                    writeXML(new string[] { "Add", "option", "-DUSE_HAL_DRIVER" }, true);
                    writeXML(new string[] { "Add", "option", "-DSTM32F407xx" }, true);
                    xw.WriteEndElement();

                    xw.WriteStartElement("Assembler");
                    writeXML(new string[] { "Add", "option", "-Wa,--gdwarf-2" }, true);
                    xw.WriteEndElement();

                    xw.WriteStartElement("Linker");
                    writeXML(new string[] { "Add", "option", "-Wl,--gc-sections" }, true);
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    writeXML(new string[] { "Target", "title", "Release" }, false);
                    writeXML(new string[] { "Option", "output", @".\Release\" + fn + ".elf" }, true);
                    writeXML(new string[] { "Option", "object_output", @".\Release" }, true);
                    writeXML(new string[] { "Option", "type", "0" }, true);
                    writeXML(new string[] { "Option", "create_hex", "1" }, true);
                    writeXML(new string[] { "Option", "compiler", "armgcc_eb" }, true);
                    writeXML(new string[] { "Option", "projectDeviceOptionsRelation", "0" }, true);

                    xw.WriteStartElement("Compiler");
                    writeXML(new string[] { "Add", "option", "-fdata-sections" }, true);
                    writeXML(new string[] { "Add", "option", "-ffunction-sections" }, true);
                    writeXML(new string[] { "Add", "option", "-O2" }, true);
                    writeXML(new string[] { "Add", "option", "-g2" }, true);
                    writeXML(new string[] { "Add", "option", "-DUSE_HAL_DRIVER" }, true);
                    writeXML(new string[] { "Add", "option", "-DSTM32F407xx" }, true);
                    xw.WriteEndElement();

                    xw.WriteStartElement("Assembler");
                    writeXML(new string[] { "Add", "option", "-Wa,--no-warn" }, true);
                    xw.WriteEndElement();

                    xw.WriteStartElement("Linker");
                    writeXML(new string[] { "Add", "option", "-Wl,--gc-sections" }, true);
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    xw.WriteStartElement("Device");
                    writeXML(new string[] { "Add", "option", "$device=cortex-m4" }, true);
                    writeXML(new string[] { "Add", "option", "$fpu=fpv4-sp-d16" }, true);
                    writeXML(new string[] { "Add", "option", @"$lscript=.\STM32F407VG_FLASH.ld" }, true);
                    writeXML(new string[] { "Add", "option", "$stack=0x0100" }, true);
                    writeXML(new string[] { "Add", "option", "$heap=0x0000" }, true);
                    xw.WriteEndElement();

                    xw.WriteStartElement("Compiler");
                    writeXML(new string[] { "Add", "option", "-mfloat-abi=hard" }, true);
                    writeXML(new string[] { "Add", "option", "-DARM_MATH_CM4" }, true);
                    writeXML(new string[] { "Add", "option", "-D__FPU_USED" }, true);
                    writeXML(new string[] { "Add", "option", "-DSTM32F407VG" }, true);
                    writeXML(new string[] { "Add", "option", "-DSTM32F4XX" }, true);
                    writeXML(new string[] { "Add", "option", "-DUSE_STDPERIPH_DRIVER" }, true);
                    writeXML(new string[] { "Add", "option", "-fno-strict-aliasing" }, true);
                    writeXML(new string[] { "Add", "directory", @".\Drivers\CMSIS\Device\ST\STM32F4xx\Include" }, true);
                    writeXML(new string[] { "Add", "directory", @".\Drivers\CMSIS\Include" }, true);
                    writeXML(new string[] { "Add", "directory", @".\Drivers\STM32F4xx_HAL_Driver\Inc" }, true);
                    writeXML(new string[] { "Add", "directory", @".\Inc" }, true);
                    xw.WriteEndElement();

                    xw.WriteStartElement("Linker");
                    writeXML(new string[] { "Add", "option", "-eb_start_files" }, true);
                    writeXML(new string[] { "Add", "option", "-eb_lib=n" }, true);
                    xw.WriteEndElement();

                    string tmp = readSrcFilePaths(pt + "\\.mxproject");
                    if (tmp != "OK")
                        MessageBox.Show(tmp);

                    xw.WriteStartElement("Extensions");
                    xw.WriteStartElement("code_completion");
                    xw.WriteEndElement();

                    xw.WriteStartElement("debugger");
                    writeXML(new string[] { "target_debugging_settings", "target", "Debug", "active_interface", "ST-link" }, false);
                    writeXML(new string[] { "debug_interface", "interface_id", "ST-link", "ip_address", "localhost", "ip_port", "4242", "path", @"${EMBLOCKS}\share\contrib", "executable", "STLinkGDB.exe", "description", "", "dont_start_server", "false", "backoff_time", "1000", "options", "6", "active_family", "STMicroelectronics" }, false);
                    writeXML(new string[] { "family_options", "family_id", "STMicroelectronics" }, false);
                    writeXML(new string[] { "option", "opt_id", "ID_JTAG_SWD", "opt_value", "swd" }, true);
                    writeXML(new string[] { "option", "opt_id", "ID_VECTOR_START", "opt_value", "0x08000000" }, true);
                    writeXML(new string[] { "option", "opt_id", "ID_RESET_TYPE", "opt_value", "System" }, true);
                    writeXML(new string[] { "option", "opt_id", "ID_LOAD_PROGRAM", "opt_value", "1" }, true);
                    writeXML(new string[] { "option", "opt_id", "ID_SEMIHOST_CHECK", "opt_value", "0" }, true);
                    writeXML(new string[] { "option", "opt_id", "ID_RAM_EXEC", "opt_value", "0" }, true);
                    writeXML(new string[] { "option", "opt_id", "ID_VEC_TABLE", "opt_value", "1" }, true);
                    writeXML(new string[] { "option", "opt_id", "ID_DONT_CONN_RESET", "opt_value", "0" }, true);
                    writeXML(new string[] { "option", "opt_id", "ID_ALL_MODE_DEBUG", "opt_value", "0" }, true);
                    writeXML(new string[] { "option", "opt_id", "ID_DEV_ADDR", "opt_value", "" }, true);
                    writeXML(new string[] { "option", "opt_id", "ID_VERBOSE_LEVEL", "opt_value", "3" }, true);
                    xw.WriteEndElement();

                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    xw.WriteStartElement("envvars");

                    xw.WriteEndDocument();
                    xw.Flush();
                }
                finally
                {
                    if (xw != null)
                        xw.Close();
                }
            }
            Application.Exit();
        }



    }
}
