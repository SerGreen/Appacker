# Appacker
This tool takes a folder with application and packs it into a single executable file (without compression) for easy distribution, portability or whatnot.

## How does it work
The principle behind Appacker is that you can write whatever you want to the end of an `.exe` file and it will still work fine.  
This app simply appends all the necessary files to the end of a wrapper executable and when the wrapper is launched, it extracts all the files from its own `.exe`, places them into a temporary folder and starts packed application. When target app is closed, wrapper removes temp folder.  
  
Also you can set self-repack flag and Appacker will append to the wrapper executable the packer tool, that will repack target application when it's closed from the temporary folder and replace original package with and updated. This can be used to create portable application that saves any changes made to its files (like settings, that are stored inside application directory).  

## How to use Appacker
To pack an application you have to:  
1. Select folder that contains all the files of the target application.  
_You can drag'n'drop the folder to the first textBox or the treeView._
2. Select save location for the package (it must be an `.exe` file).  
_You can drag'n'drop an existing `.exe` into the second textBox; in this case it will be replaced._
3. Select the main executable file of the target application inside the treeView, that will display target application files after the step #1.
4. [Optional] Check the `self-repack` checkbox.
5. Press the `Pack!` button and it's done. It may take some time if the target application is big enough / has a lot of files.

<p align="center">
  <img src="Screenshots/2018-05-06_222416.png">
</p>

## What happens when you run packed application

<p align="center">
  <img src="Screenshots/appacker_demo.png">
</p>

Packed application is in fact an `unpacker.exe` with appended files. Application life cycle differs a bit for self-repackable and regular packages.  
For the example, let's take a `CoolApp.exe` package from the screenshot above and see what it does when launched.

#### If it's NOT a self-repacking application, CoolApp.exe does the following:
1. Creates a temporary folder for the target application files in `C:\Users\%UserName%\AppData\Local\Temp`
2. Extracts all the appended files from its own `exe` into the temp folder
3. Runs the main executable of the extracted app (for `CoolApp.exe` it's `Launcher.exe`) and waits for it to finish
4. Deletes the temporary folder and finishes

#### If it is a self-repacking application, CoolApp.exe does the following:
1. Creates a temporary folder A for the target application files in `C:\Users\%UserName%\AppData\Local\Temp`
2. Creates a temporary folder B for the `packer.exe` and `unpacker.exe` tools
3. Extracts `packer.exe` and `unpacker.exe` from its own `exe` into the temp folder B
4. Extracts all the appended files from its own `exe` into the temp folder A
5. Runs the main executable of the extracted app (for `CoolApp.exe` it's `Launcher.exe`) and waits for it to finish
6. Launches the `packer.exe` from temp folder B and finishes
7. Packer takes all the files from the temp folder A and packs them into a new package. New package replaces the original `CoolApp.exe`
8. Packer deletes the temp folder A
9. Packer launches new `CoolApp.exe` with the special flag `-killme` and asks it to delete the temp folder B (because `packer.exe` can't delete itself); packer finishes
10. New `CoolApp.exe` deletes temp folder B and finishes

## Solution structure
* `Appacker` &mdash; main application with GUI
* `Packer` &mdash; console tool that creates packages
* `Unpacker` &mdash; console wrapper that is used as a core of package
* `IconLib` &mdash; DLL project for saving windows `.ico` files (vanilla .NET can't save `ico` files correctly ¯\\\_(ツ)\_/¯ )
* `IconInjector` &mdash; DLL project for replacing icons of external executables
