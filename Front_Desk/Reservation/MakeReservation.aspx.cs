﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Hotel_Management_System.Utility;

namespace Hotel_Management_System.Front_Desk.Reservation
{
    public partial class MakeReservation : System.Web.UI.Page
    {
        // Create instance of IDGerator class
        IDGenerator idGenerator = new IDGenerator();

        // Create instance of Encryption class
        IDEncryption en = new IDEncryption();

        // Create connection to database
        SqlConnection conn;
        String strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        // Create instance of ReservationUltility class
        ReservationUtility reservationUtility = new ReservationUtility();

        // Declare list of room availability
        //List<AvailableRoom> availableRoom = new List<AvailableRoom>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // set all guest into drop-down list
                setGuestList();
                ddlGuest.Items.Insert(0, new ListItem("-- Please Select --", "-- Please Select --"));

                // Set left margin for lblDurationOfStay
                lblDurationOfStay.Style["margin-left"] = "30px";
                LBAddGuest.Style["margin-left"] = "30px";

                // No of PNRoomReservationForm that Visible = true
                ViewState["NoOfRoomReservationForm"] = 1;

                // get room availability
                Session["AvailableRoom"] = new List<AvailableRoom>();

                // get facility availability
                Session["AvailableFacility"] = new List<AvailableFacility>();
                Session["AvailableFacility"] = getFacilityAvailability("");

                // define RentedFacilityList
                Session["RentedFacilityList"] = new List<RentedFacility>();

            }

