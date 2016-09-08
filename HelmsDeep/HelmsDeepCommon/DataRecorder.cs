using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;


namespace HelmsDeepCommon
{
    public class DataRecorder
    {
        public delegate void ReadDataFileCallback(DateTime dt, string name, float[] values);

        private static Logger log = LogManager.GetCurrentClassLogger();
        private string dataPath;
        private TextWriter currentFile;
        private int rotationDepth = 5;
        private object locker = new object();
        private string datetimeFormat = "dd-MM-yy-HH-mm-ss";

        public DataRecorder(string dataPath)
        {
            
            this.dataPath = dataPath;
            currentFile = File.AppendText(FileDataPath);
        }

		
        public void Record(string name, params float[] values)
        {
            lock (locker)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(DateTime.Now.ToString(datetimeFormat));
                    sb.Append(" ");
                    sb.Append(name);
                    sb.Append(" ");
                    foreach (var v in values)
                    {
                        sb.Append(v);
                        sb.Append(" ");
                    }

                    currentFile.WriteLine(sb.ToString());
                    currentFile.Flush();
                }
                catch (Exception ex)
                {

                    log.Error("Не удалось внести запись в журнал. Имя журнала: " + FileDataPath);
                    log.Error(ex);
                }
            }
        }

        public void ReadRecords(ReadDataFileCallback callback)
        {
            if (callback == null)
                return;
            lock (locker)
            {
                currentFile.Close();

                using (TextReader f = File.OpenText(FileDataPath))
                {
                    string line;
                    while ((line = f.ReadLine()) != null)
                    {
                        var sections = line.Split(new[] {' '});
                        if(sections.Length<3)
                            continue;

                        DateTime dt =
                            DateTime.ParseExact(sections[0], datetimeFormat,
                            CultureInfo.InvariantCulture);

                        float[] values = new float[sections.Length-2];
                        for (int i = 2; i < sections.Length; i++)
                        {
                            float v;
                            float.TryParse(sections[i], out v);
                            values[i - 2] = v;
                        }
                        callback(dt,sections[1], values);
                    }
                }

                currentFile = File.AppendText(FileDataPath);
            }
        }

        public void Rotate()
        {
            lock (locker)
            {
                currentFile.Close();
                string n = GetBackupPath(rotationDepth);
                if (File.Exists(n))
                    File.Delete(n);

                for (int i = rotationDepth; i > 1; i--)
                {
                    try
                    {
                        File.Move(GetBackupPath(i - 1), GetBackupPath(i));
                    }
                    catch (Exception e)
                    {
                    }
                }

                try
                {
                    File.Move(FileDataPath, GetBackupPath(1));
                }
                catch (Exception e)
                {
                }

                if (File.Exists(FileDataPath))
                {
                    log.Error("Ротация журнала не удалась, будем писать в старый журнал");
                }

                currentFile = File.AppendText(FileDataPath);
            }
        }

        private string FileDataPath
        {
            get { return Path.Combine(dataPath, "records.dat"); }
        }

        string GetBackupPath(int index)
        {
            return Path.Combine(dataPath, "records" + index + ".dat");
        }
    }
}
