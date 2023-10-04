# Cyggie Unity package
A repo that contains multiple packages w/ generic classes and scripts for Unity.

# Table of Contents
### Main
The main package that all other Cyggie packages reference to. 
This includes scripts like:
- Service Manager and Services (Singleton that contains Singletons)
- Log system (Extends the UnityEngine.Debug class)
- Helper classes for Unity Objects, Coroutines, and more!

### File Manager
A package that contains the relevant scripts to manage Save Files using Unity's persistent data path, Newtonsoft Json and AES Encryption.

### Scene Changer
A package that contains the relevant scripts to manage changing Scenes. It also supports Loading screens and Waiting for Inputs. Projects must have Unity's New Input System in order to use this.

### SQLite
A package that contains the relevant scripts to manage an SQLite database. It supports creating, opening and updating multiple SQLite database in the same project. It also has field-by-field AES encryption, meaning that the data is encrypted as opposed to the whole database being encrypted

# Instructions
1. Open your Unity Project's manifest.
	1. Navigate to your Unity Project's root folder.
	2. Navigate into your Packages folder.
	3. Open your manifest.json file.
2. Within your dependencies array add the following line:
    - "cyggie.[package-name]": "https://github.com/Shayminn/Cyggie-Unity-Package.git?path=/Cyggie-Unity-Package/Packages/[package-name]"
    - eg. "cyggie.main": "https://github.com/Shayminn/Cyggie-Unity-Package.git?path=/Cyggie-Unity-Package/Packages/main",
3. Open up your Unity Editor and the package will be automatically imported to your project.
    - If you have an Assembly Definition (.asmdef) defined, make sure you add the package's reference to it if you need to reference it in your scripts. 