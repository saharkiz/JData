using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

public partial class jData
{
    protected decimal cartTotal { get; set; }
    protected int totalQuantity { get; set; }
    private bool hasPromo { get; set; }
    private string transactionId { get; set; }
    private string promoid { get; set; }
    private string productType { get; set; }
        private DataSet getList()
        {
            DataSet ds = new DataSet();
            try
            {

                string parameters =  MainLibrary.Helper.PRODUCT;

                SqlCommand cmd = new SqlCommand("SP_Orders_GetShoppingCart");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cartType", SqlDbType.VarChar).Value = parameters.ToUpper();
                if (string.IsNullOrEmpty(MainLibrary.Helper.UserId))
                {
                    cmd.Parameters.Add("@cartId", SqlDbType.VarChar).Value = MainLibrary.Helper.shoppingCartId;
                }
                else if (!string.IsNullOrEmpty(MainLibrary.Helper.UserId))
                {
                    cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
                }
                ds = general.getSet(cmd);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        cartTotal = Convert.ToDecimal(dr["CartTotal"]);
                        totalQuantity = Convert.ToInt32(dr["TotalQuantity"]);
                        transactionId = dr["transactionId"].ToString();
                        promoid = dr["promoid"].ToString();
                        productType = dr["productType"].ToString();
                        new eException("getlist.promoid: " + promoid);
                        if (!string.IsNullOrEmpty(dr["promoid"].ToString()))
                            hasPromo = true;
                        else hasPromo = false;
                    }
                    else
                    {
                        cartTotal = 0;
                        totalQuantity = 0;
                    }
                }
                else
                {
                    cartTotal = 0;
                    totalQuantity = 0;
                }
            }
            catch (Exception ex)
            {
                //this.txtError.Text = "ERROR: CUSTOM properties are not set.";
                new eException("ShoppinCart.getList:" + ex.Message.ToString());
            }
            return ds;
        }
        private void UpdateStatus(string status, string invoiceNo)
        {
            try
            {
                if (string.IsNullOrEmpty(MainLibrary.Helper.UserId)) return;

                SqlCommand cmd = new SqlCommand(@"UPDATE t_Orders SET Status = @status, TransactionID = isnull(TransactionID,@transactionID), 
OrderDate = getdate(), UpdatedOn = getdate() ,UpdatedBy = @updatedBy WHERE 
CartType = @cartType and RecordStatus = 'Active' and Status not in ('Completed','Redeemed')  and  userID = @userId");
                cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = status;
                cmd.Parameters.Add("@transactionID", SqlDbType.VarChar).Value = invoiceNo;
                cmd.Parameters.Add("@updatedBy", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
                cmd.Parameters.Add("@userId", SqlDbType.VarChar).Value = MainLibrary.Helper.UserId;
                cmd.Parameters.Add("@cartType", SqlDbType.VarChar).Value = "PRODUCT";

                general.performActionNoTrans(cmd);

            }
            catch (Exception ex)
            {
            }
        }

        public string CheckOutFinal(string CUSTOM_Product_Redeem, string gateway)
        {
            string strArrays = MainLibrary.Helper.UserRunningNum + "-" + DateTime.Now.ToString("mmss");
            string str = DateTime.Now.ToString("MMddyy").Replace("/", "");

            string url = string.Empty;
            string script = string.Empty;
            getList();
            if (string.IsNullOrEmpty(transactionId)) transactionId = strArrays + "-" + str;

            switch (CUSTOM_Product_Redeem.ToUpper())
            {
                case MainLibrary.Helper.PRODUCT:

                    string gatewayUrl = string.Empty;
                    switch (gateway.ToUpper())
                    {
                        case "PAYPAL": gatewayUrl = "";// ConfigurationManager.AppSettings["PaypalPaymentRequestUrl"];
                            break;
                        case "RHB": gatewayUrl = "https://payment.the-v.net:8081/PaymentRequest_rhb.aspx";// ConfigurationManager.AppSettings["RHBPaymentRequestUrl"];
                            break;
                        case "PBB": gatewayUrl = "";// ConfigurationManager.AppSettings["PBBPaymentRequestUrl"];
                            break;
                    }
                    if (cartTotal > (decimal)0.0)
                    {
                        this.UpdateStatus("Pending", transactionId);
                        url = gatewayUrl + "?sid=" + MainLibrary.Helper.shoppingCartId + "&uid=" + MainLibrary.Helper.UserId + "&inv=" + transactionId;
                    }
                    else
                    {
                        this.UpdateStatus("COMPLETED", transactionId);
                        url = "/vshoppe/payment-response?session=c509a6f75849bc4ca4238a0b923820dc&sid=" + MainLibrary.Helper.shoppingCartId + "&trans=" + transactionId;
                    }

                    break;
                case MainLibrary.Helper.REDEEM:
                    //this.Redeem(transactionId);
                    //url = "/vshoppe/payment-response?session=c4ca4238a0b923820dcc509a600000&trans=R-" + transactionId;

                    break;
                default:  
                        return "[{\"error\":\"Shopping cart has encountered a problem.\"}]";
            }
            return general.TextToJson(url);
        }

    }
