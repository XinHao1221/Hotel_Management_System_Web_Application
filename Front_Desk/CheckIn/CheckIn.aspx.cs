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


namespace Hotel_Management_System.Front_Desk.CheckIn
{
    public partial class CheckIn : System.Web.UI.Page
    {
        // Create connection to database
        SqlConnection conn;
        String strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        // Create instance of ReservationUltility class
        ReservationUtility reservationUtility = new ReservationUtility();

        private String reservationID;

        protected void Page_Load(object sender, EventArgs e)
        {
            //reservationID = Request.QueryString["ID"];
            //reservationID = en.decryption(roomID);

            reservationID = "RS10000002";

            if (!IsPostBack)
            {
                Session["ReservationDetails"] = new ReservationDetail();

                Session["AvailableRoom"] = new List<AvailableRoom>();

                getReservationDetails();

                getReservedRoom();

                getRentedFacilityList();

                setStayDetails();

                displayRentedRoomAndFacility();

                getRooms();
            }
            


        }

        private void getReservationDetails()
        {
            // Get reference of ReservationDetail from view state
            ReservationDetail reservation = (ReservationDetail)Session["ReservationDetails"];

            conn = new SqlConnection(strCon);
            conn.Open();

            String getReservationDetails = "SELECT * FROM Reservation WHERE ReservationID LIKE @ID";

            SqlCommand cmdGetReservationDetails = new SqlCommand(getReservationDetails, conn);

            cmdGetReservationDetails.Parameters.AddWithValue("ID", reservationID);

            SqlDataReader sdr = cmdGetReservationDetails.ExecuteReader();

            // Set details into ReservationDetail object
            if (sdr.Read())
            {
                reservation.checkInDate = sdr.GetString(sdr.GetOrdinal("CheckInDate"));
                string temp = sdr.GetString(sdr.GetOrdinal("CheckInDate"));
                reservation.checkOutDate = sdr.GetString(sdr.GetOrdinal("CheckOutDate"));
                reservation.guestID = sdr.GetString(sdr.GetOrdinal("GuestID"));
            }

            conn.Close();
        }

        private void getReservedRoom()
        {
            // Get reference of ReservationDetail from view state
            ReservationDetail reservation = (ReservationDetail)Session["ReservationDetails"];

            List<ReservedRoom> reservedRooms = new List<ReservedRoom>();

            conn = new SqlConnection(strCon);
            conn.Open();

            string getReservedRoom = "SELECT * FROM ReservationRoom WHERE ReservationID LIKE @ID";

            SqlCommand cmdGetReservedRoom = new SqlCommand(getReservedRoom, conn);

            cmdGetReservedRoom.Parameters.AddWithValue("ID", reservationID);

            SqlDataReader sdr = cmdGetReservedRoom.ExecuteReader();

            ReservedRoom rr;

            while (sdr.Read())
            {
                if (sdr["ExtraBed"].ToString() == "True")
                {
                    rr = new ReservedRoom(sdr["ReservationRoomID"].ToString(), sdr["RoomTypeID"].ToString(), Convert.ToInt32(sdr["Adults"]), Convert.ToInt32(sdr["Kids"]), Convert.ToDouble(sdr["RoomPrice"]), Convert.ToDouble(sdr["ExtraBedCharges"]), sdr["Date"].ToString());
                }
                else
                {
                    rr = new ReservedRoom(sdr["ReservationRoomID"].ToString(), sdr["RoomTypeID"].ToString(), Convert.ToInt32(sdr["Adults"]), Convert.ToInt32(sdr["Kids"]), Convert.ToDouble(sdr["RoomPrice"]), -1, sdr["Date"].ToString());
                }

                reservedRooms.Add(rr);

            }
            conn.Close();

            reservation.reservedRoom = reservedRooms;
        }

        // Get reservation facility from database
        private List<ReservationFacility> getReservationFacility()
        {
            List<ReservationFacility> reservationFacility = new List<ReservationFacility>();

            conn = new SqlConnection(strCon);
            conn.Open();

            string getReservedRoom = "SELECT * FROM ReservationFacility WHERE ReservationID LIKE @ID";

            SqlCommand cmdGetReservedRoom = new SqlCommand(getReservedRoom, conn);

            cmdGetReservedRoom.Parameters.AddWithValue("@ID", reservationID);

            SqlDataReader sdr = cmdGetReservedRoom.ExecuteReader();

            ReservationFacility rf;

            while (sdr.Read())
            {
                rf = new ReservationFacility(sdr["ReservationFacilityID"].ToString(), sdr["FacilityID"].ToString(), Convert.ToDouble(sdr["Price"]), sdr["DateRented"].ToString(), Convert.ToInt32(sdr["Group"]));

                reservationFacility.Add(rf);
            }

            conn.Close();

            // Get one last record, contain nothing
            // To let program know, the list is ended
            rf = new ReservationFacility("", "", 0.00, "", 0);
            reservationFacility.Add(rf);

            return reservationFacility;
        }

