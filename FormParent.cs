using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using NutechLibrary;

namespace JakartaFair_VM
{
    public partial class FormParent : MetroFramework.Forms.MetroForm
    {

        int ScreenWidth, ScreenHeight;
        string ReturnStatusDevice, returnMessage;
        Thread t;
        FormSplashing frm;
        public static NutechLib _Devices;
        public static BillTray[] _BillTray;
        public static CoinTrays _CoinTrays;

        public FormParent()
        {
           

           

            InitializeComponent();
        }

        private void FormParent_Load(object sender, EventArgs e)
        {

            t = new Thread(new ThreadStart(Loading));
            t.Start();
            Screen screen = Screen.PrimaryScreen;
            ScreenWidth = screen.Bounds.Width;
            ScreenHeight = screen.Bounds.Height;


            _Devices = new NutechLib();
            returnMessage = DeviceConfig(_Devices);
            t.Abort();
            if (returnMessage!= "OK")
            {
                NotifMessage(returnMessage);
            }

            else
            {
                MessageBox.Show("LOGIN");
            }
            

        }

        

        private string DeviceConfig(NutechLib Devices)
        {
            try
            {
                //-----------------------------------------Inisialiasi Coin Dispenser------------------------
                ReturnStatusDevice = "Coin Dispenser Not Connected";
                Devices.CoinDevices = new CoinDevices[2];
                Devices.CoinDevices[0] = new CoinDevices();
                Devices.CoinDevices[0].PortName = "COM15";
                Devices.CoinDevices[0].DeviceID = 1;
                Devices.CoinDispenserInitialize(Devices.CoinDevices[0]);
                _CoinTrays = Devices.GetCoinDispenserTraysInfo(Devices.CoinDevices[0]);
                Devices.SetCoinDispenserAllLevelToZero(Devices.CoinDevices[0]);
                ReturnStatusDevice = "";

                //----------------------------------------Inisialiasi Bill Acceptor-------------------------
                ReturnStatusDevice = "Bill Acceptor Not Connected";
                Devices.SetBillAcceptorPort("COM5");
                Devices.BillAcceptorInitialize();
                ReturnStatusDevice = "";

                //-----------------------------------------Inisialasi Cash Dispenser------------------------
                ReturnStatusDevice = "Cash Dispenser Not Connected";
                _Devices.SetBillDispenserPort("COM4");  
                Devices.CashDispenserInitialize();
                _BillTray = Devices.GetCashDispenserTraysInfo();
                

                if (_BillTray.Length != 2)
                {
                    return "Error Cash Dispenser!\r\nThere is an Empty Tray, Please Enter The Tray Correctly!";
                }

                if (_BillTray[0].value != BillValues.IDR2000)
                {
                    return "Error Cash Dispenser!\r\nTray 1 Wrong Position\r\n\r\nInfo : Tray 2 Must Be IDR2000\r\nPlease Enter The Tray Correctly!";
                }

                if (_BillTray[1].value != BillValues.IDR5000)
                {
                    return "Error Cash Dispenser!\r\nTray 2 Wrong Position \r\nInfo : Tray 2 Must Be IDR5000\r\nPlease Enter The Tray Correctly!";
                }
                ReturnStatusDevice = "";
                
                return "OK";
            }

            catch (Exception ex)
            {
                return ReturnStatusDevice + "\r\n" + ex.ToString();
            }
        }

        private void FormParent_FormClosing(object sender, FormClosingEventArgs e)
        {
            _Devices.CoinDispenserUninitialize(_Devices.CoinDevices[0]);

            if (_Devices.IsBillAcceptorInitialized)
            {
                _Devices.BillAcceptorUninitialize();
            }

            if (_Devices.IsCashDispenserInitialized)
            {
                _Devices.CashDispenserUninitialize();
            }
        }

        private void Loading()
        {
            frm = new FormSplashing();
            Application.Run(frm);
 
        }
        private void NotifMessage(string message)
        {

            MetroFramework.Controls.MetroLabel lblStatusDevice = new MetroFramework.Controls.MetroLabel();
            lblStatusDevice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            lblStatusDevice.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            lblStatusDevice.Location = new System.Drawing.Point(0, 121);
            lblStatusDevice.Name = "lblStatusDevice";
            lblStatusDevice.AutoSize = true;
            lblStatusDevice.MinimumSize = new Size(1920, 0);
            lblStatusDevice.TabIndex = 3;
            lblStatusDevice.Text = message;
            lblStatusDevice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblStatusDevice.UseCustomBackColor = true;
            lblStatusDevice.UseCustomForeColor = true;

            this.Controls.Add(lblStatusDevice);
        }
    }
}
