# RAM Manager

An ultra-lightweight, transparent, and highly efficient RAM manager for Windows, specifically designed for computers with limited resources. It runs silently in the system tray and optimizes memory consumption on demand or automatically at set intervals.

## 🚀 How It Works (Total Transparency)

Unlike obscure or closed-source RAM optimizers that use aggressive memory allocation techniques (which can cause system instability or crashes), **RAM Manager** uses a native and official Windows API function from `psapi.dll`: the `EmptyWorkingSet` function.

When executed, the program iterates through all active processes and asks Windows to remove as many memory pages as possible from the Working Set of programs that are not actively being used at that exact second. Windows then safely moves this idle data to the Pagefile, freeing up physical memory (RAM) instantly.

> ℹ️ **Performance Note:** When reopening or maximizing a browser or program that had its memory cleared, Windows will read the data back from the disk into the RAM. On systems with an SSD, this process takes milliseconds and is practically unnoticeable.

## ✨ Features

- **Extreme Efficiency:** Capable of freeing memory even from native and protected Windows processes (when run as Administrator).
- **Zero Footprint:** Developed in pure C#, without heavy UI windows or third-party frameworks. The final executable is under 50 KB and consumes virtually no RAM in the background.
- **Silent Mode:** Operates 100% integrated into the system tray (next to the Windows clock).
- **Customizable Auto Cleanup:** Set the cleanup timer directly from the tray menu (5, 10, 15, 30, or 60 minutes).
- **Panic Button (Manual Mode):** Force an immediate cleanup at any time by right-clicking the icon and selecting `Free Memory Now`.
- **Auto-Start:** Automatically registers itself to run quietly in the background every time Windows starts.

## 🛠️ How to Compile (No Visual Studio Required)

If your PC is already low on RAM, you don't need to install the heavy Visual Studio IDE to generate this executable. Windows itself has the C# compiler built-in by default.

1. Save the source code in a file named `Ram_Manager.cs`.
2. Place your custom icon file named `ico.ico` in the same folder.
3. Open the **Command Prompt (CMD)** as **Administrator**.
4. Navigate to the folder where you saved the files:
   ```cmd
   cd "C:\Path\To\Your\Folder"

Run the native Windows compilation command to generate the .exe with your custom icon:

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /win32icon:

The executable Ram_Manager.exe will be generated in the same folder instantly!

🛡️ Execution and Permissions
To achieve the goal of clearing memory from core OS processes (such as Explorer, SearchUI, DWM), the program must be run with Administrator Privileges.

If you run it without elevated privileges, it will still work fine, but it will silently ignore OS-level processes due to Windows security barriers (Access Denied), clearing only your web browsers and standard user programs.

📄 License
This project is open-source and completely free for modification, study, and distribution. Feel free to use it, fork it, and improve it with total transparency!
