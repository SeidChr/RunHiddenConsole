# RunHiddenConsole
Created executable can be renamed to powershellw.exe or pwshw.exe (or any other console-flashing-window) 
and put next to the corresponding assembly. Calls to the added assembly will be sent to a new instance of 
the target assembly, which is explicitly started without creating a window. Thus these flashing windows 
should be avoided.

You can find all the magic in Programm.cs
## Logging
I have added a (cleaned up) copy of simple log (https://www.codeproject.com/Tips/585796/Simple-Log) for 
debugging puropse. When there is a crash in the tool you should find a log-file next to your *w.exe called 
*w.exe.<date>.log which contains some hopefully usefull messages.

If you need to change the log level of the tool, you can add a "*w.exe.config", and with the default values 
as seen in app.config in the repository. Then just update the LogLevel app-setting as you desire.
## Future Points
Input stream from file and output stream to file, if requested.
    Please tell me how you would like to use this. It would probably require a config file alongside 
	the assembly to configure which parameters define the inputs and outputs
