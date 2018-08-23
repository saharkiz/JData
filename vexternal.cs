using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Globalization;

/// <summary>
/// Summary description for vbooking
/// </summary>
public class vexternal
{
    public vexternal()
    {

    }
	public static string ver()
	{
		return "3.1";
	}
    #region GBD API CALLS
    public static string GBDAddTask(string CreatedBy, string marketcountry, string name, string content, string status, string revision, string location)
	{
        SqlCommand cmd = new SqlCommand("INSERT INTO t_ToDoTask(Name, Description, marketcountry, status, revision, location, CreatedOn, CreatedBy) VALUES (@name, @content, @marketcountry, @status, @revision, @location, GETDATE(), @user)");
     
        	cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@content", SqlDbType.VarChar).Value = System.Web.HttpUtility.HtmlEncode(content).Replace("\n", "<br/>");
            cmd.Parameters.Add("@marketcountry", SqlDbType.VarChar).Value = marketcountry;
        	cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = status;
            cmd.Parameters.Add("@revision", SqlDbType.VarChar).Value = System.Web.HttpUtility.HtmlEncode(revision).Replace("\n", "<br/>");
        	cmd.Parameters.Add("@location", SqlDbType.VarChar).Value = location;
        	cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = CreatedBy.ToUpper();

       	 	string saved = general.performActionNoTransClient(cmd,"GBD").ToString();
		return saved;

	}
    public static string GBDGetTopProducts()
    {
        SqlCommand command = new SqlCommand("Select distinct year from t_topproduct");
        DataSet ds2 = general.getSet(command, "GBD");

        SqlCommand command2 = new SqlCommand("Select distinct marketcountry from t_topproduct");
        DataSet ds3 = general.getSet(command2, "GBD");
        
        SqlCommand cmd = new SqlCommand("Select distinct month from t_topproduct");
        DataSet ds = general.getSet(cmd, "GBD");
        StringBuilder sbfinal = new StringBuilder();
        foreach (DataRow yr in ds2.Tables[0].Rows)
        {
            sbfinal.Append("{\"year\":\"" + yr["year"].ToString() + "\",\"markets\":");
            StringBuilder sbroot = new StringBuilder();
            foreach (DataRow market in ds3.Tables[0].Rows)
            {
                sbroot.Append("{\"marketcountry\":\"" + market["marketcountry"].ToString() + "\",\"months\":");
                StringBuilder sb = new StringBuilder();
                foreach (DataRow months in ds.Tables[0].Rows)
                {
                    sb.Append("{\"month\":\"" + months["month"].ToString() + "\",\"productname\":");

                    SqlCommand cmd2 = new SqlCommand("Select productname+'-'+productdescription as myproductname, UV from t_topproduct where marketcountry=@marketcountry AND month=@month AND year=@yr ORDER BY UV DESC");
                    cmd2.Parameters.Add("@marketcountry", SqlDbType.VarChar).Value = market["marketcountry"].ToString();
                    cmd2.Parameters.Add("@month", SqlDbType.VarChar).Value = months["month"].ToString();
                    cmd2.Parameters.Add("@yr", SqlDbType.VarChar).Value = yr["year"].ToString();
                    DataSet ds2s = general.getSet(cmd2, "GBD");
                    sb.Append(general.DStoJSON(ds2s));
                    sb.Append("},");
                }
                string data = "[" + sb.ToString().Substring(0, sb.ToString().Length - 1) + "]";
                sbroot.Append(data + "},");
            }
            string final = "[" + sbroot.ToString().Substring(0, sbroot.ToString().Length - 1) + "]";
            sbfinal.Append(final+"},");
        }
	return "["+ sbfinal.ToString().Substring(0,sbfinal.ToString().Length-1) + "]";
       
    }
	public static string GBDAddReport(string CreatedBy, string marketcountry, string name, string content, string status, string location)
	{
        SqlCommand cmd = new SqlCommand("INSERT INTO t_Report(marketcountry, name, content, status, location, CreatedOn, CreatedBy) VALUES (@marketcountry, @name, @content, @status, @location, GETDATE(), @user)");
        	cmd.Parameters.Add("@marketcountry", SqlDbType.VarChar).Value = marketcountry;
        	cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@content", SqlDbType.VarChar).Value = System.Web.HttpUtility.HtmlEncode(content).Replace("\n", "<br/>");
        	cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = status;
        	cmd.Parameters.Add("@location", SqlDbType.VarChar).Value = location;
        	cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = CreatedBy.ToUpper();

       	 	string saved = general.performActionNoTransClient(cmd,"GBD").ToString();
		return saved;
	}
	public static string GBDReadTask(string CreatedBy)
	{
		SqlCommand cmd = new SqlCommand("Select * from t_ToDoTask where CreatedBy=@user"); 
		cmd.Parameters.Add("@user", SqlDbType.VarChar).Value =  CreatedBy.ToUpper();;
		DataSet ds = general.getSet(cmd,"GBD");
        	return general.DStoJSON(ds);
	}
    public static string GBDReadSales(string CreatedBy)
    {
        SqlCommand cmd;
        if (CreatedBy == "default@the-v.net" || CreatedBy == "suresh.n@the-v.net" || CreatedBy == "jayakanth@the-v.net")
        {
            cmd = new SqlCommand("Select * from t_weeklySales order by cast(actualUSD AS FLOAT) desc");
        }
        else
        {
            cmd = new SqlCommand("SELECT * FROM t_weeklySales WHERE EXISTS (SELECT * FROM t_assignment WHERE t_assignment.BDUser=@user AND t_weeklySales.marketcountry = t_assignment.marketcountry) order by marketcountry ascs");
            cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = CreatedBy.ToUpper();
        }
        
        DataSet ds = general.getSet(cmd, "GBD");
        return general.DStoJSON(ds);
    }
	public static string GBDReadReport(string CreatedBy)
	{
		SqlCommand cmd = new SqlCommand("Select * from t_report where CreatedBy=@user"); 
		cmd.Parameters.Add("@user", SqlDbType.VarChar).Value =  CreatedBy.ToUpper();;
		DataSet ds = general.getSet(cmd,"GBD");
        	return general.DStoJSON(ds);
	}
    public static string GBDLoadMarketSelection(string username)
    {
        if (username == "default@the-v.net" || username == "suresh.n@the-v.net" || username == "jayakanth@the-v.net")
        {
            SqlCommand cmd = new SqlCommand("select * from vts_selections order by country asc");
            DataSet ds = general.getSet(cmd, "GBD");
            return general.DStoJSON(ds);
        }
        else
        {
            SqlCommand cmd = new SqlCommand("select * from t_assignment where BDUser=UPPER(@username)");
            cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
            DataSet ds = general.getSet(cmd, "GBD");
            return general.DStoJSON(ds);
        }

    }
    public static string GBDLoadUserSelection() 
    {
        SqlCommand cmd = new SqlCommand("select UPPER(FullName) as FullName,UPPER(Email) as id from t_users where (Department <> 'RESIGNED' or ROLE <> 'RESIGNED') and Department='GBD'");
        DataSet ds = general.getSet(cmd, "DBF_VGPI_Main");
        return general.DStoJSON(ds);
    }
    
