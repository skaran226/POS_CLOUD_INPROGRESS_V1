using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

namespace POSM
{
    class CallbackThreads
    {


        public static bool FileCopySuccess = false;
        public static bool db_backup = true;
        public static bool checkhotkey = true;
        public static bool checkhotkey_cloud = true;
        public static bool deviceconfig = true;
        public static bool deviceconfig_cloud = true;
        public static bool CopyFileExep = false;
        public static bool updatepend_config = false;
        public static bool updatepend_hotkey = false;
        public static TimerCallback timerDelegate = null, timerDelegate1 = null, timerDelegate2 = null, timerDelegate_cloud1 = null, timerDelegate_cloud2 = null;
        public static Timer PumpTimer = null, checkHotKey = null, device_config = null, checkHotKey_cloud = null, device_config_cloud = null;



        public static void TimerCallbackThreading()
        {

            if (db_backup)
            {

                timerDelegate = new TimerCallback(Db_Backup);
                PumpTimer = new Timer(timerDelegate, null, 1801000, 1801000); // 1hours 1 sec
            }
            if (checkhotkey)
            {

                timerDelegate1 = new TimerCallback(checkHotKeyPromotion);
                checkHotKey = new Timer(timerDelegate1, null, 2040000, 2040000); //34 minutes
            }

            if (checkhotkey_cloud)
            {

                timerDelegate_cloud1 = new TimerCallback(checkHotKeyPromotionFromCloud);
                checkHotKey_cloud = new Timer(timerDelegate_cloud1, null, 1740000, 1740000); //29 minutes
            }

            if (deviceconfig)
            {

                timerDelegate2 = new TimerCallback(device_config_check);
                device_config = new Timer(timerDelegate2, null, 5400000, 5400000);  //1.5 hours
            }

            if (deviceconfig_cloud)
            {
                timerDelegate_cloud2 = new TimerCallback(device_config_check_FromCLoud);
                device_config_cloud = new Timer(timerDelegate_cloud2, null, 5100000, 5100000);  //1.41 hours

            }





        }

        private static void device_config_check_FromCLoud(object state)
        {
            deviceconfig_cloud = false;
            string query = "select SiteKey,APP,filepath,Updatepending from ConfigurationTables where SiteKey=2 and APP='devconfig'";
            string updatepending_cloud = "update ConfigurationTables set Updatepending=0 where SiteKey=2 and APP='devconfig'";
            string updatepending_local = "update ConfigurationTables set Updatepending=1 where SiteKey=2 and APP='devconfig'";

            try
            {


                SqlCommand cmd = Cloud_DB.ExecuteReader(query);
                SqlDataReader dbr = cmd.ExecuteReader();


                if (dbr.HasRows)
                {

                    while (dbr.Read())
                    {

                        if (dbr["Updatepending"].Equals(true))
                        {

                            CopyPaste(dbr["filepath"].ToString(), Macros.DESTI_PATH + Macros.CONFIG_BCK_FILE);
                            if (CopyFileExep)
                            {

                                DB.CloseConn();
                                DB.ExecuteNonQuery(updatepending_local);
                                Cloud_DB.CloseConn();
                                Cloud_DB.ExecuteNonQuery(updatepending_cloud);
                                updatepend_config = true;
                            }



                        }


                    }
                    if (CopyFileExep && updatepend_config)
                    {
                        FileCopySuccess = true;
                        updatepend_config = false;
                    }
                    deviceconfig_cloud = true;

                }
                else
                {

                    Debug.WriteLine("Not Found data in Config table");
                }

                cmd.Dispose();
                dbr.Dispose();

            }
            catch (Exception ex)
            {

                Debug.WriteLine("Not read data in Config table");
            }


        }

        private static void checkHotKeyPromotionFromCloud(object state)
        {
            checkhotkey_cloud = false;
            string query = "select SiteKey,APP,filepath,Updatepending from ConfigurationTables where SiteKey=1 and APP='hotkeylist'";
            string updatepending_cloud = "update ConfigurationTables set Updatepending=0 where SiteKey=1 and APP='hotkeylist'";
            string updatepending_local = "update ConfigurationTables set Updatepending=1 where SiteKey=1 and APP='hotkeylist'";

            try
            {


                SqlCommand cmd = Cloud_DB.ExecuteReader(query);
                SqlDataReader dbr = cmd.ExecuteReader();


                if (dbr.HasRows)
                {

                    while (dbr.Read())
                    {

                        if (dbr["Updatepending"].Equals(true))
                        {

                            CopyPaste(dbr["filepath"].ToString(), Macros.DESTI_PATH + Macros.CONFIG_BCK_FILE);
                            if (CopyFileExep)
                            {
                                DB.CloseConn();
                                DB.ExecuteNonQuery(updatepending_local);
                                Cloud_DB.CloseConn();
                                Cloud_DB.ExecuteNonQuery(updatepending_cloud);
                                updatepend_config = true;
                            }



                        }


                    }
                    if (CopyFileExep && updatepend_config)
                    {
                        FileCopySuccess = true;
                        updatepend_config = false;
                    }
                    checkhotkey_cloud = true;

                }
                else
                {

                    Debug.WriteLine("Not Found data in Config table");
                }

                cmd.Dispose();
                dbr.Dispose();

            }
            catch (Exception ex)
            {

                Debug.WriteLine("Not read data in Config table");
            }
        }



