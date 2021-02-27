# bip38-brute-forcer
A simple brute force tool to decode BIP38 encoded private keys via word dictionaries.
It uses Bitcoin model binding from awesome **casascius** project [PaperTool](https://github.com/casascius/PaperTool) and also very efficent SCrypt binding from **replicon** [Replicon.Cryptography.SCrypt](https://github.com/replicon/Replicon.Cryptography.SCrypt). 

# Note
This tool is resumable, each time it stopped, the last processed entry id is saved into **brute_history.log** file. That value is related to the word dictionary file. Everytime you change the target address/encryption key or list of word dictionary files, consider either setting the value in that file to zero or just remove the file before running the app.   

# Configuration
Edit the application settings in *App.config* file, save the file and run the program.

Example configuration:
```
<appSettings>
  <add key="PublicAddress" value="14yhPbUgcYjbUui3B15hxotAbCLXNj7gEp" />
  <add key="EncryptedPrivateKey" value="6PnWSWJ9hTRhbC4ZebvcS4NtDAsaZRjuRhv8T6MTj1C1Ls427Tg4Yc3xvE" />
  <add key="DictionaryFiles" value="testpasswords.txt" />
  <add key="NumberOfWorkers" value="4" />
  <add key="ReportInterval" value="1" />
</appSettings>
```
  
  Parameters details:
  
  *PublicAddress* - Public Bitcoin address.  
  *EncryptedPrivateKey* - Encrypted private key.  
  *DictionaryFiles* - A comma separated list of word dictionary files. Words inside of a dictionary file must be separated by a new line.  
  *NumberOfWorkers* -  Maximum number of concurrent worker threads to use. You should default it to number of cores on your machine.
  Setting it to "-1" as a value will set limit to "No Limit" and will try to utulize all resources on your machine.   
  *ReportInterval* - Progress reporting interval in minutes, default is "1".
  
  Running this example configuration should return **testpassword** as a found passphrase. 

# Donation
If this project helped you, consider buying me a cup of coffee :)

https://ko-fi.com/ArchitectCat

Bitcoin Address: 1wuNBchDeXbKBS4zf3XLbnD2unm89BVgg
