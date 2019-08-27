using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

namespace UseNewtonsoftJson
{




    public class LoadJson
    {

        private static void ReadingJson(string desktopPath)
        {
            string myStr = null;
            //IO读取
            myStr = GetMyJson(desktopPath);
            //转换
            var jArray = JsonConvert.DeserializeObject<List<Segmentj>>(myStr);
            Debug.Log(jArray);
            //进一步的转换我就不写啦

        }
        private static string GetMyJson(string desktopPath)
        {
            using (FileStream fsRead = new FileStream(string.Format("{0}\\testdata.json", desktopPath), FileMode.Open))
            {
                //读取加转换
                int fsLen = (int)fsRead.Length;
                byte[] heByte = new byte[fsLen];
                int r = fsRead.Read(heByte, 0, heByte.Length);
                return System.Text.Encoding.UTF8.GetString(heByte);
            }
        }




        return null;
    

}
}