        private static void device_config_check(object state)
        {
            deviceconfig = false;
            string query = "select SiteKey,APP,filepath,Updatepending from ConfigurationTables where SiteKey=2 and APP='devconfig'";
            string updatepending = "update ConfigurationTables set Updatepending=0 where SiteKey=2 and APP='devconfig'";

            try
            {


                SqlCommand cmd = DB.ExecuteReader(query);
                SqlDataReader dbr = cmd.ExecuteReader();


                if (dbr.HasRows)
                {

                    while (dbr.Read())
                    {

                        if (dbr["Updatepending"].Equals(true))
                        {

                            CopyPaste(dbr["filepath"].ToString(), Macros.DESTI_PATH + Macros.CONFIG_BCK_FILE);
                            if (CopyFileExep)
                            {
                                DB.CloseConn();
                                DB.ExecuteNonQuery(updatepending);
                                updatepend_config = true;

                            }


                        }


                    }
                    if (CopyFileExep && updatepend_config)
                    {
                        FileCopySuccess = true;
                        updatepend_config = false;
                    }
                    deviceconfig = true;

                }
                else
                {

                    Debug.WriteLine("Not Found data in Config table");
                }

                cmd.Dispose();
                dbr.Dispose();

            }
            catch (Exception ex)
            {

                Debug.WriteLine("Not read data in Config table");
            }



            //check new device config file  from local db

            //first hit configuration table and get device config xml file accoring site key and app name

            //store xml file from destination folder

            // restart the app
        }

        private static void checkHotKeyPromotion(object state)
        {

            checkhotkey = false;
            string query = "select SiteKey,APP,filepath,Updatepending from ConfigurationTables where SiteKey=1 and APP='hotkeylist'";
            string updatepending = "update ConfigurationTables set Updatepending=0 where SiteKey=1 and APP='hotkeylist'";

            try
            {


                SqlCommand cmd = DB.ExecuteReader(query);
                SqlDataReader dbr = cmd.ExecuteReader();


                if (dbr.HasRows)
                {

                    while (dbr.Read())
                    {

                        if (dbr["Updatepending"].Equals(true))
                        {

                            CopyPaste(dbr["filepath"].ToString() , Macros.DESTI_PATH + Macros.HOTKEY_BCK_FILE);
                            if (CopyFileExep)
                            {
                                DB.CloseConn();
                                DB.ExecuteNonQuery(updatepending);
                                CopyFileExep = true;
                                updatepend_hotkey = true;

                            }



                        }


                    }
                    if (CopyFileExep && updatepend_hotkey)
                    {
                        FileCopySuccess = true;
                        updatepend_hotkey = false;
                    }
                    checkhotkey = true;

                }
                else
                {

                    Debug.WriteLine("Not Found data in Config table");
                }

                cmd.Dispose();
                dbr.Dispose();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Not read data in Config table");

            }

            //check new hot keys promotion from local db

            //first hit configuration table and get hot key xml file accoring site key

            //store xml file from destination folder


            //restart the app
        }




        private static void Db_Backup(object state)
        {
            db_backup = false;
            /* string source = Macros.DESTI_PATH + Macros.DB_FILE;
             string destination = Macros.DESTI_PATH + Macros.DB_BCK_FILE;

             CopyPaste(source, destination);*/

            string sPassQuery = "backup database posDB to disk='" + Macros.DB_BCK_PATH + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond + "_" + Macros.DB_FILE + "'";

            DB.CloseConn();
            DB.ExecuteNonQuery(sPassQuery);
            db_backup = true;

            //Backup Script

            //
        }

        public static void CopyPaste(string source, string destination)
        {

            try
            {


                File.Copy(source, destination, true);
                Debug.WriteLine("File copy in destination folder");
                CopyFileExep = true;

            }
            catch (Exception ex)
            {

                Debug.WriteLine("can't copy file");
            }

        }





    }
}
