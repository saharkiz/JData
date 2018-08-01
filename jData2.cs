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
using System.Configuration;

public partial class jData
{
    #region VCON APP
    public string vcon_checkin(string location, string name)
    {
        SqlCommand cmd = new SqlCommand("insert into t_checkins(location,name) values (@location,@name)");
        cmd.Parameters.Add("@location", SqlDbType.VarChar).Value = location;
        cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
        bool save = general.performActionNoTransClient(cmd, "V_Members");
        return general.TextToJson(save.ToString());
    }
    public string vcon_tally()
    {
        SqlCommand cmd = new SqlCommand(@"select * from vcon_locations");
        DataSet ds = general.getSet(cmd, "V_Members");
        return general.DStoJSON(ds);
    }
    #endregion

    #region KLIK
    public string Mob_KLIK_GetProduct(string linkUrl, string userid)
    {
            SqlCommand cmd = new SqlCommand(@"SELECT * ,
                                (SELECT COUNT(id) FROM t_WatchLater where productid = p.id and userId = @userId) watch
                                FROM View_ProductWatch p where productType = 'DVD' and WatchEntryId = @linkURL");
            cmd.Parameters.Add("@linkUrl", SqlDbType.VarChar).Value = linkUrl;
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd);
            return general.DStoJSON(ds);
       
    }
    public string KLIK_GetProduct(string linkUrl)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
        SqlCommand cmd = new SqlCommand(@"SELECT * ,
                                (SELECT COUNT(id) FROM t_WatchLater where productid = p.id and userId = @userId) watch
                                FROM View_ProductWatch p where productType = 'DVD' and linkurl = @linkURL");
        cmd.Parameters.Add("@linkUrl", SqlDbType.VarChar).Value = linkUrl;
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string KLIK_GetList()
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand(@"select *,(SELECT COUNT(id) FROM t_WatchLater where productid = p.id and userId = @userId) watch FROM View_ProductWatch p where producttype = 'DVD' and isnull(WatchEntryId,'') <> ''  order by title");
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            DataSet ds = general.getSet(cmd);
            return general.DStoJSON(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string KLIK_BasicList(string userid)
    {
            SqlCommand cmd = new SqlCommand(@"select *,(SELECT COUNT(id) FROM t_WatchLater p where productid = p.id and userId = @userId) watch FROM View_ProductWatch where producttype = 'DVD' and isnull(WatchEntryId,'') <> ''  and category = 'Basic Training Videos' order by title");
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd);
            return general.DStoJSON(ds);
    }
    public string KLIK_IntermediateList(string userid)
    {
        SqlCommand cmd = new SqlCommand(@"select *,(SELECT COUNT(id) FROM t_WatchLater p where productid = p.id and userId = @userId) watch FROM View_ProductWatch where producttype = 'DVD' and isnull(WatchEntryId,'') <> '' and category = 'Intermediate Training Videos' order by title");
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }
    public string KLIK_AdvanceList(string userid)
    {
        SqlCommand cmd = new SqlCommand(@"select *,(SELECT COUNT(id) FROM t_WatchLater p where productid = p.id and userId = @userId) watch FROM View_ProductWatch where producttype = 'DVD' and isnull(WatchEntryId,'') <> '' and category = 'Advanced Training Videos' order by title");
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }
    public string Mob_KLIK_GetList(string userid)
    {
            SqlCommand cmd = new SqlCommand(@"select *,(SELECT COUNT(id) FROM t_WatchLater where productid = p.id and userId = @userId) watch FROM View_ProductWatch p where producttype = 'DVD' and isnull(WatchEntryId,'') <> ''  order by title");
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd);
            return general.DStoJSON(ds);
    }
    public string Mob_KLIK_Search(string userid, string title)
    {
        SqlCommand cmd = new SqlCommand(@"select *,(SELECT COUNT(id) FROM t_WatchLater where productid = p.id and userId = @userId) watch FROM View_ProductWatch p where producttype = 'DVD' and LinkURL=@url and isnull(WatchEntryId,'') <> ''  order by title");
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        cmd.Parameters.Add("@url", SqlDbType.VarChar).Value = title;
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }
    #endregion
    #region Channel
    public string myChannel_update(string id, string title, string description, string allowComments, string allowpublish)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_channel SET name=@title, description=@description, access_type=@allowpublish, is_comment_allowed=@allowComments where id=@id and user_id=@userid");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@title", SqlDbType.VarChar).Value = title;
            cmd.Parameters.Add("@description", SqlDbType.VarChar).Value = description;
            cmd.Parameters.Add("@allowComments", SqlDbType.VarChar).Value = allowComments;
            cmd.Parameters.Add("@allowpublish", SqlDbType.VarChar).Value = allowpublish;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }

    public string Channel_Update(object File,string name, string description)
    {
        return "";
    }
    public string Channel_GetAll(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* FROM View_ChannelList_API) c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Channel_GetModerator(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* FROM View_ChannelList_API WHERE user_id = 'AF3102AD-09C6-4E44-9872-2C3695F5A1F6') c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Channel_GetMostViewed(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY views desc) rowNumber,* FROM View_ChannelList_API) c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Channel_GetMostFollowed(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY Followers desc) rowNumber,* FROM View_ChannelList_API) c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Channel_GetMostNumberOfVideos(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY NumVideos desc) rowNumber,* FROM View_ChannelList_API) c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Channel_GetLatest(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* FROM View_ChannelList_API) c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Channel_GetRecommended(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn asc) rowNumber,* FROM View_ChannelList_API where is_recommended=1) c
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Channel_GetUserChannels(string count, string userId)
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM View_ChannelList_API where user_id = @userId order by CreatedOn desc");

        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Channel_GetFollowers(string count, string idorname, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,* from View_FollowList_API where is_approved=1 and (channelname=@idorname or cast(channelid as varchar(10))=@idorname)) p
                    WHERE rowNumber between @start and @end
                    order by rowNumber");

        cmd.Parameters.Add("@idorname", SqlDbType.VarChar).Value = idorname;
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Channel_GetDetails(string idorname)
    {
        SqlCommand cmd = new SqlCommand("select TOP 1 * from View_ChannelList_API where (name=@idorname or cast(id as varchar(10))=@idorname)");

        cmd.Parameters.Add("@idorname", SqlDbType.VarChar).Value = idorname;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Channel_FollowChannel(string idorname)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand(@"
                declare @channelId bigint
                declare @autofollow bit

                SELECT top 1 @channelId=id,@autofollow=followAutoApprove FROM vs_channel where cast(id as varchar(500))=@idorname or name=@idorname

                INSERT INTO vs_subscribe_channel (channel_id,subscriber_user_id,is_approved,create_date)
                     VALUES (@channelId,@userId,@autofollow,getdate())
                select @autofollow
                ");
            cmd.Parameters.Add("@idorname", SqlDbType.VarChar).Value = idorname;
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            string save = general.getSingleData(cmd, "V2014_Tube");

            return general.TextToJson(save.ToString());

        }
        else
        {
            return general.TextToJson("LoginRequired");
        }
    }
    public string Channel_UnfollowChannel(string idorname)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand(@"
                DELETE  FROM vs_subscribe_channel
                WHERE   channel_id in (SELECT id FROM vs_channel where cast(id as varchar(500))=@idorname or name=@idorname) 
                    and subscriber_user_id=@userId
                ");
            cmd.Parameters.Add("@idorname", SqlDbType.VarChar).Value = idorname;
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("LoginRequired");
        }
    }

    public string Channel_GetUserFollowChannel(string idorname)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand(@"
                SELECT is_approved FROM [V2014_Tube].[dbo].[vs_subscribe_channel] 
                where channel_id = (SELECT top 1 id FROM vs_channel where cast(id as varchar(500))=@idorname or name=@idorname) 
                    and subscriber_user_id=@userId
                ");
            cmd.Parameters.Add("@idorname", SqlDbType.VarChar).Value = idorname;
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            string save = general.getSingleData(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("LoginRequired");
        }
    }

    public string Channel_GetChannelFromVideoId(string id)
    {
        SqlCommand cmd = new SqlCommand("SELECT Channel_id FROM vs_entry_details where id=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
        string channelId = general.getSingleData(cmd, "V2014_Tube");
        return general.TextToJson(channelId);
    }

    public string channel_remove(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_channel SET delete_date=GETDATE()  WHERE Id = @id  and user_id=@userid");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }

    public string follow_accept(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_subscribe_channel set is_approved=1, update_date=getdate() WHERE Id = @id and subscriber_user_id=@userid");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }

    public string follow_remove(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_subscribe_channel set is_approved=0, update_date=getdate() WHERE Id = @id and subscriber_user_id=@userid");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string follow_back(string id)
    {
        return general.TextToJson("Not Available.");
    }
    #endregion

    #region User
    public static string TicketId
    {
        get
        {
            try
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["ATSARESH"];
                if (cookie != null)
                {
                    try
                    {
                        var ticket = FormsAuthentication.Decrypt(cookie.Value);
                        string data = ticket.UserData;
                        return data;
                    }
                    catch (Exception ex)
                    {
                        new eException(ex.Message.ToString());
                    }
                }
                else
                {
                    return "NON";
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }
    }
    public static string getTicket()
    {
        return general.TextToJson(TicketId);
    }
    public static string LoginSuccessTicket(string userid)
    {

        FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(1, userid, DateTime.Now, DateTime.Now.AddMinutes(30), true, userid);
        string cookiestr = FormsAuthentication.Encrypt(tkt);
        HttpCookie ck = new HttpCookie("ATSARESH", cookiestr);
        ck.Expires = tkt.Expiration;
        ck.Domain = ".the-v.net";
        ck.Path = "/";
        //ck.HttpOnly = true;
        HttpContext.Current.Response.Cookies.Add(ck);
        return general.TextToJson(TicketId);
    }
    public string User_Shoutout(string id)
    {
        SqlCommand cmd = new SqlCommand("select * from t_ShoutOuts where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
        string save = general.getSingleData(cmd, "V_Members");
        if (save != "")
        {
            return general.TextToJson(save);
        }
        return general.TextToJson("");
    }

    public string User_GetAll(string count)
    {
        SqlCommand cmd = new SqlCommand("SELECT top 1 * FROM View_Members_API");
        DataSet ds = general.getSet(cmd, "V_Members");
        return general.DStoJSON(ds);
    }

    public string User_GetTopLeaders(string count, string type)
    {
        string query = string.Empty;
        SqlCommand cmd = new SqlCommand();
        string avoid = @"is_moderator = 0 ";
        switch (type)
        {
            case "OVERALL": query = string.Format(@"SELECT TOP {0} *  FROM View_Members_API where {1} order by points desc", count, avoid);
                break;
            case "WEEKLY": query = string.Format("SELECT TOP {0} sum(newpoint) points,imageUser,username FROM DBF_V2014_Ex.dbo.t_PointHistory ph inner join View_Members_API m on m.id = ph.userid where DATEPART(week, ph.createdon) = @week and datepart(year,ph.createdon) = datepart(year,getdate()) and {1} group by imageuser,username order by points desc", count, avoid);
                cmd.Parameters.Add("@week", SqlDbType.VarChar).Value = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
                break;
            case "DAILY": query = string.Format("SELECT TOP {0} sum(newpoint) points,imageUser,username FROM DBF_V2014_Ex.dbo.t_PointHistory ph inner join View_Members_API m on m.id = ph.userid where cast(ph.createdon as date) = cast(@date as date) and {1} group by imageuser,username  order by points desc", count, avoid);
                cmd.Parameters.Add("@date", SqlDbType.VarChar).Value = DateTime.Now;
                break;
        }
        cmd.CommandText = query;
        DataSet ds = general.getSet(cmd,"V_Members");
        return general.DStoJSON(ds);
    }

    public string User_GetRecentlyWatchVideo(string count, string id, string page = "1")
    {
        int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedOn desc)  rowNumber,
                     username,imageUser from View_Members_API m inner join DBF_V2014_Ex.dbo.t_PointHistory p on m.id=p.userid
                     where reference=@id) v
                    WHERE rowNumber between @start and @end 
                    order by rowNumber");
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
        DataSet ds = general.getSet(cmd, "V_Members");
        return general.DStoJSON(ds);
    }

    public string User_GetRecentActivities(string count, string page = "1")
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
            int end = Convert.ToInt32(count) * Convert.ToInt32(page);

            SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedOn desc)  rowNumber,isnull(description,activity) activitydesc,
                    username,imageUser from t_PointHistory p inner join DBF_V_Members.dbo.View_Members_API m on m.id=p.userid
                    where m.id=@userId) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
            cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
            cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            DataSet ds = general.getSet(cmd);
            return general.DStoJSON(ds);
        }
        else
        {
            return string.Empty;
        }
    }

    public string User_GetData(string id)
    {
        SqlCommand cmd = new SqlCommand("select TOP 1 * from View_Members_API where id=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
        DataSet ds = general.getSet(cmd, "V_Members");
        return general.DStoJSON(ds);
    }

    public string User_GetFollowingChannels(string count, string page = "1")
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
            int end = Convert.ToInt32(count) * Convert.ToInt32(page);

            SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,channelUrlReal,privacy) channelUrl,* from View_FollowList_API where userid=@userid and is_approved=1) p
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
            cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
            cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
            DataSet ds = general.getSet(cmd, "V2014_Tube");
            return general.DStoJSON(ds);
        }
        else
        {
            return string.Empty;
        }
    }

    public string User_GetLoggedInUserData()
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("select TOP 1 * from View_Members_API where id=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            DataSet ds = general.getSet(cmd, "V_Members");
            return general.DStoJSON(ds);
        }
        else
        {
            return string.Empty;
        }
    }

    public string User_GetShoppingItemsCount()
    {
        string str = MainLibrary.Helper.GetShoppingItemsCount(MainLibrary.Helper.shoppingCartId, "product").ToString();
        return general.TextToJson(str);
    }

    private string User_GetSingleUser(string id)
    {
        SqlCommand cmd = new SqlCommand("select TOP 1 * from View_Members_API where id=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
        DataSet ds = general.getSet(cmd, "V_Members");
        return general.DStoJSON(ds);
    }

    public string User_GetFollowChannel(string id)
    {
        return general.TextToJson("Feature Not Available");
    }

    public string User_GetRedemptions()
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("select * from view_RedemptionList where userId=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            DataSet ds = general.getSet(cmd);
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }

    public string myUser_update(string name, string bday, string gender)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("UPDATE t_members set first_name=@name, birthday=@bday, Gender=@gender  where id=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
            try
            {
                cmd.Parameters.Add("@bday", SqlDbType.VarChar).Value = Convert.ToDateTime(bday).ToString("s");
            }
            catch (Exception)
            {
                cmd.Parameters.Add("@bday", SqlDbType.VarChar).Value = DateTime.Now.ToString("s");
            }
            cmd.Parameters.Add("@gender", SqlDbType.VarChar).Value = gender;
            bool save = general.performActionNoTransClient(cmd, "V_Members");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string checklogin(string email, string password)
    {
        //CHECK IF PAID MEMBER
        SqlCommand cmd = new SqlCommand("select top 1 id from view_members where isQnetUser is not null and email=@email and NonEncPassword=@password");
        cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
        string save = general.getSingleData(cmd, "V_Members");
        if (save != "")
        {
            return general.TextToJson(save);
        }
        return general.TextToJson("");
    }
    public string checkloginMob(string email, string password)
    {
        //CHECK IF PAID MEMBER
        SqlCommand cmd = new SqlCommand("select top 1 id,irid,membership,first_name,membership_end,email,points from view_members where isQnetUser is not null and email=@email and NonEncPassword=@password");
        cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
        DataSet ds = general.getSet(cmd, "V_Members");
        return general.DStoJSON(ds);
    }

    public string checkloginIR(string irid, string password)
    {
        SqlCommand cmd = new SqlCommand("select top 1 id from t_members where irid=@irid and NonEncPassword=@password");
        cmd.Parameters.Add("@irid", SqlDbType.VarChar).Value = irid;

        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
        string save = general.getSingleData(cmd, "V_Members");
        if (save != "")
        {
            return general.TextToJson(save);
        }
        return general.TextToJson("");
    }
    public string checkloginNonPaid(string email, string password)
    {
        SqlCommand cmd = new SqlCommand("select top 1 id from t_members where email=@email and NonEncPassword=@password");
        cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;

        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
        string save = general.getSingleData(cmd, "V_Members");
        if (save != "")
        {
            return general.TextToJson(save);
        }
        return general.TextToJson("");
    }
    public string checklogintype(string email, string password, string type)
    {
        //VAmbassadors Login MemberType='VA'
        SqlCommand cmd = new SqlCommand("select top 1 id from t_members where email=@email and NonEncPassword=@password and MemberType=@type");
        cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;

        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
        cmd.Parameters.Add("@type", SqlDbType.VarChar).Value = type;
        string save = general.getSingleData(cmd, "V_Members");
        if (save != "")
        {
            return general.TextToJson(save);
        }
        return general.TextToJson("");
    }
    public string login(string email, string password)
    {
        SqlCommand cmd = new SqlCommand("select top 1 id from t_members where email=@email and NonEncPassword=@password");
        cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = email;

        cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = password;
        string save = general.getSingleData(cmd, "V_Members");
        if (save != "")
        {
            bool pointAdded = gamify.point(save, "login");
            if (pointAdded)
            {
                return gamify.point("login");
            }
        }
        return general.TextToJson("");
    }

    public string sp_login(string email, string password)
    {
        SqlCommand cmd = new SqlCommand("SP_Account_Login");
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@email", System.Data.SqlDbType.VarChar).Value = email;
        cmd.Parameters.Add("@irid", System.Data.SqlDbType.VarChar).Value = email;
        cmd.Parameters.Add("@password", System.Data.SqlDbType.VarChar).Value = password;// general.getMd5Hash(passWord);
        string ret = general.getSingleData(cmd, "V_Members");
        if (ret == "NotActivated")
        {
            return "[{\"error\":\"Please Activate your Account.\"}]";
        }
        else if (ret == "Deactivated")
        {
            return "[{\"error\":\"Your Account has been Deactivated\"}]";
        }
        else if (string.IsNullOrEmpty(ret))
        {
            return "[]";
        }
        else
        {
            return checkloginNonPaid(email, password);
        }
    }
    #endregion

    #region Drupal Data ---NEED TO ADD AN ADDITIONAL D TO EVERY METHOD TO HIDE IT FROM HACKERS
    public string DDrupal_User_Change_Password(string newpassword, string oldpassword, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("UPDATE t_members SET NonEncPassword=@newpassword where NonEncPassword=@oldpassword and userid=@userid");
            cmd.Parameters.Add("@newpassword", SqlDbType.VarChar).Value = newpassword;
            cmd.Parameters.Add("@oldpassword", SqlDbType.VarChar).Value = oldpassword;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
            bool save = general.performActionNoTransClient(cmd, "V_Members");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_User_GetLoggedInUserData(string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("select TOP 1 * from View_Members_API  where id=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd, "V_Members");
            return general.DStoJSON(ds);
        }
        else
        {
            return string.Empty;
        }
    }
    public string DDrupal_Order_GetShoppingCartItems(string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand(@"
                SELECT * FROM View_OrderList_API WHERE CartType = @cartType and 
                case 
			    when isnull(userid,'')='' and cartid=@cartId then 1 
			    when isnull(userid,'')<>'' and userid=@userid then 1 
			    end = 1");
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@cartType", SqlDbType.VarChar).Value = MainLibrary.Helper.PRODUCT;
            if (string.IsNullOrEmpty(userid))
            {
                cmd.Parameters.Add("@cartId", SqlDbType.VarChar).Value = MainLibrary.Helper.shoppingCartId;
                cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = "";
            }
            else if (!string.IsNullOrEmpty(userid))
            {
                cmd.Parameters.Add("@cartId", SqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            }

            DataSet ds = general.getSet(cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {

                    return general.DStoJSON(ds);
                }
                else
                {
                    return "[{\"error\":\"Please login and try again. No products in cart.\"}]";
                }
            }
            else
            {
                return "[{\"error\":\"Please login and try again. Shopping Cart Empty\"}]";
            }
        }
        else
        {
            return "[{\"error\":\"Please login and try again. User Not found.\"}]";
        }
    }
    public string DDrupal_Clear_Cart(string userid)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.shoppingCartId))
        {
            SqlCommand cmd = new SqlCommand(@"update t_orders set status='Cleared', RecordStatus='Deleted' where status <> 'Completed' and
                                case when userid='' and cartid=@unId then 1 when userid<>'' and userid=@unId then 1 end = 1");
            if (string.IsNullOrEmpty(userid))
                cmd.Parameters.Add("@unId", SqlDbType.VarChar).Value = MainLibrary.Helper.shoppingCartId;
            else if (!string.IsNullOrEmpty(userid))
                cmd.Parameters.Add("@unId", SqlDbType.VarChar).Value = userid;
            general.performActionNoTrans(cmd);
            MainLibrary.Helper.ExpireShoppingCart();
            return general.TextToJson("Cleared");
        }
        else
        {
            return general.TextToJson("Please login and try again.");
        }
    }
    public string DDrupal_Product_RemoveCart(string id, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
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
                if (string.IsNullOrEmpty(userid))
                    cmd.Parameters.Add("@unId", SqlDbType.VarChar).Value = MainLibrary.Helper.shoppingCartId;
                else if (!string.IsNullOrEmpty(userid))
                    cmd.Parameters.Add("@unId", SqlDbType.VarChar).Value = userid;
                cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
                bool saved = general.performActionNoTrans(cmd);
                return general.TextToJson(saved.ToString());
            }
            catch (Exception ex)
            {
                return general.TextToJson("False");
            }
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Product_AddToWishlist(string linkURL, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("insert into t_wishlist (userid,productid,linkurl,createdon,status) values (@userid,@productid,@linkurl,GETDATE(),'NEW')");
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
            cmd.Parameters.Add("@productid", SqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@linkurl", SqlDbType.VarChar).Value = linkURL;
            bool saved = general.performActionNoTrans(cmd);
            return general.TextToJson(saved.ToString());
        }
        return general.TextToJson("Please login and try again.");
    }
    public string DDrupal_Order_AddToShoppingCart(string linkURL, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("select TOP 1 id,price from t_Products where linkURL = @linkURL");
            cmd.Parameters.Add("@linkURL", SqlDbType.VarChar).Value = linkURL;
            DataSet ds = general.getSet(cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string productId = dr["id"].ToString();
                    string price = dr["price"].ToString();

                    if (!string.IsNullOrEmpty(productId))
                    {
                        string str = Drupal_AddToCart(userid, "buy", MainLibrary.Helper.PRODUCT, productId, MainLibrary.Helper.shoppingCartId, MainLibrary.Helper.AffiliateId, price).ToString();
                        return general.TextToJson(str);
                    }
                    else
                    {
                        return "[{\"error\":\"Invalid Product. Product not available.\"}]";
                    }
                }
            }
            else
            {
                return "[{\"error\":\"Product Not Found.\"}]";
            }
        }
        return "[{\"error\":\"Please login and try again.\"}]";
    }
    public string DDrupal_Product_AddLikes(string id, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
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
    public string DDrupal_Product_GetPurchasedProductsDownload(string userid)
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
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd);
            return returnCheck(ds);
        }
        return general.TextToJson("Please login and try again.");
    }
    public string DDrupal_Product_GetRedeemedProductsDownload(string userid)
    {
        string query = @"SELECT  distinct  p.ID,p.Title as title,p.ThumbnailURL as [image],CONVERT(VARCHAR(12), isnull(o.createdon,GETDATE()) , 107) as [date],'Claimed' Type, p.category, p.year, p.author,'' filesize, 'MP4' filetype, p.FileToDownload as link
                                    FROM t_Orders o inner join t_Products p on o.productID = p.ID
                                    where o.RecordStatus = 'Active' and o.Status = 'Completed' and p.ProductType in ('DVD','Apps') and o.userID = @userId
UNION
                                    SELECT  distinct  p.ID,p.Title,p.ThumbnailURL,CONVERT(VARCHAR(12), isnull(o.CreatedOn,GETDATE()) , 107) as [date],'Redeemed' Type, p.category, p.year, p.author,'' filesize, 'MP4' filetype, p.FileToDownload as link
                                    FROM t_Orders o inner join t_Products p on o.productID = p.ID
                                    where o.RecordStatus = 'Active' and o.Status = 'Redeemed' and p.ProductType in ('DVD','Apps') and o.userID = @userId
                                    order by Title";
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand(query);
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd);
            return returnCheck(ds);
        }
        return general.TextToJson("Please login and try again.");
    }
    public string DDrupal_UserFollowerApproved(string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("select * from view_UserFollowerApproved where userid=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd, "v2014_tube");
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }

    }
    public string DDrupal_UserFollowerRequest(string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("select distinct * from view_UserFollowerRequest where userid=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd, "v2014_tube");
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_UserFollowing(string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("select distinct * from view_UserFollowing where userid=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd, "v2014_tube");
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_UserVideoPlaylist(string userid)
    {
        SqlCommand cmd = new SqlCommand("select * from view_UserVideoPlaylist where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string DDrupal_UserMyVideos(string userid)
    {
        SqlCommand cmd = new SqlCommand("select * from view_UserMyVideos where userid=@id");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "v2014_tube");
        return returnCheck(ds);
    }
    public string DDrupal_UserChannel(string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("select * from view_UserChannel where userid=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd, "v2014_tube");
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_User_MyEvents(string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("SELECT em.id,e.LocalTitle,e.Title,e.eventstarton,EventEndOn FROM t_events_members em inner join view_events e on e.id = em.eventid where e.eventstarton > getdate()and em.userId = @userId and e.RecordStatus='Published");
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd);
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_User_point_history(string userid)
    {
        SqlCommand cmd = new SqlCommand("SELECT *, isnull(description,activity) activitydesc FROM t_PointHistory WHERE Userid = @userId order by CreatedOn desc");
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd);
        return returnCheck(ds);
    }
    public string DDrupal_User_GetFollowingChannels(string count, string page, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            int start = 1; //int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
            int end = Convert.ToInt32(count) * Convert.ToInt32(page);

            SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,channelUrlReal,privacy) channelUrl,* from View_FollowList_API where userid=@userid and is_approved=1) p
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
            cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
            cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
            cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
            DataSet ds = general.getSet(cmd, "V2014_Tube");
            return general.DStoJSON(ds);
        }
        else
        {
            return string.Empty;
        }
    }
    public string DDrupal_Video_AddToPlaylist(string id, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO t_playlist (id,userid,videoid,CreatedOn) VALUES (newid(),@userid,@videoid,getdate())");
            cmd.Parameters.Add("@videoid", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Video_RemovePlaylist(string id, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM t_playlist WHERE runningNum=(select top 1 runningNum from view_UserVideoPlaylist where userId=@userId and videoId=@videoId)");
            cmd.Parameters.Add("@videoid", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
            bool delete = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(delete.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Channel_UnfollowChannel(string idorname, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand(@"
                DELETE  FROM vs_subscribe_channel
                WHERE   channel_id in (SELECT id FROM vs_channel where cast(id as varchar(500))=@idorname or name=@idorname) 
                    and subscriber_user_id=@userId
                ");
            cmd.Parameters.Add("@idorname", SqlDbType.VarChar).Value = idorname;
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Channel_FollowChannel(string idorname, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand(@"
                declare @channelId bigint
                declare @autofollow bit

                SELECT top 1 @channelId=id,@autofollow=followAutoApprove FROM vs_channel where cast(id as varchar(500))=@idorname or name=@idorname

                INSERT INTO vs_subscribe_channel (channel_id,subscriber_user_id,is_approved,create_date)
                     VALUES (@channelId,@userId,@autofollow,getdate())
                select @autofollow
                ");
            cmd.Parameters.Add("@idorname", SqlDbType.VarChar).Value = idorname;
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            string save = general.getSingleData(cmd, "V2014_Tube");

            return general.TextToJson(save.ToString());

        }
        else
        {
            return general.TextToJson("LoginRequired");
        }
    }
    public string DDrupal_User_GetRedemptions(string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("select * from view_RedemptionList where userId=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd);
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_User_GetPurchase(string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("select * from view_PurchaseList where userId=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
            DataSet ds = general.getSet(cmd);
            return returnCheck(ds);
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Video_AddLikes(string id, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_entry_details SET Likes = isnull(Likes,0) + 1  WHERE Id = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Channel_Create(string name, string accesstype, string description, string iscomment, string israte, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("sp_Channel_Create");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@access_type", SqlDbType.VarChar).Value = accesstype;
            cmd.Parameters.Add("@user_id", SqlDbType.VarChar).Value = userid;
            //cmd.Parameters.Add("@image", SqlDbType.VarBinary).Value = null;//TO DO ARESH;
            cmd.Parameters.Add("@description", SqlDbType.VarChar).Value = description;
            cmd.Parameters.Add("@is_comment_allowed", SqlDbType.VarChar).Value = iscomment;
            cmd.Parameters.Add("@is_rate_allowed", SqlDbType.VarChar).Value = israte;
            string saved = general.getSingleData(cmd,"V2014_Tube");
            return general.TextToJson(saved.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_User_inbox(string userid)
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM [View_t_messages] where Userid = @userId order by CreatedOn desc");
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd,"V_Members");
        return returnCheck(ds);
    }
    public string DDrupal_User_inbox_single(string userid, string id)
    {
        SqlCommand cmd = new SqlCommand("SELECT * FROM [View_t_messages] where Userid = @userId and id=@id order by CreatedOn desc");
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
        DataSet ds = general.getSet(cmd, "V_Members");
        return returnCheck(ds);
    }
    public string DDrupal_Video_Create(string name, string desc, string tags, string category, string level, string targetmarket, string comment, string share, string publish, 
        string guid, string userid)
    {
        string reference = Guid.NewGuid().ToString();
        SqlCommand cmd = new SqlCommand("sp_Video_Create");
        cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
            cmd.Parameters.Add("@tags", SqlDbType.VarChar).Value = tags;
            cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = desc;
            cmd.Parameters.Add("@levelId", SqlDbType.VarChar).Value = level;
            cmd.Parameters.Add("@is_comments_allowed", SqlDbType.VarChar).Value = comment;
            cmd.Parameters.Add("@categories", SqlDbType.VarChar).Value = category;
            cmd.Parameters.Add("@is_share_allowed", SqlDbType.VarChar).Value = share;
            cmd.Parameters.Add("@fileName", SqlDbType.VarChar).Value = guid;
            cmd.Parameters.Add("@runningNum", SqlDbType.VarChar).Value = reference;
            cmd.Parameters.Add("@access_type", SqlDbType.VarChar).Value = publish;
            cmd.Parameters.Add("@market_location", SqlDbType.VarChar).Value = targetmarket;
            cmd.Parameters.Add("@createdBy", SqlDbType.VarChar).Value = userid;
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            cmd.Parameters.Add("@isApproved", SqlDbType.VarChar).Value = false;
            cmd.Parameters.Add("@allow_ads", SqlDbType.VarChar).Value = true;

            bool saved = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(saved.ToString());
    }
    public string DDrupal_channel_remove(string id, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_channel SET delete_date=GETDATE(), access_type='DELETED'  WHERE Id = @id  and user_id=@userid");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Comment_AddComment(string title, string comment, string ctype, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"INSERT INTO t_Comments 
                        (ReferenceId,Comment,CreatedBy,CreatedOn,CommentType) 
                VALUES  (@Refid,@Comment, @CreatedBy, GETDATE(),@ctype)";

            cmd.Parameters.Add("@Refid", SqlDbType.VarChar).Value = title;
            cmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = comment;
            cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = userid;
            cmd.Parameters.Add("@ctype", SqlDbType.VarChar).Value = ctype;
            bool saved = general.performAction(cmd);
            return general.TextToJson(saved.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Video_GetFirstVideoPerCategory(string count, string itemPerCategory, string userid)
    {
        SqlCommand cmd = new SqlCommand(@"
                    WITH summary AS (
	                    SELECT dbo.GetCastedUrlDrupal(@membership,videoUrlReal,videoPrivacy) videoUrl, *,ROW_NUMBER() OVER(PARTITION BY v.category ORDER BY v.createdon DESC) AS rk
	                    FROM View_VideoList_API v 
	                    WHERE category<>'' and user_id = 'AF3102AD-09C6-4E44-9872-2C3695F5A1F6' and isapproved=1)
                    SELECT * FROM summary s  where s.rk <= @itemPerCategory");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = userid;
        cmd.Parameters.Add("@itemPerCategory", SqlDbType.VarChar).Value = itemPerCategory.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string DDrupal_Video_GetAll_premium(string count, string page, string userid)
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrlDrupal(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_premium where isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = userid;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string DDrupal_Video_GetAll(string count, string page, string userid)
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrlDrupal(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = userid;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string DDrupal_Video_GetByLevel(string count, string level, string page, string userid)
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY createdon desc) rowNumber,
                    dbo.GetCastedUrlDrupal(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1 and [level] like '%'+@level+'%' and createdBy='VTube-Admin') v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = userid;
        cmd.Parameters.Add("@level", SqlDbType.VarChar).Value = level;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string DDrupal_Video_GetByChannel(string count, string IdorChannelName, string page, string userid)
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrlDrupal(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1 and (cast(ChannelId as varchar(10))=@idorname or ChannelName=@idorname)) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = userid;
        cmd.Parameters.Add("@idorname", SqlDbType.VarChar).Value = IdorChannelName;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string DDrupal_Video_GetMostViewed(string count, string page, string userid)
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY Views desc) rowNumber,
                    dbo.GetCastedUrlDrupal(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = userid;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string DDrupal_Video_GetModerator(string count, string page, string userid)
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrlDrupal(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where user_id = 'AF3102AD-09C6-4E44-9872-2C3695F5A1F6' and isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = userid;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string DDrupal_Reaction_Give(string Primaryid, string reaction, string CUSTOM_Type, string userid)
    {
        SqlCommand cmd = new SqlCommand("udp_Reaction");
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@type", SqlDbType.VarChar).Value = CUSTOM_Type;
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = Primaryid;
        cmd.Parameters.Add("@Reaction", SqlDbType.VarChar).Value = reaction;
        bool saved = general.performAction(cmd);
        return general.TextToJson(saved.ToString());
    }
    public string DDrupal_myUser_update(string name, string bday, string gender, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("UPDATE t_members set first_name=@name, birthday=@bday, Gender=@gender  where id=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
            cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
            try
            {
                cmd.Parameters.Add("@bday", SqlDbType.VarChar).Value = Convert.ToDateTime(bday).ToString("s");
            }
            catch (Exception)
            {
                cmd.Parameters.Add("@bday", SqlDbType.VarChar).Value = DateTime.Now.ToString("s");
            }
            cmd.Parameters.Add("@gender", SqlDbType.VarChar).Value = gender;
            bool save = general.performActionNoTransClient(cmd, "V_Members");
            return general.TextToJson(save.ToString());
         }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Video_Update(string vidid, string name, string descrip, string tags, string category, string level, string targetmarket, string comment, string share, string publish, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand(@"UPDATE vs_entry_details
                                                   SET Name = @Name, 
                                                      Levels = @Level,                                                 
                                                      is_comments_allowed = @is_comments_allowed,     
                                                      is_share_allowed = @is_share_allowed, 
                                                      is_auto_approve = 0,                                                   
                                                      update_date = getdate(),      
                                                      Categories = @Categories, 
                                                      Description = @Description, 
                                                      Tags = @Tags,
                                                      access_type = @access_type,
                                                      Market_Location = @Market_Location
                                                 WHERE Id = @id and [UserId]=@userid");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = vidid;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name.Trim();
            cmd.Parameters.Add("@Level", SqlDbType.VarChar).Value = level;
            cmd.Parameters.Add("@is_comments_allowed", SqlDbType.VarChar).Value = comment;
            cmd.Parameters.Add("@is_share_allowed", SqlDbType.VarChar).Value = share;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = descrip.Trim();
            cmd.Parameters.Add("@Tags", SqlDbType.VarChar).Value = tags;
            cmd.Parameters.Add("@Categories", SqlDbType.VarChar).Value = category;
            cmd.Parameters.Add("@access_type", SqlDbType.VarChar).Value = publish;
            cmd.Parameters.Add("@Market_Location", SqlDbType.VarChar).Value = targetmarket;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;

            bool save =  general.performActionClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_myUser_billShip(string billAddress, string billCity, string billState, string billPost, string billPhone, string billEmail, string billCountry,
        string shipAddress, string shipCity, string shipState, string shipPost, string shipPhone, string shipEmail, string shipCountry, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand(@"UPDATE [t_Members]
                                                       SET [billing_address1] = @billing_address1, 
                                                          [billing_city] = @billing_city, 
                                                          [billing_state_region] = @billing_state_region, 
                                                          [billing_country] = @billing_country, 
                                                          [billing_postal_code] = @billing_postal_code, 
                                                          [billing_email] = @billing_email, 
                                                          [billing_phone_number] = @billing_phone_number, 
                                                          [shipping_address1] = @shipping_address1,
                                                          [shipping_city] = @shipping_city, 
                                                          [shipping_state_region] = @shipping_state_region, 
                                                          [shipping_country] = @shipping_country, 
                                                          [shipping_postal_code] = @shipping_postal_code, 
                                                          [shipping_email] = @shipping_email,
                                                          [shipping_phone_number] = @shipping_phone_number
                                                     WHERE id = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
            cmd.Parameters.Add("@billing_address1", SqlDbType.VarChar).Value = billAddress;
            cmd.Parameters.Add("@billing_city", SqlDbType.VarChar).Value = billCity;
            cmd.Parameters.Add("@billing_state_region", SqlDbType.VarChar).Value = billState;
            cmd.Parameters.Add("@billing_country", SqlDbType.VarChar).Value = billCountry;
            cmd.Parameters.Add("@billing_postal_code", SqlDbType.VarChar).Value = billPost;
            cmd.Parameters.Add("@billing_email", SqlDbType.VarChar).Value = billEmail;
            cmd.Parameters.Add("@billing_phone_number", SqlDbType.VarChar).Value = billPhone;
            cmd.Parameters.Add("@shipping_address1", SqlDbType.VarChar).Value = shipAddress;
            cmd.Parameters.Add("@shipping_city", SqlDbType.VarChar).Value = shipCity;
            cmd.Parameters.Add("@shipping_state_region", SqlDbType.VarChar).Value = shipState;
            cmd.Parameters.Add("@shipping_country", SqlDbType.VarChar).Value = shipCountry;
            cmd.Parameters.Add("@shipping_postal_code", SqlDbType.VarChar).Value = shipPost;
            cmd.Parameters.Add("@shipping_email", SqlDbType.VarChar).Value = shipEmail;
            cmd.Parameters.Add("@shipping_phone_number", SqlDbType.VarChar).Value = shipPhone;

            bool save =  general.performActionNoTransClient(cmd, "V_Members"); 
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Channel_Update(string channelid, string title, string descript, string Comment, string publish, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand(@"UPDATE vs_channel SET access_type = @access_type,name = @name,description = @description,is_comment_allowed = @is_comment_allowed,update_date = getdate() WHERE id = @id and user_id=@uid");
            cmd.Parameters.Add("@access_type", SqlDbType.VarChar).Value = publish;
            cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = title.Trim();
            cmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = descript.Trim();
            cmd.Parameters.Add("@is_comment_allowed", SqlDbType.VarChar).Value = Comment;
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = channelid;
            cmd.Parameters.Add("@uid", SqlDbType.VarChar).Value = userid;
                
            bool Save = general.performActionClient(cmd, "V2014_Tube");
            return general.TextToJson(Save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_Checkout(string runnum, string cartid, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            string strArrays = runnum + "-" + DateTime.Now.ToString("mmss");
            string str = DateTime.Now.ToString("MMddyy").Replace("/", "");
            string invoiceNo = strArrays + "-" + str;
            SqlCommand cmd = new SqlCommand(@"UPDATE t_Orders SET Status = @status, TransactionID = isnull(TransactionID,@transactionID), 
    OrderDate = getdate(), UpdatedOn = getdate() ,UpdatedBy = @updatedBy WHERE 
    CartType = @cartType and RecordStatus = 'Active' and Status not in ('Completed','Redeemed')  and  userID = @userId");
            cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "Pending";
            cmd.Parameters.Add("@transactionID", SqlDbType.VarChar).Value = invoiceNo;
            cmd.Parameters.Add("@updatedBy", SqlDbType.VarChar).Value = userid;
            cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = userid;
            cmd.Parameters.Add("@cartType", SqlDbType.VarChar).Value = "PRODUCT";

            bool Save =  general.performActionNoTransClient(cmd,"V2014_EX");
            if (Save)
            {
                string gatewayUrl = ConfigurationManager.AppSettings["PaymentGateway"];
            
                return gatewayUrl + "?sid=" + cartid + "&uid=" + userid + "&inv=" + invoiceNo;
            }
            else
            {
                return "";
            }
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_myvideo_del(string id, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_entry_details SET isApproved=0  WHERE Id = @id and createdBy=@userid");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = userid;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    public string DDrupal_UploadFile(string type, string filename, string userid)
    {
        if (!string.IsNullOrEmpty(userid))
        {
            switch (type)
            {
                case "profile":
                    try
                    {
                        using (System.IO.FileStream str = File.OpenRead(filename))
                        {
                            byte[] buf = new byte[str.Length];
                            str.Read(buf, 0, buf.Length);

                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandText = "UPDATE t_Members SET update_date = GETDATE(), last_modified = GETDATE(),avatarURL = @avatarURL  WHERE id = @id";
                            cmd.Parameters.Add("@avatarURL", SqlDbType.VarBinary).Value = buf;
                            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
                            bool Save = general.performActionNoTransClient(cmd, "V_Members");

                            FileInfo myfileinf = new FileInfo(filename);
                            myfileinf.Delete();
                            if (Save)
                            {
                                return general.TextToJson("Saved");
                            }
                            else
                            {
                                return general.TextToJson("Failed");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //File.AppendAllText("d:/logAresh.txt", Environment.NewLine + ex + " Error : " + DateTime.Now.ToString()); 
                    }
                    break;
                default:
                    try
                    {
                        //look for an underscore
                        string[] words = type.Split('_');
                        if ((words[0] == "channel") && (words[1] != "undefined"))
                        {
                            using (System.IO.FileStream str = File.OpenRead(filename))
                            {
                                byte[] buf = new byte[str.Length];
                                str.Read(buf, 0, buf.Length);

                                SqlCommand cmd = new SqlCommand();
                                cmd.CommandText = "UPDATE vs_channel SET update_date = GETDATE(),image = @avatarURL  WHERE user_id = @id and id=@pk";
                                cmd.Parameters.Add("@avatarURL", SqlDbType.VarBinary).Value = buf;
                                cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = userid;
                                cmd.Parameters.Add("@pk", SqlDbType.VarChar).Value = words[1].ToString();
                                bool Save = general.performActionNoTransClient(cmd, "V2014_tube");
                                FileInfo myfileinf = new FileInfo(filename);
                                myfileinf.Delete();
                                if (Save)
                                {
                                    return general.TextToJson("Saved");
                                }
                                else
                                {
                                    return general.TextToJson("Failed");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //File.AppendAllText("d:/logAresh.txt", Environment.NewLine + ex + " Error : " + DateTime.Now.ToString()); 
                    }
                    break;
            }
        }
        return general.TextToJson("Login Required");
    }

    public string DDrupal_givePoint(string type, string work, string userID, string referece, string description)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("udp_Points");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("@type", SqlDbType.VarChar).Value = type;
            cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = userID;
            cmd.Parameters.Add("@referece", SqlDbType.VarChar).Value = referece;
            cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = work;
            cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "success";
            cmd.Parameters.Add("@newPoint", SqlDbType.VarChar).Value = "0";
            cmd.Parameters.Add("@willmultiply", SqlDbType.VarChar).Value = "1";
            cmd.Parameters.Add("@description", SqlDbType.VarChar).Value = description;
            general.performActionNoTrans(cmd);
            return "Point Given";
        }
        catch (Exception ex)
        {
            return "Error Encountered";
            new eException("GivePoint_VSite_Extension: " + ex.ToString());
        }
    }
    #endregion


    #region NON FIT METHODS

    public static bool Drupal_AddToCart(string userid, string action, string cartType, string productId, string cartId, string affiliateid, string price, string promoId = "")
    {
        try
        {
            decimal finalprice = 0;
            SqlCommand cmd = new SqlCommand(@"SP_Orders_AddToCart");
            cmd.CommandType = CommandType.StoredProcedure;

            if (action == "buy")
            {
                finalprice = Convert.ToDecimal(price);
                //do computation with promos here
            }
            else if (action == "activate" && !string.IsNullOrEmpty(promoId))
            {
                finalprice = GetPromoPrice(promoId, price);
                cmd.Parameters.Add("@promoId", SqlDbType.VarChar).Value = promoId;
            }
            else if (action == "invitation" && !string.IsNullOrEmpty(promoId))
            {
                finalprice = Convert.ToDecimal(price);
                cmd.Parameters.Add("@promoId", SqlDbType.VarChar).Value = promoId;
            }
            else if (action == "discounted")
            {
                // added 3/18 7:46 PM
                finalprice = GetDiscountPrice(productId, price);
            }
            else
            {
                finalprice = Convert.ToDecimal(price);
            }

            cmd.Parameters.Add("@productID", SqlDbType.VarChar).Value = productId;
            cmd.Parameters.Add("@AffiliateId", SqlDbType.VarChar).Value = affiliateid;
            cmd.Parameters.Add("@CartId", SqlDbType.VarChar).Value = cartId;
            cmd.Parameters.Add("@CartType", SqlDbType.VarChar).Value = cartType;
            cmd.Parameters.Add("@Price", SqlDbType.VarChar).Value = price;
            cmd.Parameters.Add("@FinalPrice", SqlDbType.VarChar).Value = finalprice.ToString();
            cmd.Parameters.Add("@BrowserSession", SqlDbType.VarChar).Value = HttpContext.Current.Session.SessionID.ToString();

            if (!string.IsNullOrEmpty(userid))
            {
                cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = userid;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = userid + " " + commando.GetIPAddress();
            }

            return general.performAction(cmd);
        }
        catch (Exception ex)
        {
            new eException("VShoppeHelper.AddToCart: " + ex.Message.ToString());
            return false;
        }
    }
    private static decimal GetPromoPrice(string promoId,string originalPrice)
        {
            decimal newAmount = Convert.ToDecimal(originalPrice);
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT isnull(NewAmount,0) NewAmount FROM t_Product_Promo where RecordStatus = 'Active' and Id = @id");
                cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = promoId;

                string result = general.getSingleData(cmd);

                newAmount = !string.IsNullOrEmpty(result) ? Convert.ToDecimal(result) : Convert.ToDecimal(originalPrice);
            }
            catch (Exception ex)
            {
                new eException("VShoppeHelper.GetPromoPrice: " + ex.Message.ToString());
            }

            return newAmount;
        }
    private static decimal GetDiscountPrice(string productId, string originalPrice)
        {
            decimal newAmount = Convert.ToDecimal(originalPrice);
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT isnull(promoPrice,0) promoPrice FROM t_product_discount where productid = @productid");
                cmd.Parameters.Add("@productid", SqlDbType.VarChar).Value = productId;

                string result = general.getSingleData(cmd);

                newAmount = !string.IsNullOrEmpty(result) ? Convert.ToDecimal(result) : Convert.ToDecimal(originalPrice);
            }
            catch (Exception ex)
            {
                new eException("VShoppeHelper.GetDiscountPrice: " + ex.Message.ToString());
            }

            return newAmount;
        }
    #endregion

    #region Video
    public string ChangeVideo_DatePublic(string videoid)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_entry_details SET access_type='Public', Create_date=DATEADD(Year,-1,GETDATE())  WHERE Id = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = videoid;
            general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson("Successfully Changed");
        }
        catch (Exception ex)
        {
            return general.TextToJson("Error - Cannot Execute");
        }
    }
    public string Video_AddPlays(string Id)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_entry_details SET Views = isnull(Views,0) + 1  WHERE Id = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = Id;
            general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson("Successfully Changed");
        }
        catch (Exception ex)
        {
            return general.TextToJson("Error - Cannot Execute");
        }
    }

    public void Video_AddViews(string Id)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_entry_details SET Plays = isnull(Plays,0) + 1  WHERE Id = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = Id;
            general.performActionNoTransClient(cmd, "V2014_Tube");
        }
        catch (Exception ex)
        {
        }
    }

    public void WatchVideo(String userid, string BCId)
    {
        try
        {
            if (string.IsNullOrEmpty(userid)) return;

            if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(BCId))
            {
                SqlCommand cmd = new SqlCommand("select COUNT(*) from t_PointHistory where activity='watched a video' and userid=@userid and  Cast(CreatedOn as date) = CAST(GETDATE() as date)");
                cmd.Parameters.Add("@userid", System.Data.SqlDbType.VarChar).Value = userid;
                string id = general.getSingleData(cmd);
                if (Convert.ToInt32(id) < 20)
                {
                    cmd = new SqlCommand("SELECT [name] FROM [vs_entry_details] where id=@id");
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.VarChar).Value = BCId;
                    string title = general.getSingleData(cmd, "V2014_Tube");

                    givePoint("WatchVideo", "Watched a video", userid, BCId, string.Format("Watched {0} video", title));
                }
            }
        }
        catch (Exception)
        { }
    }
    private void givePoint(string type, string action, string userID, string referece, string description)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("udp_Points");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("@type", SqlDbType.VarChar).Value = type;
            cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = userID;
            cmd.Parameters.Add("@referece", SqlDbType.VarChar).Value = referece;
            cmd.Parameters.Add("@action", SqlDbType.VarChar).Value = action;
            cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "success";
            cmd.Parameters.Add("@newPoint", SqlDbType.VarChar).Value = "0";
            cmd.Parameters.Add("@willmultiply", SqlDbType.VarChar).Value = "1";
            cmd.Parameters.Add("@description", SqlDbType.VarChar).Value = description;
            general.performActionNoTrans(cmd);
        }
        catch (Exception ex)
        {
        }
    }
    public string Video_LatestByDate(String mydate)
    {

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl('Free',videoUrlReal,videoPrivacy) videoUrl, 
                    * FROM View_VideoList_API where isapproved=1
                    and cast(createdOn as DATE) = CAST(@mydate as date)) v
                    WHERE rowNumber between 1 and 15
                    order by rowNumber");
        cmd.Parameters.Add("@mydate", SqlDbType.VarChar).Value = mydate;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string Video_GetAll(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string Video_GetAll_premium(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_premium where isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetModerator(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where user_id = 'AF3102AD-09C6-4E44-9872-2C3695F5A1F6' and isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetRecommended(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1 and is_recommended=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetMostViewed(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY Views desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetMostLikes(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY Likes desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string Video_GetSearch(string word, string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl,
                    * FROM View_VideoList_API where isapproved=1 and name like '%'+@word+'%') v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@word", SqlDbType.VarChar).Value = word.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    public string Video_GetLatest(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetHighlighted(string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1 and isHighlighted=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetByChannel(string count, string IdorChannelName, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1 and (cast(ChannelId as varchar(10))=@idorname or ChannelName=@idorname)) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@idorname", SqlDbType.VarChar).Value = IdorChannelName;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetRelated(string count, string id, string page = "1")
    {
        SqlCommand cmd = new SqlCommand(@"select TOP 1 Tags from View_VideoList_API where isapproved=1 and Id=@id order by Createdon desc");
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
        string result = general.getSingleData(cmd, "V2014_Tube");

        if (!string.IsNullOrEmpty(result))
        {
            string[] tags = result.Split(',');
            string query = string.Empty;

            foreach (string tag in tags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    query += string.Format(" select TOP {0} videoUrlReal as videoUrl, * from View_VideoList_API where tags like '%{1}%' UNION", count, tag.Trim());
                }
            }

            query = query.Remove(query.Length - 5, 5);

            cmd = new SqlCommand(query);
            DataSet ds = general.getSet(cmd, "V2014_Tube");
            return general.DStoJSON(ds);
        }
        else
        {
            return string.Empty;
        }
    }

    public string Video_GetByCategories(string count, string category, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY createdon desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1 and Category like '%'+@category+'%') v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@category", SqlDbType.VarChar).Value = category;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetByLevel(string count, string level, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);

        SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY createdon desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1 and [level] like '%'+@level+'%' and createdBy='VTube-Admin') v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@level", SqlDbType.VarChar).Value = level;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetMoreVideos(string count, string id, string page = "1")
    {
        MainLibrary.Video vid = new MainLibrary.Video(id);

        if (!string.IsNullOrEmpty(vid.Channel_id))
        {
            return Video_GetByChannel(count, vid.Channel_id, page);
        }
        else
        {
            return string.Empty;
        }
    }

    public string Video_GetCategories()
    {
        SqlCommand cmd = new SqlCommand("select category, COUNT(Id) as count from View_VideoList_API where isapproved = 1 and category<>'' group by category order by count desc");
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetFirstVideoPerCategory(string count, string itemPerCategory = "1")
    {
        SqlCommand cmd = new SqlCommand(@"
                    WITH summary AS (
	                    SELECT dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, *,ROW_NUMBER() OVER(PARTITION BY v.category ORDER BY v.createdon DESC) AS rk
	                    FROM View_VideoList_API v 
	                    WHERE category<>'' and user_id = 'AF3102AD-09C6-4E44-9872-2C3695F5A1F6' and isapproved=1)
                    SELECT * FROM summary s  where s.rk <= @itemPerCategory");
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@itemPerCategory", SqlDbType.VarChar).Value = itemPerCategory.ToString();
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_GetDetails(string idorname)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_entry_details SET Plays = isnull(Plays,0) + 1  WHERE Id = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = idorname;
            general.performActionNoTransClient(cmd, "V2014_Tube");
        }
        catch (Exception ex)
        {
            new eException("Player play point: " + ex.Message.ToString());
        }

        try
        {
            SqlCommand cmd = new SqlCommand("select TOP 1 * from View_VideoList_API where (id=@idorname or name=@idorname)");
            cmd.Parameters.Add("@idorname", SqlDbType.VarChar).Value = idorname.Replace("#", "");
            DataSet ds = general.getSet(cmd, "V2014_Tube");
            return general.DStoJSON(ds);
        }
        catch (Exception)
        {
            
            throw;
        }
    }

    public string Video_GetDetailsWed()
    {
        SqlCommand cmd = new SqlCommand("select TOP 1 * from View_VideoList_API_wed");
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }

    public string Video_AddToPlaylist(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO t_playlist (id,userid,videoid,CreatedOn) VALUES (newid(),@userid,@videoid,getdate())");
            cmd.Parameters.Add("@videoid", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }

    public string Video_RemovePlaylist(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM t_playlist WHERE runningNum=(select top 1 runningNum from view_UserVideoPlaylist where userId=@userId and videoId=@videoId)");
            cmd.Parameters.Add("@videoid", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            bool delete = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(delete.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }

    public string Video_AddLikes(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_entry_details SET Likes = isnull(Likes,0) + 1  WHERE Id = @id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }

    public string myvideo_del(string id)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("UPDATE vs_entry_details SET isApproved=0  WHERE Id = @id and createdBy=@userid");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }

    public string myvideo_update(string id, string title, string description, string tags, string category, string level, string settings, string market, string allowcomments, string allowshare)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("sp_UpdateVideo");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@userid", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            cmd.Parameters.Add("@title", SqlDbType.VarChar).Value = title;
            cmd.Parameters.Add("@description", SqlDbType.VarChar).Value = description;
            cmd.Parameters.Add("@category", SqlDbType.VarChar).Value = category;
            cmd.Parameters.Add("@level", SqlDbType.VarChar).Value = level;
            cmd.Parameters.Add("@settings", SqlDbType.VarChar).Value = settings;
            cmd.Parameters.Add("@market", SqlDbType.VarChar).Value = market;
            cmd.Parameters.Add("@allowcomments", SqlDbType.VarChar).Value = allowcomments;
            cmd.Parameters.Add("@allowshare", SqlDbType.VarChar).Value = allowshare;
            cmd.Parameters.Add("@tags", SqlDbType.VarChar).Value = tags;
            bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
            return general.TextToJson(save.ToString());
        }
        else
        {
            return general.TextToJson("Login Required");
        }
    }
    #endregion

    #region New Vtube Api
    public string App_GetAllVids(string lang, string count, string page = "1")
    {
        int start = Convert.ToInt32(count) * (Convert.ToInt32(page) - 1) + 1;
        int end = Convert.ToInt32(count) * Convert.ToInt32(page);
        SqlCommand cmd;
        if (lang != "en")
        {
            cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1) v
                    WHERE language LIKE '%' + @lang + '%' AND rowNumber between @start and @end
                    order by rowNumber");
            cmd.Parameters.Add("@lang", SqlDbType.VarChar).Value = lang.ToString();
        }
        else
        {
            cmd = new SqlCommand(@"
                    SELECT * FROM 
                    (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) rowNumber,
                    dbo.GetCastedUrl(@membership,videoUrlReal,videoPrivacy) videoUrl, 
                    dbo.GetPlaylistVideoUser(@userId,id) playlist,
                    * FROM View_VideoList_API where isapproved=1) v
                    WHERE rowNumber between @start and @end
                    order by rowNumber");
        }
        cmd.Parameters.Add("@membership", SqlDbType.VarChar).Value = MainLibrary.Helper.UserMembership;
        cmd.Parameters.Add("@start", SqlDbType.VarChar).Value = start.ToString();
        cmd.Parameters.Add("@end", SqlDbType.VarChar).Value = end.ToString();
        cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
        DataSet ds = general.getSet(cmd, "V2014_Tube");
        return general.DStoJSON(ds);
    }
    #endregion

    #region Order
    public string Order_verify_activation(string activation, string code, string prodID)
    {
        SqlCommand cmd = new SqlCommand("select id from t_Product_Promo where status='Unused' and ActivationCode=@activation and PinCode=@code");
        cmd.Parameters.Add("@activation", SqlDbType.VarChar).Value = activation;
        cmd.Parameters.Add("@code", SqlDbType.VarChar).Value = code;
        string acID = general.getSingleData(cmd);
        if (!string.IsNullOrEmpty(acID))
        {
            string str = MainLibrary.Helper.AddToCart("buy", MainLibrary.Helper.PRODUCT, prodID, MainLibrary.Helper.shoppingCartId, MainLibrary.Helper.AffiliateId, "0.00", acID).ToString();
            return general.TextToJson(str);
        }
        else
        {
            return general.TextToJson("Code does not exist");
        }
    }   
    public string Order_AddToShoppingCart(string linkURL)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("select TOP 1 id,price from t_Products where linkURL = @linkURL");
            cmd.Parameters.Add("@linkURL", SqlDbType.VarChar).Value = linkURL;
            DataSet ds = general.getSet(cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string productId = dr["id"].ToString();
                    string price = dr["price"].ToString();

                    if (!string.IsNullOrEmpty(productId))
                    {
                        string str = MainLibrary.Helper.AddToCart("buy", MainLibrary.Helper.PRODUCT, productId, MainLibrary.Helper.shoppingCartId, MainLibrary.Helper.AffiliateId, price).ToString();
                        return general.TextToJson(str);
                    }
                    else
                    {
                        return "[{\"error\":\"Invalid Product. Product not available.\"}]";
                    }
                }
            }
            else
            {
                return "[{\"error\":\"Product Not Found.\"}]";
            }
        }
        return "[{\"error\":\"Please login and try again.\"}]";
    }
    public string Order_AddToRedeemCart(string linkURL)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand("select TOP 1 id,price,title as name,RedeemPoints as needpoint from t_Products where linkURL = @linkURL");
            cmd.Parameters.Add("@linkURL", SqlDbType.VarChar).Value = linkURL;
            DataSet ds = general.getSet(cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string productId = dr["id"].ToString();
                    string price = dr["price"].ToString();
                    string name = dr["name"].ToString();
                    string point = dr["needpoint"].ToString();
                    

                    if (!string.IsNullOrEmpty(productId))
                    {
                        string str = "True";//DO NOT ADD TO CART// MainLibrary.Helper.AddToCart("buy", MainLibrary.Helper.REDEEM, productId, MainLibrary.Helper.shoppingCartId, MainLibrary.Helper.AffiliateId, price).ToString();
                        if (str == "True")
                        {
                            return "[{\"Data\":\"True\",\"name\":\"" + name + "\", \"needpoint\":\"" + point + "\"}]";
                        }
                        else
                        {
                            return general.TextToJson(str);
                        }
                    }
                    else
                    {
                        return "[{\"error\":\"Invalid Product. Product not available.\"}]";
                    }
                }
            }
            else
            {
                return "[{\"error\":\"Product Not Found.\"}]";
            }
        }
        return "[{\"error\":\"Please login and try again.\"}]";
    }
    public string Order_GetShoppingCartItems()
    {
        return Order_GetShoppingCartItemsFromType(MainLibrary.Helper.PRODUCT);
    }
    public string Order_UpdateQty(string id, string qty)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("UPDATE t_orders set Quantity=@qty where id=@id");
            cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
            cmd.Parameters.Add("@qty", SqlDbType.VarChar).Value = Convert.ToInt32(qty);
            bool save = general.performActionNoTrans(cmd);
            if (save)
            {
                return general.TextToJson(save.ToString());
            }
            else
            {
                return "[{\"error\":\"Invalid Product ID.\"}]";
            }
        }
        catch (Exception ex)
        {
            
             return "[{\"error\":\"Invalid Product Quantity.\"}]";
        }
    }
    public string Order_GetRedeemCartItems()
    {
        return Order_GetShoppingCartItemsFromType(MainLibrary.Helper.REDEEM);
    }

    private string Order_GetShoppingCartItemsFromType(string cartType)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            SqlCommand cmd = new SqlCommand(@"
                SELECT * FROM View_OrderList_API WHERE CartType = @cartType and 
                case 
			    when isnull(userid,'')='' and cartid=@cartId then 1 
			    when isnull(userid,'')<>'' and userid=@userid then 1 
			    end = 1");
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("@cartType", SqlDbType.VarChar).Value = cartType;
            if (string.IsNullOrEmpty(MainLibrary.Helper.UserId))
            {
                cmd.Parameters.Add("@cartId", SqlDbType.VarChar).Value = MainLibrary.Helper.shoppingCartId;
                cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = "";
            }
            else if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
            {
                cmd.Parameters.Add("@cartId", SqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
            }

            DataSet ds = general.getSet(cmd);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {

                    return general.DStoJSON(ds);
                }
                else
                {
                    return "[{\"error\":\"Please login and try again. No products in cart.\"}]";
                }
            }
            else
            {
                return "[{\"error\":\"Please login and try again. Shopping Cart Empty\"}]";
            }
        }
        else
        {
            return "[{\"error\":\"Please login and try again. User Not found.\"}]";
        }
    }
    #endregion

    #region ELSE
    public string GetCommentByType(string count, string type, string reference, string page = "1")
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

    public string SubmitReport(string text, string url)
    {
        string reporter = "Anonymous";
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            reporter = MainLibrary.Helper.UserId;
        }
        SqlCommand cmd = new SqlCommand(@"
                INSERT INTO t_ReportedVideos (Id,ReportText,Reporter,Url,CreatedOn)
                VALUES (newid(),@ReportText,@Reporter,@Url,getdate())");
        cmd.Parameters.Add("@Reporter", SqlDbType.VarChar).Value = reporter;
        cmd.Parameters.Add("@ReportText", SqlDbType.VarChar).Value = text;
        cmd.Parameters.Add("@Url", SqlDbType.VarChar).Value = url;
        bool save = general.performActionNoTransClient(cmd, "V2014_Tube");
        return general.TextToJson(save.ToString());
    }

    public string LogOut()
    {
        MainLibrary.Helper.LogOut("/vtube-new");
        return string.Empty;
    }

    public string Comment_GetComment(string title)
    {
        SqlCommand cmd = new SqlCommand(@"select (select first_name from DBF_V_Members.dbo.t_Members where id=cl.CreatedBy) as CreatedBy,Comment,CreatedOn,CreatedBy as UserId,Id from View_CommentList cl where referenceid=@title and parentid is null and IsVisible=1  order by runningNum desc");
        cmd.Parameters.Add("@title", SqlDbType.VarChar).Value = title;
        DataSet ds = general.getSet(cmd);
        return general.DStoJSON(ds);
    }
    public string Comment_AddComment(string title, string user, string comment, string ctype)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = @"INSERT INTO t_Comments 
                    (ReferenceId,Comment,CreatedBy,CreatedOn,CommentType) 
            VALUES  (@Refid,@Comment, @CreatedBy, GETDATE(),@ctype)";

        cmd.Parameters.Add("@Refid", SqlDbType.VarChar).Value = title; //to be changed
        cmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = comment;
        cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = user;
        cmd.Parameters.Add("@ctype", SqlDbType.VarChar).Value = ctype;
        bool saved =  general.performAction(cmd);
        return general.TextToJson(saved.ToString());
    }


    public string Reaction_AddData(string CUSTOM_Type, string Primaryid, string reaction)
    {
        SqlCommand cmd = new SqlCommand("udp_Reaction");
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add("@type", SqlDbType.VarChar).Value = CUSTOM_Type;
        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = Primaryid;
        cmd.Parameters.Add("@Reaction", SqlDbType.VarChar).Value = reaction;
        bool saved = general.performAction(cmd);
        return general.TextToJson(saved.ToString());
    }
    #endregion

    #region Upload
    public string UploadFile(string type, string filename)
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            switch (type)
            {
                case "profile":
                    try
                    {
                        System.IO.FileStream str = File.OpenRead(filename);
                        byte[] buf = new byte[str.Length];
                        str.Read(buf, 0, buf.Length);

                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = "UPDATE t_Members SET update_date = GETDATE(), last_modified = GETDATE(),avatarURL = @avatarURL  WHERE id = @id";
                        cmd.Parameters.Add("@avatarURL", SqlDbType.VarBinary).Value = buf;
                        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
                        bool Save = general.performActionNoTransClient(cmd, "V_Members");

                        if (Save)
                        {
                            FileInfo myfileinf = new FileInfo(filename);
                            myfileinf.Delete();
                            return general.TextToJson("Saved");
                        }
                        else
                        {
                            return general.TextToJson("Failed");
                        }
                    }
                    catch (Exception ex)
                    {
                        //File.AppendAllText("d:/logAresh.txt", Environment.NewLine + ex + " Error : " + DateTime.Now.ToString()); 
                    }
                    break;
                default:
                        try
                        {
                            //look for an underscore
                            string[] words = type.Split('_');
                            if ((words[0] ==  "channel") && (words[1] != "undefined"))
                            {
                                System.IO.FileStream str = File.OpenRead(filename);
                                byte[] buf = new byte[str.Length];
                                str.Read(buf, 0, buf.Length);

                                SqlCommand cmd = new SqlCommand();
                                cmd.CommandText = "UPDATE vs_channel SET update_date = GETDATE(),image = @avatarURL  WHERE user_id = @id and id=@pk";
                                cmd.Parameters.Add("@avatarURL", SqlDbType.VarBinary).Value = buf;
                                cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
                                cmd.Parameters.Add("@pk", SqlDbType.VarChar).Value = words[1].ToString();
                                bool Save = general.performActionNoTransClient(cmd, "V2014_tube");

                                if (Save)
                                {
                                    FileInfo myfileinf = new FileInfo(filename);
                                    myfileinf.Delete();
                                    return general.TextToJson("Saved");
                                }
                                else
                                {
                                    return general.TextToJson("Failed");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //File.AppendAllText("d:/logAresh.txt", Environment.NewLine + ex + " Error : " + DateTime.Now.ToString()); 
                        }
                        break;
                    }
            }
        return general.TextToJson("Login Required");
    }
    public string UploadFileCover(string type, string filename, string id = "")
    {
        if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
        {
            switch (type)
            {
                case "channel":
                    try
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = "UPDATE vs_channel SET update_date = GETDATE(),cover = @cover  WHERE user_id = @user_id and id=@id";
                        cmd.Parameters.Add("@cover", SqlDbType.VarChar).Value = filename;
                        cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
                        cmd.Parameters.Add("@user_id", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
                        bool Save = general.performActionNoTransClient(cmd, "V2014_tube");

                        if (Save)
                        {
                            return general.TextToJson("Saved");
                        }
                        else
                        {
                            return general.TextToJson("Failed");
                        }
                    }
                    catch (Exception ex)
                    {
                        //File.AppendAllText("d:/logAresh.txt", Environment.NewLine + ex + " Error : " + DateTime.Now.ToString()); 
                    }
                    break;
            }
        }
        return general.TextToJson("Login Required");
    }

    public string UploadFileVtube(string type, string extension, string userid, string guid)
    {
        SqlCommand cmd = new SqlCommand("Update [vs_entry_details] set Filename=@name where [filename]=@guid");
        cmd.Parameters.Add("@guid", SqlDbType.VarChar).Value = guid;
        cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = guid + extension;
        general.performActionNoTransClient(cmd, "V2014_Tube");
        return "true";
    }
    #endregion
}
