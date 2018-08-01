using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Net;
using System.Configuration;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Web.Configuration;

public partial class jData
{
    public string dfault()
    {
        return "[]";
    }
    public string returnCheck(DataSet ds)
    {
        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                return general.DStoJSON(ds);
            }
            else return dfault();
        }
        else
        {
            return dfault();
        }
    }

    #region QA App

    public string QA_verify(String irid, String password)
    {
        if (password == ConfigurationManager.AppSettings["AppPassword"].ToString())
        {
            SqlCommand cmd = new SqlCommand("select * from View_QA_verify where irid=@irid");
            cmd.Parameters.Add("@irid", SqlDbType.VarChar).Value = irid;
            DataSet ds = general.getSet(cmd, "V_Members");
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string QA_fixActivation(string irid, String password)
    {
        if (password == ConfigurationManager.AppSettings["AppPassword"].ToString())
        {
            SqlCommand cmd = new SqlCommand("sp_QA_fix_activation");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@irid", SqlDbType.VarChar).Value = irid;
            DataSet ds = general.getSet(cmd, "V_Members");
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string QA_Manual_App_QNET(string email, String password)
    {
        if (password == ConfigurationManager.AppSettings["AppPassword"].ToString())
        {
            SqlCommand cmd = new SqlCommand("update t_members set CreatedBy='QNET User', UpdatedBy='API_QA', membership='12 Months', membership_end=DATEADD(m, 12, GETDATE()), membership_start=GETDATE()  where email=@email");
            cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
            bool isSaved = general.performActionNoTransClient(cmd, "V_Members");
            return general.TextToJson(isSaved.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string QA_fixEmail(string irid, string newemail, string password)
    {
        if (password == ConfigurationManager.AppSettings["AppPassword"].ToString())
        {
            SqlCommand cmd = new SqlCommand("update t_members set user_name=@email, email=@email where irid=@irid");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@irid", SqlDbType.VarChar).Value = irid;
            cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = newemail;
            bool isSaved = general.performActionClient(cmd, "V_Members");
            if (isSaved)
            {
                return general.TextToJson("Succes. please verify IR ID.");
            }
            else
            {
                return general.TextToJson("Fail. More than one user using the same IR ID.");
            }

        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string QA_temp_vcon_ir(string irid, String password)
    {
        if (password == ConfigurationManager.AppSettings["AppPassword"].ToString())
        {
            SqlCommand cmd = new SqlCommand("select * from temp_view_RedemptionList where irid=@irid");
            cmd.Parameters.Add("@irid", SqlDbType.VarChar).Value = irid;
            DataSet ds = general.getSet(cmd, "V2014_EX");
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string QA_temp_vcon(String password)
    {
        if (password == ConfigurationManager.AppSettings["AppPassword"].ToString())
        {
            SqlCommand cmd = new SqlCommand("select * from temp_view_RedemptionList");
            DataSet ds = general.getSet(cmd, "V2014_EX");
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }

    #endregion
    #region App
    public string App_Search(String keyword)
    {
        string searchWord = keyword.Replace(",", "").Replace("'", "").Replace("’", "");
        SqlCommand cmd = new SqlCommand("sp_SearchResult"); //returns only IDs of specific Search
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@SearchKey", SqlDbType.VarChar).Value = searchWord;
        cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = "Video";

        DataSet ds = general.getSet(cmd);
        if (ds.Tables.Count > 0)
        {
            SqlCommand cm = new SqlCommand("sp_Results"); //gets actual values
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.Add("@Type", SqlDbType.VarChar).Value = "Video";
            cm.Parameters.Add("@table", SqlDbType.Structured).Value = ds.Tables[0];
            cm.Parameters.Add("@orderby", SqlDbType.VarChar).Value = "latest";
            DataSet dsResult = general.getSet(cm);
            return returnCheck(dsResult);
        }

        return general.TextToJson("No Search results.");

    }
    public string addAppUser(string username, string lng, string lat, string uuid, string grouping, string device, string model, string version)
    {
        try
        {

            SqlCommand cmd = new SqlCommand(@"IF(NOT EXISTS(SELECT username FROM t_appUsers where username=@USERNAME)) BEGIN INSERT INTO t_appUsers(username, lng, lat, uuid, [grouping], device, model, [version],[status],createdOn, updatedOn, createdBy,updatedBy) VALUES(@USERNAME, @LNG, @LAT, @UUID,@GROUPING,@DEVICE,@MODEL,@VERSION,'Active',GETDATE(), GETDATE(), @CREATEDBY, @UPDATEDBY) END");
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@LNG", SqlDbType.VarChar).Value = lng;
            cmd.Parameters.Add("@LAT", SqlDbType.VarChar).Value = lat;
            cmd.Parameters.Add("@UUID", SqlDbType.VarChar).Value = uuid;
            cmd.Parameters.Add("@DEVICE", SqlDbType.VarChar).Value = device;
            cmd.Parameters.Add("@MODEL", SqlDbType.VarChar).Value = model;
            cmd.Parameters.Add("@GROUPING", SqlDbType.VarChar).Value = grouping;
            cmd.Parameters.Add("@VERSION", SqlDbType.VarChar).Value = version;
            cmd.Parameters.Add("@CREATEDBY", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@UPDATEDBY", SqlDbType.VarChar).Value = username;

            bool saved = general.performAction(cmd);

            return general.TextToJson(saved.ToString());
        }
        catch (Exception ex)
        {
            ex.Message.ToString();
            return general.TextToJson(false.ToString());
        }
    }
    public string addAppChecking(string myevent, string uuid, string username, string placename, string lat, string lng)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("IF(NOT EXISTS(SELECT username FROM t_appCheckin where username=@USERNAME))BEGIN INSERT INTO t_appCheckin([event], uuid, username, placename,[status], lat, lng, createdOn, updatedOn, createdBy,updatedBy) VALUES(@MYEVENT, @UUID, @USERNAME, @PLACENAME,'Active', @LAT, @LNG, GETDATE(), GETDATE(), @CREATEDBY, @UPDATEDBY) END");
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@MYEVENT", SqlDbType.VarChar).Value = myevent;
            cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@LNG", SqlDbType.VarChar).Value = lng;
            cmd.Parameters.Add("@LAT", SqlDbType.VarChar).Value = lat;
            cmd.Parameters.Add("@UUID", SqlDbType.VarChar).Value = uuid;
            cmd.Parameters.Add("@PLACENAME", SqlDbType.VarChar).Value = placename;
            cmd.Parameters.Add("@CREATEDBY", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@UPDATEDBY", SqlDbType.VarChar).Value = username;

            bool saved = general.performAction(cmd);
            return general.TextToJson(saved.ToString());

        }
        catch (Exception ex)
        {
            ex.Message.ToString();
            return general.TextToJson(false.ToString());
        }
    }
    public string suggestPlace()
    {
        SqlCommand cmd = new SqlCommand("SELECT DISTINCT(placename) FROM t_appCheckin");
        DataSet ds = general.getSet(cmd);

        return returnCheck(ds);

    }
    public string checkAppUser(string username = "", string uuid = "")
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM t_appUsers WHERE ((Username = isnull(@USER,'')) OR (UUID=isnull(@UUID,'')))");

        cmd.Parameters.Add("@USER", SqlDbType.VarChar).Value = username;
        cmd.Parameters.Add("@UUID", SqlDbType.VarChar).Value = uuid;
        DataSet ds = general.getSet(cmd);

        return returnCheck(ds);
    }
    public string getCheckinLeaders()
    {
        SqlCommand cmd = new SqlCommand("SELECT username, placename FROM t_appCheckin GROUP BY username,placename");
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string App_Video_AddLikes(string userid, string id)
    {
        SqlCommand cmd = new SqlCommand("UPDATE vs_entry_details SET Likes = isnull(Likes,0) + 1  WHERE Id = @id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
        bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
        return general.TextToJson(save.ToString());
    }
    #endregion
    #region translation

    public string getTranslation(string language)
    {
        SqlCommand cmd = new SqlCommand("EXEC('SELECT runningNum, word, '+@LANGUAGE +' as lang FROM t_translation')");
        cmd.Parameters.Add("@LANGUAGE", SqlDbType.VarChar).Value = language;
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }


    #endregion

    #region website
    public string getOldsNews(string count, string page, string language)
    {
        int start = 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);
        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from view_news  where Language=@lang) p
                    WHERE status <> 'Inactive' and language=@lang and rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language.ToString();
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getOldsNewsCategory(string count, string page, string language, string category)
    {
        int start = 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);
        SqlCommand cmd = new SqlCommand(@"
                    SELECT top 20 * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from view_news  where Language=@lang) p
                    WHERE status <> 'Inactive' and language=@lang and SubCategory=@Category
                    order by rowNumber");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = category.ToString();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language.ToString();
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getOldsNewsMarket(string count, string page, string language, string Market)
    {
        int start = 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);
        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from view_news  where Language=@lang) p
                    WHERE status <> 'Inactive' and language=@lang and Market=@Market and rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@Market", SqlDbType.VarChar).Value = Market.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language.ToString();
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getOldsNewsCountry(string count, string page, string language, string Country)
    {
        int start = 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);
        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from view_news  where Language=@lang) p
                    WHERE status <> 'Inactive' and language=@lang and Country=@Country and rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@Country", SqlDbType.VarChar).Value = Country.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language.ToString();
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getOldsEvents(string count, string page, string language)
    {
        int start = 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);
        SqlCommand cmd = new SqlCommand(@"
                    select * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from view_events  where Language=@lang) p
                    WHERE status <> 'Inactive' and language=@lang and SubParentID is null and rowNumber between @start and @end
                    order by EventStartOn desc");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language.ToString();
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getOldsEventsCountry(string count, string page, string language, string Country)
    {
        int start = 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);
        SqlCommand cmd = new SqlCommand(@"
                    select * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from view_events  where Language=@lang) p
                    WHERE status <> 'Inactive' and language=@lang  and Country=@Country and SubParentID is null and rowNumber between @start and @end
                    order by EventStartOn desc");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@Country", SqlDbType.VarChar).Value = Country.ToString();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language.ToString();
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getOldsEventsMarket(string count, string page, string language, string Market)
    {
        int start = 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);
        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from view_events  where Language=@lang and Market=@Market and status <> 'Inactive') p
                    WHERE status <> 'Inactive' and language=@lang  and Market=@Market and SubParentID is null and rowNumber between @start and @end
                    order by EventStartOn desc");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@Market", SqlDbType.VarChar).Value = Market.ToString();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language.ToString();
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getOldsEventsCategory(string count, string page, string language, string Category)
    {
        int start = 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);
        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from view_events  where Language=@lang and SubCategory=@Category and status <> 'Inactive') p
                    WHERE status <> 'Inactive' and language=@lang  and SubCategory=@Category and SubParentID is null and rowNumber between @start and @end
                    order by EventStartOn desc");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = Category.ToString();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language.ToString();
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getOldsSubEvents(string id, string language)
    {
        SqlCommand cmd = new SqlCommand("select * from View_Events where SubParentId=@id and language=@lang and RecordStatus='Published' order by EventStartOn ");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language;
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }

    public string getLatestNews()
    {
        SqlCommand cmd = new SqlCommand(@"select * from view_news where Cast(CreatedOn as date) = cast(GETDATE() as date)");
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getNews(string URL, string language)
    {
        SqlCommand cmd = new SqlCommand(@"select * from view_news where URL=@URL and language=@lang");
        cmd.Parameters.Add("@URL", SqlDbType.VarChar).Value = URL.ToString();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language.ToString();
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getLatestNewsByDate(String mydate)
    {
        SqlCommand cmd = new SqlCommand(@"select * from view_news where Cast(CreatedOn as date) = cast(@mydate as date)");
        cmd.Parameters.Add("@mydate", SqlDbType.VarChar).Value = mydate;
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getLatestEvent()
    {
        SqlCommand cmd = new SqlCommand(@"select *  from view_events where Cast(CreatedOn as date) = cast(GETDATE() as date)");
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getEvents(string URL, string language)
    {
        SqlCommand cmd = new SqlCommand(@"select *  from view_events where URL=@URL and language=@lang and subparentid is null");
        cmd.Parameters.Add("@URL", SqlDbType.VarChar).Value = URL.ToString();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language.ToString();
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getLatestEventByDate(String mydate)
    {
        SqlCommand cmd = new SqlCommand(@"select replace(replace(REPLACE(description,'""',''''),char(10),' '),char(13),' ') as description,id, runningNum, Title,localTitle, replace(replace(REPLACE(Summary,'""',''''),char(10),' '),char(13),' ') as Summary,ImageLink,Language,Hits,Rating,language,status,CreatedOn from t_events where Cast(CreatedOn as date) = cast(@mydate as date)");
        cmd.Parameters.Add("@mydate", SqlDbType.VarChar).Value = mydate;
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string get_Search(String keyword, string mytype)
    {
        string searchWord = keyword.Replace(",", "").Replace("'", "").Replace("’", "");
        SqlCommand cmd = new SqlCommand("sp_SearchResult"); //returns only IDs of specific Search
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@SearchKey", SqlDbType.VarChar).Value = searchWord;
        cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = mytype;

        DataSet ds = general.getSet(cmd);
        if (ds.Tables.Count > 0)
        {
            SqlCommand cm = new SqlCommand("sp_Results"); //gets actual values
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.Add("@Type", SqlDbType.VarChar).Value = mytype;
            cm.Parameters.Add("@table", SqlDbType.Structured).Value = ds.Tables[0];
            cm.Parameters.Add("@orderby", SqlDbType.VarChar).Value = "latest";
            DataSet dsResult = general.getSet(cm);
            return returnCheck(dsResult);
        }

        return general.TextToJson("No Search results.");

    }
    public string getNewsMoods(string moodname)
    {
        SqlCommand cmd = new SqlCommand("select sum(Wow) as wow,sum(Lol) as lol,sum(Heart) as heart,sum(Sad) as sad,sum(Angry) as angry from t_News");
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string getNewsMoodsList(string moodname, string language)
    {
        SqlCommand cmd = new SqlCommand("select top 20 * from view_News where MOOD=@mood and language=@lang");

        cmd.Parameters.Add("@mood", SqlDbType.VarChar).Value = moodname.ToUpper();
        cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = language;
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }


    #region Account Methods
    public string register_site(string Email, string Password, string FirstName, string IRID, string cmbcountry, string groupName, string year)
    {
        try
        {
            //GetSocialMedia();
            SqlCommand cmd = new SqlCommand("sp_Register");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = Email;
            cmd.Parameters.Add("@nonencpassword", SqlDbType.VarChar).Value = Password;
            cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = general.getMd5Hash(Password);
            cmd.Parameters.Add("@fullname", SqlDbType.NVarChar).Value = FirstName;
            cmd.Parameters.Add("@IRID", SqlDbType.VarChar).Value = IRID;

            cmd.Parameters.Add("@Country", SqlDbType.VarChar).Value = cmbcountry.ToString();
            cmd.Parameters.Add("@Group", SqlDbType.VarChar).Value = groupName;
            cmd.Parameters.Add("@Year", SqlDbType.VarChar).Value = year;

            cmd.Parameters.Add("@facebook_id", SqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@googleId", SqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@liveId", SqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@yahooId", SqlDbType.VarChar).Value = "";

            string result = general.getSingleData(cmd, "V_Members");
            if (result == "Thank you")
            {
                string temp = File.ReadAllText(HttpContext.Current.Server.MapPath("~/communications/EmailTemplates/RegistrationActivation.htm"));

                string tempURL = "http://site.the-v.net/login"+ "?activation=" + HttpUtility.UrlEncode(general.EncryptIt(Email));
                string bodyTemplate = temp.Replace("$$$_NAME_$$$", FirstName).Replace("$$$_ACTIONURL_$$$", tempURL);
                general.sendMail(Email, "Site Activation", bodyTemplate, "Upline Notification");
                return general.TextToJson("The activation code has been sent to your email. Please check your email.");
            }
            else
            {
                return general.TextToJson(result);

            }
        }
        catch (Exception)
        {
            return general.TextToJson("Error");
        }
    }
    public string forgotpassword_site(string emailUser)
    {
        try
        {

            if (string.IsNullOrEmpty(emailUser.Trim())) return general.TextToJson("Invalid Email");
            SqlCommand cmds = new SqlCommand("select email from t_members where email=@email");
            cmds.Parameters.Add("@email", SqlDbType.VarChar).Value = emailUser;
            string email = general.getSingleData(cmds, "V_Members");
            if (string.IsNullOrEmpty(email.Trim())) return general.TextToJson("Invalid Email");

            string resetKey = general.GetRandomPasswordUsingGUID(30); // Signature(key);
            SqlCommand cmd = new SqlCommand("INSERT INTO t_Reset_PW_Key(generated_key,timespan) VALUES(@reset_key,@timespan)");
            cmd.Parameters.Add("@reset_key", SqlDbType.VarChar).Value = resetKey;
            cmd.Parameters.Add("@timespan", SqlDbType.VarChar).Value = DateTime.Now.ToString("s");
                //DateTime.Parse(HttpContext.Current.Request[DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)]);

            general.performActionNoTransClient(cmd, "V_Members");

            string resetLink = "http://site.the-v.net/reset-password?token=" + resetKey + "&uid=" + email.Trim(); ;
            string body = File.ReadAllText(HttpContext.Current.Server.MapPath("~/communications/EmailTemplates/ForgotPasswordRequest.htm"));

            body = body.Replace("$$$_Name_$$$", email);
            body =  body.Replace("$$$_Link_$$$", resetLink);
            
            bool isSuccess = general.sendMail(email, "Forgot Password Link", body.ToString(), "Upline Notification");

            if (isSuccess)
            {
                return general.TextToJson("Reset password was sent to your email");
            }
            else
            {
                return general.TextToJson("Oops! Looks like something went wrong while we were sending you the Reset Password email. Kindly click the submit button again to resend email.");
            }
            //return general.TextToJson("An error occurred during forgot password. Please contact administrator..");
        }
        catch (Exception ex)
        {
            return general.TextToJson("An error occurred during forgot password. Please contact administrator." + ex.ToString());
        }

    }
    public string reactivate_site(string emailUser)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("select email from t_members where email=@email");
            cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = emailUser;
            string email = general.getSingleData(cmd, "V_Members");
            if (!string.IsNullOrEmpty(email))
            {
                string temp = File.ReadAllText(HttpContext.Current.Server.MapPath("~/communications/EmailTemplates/UserReactivation.htm"));
                string bodyTemplate = temp.Replace("$$$_NAME_$$$", email).Replace("$$$_ACTIONURL_$$$", "http://site.the-v.net/login" + "?activation=" + HttpUtility.UrlEncode(general.EncryptIt(email)));

                    bool isSuccess = general.sendMail(email, "Site Reactivation", bodyTemplate, "Upline Notification");
                    if (isSuccess)
                    {
                        return general.TextToJson("Reset password was sent to your email");
                    }
                    else
                    {
                        return general.TextToJson("Oops! Looks like something went wrong while we were sending you the Reset Password email. Kindly click the submit button again to resend email.");
                    }
            }
            else
            {
                return general.TextToJson("Email does not Exists. Please <a href='/account/registration'>create an account. <a/>");
            }
        }
        catch (Exception ex)
        {
            return general.TextToJson("Email does not Exists. Please Contact your administrator.");
        }
    }
    
    #endregion account methods

    #endregion

    #region Product
    public string Clear_Cart()
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.shoppingCartId))
        {
            SqlCommand cmd = new SqlCommand(@"update t_orders set status='Cleared', RecordStatus='Deleted' where status <> 'Completed' and
                                case when userid='' and cartid=@unId then 1 when userid<>'' and userid=@unId then 1 end = 1");
            if (string.IsNullOrEmpty(MainLibrary.Helper.UserId))
                cmd.Parameters.Add("@unId", SqlDbType.VarChar).Value = MainLibrary.Helper.shoppingCartId;
            else if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
                cmd.Parameters.Add("@unId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            general.performActionNoTrans(cmd);
            MainLibrary.Helper.ExpireShoppingCart();
            return general.TextToJson("Cleared");
        }
        else
        {
            return general.TextToJson("Please login and try again.");
        }
    }
    public string Product_GetAll(string count, string page = "1")
    {
        int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from View_ProductList_API) p
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }

    public string Product_GetProducts(string count, string category, string criteria, string page = "1")
    {
        switch (criteria.ToLower())
        {
            case "purchasable": return Product_GetPurchasableByCategory(count, category, page);

            case "redeemable": return Product_GetRedeemableByCategory(count, category, page);

            case "watchable": return Product_GetWatchableByCategory(count, category, page);

            default: return Product_GetAll(count, page);

        }
    }

    public string Product_GetAllMultipleCategory(string count, string categories, string criteria, string page = "1")
    {
        switch (criteria.ToLower())
        {
            case "purchasable": Product_GetPurchasableByCategory(count, categories, page);
                break;
            case "redeemable": Product_GetRedeemableByCategory(count, categories, page);
                break;
            case "watchable": Product_GetWatchableByCategory(count, categories, page);
                break;
        }
        int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from View_ProductList_API) p
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }

    public string Product_GetPurchasable(string count, string page = "1")
    {
        int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from View_ProductList_API where purchasable=1) p
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }

    public string Product_GetPurchasableByCategory(string count, string category, string page = "1")
    {
        int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    declare @query varchar(8000)
                    set @query = '
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* 
                    from View_ProductList_API where category in ('''+replace(@category,'|',''',''')+''') and purchasable=1) p 
                    WHERE rowNumber between '+@start+' and '+@end+' 
                    order by rowNumber' 
                    exec(@query)");
        cmd.Parameters.Add("@category", SqlDbType.VarChar).Value = category;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }

    public string Product_GetWatchable(string count, string page = "1")
    {
        int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from View_ProductList_API where klik=1) p
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }

    public string Product_GetWatchableByCategory(string count, string category, string page = "1")
    {
        int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    declare @query varchar(8000)
                    set @query = '
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* 
                    from View_ProductList_API where category in ('''+replace(@category,'|',''',''')+''') and klik=1) p 
                    WHERE rowNumber between '+@start+' and '+@end+' 
                    order by rowNumber' 
                    exec(@query)");
        cmd.Parameters.Add("@category", SqlDbType.VarChar).Value = category;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }

    public string Product_GetRedeemable(string count, string page = "1")
    {
        int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from View_ProductList_API where redeemable=1) p
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }

    public string Product_GetRedeemableByCategory(string count, string category, string page = "1")
    {
        int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    declare @query varchar(8000)
                    set @query = '
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* 
                    from View_ProductList_API where category in ('''+replace(@category,'|',''',''')+''') and redeemable=1) p 
                    WHERE rowNumber between '+@start+' and '+@end+' 
                    order by rowNumber' 
                    exec(@query)");
        cmd.Parameters.Add("@category", SqlDbType.VarChar).Value = category;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }

    public string Product_GetByCategory(string count, string category, string page = "1")
    {
        int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from View_ProductList_API where category=@category) p
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@category", SqlDbType.VarChar).Value = category;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }

    public string Product_AddToWishlist(string linkURL)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("insert into t_wishlist (userid,productid,linkurl,createdon,status) values (@userid,@productid,@linkurl,GETDATE(),'NEW')");
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            cmd.Parameters.Add("@productid", SqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@linkurl", SqlDbType.VarChar).Value = linkURL;
            bool saved = general.performActionNoTrans(cmd);
            return general.TextToJson(saved.ToString());
        }
        return general.TextToJson("Please login and try again.");
    }
    public string Product_RemoveWishlist(string linkURL)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("delete from t_wishlist where id=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = linkURL;
            bool saved = general.performActionNoTrans(cmd);
            return general.TextToJson(saved.ToString());
        }
        return general.TextToJson("Please login and try again.");
    }
    public string Wishlist_GetAll()
    {
        SqlCommand cmd = new SqlCommand("select * from view_wishlist where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string Product_AddReview(string id, string msg)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("insert into t_reviews (userid,productid,msg,createdon,status) values (@userid,@productid,@msg,GETDATE(),'NEW')");
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            cmd.Parameters.Add("@productid", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@msg", SqlDbType.VarChar).Value = msg;
            bool save = general.performActionNoTrans(cmd);
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string Product_GetReviews(string id)
    {
        SqlCommand cmd = new SqlCommand("select * from View_reviews where linkURL = @linkURL order by createdon desc");
        cmd.Parameters.Add("@linkURL", SqlDbType.VarChar).Value = id;
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string Product_GetDetails(string linkURL)
    {
        SqlCommand cmd = new SqlCommand("select TOP 1 * from View_ProductList_API where linkURL = @linkURL");
        cmd.Parameters.Add("@linkURL", SqlDbType.VarChar).Value = WebUtility.HtmlDecode(linkURL);
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }

    public string Product_AddLikes(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("UPDATE t_products SET Likes = isnull(Likes,0) + 1  WHERE LinkUrl = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            bool save = general.performActionNoTrans(cmd);
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Please login and try again.");
        }
    }

    public string Product_RemoveCart(string id)
    {
        try
        {

            string query = @"UPDATE t_Orders set recordstatus='Deleted'
                              WHERE id = @productId
                              and CartType = @cartType                            
                              and Status in ('In Shopping Cart' ,'Pending')  
                              and case when userid='' and cartid=@unId then 1 when userid<>'' and userid=@unId then 1 end = 1";

            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.Add("@productId", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@cartType", SqlDbType.VarChar).Value = "PRODUCT";
            if (string.IsNullOrEmpty(MainLibrary.Helper.UserId))
                cmd.Parameters.Add("@unId", SqlDbType.VarChar).Value = MainLibrary.Helper.shoppingCartId;
            else if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
                cmd.Parameters.Add("@unId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            bool saved = general.performActionNoTrans(cmd);
            return general.TextToJson(saved.ToString());
        }
        catch (Exception ex)
        {
            return general.TextToJson("False");
        }
    }

    public string Product_GetPurchasedProductsDownload()
    {
        string query = @"  SELECT  distinct  p.ID,p.Title as title,p.ThumbnailURL as [image],CONVERT(VARCHAR(12), isnull(dc.create_date,GETDATE()) , 107) as [date],'Claimed' Type, p.category, p.year, p.author,'' filesize, 'MP4' filetype, p.FileToDownload as link
                                    FROM vs_membership_dvd_claimed dc inner join t_Products p on dc.product_id = p.ID
                                    where user_id = @userId and p.ProductType in ('DVD','Apps')
                                        UNION
                                    SELECT  distinct  p.ID,p.Title,p.ThumbnailURL,CONVERT(VARCHAR(12), isnull(o.CreatedOn,GETDATE()) , 107) as [date],'Purchased' Type, p.category, p.year, p.author,'' filesize, 'MP4' filetype, p.FileToDownload as link
                                    FROM t_Orders o inner join t_Products p on o.productID = p.ID
                                    where o.RecordStatus = 'Active' and o.Status = 'Completed' and p.ProductType in ('DVD','Apps') and o.userID = @userId
                                    order by Title
                                    ";
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            DataSet ds = general.getSet(cmd);
            return returnCheck(ds);
        }
        return general.TextToJson("Please login and try again.");
    }

    public string Product_GetRedeemedProductsDownload()
    {
        string query = @"SELECT  distinct  p.ID,p.Title as title,p.ThumbnailURL as [image],CONVERT(VARCHAR(12), isnull(o.createdon,GETDATE()) , 107) as [date],'Claimed' Type, p.category, p.year, p.author,'' filesize, 'MP4' filetype, p.FileToDownload as link
                                    FROM t_Orders o inner join t_Products p on o.productID = p.ID
                                    where o.RecordStatus = 'Active' and o.Status = 'Completed' and p.ProductType in ('DVD','Apps') and o.userID = @userId
UNION
                                    SELECT  distinct  p.ID,p.Title,p.ThumbnailURL,CONVERT(VARCHAR(12), isnull(o.CreatedOn,GETDATE()) , 107) as [date],'Redeemed' Type, p.category, p.year, p.author,'' filesize, 'MP4' filetype, p.FileToDownload as link
                                    FROM t_Orders o inner join t_Products p on o.productID = p.ID
                                    where o.RecordStatus = 'Active' and o.Status = 'Redeemed' and p.ProductType in ('DVD','Apps') and o.userID = @userId
                                    order by Title";
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            DataSet ds = general.getSet(cmd);
            return returnCheck(ds);
        }
        return general.TextToJson("Please login and try again.");
    }

    public string Product_RedeemNow(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            //aresh this needs to be fixed.
            SqlCommand cmd = new SqlCommand(@"insert into t_orders (id,OrderDate,userID,ProductName,ProductID,cartType,status,createdon,recordStatus) VALUES
                                                (newid(), GETDATE(),@id,@name, (select top 1 id from t_products where title=@name), 'REDEEM','REDEEMED',GETDATE(),'Active')");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = id;
            bool saved = general.performActionNoTrans(cmd);
            //redeemable points
            if (saved)
            {
                SqlCommand cmd2 = new SqlCommand(@"update t_members set 
                            redeemable_points= (redeemable_points-(select top 1 redeemPoints from V2014_ex.dbo.t_products where title=@name)) where id=@id");
                cmd2.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
                cmd2.Parameters.Add("@name", SqlDbType.VarChar).Value = id;
                saved = general.performActionNoTransClient(cmd2, "V_members");
            }
            return general.TextToJson(saved.ToString());
        }
        return general.TextToJson("Please login and try again.");
    }
    #endregion

    #region Dashboard
    public string App_UserVideoPlaylist(string userid)
    {
        SqlCommand cmd = new SqlCommand("select * from view_UserVideoPlaylist where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string UserVideoPlaylist()
    {
        SqlCommand cmd = new SqlCommand("select * from view_UserVideoPlaylist where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string App_purchasedProducts(string userid)
    {
        SqlCommand cmd = new SqlCommand("select * from view_purchasedProducts where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "v2014_ex");
        return returnCheck(ds);
    }
    public string purchasedProducts()
    {
        SqlCommand cmd = new SqlCommand("select * from view_purchasedProducts where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "v2014_ex");
        return returnCheck(ds);
    }

    public string UserFollowerApproved()
    {
        SqlCommand cmd = new SqlCommand("select * from view_UserFollowerApproved where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string UserFollowerRequest()
    {
        SqlCommand cmd = new SqlCommand("select distinct * from view_UserFollowerRequest where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string App_UserFollowing(string userid)
    {
        SqlCommand cmd = new SqlCommand("select distinct * from view_UserFollowing where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string UserFollowing()
    {
        SqlCommand cmd = new SqlCommand("select distinct * from view_UserFollowing where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string reddemedProducts()
    {
        SqlCommand cmd = new SqlCommand("select * from view_reddemedProducts where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "v2014_ex");
        return returnCheck(ds);
    }
    public string App_UserMyVideos(string userid)
    {
        SqlCommand cmd = new SqlCommand("select * from view_UserMyVideos where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string UserMyVideos()
    {
        SqlCommand cmd = new SqlCommand("select * from view_UserMyVideos where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string unlockedVideos()
    {
        SqlCommand cmd = new SqlCommand("select * from view_unlockedVideos where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string UserKlikPlaylist()
    {
        SqlCommand cmd = new SqlCommand("select * from view_UserKlikPlaylist where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string klik_remove(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("Delete from t_watchlater WHERE Id = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            bool save = general.performActionNoTransClient(cmd, "V2014_Ex");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string App_UserChannel(string userid)
    {
        SqlCommand cmd = new SqlCommand("select * from view_UserChannel where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string UserChannel()
    {
        SqlCommand cmd = new SqlCommand("select * from view_UserChannel where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string App_AccountProfile(string userid)
    {
        SqlCommand cmd = new SqlCommand("select TOP 1 * from AccountProfile where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "V_Members");
        return returnCheck(ds);
    }
    public string AccountProfile()
    {
        SqlCommand cmd = new SqlCommand("select TOP 1 * from AccountProfile where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId; ;
        DataSet ds = general.getSet(cmd, "V_Members");
        return returnCheck(ds);
    }

    public string follow_request(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("insert into vs_subscribe_channel (channel_id,subscriber_user_id,is_approved,create_date) select channel_id,subscriber_user_id,0,GETDATE() from vs_subscribe_channel where channel_id=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string follow_reject(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("delete from vs_subscribe_channel WHERE Id = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }

    public string App_ChangePassword(string id, string NewPassword, string OldPassword)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("UPDATE [t_Members] SET [passwd] = @passwd, [NonEncPassword] = @NonEncPassword, last_password_changed_date = getdate()  WHERE [id] = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@passwd", SqlDbType.VarChar).Value = general.getMd5Hash(NewPassword);
            cmd.Parameters.Add("@oldPass", SqlDbType.VarChar).Value = general.getMd5Hash(OldPassword);
            cmd.Parameters.Add("@NonEncPassword", SqlDbType.VarChar).Value = NewPassword;

            bool save = general.performActionNoTransClient(cmd, "V_Members");
            return general.TextToJson(save.ToString());
        }
        catch (Exception ex)
        {
            return general.TextToJson("Operation not Successful.");
        }
    }
    #endregion
}

