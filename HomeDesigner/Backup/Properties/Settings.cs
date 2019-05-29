// Decompiled with JetBrains decompiler
// Type: HomeDesigner.Properties.Settings
// Assembly: HomeDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3850526B-3877-4CBB-8C11-EAD333A0DB1D
// Assembly location: C:\Users\Dario\Desktop\HomeSensorSimulator\HomeSensorSimulator\HomeDesigner.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace HomeDesigner.Properties
{
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
  [CompilerGenerated]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        Settings defaultInstance = Settings.defaultInstance;
        return defaultInstance;
      }
    }
  }
}
