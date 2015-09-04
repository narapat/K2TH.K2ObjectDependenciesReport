using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.Forms.Management;
using SourceCode.SmartObjects.Client;
using SourceCode.Hosting.Client.BaseAPI;

using System.Data;
using System.Data.SqlClient;

namespace ConsoleApplicationDependencies
{
    class Program
    {
        static string connectionString = "Data Source=DLX;Initial Catalog=K2;integrated security=sspi;Pooling=True";

        static void Main(string[] args)
        {

            //Get Form View Relation
            DataTable dtFormView = new DataTable();
            dtFormView.Columns.Add("FormGUID", typeof(Guid));            
            dtFormView.Columns.Add("ViewGUID", typeof(Guid));            

            SCConnectionStringBuilder sb = new SCConnectionStringBuilder();
            sb.Authenticate = true;
            sb.IsPrimaryLogin = true;
            sb.Integrated = true;
            sb.Host = "localhost";
            sb.Port = 5555;

            SmartObjectClientServer socs = new SmartObjectClientServer();
            socs.CreateConnection();
            socs.Connection.Open(sb.ToString());
            FormsManager fm = new FormsManager(sb.ToString());
            FormExplorer fe = fm.GetForms();
            foreach (FormInfo fi in fe.Forms)
            {
                Guid formGUID = fi.Guid;
                //Console.WriteLine("Form: " + fi.DisplayName);
                ViewExplorer ve = fm.GetViewsForForm(fi.Guid);                                            

                foreach (ViewInfo vi in ve.Views)
                {
                    DataRow dr = dtFormView.NewRow();
                    dr["FormGUID"] = formGUID;
                    dr["ViewGUID"] = vi.Guid;

                    dtFormView.Rows.Add(dr);
                }
            }

            //Get Form Process Relation
            DataTable dtFormProcess = new DataTable();
            dtFormProcess.Columns.Add("ProcessName", typeof(string));
            dtFormProcess.Columns.Add("FormGUID", typeof(Guid));            

            fe = fm.GetForms();
            foreach (FormInfo fi in fe.Forms)
            {
                Guid formGUID = fi.Guid;

                IEnumerable<string> processes = fm.GetProcessesForForm(fi.Guid);
                
                foreach (var process in processes)
                {
                    DataRow dr = dtFormProcess.NewRow();
                    dr["FormGUID"] = formGUID;
                    dr["ProcessName"] = process;

                    dtFormProcess.Rows.Add(dr);
                }
            }

            //Get View SMO Relation
            DataTable dtViewSMO = new DataTable();
            dtViewSMO.Columns.Add("ViewGUID", typeof(Guid));
            dtViewSMO.Columns.Add("SmartObjectGUID", typeof(Guid));   

            fe = fm.GetForms();
            foreach (FormInfo fi in fe.Forms)
            {                
                ViewExplorer ve = fm.GetViewsForForm(fi.Guid);
                
                foreach (ViewInfo vi in ve.Views)
                {
                    Guid viewGUID = vi.Guid;

                    foreach (Guid soGuid in fm.GetObjectsForView(vi.Guid))
                    {
                        DataRow dr = dtViewSMO.NewRow();
                        dr["ViewGUID"] = viewGUID;
                        dr["SmartObjectGUID"] = soGuid;

                        dtViewSMO.Rows.Add(dr);
                    }                    
                }
            }
            fm.Connection.Close();
            socs.Connection.Close();

            //Delete data from table
            string stmt = "Delete From FormViewDependency";
            ExecuteNoneQuery(stmt, CommandType.Text);

            //Insert Form View Relation to table
            for(int i = 0; i < dtFormView.Rows.Count; i++)
            {
                DataRow dr = dtFormView.Rows[i];
                stmt = @"INSERT INTO [dbo].[FormViewDependency]
                     ([FormGUID]
                    ,[ViewGUID])
                    VALUES
                    ('" + dr["FormGUID"].ToString() + @"'
                    ,'" + dr["ViewGUID"].ToString() + "')";

                ExecuteNoneQuery(stmt, CommandType.Text);
            }

            //Delete data from table
            stmt = "Delete From FormProcessDependency";
            ExecuteNoneQuery(stmt, CommandType.Text);

            //Insert Form Process Relation to table
            for (int i = 0; i < dtFormProcess.Rows.Count; i++)
            {
                DataRow dr = dtFormProcess.Rows[i];
                stmt = @"INSERT INTO [dbo].[FormProcessDependency]
                     ([FormGUID]
                    ,[ProcessName])
                    VALUES
                    ('" + dr["FormGUID"].ToString() + @"'
                    ,'" + dr["ProcessName"].ToString() + "')";

                ExecuteNoneQuery(stmt, CommandType.Text);
            }

            //Delete data from table
            stmt = "Delete From ViewSMODependency";
            ExecuteNoneQuery(stmt, CommandType.Text);

            //Insert View SMO Relation to table
            for (int i = 0; i < dtViewSMO.Rows.Count; i++)
            {
                DataRow dr = dtViewSMO.Rows[i];
                stmt = @"INSERT INTO [dbo].[ViewSMODependency]
                     ([ViewGUID]
                    ,[SmartObjectGUID])
                    VALUES
                    ('" + dr["ViewGUID"].ToString() + @"'
                    ,'" + dr["SmartObjectGUID"].ToString() + "')";

                ExecuteNoneQuery(stmt, CommandType.Text);
            }

            //SmartObjectClientServer socs = new SmartObjectClientServer();
            //socs.CreateConnection();
            //socs.Connection.Open(sb.ToString());
            //FormsManager fm = new FormsManager(sb.ToString());
            //FormExplorer fe = fm.GetForms();
            //foreach (FormInfo fi in fe.Forms)
            //{                
            //    Console.WriteLine("Form: " + fi.DisplayName);
            //    ViewExplorer ve = fm.GetViewsForForm(fi.Guid);
            //    IEnumerable<string> x = fm.GetProcessesForForm(fi.Guid);

            //    foreach (var y in x)
            //    {
            //        Console.WriteLine("Process: " + y);
            //    }

            //    foreach (ViewInfo vi in ve.Views)
            //    {
            //        Console.WriteLine("View: " + vi.DisplayName);
            //        foreach(Guid soGuid in fm.GetObjectsForView(vi.Guid))
            //        {
            //            try
            //            {
            //                SmartObject so = socs.GetSmartObject(soGuid);
            //                Console.WriteLine("SmartObject: " + so.Name);
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.Message);
            //            }
            //        }
            //        Console.WriteLine();
            //    }
            //}
            //fm.Connection.Close();
            //socs.Connection.Close();
            //Console.ReadLine();

        }

        private static void ExecuteNoneQuery(string CommandName, CommandType CommType)
        {                                                          
            SqlConnection conn = new SqlConnection(connectionString);
            
            int resultInt = 0;
           
            try
            {
                using (SqlCommand comm = new SqlCommand(CommandName, conn))
                {                    
                    if (conn.State == ConnectionState.Open) { conn.Close(); }
                    conn.Open();
                    comm.CommandType = CommType;
                    resultInt = comm.ExecuteNonQuery();
                    comm.Dispose();                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); }
                //conn.Dispose();
            }            
        }
    }
}
