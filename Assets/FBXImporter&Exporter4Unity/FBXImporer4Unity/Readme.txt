----------------------------------------------------------------------------------------------

FBX Importer 4 Unity β Version 2015/08/07

Copyright (c) 2015, ACTINIA Software. All rights reserved.

Homepage: http://actinia-software.com

E-Mail: hoetan@actinia-software.com

Twitter: https://twitter.com/hoetan3

GitHub: https://github.com/hoetan

Developer: ほえたん(Hoetan)

----------------------------------------------------------------------------------------------
[Japanese Manual]

■対応OS

 Windows 10 (64bit)
 Windows 8/8.1 Update 1 (64bit)
 Windows 7 ServicePack 1 (64bit)


■対応3Dエンジン

 Unity Personal/Professional v5.1.2f1 (64bit)

※必ずこのバージョン以降でシーン(Scene)を開いて下さい。※これ以下だとシーンを正常に開けません。


■対応Visual C++ランタイム

 Visual Studio 2013 の Visual C++ 再頒布可能パッケージ (x64)（日本語）: http://www.microsoft.com/ja-jp/download/details.aspx?id=40784

※「DllNotFoundException: Assets/FBXImporter&Exporter4Unity/FBXImporter4Unity/Plugins/x86_64/FBXImporter4Unity.dll」のエラーがでた場合は、上記のランタイムのインストールが必須です。


■使用ライブラリ(DLL)

 FBX SDK 2016.0 VS2013 (64bit): http://www.autodesk.com/products/fbx/overview

※「DllNotFoundException: Assets/FBXImporter&Exporter4Unity/FBXImporter4Unity/Plugins/x86_64/FBXImporter4Unity.dll」のエラーがでた場合は、上記のＳＤＫにあるＤＬＬのコピーが必須です。

　「C:\Program Files\Autodesk\FBX\FBX SDK\2016.1\lib\vs2015\x64\release\libfbxsdk.dll」を、Unityプロジェクトのルート（※）へコピーしてください。
　(※Assetsよりひとつ上の階層のルートのプロジェクトフォルダ内へエクスプローラで「libfbxsdk.dll」をコピーする）


■使用プログラミングツール(VisualC++)

 Visual Studio 2015 Professional


■使用方法

　FBX Importerを使用したい場合は、Unity Editor上で、「Assets/FBXImporter&Exporter4Unity/FBXImporter4Unity/Scenes/FBXImporter4Unity.unity」のシーンを開いてから実行（再生）してください。
　新規シーンの場合は、Hierarchyに新規に「GameObject」を作成して、「Assets/FBXImporter&Exporter4Unity/FBXImporter4Unity/Scripts/FBXImporter4Unity.cs」のスクリプトを「GameObject」にアタッチしてから実行（再生）してみてください。
　実行すると、プロジェクトのルートディレクトにあるファイル名「Unity.fbx」を読み込みます。
　FBXファイルは、FBXExporter4Unity / 3ds Max 2016 / Maya 2016 / Softimage 2015/ MotionBuilder 2016に対応しています。


■ライセンス

　このソフトウェアで入力したＦＢＸファイルは、自由に商用・非商用で使用することを許可します。
　複数のＰＣ（パソコン）へのインストールは、台数分のライセンスが必要になりますので、注意してください。


■体験版の使用制限

　体験版では、FBXファイルは、総数90フレーム(※約３秒間）以上のアニメーションは入力されません。製品版では無制限です。
　FBXファイルを読み込み時に、アセットストアのホームページ（宣伝）を自動で開きます。製品版ではスキップされます。


■製品版紹介

　「Asset Store」にて製品版の「FBX Importer 4 Unity β」と「FBX Exporter 4 Unity β」と「FBX Importer & Exporter 4 Unity β」を販売中です。

　・FBX Importer 4 Unity β
　　http://u3d.as/iit

　・FBX Exporter 4 Unity β
　　http://u3d.as/ghk

　・FBX Importer & Exporter 4 Unity β
　　http://u3d.as/iiu


■お問い合わせ

　改善案（アイデア）やバグ報告も、メールにて受け付けております。
　ついでに、このツールの使用目的を、メールで詳細を教えてくれると助かります。（興味深々）

　E-Mail: hoetan@actinia-software.com


----------------------------------------------------------------------------------------------
[English Manual]

■Compatible OS

Windows 10 (64bit)
Windows 8/8.1 Update 1 (64bit)
Windows 7 Service Pack 1 (64bit) [Windows7: Kinect2 is no support.]


■Compatible 3D Engine

Unity Personal/Professional v5.1.2f1 (64bit)

*Always open scenes using the version stated above or newer. Scenes will not properly open on older versions than the one stated above.


■Compatible Visual C++ Runtime

Visual Studio 2015 Visual C++ Redistributable Package (x64):
Japanese: http://www.microsoft.com/ja-jp/download/details.aspx?id=40784
English: https://www.microsoft.com/en-US/download/details.aspx?id=48145

*If the following error occurs:"DllNotFoundException: Assets/FBXImporter&Exporter4Unity/FBXImporter4Unity/Plugins/x86_64/FBXImporter4Unity.dll"

Please download a runtime from one of the links above that best matches your language.


■Library Dependency(DLL)

FBX SDK 2016.1 VS2013 (64bit): http://www.autodesk.com/products/fbx/overview

*If the following error occurs:"DllNotFoundException: Assets/FBXImporter&Exporter4Unity/FBXImporter4Unity/Plugins/x86_64/FBXImporter4Unity.dll"

Please copy-paste the DLL that is found in the above SDK.
Copy the DLL found from this path, "C:\Program Files\Autodesk\FBX\FBX SDK\2016.1\lib\vs2015\x64\release\libfbxsdk.dll", to the root (*) of the Unity project.
(*Copy-paste "libfbxsdk.dll" into the root project directory that is one hierarchy above the Assets directory.)


■Programming Tool Dependency (Visual C++)

Visual Studio 2013 Professional Update 5


■How to use

Using the FBX Importer on Unity Editor: "Assets/FBXImporter&Exporter4Unity/FBXImporter4Unity/Scenes/FBXImporter4Unity.unity" open this scene and play.

*If it is a new scene, create "GameObject"in the Hierarchy and attach the following script, “Assets/FBXImporter&Exporter4Unity/FBXImporter4Unity/Scripts/FBXImporter4Unity.cs”.
Running(playing) the scene will automatically load the file “Unity.fbx” located in the project's root directory.
This FBX file is compatible with: FBXExporter4Unity, 3ds Max 2016, Maya 2016, Softimage 2015, MotionBuilder 2016.


■License

The FBX files exported through this software are free for personal and commercial use.
Please be aware that it is prohibited to use a single license to install this software on multiple computers. Each license purchased can only be applied to a single computer. Multiple computers will mean a need to purchase an equivalent amount of licenses.


■Limitations for the trial version

In the trial version, a maximum of only 90 frames will be imported from a FBX animation file. There is no frame limitation for the product version.

Everytime a FBX file is loaded, the asset store homepage will be automatically opened.
This will not occur for the product version.


■Now on sale

"FBX Importer 4 Unity β" and "FBX Exporter 4 Unity β" and "FBX Importer & Exporter 4 Unity β" is now on sale in the "Asset Store".

FBX Importer 4 Unity β
http://u3d.as/iit

FBX Exporter 4 Unity β
http://u3d.as/ghk

FBX Importer & Exporter 4 Unity β
http://u3d.as/iiu


■Contact Us

Mails about ideas for improving this software and bug reports are welcome.
Mails about how this software is being used are also very welcome.
 
E-Mail: hoetan@actinia-software.com
