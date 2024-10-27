# Information

Hello, this is my first project here in GitHub. So everything is probably not perfect yet...

# What is NetUse?

NetUse is a Windows program that allows you to create and disconnect Windows network shares. The focus is on the console integration by other programs, e.g. backup tools or batch scripts - without having to fiddle with the various parameters.

The idea for the program came to me when I was trying to create network connections for a backup tool using batch scripts (_net use_). However, the required user passwords contained special characters that were invalid for the batch scripts.

## The NetUse program consists of 2 components:

* **NetUse.exe** This program is a pure CLI program. It is intended to be used by other programs such as backup tools, batch scripts, Powershell scripts, etc. to be called. Only one parameter is passed to the CLI program: a path to a configuration file. All relevant settings for establishing the network connection using _net use_ are stored in the configuration file.

* **NetUseGui.exe** This program - it has a user interface - can be used to create or modify the configuration files. 
In addition, the configuration files with the file extension _.netcfg_ can be registered under Windows, so that a simple double-click on such a file in Explorer executes the desired network connection function.

# Status
The program is so far executable and _almost_ completed.
The only 2 improvements I can think of are an **encryption** of the passwords and a **selection list** of the recently opened configuration files "Recently Opened Files".

# Licensing
GNU Affero General Public License v3.0
