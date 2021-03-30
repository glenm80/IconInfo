# IconInfo
This application is a testbed used to develop code that will extract some information from a file dropped on it.

The information extracted will depend on the file dropped.
If the file is a windows shortcut it will display the target, arguments, start in folder and the icon set for the shortcut.  
  If thee is no icon set it will extract the icon from the target of possible.

If the file is an executable it will extract the icon form the file.

If the file is an internet shortcut it will extract the icon if it has been set

Any other file it will determine the application used to open the file and extract the icon from that application if there is one
