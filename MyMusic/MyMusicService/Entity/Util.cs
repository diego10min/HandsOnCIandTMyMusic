using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Entity
{
    public class Util
    {
        public string ConnectionStringDB()
        {
            string directory = Environment.CurrentDirectory.ToString();

            GetParentDirectory(ref directory);

            return "Filename=" + Path.Combine(directory, @"MyMusicDatabase\MyMusic.db");
        }

        public void GetParentDirectory(ref string directory)
        {
            directory = Directory.GetParent(directory).ToString();

            string[] splitDirectory = directory.Split(@"\");
            if (splitDirectory != null && splitDirectory.Length > 0)
            {
                if (splitDirectory[splitDirectory.Length - 1] != "MyMusic")
                {
                    GetParentDirectory(ref directory);
                }
            }
        }
    }
}
