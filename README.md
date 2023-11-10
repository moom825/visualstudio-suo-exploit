
# VisualStudio .suo deserialization exploit

  

A Proof of Concept (PoC) that will create a .suo file, this .suo file and can be put into any VisualStudio code project. When the sln/project is opened it will cause code execution. This technique was discovered by cjm00nw & edwardzpeng!

  

## Installation

  

This project is a Visual Studio Code project and requires Visual Studio and C# to be installed.

  

## Usage

  

1. Go to the "Releases" section of this repository.

2. Download the latest release of "suo_exploit_test.exe"

3. Open a command prompt or PowerShell.

4. Run the exploit executable with the desired command, like:

	suo_exploit_test.exe input.suo injected.suo calc
	
	or
	
	suo_exploit_test.exe input.suo injected.suo cmd /c start calc

  

## Help menu

  

Usage: 

	suo_exploit_test.exe input.suo output.suo command [optional args]

  

Examples:

  

	suo_exploit_test.exe input.suo injected.suo calc

	suo_exploit_test.exe input.suo injected.suo cmd /c start calc

  


The input.suo is an existing .suo for the program to modify


The injected.suo is the output, thats the file which when open by visual studio's will run your command

  

## Credits

  

Credits to cjm00nw & edwardzpeng for discovering this technique.

  

## Contact

  

**Contact the Developer:**

- **Telegram:**  [moom825](https://t.me/moom825)

- **Discord:** moom825

  

## Donation

### Buy me a coffee!

BTC: bc1qg4zy8w5swc66k9xg29c2x6ennn5cyv2ytlp0a6