using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using ShellLink;
using IWshRuntimeLibrary;
// required for the icon extractor code
using TsudaKageyu;

namespace MenuApp
{
    public partial class Menu : Form
    {
        // used to extract the data from a shortcut
        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath,
                                                    uint dwFileAttributes,
                                                    ref SHFILEINFO psfi,
                                                    uint cbSizeFileInfo,
                                                    uint uFlags
                                                  );

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;

        // contains any extracted icons
        Icon[] splitIcons = null;
        int iconCount = 0;

        // required to etract the icon from a file
        [DllImport("shell32.dll", EntryPoint = "ExtractIcon")]
        extern static IntPtr ExtractIcon(IntPtr hInst,
                                          string lpszExeFileName,
                                          int nIconIndex
                                        );

        //Struct used by SHGetFileInfo function
        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        // used to retrieve the contents of an internet shortcut which is a disguised INI file
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        // used to retrieve the registered application for a file
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern uint AssocQueryString(
                                                    AssocF flags,
                                                    AssocStr str,
                                                    string pszAssoc,
                                                    string pszExtra,
                                                    [Out] StringBuilder pszOut,
                                                    ref uint pcchOut
                                                  );

        [Flags]
        public enum AssocF
        {
            None = 0,
            Init_NoRemapCLSID = 0x1,
            Init_ByExeName = 0x2,
            Open_ByExeName = 0x2,
            Init_DefaultToStar = 0x4,
            Init_DefaultToFolder = 0x8,
            NoUserSettings = 0x10,
            NoTruncate = 0x20,
            Verify = 0x40,
            RemapRunDll = 0x80,
            NoFixUps = 0x100,
            IgnoreBaseClass = 0x200,
            Init_IgnoreUnknown = 0x400,
            Init_Fixed_ProgId = 0x800,
            Is_Protocol = 0x1000,
            Init_For_File = 0x2000
        }

        public enum AssocStr
        {
            Command = 1,
            Executable,
            FriendlyDocName,
            FriendlyAppName,
            NoOpen,
            ShellNewValue,
            DDECommand,
            DDEIfExec,
            DDEApplication,
            DDETopic,
            InfoTip,
            QuickTip,
            TileInfo,
            ContentType,
            DefaultIcon,
            ShellExtension,
            DropTarget,
            DelegateExecute,
            Supported_Uri_Protocols,
            ProgID,
            AppID,
            AppPublisher,
            AppIconReference,
            Max
        }


        // this is a holder over for the original source code
        string IconFolder = System.AppDomain.CurrentDomain.BaseDirectory + @"Icons";

        public Menu()
        {
            InitializeComponent();
        }

        public void Menu_Load(object sender, EventArgs e)
        {
            // start up in the last location the form was on the screen
            this.Location = Properties.Settings.Default.MenuPosition;
            this.Location = Properties.Settings.Default.MenuPosition;
            if (!Directory.Exists(IconFolder))
            {
                Directory.CreateDirectory(IconFolder);
            }

            // initialize this to allow dropping files
            DropHere.AllowDrop = true;

        }