        // Format reservation facility into a list
        private void getRentedFacilityList()
        {
            // A list of reservation facility from database
            List<ReservationFacility> temp = getReservationFacility();

            // Formated facility list to be displayed on the screen
            List<ReservationFacility> reservationFacilities = new List<ReservationFacility>();

            // Get reference of ReservationDetail from view state
            ReservationDetail reservation = (ReservationDetail)Session["ReservationDetails"];

            string rentDate, facilityID, returnDate;
            int qty = 0, group;
            int counter;

            // Check if it contains at least one element
            // Got one default element
            // Therefore start with 1
            if (temp.Count > 1)
            {
                int i = 0;

                while ((i < temp.Count) && (temp[i].group != 0))
                {

                    int test = temp[i].group;

                    counter = 0;

                    rentDate = temp[i].rentDate;
                    returnDate = temp[i].rentDate;

                    group = temp[i].group;

                    qty = 1;

                    if ((i < temp.Count - 1))
                    {
                        if (temp[i + 1].group == group)
                        {
                            do
                            {
                                if (temp[i + 1].rentDate == rentDate)
                                {
                                    qty++;
                                    i++;

                                }
                                else if ((temp[i + 1].group == group))
                                {
                                    returnDate = temp[i + 1].rentDate;
                                    i++;
                                }

                            } while ((temp[i + 1].group == group));


                        }
                    }

                    // Add one into return date
                    returnDate = reservationUtility.getNextDate(returnDate);

                    ReservationFacility rf = new ReservationFacility(temp[i].reservationFacilityID, temp[i].facilityID,
                                                                            qty, temp[i].price, rentDate, returnDate);

                    reservationFacilities.Add(rf);

                    i++;

                }


            }

            reservation.rentedFacility = reservationFacilities;
        }

        private void setStayDetails()
        {
            // Get reference of ReservationDetail from view state
            ReservationDetail reservation = (ReservationDetail)Session["ReservationDetails"];

            setGuestInformation(reservation.guestID);

            // Format date base on date format on user's computer
            DateTime formatedCheckInDate = Convert.ToDateTime(reservation.checkInDate);
            DateTime formatedCheckOutDate = Convert.ToDateTime(reservation.checkOutDate);

            lblCheckIn.Text = formatedCheckInDate.ToShortDateString();
            lblCheckOut.Text = formatedCheckOutDate.ToShortDateString();

            string temp = formatedCheckOutDate.ToShortDateString();

            int durationOfStay = reservationUtility.getdurationOfStay(lblCheckIn.Text, lblCheckOut.Text);

            lblDurationOfStay.Text = durationOfStay.ToString() + " Night";
        }

        private void setGuestInformation(string guestID)
        {

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

        private void displayRentedRoomAndFacility()
        {
            // Get reference of ReservationDetail from view state
            ReservationDetail reservation = (ReservationDetail)Session["ReservationDetails"];


            RepeaterReservedRoom.DataSource = reservation.reservedRoom;
            RepeaterReservedRoom.DataBind();

            RepeaterRentedFacility.DataSource = reservation.rentedFacility;
            RepeaterRentedFacility.DataBind();


        }

        public void getRooms()
        {
            // Get reference of ReservationDetail from view state
            ReservationDetail reservation = (ReservationDetail)Session["ReservationDetails"];

            List<ReservedRoom> reservedRooms = reservation.reservedRoom;

            List<AvailableRoom> availableRooms = (List<AvailableRoom>)Session["AvailableRoom"];

            for (int i = 0; i < reservedRooms.Count; i++)
            {
                getAvailableRooms(reservedRooms[i].roomTypeID, reservedRooms[i].date);
            }
        }

        private void getAvailableRooms(string roomTypeID, string date)
        {
            List<AvailableRoom> availableRooms = (List<AvailableRoom>)Session["AvailableRoom"];

            conn = new SqlConnection(strCon);
            conn.Open();

            String getAvailableRoom = "(SELECT RoomID FROM Room WHERE RoomTypeID LIKE @RoomTypeID AND Status LIKE 'Active') " +
                                        "EXCEPT " +
                                        "(SELECT RoomID FROM ReservationRoom WHERE RoomTypeID LIKE @RoomTypeID AND Date LIKE @Date)";

            SqlCommand cmdGetAvailableRoom = new SqlCommand(getAvailableRoom, conn);

            cmdGetAvailableRoom.Parameters.AddWithValue("@RoomTypeID", roomTypeID);
            cmdGetAvailableRoom.Parameters.AddWithValue("@Date", date);

            SqlDataReader sdr = cmdGetAvailableRoom.ExecuteReader();

            AvailableRoom ar = null;

            while (sdr.Read())
            {
                ar = new AvailableRoom(sdr["RoomID"].ToString(), roomTypeID, date);

                availableRooms.Add(ar);
            }

            conn.Close();

        }

        protected void LBBack_Click(object sender, EventArgs e)
        {

        }

        protected void IBDeleteRentedFacility_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void RepeaterReservedRoom_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Get control's refernce
            Label lblDate = e.Item.FindControl("lblDate") as Label;

            // Format date base on date format on user's computer
            DateTime formatedDate = Convert.ToDateTime(lblDate.Text);

            // Display the formated date
            lblDate.Text = formatedDate.ToShortDateString();
        }

        protected void IBClosePopUpBox_Click(object sender, ImageClickEventArgs e)
        {
            PopupBoxSelectRoom.Visible = false;
            PopupCover.Visible = false;
        }

        protected void LBSelectRoom_Click(object sender, EventArgs e)
        {
            PopupBoxSelectRoom.Visible = true;
            PopupCover.Visible = true;
        }
    }
}