            CompareValidator1.Validate();
            
        }

        private void setGuestList()
        {
            conn = new SqlConnection(strCon);
            conn.Open();

            // Get all guest
            String getGuest = "SELECT GuestID, Name, IDNo FROM Guest";

            SqlCommand cmdGetGuest = new SqlCommand(getGuest, conn);

            SqlDataAdapter sda = new SqlDataAdapter(cmdGetGuest);

            DataTable dt = new DataTable();

            sda.Fill(dt);

            // Set guest into drop-down list

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ddlGuest.Items.Add(new ListItem((dt.Rows[i]["Name"].ToString() + " - " + dt.Rows[i]["IDNo"].ToString()), dt.Rows[i]["GuestID"].ToString()));
            }


            conn.Close();
        }

        protected void LBBack_Click(object sender, EventArgs e)
        {

        }

        protected void ddlGuest_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Display guest details
            PNStayDetails.Visible = true;

            // Get selected guestID
            String guestID = ddlGuest.SelectedValue;

            conn = new SqlConnection(strCon);
            conn.Open();

            // Get guest details for the selected guest
            String getGuestDetails = "SELECT * FROM Guest WHERE GuestID LIKE @ID";

            SqlCommand cmdGetGuestDetails = new SqlCommand(getGuestDetails, conn);

            cmdGetGuestDetails.Parameters.AddWithValue("@ID", guestID);

            SqlDataReader sdr = cmdGetGuestDetails.ExecuteReader();

            // Set guest details into label
            if (sdr.Read())
            {
                lblGuestName.Text = sdr.GetString(sdr.GetOrdinal("Name"));
                lblIDNo.Text = sdr.GetString(sdr.GetOrdinal("IDNo"));
            }

            conn.Close();
        }

        

        protected void btnSave_Click(object sender, EventArgs e)
        {

            //Response.Redirect("Payment.aspx");

        }

        protected void formBtnCancel_Click(object sender, EventArgs e)
        {

        }

        protected void txtCheckInDate_TextChanged(object sender, EventArgs e)
        {
            // Check if user have select any checkout date
            if (txtCheckOutDate.Text == "")
            {
                // Set check out date automatically
                DateTime checkInDate = Convert.ToDateTime(txtCheckInDate.Text);

                txtCheckOutDate.Text = reservationUtility.getNextDate(checkInDate.ToString());
            }

            // Run the comparevalidator
            CompareValidator1.Validate();

            // Check if check-in and check-out date is valid
            if (!CompareValidator1.IsValid)
            {
                lblDurationOfStay.Text = "";

                // Style if error occurred
                txtCheckInDate.Style["border-bottom"] = "2px solid rgb(255, 0, 0)";
                txtCheckInDate.Style["background-color"] = "rgb(255, 240, 240)";

                txtCheckOutDate.Style["border-bottom"] = "2px solid rgb(255, 0, 0)";
                txtCheckOutDate.Style["background-color"] = "rgb(255, 240, 240)";

                lblDurationOfStay.Text = "";

                PNReserveRoom.Visible = false;
            }
            else
            {
                // Style if no error occurred
                txtCheckInDate.Style["border-bottom"] = "border-bottom: 2px solid rgb(128, 128, 128);";
                txtCheckInDate.Style["background-color"] = "none";

                txtCheckOutDate.Style["border-bottom"] = "border-bottom: 2px solid rgb(128, 128, 128);";
                txtCheckOutDate.Style["background-color"] = "none";

                PNReserveRoom.Visible = true;

                Session["AvailableRoom"] = getRoomAvailability();

                // Set room type into dropdown
                setRoomType(1);

                // Set facility into dropdown
                setFacility();

                // Set duration of stay
                lblDurationOfStay.Text = reservationUtility.getdurationOfStay(txtCheckInDate.Text, txtCheckOutDate.Text).ToString() + " Night";
            }
        }

        protected void txtCheckOutDate_TextChanged(object sender, EventArgs e)
        {
            // Run the comparevalidator
            CompareValidator1.Validate();

            // Check if check-in and check-out date is valid
            if (!CompareValidator1.IsValid)
            {
                lblDurationOfStay.Text = "";

                // Style if error occurred
                txtCheckInDate.Style["border-bottom"] = "2px solid rgb(255, 0, 0)";
                txtCheckInDate.Style["background-color"] = "rgb(255, 240, 240)";

                txtCheckOutDate.Style["border-bottom"] = "2px solid rgb(255, 0, 0)";
                txtCheckOutDate.Style["background-color"] = "rgb(255, 240, 240)";

                lblDurationOfStay.Text = "";

                PNReserveRoom.Visible = false;
            }
            else
            {
                // Style if no error occurred
                txtCheckInDate.Style["border-bottom"] = "border-bottom: 2px solid rgb(128, 128, 128);";
                txtCheckInDate.Style["background-color"] = "none";

                txtCheckOutDate.Style["border-bottom"] = "border-bottom: 2px solid rgb(128, 128, 128);";
                txtCheckOutDate.Style["background-color"] = "none";

                // Set duration of stay
                lblDurationOfStay.Text = reservationUtility.getdurationOfStay(txtCheckInDate.Text, txtCheckOutDate.Text).ToString() + " Night";

                PNReserveRoom.Visible = true;

                Session["AvailableRoom"] = getRoomAvailability();
            }

            
        }

        private void setFacility()
        {
            // Get reference of the ddlFacility & 
            DropDownList ddlFacility = PNReserveRoom.FindControl("ddlFacilityName") as DropDownList;
            DropDownList ddlFacilityQty = PNReserveRoom.FindControl("ddlFacilityQty") as DropDownList;

            // Get facility availability from Session 
            List<AvailableFacility> availableFacility = (List<AvailableFacility>)Session["AvailableFacility"];

            // Clear all items
            ddlFacility.Items.Clear();
            ddlFacilityQty.Items.Clear();

            // Set first item
            ddlFacility.Items.Add(new ListItem("-- Please Select --"));

            for (int i = 0; i < availableFacility.Count; i++)
            {
                if (availableFacility[i].availableQty > 0)
                {
                    ddlFacility.Items.Add(new ListItem(availableFacility[i].facilityName, availableFacility[i].facilityID));
                }
            }

        }

        protected void LBAddGuest_Click(object sender, EventArgs e)
        {
            // Navigate to add guest
            Response.Redirect("~/Front_Desk/Guest/AddGuest.aspx");
        }

        protected void AddReservationForm(object sender, EventArgs e)
        {
            // Get reference of LinkButton current clicked
            LinkButton lbAdd = (LinkButton)sender;

            // Get ID of LinkButton
            String id = lbAdd.ID;

            // Get last value of ID
            // To identify which panel is being click
            int index = id.Last() - '0';

            // Get ID for next panel
            String nextReservationPanelID = "PNReservationForm" + (index + 1).ToString();

            // Get reference of next panel
            Panel PNReservationForm = PNReserveRoom.FindControl(nextReservationPanelID) as Panel;

            // Make the panel visible to the user
            PNReservationForm.Visible = true;

            refreshDDLRoomType(index + 1);

            // Get 
            LinkButton currentLBAdd = PNReserveRoom.FindControl("LBAddReservationForm" + index.ToString()) as LinkButton;

            currentLBAdd.Visible = false;

            int i = (int)ViewState["NoOfRoomReservationForm"];

            i++;

            ViewState["NoOfRoomReservationForm"] = i;

            setRoomType(i);
        }

        protected void DeleteReservationForm(object sender, EventArgs e)
        {
            // Get reference of LinkButton current clicked
            LinkButton lbDelete = (LinkButton)sender;

            // Get ID of LinkButton
            String id = lbDelete.ID;

            // Get last value of ID
            // To identify which panel is being click
            int index = id.Last() - '0';

            String currentReservationPanelID;

            int noOfRoomReservationForm = (int)ViewState["NoOfRoomReservationForm"];

            // Get reference of the current ddlRoomType
            DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + index.ToString()) as DropDownList;

            // Check if the panel is last panel
            if (index < noOfRoomReservationForm)
            {
                // If no
                // shift the content forward on step
                // Before making the panel invisible to user
                if(ddlRoomType.SelectedValue != "-- Please Select --")
                {
                    increaseRoomQty(ddlRoomType.SelectedValue, index);
                }

                shiftPanelContent(index);

                currentReservationPanelID = "PNReservationForm" + noOfRoomReservationForm.ToString();

                LinkButton currentLBAdd = PNReserveRoom.FindControl("LBAddReservationForm" + index.ToString()) as LinkButton;

                currentLBAdd.Visible = true;

                resetReservationFormData(noOfRoomReservationForm);

                refreshDDLRoomType(index);
            }
            else
            {
                currentReservationPanelID = "PNReservationForm" + index.ToString();

                LinkButton currentLBAdd = PNReserveRoom.FindControl("LBAddReservationForm" + (index - 1).ToString()) as LinkButton;

                currentLBAdd.Visible = true;

                if (ddlRoomType.SelectedValue != "-- Please Select --")
                {
                    increaseRoomQty(ddlRoomType.SelectedValue, index);
                }

                //refreshDDLRoomType(index);

                resetReservationFormData(index);

                refreshDDLRoomType(index);
            }

            // Make the panel invisible to user
            Panel PNReservationForm = PNReserveRoom.FindControl(currentReservationPanelID) as Panel;

            PNReservationForm.Visible = false;

            // Update total number of visible reservation form
            noOfRoomReservationForm--;

            ViewState["NoOfRoomReservationForm"] = noOfRoomReservationForm;

        }

        private void resetReservationFormData(int index)
        {
            // Reset all for a specific reservation form
            DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + index.ToString()) as DropDownList;

            DropDownList ddlAdults = PNReserveRoom.FindControl("ddlAdults" + index.ToString()) as DropDownList;

            TextBox txtAdults = PNReserveRoom.FindControl("txtAdults" + index.ToString()) as TextBox;

            DropDownList ddlKids = PNReserveRoom.FindControl("ddlKids" + index.ToString()) as DropDownList;

            TextBox txtKids = PNReserveRoom.FindControl("txtKids" + index.ToString()) as TextBox;

            CheckBox cbExtraBed = PNReserveRoom.FindControl("cbExtraBed" + index.ToString()) as CheckBox;

            Label lblExtraBed = PNReserveRoom.FindControl("lblExtraBed" + index.ToString()) as Label;

            ddlRoomType.Items.Clear();

            ddlAdults.Items.Clear();

            ddlAdults.Visible = true;

            txtAdults.Text = "";

            txtAdults.Visible = false;

            ddlKids.Items.Clear();

            ddlKids.Visible = true;

            txtKids.Text = "";

            txtKids.Visible = false;

            cbExtraBed.Checked = false;

            cbExtraBed.Visible = false;

            lblExtraBed.Visible = false;
        }

        private void shiftPanelContent(int currentIndex)
        {
            int noOfRoomReservationForm = (int)ViewState["NoOfRoomReservationForm"];

            // Shift panel one step forward
            for (int i = currentIndex; i < noOfRoomReservationForm; i++)
            {
                // Get reference for all control in current and next panel
                DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + currentIndex.ToString()) as DropDownList;

                DropDownList nextDDLRoomType = PNReserveRoom.FindControl("ddlRoomType" + (currentIndex + 1).ToString()) as DropDownList;

                DropDownList ddlAdults = PNReserveRoom.FindControl("ddlAdults" + currentIndex.ToString()) as DropDownList;

                DropDownList nextDDLAdults = PNReserveRoom.FindControl("ddlAdults" + (currentIndex + 1).ToString()) as DropDownList;

                TextBox txtAdults = PNReserveRoom.FindControl("txtAdults" + currentIndex.ToString()) as TextBox;

                TextBox nextTXTAdults = PNReserveRoom.FindControl("txtAdults" + (currentIndex + 1).ToString()) as TextBox;

                DropDownList ddlKids = PNReserveRoom.FindControl("ddlKids" + currentIndex.ToString()) as DropDownList;

                DropDownList nextDDLKids = PNReserveRoom.FindControl("ddlKids" + (currentIndex + 1).ToString()) as DropDownList;

                TextBox txtKids = PNReserveRoom.FindControl("txtKids" + currentIndex.ToString()) as TextBox;

                TextBox nextTXTKids = PNReserveRoom.FindControl("txtKids" + (currentIndex + 1).ToString()) as TextBox;

                CheckBox cbExtraBed = PNReserveRoom.FindControl("cbExtraBed" + currentIndex.ToString()) as CheckBox;

                CheckBox nextCBExtraBed = PNReserveRoom.FindControl("cbExtraBed" + (currentIndex + 1).ToString()) as CheckBox;

                Label lblExtraBed = PNReserveRoom.FindControl("lblExtraBed" + currentIndex.ToString()) as Label;

                Label nextLBLExtraBed = PNReserveRoom.FindControl("lblExtraBed" + (currentIndex + 1).ToString()) as Label;

                // *** ddlRoomType ***
                ddlRoomType.Items.Clear();
                ddlRoomType.Items.AddRange(nextDDLRoomType.Items.OfType<ListItem>().ToArray());
                ddlRoomType.SelectedValue = nextDDLRoomType.SelectedValue;  // Set the option currently selected
                

                // *** ddlAdults ***
                ddlAdults.DataSource = nextDDLAdults.Items;
                ddlAdults.DataBind();
                ddlAdults.SelectedIndex = nextDDLAdults.SelectedIndex;
                ddlAdults.Visible = nextDDLAdults.Visible;

                // *** txtAdults ***
                txtAdults.Text = nextTXTAdults.Text;
                txtAdults.Visible = nextTXTAdults.Visible;

                //*** ddlKids ***
                ddlKids.DataSource = nextDDLKids.Items;
                ddlKids.DataBind();
                ddlKids.SelectedIndex = nextDDLKids.SelectedIndex;
                ddlKids.Visible = nextDDLKids.Visible;

                // *** txtKids ***
                txtKids.Text = nextTXTKids.Text;
                txtKids.Visible = nextTXTKids.Visible;

                // *** cbExtraBed ***
                cbExtraBed.Visible = nextCBExtraBed.Visible;
                cbExtraBed.Checked = nextCBExtraBed.Checked;

                // *** lblExtraBed ***
                lblExtraBed.Text = nextLBLExtraBed.Text;
                lblExtraBed.Visible = nextLBLExtraBed.Visible;




            }
        }

        protected void DropDownSelectRoomType(object sender, EventArgs e)
        {
            // Get reference of DropDownList current selected
            DropDownList ddlRoomType = (DropDownList)sender;

            // Get the ID of DropDownList
            String id = ddlRoomType.ID;

            // Get last value of ID
            // To identify which panel is being click
            int index = id.Last() - '0';

            // Get the id of link button for current panel
            String currrentLBAddReservationForm = "LBAddReservationForm" + index.ToString();

            // Get reference to the link button add
            LinkButton addReservationForm = PNReserveRoom.FindControl(currrentLBAddReservationForm) as LinkButton;

            int noOfRoomReservationForm = (int)ViewState["NoOfRoomReservationForm"];
            // If the it is not the last panel
            if (noOfRoomReservationForm <= 3)
            {
                // DropDownList already been selected 
                if (ddlRoomType.SelectedIndex != 0)
                {
                    // Make the add next panel button visible to user
                    //setRoomTypeQty(index);
                    setExtraBedOption(index);

                    setOccupancy(index);

                    reduceRoomQty("ddlRoomType" + index.ToString());

                    refreshDDLRoomType(index);

                    if(!(index < noOfRoomReservationForm))
                    {
                        addReservationForm.Visible = true;
                    }
                    

                }
                else
                {
                    // Else
                    // Make it invisible to the user
                    addReservationForm.Visible = false;

                    refreshDDLRoomType(index);
                }
            }

            

        }

        private void setExtraBedOption(int index)
        {
            // Get reference of cbExtraBed of current index
            CheckBox cbExtraBed = PNReserveRoom.FindControl("cbExtraBed" + index.ToString()) as CheckBox;

            Label lblExtraBed = PNReserveRoom.FindControl("lblExtraBed" + index.ToString()) as Label;

            // Get reference to ddlAdults of current index
            DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + index.ToString()) as DropDownList;

            // Get reference of AvailableRoom store inside session
            List<AvailableRoom> availableRoom = (List<AvailableRoom>)Session["AvailableRoom"];


            for (int i = 0; i < availableRoom.Count; i++)
            {

                // If found the roomTypeID selcted by the user
                if (ddlRoomType.SelectedValue == availableRoom[i].roomTypeID)
                {
                    // Check if the extrabed is allowed
                    if(availableRoom[i].extraBed == "True")
                    {
                        lblExtraBed.Visible = true;
                        cbExtraBed.Visible = true;
                    }
                    else
                    {
                        lblExtraBed.Visible = false;
                        cbExtraBed.Visible = false;
                    }

                }
            }
        }

        private void setOccupancy(int index)
        {
            // Get reference to ddlAdults of current index
            DropDownList ddlAdults = PNReserveRoom.FindControl("ddlAdults" + index.ToString()) as DropDownList;

            // Get reference to ddlAdults of current index
            DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + index.ToString()) as DropDownList;

            // Get reference of AvailableRoom store inside session
            List<AvailableRoom> availableRoom = (List<AvailableRoom>)Session["AvailableRoom"];

            // Remove all items inside ddlAdults
            ddlAdults.Items.Clear();

            // Set the list of quantity option to ddlAdults
            for (int i = 0; i < availableRoom.Count; i++)
            {
                AvailableRoom ar = new AvailableRoom();

                ar = availableRoom[i];

                // If found the roomTypeID selcted by the user
                if (ddlRoomType.SelectedValue == ar.roomTypeID)
                {
                    // Set the quantity into ddlAdults
                    for (int j = 0; j <= ar.higherOccupancy; j++)
                    {
                        ddlAdults.Items.Add(new ListItem(j.ToString()));
                    }
                }
            }
        }

        private void reduceRoomQty(string roomTypeID)
        {
            // Get total number of reservation form there are visible to the user currently
            int noOfRoomReservationForm = (int)ViewState["NoOfRoomReservationForm"];

            // Get reference of AvailableRoom store inside session
            Session["AvailableRoom"] = getRoomAvailability();

            // Get the reference for AvailableRoom List object from Session
            List<AvailableRoom> availableRoom = (List<AvailableRoom>)Session["AvailableRoom"];
            
            // Loop though the reservation form visible to be user currently
            for(int i = 1; i <= noOfRoomReservationForm; i++)
            {
                // Loop to reach the roomType there are selected by the user.
                for (int j = 0; j < availableRoom.Count; j++)
                {
                    DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + i.ToString()) as DropDownList;

                    // If reach, reduce the room quantity by one
                    if (availableRoom[j].roomTypeID == ddlRoomType.Text)
                    {
                        availableRoom[j].quantity--;

                    }
                }
            }
            
        }

        private void increaseRoomQty(string roomTypeID, int index)
        {

            // Get the reference for AvailableRoom List object from Session
            List<AvailableRoom> availableRoom = (List<AvailableRoom>)Session["AvailableRoom"];

            // Loop to reach the roomType there are selected by the user.
            for (int i = 0; i < availableRoom.Count; i++)
            {
                //DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + index.ToString()) as DropDownList;

                // If reach, reduce the room quantity by one
                if (availableRoom[i].roomTypeID == roomTypeID)
                {
                    availableRoom[i].quantity++;

                }
            }

        }

        // *** Not Used***
        private void setRoomTypeQty(int index)
        {
            // Get reference to ddlRoomTypeQty of current index
            DropDownList roomTypeQty = PNReserveRoom.FindControl("ddlRoomTypeQty" + index.ToString()) as DropDownList;

            // Get reference to ddlRoomType of current index
            DropDownList roomType = PNReserveRoom.FindControl("ddlRoomType" + index.ToString()) as DropDownList;

            // Clear all ddlRoomTypeQty option
            roomTypeQty.Items.Clear();

            // Get reference of AvailableRoom store inside session
            List<AvailableRoom> availableRoom = (List<AvailableRoom>)Session["AvailableRoom"];

            for (int i = 0; i < availableRoom.Count; i++)
            {
                AvailableRoom ar = new AvailableRoom();

                ar = availableRoom[i];


                if(roomType.SelectedValue == ar.roomTypeID)
                {
                    for(int j = 1; j <= ar.quantity; j++)
                    {
                        roomTypeQty.Items.Add(new ListItem(j.ToString()));
                    }
                }
            }

        }


        protected void LBCheckAvailability_Click(object sender, EventArgs e)
        {
            // Display the popup that are showing room availability
            PopupBoxRoomAvailability.Visible = true;
            PopupCover.Visible = true;
            setItemToRepeater1();
        }

        protected void IBClosePopUpBox_Click(object sender, ImageClickEventArgs e)
        {

            PopupCover.Visible = false;
            PopupBoxRoomAvailability.Visible = false;
            PopupBoxFacilityAvailability.Visible = false;
        }

        private void setItemToRepeater1()
        {
            // Get reference of AvailableRoom store inside session
            List<AvailableRoom> availableRoom = (List<AvailableRoom>)Session["AvailableRoom"];

            // Set data into repeater 1
            Repeater1.DataSource = availableRoom;
            Repeater1.DataBind();

        }

        private List<AvailableRoom> getRoomAvailability()
        {
            conn = new SqlConnection(strCon);
            conn.Open();

            // Query to get room availability
            string getRoomType = "SELECT (RT.RoomTypeID) AS RoomTypeID, (RT.Title) AS Title, (RT.BaseOccupancy) AS BaseOccupancy, (RT.HigherOccupancy) AS HigherOccupancy, (RT.ExtraBed) AS ExtraBed, " +
                                "coalesce(R.Count, 0) AS NumberOfRoom " +
                                "from RoomType RT " +
                                "left join ( " +
                                "select RoomTypeID, count(*) as Count " +
                                "from Room " +
                                "Where Status IN('Active', 'Blocked') " +
                                "group by RoomTypeID " +
                                ") R on(RT.RoomTypeID = R.RoomTypeID) " +
                                "WHERE RT.Status LIKE 'Active'";

            SqlCommand cmdGetRoomType = new SqlCommand(getRoomType, conn);

            // Hold the data read from database
            var sdr = cmdGetRoomType.ExecuteReader();

            // Get reference of AvailableRoom store inside session
            List<AvailableRoom> availableRoom = new List<AvailableRoom>();

            // set all data into list object 
            while (sdr.Read())
            {
                string roomTypeID = sdr.GetString(sdr.GetOrdinal("RoomTypeID"));

                AvailableRoom ar = new AvailableRoom(
                    sdr.GetString(sdr.GetOrdinal("RoomTypeID")),
                    sdr.GetString(sdr.GetOrdinal("Title")),
                    sdr.GetInt32(sdr.GetOrdinal("NumberOfRoom")),
                    sdr.GetInt32(sdr.GetOrdinal("BaseOccupancy")),
                    sdr.GetInt32(sdr.GetOrdinal("HigherOccupancy")),
                    sdr.GetString(sdr.GetOrdinal("ExtraBed")),
                    "Available");

                availableRoom.Add(ar);
            }

            conn.Close();

            string[] reservationDate = reservationUtility.getReservationDate(txtCheckInDate.Text, txtCheckOutDate.Text);

            // Reduce room quantity there have already been selected by the user.
            for (int i = 0; i < availableRoom.Count; i++)
            {
                for(int j = 0; j < reservationDate.Length; j++)
                {
                    conn = new SqlConnection(strCon);
                    conn.Open();

                    AvailableRoom ar = availableRoom.ElementAt(i);

                    int qty = getReservedRoomTypeQty(ar.roomTypeID, reservationDate[j]);

                    ar.quantity -= qty;

                    if (ar.quantity == 0)
                    {
                        ar.status = "Unavailable";
                    }

                    conn.Close();
                }
            }

            return availableRoom;
        }

        private void setRoomType(int index)
        {
            // Get reference of the ddlRoomType
            DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + index.ToString()) as DropDownList;

            // Get reference of AvailableRoom store inside session
            List<AvailableRoom> availableRoom = (List<AvailableRoom>)Session["AvailableRoom"];

            // Clear all items
            ddlRoomType.Items.Clear();

            // Set first item
            ddlRoomType.Items.Add(new ListItem("-- Please Select --"));

            for (int i = 0; i < availableRoom.Count; i++)
            {
                if(availableRoom[i].quantity > 0)
                {
                    ddlRoomType.Items.Add(new ListItem(availableRoom[i].roomType, availableRoom[i].roomTypeID));
                }
            }
            
        }

        private int getReservedRoomTypeQty(string roomTypeID, string date)
        {
            // Query to get the quantity of room have reserved by other guest
            String getReservedRoomType = "SELECT COUNT(RR.RoomTypeID) AS TotalReserved, RR.RoomTypeID AS RoomTypeID " +
                 "FROM Reservation R, ReservationRoom RR " +
                 "WHERE R.ReservationID LIKE RR.ReservationID AND R.Status LIKE 'Created' AND RR.RoomTypeID LIKE @ID AND RR.Date LIKE @Date " +
                 "GROUP BY RR.RoomTypeID ";

            SqlCommand cmdGetReservedRoomType = new SqlCommand(getReservedRoomType, conn);

            cmdGetReservedRoomType.Parameters.AddWithValue("@ID", roomTypeID);
            cmdGetReservedRoomType.Parameters.AddWithValue("@Date", date);

            // Hold the data read from database
            SqlDataReader sdr = cmdGetReservedRoomType.ExecuteReader();

            if (sdr.Read())
            {
                return sdr.GetInt32(sdr.GetOrdinal("TotalReserved"));
            }

            // If not any reservation found under the category
            return 0;
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            // Get control's reference
            Label lblStatus = e.Item.FindControl("lblStatus") as Label;

            if (lblStatus.Text == "Available")
            {
                lblStatus.Style["color"] = "#00ce1b";  // Assign green color
            }
            else
            {
                lblStatus.Style["color"] = "red";  // Assign red color
            }
        }

        protected void CheckBoxExtraBed(object sender, EventArgs e)
        {
            // Get reference of checkbox current selected
            CheckBox cbExtraBed = (CheckBox)sender;

            // Get the ID of checkbox
            String id = cbExtraBed.ID;

            // Get last value of ID
            // To identify which panel is being click
            int index = id.Last() - '0';

            // Get the id of ddlAdults and ddlKids for current panel
            String currentDDLAdults = "ddlAdults" + index.ToString();
            String currentDDLKids = "ddlKids" + index.ToString();

            // Get the id of txtAdults and txtKids for current panel
            String currentTxtAdults = "txtAdults" + index.ToString();
            String currentTxtKids = "txtKids" + index.ToString();

            // Get reference of the ddlAdults and ddlKids
            DropDownList ddlAdults = PNReserveRoom.FindControl(currentDDLAdults) as DropDownList;
            DropDownList ddlKids = PNReserveRoom.FindControl(currentDDLKids) as DropDownList;

            // Get reference of the txtAdults and txtKids
            TextBox txtAdults = PNReserveRoom.FindControl(currentTxtAdults) as TextBox;
            TextBox txtKids = PNReserveRoom.FindControl(currentTxtKids) as TextBox;

            // Change visibility for textbox and dropdown
            if (cbExtraBed.Checked)
            {
                // If cbExtrabed has checked display textbox
                ddlAdults.Visible = false;
                txtAdults.Visible = true;

                ddlKids.Visible = false;
                txtKids.Visible = true;

                // Set the previous dropdownlist value into textbox
                txtAdults.Text = ddlAdults.SelectedValue;
                txtKids.Text = ddlKids.SelectedValue;
            }
            else
            {
                // Else display dropdown
                ddlAdults.Visible = true;
                txtAdults.Visible = false;

                ddlKids.Visible = true;
                txtKids.Visible = false;
            }
        }

        protected void DropDownAdults(object sender, EventArgs e)
        {
            // Get reference of checkbox current selected
            DropDownList ddlAdults = (DropDownList)sender;

            // Get ID of LinkButton
            String id = ddlAdults.ID;

            // Get last value of ID
            // To identify which panel is being click
            int index = id.Last() - '0';

            // Get reference to ddlAdults of current index
            DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + index.ToString()) as DropDownList;

            // Get reference to ddlKids of current index
            DropDownList ddlKids = PNReserveRoom.FindControl("ddlKids" + index.ToString()) as DropDownList;

            int selectedIndex;

            if (ddlKids.Items.Count > 0)
            {
                selectedIndex = ddlKids.SelectedIndex;

            }
            else
            {
                selectedIndex = -1;
            }

            // Remove all items inside ddlKids
            ddlKids.Items.Clear();

            // Get reference of AvailableRoom store inside session
            List<AvailableRoom> availableRoom = (List<AvailableRoom>)Session["AvailableRoom"];

            // Loop to reach the roomType selected by the user
            for (int i = 0; i < availableRoom.Count; i++)
            {
                if (ddlRoomType.SelectedValue == availableRoom[i].roomTypeID)
                {
                    // Set the remaining quantity into ddlKids
                    for (int j = 0; j <= availableRoom[i].higherOccupancy - int.Parse(ddlAdults.SelectedValue); j++)
                    {
                        ddlKids.Items.Add(j.ToString());
                    }
                }
            }

            if (selectedIndex != -1)
            {
                ddlKids.SelectedIndex = selectedIndex;
            }
        }

        protected void DropDownKids(object sender, EventArgs e)
        {
            // Get reference of checkbox current selected
            DropDownList ddlKids = (DropDownList)sender;

            // Get ID of LinkButton
            String id = ddlKids.ID;

            // Get last value of ID
            // To identify which panel is being click
            int index = id.Last() - '0';

            // Get reference to ddlAdults of current index
            DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + index.ToString()) as DropDownList;

            // Get reference to ddlAdults of current index
            DropDownList ddlAdults = PNReserveRoom.FindControl("ddlAdults" + index.ToString()) as DropDownList;

            // Hold the option selected by the user currently
            int totalNoOfAdults = ddlAdults.SelectedIndex;

            // Update the occupancy to ddlAdults
            setOccupancy(index);

            // set back the selected index
            ddlAdults.SelectedIndex = totalNoOfAdults;

            int totalNoOfItem = ddlAdults.Items.Count - 1;

            // Remove the option base on the qty for kids
            for (int i = ddlAdults.Items.Count - 1; i > totalNoOfItem - ddlKids.SelectedIndex; i--)
            {
                ddlAdults.Items.Remove(ddlAdults.Items[i]);
            }
        }

        private void refreshDDLRoomType(int currentIndex)
        {
            // Get totalNoOFReservation Form
            int noOfRoomReservationForm = (int)ViewState["NoOfRoomReservationForm"];

            // Get reference of AvailableRoom store inside session
            List<AvailableRoom> availableRoom = (List<AvailableRoom>)Session["AvailableRoom"];

            DropDownList ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + currentIndex.ToString()) as DropDownList;

            string selectedRoomTypeID;

            if (ddlRoomType.SelectedValue == "-- Please Select --")
            {
                availableRoom = getRoomAvailability();

                for (int i = 1; i < noOfRoomReservationForm; i++)
                {
                    if (currentIndex != i)
                    {
                        ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + i.ToString()) as DropDownList;

                        selectedRoomTypeID = ddlRoomType.SelectedValue;

                        for (int j = 0; j < availableRoom.Count; j++)
                        {
                            if (availableRoom[j].roomTypeID == selectedRoomTypeID)
                            {
                                Session["AvailableRoom"] = availableRoom;

                                setRoomType(i);

                                availableRoom[j].quantity--;

                                ddlRoomType.SelectedValue = selectedRoomTypeID;
                            }


                        }
                    }

                }

            }
            else
            { 
                for (int i = 1; i <= noOfRoomReservationForm; i++)
                {
                    //if (i != currentIndex)
                    //{
                        ddlRoomType = PNReserveRoom.FindControl("ddlRoomType" + i.ToString()) as DropDownList;

                        selectedRoomTypeID = ddlRoomType.SelectedValue;

                        if (selectedRoomTypeID != "")
                        {
                            for (int j = 0; j < availableRoom.Count; j++)
                            {
                                if (availableRoom[j].roomTypeID == selectedRoomTypeID)
                                {
                                    availableRoom[j].quantity++;
                                }
                            }

                            setRoomType(i);

                            ddlRoomType.SelectedValue = selectedRoomTypeID;

                            for (int j = 0; j < availableRoom.Count; j++)
                            {
                                if (availableRoom[j].roomTypeID == selectedRoomTypeID)
                                {
                                    availableRoom[j].quantity--;
                                }
                            }
                        }

                    //}
                }
            }
        }

        protected void LBCheckFacilityAvailability_Click(object sender, EventArgs e)
        {
            PopupBoxFacilityAvailability.Visible = true;
            PopupCover.Visible = true;

            txtCheckFacilityDate.Text = txtCheckInDate.Text;

            setItemToRepeater2(txtCheckFacilityDate.Text);
        }

        private List<AvailableFacility> getFacilityAvailability(string date)
        {
            conn = new SqlConnection(strCon);
            conn.Open();

            // Query to get facility availability
            string getFacility = "SELECT F.FacilityID, F.FacilityName, F.Quantity, F.Price, F.PriceType " +
                                "FROM Facility F " +
                                "WHERE F.Status LIKE 'Active'";

            SqlCommand cmdGetFacility = new SqlCommand(getFacility, conn);

            // Hold the data read from database
            var sdr = cmdGetFacility.ExecuteReader();

            // Get reference of AvailableFacility stored inside session
            List<AvailableFacility> availableFacility = new List<AvailableFacility>();

            // set all data into list object 
            while (sdr.Read())
            {

                AvailableFacility af = new AvailableFacility(
                    sdr.GetString(sdr.GetOrdinal("FacilityID")),
                    sdr.GetString(sdr.GetOrdinal("FacilityName")),
                    sdr.GetInt32(sdr.GetOrdinal("Quantity")),
                    Convert.ToDouble(sdr.GetDecimal(sdr.GetOrdinal("Price"))),
                    sdr.GetString(sdr.GetOrdinal("PriceType")),
                    "Available",
                    "No");

                availableFacility.Add(af);
            }

            conn.Close();

            if(date != "")
            {
                // Reduce facility quantity there have already been selected by the user.
                for (int i = 0; i < availableFacility.Count; i++)
                {

                    conn = new SqlConnection(strCon);
                    conn.Open();

                    AvailableFacility af = availableFacility.ElementAt(i);

                    int qty = getFacilityRentedQty(af.facilityID, date);

                    af.availableQty -= qty;

                    if (af.availableQty == 0)
                    {
                        af.status = "Unavailable";
                    }

                    conn.Close();
                }
            }

            return availableFacility;
        }

        public int getFacilityRentedQty(string facilityID, string date)
        {
            // Query to get the quantity of facility have reserved by other guest
            String getReservedFacility = "SELECT FacilityID, COUNT(FacilityID) AS RentedQty " + 
                                        "FROM ReservationFacility " + 
                                        "WHERE DateRented LIKE @Date AND FacilityID LIKE @FacilityID " + 
                                        "GROUP BY FacilityID ";

            SqlCommand cmdGetReservedFacility = new SqlCommand(getReservedFacility, conn);

            cmdGetReservedFacility.Parameters.AddWithValue("@FacilityID", facilityID);
            cmdGetReservedFacility.Parameters.AddWithValue("@Date", date);

            // Hold the data read from database
            SqlDataReader sdr = cmdGetReservedFacility.ExecuteReader();

            if (sdr.Read())
            {
                return sdr.GetInt32(sdr.GetOrdinal("RentedQty"));
            }

            // If not any reservation found under the category
            return 0;
        }

        private void setItemToRepeater2(string date)
        {
           // getFacilityAvailability(date);

            // Set data into repeater 1
            Repeater2.DataSource = getFacilityAvailability(date);
            Repeater2.DataBind();
        }

        protected void Repeater2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Get control's reference
            Label lblStatus = e.Item.FindControl("lblStatus") as Label;

            if (lblStatus.Text == "Available")
            {
                lblStatus.Style["color"] = "#00ce1b";  // Assign green color
            }
            else
            {
                lblStatus.Style["color"] = "red";  // Assign red color
            }
        }

        protected void txtCheckFacilityDate_TextChanged(object sender, EventArgs e)
        {
            
            setItemToRepeater2(txtCheckFacilityDate.Text);
        }

        protected void ddlFacilityName_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            txtRentDate.Text = txtCheckInDate.Text;
            txtReturnDate.Text = txtCheckInDate.Text;

            // Get reference of the ddlFacility & 
            DropDownList ddlFacility = PNReserveRoom.FindControl("ddlFacilityName") as DropDownList;
            DropDownList ddlFacilityQty = PNReserveRoom.FindControl("ddlFacilityQty") as DropDownList;

            if(ddlFacility.SelectedIndex != 0)
            {
                // Get facility availability from Session 
                List<AvailableFacility> availableFacility = (List<AvailableFacility>)Session["AvailableFacility"];

                for (int i = 0; i < availableFacility.Count; i++)
                {
                    if (availableFacility[i].facilityID == ddlFacility.SelectedValue)
                    {
                        for (int j = 1; j <= availableFacility[i].availableQty; j++)
                        {
                            ddlFacilityQty.Items.Add(new ListItem(j.ToString()));
                        }

                        if (availableFacility[i].priceType == "Per Reservation")
                        {
                            PNFacilityRentedDate.Visible = true;
                            CVFacilityRentedDate.Enabled = true;
                        }
                        else
                        {
                            PNFacilityRentedDate.Visible = false;
                            CVFacilityRentedDate.Enabled = false;
                        }
                    }
                }
            }
            else
            {
                ddlFacilityQty.Items.Clear();
                PNFacilityRentedDate.Visible = false;
                CVFacilityRentedDate.Enabled = false;
            }
            
        }

        protected void txtRentDate_TextChanged(object sender, EventArgs e)
        {
            // Run the comparevalidator
            CVFacilityRentedDate.Validate();

            // Check if rent date & return date is valid
            if (!CVFacilityRentedDate.IsValid)
            {

                // Style if error occurred
                txtRentDate.Style["border-bottom"] = "2px solid rgb(255, 0, 0)";
                txtRentDate.Style["background-color"] = "rgb(255, 240, 240)";

                txtReturnDate.Style["border-bottom"] = "2px solid rgb(255, 0, 0)";
                txtReturnDate.Style["background-color"] = "rgb(255, 240, 240)";

            }
            else
            {
                // Style if no error occurred
                txtRentDate.Style["border-bottom"] = "border-bottom: 2px solid rgb(128, 128, 128);";
                txtRentDate.Style["background-color"] = "none";

                txtReturnDate.Style["border-bottom"] = "border-bottom: 2px solid rgb(128, 128, 128);";
                txtReturnDate.Style["background-color"] = "none";
            }
        }

        protected void txtReturnDate_TextChanged(object sender, EventArgs e)
        {
            // Run the comparevalidator
            CVFacilityRentedDate.Validate();

            // Check if rent date & return date is valid
            if (!CVFacilityRentedDate.IsValid)
            {

                // Style if error occurred
                txtRentDate.Style["border-bottom"] = "2px solid rgb(255, 0, 0)";
                txtRentDate.Style["background-color"] = "rgb(255, 240, 240)";

                txtReturnDate.Style["border-bottom"] = "2px solid rgb(255, 0, 0)";
                txtReturnDate.Style["background-color"] = "rgb(255, 240, 240)";

            }
            else
            {
                // Style if no error occurred
                txtRentDate.Style["border-bottom"] = "border-bottom: 2px solid rgb(128, 128, 128);";
                txtRentDate.Style["background-color"] = "none";

                txtReturnDate.Style["border-bottom"] = "border-bottom: 2px solid rgb(128, 128, 128);";
                txtReturnDate.Style["background-color"] = "none";
            }
        }

        protected void btnAddFacility_Click(object sender, EventArgs e)
        {
            // Get refernce of RentedFacilityList
            List<RentedFacility> rentedFacility = (List<RentedFacility>)Session["RentedFacilityList"];

            // Get facility availability from Session 
            List<AvailableFacility> availableFacility = (List<AvailableFacility>)Session["AvailableFacility"];

            string temp = ddlFacilityName.SelectedValue;

            for (int i = 0; i < availableFacility.Count; i++)
            {
                if(availableFacility[i].facilityID == ddlFacilityName.SelectedValue)
                {
                    RentedFacility rf = new RentedFacility(ddlFacilityName.SelectedValue,
                                                        availableFacility[i].facilityName,
                                                        int.Parse(ddlFacilityQty.SelectedValue),
                                                        availableFacility[i].priceType,
                                                        availableFacility[i].price,
                                                        txtRentDate.Text,
                                                        txtReturnDate.Text, 
                                                        (availableFacility[i].price * int.Parse(ddlFacilityQty.SelectedValue))
                                                        );
                }
            }

            

            
        }

        
    }
}