    public static string GBDLoadPlans(string planType)
    {
        SqlCommand cmd = new SqlCommand("Select * from t_plans where PlanType=@planType");
        cmd.Parameters.Add("@planType", SqlDbType.VarChar).Value = planType;
        DataSet ds = general.getSet(cmd, "GBD");
        return general.DStoJSON(ds);
    }
    public static string GBDCheckLogin(string username, string password)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("select * from t_users where department='GBD' AND Username=@username AND Password=@password");
            cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;// general.getMd5Hash(password);
            DataSet ds = general.getSet(cmd, "DBF_VGPI_Main");
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string id = ds.Tables[0].Rows[0]["id"].ToString();
                    if (string.IsNullOrEmpty(id))
                    {
                        return "False";
                    }
                    else if (ds.Tables[0].Rows[0]["Department"].ToString() == "GBD")
                    {
                        return "True";
                    }
                    else {
                        return "False";
                    }
                }
                else return "False";
            }
            else return "False";
        }
        catch (Exception) 
        {
            return "False";
        }
    }

    public static string GBDGetUserLogin(string username, string password)
    {
        SqlCommand cmd = new SqlCommand("select * from t_users where department='GBD' AND Username=@username AND Password=@password");
        cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;// general.getMd5Hash(password);
        DataSet ds = general.getSet(cmd, "DBF_VGPI_Main");
        return general.DStoJSON(ds);
    }
    #endregion
    
    #region ISB API
    public static string ISBCheckLogin(string irid, string password)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM t_participant WHERE IRID=@IRID AND password=@password AND status='Approved'");
            cmd.Parameters.Add("@IRID", SqlDbType.NChar).Value = irid;
            cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
            DataSet ds = general.getSet(cmd, "ISB");
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                    return "True";
                else
                    return "False";
            }
            else
                return "False";
            
        }
        catch (Exception)
        {
            return "False";
        }
    }
    public static string ISBGetUserExperience(string irid)
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM t_experience WHERE participant_irid=@irid");
        cmd.Parameters.Add("@irid", SqlDbType.NChar).Value = irid;
        DataSet ds = general.getSet(cmd, "ISB");
        return general.DStoJSON(ds);
    }
    public static string ISBGetUserData(string irid)
    {
        SqlCommand cmd = new SqlCommand("SELECT IRID,email,f_name,m_name,l_name,card_name,gender, birth,yrs_business,upline_name,status FROM t_participant WHERE IRID=@IRID");
        cmd.Parameters.Add("@IRID", SqlDbType.NChar).Value = irid;
        DataSet ds = general.getSet(cmd, "ISB");
        return general.DStoJSON(ds);
    }
    public static string ISBGetUserRequestPrice(string eventID, string irid)
    {
        SqlCommand cmd = new SqlCommand("SELECT price FROM t_requests WHERE IRID=@IRID AND EventID=@EventID");
        cmd.Parameters.Add("@IRID", SqlDbType.NChar).Value = irid;
        cmd.Parameters.Add("@EventID", SqlDbType.UniqueIdentifier).Value = new Guid(eventID);
        DataSet ds = general.getSet(cmd, "ISB");
        return general.DStoJSON(ds);
    }
    public static string ISBCheckUserRequestStatus(string eventID, string irid)
    {
        SqlCommand cmd = new SqlCommand("SELECT status FROM t_requests WHERE IRID=@IRID AND EventID=@EventID");
        cmd.Parameters.Add("@IRID", SqlDbType.NChar).Value = irid;
        cmd.Parameters.Add("@EventID", SqlDbType.UniqueIdentifier).Value = new Guid(eventID);
        DataSet ds = general.getSet(cmd, "ISB");
        return general.DStoJSON(ds);
    }
    public static string ISBGetSelection(string grouping)
    {
        SqlCommand cmd = new SqlCommand("select * from ts_selections where grouping=@grouping order by data asc");
        cmd.Parameters.Add("@grouping", SqlDbType.VarChar).Value = grouping;
        DataSet ds = general.getSet(cmd, "ISB");
        return general.DStoJSON(ds);
    }
    public static string ISBSubmitExperience(string v_event, string country, string event_date, string inservice, string participant_irid)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO t_experience (CreatedOn, CreatedBy, v_event, country, event_date, inservice, participant_irid) Values (GETDATE(), @CreatedBy, @v_event, @country, @event_date, @inservice, @participant_irid)");
            cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = "App User: " + participant_irid;
            cmd.Parameters.Add("@v_event", SqlDbType.NVarChar).Value = v_event;
            cmd.Parameters.Add("@country", SqlDbType.NVarChar).Value = country;
            cmd.Parameters.Add("@event_date", SqlDbType.Date).Value = DateTime.Parse(event_date);
            cmd.Parameters.Add("@inservice", SqlDbType.NVarChar).Value = inservice;
            cmd.Parameters.Add("@participant_irid", SqlDbType.NChar).Value = participant_irid;
            string save = general.performActionNoTransClient(cmd, "ISB").ToString();
            return save;
        }
        catch (Exception)
        {
            return "False";
        }
    }
    public static string ISBSendEmail(string irid, string email)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("select * from t_participant where IRID=@irid AND email=@id");
            cmd.Parameters.Add("@irid", SqlDbType.VarChar).Value = irid;
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = email;
            DataSet ds = general.getSet(cmd, "ISB");

            MailAddress fromEmail = new MailAddress("vbox@the-v.net", "ISB App Forgot Password");

            MailMessage message = new MailMessage();
            message.From = fromEmail;
            message.To.Add(email);

            message.Subject = "ISB App Login Credantials";
            string temp = File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/invoice2.html"));
            temp = temp.Replace("$$$_RequestDate_$$$", ds.Tables[0].Rows[0]["CreatedOn"].ToString());
            temp = temp.Replace("$$$_IRIDL_$$$", ds.Tables[0].Rows[0]["IRID"].ToString());
            temp = temp.Replace("$$$_password_$$$", ds.Tables[0].Rows[0]["password"].ToString());
            temp = temp.Replace("$$$_NAME_$$$", ds.Tables[0].Rows[0]["f_name"].ToString() + " " + ds.Tables[0].Rows[0]["m_name"].ToString() + ". " + ds.Tables[0].Rows[0]["l_name"].ToString());
            temp = temp.Replace("$$$_EMAIL_$$$", ds.Tables[0].Rows[0]["email"].ToString());
            temp = temp.Replace("$$$_IRID_$$$", ds.Tables[0].Rows[0]["IRID"].ToString());

            message.Body = temp;
            message.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Send(message);
            return "Email Sent!";
        }
        catch (Exception ex)
        {
            return "Error sending Email!";
        }
    }
    public static string ISBSubmitApplication(string IRID,string email, string f_name,string m_name,string l_name,string card_name,string gender,string birth,string h_address,string city_address,string state_address,string zip_code,string country_address,string tel_no,string mobile_no,string language,string yrs_business,string net_name,string upline_name,string shirt_size,string ref_num,string v_position)
    {
        //send application
        //if true, upload images
        try
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO t_participant (IRID, email, f_name, m_name, l_name, card_name, gender, birth, h_address, city_address, state_address, zip_code, country_address, tel_no, mobile_no, language, yrs_business, net_name, upline_name, shirt_size, ref_num, v_position, CreatedOn, CreatedBy, status) VALUES (@IRID, @email, @f_name, @m_name, @l_name, @card_name, @gender, @birth, @h_address, @city_address, @state_address, @zip_code, @country_address, @tel_no, @mobile_no, @language, @yrs_business, @net_name, @upline_name, @shirt_size, @ref_num, @v_position, GETDATE(), @CreatedBy, @status)");
            cmd.Parameters.Add("@IRID", SqlDbType.NChar).Value = IRID;
            cmd.Parameters.Add("@email", SqlDbType.NChar).Value = email;
            cmd.Parameters.Add("@f_name", SqlDbType.NVarChar).Value = f_name;
            cmd.Parameters.Add("@m_name", SqlDbType.NVarChar).Value = m_name;
            cmd.Parameters.Add("@l_name", SqlDbType.NVarChar).Value = l_name;
            cmd.Parameters.Add("@card_name", SqlDbType.NVarChar).Value = card_name;
            if (gender == "Male")
                cmd.Parameters.Add("@gender", SqlDbType.Bit).Value = false;
            else
                cmd.Parameters.Add("@gender", SqlDbType.Bit).Value = true;
            cmd.Parameters.Add("@birth", SqlDbType.Date).Value = birth;
            cmd.Parameters.Add("@h_address", SqlDbType.NVarChar).Value = h_address;
            cmd.Parameters.Add("@city_address", SqlDbType.NVarChar).Value = city_address;
            cmd.Parameters.Add("@state_address", SqlDbType.NVarChar).Value = state_address;
            cmd.Parameters.Add("@zip_code", SqlDbType.Int).Value = Convert.ToInt32(zip_code);
            cmd.Parameters.Add("@country_address", SqlDbType.NVarChar).Value = country_address;
            cmd.Parameters.Add("@tel_no", SqlDbType.VarChar).Value = tel_no;
            cmd.Parameters.Add("@mobile_no", SqlDbType.VarChar).Value = mobile_no;
            cmd.Parameters.Add("@language", SqlDbType.VarChar).Value = language; //get language values
            cmd.Parameters.Add("@yrs_business", SqlDbType.Int).Value = yrs_business;
            cmd.Parameters.Add("@net_name", SqlDbType.UniqueIdentifier).Value = new Guid(net_name);
            cmd.Parameters.Add("@upline_name", SqlDbType.NVarChar).Value = upline_name;
            cmd.Parameters.Add("@shirt_size", SqlDbType.NVarChar).Value = shirt_size;
            cmd.Parameters.Add("@ref_num", SqlDbType.VarChar).Value = ref_num;
            cmd.Parameters.Add("@v_position", SqlDbType.NVarChar).Value = v_position;
            cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = "App User: " + IRID;
            cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "Pending";
            string save = general.performActionNoTransClient(cmd, "ISB").ToString();
            return save;
        }
        catch (Exception)
        {
            return "False";
        }
    }
    public static string ISBGetEventsList()
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM v_Event where status='NEW' AND ISB='True' ORDER BY CreatedOn DESC");
        DataSet ds = general.getSet(cmd, "DBF_VReg_Live");
        return general.DStoJSON(ds);
    }
    public static string ISBGetSingleEvent(string id)
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM v_Event where id=@id");
        cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = new Guid(id);
        DataSet ds = general.getSet(cmd, "DBF_VReg_Live");
        return general.DStoJSON(ds);
    }
    public static string ISBGetUserRequestList(string irid)
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM t_requests where IRID=@IRID AND status!='DONE' ORDER BY CreatedOn DESC");
        cmd.Parameters.Add("@IRID", SqlDbType.NChar).Value = irid;
        DataSet ds = general.getSet(cmd, "ISB");
        return general.DStoJSON(ds);
    }
    public static string ISBSubmitAction(string eventID, string irid, string requestType, string eventName, string eventDate, string eventVenue)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO t_requests (EventID,IRID,RequestType, EventName,CreatedBy, CreatedOn, status, EventDate, EventVenue) VALUES (@EventID,@IRID,@RequestType,@EventName,@CreatedBy,GETDATE(),@status,@EventDate,@EventVenue)");
            cmd.Parameters.Add("@EventID", SqlDbType.UniqueIdentifier).Value = new Guid(eventID);
            cmd.Parameters.Add("@EventName", SqlDbType.VarChar).Value = eventName;
            cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "Pending";
            cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = "App user: "+irid;
            cmd.Parameters.Add("@IRID", SqlDbType.NChar).Value = irid;
            cmd.Parameters.Add("@RequestType", SqlDbType.VarChar).Value = requestType;
            cmd.Parameters.Add("@EventDate", SqlDbType.VarChar).Value = eventDate;
            cmd.Parameters.Add("@EventVenue", SqlDbType.VarChar).Value = eventVenue;
            string saved = general.performActionNoTransClient(cmd, "ISB").ToString();
            return saved;
        }
        catch (Exception)
        {
            return "False";
        }
    }
    public static string ISBCheckRequestAvailable(string eventID, string irid)
    {
        try {
            SqlCommand cmd = new SqlCommand("Select * from t_requests where EventID=@EventID AND IRID=@IRID");
            cmd.Parameters.Add("@EventID", SqlDbType.UniqueIdentifier).Value = new Guid(eventID);
            cmd.Parameters.Add("@IRID", SqlDbType.NChar).Value = irid;
            DataSet ds = general.getSet(cmd, "ISB");
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                    return "False";
                else
                    return "True";
            }
            else
                return "True";
        }
        catch (Exception)
        {
            return "False";
        }
    }
    #endregion

    #region VCON API
    public static string VCONAskDatoLoadQuestion(string irid, string email)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("Select * from t_pledge where irid=@irid AND email=@email");
            cmd.Parameters.Add("@irid", SqlDbType.NVarChar).Value = irid;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
            DataSet ds = general.getSet(cmd, "DBF_V2014_EX");
            return general.DStoJSON(ds);
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    public static string VCONAskDatoSubmitQuestion(string irid, string email, string question)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO t_pledge(email,irid,note,Status,CreatedOn,CreatedBy)VALUES(@email,@irid,@question,'Submitted',GETDATE(),@email)");
            cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
            cmd.Parameters.Add("@irid", SqlDbType.NVarChar).Value = irid;
            cmd.Parameters.Add("@question", SqlDbType.NVarChar).Value = System.Web.HttpUtility.HtmlEncode(question).Replace("\n", "<br/>");
            string saved = general.performActionNoTransClient(cmd, "DBF_V2014_EX").ToString();
            return saved;
        }
        catch (Exception ex) 
        {
            return "False";
        }
    }
    public static string VCONPollQuestions(string lang)
    {
        try
          {
            SqlCommand cmd = new SqlCommand("Select * from t_PollQuestion WHERE tag='VCONSurvey' AND Status='Active'");
            cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = lang;
            DataSet ds = general.getSet(cmd, "DBF_V2014_EX");
            StringBuilder sbfinal = new StringBuilder();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow q in ds.Tables[0].Rows)
                    {
                        sbfinal.Append("{\"id\":\"" + q["id"].ToString() + "\",\"pollTitle\":\"" + q["pollTitle"].ToString() + "\",\"pollQuestion\":\"" + q["pollQuestion"].ToString() + "\",\"language\":\"" + q["language"].ToString() + "\",\"answers\":");
                        SqlCommand cmd2 = new SqlCommand("Select id,Answer from t_PollAnswers where QuestionID=@qID");
                        cmd2.Parameters.Add("@qID", SqlDbType.UniqueIdentifier).Value = new Guid(q["id"].ToString());
                        DataSet ds2 = general.getSet(cmd2, "DBF_V2014_EX");
                        sbfinal.Append(general.DStoJSON(ds2));
                        sbfinal.Append(",\"answerID\":\"\"},");
                    }
                }
                else { return "[]"; }
            }
            else { return "[]"; }
            return "[" + sbfinal.ToString().Substring(0, sbfinal.ToString().Length - 1) + "]";
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    public static string VCONSubmitPollAnswers(string id)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("UPDATE t_PollAnswers SET Votes = Votes + 1 where id=@id");
            cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = new Guid(id);
            string saved = general.performActionNoTransClient(cmd, "DBF_V2014_EX").ToString();
            return saved;
        }
        catch (Exception ex)
        {
            return "False";
        }
    }
    public static string VCONGetTeams()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT Answer,id from t_PollAnswers WHERE QuestionID=@qid ORDER BY Answer asc");
            cmd.Parameters.Add("@qid", SqlDbType.UniqueIdentifier).Value = new Guid("6c48a05a-f007-4e67-9be2-712ddfd00398");
            DataSet ds = general.getSet(cmd, "DBF_V2014_EX");
            return general.DStoJSON(ds);
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    public static string VCONSubmitVote(string id)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("UPDATE t_PollAnswers SET Votes = Votes + 1 where id=@id");
            cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = new Guid(id);
            string saved = general.performActionNoTransClient(cmd, "DBF_V2014_EX").ToString();
            return saved;
        }
        catch (Exception ex)
        {
            return "False";
        }
    }
    public static string VCONGetWinner()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT Answer from t_PollAnswers WHERE QuestionID=@qid AND Status='Winner'");
            cmd.Parameters.Add("@qid", SqlDbType.UniqueIdentifier).Value = new Guid("6c48a05a-f007-4e67-9be2-712ddfd00398");
            DataSet ds = general.getSet(cmd, "DBF_V2014_EX");
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    return dr["Answer"].ToString();
                }
                else
                    return "";
            }
            else
                return "";
        }
        catch (Exception ex)
        {
            return "Error";
        }
    }
    public static string VCONVotingStatus()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SELECT Status FROM t_PollQuestion where tag='VCONSuperstar'");
            DataSet ds = general.getSet(cmd, "DBF_V2014_EX");
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    return dr["Status"].ToString();
                }
                else
                    return "";
            }
            else
                return "";
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    #endregion
   
}