// Decompiled with JetBrains decompiler
// Type: HomeDesigner.Properties.Resources
// Assembly: HomeDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3850526B-3877-4CBB-8C11-EAD333A0DB1D
// Assembly location: C:\Users\Dario\Desktop\HomeSensorSimulator\HomeSensorSimulator\HomeDesigner.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace HomeDesigner.Properties
{
  [DebuggerNonUserCode]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) HomeDesigner.Properties.Resources.resourceMan, (object) null))
          HomeDesigner.Properties.Resources.resourceMan = new ResourceManager("HomeDesigner.Properties.Resources", typeof (HomeDesigner.Properties.Resources).Assembly);
        return HomeDesigner.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return HomeDesigner.Properties.Resources.resourceCulture;
      }
      set
      {
        HomeDesigner.Properties.Resources.resourceCulture = value;
      }
    }

    internal static Bitmap grid_25
    {
      get
      {
        return (Bitmap) HomeDesigner.Properties.Resources.ResourceManager.GetObject(nameof (grid_25), HomeDesigner.Properties.Resources.resourceCulture);
      }
    }
  }
}
