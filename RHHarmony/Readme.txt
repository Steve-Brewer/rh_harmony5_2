There are 2 main parts to working with the new Harmony approach :

	1. Run SDX mods
	2. Patch the game with Harmony 

The following steps explain how to patch the game with the latest changes from Gitlab : 

1. Get latest from SDX RHHarmony Gitlab
2. Get latest from RHHarmony Gitlab
3. Copy the following folder/files into SDX mods folder
	- From : D:\7D2D_Mods\Mod VS Project\rh_sdxharmony_5_2\rh_sdxharmony_5_2\SDXMods\Mods\RHMods\*
	- Into : D:\SDX\SDX0.7.4Test\Targets\7DaysToDie\Mods\RHMods\*
4. Open SDX
5. Build SDX
6. Copy generated Assembly-CSharp.dll from SDX into 2 places : 
	- From : wherever you have set the SDX Game Directory
	- Into : D:\7D2D_Mods\Mod VS Project\rh_harmony5_2\rh_harmony5_2\Libs\7Days\Assembly-CSharp.dll
	- Into : Your game install folder Managed folder /7DaysToDie_Data/Managed/
	- Also copy the usual SDX files if needed (Mods.dll, SDX.Core.dll, SDX.Payload.dll)
7. Open the RHHarmony solution/project in Visual Studio (proj or sln file)
	- D:\7D2D_Mods\Mod VS Project\rh_harmony5_2\rh_harmony5_2\RHHarmony\RHHarmony.sln
8. Build project in VS in Release mode (see dropdown in top toolbar, change from debug to release)
10. Copy following files : 
	- From : D:\7D2D_Mods\Mod VS Project\rh_harmony5_2\rh_harmony5_2\RHHarmony\bin\Release\RHHarmony.dll
	- From : D:\7D2D_Mods\Mod VS Project\rh_harmony5_2\rh_harmony5_2\Libs\Harmony\net35\0Harmony.dll (this only ever needs doing once)
	- Into : Your game install folder Managed folder /7DaysToDie_Data/Managed/


NOTE : Be aware there is a slight circular dependency if you are doing this for the first time. SDX requires a copy of the RHHarmony.dll to build but the RHHarmony needs the SDX Assembly to build.
To get around this you need to run a reduced version of the RHHarmony project to generate a RHHarmony.dll with just the injector code. 

Only follow this if you do NOT have a RHHarmony.dll file in the root of your SDX install
1. Open the RHHarmony solution/project in Visual Studio (proj or sln file)
	- D:\7D2D_Mods\Mod VS Project\rh_harmony5_2\rh_harmony5_2\RHHarmony\RHHarmony.sln 
2. Right click all the A17* folder and select Exclude From Project
3. Build project in VS in Release mode (see dropdown in top toolbar, change from debug to release)
4. Copy following file : 
	- From : D:\7D2D_Mods\Mod VS Project\rh_harmony5_2\rh_harmony5_2\RHHarmony\bin\Release\RHHarmony.dll
	- Into : D:\SDX\SDX0.7.4Test\RHHarmony.dll
5. Go back to the RHHarmony solution/project in Visual Studio and Undo the folder excludes
