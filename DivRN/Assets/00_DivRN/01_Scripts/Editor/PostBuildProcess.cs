using UnityEditor;
using UnityEditor.Callbacks;
#if  (UNITY_IOS || UNITY_TVOS)
using UnityEditor.iOS.Xcode;
#endif
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class PostBuildProcess
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
#if  (UNITY_IOS || UNITY_TVOS)
        if (buildTarget == BuildTarget.iOS)
        {
            ProcessForiOS(path);
        }
#endif
    }

#if  (UNITY_IOS || UNITY_TVOS)
    //PUSH自動ON
    // https://teratail.com/questions/52234
    // http://blog.livedoor.jp/abars/archives/52394874.html
    private static void CreateEntitlements(string path, string yourappname, PBXProject pj, string target, bool prodmode)
    {
        XmlDocument document = new XmlDocument();
        XmlDocumentType doctype = document.CreateDocumentType("plist", "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
        document.AppendChild(doctype);

        XmlElement plist = document.CreateElement("plist");
        plist.SetAttribute("version", "1.0");
        XmlElement dict = document.CreateElement("dict");
        plist.AppendChild(dict);
        document.AppendChild(plist);

        XmlElement e = (XmlElement)document.SelectSingleNode("/plist/dict");

        XmlElement key = document.CreateElement("key");
        key.InnerText = "aps-environment";
        e.AppendChild(key);

        XmlElement value = document.CreateElement("string");
        if (prodmode == true)
        {
            value.InnerText = "production";
        }
        else
        {
            value.InnerText = "development";
        }
        e.AppendChild(value);

        string entilementions = yourappname + ".entitlements";
        string entitlementsPath = path + "/" + entilementions;
        document.Save(entitlementsPath);

        string guid = pj.AddFile(entitlementsPath, entilementions);
        pj.SetBuildProperty(target, "CODE_SIGN_ENTITLEMENTS", entilementions);
        pj.AddFileToBuild(target, guid);
    }

    private static void SetCapabilities(string path, PBXProject pj, bool prodmode)
    {
        string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

        string[] lines = pj.WriteToString().Split('\n');
        List<string> newLines = new List<string>();
        bool editFinish = false;

        for (int i = 0; i < lines.Length; i++)
        {

            string line = lines[i];

            if (editFinish)
            {
                newLines.Add(line);
            }
            else if (line.IndexOf("isa = PBXProject;") > -1)
            {
                do
                {
                    newLines.Add(line);
                    line = lines[++i];
                } while (line.IndexOf("TargetAttributes = {") == -1);

                // 以下の内容はxcodeprojの内容にあるproject.pbxprojを参照してください
                newLines.Add("TargetAttributes = {");
                newLines.Add("1D6058900D05DD3D006BFB54 = {");
                if (prodmode == true)
                {
                    newLines.Add("DevelopmentTeam = XXXXXXXXXX;");
                }
                else
                {
                    newLines.Add("DevelopmentTeam = XXXXXXXXXX;");
                }
                newLines.Add("SystemCapabilities = {");
                newLines.Add("com.apple.Push = {");
                newLines.Add("enabled = 1;");
                newLines.Add("};");
                newLines.Add("};");
                newLines.Add("};");

                editFinish = true;
            }
            else
            {
                newLines.Add(line);
            }
        }

        File.WriteAllText(projPath, string.Join("\n", newLines.ToArray()));
    }

    private static void SetBackgroundMode(string path)
    {
        var plistPath = Path.Combine(path, "Info.plist");

        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        plist.root.SetBoolean("FirebaseAppDelegateProxyEnabled", false);

        PlistElementArray bgModes = plist.root.CreateArray("UIBackgroundModes");
        bgModes.AddString("remote-notification");

        plist.WriteToFile(plistPath);
    }

    private static void ProcessForiOS(string path)
    {
        string pjPath = PBXProject.GetPBXProjectPath(path);
        PBXProject pj = new PBXProject();
        pj.ReadFromString(File.ReadAllText(pjPath));
        string target = pj.TargetGuidByName("Unity-iPhone");

        // Enable BitCode -> NO
        pj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

        //フレームワーク追加
        List<string> frameworks = new List<string>() {
            "libz.tbd",
            "CoreData.framework",
            "iAd.framework",
            "SafariServices.framework",
        };

        foreach (var framework in frameworks)
        {
            pj.AddFrameworkToProject(target, framework, false);
        }

        const string STR_DGRNDEV = "dgrndev";
        const string STR_DGPROD = "DivineGate4Store";
        string profile = STR_DGRNDEV;

        if (BuildEditor.JenkinsBuild == true)
        {
            string[] settings = BuildEditor.OnLoadSettong("user.conf");

            if (settings.Length == 3)
            {
                pj.SetBuildProperty(target, "DEVELOPMENT_TEAM", settings[0]); // チーム名はADCで確認できるPrefix値を設定する
                pj.SetBuildProperty(target, "CODE_SIGN_IDENTITY", settings[1]);
                pj.SetBuildProperty(target, "PROVISIONING_PROFILE_SPECIFIER", settings[2]); // XCode8からProvisioning名で指定できる
                profile = settings[2];
            }
        }
        else
        {
            pj.SetBuildProperty(target, "DEVELOPMENT_TEAM", "XXXXXXX"); // チーム名はADCで確認できるPrefix値を設定する
            pj.SetBuildProperty(target, "CODE_SIGN_IDENTITY", "iPhone Developer: XXXXXXX (XXXXXXX)");
            pj.SetBuildProperty(target, "PROVISIONING_PROFILE_SPECIFIER", profile); // XCode8からProvisioning名で指定できる
        }

        if (profile.Equals(STR_DGPROD))
        {
            CreateEntitlements(path, "dg", pj, target, true);
            SetCapabilities(path, pj, true);
            SetBackgroundMode(path);
        }
        else if (profile.Equals(STR_DGRNDEV))
        {
            CreateEntitlements(path, "dgrn", pj, target, false);
            SetCapabilities(path, pj, false);
            SetBackgroundMode(path);
        }

        File.WriteAllText(pjPath, pj.WriteToString());

        //Info.plistの設定
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

#if DEBUG_EXPORT_BATTLE_LOG && BUILD_TYPE_DEBUG
        plist.root.SetBoolean("UIFileSharingEnabled", true);
#endif

        //URLスキームの追加
        var urlTypes = plist.root.CreateArray("CFBundleURLTypes");
        var dict = urlTypes.AddDict();
        dict.SetString("CFBundleURLName", "jp.example.dg");
        var urlSchemes = dict.CreateArray("CFBundleURLSchemes");
        urlSchemes.AddString("jp.example.divinegate");

        plist.WriteToFile(plistPath);
    }
#endif
}