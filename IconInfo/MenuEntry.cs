using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuApp
{
    [Serializable]
    
    public class MenuEntry
    {
        /// <summary>
        /// This object holds the extracted information about the file 
        /// </summary>
        private string _Target = "";
        private string _TargetOriginal = "";
        private string _Arguments = "";
        private string _StartIn = "";
        private string _IconPath = "";
        private string _IconPathOriginal = "";
        private string _Description = "";
        private int _IconIndex = 0;

        /*
         * The Target line of the file.  
         * If the file is
         *  - a shortcut - the target line of the shortcut
         *  - an Internet shortcut - the URL 
         *  - an exe file - the fully qualified name of the executable
         *  - any other file - the registerd application the file opens with or blank of there is no registered application
         *  
         */
        public string Target
        {
            get { return _Target; }
            set { _Target = value; }
        }

        /*
         * If the file is a shortcut there is the possiblity that there is a ",Digit" e.g. ",0" appended to it
         * This refers to the index of the image within the icon file.
         * TargetOriginal contains the original target line of a shortcut file including the icon index
         * 
         * The Target property does not include the icon index
         */
        public string TargetOriginal
        {
            get { return _TargetOriginal; }
            set { _TargetOriginal = value; }
        }

        /*
         * If the file is a shortcut this contains the contents of the Arguments line in the properties dialog of the shortcut
         */
        public string Arguments
        {
            get { return _Arguments; }
            set { _Arguments = value; }
        }
        /*
         * If the file is a shortcut this contains the contents of the Start In line in the properties dialog of the shortcut
         */
        public string StartIn
        {
            get { return _StartIn; }
            set { _StartIn = value; }
        }
        /*
         * If the file is a shortcut and the icon property is set this contains the contents of the Icon Path line in the properties dialog of the shortcut minus any icon index
         */
        public string IconPath
        {
            get { return _IconPath; }
            set { _IconPath = value; }
        }
        /*
         * If the file is a shortcut and the icon property is set this contains the contents of the Icon Path line in the properties dialog of the shortcut including any icon index
         */
        public string IconPathOriginal
        {
            get { return _IconPathOriginal; }
            set { _IconPathOriginal = value; }
        }
        /*
         * If the file is a shortcut and the icon property is set this contains the contents of the Description line in the properties dialog of the shortcut 
         */
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        /*
         * If the file is a shortcut and this contains the value of any iconindex appende to the target or icon values in the properties dialog of the shortcut
         * If there is no icon index found this will have a value of zero
         */
        public int IconIndex
        {
            get { return _IconIndex; }
            set { _IconIndex = value; }
        }

    }
}
