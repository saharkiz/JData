using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;

public partial class jData
{
    public string Portal_Email_Verify(String email)
    {
        SqlCommand cmd = new SqlCommand(@"select * from view_portal_user where username=@username");
        cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = email;
        DataSet ds = general.getSet(cmd, "brighttankportal");
        return general.DStoJSON(ds);
    }
    public string Portal_Task_MyTask(String email)
    {
        SqlCommand cmd = new SqlCommand(@"select replace(replace(REPLACE(TaskName,'""',''''),char(10),' '),char(13),' ') as TaskName,joid,id,
                           replace(replace(REPLACE(TaskRemarks,'""',''''),char(10),' '),char(13),' ') as TaskRemarks, AssignedTo,CreatedBy,CreatedOn from view_task where createdBy=@user");
        cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = email;
        DataSet ds = general.getSet(cmd, "brighttankportal");
        return general.DStoJSON(ds);
    }
    public string Portal_Task(String user, String hours, String task, String actions)
    {
        SqlCommand cmd = new SqlCommand(@"INSERT INTO t_task (status, createdon, createdby, timeSpent, timeMin, taskName)
        VALUES (@action, getdate(), @user, @hours, 0, @task)");
        cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = user;
        cmd.Parameters.Add("@hours", SqlDbType.VarChar).Value = hours;
        cmd.Parameters.Add("@task", SqlDbType.VarChar).Value = task;
        cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = actions;
        bool saved = general.performActionNoTransClient(cmd, "brighttankportal");
        return general.TextToJson(saved.ToString());
    }
    public string Portal_Taskjo(String user, String hours, String task, String actions, String joid)
    {
        SqlCommand cmd = new SqlCommand(@"INSERT INTO t_task (status, createdon, createdby, timeSpent, timeMin, taskName, joid)
        VALUES (@action, getdate(), @user, @hours, 0, @task, @joid)");
        cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = user;
        cmd.Parameters.Add("@hours", SqlDbType.VarChar).Value = hours;
        cmd.Parameters.Add("@task", SqlDbType.VarChar).Value = task;
        cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = actions;
        cmd.Parameters.Add("@joid", SqlDbType.VarChar).Value = joid;
        bool saved = general.performActionNoTransClient(cmd, "brighttankportal");
        return general.TextToJson(saved.ToString());
    }
    public string Portal_User(String username, String password)
    {
        SqlCommand cmd = new SqlCommand(@"
                    select * from view_portal_user where username=@username and password = @password");
        cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
        DataSet ds = general.getSet(cmd, "brighttankportal");
        return general.DStoJSON(ds);
    }
    public string Portal_Request(String refno, String description, String user,String status)
    {
        SqlCommand cmd = new SqlCommand("INSERT INTO t_requests (createdBy,status,refNumber,request, createdOn) VALUES (@user,@status,@refno,@description, getDate())");
        cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = user;
        cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = status;
        cmd.Parameters.Add("@refno", SqlDbType.VarChar).Value = refno;
        cmd.Parameters.Add("@description", SqlDbType.VarChar).Value = description;
        bool saved = general.performActionNoTransClient(cmd, "brighttankportal"); 
        return general.TextToJson(saved.ToString());
    }
    public string Portal_Request_GetAll(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT  replace(replace(REPLACE(refNumber,'""',''''),char(10),' '),char(13),' ') as refNumber
,id, createdBy,createdon,status,updatedBy,UpdatedOn, 
replace(replace(REPLACE(request,'""',''''),char(10),' '),char(13),' ') as request FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* FROM t_requests) c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "brighttankportal");
        return general.DStoJSON(ds);
    }
    public string Portal_Project_GetAll(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* FROM t_projects) c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "brighttankportal");
        return general.DStoJSON(ds);
    }
    public string Portal_JO_GetAll(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* FROM t_jo) c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "brighttankportal");
        return general.DStoJSON(ds);
    }
    public string Portal_Task_GetAll(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT replace(replace(REPLACE(TaskName,'""',''''),char(10),' '),char(13),' ') as TaskName,joid,status,id,
                           replace(replace(REPLACE(TaskRemarks,'""',''''),char(10),' '),char(13),' ') as TaskRemarks, AssignedTo,CreatedBy,CreatedOn  FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* FROM t_task) c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "brighttankportal");
        return general.DStoJSON(ds);
    }

    public string Portal_Task_GetSingle(string joid, string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT replace(replace(REPLACE(TaskName,'""',''''),char(10),' '),char(13),' ') as TaskName,joid,status,id,
                           replace(replace(REPLACE(TaskRemarks,'""',''''),char(10),' '),char(13),' ') as TaskRemarks, AssignedTo,CreatedBy,CreatedOn  FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* FROM t_task WHERE joid = @joid) c
                    WHERE rowNumber between 1 and 100 and joid = @joid
                    order by rowNumber
                    ");

        cmd.Parameters.Add("@joid", SqlDbType.VarChar).Value = joid.ToString();
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "brighttankportal");
        return general.DStoJSON(ds);
    }

}
