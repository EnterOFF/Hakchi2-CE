﻿using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class TechInfo : Form
    {
        public TechInfo()
        {
            InitializeComponent();
        }

        private void TechInfo_Load(object sender, EventArgs e)
        {
            string devTools = "";
            if (ConfigIni.Instance.ForceClovershell) devTools += "Force Clovershell, ";
            if (ConfigIni.Instance.ForceSSHTransfers) devTools += "Force SSH Transfers, ";
            if (ConfigIni.Instance.UploadToTmp) devTools += "Upload to /tmp, ";
            if (ConfigIni.Instance.DisableClovershellListener) devTools += "Disable Clovershell listener, ";
            if (ConfigIni.Instance.DisableSSHListener) devTools += "Disable SSH listener, ";
            devTools = devTools.TrimEnd(new char[] { ',', ' ' });
            if (string.IsNullOrWhiteSpace(devTools)) devTools = "None";

            var gamesSize = Shared.DirectorySize(Path.Combine(Program.BaseDirectoryExternal, "games"));
            var gamesNum = Directory.EnumerateDirectories(Path.Combine(Program.BaseDirectoryExternal, "games")).Count();
            var gamesSnesSize = Shared.DirectorySize(Path.Combine(Program.BaseDirectoryExternal, "games_snes"));
            var gamesSnesNum = Directory.EnumerateDirectories(Path.Combine(Program.BaseDirectoryExternal, "games_snes")).Count();
            var gamesCacheSize = Shared.DirectorySize(Path.Combine(Program.BaseDirectoryExternal, "games_cache"));
            var gamesCacheNum = Directory.EnumerateDirectories(Path.Combine(Program.BaseDirectoryExternal, "games_cache")).Count();

            listView1.Items.AddRange(new ListViewItem[] {
                // general info
                new ListViewItem(new string[] { "hakchi2 CE version:", Shared.AppDisplayVersion }, listView1.Groups[0]),
                new ListViewItem(new string[] { "Portable mode:", Program.isPortable ? Resources.Yes : Resources.No }, listView1.Groups[0]),
                new ListViewItem(new string[] { "Developer tools:", devTools }, listView1.Groups[0]),
                new ListViewItem(new string[] { "Internal path:", Program.BaseDirectoryInternal }, listView1.Groups[0]),
                new ListViewItem(new string[] { "External path:", Program.BaseDirectoryExternal }, listView1.Groups[0]),
                new ListViewItem(new string[] { "/games:", $"{gamesNum} directories (" + Shared.SizeSuffix(gamesSize) + ")" }, listView1.Groups[0]),
                new ListViewItem(new string[] { "/games_snes:", $"{gamesSnesNum} directories (" + Shared.SizeSuffix(gamesSnesSize) + ")" }, listView1.Groups[0]),
                new ListViewItem(new string[] { "/games_cache", $"{gamesCacheNum} directories (" + Shared.SizeSuffix(gamesCacheSize) + ")" }, listView1.Groups[0]),

                // settings
                new ListViewItem(new string[] { "Separate games for multiboot:", ConfigIni.Instance.SeparateGameStorage ? Resources.Yes : Resources.No }, listView1.Groups[6]),
                new ListViewItem(new string[] { "Use linked sync:", ConfigIni.Instance.SyncLinked ? Resources.Yes : Resources.No }, listView1.Groups[6]),
            });

            string shell = Resources.Unknown;
            string connected = Resources.No;
            string minimalMemboot = Resources.No;
            if (hakchi.Connected)
            {
                // shell
                connected = Resources.Yes;
                if (hakchi.Shell is INetworkShell)
                    shell = "SSH";
                else if (hakchi.Shell is clovershell.ClovershellConnection)
                    shell = "Clovershell";

                // shell info
                listView1.Items.AddRange(new ListViewItem[] {
                    new ListViewItem(new string[] { "Connected:", connected }, listView1.Groups[1]),
                    new ListViewItem(new string[] { "Shell:", shell }, listView1.Groups[1]),
                    new ListViewItem(new string[] { "Can interact:", hakchi.CanInteract ? Resources.Yes : Resources.No }, listView1.Groups[1]),
                    new ListViewItem(new string[] { "Minimal memboot:", hakchi.MinimalMemboot ? Resources.Yes : Resources.No }, listView1.Groups[1]),
                });

                if (hakchi.MinimalMemboot)
                {
                    // no-op
                }
                if (hakchi.CanInteract)
                {
                    listView1.Items.AddRange(new ListViewItem[] {
                        // more shell info
                        new ListViewItem(new string[] { "Detected console type:", MainForm.GetConsoleTypeName(hakchi.DetectedConsoleType) }, listView1.Groups[1]),
                        new ListViewItem(new string[] { "Custom firmware:", hakchi.CustomFirmwareLoaded ? Resources.Yes : Resources.No }, listView1.Groups[1]),
                        new ListViewItem(new string[] { "Console unique ID:", hakchi.UniqueID }, listView1.Groups[1]),

                        // hakchi info
                        new ListViewItem(new string[] { "Boot version:", hakchi.BootVersion }, listView1.Groups[2]),
                        new ListViewItem(new string[] { "Kernel version:", hakchi.KernelVersion }, listView1.Groups[2]),
                        new ListViewItem(new string[] { "Script version:", hakchi.ScriptVersion }, listView1.Groups[2]),

                        // paths
                        new ListViewItem(new string[] { "Config:", hakchi.ConfigPath }, listView1.Groups[3]),
                        new ListViewItem(new string[] { "Remote sync:", hakchi.RemoteGameSyncPath }, listView1.Groups[3]),
                        new ListViewItem(new string[] { "System code:", hakchi.SystemCode ?? "-" }, listView1.Groups[3]),
                        new ListViewItem(new string[] { "Media:", hakchi.MediaPath }, listView1.Groups[3]),
                        new ListViewItem(new string[] { "Original games:", hakchi.OriginalGamesPath }, listView1.Groups[3]),
                        new ListViewItem(new string[] { "Games:", hakchi.GamesPath }, listView1.Groups[3]),
                        new ListViewItem(new string[] { "RootFS:", hakchi.RootFsPath }, listView1.Groups[3]),
                        new ListViewItem(new string[] { "Profile:", hakchi.GamesProfilePath }, listView1.Groups[3]),
                        new ListViewItem(new string[] { "SquashFS:", hakchi.SquashFsPath }, listView1.Groups[3]),

                        // memory stats
                        new ListViewItem(new string[] { "Storage total:", Shared.SizeSuffix(MemoryStats.StorageTotal) }, listView1.Groups[4]),
                        new ListViewItem(new string[] { "Storage used:", Shared.SizeSuffix(MemoryStats.StorageUsed) }, listView1.Groups[4]),
                        new ListViewItem(new string[] { "Storage free:", Shared.SizeSuffix(MemoryStats.StorageFree) }, listView1.Groups[4]),
                        new ListViewItem(new string[] { "External saves:", MemoryStats.ExternalSaves ? Resources.Yes : Resources.No }, listView1.Groups[4]),
                        new ListViewItem(new string[] { "Saves:", Shared.SizeSuffix(MemoryStats.SaveStatesSize) }, listView1.Groups[4]),
                        new ListViewItem(new string[] { "All games:", Shared.SizeSuffix(MemoryStats.AllGamesSize) }, listView1.Groups[4]),
                        new ListViewItem(new string[] { "Non multiboot games:", Shared.SizeSuffix(MemoryStats.NonMultibootGamesSize) }, listView1.Groups[4]),
                        new ListViewItem(new string[] { "Extra files:", Shared.SizeSuffix(MemoryStats.ExtraFilesSize) }, listView1.Groups[4]),

                        // console settings
                        new ListViewItem(new string[] { "USB host enabled", ConfigIni.Instance.UsbHost ? Resources.Yes : Resources.No }, listView1.Groups[5]),
                        new ListViewItem(new string[] { "Fontfix enabled", ConfigIni.Instance.UseFont ? Resources.Yes : Resources.No }, listView1.Groups[5]),
                    });

                    // collections sizes
                    foreach (var pair in MemoryStats.Collections)
                    {
                        listView1.Items.Add(
                            new ListViewItem(new string[] { MainForm.GetConsoleTypeName(pair.Key) + " games:", Shared.SizeSuffix(pair.Value) }, listView1.Groups[4]));
                    }
                }
            }
            else
            {
                listView1.Items.AddRange(new ListViewItem[] {
                    new ListViewItem(new string[] { "Connected:", connected }, listView1.Groups[1]),
                });
            }

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (var writer = new StreamWriter(File.OpenWrite(saveFileDialog1.FileName)))
                {
                    listView1.Items.Cast<ListViewItem>().ToList().ForEach(item => { writer.WriteLine(item.SubItems[0].Text + " " + item.SubItems[1].Text); });
                }
                if (!ConfigIni.Instance.DisablePopups)
                    Tasks.MessageForm.Show(this, Resources.Wow, Resources.Done, Resources.sign_check);
            }
        }

        private void TechInfo_Resize(object sender, EventArgs e)
        {
            listView1.Columns[1].Width = -2;
        }

        private void TechInfo_Shown(object sender, EventArgs e)
        {
            listView1.Columns[0].Width = -2;
            listView1.Columns[1].Width = -2;
        }
    }
}