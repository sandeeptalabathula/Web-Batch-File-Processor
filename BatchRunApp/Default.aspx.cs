using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.IO;
using System.Web.Configuration;
using System.Data;

namespace BatchRunApp
{
    public partial class _Default : System.Web.UI.Page
    { 
        // Get the full file path
        string strWorkingDirectory = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                strWorkingDirectory = WebConfigurationManager.AppSettings["workingDirectory"];
                pnlResult.Visible = false;

                if (!Page.IsPostBack)
                {
                    BindBatchGrid();
                }
            }
            catch (UnauthorizedAccessException unauthEx)
            {
                Response.Write("Application not getting permission to Working directory you specified in web.config. <br/>" + 
                    unauthEx.Message + "<br/>" + unauthEx.StackTrace);
            }
            catch (Exception ex)
            {
                Response.Write("Ooops! something went wrong.. <br/> Error Detail:<br/>" + ex.Message + "<br/>" + ex.StackTrace);
            }
        }

        private void BindBatchGrid()
        {
            //Get Files
            string[] filePaths = Directory.GetFiles(strWorkingDirectory + @"\", "*.bat", SearchOption.TopDirectoryOnly);
            if (filePaths != null && filePaths.Length > 0)
            {
                //Prepare Data
                DataTable dt = new DataTable();
                dt.Columns.Add("FileName", Type.GetType("System.String"));
                dt.Columns.Add("FilePath", Type.GetType("System.String"));
                dt.Columns.Add("RunDate", Type.GetType("System.String"));
                FileInfo info;
                for (int i = 0; i < filePaths.Length; i++)
                {
                    info = new FileInfo(filePaths[i]);
                    dt.Rows.Add();
                    dt.Rows[dt.Rows.Count - 1]["FileName"] = Path.GetFileNameWithoutExtension(filePaths[i]);
                    dt.Rows[dt.Rows.Count - 1]["FilePath"] = filePaths[i];
                 }
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {

            string fileExt = Path.GetExtension(batchFileLoader.FileName);

            if (batchFileLoader.HasFile && fileExt == ".bat" && !string.IsNullOrEmpty(strWorkingDirectory))
            {
                string fileName = batchFileLoader.FileName;
                //Save the uploaded file on server
                string strFilePath = strWorkingDirectory + @"\" + GetUniqueFileName(fileName) + ".bat";
                batchFileLoader.SaveAs(strFilePath);

                ExecuteBatchFileCommands(fileName, strFilePath);

                BindBatchGrid();
            }
            else
            {
                Response.Write("<h2>Sorry! Please choose batch file to run.<h2>");
            }

        }

        private void ExecuteBatchFileCommands(string fileName, string strFilePath)
        {
            try
            {
                // Create the ProcessInfo object
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardError = true;
                psi.WorkingDirectory = strWorkingDirectory; 

                // Start the process
                System.Diagnostics.Process proc = System.Diagnostics.Process.Start(psi);

                // Open the batch file for reading
                System.IO.StreamReader strm = System.IO.File.OpenText(strFilePath);

                // Attach the output for reading
                System.IO.StreamReader sOut = proc.StandardOutput;

                // Attach the in for writing
                System.IO.StreamWriter sIn = proc.StandardInput;

                // Write each line of the batch file to standard input
                while (strm.Peek() != -1)
                {
                    sIn.WriteLine(strm.ReadLine());
                }
                strm.Close();

                // Exit CMD.EXE
                string stEchoFmt = "# {0} run successfully. Exiting..";


                sIn.WriteLine(String.Format(stEchoFmt, strFilePath));
                sIn.WriteLine("EXIT");

                // Close the process
                proc.Close();

                // Read the sOut to a string.
                string results = sOut.ReadToEnd().Trim();

                // Close the io Streams;
                sIn.Close();
                sOut.Close();

                // Write out the results.
                string fmtStdOut = "<font face=courier size=0>{0}</font>";
                //this.Response.Write(String.Format(fmtStdOut, results.Replace(System.Environment.NewLine, "<br>")));

                string outputFile = strWorkingDirectory + @"\" + GetUniqueFileName(fileName) + ".html";
                using (StreamWriter outfile = new StreamWriter(outputFile))
                {
                    outfile.Write(String.Format(fmtStdOut, results.Replace(System.Environment.NewLine, "<br>")));

                    hlnkResults.NavigateUrl = @"file:///" + outputFile;
                    pnlResult.Visible = true;
                } 
            }
            catch (UnauthorizedAccessException unauthEx)
            {
                Response.Write("Working directory you specified in web.config does not have write permission. <br/>" + unauthEx.Message + "<br/>" + unauthEx.StackTrace);
            }
            catch (Exception ex)
            {
                Response.Write("Ooops! something went wrong.. <br/> Error Detail:<br/>" + ex.Message + "<br/>" + ex.StackTrace);
            }
        }

        internal static string GetUniqueFileName(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                //string fileName = filePathAndName.Substring(filePathAndName.Trim().LastIndexOf('\\'));
                string strDate, strTime, strDateTime;
                strDate = DateTime.Now.ToString("MMddyyy").Replace("/", "");
                strTime = DateTime.Now.ToString("hhmmss");
                strDateTime = strDate + "_" + strTime;

                string strFileNameOnServer = fileName.Substring(0, fileName.LastIndexOf('.')) + "_" + strDateTime;

                return strFileNameOnServer;
            }
            else
                return string.Empty;
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RunBatch")
            {
                string[] commandArgs = e.CommandArgument.ToString().Split(new char[] { ',' });
                string filePath = Convert.ToString(commandArgs[0]);
                int rowIndex = Convert.ToInt32(commandArgs[1].ToString());
                string fileName = Path.GetFileName(filePath);
                if (!string.IsNullOrEmpty(fileName))
                {
                    ExecuteBatchFileCommands(fileName, filePath);
                }
                else
                {
                    Response.Write("We couldn't find this file on disk");
                }
            }
        }
    }
}
