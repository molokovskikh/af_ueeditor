using System;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using System.Reflection;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System.Threading;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Diagnostics;



[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum, ViewAndModify = "HKEY_CURRENT_USER")]
namespace UEEditor
{

	[FlagsAttribute]
	public enum FormMask : byte
	{
		NameForm = 1,
		FirmForm = 2,
		CurrForm = 4,
		MarkForb = 8
	}
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmUEEMain : System.Windows.Forms.Form
	{
		private System.Data.DataSet dataSet1;
		private System.Data.DataTable dtJobs;
		private System.Data.DataTable dtForm;
		private System.Data.DataTable dtZero;
		private System.Data.DataTable dtForb;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.TabControl tcMain;
		private System.Windows.Forms.TabPage tpJobs;
		private System.Windows.Forms.TabPage tpUnrecExp;
		private System.Windows.Forms.Panel pnlTop2;
		private System.Windows.Forms.Label BigNameLabel2;
		private System.Windows.Forms.TabPage tpZero;
		private System.Windows.Forms.TabPage tpForb;
		private System.Data.DataColumn JName;
		private System.Data.DataColumn JRegion;
		private System.Data.DataColumn JPos;
		private System.Data.DataColumn JNamePos;
		private System.Data.DataColumn JJobDate;
		private System.Data.DataColumn JBlockBy;
		private System.Data.DataColumn UECode;
		private System.Data.DataColumn UECodeCr;
		private System.Data.DataColumn UEName1;
		private System.Data.DataColumn UEFirmCr;
		private System.Data.DataColumn UECurrency;
		private System.Data.DataColumn UEBaseCost;
		private System.Data.DataColumn UEUnit;
		private System.Data.DataColumn UEVolume;
		private System.Data.DataColumn UEQuantity;
		private System.Data.DataColumn UEPeriod;
		private System.Data.DataColumn UEJunk;
		private System.Data.DataColumn ZCode;
		private System.Data.DataColumn ZCodeCr;
		private System.Data.DataColumn ZName;
		private System.Data.DataColumn ZFirmCr;
		private System.Data.DataColumn ZCurrency;
		private System.Data.DataColumn ZBaseCost;
		private System.Data.DataColumn ZUnit;
		private System.Data.DataColumn ZVolume;
		private System.Data.DataColumn ZQuantity;
		private System.Data.DataColumn ZPeriod;
		private System.Data.DataColumn ZJunk;
		private System.Data.DataColumn FForb;
		private System.Data.DataColumn EUColumn1;
		private System.Data.DataColumn UEColumn2;
		private System.Data.DataColumn UEColumn3;
		private System.Data.DataTable dtUnrecExp;
		private System.ComponentModel.IContainer components;

        public string PriceFMT = String.Empty;
        public string FileExt = String.Empty;
		public long LockedPriceCode = -1;
		public long LockedSynonym = -1;
		public frmProgress f = null;
		public int SynonymCount = 0;
		public int HideSynonymCount = 0;
		public int DuplicateSynonymCount = 0;
		public int SynonymFirmCrCount = 0;
		public int SynonymCurrencyCount = 0;
		public int ForbiddenCount = 0;
	
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private DevExpress.XtraGrid.GridControl ForbGridControl;
		private DevExpress.XtraGrid.Columns.GridColumn colFForb;
		private DevExpress.XtraGrid.GridControl ZeroGridControl;
		private DevExpress.XtraGrid.Columns.GridColumn colZCode;
		private DevExpress.XtraGrid.Columns.GridColumn colZCodeCr;
		private DevExpress.XtraGrid.Columns.GridColumn colZName;
		private DevExpress.XtraGrid.Columns.GridColumn colZFirmCr;
		private DevExpress.XtraGrid.Columns.GridColumn colZCurrency;
		private DevExpress.XtraGrid.Columns.GridColumn colZUnit;
		private DevExpress.XtraGrid.Columns.GridColumn colZVolume;
		private DevExpress.XtraGrid.Columns.GridColumn colZQuantity;
		private DevExpress.XtraGrid.Columns.GridColumn colZPeriod;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn10;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn11;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn12;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn13;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn14;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn15;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn16;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn26;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn27;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn28;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn29;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn30;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn31;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn32;
		private DevExpress.XtraGrid.Columns.GridColumn colCColumn11;
		private System.Windows.Forms.Panel pnlCenter2;
		private System.Windows.Forms.Panel pnlLeft2;
		private System.Windows.Forms.GroupBox grpBoxCatalog2;
		private DevExpress.XtraGrid.GridControl CatalogGridControl;
		private System.Windows.Forms.ImageList imageList1;
		private System.Data.DataColumn JMinReq;
		private System.Data.DataTable dtRegions;
		private System.Data.DataColumn RRegion;
		private System.Data.DataColumn JWholeSale;
		private System.Data.DataColumn JPriceCode;
		private System.Windows.Forms.Panel pnlBottom2;
		private DevExpress.XtraGrid.GridControl UnrecExpGridControl;
		private DevExpress.XtraGrid.Columns.GridColumn colUEColumn1;
		private DevExpress.XtraGrid.Columns.GridColumn colUEColumn2;
		private DevExpress.XtraGrid.Columns.GridColumn colUEColumn3;
		private DevExpress.XtraGrid.Columns.GridColumn colUECode;
		private DevExpress.XtraGrid.Columns.GridColumn colUECodeCr;
		private DevExpress.XtraGrid.Columns.GridColumn colUEName1;
		private DevExpress.XtraGrid.Columns.GridColumn colUEFirmCr;
		private DevExpress.XtraGrid.Columns.GridColumn colUECurrency;
		private DevExpress.XtraGrid.Columns.GridColumn colUEBaseCost;
		private DevExpress.XtraGrid.Columns.GridColumn colUEUnit;
		private DevExpress.XtraGrid.Columns.GridColumn colUEVolume;
		private DevExpress.XtraGrid.Columns.GridColumn colUEQuantity;
		private DevExpress.XtraGrid.Columns.GridColumn colUEPeriod;
		private DevExpress.XtraGrid.Columns.GridColumn colUEJunk;
		private System.Data.DataColumn UEStatus;
		private System.Data.DataColumn UETmpFullCode;
		private System.Data.DataTable dtCatalogName;
		private System.Data.DataTable dtCatalogFirmCr;
		private System.Windows.Forms.Timer MainTimer;

		private	MySqlConnection MyCn = new MySqlConnection("server=sql.analit.net; user id=system; password=123; database=farm;");
		private MySqlCommand MyCmd = new MySqlCommand();
		private DevExpress.XtraGrid.Views.Grid.GridView gvUnrecExp;
		private DevExpress.XtraGrid.Views.Grid.GridView gvCatalog;
		private DevExpress.XtraGrid.Views.Grid.GridView gvForb;
		private DevExpress.XtraGrid.Views.Grid.GridView gvZero;
		private System.Data.DataColumn CCodeFirmCr;
		private System.Data.DataColumn CShortCode;
		private System.Data.DataColumn FShortCode;
		private System.Data.DataColumn CName;
		private System.Data.DataColumn CFirmCr;
		private System.Data.DataColumn FFullCode;
		private System.Data.DataColumn FForm;
		private DevExpress.XtraGrid.Views.Grid.GridView gvCatForm;
		private DevExpress.XtraGrid.Columns.GridColumn colCName;
		private DevExpress.XtraGrid.Columns.GridColumn colFForm;
		private MySqlDataAdapter MyDA = new MySqlDataAdapter();
		private string BaseRegKey = "Software\\Inforoom\\UEEditor";
		private string JregKey;
		private string CregKey;
		private string UEregKey;
		private string FregKey;
		private string ZregKey;
		private System.Data.DataColumn UEName2;
		private System.Data.DataColumn UEName3;
		private DevExpress.XtraGrid.Columns.GridColumn colUEStatus;
		private System.Data.DataColumn UETmpShortCode;
		private System.Data.DataColumn UETmpCodeFirmCr;
		private System.Data.DataColumn UEAlready;
		private DevExpress.XtraGrid.Columns.GridColumn colUEAlready;
		private System.Data.DataColumn JParentSynonym;
		private System.Data.DataColumn JPriceFMT;
		private System.Data.DataColumn JPhone;
		private System.Data.DataColumn UETmpCurrency;
		private System.Data.DataTable dtCurrency;
		private System.Data.DataColumn CCurrency;
		private System.Data.DataColumn CExchange;
		private System.Windows.Forms.Panel pnlCenter1;
		private System.Windows.Forms.Panel pnlWithButton1;
		private System.Windows.Forms.Panel pnlTop1;
		private System.Windows.Forms.Button btnDelJob;
		private System.Data.DataTable dtSections;
		private System.Data.DataColumn SSection;
		private System.Data.DataColumn UERowID;
		private System.Windows.Forms.ContextMenu cmUnrecExp;
		private System.Windows.Forms.MenuItem miSendAboutNames;
		private System.Windows.Forms.MenuItem miSendAboutFirmCr;
		private System.Windows.Forms.ImageList imageList2;
		private System.Windows.Forms.ColorDialog cdLegend;
		private System.Windows.Forms.GroupBox groupBox1;
		private DevExpress.XtraGrid.GridControl JobsGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gvJobs;
		private DevExpress.XtraGrid.Columns.GridColumn colJName;
		private DevExpress.XtraGrid.Columns.GridColumn colJBlockBy;
		private DevExpress.XtraGrid.Columns.GridColumn colJRegion;
		private DevExpress.XtraGrid.Columns.GridColumn colJPos;
		private DevExpress.XtraGrid.Columns.GridColumn colJNamePos;
		private DevExpress.XtraGrid.Columns.GridColumn colJJobDate;
		private DevExpress.XtraGrid.Columns.GridColumn colJWholeSale;
		private System.Windows.Forms.Label lbJobs50Text;
		private System.Windows.Forms.Label lbJobsNamePosText;
		private System.Windows.Forms.Label lbJobsBlockText;
		private System.Data.DataColumn UEHandMade;
		private System.Windows.Forms.Button btnJobsBlock;
		private System.Windows.Forms.Button btnJobsNamePos;
		private System.Windows.Forms.Button btnJobs50;
		private System.Windows.Forms.StatusBarPanel sbpAll;
		private System.Windows.Forms.StatusBarPanel sbpCurrent;
        private DataColumn JNeedRetrans;
        private DataColumn JRetranced;
        private GridColumn colJRetranced;
        private GridColumn colJNeedRetrans;
        private DataColumn JParentName;
        private GridColumn colJParentName;
        private DataColumn JExt;
		private DataColumn JFirmCode;
		private MySqlDataAdapter daJobs;

		public frmUEEMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			try
			{
				LoadColor(btnJobsBlock, btnJobsBlock.BackColor.ToArgb());
				LoadColor(btnJobsNamePos, btnJobsNamePos.BackColor.ToArgb());
				LoadColor(btnJobs50, btnJobs50.BackColor.ToArgb());

				JregKey = BaseRegKey + "\\J";
				CregKey = BaseRegKey + "\\C";
				UEregKey = BaseRegKey + "\\UE";
				FregKey = BaseRegKey + "\\F";
				ZregKey = BaseRegKey + "\\Z";
				JobsGridControl.MainView.RestoreLayoutFromRegistry(JregKey);
				CatalogGridControl.MainView.RestoreLayoutFromRegistry(CregKey);
				UnrecExpGridControl.MainView.RestoreLayoutFromRegistry(UEregKey);
				ForbGridControl.MainView.RestoreLayoutFromRegistry(FregKey);
				ZeroGridControl.MainView.RestoreLayoutFromRegistry(ZregKey);
			}
			catch
			{
			}

#if DEBUG
			MyCn.ConnectionString = "server=testsql.analit.net; user id=system; password=123; database=farm;";
#endif
			MyCn.Open();
			MyDA = new MySqlDataAdapter(MyCmd);
			MyCmd.Connection = MyCn;

			tcMain.TabPages.Remove(tpUnrecExp);
			tcMain.TabPages.Remove(tpZero);
			tcMain.TabPages.Remove(tpForb);

			//Создали ДатаАдаптер для таблицы заданий
			DAJobsCreate();

			//Заполняем таблицу заданий
			JobsGridFill();

			//Запоняем каталожные таблицы
			dtForm.Clear();

			CatalogFirmCrGridFill(MyCmd, MyDA);

			CatalogNameGridFill(MyCmd, MyDA);

			FormGridFill(MyCmd, MyDA);

			//Заполняем таблицу валют
			CurrencyGridFill(MyCmd, MyDA);

			//
			JobsGridControl.Select();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUEEMain));
			this.gvCatForm = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colFForm = new DevExpress.XtraGrid.Columns.GridColumn();
			this.CatalogGridControl = new DevExpress.XtraGrid.GridControl();
			this.dataSet1 = new System.Data.DataSet();
			this.dtJobs = new System.Data.DataTable();
			this.JPriceCode = new System.Data.DataColumn();
			this.JName = new System.Data.DataColumn();
			this.JRegion = new System.Data.DataColumn();
			this.JPos = new System.Data.DataColumn();
			this.JNamePos = new System.Data.DataColumn();
			this.JJobDate = new System.Data.DataColumn();
			this.JBlockBy = new System.Data.DataColumn();
			this.JPhone = new System.Data.DataColumn();
			this.JMinReq = new System.Data.DataColumn();
			this.JWholeSale = new System.Data.DataColumn();
			this.JParentSynonym = new System.Data.DataColumn();
			this.JPriceFMT = new System.Data.DataColumn();
			this.JNeedRetrans = new System.Data.DataColumn();
			this.JRetranced = new System.Data.DataColumn();
			this.JParentName = new System.Data.DataColumn();
			this.JExt = new System.Data.DataColumn();
			this.JFirmCode = new System.Data.DataColumn();
			this.dtUnrecExp = new System.Data.DataTable();
			this.EUColumn1 = new System.Data.DataColumn();
			this.UEColumn2 = new System.Data.DataColumn();
			this.UEColumn3 = new System.Data.DataColumn();
			this.UECode = new System.Data.DataColumn();
			this.UECodeCr = new System.Data.DataColumn();
			this.UEName1 = new System.Data.DataColumn();
			this.UEFirmCr = new System.Data.DataColumn();
			this.UECurrency = new System.Data.DataColumn();
			this.UEBaseCost = new System.Data.DataColumn();
			this.UEUnit = new System.Data.DataColumn();
			this.UEVolume = new System.Data.DataColumn();
			this.UEQuantity = new System.Data.DataColumn();
			this.UEPeriod = new System.Data.DataColumn();
			this.UEJunk = new System.Data.DataColumn();
			this.UEStatus = new System.Data.DataColumn();
			this.UETmpFullCode = new System.Data.DataColumn();
			this.UEName2 = new System.Data.DataColumn();
			this.UEName3 = new System.Data.DataColumn();
			this.UETmpShortCode = new System.Data.DataColumn();
			this.UETmpCodeFirmCr = new System.Data.DataColumn();
			this.UEAlready = new System.Data.DataColumn();
			this.UETmpCurrency = new System.Data.DataColumn();
			this.UERowID = new System.Data.DataColumn();
			this.UEHandMade = new System.Data.DataColumn();
			this.dtCatalogName = new System.Data.DataTable();
			this.CShortCode = new System.Data.DataColumn();
			this.CName = new System.Data.DataColumn();
			this.dtForm = new System.Data.DataTable();
			this.FShortCode = new System.Data.DataColumn();
			this.FFullCode = new System.Data.DataColumn();
			this.FForm = new System.Data.DataColumn();
			this.dtZero = new System.Data.DataTable();
			this.ZCode = new System.Data.DataColumn();
			this.ZCodeCr = new System.Data.DataColumn();
			this.ZName = new System.Data.DataColumn();
			this.ZFirmCr = new System.Data.DataColumn();
			this.ZCurrency = new System.Data.DataColumn();
			this.ZBaseCost = new System.Data.DataColumn();
			this.ZUnit = new System.Data.DataColumn();
			this.ZVolume = new System.Data.DataColumn();
			this.ZQuantity = new System.Data.DataColumn();
			this.ZPeriod = new System.Data.DataColumn();
			this.ZJunk = new System.Data.DataColumn();
			this.dtForb = new System.Data.DataTable();
			this.FForb = new System.Data.DataColumn();
			this.dtRegions = new System.Data.DataTable();
			this.RRegion = new System.Data.DataColumn();
			this.dtCatalogFirmCr = new System.Data.DataTable();
			this.CCodeFirmCr = new System.Data.DataColumn();
			this.CFirmCr = new System.Data.DataColumn();
			this.dtCurrency = new System.Data.DataTable();
			this.CCurrency = new System.Data.DataColumn();
			this.CExchange = new System.Data.DataColumn();
			this.dtSections = new System.Data.DataTable();
			this.SSection = new System.Data.DataColumn();
			this.gvCatalog = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colCName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.sbpAll = new System.Windows.Forms.StatusBarPanel();
			this.sbpCurrent = new System.Windows.Forms.StatusBarPanel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.tcMain = new System.Windows.Forms.TabControl();
			this.tpJobs = new System.Windows.Forms.TabPage();
			this.pnlCenter1 = new System.Windows.Forms.Panel();
			this.pnlTop1 = new System.Windows.Forms.Panel();
			this.JobsGridControl = new DevExpress.XtraGrid.GridControl();
			this.gvJobs = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colJName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJParentName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJWholeSale = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJRegion = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJNeedRetrans = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJRetranced = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJPos = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJNamePos = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJJobDate = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colJBlockBy = new DevExpress.XtraGrid.Columns.GridColumn();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnJobs50 = new System.Windows.Forms.Button();
			this.btnJobsNamePos = new System.Windows.Forms.Button();
			this.btnJobsBlock = new System.Windows.Forms.Button();
			this.lbJobs50Text = new System.Windows.Forms.Label();
			this.lbJobsNamePosText = new System.Windows.Forms.Label();
			this.lbJobsBlockText = new System.Windows.Forms.Label();
			this.pnlWithButton1 = new System.Windows.Forms.Panel();
			this.btnDelJob = new System.Windows.Forms.Button();
			this.imageList2 = new System.Windows.Forms.ImageList(this.components);
			this.tpUnrecExp = new System.Windows.Forms.TabPage();
			this.cmUnrecExp = new System.Windows.Forms.ContextMenu();
			this.miSendAboutNames = new System.Windows.Forms.MenuItem();
			this.miSendAboutFirmCr = new System.Windows.Forms.MenuItem();
			this.pnlBottom2 = new System.Windows.Forms.Panel();
			this.UnrecExpGridControl = new DevExpress.XtraGrid.GridControl();
			this.gvUnrecExp = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colUEColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUECode = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUECodeCr = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEName1 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEFirmCr = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUECurrency = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEBaseCost = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEUnit = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEVolume = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEQuantity = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEPeriod = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEJunk = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEStatus = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colUEAlready = new DevExpress.XtraGrid.Columns.GridColumn();
			this.pnlCenter2 = new System.Windows.Forms.Panel();
			this.pnlLeft2 = new System.Windows.Forms.Panel();
			this.grpBoxCatalog2 = new System.Windows.Forms.GroupBox();
			this.pnlTop2 = new System.Windows.Forms.Panel();
			this.BigNameLabel2 = new System.Windows.Forms.Label();
			this.tpZero = new System.Windows.Forms.TabPage();
			this.ZeroGridControl = new DevExpress.XtraGrid.GridControl();
			this.gvZero = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colZCode = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZCodeCr = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZFirmCr = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZCurrency = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZUnit = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZVolume = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZQuantity = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colZPeriod = new DevExpress.XtraGrid.Columns.GridColumn();
			this.tpForb = new System.Windows.Forms.TabPage();
			this.ForbGridControl = new DevExpress.XtraGrid.GridControl();
			this.gvForb = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colFForb = new DevExpress.XtraGrid.Columns.GridColumn();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.dataGridTextBoxColumn10 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn11 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn12 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn13 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn14 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn15 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn16 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn26 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn27 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn28 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn29 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn30 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn31 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn32 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.colCColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.MainTimer = new System.Windows.Forms.Timer(this.components);
			this.cdLegend = new System.Windows.Forms.ColorDialog();
			((System.ComponentModel.ISupportInitialize)(this.gvCatForm)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.CatalogGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtJobs)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtUnrecExp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCatalogName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtForm)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtZero)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtForb)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtRegions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCatalogFirmCr)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCurrency)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dtSections)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvCatalog)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpAll)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpCurrent)).BeginInit();
			this.panel3.SuspendLayout();
			this.tcMain.SuspendLayout();
			this.tpJobs.SuspendLayout();
			this.pnlCenter1.SuspendLayout();
			this.pnlTop1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.JobsGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvJobs)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.pnlWithButton1.SuspendLayout();
			this.tpUnrecExp.SuspendLayout();
			this.pnlBottom2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.UnrecExpGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvUnrecExp)).BeginInit();
			this.pnlCenter2.SuspendLayout();
			this.pnlLeft2.SuspendLayout();
			this.grpBoxCatalog2.SuspendLayout();
			this.pnlTop2.SuspendLayout();
			this.tpZero.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ZeroGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvZero)).BeginInit();
			this.tpForb.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ForbGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvForb)).BeginInit();
			this.SuspendLayout();
			// 
			// gvCatForm
			// 
			this.gvCatForm.Appearance.HideSelectionRow.Options.UseBackColor = true;
			this.gvCatForm.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colFForm});
			this.gvCatForm.GridControl = this.CatalogGridControl;
			this.gvCatForm.Name = "gvCatForm";
			this.gvCatForm.OptionsBehavior.AllowIncrementalSearch = true;
			this.gvCatForm.OptionsBehavior.Editable = false;
			this.gvCatForm.OptionsDetail.EnableMasterViewMode = false;
			this.gvCatForm.OptionsFilter.AllowColumnMRUFilterList = false;
			this.gvCatForm.OptionsFilter.AllowMRUFilterList = false;
			this.gvCatForm.OptionsMenu.EnableColumnMenu = false;
			this.gvCatForm.OptionsMenu.EnableFooterMenu = false;
			this.gvCatForm.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvCatForm.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gvCatForm.OptionsView.ShowFilterPanel = false;
			this.gvCatForm.OptionsView.ShowGroupPanel = false;
			// 
			// colFForm
			// 
			this.colFForm.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.colFForm.AppearanceCell.Options.UseFont = true;
			this.colFForm.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.colFForm.AppearanceHeader.Options.UseFont = true;
			this.colFForm.Caption = "Форма выпуска";
			this.colFForm.FieldName = "FForm";
			this.colFForm.Name = "colFForm";
			this.colFForm.OptionsColumn.ReadOnly = true;
			this.colFForm.Visible = true;
			this.colFForm.VisibleIndex = 0;
			// 
			// CatalogGridControl
			// 
			this.CatalogGridControl.DataMember = "CatalogNameGrid";
			this.CatalogGridControl.DataSource = this.dataSet1;
			this.CatalogGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.CatalogGridControl.EmbeddedNavigator.Name = "";
			this.CatalogGridControl.Enabled = false;
			gridLevelNode1.LevelTemplate = this.gvCatForm;
			gridLevelNode1.RelationName = "Relation1";
			this.CatalogGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
			this.CatalogGridControl.Location = new System.Drawing.Point(3, 16);
			this.CatalogGridControl.MainView = this.gvCatalog;
			this.CatalogGridControl.Name = "CatalogGridControl";
			this.CatalogGridControl.Size = new System.Drawing.Size(706, 237);
			this.CatalogGridControl.TabIndex = 0;
			this.CatalogGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvCatalog,
            this.gvCatForm});
			this.CatalogGridControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CatalogGridControl_KeyDown);
			// 
			// dataSet1
			// 
			this.dataSet1.DataSetName = "NewDataSet";
			this.dataSet1.Locale = new System.Globalization.CultureInfo("ru-RU");
			this.dataSet1.Relations.AddRange(new System.Data.DataRelation[] {
            new System.Data.DataRelation("Relation1", "CatalogNameGrid", "FormGrid", new string[] {
                        "CCode"}, new string[] {
                        "FShortCode"}, false)});
			this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dtJobs,
            this.dtUnrecExp,
            this.dtCatalogName,
            this.dtForm,
            this.dtZero,
            this.dtForb,
            this.dtRegions,
            this.dtCatalogFirmCr,
            this.dtCurrency,
            this.dtSections});
			// 
			// dtJobs
			// 
			this.dtJobs.Columns.AddRange(new System.Data.DataColumn[] {
            this.JPriceCode,
            this.JName,
            this.JRegion,
            this.JPos,
            this.JNamePos,
            this.JJobDate,
            this.JBlockBy,
            this.JPhone,
            this.JMinReq,
            this.JWholeSale,
            this.JParentSynonym,
            this.JPriceFMT,
            this.JNeedRetrans,
            this.JRetranced,
            this.JParentName,
            this.JExt,
            this.JFirmCode});
			this.dtJobs.TableName = "JobsGrid";
			// 
			// JPriceCode
			// 
			this.JPriceCode.Caption = "Код прайса";
			this.JPriceCode.ColumnName = "JPriceCode";
			this.JPriceCode.DataType = typeof(long);
			this.JPriceCode.Namespace = "";
			// 
			// JName
			// 
			this.JName.Caption = "Фирмы (прайс-лист)";
			this.JName.ColumnName = "JName";
			// 
			// JRegion
			// 
			this.JRegion.Caption = "Регион";
			this.JRegion.ColumnName = "JRegion";
			// 
			// JPos
			// 
			this.JPos.Caption = "Всего";
			this.JPos.ColumnName = "JPos";
			this.JPos.DataType = typeof(long);
			// 
			// JNamePos
			// 
			this.JNamePos.Caption = "Строго";
			this.JNamePos.ColumnName = "JNamePos";
			this.JNamePos.DataType = typeof(long);
			// 
			// JJobDate
			// 
			this.JJobDate.Caption = "Дата задания";
			this.JJobDate.ColumnName = "JJobDate";
			// 
			// JBlockBy
			// 
			this.JBlockBy.Caption = "Блокировано";
			this.JBlockBy.ColumnName = "JBlockBy";
			// 
			// JPhone
			// 
			this.JPhone.ColumnName = "JPhone";
			// 
			// JMinReq
			// 
			this.JMinReq.ColumnName = "JMinReq";
			// 
			// JWholeSale
			// 
			this.JWholeSale.ColumnName = "JWholeSale";
			this.JWholeSale.DataType = typeof(long);
			this.JWholeSale.ReadOnly = true;
			// 
			// JParentSynonym
			// 
			this.JParentSynonym.ColumnName = "JParentSynonym";
			this.JParentSynonym.DataType = typeof(long);
			// 
			// JPriceFMT
			// 
			this.JPriceFMT.ColumnName = "JPriceFMT";
			// 
			// JNeedRetrans
			// 
			this.JNeedRetrans.ColumnName = "JNeedRetrans";
			this.JNeedRetrans.DataType = typeof(int);
			// 
			// JRetranced
			// 
			this.JRetranced.ColumnName = "JRetranced";
			this.JRetranced.DataType = typeof(int);
			// 
			// JParentName
			// 
			this.JParentName.ColumnName = "JParentName";
			// 
			// JExt
			// 
			this.JExt.ColumnName = "JExt";
			// 
			// JFirmCode
			// 
			this.JFirmCode.ColumnName = "JFirmCode";
			this.JFirmCode.DataType = typeof(long);
			// 
			// dtUnrecExp
			// 
			this.dtUnrecExp.Columns.AddRange(new System.Data.DataColumn[] {
            this.EUColumn1,
            this.UEColumn2,
            this.UEColumn3,
            this.UECode,
            this.UECodeCr,
            this.UEName1,
            this.UEFirmCr,
            this.UECurrency,
            this.UEBaseCost,
            this.UEUnit,
            this.UEVolume,
            this.UEQuantity,
            this.UEPeriod,
            this.UEJunk,
            this.UEStatus,
            this.UETmpFullCode,
            this.UEName2,
            this.UEName3,
            this.UETmpShortCode,
            this.UETmpCodeFirmCr,
            this.UEAlready,
            this.UETmpCurrency,
            this.UERowID,
            this.UEHandMade});
			this.dtUnrecExp.TableName = "UnrecExpGrid";
			// 
			// EUColumn1
			// 
			this.EUColumn1.Caption = "";
			this.EUColumn1.ColumnName = "UEColumn1";
			this.EUColumn1.DefaultValue = "";
			// 
			// UEColumn2
			// 
			this.UEColumn2.Caption = "";
			this.UEColumn2.ColumnName = "UEColumn2";
			// 
			// UEColumn3
			// 
			this.UEColumn3.Caption = "";
			this.UEColumn3.ColumnName = "UEColumn3";
			// 
			// UECode
			// 
			this.UECode.Caption = "Код";
			this.UECode.ColumnName = "UECode";
			// 
			// UECodeCr
			// 
			this.UECodeCr.Caption = "Код производителя";
			this.UECodeCr.ColumnName = "UECodeCr";
			// 
			// UEName1
			// 
			this.UEName1.Caption = "Наименование";
			this.UEName1.ColumnName = "UEName1";
			// 
			// UEFirmCr
			// 
			this.UEFirmCr.Caption = "Производитель";
			this.UEFirmCr.ColumnName = "UEFirmCr";
			// 
			// UECurrency
			// 
			this.UECurrency.Caption = "Валюта";
			this.UECurrency.ColumnName = "UECurrency";
			// 
			// UEBaseCost
			// 
			this.UEBaseCost.Caption = "Базовая цена";
			this.UEBaseCost.ColumnName = "UEBaseCost";
			this.UEBaseCost.DataType = typeof(double);
			// 
			// UEUnit
			// 
			this.UEUnit.Caption = "Ед. измерения";
			this.UEUnit.ColumnName = "UEUnit";
			// 
			// UEVolume
			// 
			this.UEVolume.Caption = "Цех. уп.";
			this.UEVolume.ColumnName = "UEVolume";
			// 
			// UEQuantity
			// 
			this.UEQuantity.Caption = "Количество";
			this.UEQuantity.ColumnName = "UEQuantity";
			// 
			// UEPeriod
			// 
			this.UEPeriod.Caption = "Срок годности";
			this.UEPeriod.ColumnName = "UEPeriod";
			// 
			// UEJunk
			// 
			this.UEJunk.Caption = "Просроченный";
			this.UEJunk.ColumnName = "UEJunk";
			this.UEJunk.DataType = typeof(byte);
			// 
			// UEStatus
			// 
			this.UEStatus.ColumnName = "UEStatus";
			this.UEStatus.DataType = typeof(int);
			// 
			// UETmpFullCode
			// 
			this.UETmpFullCode.ColumnName = "UETmpFullCode";
			this.UETmpFullCode.DataType = typeof(long);
			// 
			// UEName2
			// 
			this.UEName2.ColumnName = "UEName2";
			// 
			// UEName3
			// 
			this.UEName3.ColumnName = "UEName3";
			// 
			// UETmpShortCode
			// 
			this.UETmpShortCode.ColumnName = "UETmpShortCode";
			this.UETmpShortCode.DataType = typeof(long);
			// 
			// UETmpCodeFirmCr
			// 
			this.UETmpCodeFirmCr.ColumnName = "UETmpCodeFirmCr";
			this.UETmpCodeFirmCr.DataType = typeof(long);
			// 
			// UEAlready
			// 
			this.UEAlready.ColumnName = "UEAlready";
			this.UEAlready.DataType = typeof(int);
			// 
			// UETmpCurrency
			// 
			this.UETmpCurrency.ColumnName = "UETmpCurrency";
			// 
			// UERowID
			// 
			this.UERowID.ColumnName = "UERowID";
			this.UERowID.DataType = typeof(long);
			// 
			// UEHandMade
			// 
			this.UEHandMade.ColumnName = "UEHandMade";
			this.UEHandMade.DataType = typeof(byte);
			// 
			// dtCatalogName
			// 
			this.dtCatalogName.Columns.AddRange(new System.Data.DataColumn[] {
            this.CShortCode,
            this.CName});
			this.dtCatalogName.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "CCode"}, false),
            new System.Data.UniqueConstraint("Constraint2", new string[] {
                        "CName"}, false)});
			this.dtCatalogName.TableName = "CatalogNameGrid";
			// 
			// CShortCode
			// 
			this.CShortCode.AllowDBNull = false;
			this.CShortCode.Caption = "CCode";
			this.CShortCode.ColumnName = "CCode";
			this.CShortCode.DataType = typeof(long);
			// 
			// CName
			// 
			this.CName.ColumnName = "CName";
			// 
			// dtForm
			// 
			this.dtForm.Columns.AddRange(new System.Data.DataColumn[] {
            this.FShortCode,
            this.FFullCode,
            this.FForm});
			this.dtForm.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.ForeignKeyConstraint("Relation1", "CatalogNameGrid", new string[] {
                        "CCode"}, new string[] {
                        "FShortCode"}, System.Data.AcceptRejectRule.None, System.Data.Rule.Cascade, System.Data.Rule.Cascade)});
			this.dtForm.TableName = "FormGrid";
			// 
			// FShortCode
			// 
			this.FShortCode.Caption = "";
			this.FShortCode.ColumnName = "FShortCode";
			this.FShortCode.DataType = typeof(long);
			// 
			// FFullCode
			// 
			this.FFullCode.AllowDBNull = false;
			this.FFullCode.Caption = "";
			this.FFullCode.ColumnName = "FFullCode";
			this.FFullCode.DataType = typeof(long);
			// 
			// FForm
			// 
			this.FForm.Caption = "";
			this.FForm.ColumnName = "FForm";
			// 
			// dtZero
			// 
			this.dtZero.Columns.AddRange(new System.Data.DataColumn[] {
            this.ZCode,
            this.ZCodeCr,
            this.ZName,
            this.ZFirmCr,
            this.ZCurrency,
            this.ZBaseCost,
            this.ZUnit,
            this.ZVolume,
            this.ZQuantity,
            this.ZPeriod,
            this.ZJunk});
			this.dtZero.TableName = "ZeroGrid";
			// 
			// ZCode
			// 
			this.ZCode.ColumnName = "ZCode";
			// 
			// ZCodeCr
			// 
			this.ZCodeCr.ColumnName = "ZCodeCr";
			// 
			// ZName
			// 
			this.ZName.ColumnName = "ZName";
			// 
			// ZFirmCr
			// 
			this.ZFirmCr.ColumnName = "ZFirmCr";
			// 
			// ZCurrency
			// 
			this.ZCurrency.ColumnName = "ZCurrency";
			// 
			// ZBaseCost
			// 
			this.ZBaseCost.ColumnName = "ZBaseCost";
			this.ZBaseCost.DataType = typeof(double);
			// 
			// ZUnit
			// 
			this.ZUnit.ColumnName = "ZUnit";
			// 
			// ZVolume
			// 
			this.ZVolume.ColumnName = "ZVolume";
			// 
			// ZQuantity
			// 
			this.ZQuantity.ColumnName = "ZQuantity";
			// 
			// ZPeriod
			// 
			this.ZPeriod.ColumnName = "ZPeriod";
			// 
			// ZJunk
			// 
			this.ZJunk.ColumnName = "ZJunk";
			this.ZJunk.DataType = typeof(byte);
			// 
			// dtForb
			// 
			this.dtForb.Columns.AddRange(new System.Data.DataColumn[] {
            this.FForb});
			this.dtForb.TableName = "ForbGrid";
			// 
			// FForb
			// 
			this.FForb.ColumnName = "FForb";
			// 
			// dtRegions
			// 
			this.dtRegions.Columns.AddRange(new System.Data.DataColumn[] {
            this.RRegion});
			this.dtRegions.TableName = "RegionsGrid";
			// 
			// RRegion
			// 
			this.RRegion.ColumnName = "RRegion";
			// 
			// dtCatalogFirmCr
			// 
			this.dtCatalogFirmCr.Columns.AddRange(new System.Data.DataColumn[] {
            this.CCodeFirmCr,
            this.CFirmCr});
			this.dtCatalogFirmCr.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "CCode"}, true)});
			this.dtCatalogFirmCr.PrimaryKey = new System.Data.DataColumn[] {
        this.CCodeFirmCr};
			this.dtCatalogFirmCr.TableName = "CatalogFirmCrGrid";
			// 
			// CCodeFirmCr
			// 
			this.CCodeFirmCr.AllowDBNull = false;
			this.CCodeFirmCr.ColumnName = "CCode";
			this.CCodeFirmCr.DataType = typeof(long);
			// 
			// CFirmCr
			// 
			this.CFirmCr.ColumnName = "CName";
			// 
			// dtCurrency
			// 
			this.dtCurrency.Columns.AddRange(new System.Data.DataColumn[] {
            this.CCurrency,
            this.CExchange});
			this.dtCurrency.TableName = "CurrencyGrid";
			// 
			// CCurrency
			// 
			this.CCurrency.ColumnName = "CCurrency";
			// 
			// CExchange
			// 
			this.CExchange.ColumnName = "CExchange";
			this.CExchange.DataType = typeof(decimal);
			// 
			// dtSections
			// 
			this.dtSections.Columns.AddRange(new System.Data.DataColumn[] {
            this.SSection});
			this.dtSections.TableName = "SectionsGrid";
			// 
			// SSection
			// 
			this.SSection.ColumnName = "SSection";
			// 
			// gvCatalog
			// 
			this.gvCatalog.Appearance.HideSelectionRow.Options.UseBackColor = true;
			this.gvCatalog.ChildGridLevelName = "gvCatForm";
			this.gvCatalog.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCName});
			this.gvCatalog.GridControl = this.CatalogGridControl;
			this.gvCatalog.Name = "gvCatalog";
			this.gvCatalog.OptionsBehavior.AllowIncrementalSearch = true;
			this.gvCatalog.OptionsMenu.EnableColumnMenu = false;
			this.gvCatalog.OptionsMenu.EnableFooterMenu = false;
			this.gvCatalog.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvCatalog.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gvCatalog.OptionsView.ShowFilterPanel = false;
			this.gvCatalog.OptionsView.ShowGroupPanel = false;
			// 
			// colCName
			// 
			this.colCName.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.colCName.AppearanceCell.Options.UseFont = true;
			this.colCName.Caption = "Наименование";
			this.colCName.FieldName = "CName";
			this.colCName.Name = "colCName";
			this.colCName.OptionsColumn.AllowEdit = false;
			this.colCName.OptionsColumn.ReadOnly = true;
			this.colCName.Visible = true;
			this.colCName.VisibleIndex = 0;
			// 
			// statusBarPanel1
			// 
			this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.statusBarPanel1.Name = "statusBarPanel1";
			this.statusBarPanel1.Width = 234;
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 731);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.sbpAll,
            this.sbpCurrent});
			this.statusBar1.ShowPanels = true;
			this.statusBar1.Size = new System.Drawing.Size(720, 22);
			this.statusBar1.TabIndex = 1;
			this.statusBar1.Text = "statusBar1";
			// 
			// sbpAll
			// 
			this.sbpAll.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.sbpAll.Name = "sbpAll";
			this.sbpAll.Width = 234;
			// 
			// sbpCurrent
			// 
			this.sbpCurrent.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.sbpCurrent.Name = "sbpCurrent";
			this.sbpCurrent.Width = 234;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.tcMain);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(720, 731);
			this.panel3.TabIndex = 2;
			// 
			// tcMain
			// 
			this.tcMain.Controls.Add(this.tpJobs);
			this.tcMain.Controls.Add(this.tpUnrecExp);
			this.tcMain.Controls.Add(this.tpZero);
			this.tcMain.Controls.Add(this.tpForb);
			this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcMain.Location = new System.Drawing.Point(0, 0);
			this.tcMain.Name = "tcMain";
			this.tcMain.SelectedIndex = 0;
			this.tcMain.Size = new System.Drawing.Size(720, 731);
			this.tcMain.TabIndex = 0;
			this.tcMain.SelectedIndexChanged += new System.EventHandler(this.tcMain_SelectedIndexChanged);
			// 
			// tpJobs
			// 
			this.tpJobs.Controls.Add(this.pnlCenter1);
			this.tpJobs.Location = new System.Drawing.Point(4, 22);
			this.tpJobs.Name = "tpJobs";
			this.tpJobs.Size = new System.Drawing.Size(712, 705);
			this.tpJobs.TabIndex = 0;
			this.tpJobs.Text = "Задания";
			// 
			// pnlCenter1
			// 
			this.pnlCenter1.Controls.Add(this.pnlTop1);
			this.pnlCenter1.Controls.Add(this.pnlWithButton1);
			this.pnlCenter1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlCenter1.Location = new System.Drawing.Point(0, 0);
			this.pnlCenter1.Name = "pnlCenter1";
			this.pnlCenter1.Size = new System.Drawing.Size(712, 705);
			this.pnlCenter1.TabIndex = 5;
			// 
			// pnlTop1
			// 
			this.pnlTop1.Controls.Add(this.JobsGridControl);
			this.pnlTop1.Controls.Add(this.groupBox1);
			this.pnlTop1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlTop1.Location = new System.Drawing.Point(0, 0);
			this.pnlTop1.Name = "pnlTop1";
			this.pnlTop1.Size = new System.Drawing.Size(712, 681);
			this.pnlTop1.TabIndex = 1;
			// 
			// JobsGridControl
			// 
			this.JobsGridControl.DataSource = this.dtJobs;
			this.JobsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.JobsGridControl.EmbeddedNavigator.Name = "";
			this.JobsGridControl.Location = new System.Drawing.Point(0, 0);
			this.JobsGridControl.MainView = this.gvJobs;
			this.JobsGridControl.Name = "JobsGridControl";
			this.JobsGridControl.Size = new System.Drawing.Size(592, 681);
			this.JobsGridControl.TabIndex = 4;
			this.JobsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvJobs});
			this.JobsGridControl.DoubleClick += new System.EventHandler(this.JobsGridControl_DoubleClick);
			this.JobsGridControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JobsGridControl_KeyDown);
			// 
			// gvJobs
			// 
			this.gvJobs.Appearance.FocusedCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.gvJobs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colJName,
            this.colJParentName,
            this.colJWholeSale,
            this.colJRegion,
            this.colJNeedRetrans,
            this.colJRetranced,
            this.colJPos,
            this.colJNamePos,
            this.colJJobDate,
            this.colJBlockBy});
			this.gvJobs.GridControl = this.JobsGridControl;
			this.gvJobs.Images = this.imageList1;
			this.gvJobs.IndicatorWidth = 16;
			this.gvJobs.Name = "gvJobs";
			this.gvJobs.OptionsBehavior.AllowIncrementalSearch = true;
			this.gvJobs.OptionsMenu.EnableColumnMenu = false;
			this.gvJobs.OptionsMenu.EnableFooterMenu = false;
			this.gvJobs.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvJobs.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gvJobs.OptionsView.ShowGroupPanel = false;
			this.gvJobs.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvJobs_RowStyle);
			this.gvJobs.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvJobs_CustomDrawRowIndicator);
			this.gvJobs.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvJobs_CustomColumnDisplayText);
			// 
			// colJName
			// 
			this.colJName.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.colJName.AppearanceCell.Options.UseFont = true;
			this.colJName.Caption = "Фирма (прайс-лист)";
			this.colJName.FieldName = "JName";
			this.colJName.Name = "colJName";
			this.colJName.OptionsColumn.AllowEdit = false;
			this.colJName.OptionsColumn.ReadOnly = true;
			this.colJName.Visible = true;
			this.colJName.VisibleIndex = 0;
			// 
			// colJParentName
			// 
			this.colJParentName.Caption = "Родитель";
			this.colJParentName.FieldName = "JParentName";
			this.colJParentName.Name = "colJParentName";
			this.colJParentName.OptionsColumn.AllowEdit = false;
			this.colJParentName.OptionsColumn.AllowFocus = false;
			this.colJParentName.OptionsColumn.AllowIncrementalSearch = false;
			this.colJParentName.OptionsColumn.ReadOnly = true;
			this.colJParentName.Visible = true;
			this.colJParentName.VisibleIndex = 1;
			// 
			// colJWholeSale
			// 
			this.colJWholeSale.AppearanceCell.Options.UseTextOptions = true;
			this.colJWholeSale.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.colJWholeSale.Caption = "Сегмент";
			this.colJWholeSale.FieldName = "JWholeSale";
			this.colJWholeSale.Name = "colJWholeSale";
			this.colJWholeSale.OptionsColumn.AllowEdit = false;
			this.colJWholeSale.OptionsColumn.AllowFocus = false;
			this.colJWholeSale.OptionsColumn.AllowIncrementalSearch = false;
			this.colJWholeSale.OptionsColumn.ReadOnly = true;
			this.colJWholeSale.Visible = true;
			this.colJWholeSale.VisibleIndex = 2;
			// 
			// colJRegion
			// 
			this.colJRegion.Caption = "Регион";
			this.colJRegion.FieldName = "JRegion";
			this.colJRegion.Name = "colJRegion";
			this.colJRegion.OptionsColumn.AllowEdit = false;
			this.colJRegion.OptionsColumn.AllowFocus = false;
			this.colJRegion.OptionsColumn.AllowIncrementalSearch = false;
			this.colJRegion.OptionsColumn.ReadOnly = true;
			this.colJRegion.Visible = true;
			this.colJRegion.VisibleIndex = 3;
			// 
			// colJNeedRetrans
			// 
			this.colJNeedRetrans.Caption = "Требуется формализация";
			this.colJNeedRetrans.FieldName = "JNeedRetrans";
			this.colJNeedRetrans.Name = "colJNeedRetrans";
			this.colJNeedRetrans.OptionsColumn.AllowEdit = false;
			this.colJNeedRetrans.OptionsColumn.AllowFocus = false;
			this.colJNeedRetrans.OptionsColumn.AllowIncrementalSearch = false;
			this.colJNeedRetrans.OptionsColumn.ReadOnly = true;
			this.colJNeedRetrans.ToolTip = "Требуется формализация";
			this.colJNeedRetrans.Visible = true;
			this.colJNeedRetrans.VisibleIndex = 4;
			// 
			// colJRetranced
			// 
			this.colJRetranced.Caption = "В очереди";
			this.colJRetranced.FieldName = "JRetranced";
			this.colJRetranced.Name = "colJRetranced";
			this.colJRetranced.OptionsColumn.AllowEdit = false;
			this.colJRetranced.OptionsColumn.AllowFocus = false;
			this.colJRetranced.OptionsColumn.AllowIncrementalSearch = false;
			this.colJRetranced.OptionsColumn.ReadOnly = true;
			this.colJRetranced.ToolTip = "В очереди";
			this.colJRetranced.Visible = true;
			this.colJRetranced.VisibleIndex = 5;
			// 
			// colJPos
			// 
			this.colJPos.Caption = "Всего";
			this.colJPos.FieldName = "JPos";
			this.colJPos.Name = "colJPos";
			this.colJPos.OptionsColumn.AllowEdit = false;
			this.colJPos.OptionsColumn.AllowFocus = false;
			this.colJPos.OptionsColumn.AllowIncrementalSearch = false;
			this.colJPos.OptionsColumn.ReadOnly = true;
			this.colJPos.Visible = true;
			this.colJPos.VisibleIndex = 6;
			// 
			// colJNamePos
			// 
			this.colJNamePos.Caption = "Строго";
			this.colJNamePos.FieldName = "JNamePos";
			this.colJNamePos.Name = "colJNamePos";
			this.colJNamePos.OptionsColumn.AllowEdit = false;
			this.colJNamePos.OptionsColumn.AllowFocus = false;
			this.colJNamePos.OptionsColumn.AllowIncrementalSearch = false;
			this.colJNamePos.OptionsColumn.ReadOnly = true;
			this.colJNamePos.Visible = true;
			this.colJNamePos.VisibleIndex = 7;
			// 
			// colJJobDate
			// 
			this.colJJobDate.Caption = "Дата задания";
			this.colJJobDate.FieldName = "JJobDate";
			this.colJJobDate.Name = "colJJobDate";
			this.colJJobDate.OptionsColumn.AllowEdit = false;
			this.colJJobDate.OptionsColumn.AllowFocus = false;
			this.colJJobDate.OptionsColumn.AllowIncrementalSearch = false;
			this.colJJobDate.OptionsColumn.ReadOnly = true;
			this.colJJobDate.Visible = true;
			this.colJJobDate.VisibleIndex = 8;
			// 
			// colJBlockBy
			// 
			this.colJBlockBy.Caption = "Блокировано";
			this.colJBlockBy.FieldName = "JBlockBy";
			this.colJBlockBy.Name = "colJBlockBy";
			this.colJBlockBy.OptionsColumn.AllowEdit = false;
			this.colJBlockBy.OptionsColumn.AllowFocus = false;
			this.colJBlockBy.OptionsColumn.AllowIncrementalSearch = false;
			this.colJBlockBy.OptionsColumn.ReadOnly = true;
			this.colJBlockBy.Visible = true;
			this.colJBlockBy.VisibleIndex = 9;
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Silver;
			this.imageList1.Images.SetKeyName(0, "");
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnJobs50);
			this.groupBox1.Controls.Add(this.btnJobsNamePos);
			this.groupBox1.Controls.Add(this.btnJobsBlock);
			this.groupBox1.Controls.Add(this.lbJobs50Text);
			this.groupBox1.Controls.Add(this.lbJobsNamePosText);
			this.groupBox1.Controls.Add(this.lbJobsBlockText);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupBox1.Location = new System.Drawing.Point(592, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(120, 681);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Легенда";
			// 
			// btnJobs50
			// 
			this.btnJobs50.BackColor = System.Drawing.Color.NavajoWhite;
			this.btnJobs50.Location = new System.Drawing.Point(8, 160);
			this.btnJobs50.Name = "btnJobs50";
			this.btnJobs50.Size = new System.Drawing.Size(32, 16);
			this.btnJobs50.TabIndex = 19;
			this.btnJobs50.UseVisualStyleBackColor = false;
			this.btnJobs50.Click += new System.EventHandler(this.lbColorChange);
			// 
			// btnJobsNamePos
			// 
			this.btnJobsNamePos.BackColor = System.Drawing.Color.Coral;
			this.btnJobsNamePos.Location = new System.Drawing.Point(8, 88);
			this.btnJobsNamePos.Name = "btnJobsNamePos";
			this.btnJobsNamePos.Size = new System.Drawing.Size(32, 16);
			this.btnJobsNamePos.TabIndex = 18;
			this.btnJobsNamePos.UseVisualStyleBackColor = false;
			this.btnJobsNamePos.Click += new System.EventHandler(this.lbColorChange);
			// 
			// btnJobsBlock
			// 
			this.btnJobsBlock.BackColor = System.Drawing.Color.OrangeRed;
			this.btnJobsBlock.Location = new System.Drawing.Point(8, 24);
			this.btnJobsBlock.Name = "btnJobsBlock";
			this.btnJobsBlock.Size = new System.Drawing.Size(32, 16);
			this.btnJobsBlock.TabIndex = 17;
			this.btnJobsBlock.UseVisualStyleBackColor = false;
			this.btnJobsBlock.Click += new System.EventHandler(this.lbColorChange);
			// 
			// lbJobs50Text
			// 
			this.lbJobs50Text.Location = new System.Drawing.Point(8, 176);
			this.lbJobs50Text.Name = "lbJobs50Text";
			this.lbJobs50Text.Size = new System.Drawing.Size(104, 56);
			this.lbJobs50Text.TabIndex = 6;
			this.lbJobs50Text.Text = "Процент позиций,  нераспознанных по наименованию меньше 50";
			// 
			// lbJobsNamePosText
			// 
			this.lbJobsNamePosText.Location = new System.Drawing.Point(8, 104);
			this.lbJobsNamePosText.Name = "lbJobsNamePosText";
			this.lbJobsNamePosText.Size = new System.Drawing.Size(104, 40);
			this.lbJobsNamePosText.TabIndex = 5;
			this.lbJobsNamePosText.Text = "Нет позиций нерапознанных по наименованию";
			// 
			// lbJobsBlockText
			// 
			this.lbJobsBlockText.Location = new System.Drawing.Point(8, 40);
			this.lbJobsBlockText.Name = "lbJobsBlockText";
			this.lbJobsBlockText.Size = new System.Drawing.Size(80, 40);
			this.lbJobsBlockText.TabIndex = 4;
			this.lbJobsBlockText.Text = "Прайс-лист заблокирован оператором";
			// 
			// pnlWithButton1
			// 
			this.pnlWithButton1.Controls.Add(this.btnDelJob);
			this.pnlWithButton1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlWithButton1.Location = new System.Drawing.Point(0, 681);
			this.pnlWithButton1.Name = "pnlWithButton1";
			this.pnlWithButton1.Size = new System.Drawing.Size(712, 24);
			this.pnlWithButton1.TabIndex = 0;
			// 
			// btnDelJob
			// 
			this.btnDelJob.BackColor = System.Drawing.SystemColors.Control;
			this.btnDelJob.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnDelJob.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnDelJob.ImageIndex = 4;
			this.btnDelJob.ImageList = this.imageList2;
			this.btnDelJob.Location = new System.Drawing.Point(528, 0);
			this.btnDelJob.Name = "btnDelJob";
			this.btnDelJob.Size = new System.Drawing.Size(184, 24);
			this.btnDelJob.TabIndex = 6;
			this.btnDelJob.Text = "Удалить ЗАДАНИЕ";
			this.btnDelJob.UseVisualStyleBackColor = false;
			this.btnDelJob.Click += new System.EventHandler(this.btnDelJob_Click);
			// 
			// imageList2
			// 
			this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
			this.imageList2.TransparentColor = System.Drawing.SystemColors.Control;
			this.imageList2.Images.SetKeyName(0, "");
			this.imageList2.Images.SetKeyName(1, "");
			this.imageList2.Images.SetKeyName(2, "");
			this.imageList2.Images.SetKeyName(3, "");
			this.imageList2.Images.SetKeyName(4, "");
			// 
			// tpUnrecExp
			// 
			this.tpUnrecExp.ContextMenu = this.cmUnrecExp;
			this.tpUnrecExp.Controls.Add(this.pnlBottom2);
			this.tpUnrecExp.Controls.Add(this.pnlCenter2);
			this.tpUnrecExp.Controls.Add(this.pnlTop2);
			this.tpUnrecExp.Location = new System.Drawing.Point(4, 22);
			this.tpUnrecExp.Name = "tpUnrecExp";
			this.tpUnrecExp.Size = new System.Drawing.Size(712, 705);
			this.tpUnrecExp.TabIndex = 1;
			this.tpUnrecExp.Text = "Нераспознанные";
			this.tpUnrecExp.Visible = false;
			// 
			// cmUnrecExp
			// 
			this.cmUnrecExp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miSendAboutNames,
            this.miSendAboutFirmCr});
			// 
			// miSendAboutNames
			// 
			this.miSendAboutNames.Index = 0;
			this.miSendAboutNames.Text = "Отправить письмо о наименованиях";
			this.miSendAboutNames.Click += new System.EventHandler(this.miSendAboutNames_Click);
			// 
			// miSendAboutFirmCr
			// 
			this.miSendAboutFirmCr.Index = 1;
			this.miSendAboutFirmCr.Text = "Отправить письмо о производителях";
			this.miSendAboutFirmCr.Click += new System.EventHandler(this.miSendAboutFirmCr_Click);
			// 
			// pnlBottom2
			// 
			this.pnlBottom2.Controls.Add(this.UnrecExpGridControl);
			this.pnlBottom2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlBottom2.Location = new System.Drawing.Point(0, 280);
			this.pnlBottom2.Name = "pnlBottom2";
			this.pnlBottom2.Size = new System.Drawing.Size(712, 425);
			this.pnlBottom2.TabIndex = 7;
			// 
			// UnrecExpGridControl
			// 
			this.UnrecExpGridControl.DataSource = this.dtUnrecExp;
			this.UnrecExpGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.UnrecExpGridControl.EmbeddedNavigator.Name = "";
			this.UnrecExpGridControl.Location = new System.Drawing.Point(0, 0);
			this.UnrecExpGridControl.MainView = this.gvUnrecExp;
			this.UnrecExpGridControl.Name = "UnrecExpGridControl";
			this.UnrecExpGridControl.Size = new System.Drawing.Size(712, 425);
			this.UnrecExpGridControl.TabIndex = 1;
			this.UnrecExpGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvUnrecExp});
			this.UnrecExpGridControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UnrecExpGridControl_KeyDown);
			this.UnrecExpGridControl.Click += new System.EventHandler(this.UnrecExpGridControl_Click);
			// 
			// gvUnrecExp
			// 
			this.gvUnrecExp.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colUEColumn1,
            this.colUEColumn2,
            this.colUEColumn3,
            this.colUECode,
            this.colUECodeCr,
            this.colUEName1,
            this.colUEFirmCr,
            this.colUECurrency,
            this.colUEBaseCost,
            this.colUEUnit,
            this.colUEVolume,
            this.colUEQuantity,
            this.colUEPeriod,
            this.colUEJunk,
            this.colUEStatus,
            this.colUEAlready});
			this.gvUnrecExp.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
			this.gvUnrecExp.GridControl = this.UnrecExpGridControl;
			this.gvUnrecExp.Name = "gvUnrecExp";
			this.gvUnrecExp.OptionsCustomization.AllowSort = false;
			this.gvUnrecExp.OptionsDetail.EnableMasterViewMode = false;
			this.gvUnrecExp.OptionsMenu.EnableColumnMenu = false;
			this.gvUnrecExp.OptionsMenu.EnableFooterMenu = false;
			this.gvUnrecExp.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvUnrecExp.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gvUnrecExp.OptionsView.ShowGroupPanel = false;
			this.gvUnrecExp.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvUnrecExp_RowCellStyle);
			this.gvUnrecExp.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gvUnrecExp_CustomDrawCell);
			this.gvUnrecExp.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvUnrecExp_FocusedRowChanged);
			this.gvUnrecExp.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(this.gvUnrecExp_CustomColumnSort);
			this.gvUnrecExp.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvUnrecExp_CustomColumnDisplayText);
			// 
			// colUEColumn1
			// 
			this.colUEColumn1.Caption = "Name";
			this.colUEColumn1.FieldName = "UEColumn1";
			this.colUEColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
			this.colUEColumn1.Name = "colUEColumn1";
			this.colUEColumn1.OptionsColumn.AllowEdit = false;
			this.colUEColumn1.OptionsColumn.AllowSize = false;
			this.colUEColumn1.OptionsColumn.FixedWidth = true;
			this.colUEColumn1.ToolTip = "Признак того, что позиция распознана по наименованию";
			this.colUEColumn1.Visible = true;
			this.colUEColumn1.VisibleIndex = 0;
			this.colUEColumn1.Width = 20;
			// 
			// colUEColumn2
			// 
			this.colUEColumn2.Caption = "FirmCr";
			this.colUEColumn2.FieldName = "UEColumn2";
			this.colUEColumn2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
			this.colUEColumn2.Name = "colUEColumn2";
			this.colUEColumn2.OptionsColumn.AllowEdit = false;
			this.colUEColumn2.OptionsColumn.AllowSize = false;
			this.colUEColumn2.OptionsColumn.FixedWidth = true;
			this.colUEColumn2.ToolTip = "Признак того, что позиция распознана по производителю";
			this.colUEColumn2.Visible = true;
			this.colUEColumn2.VisibleIndex = 1;
			this.colUEColumn2.Width = 20;
			// 
			// colUEColumn3
			// 
			this.colUEColumn3.Caption = "Curr";
			this.colUEColumn3.FieldName = "UEColumn3";
			this.colUEColumn3.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
			this.colUEColumn3.Name = "colUEColumn3";
			this.colUEColumn3.OptionsColumn.AllowEdit = false;
			this.colUEColumn3.OptionsColumn.AllowSize = false;
			this.colUEColumn3.OptionsColumn.FixedWidth = true;
			this.colUEColumn3.ToolTip = "Признак того, что позиция распознана по валюте";
			this.colUEColumn3.Visible = true;
			this.colUEColumn3.VisibleIndex = 2;
			this.colUEColumn3.Width = 20;
			// 
			// colUECode
			// 
			this.colUECode.Caption = "Код";
			this.colUECode.FieldName = "UECode";
			this.colUECode.Name = "colUECode";
			this.colUECode.OptionsColumn.AllowEdit = false;
			this.colUECode.Visible = true;
			this.colUECode.VisibleIndex = 3;
			this.colUECode.Width = 55;
			// 
			// colUECodeCr
			// 
			this.colUECodeCr.Caption = "Код производителя";
			this.colUECodeCr.FieldName = "UECodeCr";
			this.colUECodeCr.Name = "colUECodeCr";
			this.colUECodeCr.OptionsColumn.AllowEdit = false;
			this.colUECodeCr.Visible = true;
			this.colUECodeCr.VisibleIndex = 4;
			this.colUECodeCr.Width = 55;
			// 
			// colUEName1
			// 
			this.colUEName1.Caption = "Наименование";
			this.colUEName1.FieldName = "UEName1";
			this.colUEName1.Name = "colUEName1";
			this.colUEName1.OptionsColumn.AllowEdit = false;
			this.colUEName1.Visible = true;
			this.colUEName1.VisibleIndex = 5;
			this.colUEName1.Width = 55;
			// 
			// colUEFirmCr
			// 
			this.colUEFirmCr.Caption = "Производитель";
			this.colUEFirmCr.FieldName = "UEFirmCr";
			this.colUEFirmCr.Name = "colUEFirmCr";
			this.colUEFirmCr.OptionsColumn.AllowEdit = false;
			this.colUEFirmCr.Visible = true;
			this.colUEFirmCr.VisibleIndex = 6;
			this.colUEFirmCr.Width = 55;
			// 
			// colUECurrency
			// 
			this.colUECurrency.Caption = "Валюта";
			this.colUECurrency.FieldName = "UECurrency";
			this.colUECurrency.Name = "colUECurrency";
			this.colUECurrency.OptionsColumn.AllowEdit = false;
			this.colUECurrency.Visible = true;
			this.colUECurrency.VisibleIndex = 7;
			this.colUECurrency.Width = 55;
			// 
			// colUEBaseCost
			// 
			this.colUEBaseCost.Caption = "Базовая цена";
			this.colUEBaseCost.FieldName = "UEBaseCost";
			this.colUEBaseCost.Name = "colUEBaseCost";
			this.colUEBaseCost.OptionsColumn.AllowEdit = false;
			this.colUEBaseCost.Visible = true;
			this.colUEBaseCost.VisibleIndex = 8;
			this.colUEBaseCost.Width = 55;
			// 
			// colUEUnit
			// 
			this.colUEUnit.Caption = "Ед. измерения";
			this.colUEUnit.FieldName = "UEUnit";
			this.colUEUnit.Name = "colUEUnit";
			this.colUEUnit.OptionsColumn.AllowEdit = false;
			this.colUEUnit.Visible = true;
			this.colUEUnit.VisibleIndex = 9;
			this.colUEUnit.Width = 55;
			// 
			// colUEVolume
			// 
			this.colUEVolume.Caption = "Цех. уп.";
			this.colUEVolume.FieldName = "UEVolume";
			this.colUEVolume.Name = "colUEVolume";
			this.colUEVolume.OptionsColumn.AllowEdit = false;
			this.colUEVolume.Visible = true;
			this.colUEVolume.VisibleIndex = 10;
			this.colUEVolume.Width = 55;
			// 
			// colUEQuantity
			// 
			this.colUEQuantity.Caption = "Количество";
			this.colUEQuantity.FieldName = "UEQuantity";
			this.colUEQuantity.Name = "colUEQuantity";
			this.colUEQuantity.OptionsColumn.AllowEdit = false;
			this.colUEQuantity.Visible = true;
			this.colUEQuantity.VisibleIndex = 11;
			this.colUEQuantity.Width = 55;
			// 
			// colUEPeriod
			// 
			this.colUEPeriod.Caption = "Срок годности";
			this.colUEPeriod.FieldName = "UEPeriod";
			this.colUEPeriod.Name = "colUEPeriod";
			this.colUEPeriod.OptionsColumn.AllowEdit = false;
			this.colUEPeriod.Visible = true;
			this.colUEPeriod.VisibleIndex = 12;
			this.colUEPeriod.Width = 55;
			// 
			// colUEJunk
			// 
			this.colUEJunk.Caption = "Просроченный";
			this.colUEJunk.FieldName = "UEJunk";
			this.colUEJunk.Name = "colUEJunk";
			this.colUEJunk.OptionsColumn.AllowEdit = false;
			this.colUEJunk.Visible = true;
			this.colUEJunk.VisibleIndex = 13;
			this.colUEJunk.Width = 55;
			// 
			// colUEStatus
			// 
			this.colUEStatus.Caption = "UEStatus";
			this.colUEStatus.FieldName = "UEStatus";
			this.colUEStatus.Name = "colUEStatus";
			// 
			// colUEAlready
			// 
			this.colUEAlready.Caption = "UEAlready";
			this.colUEAlready.FieldName = "UEAlready";
			this.colUEAlready.Name = "colUEAlready";
			this.colUEAlready.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
			// 
			// pnlCenter2
			// 
			this.pnlCenter2.Controls.Add(this.pnlLeft2);
			this.pnlCenter2.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlCenter2.Location = new System.Drawing.Point(0, 24);
			this.pnlCenter2.Name = "pnlCenter2";
			this.pnlCenter2.Size = new System.Drawing.Size(712, 256);
			this.pnlCenter2.TabIndex = 6;
			// 
			// pnlLeft2
			// 
			this.pnlLeft2.Controls.Add(this.grpBoxCatalog2);
			this.pnlLeft2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlLeft2.Location = new System.Drawing.Point(0, 0);
			this.pnlLeft2.Name = "pnlLeft2";
			this.pnlLeft2.Size = new System.Drawing.Size(712, 256);
			this.pnlLeft2.TabIndex = 1;
			// 
			// grpBoxCatalog2
			// 
			this.grpBoxCatalog2.Controls.Add(this.CatalogGridControl);
			this.grpBoxCatalog2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grpBoxCatalog2.Location = new System.Drawing.Point(0, 0);
			this.grpBoxCatalog2.Name = "grpBoxCatalog2";
			this.grpBoxCatalog2.Size = new System.Drawing.Size(712, 256);
			this.grpBoxCatalog2.TabIndex = 0;
			this.grpBoxCatalog2.TabStop = false;
			this.grpBoxCatalog2.Text = "Каталог";
			// 
			// pnlTop2
			// 
			this.pnlTop2.Controls.Add(this.BigNameLabel2);
			this.pnlTop2.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlTop2.Location = new System.Drawing.Point(0, 0);
			this.pnlTop2.Name = "pnlTop2";
			this.pnlTop2.Size = new System.Drawing.Size(712, 24);
			this.pnlTop2.TabIndex = 0;
			// 
			// BigNameLabel2
			// 
			this.BigNameLabel2.BackColor = System.Drawing.SystemColors.Info;
			this.BigNameLabel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.BigNameLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BigNameLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.BigNameLabel2.Location = new System.Drawing.Point(0, 0);
			this.BigNameLabel2.Name = "BigNameLabel2";
			this.BigNameLabel2.Size = new System.Drawing.Size(712, 24);
			this.BigNameLabel2.TabIndex = 0;
			this.BigNameLabel2.Text = "BigNameLabel";
			this.BigNameLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tpZero
			// 
			this.tpZero.Controls.Add(this.ZeroGridControl);
			this.tpZero.Location = new System.Drawing.Point(4, 22);
			this.tpZero.Name = "tpZero";
			this.tpZero.Size = new System.Drawing.Size(712, 705);
			this.tpZero.TabIndex = 2;
			this.tpZero.Text = "Цена 0";
			this.tpZero.Visible = false;
			// 
			// ZeroGridControl
			// 
			this.ZeroGridControl.DataSource = this.dtZero;
			this.ZeroGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.ZeroGridControl.EmbeddedNavigator.Name = "";
			this.ZeroGridControl.Location = new System.Drawing.Point(0, 0);
			this.ZeroGridControl.MainView = this.gvZero;
			this.ZeroGridControl.Name = "ZeroGridControl";
			this.ZeroGridControl.Size = new System.Drawing.Size(712, 705);
			this.ZeroGridControl.TabIndex = 0;
			this.ZeroGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvZero});
			// 
			// gvZero
			// 
			this.gvZero.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colZCode,
            this.colZCodeCr,
            this.colZName,
            this.colZFirmCr,
            this.colZCurrency,
            this.colZUnit,
            this.colZVolume,
            this.colZQuantity,
            this.colZPeriod});
			this.gvZero.GridControl = this.ZeroGridControl;
			this.gvZero.Name = "gvZero";
			this.gvZero.OptionsMenu.EnableColumnMenu = false;
			this.gvZero.OptionsMenu.EnableFooterMenu = false;
			this.gvZero.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvZero.OptionsView.ShowGroupPanel = false;
			// 
			// colZCode
			// 
			this.colZCode.Caption = "Код";
			this.colZCode.FieldName = "ZCode";
			this.colZCode.Name = "colZCode";
			this.colZCode.OptionsColumn.AllowEdit = false;
			this.colZCode.Visible = true;
			this.colZCode.VisibleIndex = 0;
			// 
			// colZCodeCr
			// 
			this.colZCodeCr.Caption = "Код производителя";
			this.colZCodeCr.FieldName = "ZCodeCr";
			this.colZCodeCr.Name = "colZCodeCr";
			this.colZCodeCr.OptionsColumn.AllowEdit = false;
			this.colZCodeCr.Visible = true;
			this.colZCodeCr.VisibleIndex = 1;
			// 
			// colZName
			// 
			this.colZName.Caption = "Наименование";
			this.colZName.FieldName = "ZName";
			this.colZName.Name = "colZName";
			this.colZName.OptionsColumn.AllowEdit = false;
			this.colZName.Visible = true;
			this.colZName.VisibleIndex = 2;
			// 
			// colZFirmCr
			// 
			this.colZFirmCr.Caption = "Производитель";
			this.colZFirmCr.FieldName = "ZFirmCr";
			this.colZFirmCr.Name = "colZFirmCr";
			this.colZFirmCr.OptionsColumn.AllowEdit = false;
			this.colZFirmCr.Visible = true;
			this.colZFirmCr.VisibleIndex = 3;
			// 
			// colZCurrency
			// 
			this.colZCurrency.Caption = "Валюта";
			this.colZCurrency.FieldName = "ZCurrency";
			this.colZCurrency.Name = "colZCurrency";
			this.colZCurrency.OptionsColumn.AllowEdit = false;
			this.colZCurrency.Visible = true;
			this.colZCurrency.VisibleIndex = 4;
			// 
			// colZUnit
			// 
			this.colZUnit.Caption = "Ед. измерения";
			this.colZUnit.FieldName = "ZUnit";
			this.colZUnit.Name = "colZUnit";
			this.colZUnit.OptionsColumn.AllowEdit = false;
			this.colZUnit.Visible = true;
			this.colZUnit.VisibleIndex = 5;
			// 
			// colZVolume
			// 
			this.colZVolume.Caption = "Цех. уп.";
			this.colZVolume.FieldName = "ZVolume";
			this.colZVolume.Name = "colZVolume";
			this.colZVolume.OptionsColumn.AllowEdit = false;
			this.colZVolume.Visible = true;
			this.colZVolume.VisibleIndex = 6;
			// 
			// colZQuantity
			// 
			this.colZQuantity.Caption = "Количество";
			this.colZQuantity.FieldName = "ZQuantity";
			this.colZQuantity.Name = "colZQuantity";
			this.colZQuantity.OptionsColumn.AllowEdit = false;
			this.colZQuantity.Visible = true;
			this.colZQuantity.VisibleIndex = 7;
			// 
			// colZPeriod
			// 
			this.colZPeriod.Caption = "Срок годности";
			this.colZPeriod.FieldName = "ZPeriod";
			this.colZPeriod.Name = "colZPeriod";
			this.colZPeriod.OptionsColumn.AllowEdit = false;
			this.colZPeriod.Visible = true;
			this.colZPeriod.VisibleIndex = 8;
			// 
			// tpForb
			// 
			this.tpForb.Controls.Add(this.ForbGridControl);
			this.tpForb.Location = new System.Drawing.Point(4, 22);
			this.tpForb.Name = "tpForb";
			this.tpForb.Size = new System.Drawing.Size(712, 705);
			this.tpForb.TabIndex = 3;
			this.tpForb.Text = "Запрещенные";
			this.tpForb.Visible = false;
			// 
			// ForbGridControl
			// 
			this.ForbGridControl.DataSource = this.dtForb;
			this.ForbGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			// 
			// 
			// 
			this.ForbGridControl.EmbeddedNavigator.Name = "";
			this.ForbGridControl.Location = new System.Drawing.Point(0, 0);
			this.ForbGridControl.MainView = this.gvForb;
			this.ForbGridControl.Name = "ForbGridControl";
			this.ForbGridControl.Size = new System.Drawing.Size(712, 705);
			this.ForbGridControl.TabIndex = 0;
			this.ForbGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvForb});
			// 
			// gvForb
			// 
			this.gvForb.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colFForb});
			this.gvForb.GridControl = this.ForbGridControl;
			this.gvForb.Name = "gvForb";
			this.gvForb.OptionsMenu.EnableColumnMenu = false;
			this.gvForb.OptionsMenu.EnableFooterMenu = false;
			this.gvForb.OptionsMenu.EnableGroupPanelMenu = false;
			this.gvForb.OptionsView.ShowGroupPanel = false;
			// 
			// colFForb
			// 
			this.colFForb.Caption = "Запрещенные";
			this.colFForb.FieldName = "FForb";
			this.colFForb.Name = "colFForb";
			this.colFForb.OptionsColumn.AllowEdit = false;
			this.colFForb.Visible = true;
			this.colFForb.VisibleIndex = 0;
			// 
			// menuItem1
			// 
			this.menuItem1.Checked = true;
			this.menuItem1.Index = -1;
			this.menuItem1.Text = "Упреждающий поиск";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = -1;
			this.menuItem2.Text = "Показывать сопоставление";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = -1;
			this.menuItem3.Text = "Отослать письмо о товаре";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = -1;
			this.menuItem4.Text = "Отослать письмо о изготовителях";
			// 
			// dataGridTextBoxColumn10
			// 
			this.dataGridTextBoxColumn10.Format = "";
			this.dataGridTextBoxColumn10.FormatInfo = null;
			this.dataGridTextBoxColumn10.MappingName = "UEColumn1";
			this.dataGridTextBoxColumn10.Width = 75;
			// 
			// dataGridTextBoxColumn11
			// 
			this.dataGridTextBoxColumn11.Format = "";
			this.dataGridTextBoxColumn11.FormatInfo = null;
			this.dataGridTextBoxColumn11.MappingName = "UEColumn2";
			this.dataGridTextBoxColumn11.Width = 75;
			// 
			// dataGridTextBoxColumn12
			// 
			this.dataGridTextBoxColumn12.Format = "";
			this.dataGridTextBoxColumn12.FormatInfo = null;
			this.dataGridTextBoxColumn12.MappingName = "UEColumn3";
			this.dataGridTextBoxColumn12.Width = 75;
			// 
			// dataGridTextBoxColumn13
			// 
			this.dataGridTextBoxColumn13.Format = "";
			this.dataGridTextBoxColumn13.FormatInfo = null;
			this.dataGridTextBoxColumn13.HeaderText = "Код";
			this.dataGridTextBoxColumn13.MappingName = "UECode";
			this.dataGridTextBoxColumn13.Width = 75;
			// 
			// dataGridTextBoxColumn14
			// 
			this.dataGridTextBoxColumn14.Format = "";
			this.dataGridTextBoxColumn14.FormatInfo = null;
			this.dataGridTextBoxColumn14.HeaderText = "Код производителя";
			this.dataGridTextBoxColumn14.MappingName = "UECodeCr";
			this.dataGridTextBoxColumn14.Width = 75;
			// 
			// dataGridTextBoxColumn15
			// 
			this.dataGridTextBoxColumn15.Format = "";
			this.dataGridTextBoxColumn15.FormatInfo = null;
			this.dataGridTextBoxColumn15.HeaderText = "Наименование";
			this.dataGridTextBoxColumn15.MappingName = "UEName1";
			this.dataGridTextBoxColumn15.Width = 75;
			// 
			// dataGridTextBoxColumn16
			// 
			this.dataGridTextBoxColumn16.Format = "";
			this.dataGridTextBoxColumn16.FormatInfo = null;
			this.dataGridTextBoxColumn16.HeaderText = "Производитель";
			this.dataGridTextBoxColumn16.MappingName = "UEFirmCr";
			this.dataGridTextBoxColumn16.Width = 75;
			// 
			// dataGridTextBoxColumn26
			// 
			this.dataGridTextBoxColumn26.Format = "";
			this.dataGridTextBoxColumn26.FormatInfo = null;
			this.dataGridTextBoxColumn26.HeaderText = "Валюта";
			this.dataGridTextBoxColumn26.MappingName = "UECurrency";
			this.dataGridTextBoxColumn26.Width = 75;
			// 
			// dataGridTextBoxColumn27
			// 
			this.dataGridTextBoxColumn27.Format = "";
			this.dataGridTextBoxColumn27.FormatInfo = null;
			this.dataGridTextBoxColumn27.HeaderText = "Базовая цена";
			this.dataGridTextBoxColumn27.MappingName = "UEBaseCost";
			this.dataGridTextBoxColumn27.Width = 75;
			// 
			// dataGridTextBoxColumn28
			// 
			this.dataGridTextBoxColumn28.Format = "";
			this.dataGridTextBoxColumn28.FormatInfo = null;
			this.dataGridTextBoxColumn28.HeaderText = "Ед. измерения";
			this.dataGridTextBoxColumn28.MappingName = "UEUnit";
			this.dataGridTextBoxColumn28.Width = 75;
			// 
			// dataGridTextBoxColumn29
			// 
			this.dataGridTextBoxColumn29.Format = "";
			this.dataGridTextBoxColumn29.FormatInfo = null;
			this.dataGridTextBoxColumn29.HeaderText = "Цех. уп.";
			this.dataGridTextBoxColumn29.MappingName = "UEVolume";
			this.dataGridTextBoxColumn29.Width = 75;
			// 
			// dataGridTextBoxColumn30
			// 
			this.dataGridTextBoxColumn30.Format = "";
			this.dataGridTextBoxColumn30.FormatInfo = null;
			this.dataGridTextBoxColumn30.HeaderText = "Количество";
			this.dataGridTextBoxColumn30.MappingName = "UEQuantity";
			this.dataGridTextBoxColumn30.Width = 75;
			// 
			// dataGridTextBoxColumn31
			// 
			this.dataGridTextBoxColumn31.Format = "";
			this.dataGridTextBoxColumn31.FormatInfo = null;
			this.dataGridTextBoxColumn31.HeaderText = "Срок годности";
			this.dataGridTextBoxColumn31.MappingName = "UEPeriod";
			this.dataGridTextBoxColumn31.Width = 75;
			// 
			// dataGridTextBoxColumn32
			// 
			this.dataGridTextBoxColumn32.Format = "";
			this.dataGridTextBoxColumn32.FormatInfo = null;
			this.dataGridTextBoxColumn32.HeaderText = "Признак";
			this.dataGridTextBoxColumn32.MappingName = "UEJunk";
			this.dataGridTextBoxColumn32.Width = 75;
			// 
			// colCColumn11
			// 
			this.colCColumn11.FieldName = "CColumn1";
			this.colCColumn11.Name = "colCColumn11";
			this.colCColumn11.Visible = true;
			this.colCColumn11.VisibleIndex = 0;
			// 
			// MainTimer
			// 
			this.MainTimer.Enabled = true;
			this.MainTimer.Interval = 15000;
			this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
			// 
			// frmUEEMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(720, 753);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.statusBar1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.KeyPreview = true;
			this.Name = "frmUEEMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Редактор нераспознанных выражений";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmUEEMain_Closing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmUEEMain_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.gvCatForm)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.CatalogGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtJobs)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtUnrecExp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCatalogName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtForm)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtZero)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtForb)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtRegions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCatalogFirmCr)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtCurrency)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dtSections)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvCatalog)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpAll)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpCurrent)).EndInit();
			this.panel3.ResumeLayout(false);
			this.tcMain.ResumeLayout(false);
			this.tpJobs.ResumeLayout(false);
			this.pnlCenter1.ResumeLayout(false);
			this.pnlTop1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.JobsGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvJobs)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.pnlWithButton1.ResumeLayout(false);
			this.tpUnrecExp.ResumeLayout(false);
			this.pnlBottom2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.UnrecExpGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvUnrecExp)).EndInit();
			this.pnlCenter2.ResumeLayout(false);
			this.pnlLeft2.ResumeLayout(false);
			this.grpBoxCatalog2.ResumeLayout(false);
			this.pnlTop2.ResumeLayout(false);
			this.tpZero.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ZeroGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvZero)).EndInit();
			this.tpForb.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ForbGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvForb)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmUEEMain());	
		}

		private void SaveTableStyle(StreamWriter sr, GridControl dt)
		{
			foreach(GridColumn gc in ((GridView)dt.DefaultView).Columns)
			{
				sr.WriteLine(gc.Width);
			}
		}

		private void LoadTableStyle(StreamReader sr, GridControl dt)
		{
			string line;
			foreach(GridColumn dc in ((GridView)dt.DefaultView).Columns)
			{
				line=sr.ReadLine();
				dc.Width = Convert.ToInt32(line);
			}
		}

		private void frmUEEMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (tcMain.SelectedTab == tpUnrecExp)
			{
				DialogResult DRes;
				DRes = MessageBox.Show("Вы находитесь в режиме редактирования прайс-листа. Сохранить изменения?", "Вопрос", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				UnlockJob(DRes);
				if (DRes == DialogResult.Cancel)
				{
					e.Cancel = true;
					return;
				}
			}
			SaveColor(btnJobsBlock);
			SaveColor(btnJobsNamePos);
			SaveColor(btnJobs50);

			JobsGridControl.MainView.SaveLayoutToRegistry(JregKey);
			CatalogGridControl.MainView.SaveLayoutToRegistry(CregKey);
			UnrecExpGridControl.MainView.SaveLayoutToRegistry(UEregKey);
			ForbGridControl.MainView.SaveLayoutToRegistry(FregKey);
			ZeroGridControl.MainView.SaveLayoutToRegistry(ZregKey);

			MyCn.Close();
		}

		private void DAJobsCreate()
		{
			daJobs = new MySqlDataAdapter(
				@"
SELECT  PD.FirmCode as JFirmCode, 
        PD.PriceCode                                                                                                         As JPriceCode, 
        concat(CD.ShortName, '(', if(pc.PriceCode = pc.ShowPriceCode, pd.PriceName, concat('[Колонка] ', pc.CostName)), ')') as JName, 
        regions.region                                                                                                       As JRegion, 
        FormRules.DateCurPrice                                                                                               AS JPriceDate, 
        FormRules.MaxOld, 
        0                                                                                       AS JPos, 
        0                                                                                       AS JNamePos, 
        ''                                                                                      AS JJobDate, 
        CD.FirmSegment                                                                          As JWholeSale, 
        bp.BlockBy                                                                              As JBlockBy, 
        FormRules.ParentSynonym                                                                 as JParentSynonym, 
        FormRules.PriceFmt                                                                      As JPriceFMT, 
        pfmt.FileExtention                                                                      as JExt, 
        CD.Phone                                                                                As JPhone, 
        PD.MinReq                                                                               As JMinReq, 
        0                                                                                       AS JNeedRetrans, 
        0                                                                                       AS JRetranced, 
        FormRules.DateLastForm                                                                  AS JDateLastForm, 
        if(FormRules.ParentSynonym is null, '', concat(pcd.ShortName, '(', ppd.PriceName, ')')) as JParentName 
FROM ( usersettings.ClientsData AS CD, FormRules, PriceFMTs as pfmt, usersettings.pricesdata AS PD, regions, usersettings.pricescosts pc ) 
LEFT JOIN blockedprice bp 
ON      bp.PriceCode = PD.PriceCode 
LEFT JOIN usersettings.pricesdata ppd 
ON      ppd.pricecode = FormRules.ParentSynonym 
LEFT JOIN usersettings.clientsdata pcd 
ON      pcd.FirmCode       = ppd.firmcode 
WHERE   FormRules.firmcode =PD.pricecode 
    AND FormRules.PriceFmt = pfmt.Format 
    AND CD.firmcode        =PD.firmcode 
    AND regions.regioncode =CD.regioncode 
    AND pd.agencyenabled   =1 
    AND exists(SELECT * FROM UnrecExp un WHERE un.FirmCode = PD.PriceCode) 
    AND pc.PriceCode = pd.PriceCode 
GROUP BY PD.pricecode", 
				MyCn);
		}

		private void JobsGridFill()
		{
			long CurrPriceCode = -1;
			if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow drJ = gvJobs.GetDataRow(gvJobs.FocusedRowHandle);
				if (drJ != null)
					CurrPriceCode = Convert.ToInt64(drJ["JPriceCode"]);
			}

			dtJobs.Clear();
			
			JobsGridControl.BeginUpdate();
			try
			{
				daJobs.Fill(dtJobs);

			using (DataTable dt = new DataTable())
			{
				foreach(DataRow dr in dtJobs.Rows)
				{
					dt.Clear();
                    MyCmd.Parameters.Clear();
                    MyCmd.Parameters.AddWithValue("?JPriceCode", dr["JPriceCode"]);
			
					MyCmd.CommandText = 
						@"SELECT 
							COUNT(*) AS JPos, 
							COUNT(IF(TmpFullCode=0 OR TmpCurrency='' ,TmpFullCode,NULL)) AS JNamePos,  
							MAX(AddDate) AS JJobDate
						FROM 
							UnrecExp 
						WHERE FirmCode = ?JPriceCode 
							GROUP BY FirmCode";

					MyDA.Fill(dt);

					if (dt.Rows.Count > 0 )
					{
						dr["JPos"] = dt.Rows[0]["JPos"];
						dr["JNamePos"] = dt.Rows[0]["JNamePos"];
						dr["JJobDate"] = dt.Rows[0]["JJobDate"];
					}

                    dt.Clear();

                    MyCmd.CommandText =
                        @"SELECT 
							MAX(LogTime) AS JSynonymDate
						FROM 
							logs.synonymlogs 
						WHERE PriceCode = ?JPriceCode";

                    object JSynonymDate = MyCmd.ExecuteScalar();

                    MyCmd.CommandText =
                        @"SELECT 
							MAX(LogTime) AS JSynonymFirmCrDate
						FROM 
							logs.synonymfirmcrlogs 
						WHERE PriceCode = ?JPriceCode";

                    object JSynonymFirmCrDate = MyCmd.ExecuteScalar();

                    MyCmd.CommandText =
                        @"SELECT 
							MAX(LogTime) AS JPricesRetrans
						FROM 
							logs.pricesretrans 
						WHERE PriceCode = ?JPriceCode";

                    object JPricesRetrans = MyCmd.ExecuteScalar();

                    //if (dt.Rows.Count > 0)
                    //{
                        try
                        {
                            if (Convert.ToDateTime(dr["JDateLastForm"]) < Convert.ToDateTime(JPricesRetrans))
                                dr["JRetranced"] = 1;
                        }
                        catch
                        {
                            dr["JRetranced"] = 0;
                        }

                        if ((JSynonymDate is DBNull) && (JSynonymFirmCrDate is DBNull))
                        {
                            dr["JNeedRetrans"] = 0;
                        }
                        else
                        {
                            if (!(JSynonymDate is DBNull) && !(JSynonymFirmCrDate is DBNull))
                            {
                                if (Convert.ToDateTime(dr["JDateLastForm"]) < Convert.ToDateTime(JSynonymFirmCrDate))
                                    dr["JNeedRetrans"] = 1;
                                if (Convert.ToDateTime(dr["JDateLastForm"]) < Convert.ToDateTime(JSynonymDate))
                                    dr["JNeedRetrans"] = 1;
                            }
                            else
                            {
                                if (!(JSynonymDate is DBNull))
                                {
                                    if (Convert.ToDateTime(dr["JDateLastForm"]) < Convert.ToDateTime(JSynonymDate))
                                        dr["JNeedRetrans"] = 1;
                                }
                                else if (!(JSynonymFirmCrDate is DBNull))
                                {
                                    if (Convert.ToDateTime(dr["JDateLastForm"]) < Convert.ToDateTime(JSynonymFirmCrDate))
                                        dr["JNeedRetrans"] = 1;
                                }
                            }
                        }
                    }
				//}
			}
			}
			finally
			{
				JobsGridControl.EndUpdate();
			}

			LocateJobs(CurrPriceCode);
			statusBar1.Panels[0].Text = "Заданий в очереди: " + dtJobs.Rows.Count;
		}

		private void UnrecExpGridFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA)
		{
			dtUnrecExp.Clear();

			MyCmd.CommandText = 
				@"SELECT RowID As UERowID,
                  Name1 As UEName1, 
				  Name2 AS UEName2, 
				  Name3 As UEName3, 
				  FirmCr As UEFirmCr, 
				  CountryCr, 
				  Code As UECode, 
				  CodeCr As UECodeCr, 
				  Unit As UEUnit, 
				  Volume As UEVolume, 
				  Quantity As UEQuantity, 
				  Note, 
				  Period As UEPeriod, 
				  Doc, 
				  BaseCost As UEBaseCost, 
				  Currency As UECurrency, 
				  TmpFullCode As UETmpFullCode,  
				  TmpCodeFirmCr As UETmpCodeFirmCr, 
				  TmpCurrency, 
				  Status As UEStatus,
                  Already As UEAlready, 
				  Junk As UEJunk,
				  HandMade As UEHandMade
				  FROM UnrecExp 
				  WHERE FirmCode= ?LockedPriceCode ORDER BY Name1";

			MyCmd.Parameters.Clear();
			MyCmd.Parameters.AddWithValue("?LockedPriceCode", LockedPriceCode);
			
			UnrecExpGridControl.BeginUpdate();
			try
			{
				MyDA.Fill(dtUnrecExp);
			}
			finally
			{
				UnrecExpGridControl.EndUpdate();
			}
		}

		private void CurrencyGridFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA)
		{
			dtCurrency.Clear();
			MyCmd.CommandText = 
				@"SELECT 
					Currency As CCurrency, 
					Exchange As CExchange
				FROM
					CatalogCurrency";
			MyDA.Fill(dtCurrency);
		}

		private void CatalogNameGridFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA)
		{
			//dataSet1.Tables["CatalogNameGrid"].Clear();
			dtCatalogName.Clear();
			MyCmd.CommandText = 
				@"SELECT 
					ShortCode As CCode, 
					Name As CName
				FROM 
					Catalog
				WHERE
					Hide=0
				Group by ShortCode
";

			MyDA.Fill(dtCatalogName);
		}
		
		private void CatalogFirmCrGridFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA)
		{
			//dataSet1.Tables["CatalogFirmCrGrid"].Clear();
			dtCatalogFirmCr.Clear();
			MyCmd.CommandText = 
				@"SELECT 
					CodeFirmCr As CCode, 
					FirmCr As CName 
				FROM CatalogFirmCr
                where CodeFirmCr <> 1 
				Order By FirmCr";

			MyDA.Fill(dtCatalogFirmCr);

		}

		private void FormGridFill(MySqlCommand MyCmd, MySqlDataAdapter MyDA)
		{
			//DataRow dr = ((GridView)UnrecExpGridControl.DefaultView).GetDataRow(((GridView)UnrecExpGridControl.DefaultView).FocusedRowHandle);
			//	dataSet1.Tables["FormGrid"].Clear();
			dtForm.Clear();
			MyCmd.CommandText = 
				@"SELECT
					ShortCode AS FShortCode,
					FullCode As FFullCode, 
					Form AS FForm,
                    Name As FName
				FROM
					Catalog
				WHERE
					Hide=0
                order by shortcode, fullcode, form";
			//WHERE FullCode = ?TmpFullCOde";
			//MyCmd.Parameters.Clear();
			//MyCmd.Parameters.AddWithValue("?TmpFullCode", dr[UETmpFullCode]);

			MyDA.Fill(dtForm);
		}

		private void UpdateCatalog()
		{
			CatalogGridControl.BeginUpdate();
			try
			{
				dtForm.Clear();
				CurrencyGridFill(MyCmd, MyDA);
				CatalogFirmCrGridFill(MyCmd, MyDA);
				CatalogNameGridFill(MyCmd, MyDA);
				FormGridFill(MyCmd, MyDA);
			}
			finally
			{
				CatalogGridControl.EndUpdate();
			}
		}

		private void btnDelJob_Click(object sender, System.EventArgs e)
		{
			bool res = false;
			if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				if (MessageBox.Show("Вы действительно хотите удалить задание?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					do
					{
						DataRow JobsDR = gvJobs.GetDataRow(gvJobs.FocusedRowHandle);
						try
						{
							MySqlTransaction tran = MyCn.BeginTransaction();
							MySqlCommand cmdDeleteJob = new MySqlCommand(										
								@"DELETE FROM UnrecExp
							WHERE FirmCode = ?PriceCode
							AND not exists(select * from blockedprice bp where bp.PriceCode = FirmCode)", 
								MyCn, tran);
							cmdDeleteJob.Parameters.AddWithValue("?PriceCode", JobsDR["JPriceCode"]);
							cmdDeleteJob.ExecuteNonQuery();
							tran.Commit();
							res = true;
							MessageBox.Show("Задание удалено!");
						}
						catch(MySqlException ex)
						{
							MessageBox.Show(ex.ToString());
						}
					}
					while(!res);
				}
				JobsGridFill();
			}
		}
		
		private void JobsGridControl_DoubleClick(object sender, System.EventArgs e)
		{
			if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(gvJobs.FocusedRowHandle);
				if (dr[colJBlockBy.FieldName].ToString() == String.Empty || dr[colJBlockBy.FieldName].ToString() == Environment.UserName)
					LockJob();
			}
		}

		private void tcMain_SelectedIndexChanged(object sender, System.EventArgs e)
		{	
			if (tcMain.SelectedTab == tpJobs)
			{
				JobsGridControl.Select();
			}

			if (tcMain.SelectedTab == tpUnrecExp)
				UnrecExpGridControl.Select();

			if (tcMain.SelectedTab == tpForb)
			{
				ForbGridControl.Select();
				dtForb.Clear();

				MyCmd.CommandText = 
					@"SELECT Forb As FForb 
					            FROM Forb 
					            WHERE FirmCode= ?JPriceCode";
				MyCmd.Parameters.Clear();
				MyCmd.Parameters.AddWithValue("?JPriceCode", LockedPriceCode);

				MyDA.Fill(dtForb);

			}

			if (tcMain.SelectedTab == tpZero)
			{
				ZeroGridControl.Select();

				dataSet1.Tables["ZeroGrid"].Clear();

				MyCmd.CommandText = 
					@"SELECT 
									Code As ZCode, 
									CodeCr As ZCodeCr, 
									Name As ZName, 
									FirmCr As ZFirmCr, 
									Currency As ZCurrency, 
									Unit As ZUnit, 
									Volume AS ZVolume, 
									Quantity As ZQuantity, 
									Period As ZPeriod
								FROM Zero 
								WHERE FirmCode= ?JPriceCode";

				MyCmd.Parameters.Clear();
				MyCmd.Parameters.AddWithValue("?JPriceCode", LockedPriceCode);
				MyDA.Fill(dtZero);
			}

     	}

		private FormMask GetMask(int NumRow, string FieldName)
		{
			DataRow dr=gvUnrecExp.GetDataRow(NumRow);
			FormMask mask = (FormMask)Convert.ToByte(dr[FieldName]);
			return mask;
		}

		private bool NotNameForm(int NumRow, string FieldName)
		{
			FormMask m = GetMask(NumRow, FieldName);
			return (m & FormMask.NameForm) != FormMask.NameForm;
		}

		private bool NotFirmForm(int NumRow, string FieldName)
		{
			FormMask m = GetMask(NumRow, FieldName);
			return (m & FormMask.FirmForm) != FormMask.FirmForm;
		}

		private bool NotCurrForm(int NumRow, string FieldName)
		{
			FormMask m = GetMask(NumRow, FieldName);
			return (m & FormMask.CurrForm) != FormMask.CurrForm;
		}

		private bool MarkForbidden(int NumRow, string FieldName)
		{
			FormMask m = GetMask(NumRow, FieldName);
			return (m & FormMask.MarkForb) == FormMask.MarkForb;
		}

		private string GetFilterString(string Value)
		{
			string[] flt = Value.Split(' ');
			ArrayList newflt = new ArrayList();
			for(int i=0;i<flt.Length;i++)
			{
				if (flt[i].Length >=4)
					newflt.Add( PrepareArg( flt[i].Substring(0, 4).Replace("'", "''") ) );
			}
			string[] flt2 = new string[newflt.Count];
			newflt.CopyTo(flt2);
			return "[CName] like '" + String.Join("%' or [CName] like '", flt2) + "%'";
		}

		private void ShowCatalog(int FocusedRowHandle)
		{
			DataRow dr=gvUnrecExp.GetDataRow(FocusedRowHandle);
			grpBoxCatalog2.Text = "Каталог товаров";
			CatalogGridControl.DataMember = "CatalogNameGrid";

			gvCatalog.ActiveFilter.Clear();
			gvCatalog.ActiveFilter.Add(gvCatalog.Columns["CName"], new ColumnFilterInfo( GetFilterString( GetFullUnrecName(FocusedRowHandle) ) , ""));
			if (gvCatalog.DataRowCount == 0)
				gvCatalog.ActiveFilter.Clear();
		}

		private void ShowCatalogFirmCr(int FocusedRowHandle)
		{
			DataRow dr=gvUnrecExp.GetDataRow(FocusedRowHandle);
			grpBoxCatalog2.Text = "Каталог фирм производителей";
			CatalogGridControl.DataMember = "CatalogFirmCrGrid";
			
			if (dr["UEFirmCr"].ToString() != String.Empty)
			{
				gvCatalog.ActiveFilter.Clear();
				gvCatalog.ActiveFilter.Add(gvCatalog.Columns["CName"], new ColumnFilterInfo(GetFilterString( dr["UEFirmCr"].ToString() ), ""));
				if (gvCatalog.DataRowCount == 0)
					gvCatalog.ActiveFilter.Clear();
			}
			else
			{
				gvCatalog.ActiveFilter.Clear();
			}
		}

		private void MoveToCatalog()
		{
			if (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr=gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
				
				if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb)
				{
					CatalogGridControl.Enabled = false;
					ClearCatalogGrid();
				}
				else
					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.NameForm) != FormMask.NameForm)
				{
					CatalogGridControl.Enabled = true;
					ShowCatalog(gvUnrecExp.FocusedRowHandle);
					CatalogGridControl.Focus();
				}
				else
					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.FirmForm) != FormMask.FirmForm)
				{
					CatalogGridControl.Enabled = true;
					ShowCatalogFirmCr(gvUnrecExp.FocusedRowHandle);
					CatalogGridControl.Focus();
				}
				else
					if( 7 == (int)dr["UEStatus"])
				{
					CatalogGridControl.Enabled = false;
					ClearCatalogGrid();
				}
			}
		}

		private void UnrecExpGridControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				bool flag = false;
				DataRow UEdr = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);

				if (e.KeyCode == Keys.Enter)
					MoveToCatalog();

				if (e.KeyCode == Keys.Escape)
					gvUnrecExp.FocusedRowHandle += 1;

				if (e.KeyCode == Keys.Back)
				{
					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb)
						if(MessageBox.Show("Отменить запрещение?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						{
							UnmarkUnrecExpAsForbidden(gvUnrecExp.FocusedRowHandle);
							return;
						}
						else
							return;

					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.NameForm) == FormMask.NameForm)
					{
						DataRow drUN = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
						if (drUN != null)
						{
							DataRow[] drFM = dtForm.Select("FFullCode = " + drUN[UETmpFullCode].ToString());
							if (drFM.Length > 0)
							{
								string Mess = String.Format("Наименование: {0}\r\nФорма: {1}\r\nОтменить сопоставление по наименованию?", drFM[0]["FName"], drFM[0][FForm]);
								if(MessageBox.Show(Mess, "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
								{
									UnmarkUnrecExpAsNameForm(gvUnrecExp.FocusedRowHandle);
									SetReserved(false, gvUnrecExp.FocusedRowHandle);
									flag = true;
								}
							}
						}
					}

					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.FirmForm) == FormMask.FirmForm)
					{
						DataRow drUN = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
						if (drUN != null)
						{
							string FirmName = String.Empty;
							if ((Int64)drUN[UETmpCodeFirmCr] == 1)
								FirmName = "-";
							else
							{
								DataRow[] drFM = dtCatalogFirmCr.Select("CCode = " + drUN[UETmpCodeFirmCr].ToString());
								if (drFM.Length > 0)
								{
									FirmName = drFM[0]["CName"].ToString();
								}
							}
							if(FirmName != String.Empty && MessageBox.Show("Производитель: " + FirmName+ "\r\nОтменить сопоставление по производителю?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
							{
								UnmarkUnrecExpAsFirmForm(gvUnrecExp.FocusedRowHandle);
								flag = true;
							}
						}
					}

					if ((GetMask(gvUnrecExp.FocusedRowHandle, "UEStatus") & FormMask.CurrForm) == FormMask.CurrForm)
						if(MessageBox.Show("Отменить сопоставление по валюте?", "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						{
							UnmarkUnrecExpAsCurrForm(gvUnrecExp.FocusedRowHandle);
							flag = true;
						}

					if (flag)
						MoveToCatalog();
//					else
//						if ((7 == (int)UEdr["UEStatus"]))
//							GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle+1);
				}

				if (e.KeyCode == Keys.F2 && (byte)UEdr["UEHandMade"] != 1)
				{
					MarkUnrecExpAsForbidden(UEdr);
					GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle+1);
				}
			}
		}

		private void SetReserved(bool reserved, int FocusedRowHandle)
		{
			DataRow drUnrecExp = gvUnrecExp.GetDataRow(FocusedRowHandle);
			drUnrecExp["UEJunk"]=Convert.ToByte(reserved);
		}

		private void UnmarkUnrecExpAsNameForm(int FocusedRowHandle)
		{
			try
			{
				if ((GetMask(FocusedRowHandle, "UEStatus") & FormMask.NameForm) == FormMask.NameForm)
				{
					DataRow drUnrecExp = gvUnrecExp.GetDataRow(FocusedRowHandle);
					drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) & (~FormMask.NameForm));
					drUnrecExp["UETmpFullCode"]=0;
				}
			}
			catch
			{
			}
		}

		private void UnmarkUnrecExpAsFirmForm(int FocusedRowHandle)
		{
			try
			{
				if ((GetMask(FocusedRowHandle, "UEStatus") & FormMask.FirmForm) == FormMask.FirmForm)
				{
					DataRow drUnrecExp = gvUnrecExp.GetDataRow(FocusedRowHandle);
					drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) & (~FormMask.FirmForm));
					drUnrecExp["UETmpCodeFirmCr"]=0;
				}
			}
			catch
			{
			}
		}

		private void UnmarkUnrecExpAsCurrForm(int FocusedRowHandle)
		{
			try
			{
				if ((GetMask(FocusedRowHandle, "UEStatus") & FormMask.CurrForm) == FormMask.CurrForm)
				{
					DataRow drUnrecExp = gvUnrecExp.GetDataRow(FocusedRowHandle);
					drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) & (~FormMask.CurrForm));
					drUnrecExp["UETmpCurrency"]=0;
				}
			}
			catch
			{
			}
		}

		private void UnmarkUnrecExpAsForbidden(int FocusedRowHandle)
		{
			if ((GetMask(FocusedRowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb)
			{
				DataRow drUnrecExp = gvUnrecExp.GetDataRow(FocusedRowHandle);
				drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) & (~FormMask.MarkForb));
			}
		}

		private void MainTimer_Tick(object sender, System.EventArgs e)
		{
			if (tcMain.SelectedTab == tpJobs && LockedPriceCode == -1)
			{
//				DateTime n = DateTime.Now;
//				try
//				{
//					Trace.Write(n);
//					Trace.WriteLine(" Start JobsGridFill");
					JobsGridFill();
//					Trace.Write(n);
//					Trace.WriteLine(" Stop  JobsGridFill");
//				}
//				catch(Exception ex)
//				{
//					//There is already an open DataReader associated with this Connection which must be closed first.
//
//					Trace.WriteLine(String.Format("Error on MainTimer : {0}", ex));
//					//MessageBox.Show(ex.ToString());
//				}
			}
		}

		private void MarkUnrecExpAsNameForm(DataRow drUnrecExp, bool MarkAsJUNK)
		{
			if (((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) &  FormMask.NameForm) != FormMask.NameForm)
			{
				//TODO: Здесь потребуется завести дополнительный столбец в таблицу нераспознанных выражений
				drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) | FormMask.NameForm);
				drUnrecExp["UEJunk"] = Convert.ToByte(MarkAsJUNK);
				GridView bv = (GridView)gvCatalog.GetDetailView(gvCatalog.FocusedRowHandle,0);
				drUnrecExp["UETmpFullCode"] = bv.GetDataRow(bv.FocusedRowHandle)["FFullCode"];
			}
		}

		private void MarkUnrecExpAsFirmForm(DataRow drUnrecExp)
		{
			DataRow drCatalogFirmCr = gvCatalog.GetDataRow(gvCatalog.FocusedRowHandle);

			if (((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) &  FormMask.FirmForm) != FormMask.FirmForm)
			{
				drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) | FormMask.FirmForm);
				drUnrecExp["UETmpCodeFirmCr"] = gvCatalog.GetDataRow(gvCatalog.FocusedRowHandle)["CCode"];
			}
		}

		private void MarkUnrecExpAsCurrForm(DataRow drUnrecExp)
		{
			if (((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) &  FormMask.CurrForm) != FormMask.CurrForm)
			{
				drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) | FormMask.CurrForm);
				drUnrecExp["UETmpCurrency"] = gvCatalog.GetDataRow(gvCatalog.FocusedRowHandle)["CCode"];
			}
		}

		private void MarkUnrecExpAsForbidden(DataRow drUnrecExp)
		{
			if (((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) &  FormMask.MarkForb) != FormMask.MarkForb)
				drUnrecExp["UEStatus"] = (int)((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) | FormMask.MarkForb);
		}


		private string PrepareArg(string source)
		{
			string res = String.Empty;
			foreach(char c in source)
			{
				if (c == '*' || c == '%' || c == '[' || c == ']')
					res += String.Format("[{0}]", c);
				else
					res += Char.ToUpper(c).ToString();
			}
			return res;
		}

		private void CatalogGridControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//Снимаем фильтр при поиске
			if ( (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || e.KeyCode == Keys.OemCloseBrackets || 
				e.KeyCode == Keys.OemOpenBrackets || e.KeyCode == Keys.OemSemicolon || e.KeyCode == Keys.OemQuotes || 
				e.KeyCode == Keys.Oemcomma || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.OemQuestion)
			{
				if (CatalogGridControl.FocusedView.LevelName == String.Empty)
					gvCatalog.ActiveFilter.Clear();
				else
					gvCatForm.ActiveFilter.Clear();
			}
			if (CatalogGridControl.DataMember == "CatalogNameGrid")
			{
				if (CatalogGridControl.FocusedView.LevelName == String.Empty)
				{
					if (e.KeyCode == Keys.Enter)
						if (gvCatalog.FocusedRowHandle != GridControl.InvalidRowHandle)
						{
							gvCatalog.ExpandMasterRow(gvCatalog.FocusedRowHandle);
							GridView bv = (GridView)gvCatalog.GetDetailView(gvCatalog.FocusedRowHandle, 0);
							bv.ZoomView();
							bv.MoveFirst();
							CatalogGridControl.FocusedView = bv;
							string[] flt = (GetFullUnrecName(gvUnrecExp.FocusedRowHandle)).Split(' ');
							ArrayList newflt = new ArrayList();
							for(int i=0;i<flt.Length;i++)
							{
								if (flt[i] != null && flt[i] != String.Empty)
									newflt.Add( PrepareArg( flt[i].Replace("'", "''") ) );
							}
							string[] flt2 = new string[newflt.Count];
							newflt.CopyTo(flt2);
							gvCatForm.ActiveFilter.Clear();					
							gvCatForm.ActiveFilter.Add(gvCatForm.Columns["FForm"], new ColumnFilterInfo("[FForm] like '%" + String.Join("%' or [FForm] like '%", flt2) + "%'", ""));
							//if (gvCatForm.DataRowCount == 0)
								gvCatForm.ActiveFilter.Clear();
							DataRow dr = gvCatalog.GetDataRow(gvCatalog.FocusedRowHandle);
							colFForm.Caption = dr[colCName.FieldName].ToString();
						}

					if (e.KeyCode == Keys.Escape)
					{
						ClearCatalogGrid();
						GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle+1);
					}

					if (e.KeyCode == Keys.A && e.Control)
						gvCatalog.ActiveFilter.Clear();

					if (e.KeyCode == Keys.F2 && (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle))
					{
						DataRow UEdr = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);

						if ((byte)UEdr["UEHandMade"] != 1)
						{
							MarkUnrecExpAsForbidden(UEdr);
							GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle+1);
						}
					}
				}
				else
				{
					if (e.KeyCode == Keys.Escape)
						gvCatalog.CollapseMasterRow(gvCatalog.FocusedRowHandle);

					if (e.KeyCode == Keys.A && e.Control)
						gvCatForm.ActiveFilter.Clear();
					
					if (e.KeyCode == Keys.Enter)
					{
						if (((GridView)CatalogGridControl.FocusedView).FocusedRowHandle != GridControl.InvalidRowHandle)
						{
							DoSynonym(e.Shift);
							ChangeBigName(gvUnrecExp.FocusedRowHandle);
						}
					}
				}
			}
			else 
			{
				if (e.KeyCode == Keys.Enter)
					if (gvCatalog.FocusedRowHandle != GridControl.InvalidRowHandle)
					{
						DataRow drUnrecExp = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);
						if (((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) &  FormMask.FirmForm) != FormMask.FirmForm)
						{
							DoSynonymFirmCr();
							ChangeBigName(gvUnrecExp.FocusedRowHandle);
						}
						else
							if (((FormMask)Convert.ToByte(drUnrecExp["UEStatus"]) &  FormMask.CurrForm) != FormMask.CurrForm)
						{
							DoSynonymCurrency();
							ChangeBigName(gvUnrecExp.FocusedRowHandle);
						}
					}
				if (e.KeyCode == Keys.Escape)
				{
					ClearCatalogGrid();
					GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle+1);
				}

				if (e.KeyCode == Keys.A && e.Control)
					gvCatalog.ActiveFilter.Clear();

				if (e.KeyCode == Keys.F2 && (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle))
				{
					DataRow UEdr = gvUnrecExp.GetDataRow(gvUnrecExp.FocusedRowHandle);

					if ((byte)UEdr["UEHandMade"] != 1)
					{
						MarkUnrecExpAsForbidden(UEdr);
						GoToNextUnrecExp(gvUnrecExp.FocusedRowHandle+1);
					}
				}

			}
		}

		private void ClearCatalogGrid()
		{
			UnrecExpGridControl.Focus();
			CatalogGridControl.DataMember = null;
			grpBoxCatalog2.Text = "Каталог";
			CatalogGridControl.Enabled = false;
		}

		private void GoToNextUnrecExp(int FromFocusHandle)
		{
			for(int i = FromFocusHandle; i < gvUnrecExp.RowCount; i++)
			{
				if (i != GridControl.InvalidRowHandle)
					if (((GetMask(i, "UEStatus") & FormMask.NameForm) != FormMask.NameForm) || ((GetMask(i, "UEStatus") & FormMask.FirmForm) != FormMask.FirmForm))
					{
						gvUnrecExp.FocusedRowHandle = i;
						break;
					}
			}
			ClearCatalogGrid();
			MoveToCatalog();
		}

		private void DoSynonym(bool MarkAsJUNK)
		{
			int CurrentFocusHandle = gvUnrecExp.FocusedRowHandle;
			string TmpName = GetFullUnrecName(CurrentFocusHandle);
			for(int i = 0; i < gvUnrecExp.RowCount; i++)
			{
				DataRow dr = gvUnrecExp.GetDataRow(i);
				if (dr != null)
				{
					string drName = GetFullUnrecName(i);
					if (drName == TmpName)
					{
						MarkUnrecExpAsNameForm(dr, MarkAsJUNK);

						if (String.Empty == dr["UEFirmCr"].ToString())
						{
							dr["UEStatus"] = (int)((FormMask)Convert.ToByte(dr["UEStatus"]) | FormMask.NameForm);
							//TODO: Здесь надо корректно 
							dr["UETmpCodeFirmCr"] = 1;
						}
					}
				}
			}
			GoToNextUnrecExp(CurrentFocusHandle);
		}

		private void DoSynonymFirmCr()
		{
			int CurrentFocusHandle = gvUnrecExp.FocusedRowHandle;
			string TmpFirm = GetFirmCr(CurrentFocusHandle);
			for(int i = 0; i < gvUnrecExp.RowCount; i++)
			{
				DataRow dr = gvUnrecExp.GetDataRow(i);
				if (dr != null)
				{
					string drFirm = GetFirmCr(i);
					if (drFirm == TmpFirm)
					{
						MarkUnrecExpAsFirmForm(dr);
					}
				}
			}
			//			MessageBox.Show("Проведено сопоставление по производителю");
			GoToNextUnrecExp(CurrentFocusHandle);
		}

		private void DoSynonymCurrency()
		{
			int CurrentFocusHandle = gvUnrecExp.FocusedRowHandle;
			string TmpCurr = GetCurrency(CurrentFocusHandle);
			for(int i = 0; i < gvUnrecExp.RowCount; i++)
			{
				DataRow dr = gvUnrecExp.GetDataRow(i);
				if (dr != null)
				{
					string drFirm = GetCurrency(i);
					if (drFirm == TmpCurr)
					{
						MarkUnrecExpAsCurrForm(dr);
					}
				}
			}
			GoToNextUnrecExp(CurrentFocusHandle);
		}

		private string GetFirmCr(int FocusedRowHandle)
		{
			if (FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvUnrecExp.GetDataRow(FocusedRowHandle);
				if (dr != null)
					return dr["UEFirmCr"].ToString();
				else
					return String.Empty;
			}
			else
				return String.Empty;
		}

		private string GetCurrency(int FocusedRowHandle)
		{
			if (FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvUnrecExp.GetDataRow(FocusedRowHandle);
				if (dr != null)
					return dr["UECurrency"].ToString();
				else
					return String.Empty;
			}
			else
				return String.Empty;
		}

		private void JobsGridControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
					//Начинае разбор нераспознанных выражений
				case Keys.Enter:
					if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
						LockJob();
					break;

					//Обновляем таблицу заданий вручную
				case Keys.F5:
					JobsGridFill();
					break;
			}
		}

		private void gvJobs_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(e.RowHandle);
				if (dr != null)
				{
					if (dr["JBlockBy"].ToString() != "")
					{
						e.Appearance.BackColor = btnJobsBlock.BackColor;
					}
					else
					{
						if ((System.Int64)dr["JPos"] == 0)
							return;

						if ((System.Int64)dr["JNamePos"] == 0)
							e.Appearance.BackColor = btnJobsNamePos.BackColor;

						double jnp = Convert.ToDouble(dr["JNamePos"]), jp = Convert.ToDouble(dr["JPos"]);

						if( ( ( jnp/(jp+1) ) * 100) < 50 )
							e.Appearance.BackColor = btnJobs50.BackColor;
					}
				}	
			}
		}

		//Блокируем задачу
		private void LockJob()
		{
			JobsGridFill();
			if (gvJobs.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(gvJobs.FocusedRowHandle);
				if (dr[colJBlockBy.FieldName].ToString() == String.Empty || dr[colJBlockBy.FieldName].ToString() == Environment.UserName)
				{
					LockedPriceCode = Convert.ToInt64(dr["JPriceCode"]);
                    PriceFMT = dr[JPriceFMT].ToString();
                    FileExt = dr[JExt].ToString();
                    if (dr[JParentSynonym] is DBNull)
						LockedSynonym = LockedPriceCode;
					else
						LockedSynonym = Convert.ToInt64(dr[JParentSynonym]);
					LockedInBlockedPrice(LockedPriceCode, Environment.UserName);
					CatalogGridControl.DataMember = "";
					grpBoxCatalog2.Text = "Каталог";

					tcMain.TabPages.Add(tpUnrecExp);
					tcMain.TabPages.Add(tpZero);
					tcMain.TabPages.Add(tpForb);

					Text += String.Format("   --   {0}  --  {1}", LockedPriceCode.ToString(), dr[colJName.FieldName].ToString());
					
					tcMain.TabPages.Remove(tpJobs);

					tcMain.SelectedTab = tpUnrecExp;

					UnrecExpGridFill(MyCmd, MyDA);
					UnrecExpGridControl.Focus();
					gvUnrecExp.FocusedRowHandle = GridControl.InvalidRowHandle;
					gvUnrecExp.FocusedRowHandle = 0;
					GoToNextUnrecExp(0);
					sbpAll.Text = String.Format("Общее количество: {0}", dtUnrecExp.Rows.Count);
				}
			}
		}

		//Разблокируем задачу
		private void UnlockJob(DialogResult DRes)
		{  
			switch (DRes)
			{
				case DialogResult.Yes:
				{
					f = new frmProgress();

					Thread t = new Thread( new ThreadStart( ThreadMethod ) );
					t.Start();

					DialogResult dr = f.ShowDialog();
					f = null;

					if (dr == DialogResult.Cancel)
						t.Abort();
					
					goto case DialogResult.No;
				}
				case DialogResult.No:
				{
					tcMain.TabPages.Add(tpJobs);
					tcMain.TabPages.Remove(tpUnrecExp);
					tcMain.TabPages.Remove(tpZero);
					tcMain.TabPages.Remove(tpForb);
					tcMain.SelectedTab = tpJobs;
					JobsGridControl.Select();
					LocateJobs(LockedPriceCode);
					UnLockedInBlockedPrice(LockedPriceCode);
					LockedPriceCode = -1;
                    PriceFMT = String.Empty;
                    FileExt = String.Empty;
                    LockedSynonym = -1;
					gvUnrecExp.FocusedRowHandle = GridControl.InvalidRowHandle;
					dtUnrecExp.Clear();
					//Обновляем таблицу заданий
					JobsGridFill();
					sbpAll.Text = String.Empty;
					sbpCurrent.Text = String.Empty;
					Text = "Редактор нераспознанных выражений";
					return;
				}
				case DialogResult.Cancel:
				{
					return;
				}
			}
		}

		private void ThreadMethod()
		{
			try
			{
				ApplyChanges();
				f.Stop = true;
			}
			catch(ThreadAbortException)
			{
				//MessageBox.Show("Поймали исключение ThreadAbortException : {0}", e.ToString());
			}
			catch(Exception e)
			{
				if (f != null)
					f.Error = String.Format("Поймали исключение : {0}", e.ToString());
			}
		}

		delegate bool ShowRetransPriceDelegate();

		private bool ShowRetransPrice()
		{
			string str = String.Empty;
			str = String.Format(
@"Создано:
	запрещённых выражений - {0}
Синонимов:
	по наименованию - {1}
	по производителю - {2}
	по валюте - {3}
Отклонено скрытых синонимов: {4}
Отклонено дублирующихся синонимов: {5}

Перепровести прайс?", ForbiddenCount, SynonymCount, SynonymFirmCrCount, SynonymCurrencyCount, HideSynonymCount, DuplicateSynonymCount);
			return (MessageBox.Show(str, "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes);
		}

		private void ApplyChanges()
		{
			bool res = false;
			//Имеются ли родительские синонимы
			bool HasParentSynonym = LockedSynonym != LockedPriceCode;
			DateTime now = DateTime.Now;
			f.Status = "Подготовка таблиц...";

			//Список прайсов, которые нужно перепровести
			List<RetransedPrice> RetransedPriceList = new List<RetransedPrice>();

			//Если прайс-лист уже имеет родительский синоним, то ищем FileExtention родителя
			//и добавляем родителя первым в списке
			if (LockedSynonym != LockedPriceCode)
			{
				object ParentFileExt = MySqlHelper.ExecuteScalar(MyCn, @"
select 
  pf.FileExtention 
from 
  farm.formrules fr, 
  farm.pricefmts pf,
  usersettings.price_update_info pui 
where 
    fr.FirmCode = ?LockedSynonym 
and pf.Format = fr.PriceFmt
and pui.PriceCode = fr.FirmCode
and (pui.UnformCount > 0)", 
							new MySqlParameter("LockedSynonym", LockedSynonym));
				//Первым в списке добавляем прайс-лист c родительскими синонимами
				if ((ParentFileExt != null) && !(ParentFileExt is DBNull) && (ParentFileExt is String))
					RetransedPriceList.Add(new RetransedPrice(LockedSynonym, (string)ParentFileExt));
			}
			else
				//Первым в списке добавляем сам прайс-лист
				RetransedPriceList.Add(new RetransedPrice(LockedPriceCode, FileExt));

			//Попытка найти всех потомков, которые используют родительские синонимы
			DataSet dsInerPrices = MySqlHelper.ExecuteDataset(MyCn, @"
select
  pd.PriceCode,
  pf.FileExtention
from
  farm.formrules fr,
  usersettings.pricesdata pd,
  usersettings.clientsdata cd,
  usersettings.pricescosts pc,
  usersettings.pricesdata parentpd,
  farm.pricefmts pf,
  usersettings.price_update_info pui
where
    fr.ParentSynonym = ?LockedSynonym
and pd.PriceCode = fr.FirmCode
and pf.Format = fr.PriceFmt
and pd.AgencyEnabled = 1
and cd.FirmCode = pd.FirmCode
and cd.FirmStatus = 1
and cd.BillingStatus = 1
and pc.PriceCode = pd.PriceCode
and parentpd.PriceCode = pc.ShowPriceCode
and ((pc.PriceCode = pc.ShowPriceCode) or (parentpd.CostType = 1))
and pui.PriceCode = pd.PriceCode
and (pui.UnformCount > 0)
",
				new MySqlParameter("LockedSynonym", LockedSynonym));

			//Если в наборе данных будут записи, то добавляем их в список
			if (dsInerPrices.Tables[0].Rows.Count > 0)
			{
				HasParentSynonym = true;
				foreach(DataRow drInerPrice in dsInerPrices.Tables[0].Rows)
					RetransedPriceList.Add(
						new RetransedPrice(
							Convert.ToInt64(drInerPrice["PriceCode"]), 
							drInerPrice["FileExtention"].ToString()));
			}

			//Если в списке нет прайс-листа, с которым происходит работа, то мы его добавляем в список
			if (!RetransedPriceList.Exists(delegate(RetransedPrice value) { return (value.PriceCode == LockedPriceCode);}) )
				RetransedPriceList.Add(new RetransedPrice(LockedPriceCode, FileExt));

			SynonymCount = 0; 
			SynonymFirmCrCount = 0;
			SynonymCurrencyCount = 0;
			ForbiddenCount = 0;
			HideSynonymCount = 0;
			DuplicateSynonymCount = 0;

			//Кол-во удаленных позиций - если оно равно кол-во нераспознанных позиций, то прайс автоматически проводится
			int DelCount = 0;
			
			f.Pr = 1;
			//Заполнение таблиц перед вставкой

			//Заполнили таблицу нераспознанных наименований для обновления
			MySqlDataAdapter daUnrecUpdate = new MySqlDataAdapter("select * from UnrecExp where FirmCode = ?FirmCode", MyCn);
			MySqlCommandBuilder cbUnrecUpdate = new MySqlCommandBuilder(daUnrecUpdate);
			daUnrecUpdate.SelectCommand.Parameters.AddWithValue("?FirmCode", LockedPriceCode);
			DataTable dtUnrecUpdate = new DataTable();
			daUnrecUpdate.Fill(dtUnrecUpdate);
			dtUnrecUpdate.Constraints.Add("UnicNameCode", dtUnrecUpdate.Columns["RowID"], true);

			//Заполнили таблицу синонимов наименований
			MySqlDataAdapter daSynonym = new MySqlDataAdapter("select * from Synonym where FirmCode = ?FirmCode limit 0", MyCn);
			MySqlCommandBuilder cbSynonym = new MySqlCommandBuilder(daSynonym);
			daSynonym.SelectCommand.Parameters.AddWithValue("?FirmCode", LockedSynonym);
			DataTable dtSynonym = new DataTable();
			daSynonym.Fill(dtSynonym);
			dtSynonym.Constraints.Add("UnicNameCode", dtSynonym.Columns["Synonym"], false);
			
			f.Pr += 1;
			//Заполнили таблицу синонимов производителей
			MySqlDataAdapter daSynonymFirmCr = new MySqlDataAdapter("select * from SynonymFirmCr where FirmCode = ?FirmCode limit 0", MyCn);
			MySqlCommandBuilder cbSynonymFirmCr = new MySqlCommandBuilder(daSynonymFirmCr);
			daSynonymFirmCr.SelectCommand.Parameters.AddWithValue("?FirmCode", LockedSynonym);
			DataTable dtSynonymFirmCr = new DataTable();
			daSynonymFirmCr.Fill(dtSynonymFirmCr);
			dtSynonymFirmCr.Constraints.Add("UnicNameCode", new DataColumn[] {dtSynonymFirmCr.Columns["Synonym"]}, false);

			f.Pr += 1;
			//Заполнили таблицу синонимов валют
			MySqlDataAdapter daSynonymCurrency = new MySqlDataAdapter("select * from SynonymCurrency where FirmCode = ?FirmCode limit 0", MyCn);
			MySqlCommandBuilder cbSynonymCurrency = new MySqlCommandBuilder(daSynonymCurrency);
			daSynonymCurrency.SelectCommand.Parameters.AddWithValue("?FirmCode", LockedSynonym);
			DataTable dtSynonymCurrency = new DataTable();
			daSynonymCurrency.Fill(dtSynonymCurrency);
			dtSynonymCurrency.Constraints.Add("UnicNameCode", new DataColumn[] {dtSynonymCurrency.Columns["Synonym"]}, false);

			f.Pr += 1;
			//Заполнили таблицу запрещённых выражений
			MySqlDataAdapter daForbidden = new MySqlDataAdapter("select * from Forbidden limit 0", MyCn);
			MySqlCommandBuilder cbForbidden = new MySqlCommandBuilder(daForbidden);
			DataTable dtForbidden = new DataTable();
			daForbidden.Fill(dtForbidden);
			dtForbidden.Constraints.Add("UnicNameCode", new DataColumn[] {dtForbidden.Columns["Forbidden"]}, false);

			f.Pr += 1;
			//Заполнили таблицу логов синонимов наименований
			MySqlDataAdapter daSynonymLogs = new MySqlDataAdapter("select * from logs.SynonymLogs limit 0", MyCn);
			MySqlCommandBuilder cbSynonymLogs = new MySqlCommandBuilder(daSynonymLogs);
			DataTable dtSynonymLogs = new DataTable();
			daSynonymLogs.Fill(dtSynonymLogs);

			f.Pr += 1;
			//Заполнили таблицу логов синонимов производителей
			MySqlDataAdapter daSynonymFirmCrLogs = new MySqlDataAdapter("select * from logs.SynonymFirmCrLogs limit 0", MyCn);
			MySqlCommandBuilder cbSynonymFirmCrLogs = new MySqlCommandBuilder(daSynonymFirmCrLogs);
			DataTable dtSynonymFirmCrLogs = new DataTable();
			daSynonymFirmCrLogs.Fill(dtSynonymFirmCrLogs);

			f.Pr += 1;
			//Заполнили таблицу логов синонимов валют
			MySqlDataAdapter daSynonymCurrencyLogs = new MySqlDataAdapter("select * from logs.SynonymCurrencyLogs limit 0", MyCn);
			MySqlCommandBuilder cbSynonymCurrencyLogs = new MySqlCommandBuilder(daSynonymCurrencyLogs);
			DataTable dtSynonymCurrencyLogs = new DataTable();
			daSynonymCurrencyLogs.Fill(dtSynonymCurrencyLogs);

			f.Pr += 1;
			//Заполнили таблицу логов запрещённых выражений
			MySqlDataAdapter daForbiddenLogs = new MySqlDataAdapter("select * from logs.ForbiddenLogs limit 0", MyCn);
			MySqlCommandBuilder cbForbiddenLogs = new MySqlCommandBuilder(daForbiddenLogs);
			DataTable dtForbiddenLogs = new DataTable();
			daForbiddenLogs.Fill(dtForbiddenLogs);

			f.Pr = 10;

			for(int i = 0; i < gvUnrecExp.RowCount; i++)
			{
				if (i != GridControl.InvalidRowHandle)
				{
					DataRow dr = gvUnrecExp.GetDataRow(i);

					DelCount += UpDateUnrecExp(dtUnrecUpdate, dr);
					
					//Вставили новую запись в таблицу запрещённых выражений
					if (!MarkForbidden(i, "UEAlready") && MarkForbidden(i, "UEStatus"))
					{
						DataRow newDR = dtForbidden.NewRow();
								
						newDR["FirmCode"] = LockedSynonym;
						newDR["Forbidden"] = GetFullUnrecName(i);
						try
						{
							dtForbidden.Rows.Add(newDR);
							ForbiddenCount += 1;
						}
						catch(ConstraintException)
						{
						}
					}
					else
					{
						//Вставили новую запись в таблицу синонимов наименований
						if (NotNameForm(i, "UEAlready") && !NotNameForm(i, "UEStatus"))
						{
							DataRow newDR = dtSynonym.NewRow();

							newDR["FirmCode"] = LockedSynonym;
							newDR["Synonym"] = GetFullUnrecName(i);
							newDR["FullCode"] = dr[UETmpFullCode];
							newDR["Junk"] = dr[UEJunk];
							try
							{
								dtSynonym.Rows.Add(newDR);
								SynonymCount += 1;
							}
							catch (ConstraintException)
							{
							}
						}

						//Вставили новую запись в таблицу синонимов производителей
						if (NotFirmForm(i, "UEAlready") && !NotFirmForm(i, "UEStatus"))
						{
							DataRow newDR = dtSynonymFirmCr.NewRow();

							newDR["FirmCode"] = LockedSynonym;
							newDR["CodeFirmCr"] = dr[UETmpCodeFirmCr];
							newDR["Synonym"] = GetFirmCr(i);
							try
							{
								dtSynonymFirmCr.Rows.Add(newDR);
								SynonymFirmCrCount +=1;
							}
							catch(ConstraintException)
							{
							}
						} 

						//Вставили новую запись в таблицу синонимов валют
						if (NotCurrForm(i, "UEAlready") && !NotCurrForm(i, "UEStatus"))
						{
							DataRow newDR = dtSynonymCurrency.NewRow();

							newDR["FirmCode"] = LockedSynonym;
							newDR["Currency"] = dr[UETmpCurrency];
							newDR["Synonym"] = GetCurrency(i);
							try
							{
								dtSynonymCurrency.Rows.Add(newDR);
								SynonymCurrencyCount +=1;
							}
							catch(ConstraintException)
							{
							}
						}
					}
				}
			}

			f.Status = "Применение изменений в базу данных...";
			do
			{
				f.Pr = 30;
				MySqlTransaction tran = null;
				try
				{
					tran = MyCn.BeginTransaction(IsolationLevel.RepeatableRead);

					//Заполнили таблицу логов для синонимов наименований
					daSynonym.SelectCommand.Transaction = tran;
					DataTable dtSynonymCopy = dtSynonym.Copy();
					daSynonym.Update(dtSynonymCopy);

					dtSynonymLogs.Clear();
					foreach(DataRow drSynonymCopy in dtSynonymCopy.Rows)
					{
						DataRow drNewSynonymLogs = dtSynonymLogs.NewRow();
						drNewSynonymLogs["OperatorName"] = Environment.UserName;
						drNewSynonymLogs["OperatorHost"] = Environment.MachineName;
						drNewSynonymLogs["LogTime"] = now;
						drNewSynonymLogs["Operation"] = 0;
						drNewSynonymLogs["PriceCode"] = drSynonymCopy["FirmCode"];
						drNewSynonymLogs["SynonymCode"] = drSynonymCopy["SynonymCode"];
						drNewSynonymLogs["Synonym"] = drSynonymCopy["Synonym"];
						drNewSynonymLogs["FullCode"] = drSynonymCopy["FullCode"];
						drNewSynonymLogs["Junk"] = drSynonymCopy["Junk"];
						dtSynonymLogs.Rows.Add(drNewSynonymLogs);
					}
					daSynonymLogs.SelectCommand.Transaction = tran;

					f.Pr += 10;
                    
					//Заполнили таблицу логов для синонимов производителей
					daSynonymFirmCr.SelectCommand.Transaction = tran;
					DataTable dtSynonymFirmCrCopy = dtSynonymFirmCr.Copy();
					daSynonymFirmCr.Update(dtSynonymFirmCrCopy);

					dtSynonymFirmCrLogs.Clear();
					foreach(DataRow drSynonymFirmCrCopy in dtSynonymFirmCrCopy.Rows)
					{
						DataRow drNewSynonymFirmCrLogs = dtSynonymFirmCrLogs.NewRow();
						drNewSynonymFirmCrLogs["OperatorName"] = Environment.UserName;
						drNewSynonymFirmCrLogs["OperatorHost"] = Environment.MachineName;
						drNewSynonymFirmCrLogs["LogTime"] = now;
						drNewSynonymFirmCrLogs["Operation"] = 0;
						drNewSynonymFirmCrLogs["SynonymFirmCrCode"] = drSynonymFirmCrCopy["SynonymFirmCrCode"];
						drNewSynonymFirmCrLogs["Synonym"] = drSynonymFirmCrCopy["Synonym"];
						drNewSynonymFirmCrLogs["PriceCode"] = drSynonymFirmCrCopy["FirmCode"];
						drNewSynonymFirmCrLogs["CodeFirmCr"] = drSynonymFirmCrCopy["CodeFirmCr"];
						dtSynonymFirmCrLogs.Rows.Add(drNewSynonymFirmCrLogs);
					}
					daSynonymFirmCrLogs.SelectCommand.Transaction = tran;

					f.Pr += 10;

					//Заполнили таблицу логов для синонимов валют
					daSynonymCurrency.SelectCommand.Transaction = tran;
					DataTable dtSynonymCurrencyCopy = dtSynonymCurrency.Copy();
					daSynonymCurrency.Update(dtSynonymCurrencyCopy);

					dtSynonymCurrencyLogs.Clear();
					foreach(DataRow drSynonymCurrencyCopy in dtSynonymCurrencyCopy.Rows)
					{
						DataRow drNewSynonymCurrencyLogs = dtSynonymCurrencyLogs.NewRow();
						drNewSynonymCurrencyLogs["OperatorName"] = Environment.UserName;
						drNewSynonymCurrencyLogs["OperatorHost"] = Environment.MachineName;
						drNewSynonymCurrencyLogs["LogTime"] = now;
						drNewSynonymCurrencyLogs["Operation"] = 0;
						drNewSynonymCurrencyLogs["Currency"] = drSynonymCurrencyCopy["Currency"];
						drNewSynonymCurrencyLogs["Synonym"] = drSynonymCurrencyCopy["Synonym"];
						drNewSynonymCurrencyLogs["PriceCode"] = drSynonymCurrencyCopy["FirmCode"];
						dtSynonymCurrencyLogs.Rows.Add(drNewSynonymCurrencyLogs);
					}
					daSynonymCurrencyLogs.SelectCommand.Transaction = tran;

					f.Pr += 10;
					
					//Заполнили таблицу логов для запрещённых выражений
					daForbidden.SelectCommand.Transaction = tran;
					DataTable dtForbiddenCopy = dtForbidden.Copy();
					daForbidden.Update(dtForbiddenCopy);

					dtForbiddenLogs.Clear();
					foreach(DataRow drForbiddenCopy in dtForbiddenCopy.Rows)
					{
						DataRow drNewForbiddenLogs = dtForbiddenLogs.NewRow();
						drNewForbiddenLogs["OperatorName"] = Environment.UserName;
						drNewForbiddenLogs["OperatorHost"] = Environment.MachineName;
						drNewForbiddenLogs["LogTime"] = now;
						drNewForbiddenLogs["Operation"] = 0;
						drNewForbiddenLogs["Forbidden"] = drForbiddenCopy["Forbidden"];
						drNewForbiddenLogs["PriceCode"] = drForbiddenCopy["FirmCode"];
						dtForbiddenLogs.Rows.Add(drNewForbiddenLogs);
					}
					daForbiddenLogs.SelectCommand.Transaction = tran;

					//Применяем логи отдельно, т.к. требуется сменить базу данных
					try
					{
						MyCn.ChangeDatabase("logs");
						daSynonymLogs.Update(dtSynonymLogs);
						daSynonymFirmCrLogs.Update(dtSynonymFirmCrLogs);
						daSynonymCurrencyLogs.Update(dtSynonymCurrencyLogs);
						daForbiddenLogs.Update(dtForbiddenLogs);
					}
					finally
					{
						MyCn.ChangeDatabase("farm");
					}

					f.Pr += 10;
                   
					//Обновление таблицы нераспознанных выражений
					daUnrecUpdate.SelectCommand.Transaction = tran;
					DataTable dtUnrecUpdateCopy = dtUnrecUpdate.Copy();
					daUnrecUpdate.Update(dtUnrecUpdateCopy);

					if (HasParentSynonym)
					{
						foreach (RetransedPrice rp in RetransedPriceList)
							MySqlHelper.ExecuteNonQuery(MyCn, @"
delete
from
  farm.UnrecExp
where
  FirmCode = ?DeletePriceCode
and not Exists(select * from farm.blockedprice bp where bp.PriceCode = ?DeletePriceCode and bp.BlockBy <> ?LockUserName)",
										new MySqlParameter("DeletePriceCode", rp.PriceCode),
										new MySqlParameter("LockUserName", Environment.UserName));
					}

					//DelCount = UpDateUnrecExp(tran);

					tran.Commit();
					res = true;

					f.Pr +=10;
				}
				catch(MySqlException ex)
				{
					try{ 
						tran.Rollback(); } 
					catch{}
					f.Error = String.Format("При обновлении синонимов произошла ошибка : {0}\r\n", ex);
					f.Pr = 50;
					Thread.Sleep(500);
				}
				finally
				{
					if (tran != null && !res)
						try{ 
							tran.Rollback(); } 
						catch{}
				}
			}
			while(!res);
			
			f.Pr = 80;

			f.Status = String.Empty;
			f.Error = String.Empty;

			bool S = DelCount == dtUnrecExp.Rows.Count;
			if (!S)
				S = (bool)f.Invoke( new ShowRetransPriceDelegate( ShowRetransPrice ) );

			if (res &&  S)
			{

#if DEBUG
				string rootpath = @"C:\Temp\";
#else
				string rootpath = @"\\fms\Prices\";
#endif

				f.Status = "Перепроведение пpайса...";
				f.Pr = 80;

				int CurrentPriceCode = 0;
				string CurrentFileName;
				do
				{
					CurrentFileName = RetransedPriceList[CurrentPriceCode].PriceCode.ToString() + RetransedPriceList[CurrentPriceCode].FileExt;
					try
					{
						if (File.Exists(rootpath + "Base\\" + CurrentFileName))
						{
							if (!File.Exists(rootpath + "Inbound0\\" + CurrentFileName))
							{
								File.Copy(rootpath + "Base\\" + CurrentFileName, rootpath + "Inbound0\\" + CurrentFileName);
								PricesRetrans(now, RetransedPriceList[CurrentPriceCode].PriceCode);
							}
						}
						RetransedPriceList.RemoveAt(CurrentPriceCode);
					}
					catch (Exception e)
					{
						if (f != null)
							f.Error = String.Format("При копировании файла {1} возникла ошибка : {0}\r\n", e, CurrentFileName);
						CurrentPriceCode++;
						Thread.Sleep(500);
					}
					if (CurrentPriceCode >= RetransedPriceList.Count)
						CurrentPriceCode = 0;
				}
				while(RetransedPriceList.Count > 0);

			}

			f.Pr = 100;
		}

		private void PricesRetrans(DateTime now, long RetransPriceCode)
		{
			MySqlCommand mcInsert = new MySqlCommand();
			mcInsert.Connection = MyCn;
			mcInsert.Parameters.Clear();
			mcInsert.Parameters.AddWithValue("?RetransPriceCode", RetransPriceCode);
			mcInsert.Parameters.AddWithValue("?UserName", Environment.UserName);
			mcInsert.Parameters.AddWithValue("?UserHost", Environment.MachineName);
			mcInsert.Parameters.AddWithValue("?Now", now);

			mcInsert.CommandText = 
					@"insert into logs.pricesretrans 
						(LogTime, 
						OperatorName,
						OperatorHost,
						PriceCode) 
					values 
						(?Now,
						?UserName,
						?UserHost,
						?RetransPriceCode)";
	
			mcInsert.ExecuteNonQuery();
		}

		private int UpDateUnrecExp(DataTable dtUnrecExpUpdate, DataRow drUpdated)
		{
			int DelCount = 0;

			int FULLFORM = (int)(FormMask.NameForm | FormMask.FirmForm | FormMask.CurrForm);

			//Производим проверку того, что синоним может быть сопоставлен со скрытым каталожным наименованием
			bool HidedSynonym = Convert.ToBoolean(MySqlHelper.ExecuteScalar(MyCn, "select Hide from farm.catalog where FullCode = " + drUpdated[UETmpFullCode].ToString()));
			if (HidedSynonym)
			{
				//Если в процессе распознования каталожное наименование скрыли, то сбрасываем распознавание
				drUpdated["UETmpFullCode"] = 0;
				drUpdated["UEStatus"] = (int)((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & (~FormMask.NameForm));
				HideSynonymCount++;
			}

			//Производим проверку того, что синоним может быть уже вставлен в таблицу синонимов
			object SynonymExists = MySqlHelper.ExecuteScalar(MyCn, 
				"select FullCode from farm.synonym where synonym = ?Synonym and FirmCode=" + LockedSynonym.ToString(), 
				new MySqlParameter("?Synonym", String.Format("{0} {1} {2}", drUpdated["UEName1"], drUpdated["UEName2"], drUpdated["UEName3"])));
			if ((SynonymExists != null))
			{
				//Если в процессе распознования синоним уже кто-то добавил, то сбрасываем распознавание
				drUpdated["UETmpFullCode"] = 0;
				drUpdated["UEStatus"] = (int)((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & (~FormMask.NameForm));
				DuplicateSynonymCount++;
			}

			DataRow drNew = dtUnrecExpUpdate.Rows.Find( Convert.ToUInt32( drUpdated["UERowID"] ) );

			if (drNew != null)
			{

				if ((int)drUpdated["UEStatus"] == FULLFORM)
				{
					drNew.Delete();
					DelCount++;
				}
				else
				{
					drNew["Status"] = drUpdated["UEStatus"];
					drNew["TmpFullCode"] = drUpdated["UETmpFullCode"];
					drNew["TmpCodeFirmCr"] = drUpdated["UETmpCodeFirmCr"];
					drNew["TmpCurrency"] = drUpdated["UETmpCurrency"];
					drNew["RowID"] = drUpdated["UERowID"];
					if ((byte)drUpdated["UEHandMade"] == 0)
					{
						int r = (int)drUpdated["UEStatus"] ^ (int)drUpdated["UEAlready"];
						if ( (r > 0 && (r & (int)FormMask.MarkForb) == 0))
						{
							drNew["HandMade"] = 1;
						}
						else
						{
							drNew["HandMade"] = 0;
						}
					}
					else
						drNew["HandMade"] = 1;
				}

			}

			return DelCount;
		}

		private void frmUEEMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
					//Если нажали F12 на вкладке нераспознанные выражения, то завершаем распознование
				case Keys.F12:
					if (tcMain.SelectedTab == tpUnrecExp)
					{
						DialogResult DRes;
						DRes = MessageBox.Show("Сохранить результаты?" , "Вопрос", MessageBoxButtons.YesNoCancel);
						UnlockJob(DRes);
					}
					break;

				case Keys.F11:
					UpdateCatalog();
					break;
			}
		}

		private void LocateJobs(long JCode)
		{
			if (JCode != -1)			
			{
				for (int i = 0; i < gvJobs.RowCount; i++)
				{
					DataRow dr = gvJobs.GetDataRow(i);
					if (dr[JPriceCode.ColumnName].ToString() == JCode.ToString())
					{
						gvJobs.FocusedRowHandle = i;
						return;
					}
				}
			}
		}

		private void LockedInBlockedPrice(long LockPriceCode, string BlockBy)
		{
			MySqlCommand mcInsert = new MySqlCommand("select * from blockedprice where PriceCode = ?LockPriceCode", MyCn);
			mcInsert.Parameters.Clear();
			mcInsert.Parameters.AddWithValue("?LockPriceCode", LockedPriceCode);
			MySqlDataReader drInsert = mcInsert.ExecuteReader();
			bool NotExist = !drInsert.Read();
			drInsert.Close();
			drInsert = null;
			if (NotExist)
			{
				mcInsert.CommandText = @"insert into blockedprice (PriceCode, BlockBy) values (?LockPriceCode, ?BlockBy)";
				mcInsert.Parameters.AddWithValue("?BlockBy", BlockBy);
				mcInsert.ExecuteNonQuery();
			}
		}

		private void UnLockedInBlockedPrice(long LockPriceCode)
		{
			MySqlCommand mcInsert = new MySqlCommand("delete from blockedprice where PriceCode = ?LockPriceCode", MyCn);
			mcInsert.Parameters.Clear();
			mcInsert.Parameters.AddWithValue("?LockPriceCode", LockedPriceCode);
			mcInsert.ExecuteNonQuery();
		}

		private void ChangeBigName(int FocusedRowHandle)
		{
			if (FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				if (NotNameForm(FocusedRowHandle, "UEStatus"))
					BigNameLabel2.Text = GetFullUnrecName(FocusedRowHandle);
				else
					if (NotFirmForm(FocusedRowHandle, "UEStatus"))
					BigNameLabel2.Text = GetFirmCr(FocusedRowHandle);
				else
					BigNameLabel2.Text = GetFullUnrecName(FocusedRowHandle);

				sbpCurrent.Text = String.Format("Текущая позиция: {0}", FocusedRowHandle+1);
			}
			else
				sbpCurrent.Text = String.Empty;
		}

		private void gvUnrecExp_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
		{
			ChangeBigName(e.FocusedRowHandle);
		}

		private string GetFullUnrecName(int FocusedRowHandle)
		{
			if (FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvUnrecExp.GetDataRow(FocusedRowHandle);
				if (dr != null)
					return String.Format("{0} {1} {2}", dr["UEName1"], dr["UEName2"], dr["UEName3"]);
				else
					return String.Empty;
			}
			else
				return String.Empty;
		}

		private void gvUnrecExp_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
		{
			if (e.Column.FieldName == "UEAlready")
			{
				int v1 = (int)e.Value1;
				int v2 = (int)e.Value2;
				e.Handled = true;
				if (v1 == v2)
					e.Result = 0;
				else
					if (v1 == 6)
				{
					if (v2 == 0)
						e.Result = 1;
					else
						e.Result = -1;
				}
				else
					if (v2 == 6)
				{
					if (v1 == 0)
						e.Result = -1;
					else
						e.Result = 1;
				}
				else
					e.Result = v1-v2;
			}
		}

		private void gvJobs_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
		{
		    if (e.Column == colJWholeSale)
		    {
			    if (e.Value.ToString() == "0")
				    e.DisplayText = "Опт";
			    else
				    e.DisplayText = "Розница";
		    }
            if ((e.Column == colJNeedRetrans)||(e.Column == colJRetranced))
            {
                if (e.Value.ToString() == "1")
                    e.DisplayText = "Да";
                else
                    e.DisplayText = "Нет";
            }
		}

		private void UnrecExpGridControl_Click(object sender, System.EventArgs e)
		{
			if (gvUnrecExp.FocusedRowHandle != GridControl.InvalidRowHandle)
			{
				CatalogGridControl.Enabled = false;
				ClearCatalogGrid();
			}
		}

		private void gvJobs_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				DataRow dr = gvJobs.GetDataRow(e.RowHandle);
				if ((dr["JBlockBy"].ToString() != ""))
				{
					Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Gray, Color.Gray, 90);
					Rectangle  r = e.Bounds;
					r.Inflate(-1, -1);
					//e.Graphics.FillRectangle(brush, r);

					int x = r.X + (r.Width - e.Info.Images.ImageSize.Width) / 2;
					int y = r.Y + (r.Height - e.Info.Images.ImageSize.Height) / 2;
					e.Graphics.DrawImageUnscaled(imageList1.Images[0], r.X, r.Y);
					ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
					e.Handled = true;
				}
			}		
		}

		private string ConvertName(string tmp)
		{
			string res = String.Empty;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i=0; i<tmp.Length; i++)
			{
				if (Char.IsLetterOrDigit(tmp[i]) )
				{
					sb.Append(tmp[i]);
				}
				else
				{
					sb.Append("%");
					sb.Append( Convert.ToInt32(tmp[i]).ToString("X2") );
				}
			}
			return sb.ToString();
		}

		private void miSendAboutNames_Click(object sender, System.EventArgs e)
		{	
			DataRow[] drs = dtJobs.Select("JPriceCode = " + LockedPriceCode.ToString());

			if (drs.Length > 0)
			{
				DataRow dr = drs[0];

				ArrayList NameArray = new ArrayList();

				foreach(DataRow UEdr in dtUnrecExp.Rows)
				{
					if ( ((FormMask)Convert.ToByte(UEdr["UEStatus"]) & FormMask.MarkForb) == FormMask.MarkForb )
					{
	
						string tmp = (UEdr["UEName1"].ToString() + " " + UEdr["UEFirmCr"].ToString()).Trim();
						if (!NameArray.Contains(tmp))
						{
							//ConvertName(tmp);
							NameArray.Add( tmp );
						}
					}
				}

				string UnrecName = String.Join("\r\n", (string[])NameArray.ToArray(typeof(string)));

				Clipboard.SetDataObject(UnrecName);

				string body = "";
                body = UEEditor.Properties.Settings.Default.AboutNamesBody;

				body = String.Format(body, dr["JName"], "");

				System.Diagnostics.Process.Start(String.Format("mailto:{0}?cc={1}&Subject={2}&Body={3}", GetContactText((long)dr[JFirmCode.ColumnName], 2, 0), "farm@analit.net", String.Format(UEEditor.Properties.Settings.Default.AboutNamesSubject, dr["JName"]), body));
			}
		}

		private void miSendAboutFirmCr_Click(object sender, System.EventArgs e)
		{
			DataRow[] drs = dtJobs.Select("JPriceCode = " + LockedPriceCode.ToString());

			if (drs.Length > 0)
			{
				DataRow dr = drs[0];

				ArrayList FirmCrArray = new ArrayList();

				foreach(DataRow UEdr in dtUnrecExp.Rows)
				{
					if ( ((FormMask)Convert.ToByte(UEdr["UEStatus"]) & FormMask.FirmForm) != FormMask.FirmForm )
					{
						string tmp = UEdr["UEFirmCr"].ToString().Trim();
						if (!FirmCrArray.Contains(tmp))
						{
							FirmCrArray.Add(tmp);
						}
					}
				}

				string UnrecFirmCr = String.Join("\r\n", (string[])FirmCrArray.ToArray(typeof(string)));

				Clipboard.SetDataObject(UnrecFirmCr);

				string body = "";
                body = UEEditor.Properties.Settings.Default.AboutFirmBody;
			
				body = String.Format(body, dr["JName"], "");

				System.Diagnostics.Process.Start(String.Format("mailto:{0}?cc={1}&Subject={2}&Body={3}", GetContactText((long)dr[JFirmCode.ColumnName], 2, 0), "farm@analit.net", String.Format(UEEditor.Properties.Settings.Default.AboutFirmSubject, dr["JName"]), body));
			}
		}

		/// <summary>
		/// Получить текст контактов из базы
		/// </summary>
		/// <param name="FirmCode">Код поставщика</param>
		/// <param name="ContactGroupType">Тип контактной группы: 0 - General, 1 - ClientManager, 2 - OrderManager, 3 - Accountant</param>
		/// <param name="ContactType">Тип контакта: 0 - Email, 1 - Phone</param>
		/// <returns>Текст контактов, разделенный ";"</returns>
		private string GetContactText(long FirmCode, byte ContactGroupType, byte ContactType)
		{
			DataSet dsContacts = MySqlHelper.ExecuteDataset(MyCn, @"
select distinct c.contactText
from usersettings.clientsdata cd
  join contacts.contact_groups cg on cd.ContactGroupOwnerId = cg.ContactGroupOwnerId
    join contacts.contacts c on cg.Id = c.ContactOwnerId
where
    firmcode = ?FirmCode
and cg.Type = ?ContactGroupType
and c.Type = ?ContactType

union

select distinct c.contactText
from usersettings.clientsdata cd
  join contacts.contact_groups cg on cd.ContactGroupOwnerId = cg.ContactGroupOwnerId
    join contacts.persons p on cg.id = p.ContactGroupId
      join contacts.contacts c on p.Id = c.ContactOwnerId
where
    firmcode = ?FirmCode
and cg.Type = ?ContactGroupType
and c.Type = ?ContactType;",
				new MySqlParameter("?FirmCode", FirmCode),
				new MySqlParameter("?ContactGroupType", ContactGroupType),
				new MySqlParameter("?ContactType", ContactType));
			List<string> contacts = new List<string>();
			foreach (DataRow drContact in dsContacts.Tables[0].Rows)
			{
				if (!contacts.Contains(drContact["contactText"].ToString()))
					contacts.Add(drContact["contactText"].ToString());
			}

			return String.Join(";", contacts.ToArray());
		}

		private void gvUnrecExp_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				if (e.Column == colUEColumn1)
				{
					if (((GetMask(e.RowHandle, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb))
					{
						Rectangle  r = e.Bounds;
						r.Inflate(-1, -1);
						System.Drawing.Brush br = new System.Drawing.SolidBrush(System.Drawing.SystemColors.Control);
						e.Graphics.FillRectangle(br, r);
						e.Graphics.DrawImageUnscaled(imageList2.Images[3], r.X, r.Y);
						ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.Adjust);
						e.Handled = true;
					}
					else
					if (((GetMask(e.RowHandle, "UEStatus") & FormMask.NameForm) == FormMask.NameForm))
					{
						Rectangle  r = e.Bounds;
						r.Inflate(-1, -1);
						System.Drawing.Brush br = new System.Drawing.SolidBrush(System.Drawing.SystemColors.Control);
						e.Graphics.FillRectangle(br, r);
						e.Graphics.DrawImageUnscaled(imageList2.Images[0], r.X, r.Y);
						ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.Adjust);
						e.Handled = true;
					}
				}

				if (e.Column == colUEColumn2)
				{
					if (((GetMask(e.RowHandle, "UEStatus") & FormMask.FirmForm) == FormMask.FirmForm) && ((GetMask(e.RowHandle, "UEStatus") & FormMask.MarkForb) != FormMask.MarkForb))
					{
						Rectangle  r = e.Bounds;
						r.Inflate(-1, -1);
						System.Drawing.Brush br = new System.Drawing.SolidBrush(System.Drawing.SystemColors.Control);
						e.Graphics.FillRectangle(br, r);
						e.Graphics.DrawImageUnscaled(imageList2.Images[1], r.X, r.Y);
						ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.Adjust);
						e.Handled = true;
					}
				}

				if (e.Column == colUEColumn3)
				{
					if (((GetMask(e.RowHandle, "UEStatus") & FormMask.CurrForm) == FormMask.CurrForm) && ((GetMask(e.RowHandle, "UEStatus") & FormMask.MarkForb) != FormMask.MarkForb))
					{
						Rectangle  r = e.Bounds;
						r.Inflate(-1, -1);
						System.Drawing.Brush br = new System.Drawing.SolidBrush(System.Drawing.SystemColors.Control);
						e.Graphics.FillRectangle(br, r);
						e.Graphics.DrawImageUnscaled(imageList2.Images[2], r.X, r.Y);
						ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.Adjust);
						e.Handled = true;
					}
				}
			}		
		}

		private void gvUnrecExp_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			if (e.RowHandle != GridControl.InvalidRowHandle)
			{
				int i = e.RowHandle;

				DataRow UEdr = gvUnrecExp.GetDataRow(i);

				if (UEdr != null)
				{
					if (e.Column.VisibleIndex ==0)
						e.Appearance.BackColor = Color.White;
					else
						if (e.Column.VisibleIndex ==1)
						e.Appearance.BackColor = Color.White;

					else
						if (e.Column.VisibleIndex ==2)
						e.Appearance.BackColor = Color.White;
					else
					{
						if (7 == (int)UEdr["UEStatus"])
							e.Appearance.BackColor = Color.Lime;
						else
							if (((GetMask(i, "UEStatus") & FormMask.MarkForb) == FormMask.MarkForb))
							e.Appearance.BackColor = SystemColors.GrayText;
						else
							if (((GetMask(i, "UEStatus") & FormMask.NameForm) == FormMask.NameForm))
							e.Appearance.BackColor = Color.PaleGreen;
					}
				}
			}
		}

		private void lbColorChange(object sender, System.EventArgs e)
		{
			cdLegend.Color = ((Button)sender).BackColor;
			if (cdLegend.ShowDialog((Button)sender) == DialogResult.OK)
			{
				((Button)sender).BackColor = cdLegend.Color;
				gvJobs.RefreshData();
			}
		}

		private void LoadColor(Button lColor, int defaultArgb)
		{
			using(RegistryKey k = Registry.CurrentUser.OpenSubKey(BaseRegKey, true))
			{
				int argbColor;
				if (k != null)
					argbColor = (int)k.GetValue(lColor.Name, defaultArgb);
				else
					argbColor = defaultArgb;
				lColor.BackColor = Color.FromArgb(argbColor);
			}
		}
				
		private void SaveColor(Button lColor)
		{
			using(RegistryKey k = Registry.CurrentUser.CreateSubKey(BaseRegKey))
			{
				if (k != null)
					k.SetValue(lColor.Name, lColor.BackColor.ToArgb());
			}
		}

		private void gvUnrecExp_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
		{
			if (e.Column == colUEJunk)
				e.DisplayText = ((byte)e.Value == 1) ? "Да" : "Нет";
		}

	}

	public class RetransedPrice
	{
		public long PriceCode;
		public string FileExt;

		public RetransedPrice(long APriceCode, string AFileExt)
		{
			this.PriceCode = APriceCode;
			this.FileExt = AFileExt;
		}
	}

}