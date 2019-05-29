// Decompiled with JetBrains decompiler
// Type: HomeDesigner.Printer
// Assembly: HomeDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3850526B-3877-4CBB-8C11-EAD333A0DB1D
// Assembly location: C:\Users\Dario\Desktop\HomeSensorSimulator\HomeSensorSimulator\HomeDesigner.exe

using SensorSim.Data;
using SensorSim.Util;
using System.Collections.Generic;

namespace HomeDesigner
{
  internal static class Printer
  {
    public static string printPos(List<Position> pos)
    {
      string str = "";
      foreach (Position po in pos)
        str = str + po.name + " " + po.x.ToString() + " " + po.y.ToString() + "\r\n";
      return str;
    }

    public static string printWall(List<Pair<Position, Position>> wall)
    {
      string str = "";
      foreach (Pair<Position, Position> pair in wall)
        str = str + pair.First.name + " " + pair.Second.name + "\r\n";
      return str;
    }

    public static string printSensor(List<Sensor> sensor)
    {
      string str = "";
      foreach (Sensor sensor1 in sensor)
        str = str + sensor1.name + " " + sensor1.x.ToString() + " " + sensor1.y.ToString() + " " + sensor1.orinentation.ToString() + " " + sensor1.type + "\r\n";
      return str;
    }
  }
}
