# Information
Hello, this is my first project here in GitHub. So everything is probably not perfect yet...

# What is NetUse?
NetUse is a Windows program that allows you to create and disconnect Windows network shares. The focus is on the console integration by other programs, e.g. backup tools or batch scripts - without having to fiddle with the various parameters.

The idea for the program came to me when I was trying to create network connections for a backup tool using batch scripts with `net use`.
Since I have to use special characters -for the passwords for the network shares - which are ivalid for batch scripts, I couldn't used batch scripts and `net use`.

## The NetUse program consists of 2 components:

* **NetUse.exe** This program is a pure CLI program. It is intended to be used by other programs such as backup tools, batch scripts, Powershell scripts, etc. to be called. Only one parameter is passed to the CLI program: a path to a configuration file. All relevant settings for establishing the network connection using **net use** are stored in the configuration file.

* **NetUseGui.exe** This program - it has a user interface - can be used to create or modify the configuration files. 
In addition, the configuration files with the file extension `.netcfg` can be registered under Windows, so that a simple double-click on such a file in Explorer executes the desired network connection function.

> [!WARNING]
> The stored network connections password in the configuration files is not ecrypted ye!

---

# Development status
The program is so far executable and _almost_ completed.

However, the following points are conceivable:
- [ ] Implement an **encryption** to protect the passwords stored in the configuration files
- [ ] Add a **selection list** of the recently opened configuration files "Recently Opened Files"
- [ ] Change the default directory of the configuration files to users `AppData` folder
- [ ] Add an **installer** to install the app

# Getting Started
The project was created using Visual Studio 2022. The language used is C# and the framework is .NET Framework 4.8.

## Installation

Copy the files
- NetUse.Common.dll
- NetUse.Core.dll
- NetUse.NetConfigFile.dll
- NetUseGui.exe
- NetUse.exe

to a new directory on your computer and then run `NetUseGui.exe` to create and modify your first configuration files.

> [!TIP]
> Create a shortcut to `NetUseGui.exe` on your desktop, so you can easily launch the program later.

> [!TIP]
> In the program, select the menu item *Tools / Register file extensions* to register the configuration files in Windows. This allows you to establish the network connection immediately by double-clicking the configuration file in **Explorer**.

> [!Note]
> Do not create and copy the files to a folder located in *Program Files* or *Program Files (x86)*: Configuration files cannot be saved in these directories later! This is a Windows security measure.

> [!IMPORTANT]
> If multiple users are working on the computer, the following should be noted: By default, the configuration files are stored in the folder containing `NetUseGui.exe`. This means that all users who have access to the folder containing `NetUseGui.exe` can access the configuration files and establish the corresponding network connections.
If that is not desired, `NetUseGui.exe` and the other files must be copied to a folder accessible only by selected users (Keyword *File and Folder permissions*).

## Usage

You can use the program to establish a network connection in two ways:
* By double-clicking the configuration file in **Explorer**
* By integrating it into **third-party software**, such as backup software.

### Using by **Explorer**
To use these, you must execute first a Windows file extension registration for the configuration files. In `NetUseGui.exe`, select the menu item *Tools / Register file extensions* to register the configuration files.
Then you can connect / disconnet a network connections by double-clicking the configuration file in **Explorer**.

> [!IMPORTANT]
> After file extension registration, `NetUseGui.exe` should not be renamed or moved to an other folder. In suce case, you have to rerun the registration.

### Using by **third-party software**
Example of a backup program: Typically, you can define / run a *Before* action. This is where we come in: Start / execute an external program with arguments - in this example, that's our app. The syntax for this is `<Path_To>NetUse.exe Config_File_Name`.

Eg:
- `"C:\Users\Backup User1\Desktop\NetUse\NetUse.exe" "Connet to Server A"`
- `"C:\Users\Backup User1\Desktop\NetUse\NetUse.exe" "Connet to Server A.netcfg"`
- `"C:\Users\Backup User1\Desktop\NetUse\NetUse.exe" "C:\My NetUse Configs\Connet to Server A.netcfg"`

> [!Note]
> If not path is present for the configuration file argument (first two examples), then `NetUse.exe` search in the same folder.

> [!NOTE]
> `NetUse.exe` returns an errorcode, which can be processed by the calling app.
  
---

# Licensing
GNU Affero General Public License v3.0