        private void Menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            //save the position of the form for when its opened again
            Properties.Settings.Default.MenuPosition = this.Location;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="FileName">The path to an image file to resize.</param>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(string FileName, int width, int height)
        {
            return ResizeImage(Image.FromFile(FileName), width, height);
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Process any dropped files.  
        /// Note: we only use the first dropped file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //https://stackoverflow.com/questions/57156405/drag-and-drop-a-windows-desktop-shortcut-to-a-listview
        private void item_DragDrop(object sender, DragEventArgs e)
        {
            ClearItem();

            //
            if (e.Data.GetDataPresent(DataFormats.FileDrop) &&
                e.Data.GetData(DataFormats.FileDrop, true) is string[] filePaths)
            {
                // we only want the first of the dropped shortcuts
                string filePath = filePaths[0];

                FileInfo lnk = new FileInfo(filePath);
                if (lnk.Extension.ToLower() == ".exe")
                {
                    LoadAnExe(filePath);
                }
                else if (lnk.Extension.ToLower() == ".url")
                {
                    LoadAURL(filePath, (TextBox)sender);
                }
                else if (lnk.Extension.ToLower() == ".lnk")
                {
                    LoadAShortcut(filePath, (TextBox)sender);
                }
                else
                {
                    LoadAFile(filePath, (TextBox)sender);
                }
            }
        }

        /// <summary>
        /// Extract the information from an executable file
        /// </summary>
        /// <param name="filePath"> Fully qualified name of the file</param>
        /// <param name="entry"> </param>
        private void LoadAnExe(string filePath, TextBox entry)
        {
            // we have an internet shortcut
            FileInfo lnk = new FileInfo(filePath);
            MenuEntry thisEntry = new MenuEntry();
            thisEntry.TargetOriginal = filePath;
            thisEntry.Target = filePath;
            thisEntry.Arguments = "";
            thisEntry.IconPath = "";
            thisEntry.IconIndex = 0;
            thisEntry.PanelName = entry.Name;
            thisEntry.StartIn = lnk.DirectoryName;

            string iconfilename = IconFolder + @"\" + lnk.Name.Substring(0, lnk.Name.Length - lnk.Extension.Length) + ".ico";
            IconExtractor extractor = new IconExtractor(filePath);
            Icon icon = extractor.GetIcon(thisEntry.IconIndex);

            splitIcons = IconUtil.Split(icon);
            iconCount = splitIcons.GetUpperBound(0);

            Image1.Image = splitIcons[0].ToBitmap();
            IconNumber.Text = "0";

            thisEntry.IconPath = iconfilename;

            SetItem(thisEntry);
        }

        private void LoadAURL(string filePath, TextBox entry)
        {

            string LinkTarget = "";
            string LinkArguments = "";
            string LinkIconLocation = "";
            string LinkWorkingDirectory = "";
            int LinkIconIndex = 0;
            string LinkDescription = "";


            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString("InternetShortcut", "URL", "", temp, 255, filePath);
            LinkTarget = temp.ToString();
            i = GetPrivateProfileString("InternetShortcut", "IconFile", "", temp, 255, filePath);
            LinkIconLocation = temp.ToString();
            i = GetPrivateProfileString("InternetShortcut", "IconIndex", "", temp, 255, filePath);
            LinkIconIndex = Convert.ToInt32(temp.ToString());

            MenuEntry thisEntry = new MenuEntry();
            thisEntry.TargetOriginal = LinkTarget;
            thisEntry.Target = LinkTarget;
            thisEntry.Arguments = LinkArguments;
            thisEntry.IconPathOriginal = LinkIconLocation;
            thisEntry.IconIndex = LinkIconIndex;
            thisEntry.StartIn = LinkWorkingDirectory;
            thisEntry.Description = LinkDescription;

            FileInfo lnk = new FileInfo(filePath);
            //            string iconfilename = IconFolder + @"\" + LinkIconLocation.Substring(LinkIconLocation.LastIndexOf("/") + 1);

            string iconfilename = IconFolder + @"\" + lnk.Name.Substring(0, lnk.Name.Length - lnk.Extension.Length) + ".ico";

            var client = new System.Net.WebClient();
            client.DownloadFile(LinkIconLocation, iconfilename);

            Icon icon = new Icon(iconfilename);

            splitIcons = IconUtil.Split(icon);
            iconCount = splitIcons.GetUpperBound(0);

            Image1.Image = splitIcons[0].ToBitmap();
            IconNumber.Text = "0";
            IconSize.Text = splitIcons[0].Width.ToString() + "x" + splitIcons[0].Height.ToString();
            SetItem(thisEntry);
        }

        private void LoadAShortcut(string filePath, TextBox entry)
        {
            FileInfo lnk = new FileInfo(filePath);
            try
            {
                //MessageBox.Show(linkInfo.ToString());
                //string LinkTarget = Environment.ExpandEnvironmentVariables(((linkInfo.LinkTargetIDList == null || linkInfo.LinkTargetIDList.Path == null) ? linkInfo.StringData.NameString : linkInfo.LinkTargetIDList.Path)).Replace("@", "");
                string LinkTarget = "";
                string LinkArguments = "";
                string LinkIconLocation = "";
                string LinkWorkingDirectory = "";
                int LinkIconIndex = 0;
                string LinkDescription = "";

                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(filePath);

                LinkTarget = Environment.ExpandEnvironmentVariables(shortcut.TargetPath);
                LinkArguments = shortcut.Arguments;
                LinkIconLocation = Environment.ExpandEnvironmentVariables(shortcut.IconLocation);
                LinkWorkingDirectory = Environment.ExpandEnvironmentVariables(shortcut.WorkingDirectory);
                if (LinkIconLocation.Contains(","))
                {
                    LinkIconIndex = Convert.ToInt32(LinkIconLocation.Substring(LinkIconLocation.LastIndexOf(",") + 1));
                }
                else
                if (LinkTarget.Contains(","))
                {
                    LinkIconIndex = Convert.ToInt32(LinkTarget.Substring(LinkTarget.LastIndexOf(",") + 1));
                }
                LinkDescription = shortcut.Description;

                MenuEntry thisEntry = new MenuEntry();
                thisEntry.TargetOriginal = LinkTarget;
                thisEntry.Target = LinkTarget;
                if (LinkTarget.IndexOf(",") > 0)
                {
                    thisEntry.Target = LinkTarget.Substring(0, LinkTarget.LastIndexOf(","));
                }
                thisEntry.Arguments = LinkArguments;
                thisEntry.IconPathOriginal = LinkIconLocation;
                thisEntry.IconPath = LinkIconLocation;
                if (LinkIconLocation.IndexOf(",") > 0)
                {
                    thisEntry.IconPath = LinkIconLocation.Substring(0, LinkIconLocation.LastIndexOf(","));
                }
                else
                    if (LinkIconLocation.IndexOf(",") == 0)
                {
                    thisEntry.IconPath = thisEntry.Target;
                }
                thisEntry.IconIndex = LinkIconIndex;
                thisEntry.PanelName = entry.Name;
                thisEntry.StartIn = LinkWorkingDirectory;
                thisEntry.Description = LinkDescription;

                /*
                 * 
                 * If LinkIconPath == LinkTarget
                 *      Get the first icon 
                 * If LinkIconIndex == 0
                 *      Get the first icon
                 * else
                 *      Get the correct icon
                 */

                /*Icon can be 
                 *  ico
                 *  icl indexed
                 *  exe indexed?
                 *  dll indexed?
                 * 
                 * 
                 * 
                 * */

                //if the iconpath exists then work from that
                if (System.IO.File.Exists(thisEntry.IconPath))
                {
                    FileInfo f = new FileInfo(thisEntry.IconPath);
                    if ((f.Extension.ToLower() == ".png"))
                    {
                        Image1.Image = Image.FromFile(thisEntry.IconPath);
                    }
                    else
                    if ((f.Extension.ToLower() == ".ico"))
                    {
                        Image1.Image = Image.FromFile(thisEntry.IconPath);
                    }
                    else
                    if ((f.Extension.ToLower() == ".exe") || (f.Extension.ToLower() == ".dll"))
                    {
                        IconExtractor extractor = new IconExtractor(thisEntry.IconPath);
                        Icon icon = extractor.GetIcon(thisEntry.IconIndex);

                        if (icon != null)
                        {
                            splitIcons = IconUtil.Split(icon);
                            iconCount = splitIcons.GetUpperBound(0);

                            //Icon ico = Icon.ExtractAssociatedIcon(filePath);
                            Image1.Image = splitIcons[0].ToBitmap();
                            IconNumber.Text = "0";
                        }
                    }
                    else
                    //other files like icl
                    {
                        IconExtractor extractor = new IconExtractor(thisEntry.IconPath);
                        Icon icon = extractor.GetIcon(thisEntry.IconIndex);

                        if (icon != null)
                        {
                            splitIcons = IconUtil.Split(icon);
                            iconCount = splitIcons.GetUpperBound(0);

                            //Icon ico = Icon.ExtractAssociatedIcon(filePath);
                            Image1.Image = splitIcons[0].ToBitmap();
                            IconNumber.Text = "0";
                        }
                    }
                }
                else
                {
                    // the filename in the iconpath does not exist
                    // or there is no value in the iconpath
                    FileInfo f = new FileInfo(thisEntry.Target);
                    if (Directory.Exists(thisEntry.Target))
                    {
                        //we have a target that is a folder
                        //if the target is a folder and no iconfile is specified just make it the standard folder icon
                        string iconfilename = IconFolder + @"\" + f.Name.Substring(0, f.Name.Length - f.Extension.Length) + ".ico";
                        Icon ico = ExtractFromPath(thisEntry.Target);
                        Image1.Image = ico.ToBitmap();
                    }
                    else
                    {
                        if (System.IO.File.Exists(thisEntry.Target))
                        {
                            //We have a target that is a file of some type
                            string iconfilename = IconFolder + @"\" + f.Name.Substring(0, f.Name.Length - f.Extension.Length) + ".ico";
                            IconExtractor extractor = new IconExtractor(filePath);
                            Icon icon = extractor.GetIcon(thisEntry.IconIndex);

                            if (icon != null)
                            {
                                splitIcons = IconUtil.Split(icon);
                                iconCount = splitIcons.GetUpperBound(0);

                                //Icon ico = Icon.ExtractAssociatedIcon(filePath);
                                Image1.Image = splitIcons[0].ToBitmap();
                                IconNumber.Text = "0";
                            }
                            else
                            {
                                Image1.Image = null;
                                IconNumber.Text = "0";
                            }
                        }
                        else
                        {
                            MessageBox.Show("Link Target does not exist", "Fiel doesn't exist", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }

                SetItem(thisEntry);
            }
            catch (Exception ex)
            {
                // Target cannot be processed
                MessageBox.Show("Link " + filePath + "caused an error - " + ex.Message, "Link Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAFile(string filePath, TextBox entry)
        {
            // we have an internet shortcut
            FileInfo lnk = new FileInfo(filePath);
            MenuEntry thisEntry = new MenuEntry();
            thisEntry.TargetOriginal = "";
            thisEntry.Target = "";
            thisEntry.Arguments = filePath;
            thisEntry.IconPath = "";
            thisEntry.IconIndex = 0;
            thisEntry.PanelName = entry.Name;
            thisEntry.StartIn = lnk.DirectoryName;
            thisEntry.Description = "FILE";

            string iconfilename = IconFolder + @"\" + lnk.Name.Substring(0, lnk.Name.Length - lnk.Extension.Length) + ".ico";

            String programName = AssocQueryString(AssocStr.Executable, lnk.Extension);

            Image1.Image = null;
            if (programName != null)
            {
                IconExtractor extractor = new IconExtractor(programName.ToString());
                Icon icon = extractor.GetIcon(thisEntry.IconIndex);

                splitIcons = IconUtil.Split(icon);
                iconCount = splitIcons.GetUpperBound(0);

                Image1.Image = splitIcons[0].ToBitmap();
            }
            IconNumber.Text = "0";

            thisEntry.IconPath = iconfilename;

            SetItem(thisEntry);
        }

        /// <summary>
        /// Get an icon from a file
        /// </summary>
        /// <param name="path">The name of the icon file</param>
        /// <returns>An Icon object</returns>
        private static Icon ExtractFromPath(string path)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            SHGetFileInfo(
                path,
                0, ref shinfo, (uint)Marshal.SizeOf(shinfo),
                SHGFI_ICON | SHGFI_LARGEICON);
            return System.Drawing.Icon.FromHandle(shinfo.hIcon);
        }

        /// <summary>
        /// Gets the icon from the file
        /// </summary>
        /// <param name="iconFile">The file to extract the icon from</param>
        /// <param name="index">The index of the icon in the file</param>
        /// <returns>An Icon object</returns>
        static private Icon GetIcon(string iconFile, int index)
        {
            try
            {
                IntPtr Hicon = ExtractIcon(IntPtr.Zero, iconFile, index);
                Icon icon = Icon.FromHandle(Hicon);
                return icon;
            }
            catch
            {
                MessageBox.Show("Executable contains no icons", "File has no icons", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }

        private void item_DragEnter(object sender, DragEventArgs e)
        {
            //
        }

        private void item_DragOver(object sender, DragEventArgs e)
        {
            //
            // This checks that each file being dragged over is a .lnk file.
            // If it is not, it will show the invalid cursor thanks to some
            // e.Effect being set to none by default.
            bool dropEnabled = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, true) &&
                    e.Data.GetData(DataFormats.FileDrop, true) is string[] filePaths &&
                    ((filePaths.Any(filePath => Path.GetExtension(filePath)?.ToLowerInvariant() == ".xxxx"))))
                {
                    dropEnabled = false;
                }
            }
            else
            {
                dropEnabled = false;
            }

            if (dropEnabled)
            {
                // Set the effect to copy so we can drop the item
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void SetItem(MenuEntry thisEntry)
        {
            LinkTarget.Text = thisEntry.Target;
            LinkArgument.Text = thisEntry.Arguments;
            LinkStartIn.Text = thisEntry.StartIn;
            LinkIconPath.Text = thisEntry.IconPath;
            LinkDescription.Text = thisEntry.Description;
        }
        private void ClearItem()
        {
            LinkTarget.Text = "";
            LinkArgument.Text = "";
            LinkStartIn.Text = "";
            LinkIconPath.Text = "";
            LinkDescription.Text = "";
            Image1.Image = null;
        }

        private void Prev_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(IconNumber.Text) > 0)
            {
                IconNumber.Text = (Convert.ToInt32(IconNumber.Text) - 1).ToString();
                Image1.Image = splitIcons[Convert.ToInt32(IconNumber.Text)].ToBitmap();
                IconSize.Text = splitIcons[Convert.ToInt32(IconNumber.Text)].Width.ToString() + "x" + splitIcons[Convert.ToInt32(IconNumber.Text)].Height.ToString();
            }
        }

        private void Next_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(IconNumber.Text) < iconCount)
            {
                IconNumber.Text = (Convert.ToInt32(IconNumber.Text) + 1).ToString();
                Image1.Image = splitIcons[Convert.ToInt32(IconNumber.Text)].ToBitmap();
                IconSize.Text = splitIcons[Convert.ToInt32(IconNumber.Text)].Width.ToString() + "x" + splitIcons[Convert.ToInt32(IconNumber.Text)].Height.ToString();
            }
        }

        static string AssocQueryString(AssocStr association, string extension)
        {
            const int S_OK = 0;
            const int S_FALSE = 1;

            uint length = 0;
            uint ret = AssocQueryString(AssocF.None, association, extension, null, null, ref length);
            if (ret != S_FALSE)
            {
                //throw new InvalidOperationException("Could not determine associated string");
                return null;
            }

            var sb = new StringBuilder((int)length); // (length-1) will probably work too as the marshaller adds null termination
            ret = AssocQueryString(AssocF.None, association, extension, null, sb, ref length);
            if (ret != S_OK)
            {
                //throw new InvalidOperationException("Could not determine associated string");
                return null;
            }

            return sb.ToString();
        }
    }
}
